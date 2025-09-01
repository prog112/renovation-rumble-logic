namespace RenovationRumble.Logic.Generators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Scans for base classes annotated with [JsonDiscriminatedUnion()] and generates an appropriate converter.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public sealed class DiscriminatedUnionGenerator : ISourceGenerator
    {
        private const string AttributeFullName = "RenovationRumble.Logic.Serialization.JsonDiscriminatedUnionAttribute";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new Receiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not Receiver receiver)
                return;

            var compilation = context.Compilation;
            var attributeSymbol = compilation.GetTypeByMetadataName(AttributeFullName);
            if (attributeSymbol is null)
                return;

            // Collect bases (annotated) and all sealed class candidates for subclass discovery
            var bases = new List<(INamedTypeSymbol baseType, INamedTypeSymbol enumType, string enumProperty, string discriminator)>();
            var sealedTypes = new List<INamedTypeSymbol>();

            foreach (var cls in receiver.Candidates)
            {
                var model = compilation.GetSemanticModel(cls.SyntaxTree);
                if (model.GetDeclaredSymbol(cls) is not INamedTypeSymbol namedType)
                    continue;

                if (namedType.IsSealed)
                {
                    sealedTypes.Add(namedType);
                    continue;
                }

                // Base candidates: any class with the attribute
                var attribute = namedType.GetAttributes().FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeSymbol));
                if (attribute is null)
                    continue;

                if (attribute.ConstructorArguments.Length < 2)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.InvalidAttributeUsage, Location.None, namedType.ToDisplayString()));
                    continue;
                }

                var enumArg = attribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                var enumPropName = attribute.ConstructorArguments[1].Value as string ?? "Kind";
                var discriminator = (attribute.ConstructorArguments.Length >= 3 ? attribute.ConstructorArguments[2].Value as string : null) ?? "type";

                if (enumArg is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.InvalidAttributeUsage, Location.None, namedType.ToDisplayString()));
                    continue;
                }

                bases.Add((namedType, enumArg, enumPropName, discriminator));
            }

            // For each base class gather subclasses and emit the helper
            var registryCalls = new List<string>();
            foreach (var (baseType, enumType, enumProperty, discriminator) in bases)
            {
                var pairs = new List<(string enumName, string typeName)>();
                var diagnostics = new List<Diagnostic>();

                foreach (var subType in sealedTypes)
                {
                    if (subType.IsAbstract)
                        continue;
                    
                    if (!InheritsFrom(subType, baseType))
                        continue;

                    var (isOk, enumName, diag) = TryGetEnumMemberName(subType, enumType, enumProperty);
                    if (!isOk)
                    {
                        if (diag is not null)
                            diagnostics.Add(diag);

                        continue;
                    }

                    var typeName = subType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    pairs.Add((enumName, typeName));
                }

                foreach (var duplicate in pairs.GroupBy(p => p.enumName).Where(g => g.Count() > 1))
                {
                    var message = string.Join(", ", duplicate.Select(x => x.typeName));
                    diagnostics.Add(Diagnostic.Create(Diagnostics.DuplicateEnumValue, Location.None, duplicate.Key, message));
                }

                foreach (var diagnostic in diagnostics)
                    context.ReportDiagnostic(diagnostic);

                var source = EmitPerBaseHelper(baseType, enumType, enumProperty, discriminator, pairs.OrderBy(p => p.enumName));
                var hintName = $"{baseType.Name}Union.g.cs";
                context.AddSource(hintName, source);

                registryCalls.Add($"{baseType.ContainingNamespace.ToDisplayString()}.Serialization.{baseType.Name}Union.Register");
            }

            // Emit the central registry once
            var registrySource = EmitRegistry(registryCalls);
            context.AddSource("DiscriminatedUnionRegistry.g.cs", registrySource);
        }

        private static bool InheritsFrom(INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol baseType)
        {
            for (var current = namedTypeSymbol.BaseType; current is not null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                    return true;
            }

            return false;
        }

        private static (bool isOk, string enumName, Diagnostic diag) TryGetEnumMemberName(INamedTypeSymbol type, INamedTypeSymbol enumType,
            string enumPropertyName)
        {
            // Look for: public override <EnumType> <enumPropertyName> => <EnumType>.<Member>;
            var property = type.GetMembers().OfType<IPropertySymbol>()
                .FirstOrDefault(p =>
                    p.Name == enumPropertyName &&
                    p.IsOverride &&
                    p.SetMethod is null &&
                    p.GetMethod is not null &&
                    SymbolEqualityComparer.Default.Equals(p.Type, enumType));

            if (property is null)
            {
                return (false, null, Diagnostic.Create(Diagnostics.MissingOverride, Location.None, type.ToDisplayString(), enumPropertyName));
            }

            var node = property.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

            // Expression-bodied property:  public override Enum Prop => Enum.Member;
            if (node is PropertyDeclarationSyntax { ExpressionBody.Expression: MemberAccessExpressionSyntax propertySyntax })
                return (true, propertySyntax.Name.Identifier.Text, null);

            // Accessor with expression body: get => Enum.Member;
            if (node is AccessorDeclarationSyntax { ExpressionBody.Expression: MemberAccessExpressionSyntax accessorSyntax })
                return (true, accessorSyntax.Name.Identifier.Text, null);

            return (false, null, Diagnostic.Create(Diagnostics.UnsupportedGetter, Location.None, type.ToDisplayString(), enumPropertyName));
        }

        private static string EmitPerBaseHelper(INamedTypeSymbol baseType, INamedTypeSymbol enumType, string enumPropertyName, string discriminator,
            IEnumerable<(string enumName, string fqTypeName)> pairs)
        {
            var namespaceString = baseType.ContainingNamespace.IsGlobalNamespace
                ? "RenovationRumble.Logic.Serialization"
                : $"{baseType.ContainingNamespace.ToDisplayString()}.Serialization";

            var baseName = baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var enumNameFull = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var className = $"{baseType.Name}Union";

            var sb = new StringBuilder();
            sb.AppendLine("// This file has been autogenerated. Please do not edit manually.");
            sb.AppendLine($"namespace {namespaceString}");
            sb.AppendLine("{");
            sb.AppendLine("    using System;");
            sb.AppendLine("    using System.Collections.Generic;");
            sb.AppendLine("    using Newtonsoft.Json;");
            sb.AppendLine($"    using {baseType.ContainingNamespace.ToDisplayString()};");
            sb.AppendLine();
            sb.AppendLine($"    public static partial class {className}");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static readonly Dictionary<{enumNameFull}, Func<{baseName}>> Factories =");
            sb.AppendLine($"            new Dictionary<{enumNameFull}, Func<{baseName}>>");
            sb.AppendLine("            {");
       
            foreach (var (name, type) in pairs)
                sb.AppendLine($"                [{enumNameFull}.{name}] = () => new {type}(),");
    
            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine($"        public static readonly JsonConverter<{baseName}> Converter =");
            sb.AppendLine($"            new RenovationRumble.Logic.Serialization.EnumDiscriminatedConverter<{baseName}, {enumNameFull}>(");
            sb.AppendLine($"                discriminatorName: \"{discriminator}\",");
            sb.AppendLine("                factories: Factories,");
            sb.AppendLine($"                getType: x => x.{enumPropertyName});");
            sb.AppendLine();
            sb.AppendLine("        public static void Register(JsonSerializerSettings settings)");
            sb.AppendLine("        {");
            sb.AppendLine("            settings.Converters.Add(Converter);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private static string EmitRegistry(IEnumerable<string> registerCalls)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// This file has been autogenerated. Please do not edit manually.");
            sb.AppendLine("namespace RenovationRumble.Logic.Serialization");
            sb.AppendLine("{");
            sb.AppendLine("    using System;");
            sb.AppendLine("    using System.Collections.Generic;");
            sb.AppendLine("    using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine("    public static partial class DiscriminatedUnionRegistry");
            sb.AppendLine("    {");
            sb.AppendLine("        private static readonly List<Action<JsonSerializerSettings>> _registrars =");
            sb.AppendLine("            new List<Action<JsonSerializerSettings>>");
            sb.AppendLine("            {");
         
            foreach (var call in registerCalls.Distinct())
                sb.AppendLine($"                s => {call}(s),");
       
            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine("        public static void RegisterAll(JsonSerializerSettings settings)");
            sb.AppendLine("        {");
            sb.AppendLine("            for (var i = 0; i < _registrars.Count; i++)");
            sb.AppendLine("                _registrars[i](settings);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private static class Diagnostics
        {
            public static readonly DiagnosticDescriptor InvalidAttributeUsage = new(
                id: "RRGEN100",
                title: "Invalid JsonDiscriminatedUnion usage",
                messageFormat: "Type '{0}' has invalid JsonDiscriminatedUnion arguments.",
                category: "Generation",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            public static readonly DiagnosticDescriptor MissingOverride = new(
                id: "RRGEN101",
                title: "Subclass must override enum discriminator property",
                messageFormat: "Type '{0}' must override the enum property specified on the base.",
                category: "Generation",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            public static readonly DiagnosticDescriptor UnsupportedGetter = new(
                id: "RRGEN102",
                title: "Unsupported enum property getter",
                messageFormat: "Type '{0}' has an enum getter the generator cannot evaluate. Use '=> Enum.Member'.",
                category: "Generation",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            public static readonly DiagnosticDescriptor DuplicateEnumValue = new(
                id: "RRGEN103",
                title: "Duplicate enum discriminator value",
                messageFormat: "Enum member '{0}' is assigned to multiple subclasses: {1}",
                category: "Generation",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);
        }

        private sealed class Receiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> Candidates { get; } = [];

            public void OnVisitSyntaxNode(SyntaxNode node)
            {
                if (node is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    Candidates.Add(classDeclarationSyntax);
                }
            }
        }
    }
}