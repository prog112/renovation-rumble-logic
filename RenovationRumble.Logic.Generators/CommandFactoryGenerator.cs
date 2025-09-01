namespace RenovationRumble.Logic.Generators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Generator(LanguageNames.CSharp)]
    public sealed class CommandFactoryGenerator : ISourceGenerator
    {
        private const string BaseTypeMetadata = "RenovationRumble.Logic.Data.Commands.CommandDataModel";
        private const string EnumMetadata = "RenovationRumble.Logic.Data.Commands.Command";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new Receiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not Receiver rx) 
                return;

            var compilation = context.Compilation;
            var baseType = compilation.GetTypeByMetadataName(BaseTypeMetadata);
            var enumType = compilation.GetTypeByMetadataName(EnumMetadata);
            
            if (baseType is null || enumType is null) 
                return;

            var pairs = new List<(string enumName, string fqTypeName)>();
            var diagnostics = new List<Diagnostic>();

            foreach (var cls in rx.Candidates)
            {
                var model = compilation.GetSemanticModel(cls.SyntaxTree);
                if (model.GetDeclaredSymbol(cls) is not INamedTypeSymbol namedType) 
                    continue;
                
                if (namedType.IsAbstract)
                    continue;
               
                if (!InheritsFrom(namedType, baseType)) 
                    continue;

                var (isOk, enumName, diag) = TryGetCommandEnumName(namedType);
                if (!isOk)
                {
                    if (diag is not null)
                        diagnostics.Add(diag);
                    
                    continue;
                }

                var fullyQualified = namedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                pairs.Add((enumName!, fullyQualified));
            }

            foreach (var duplicate in pairs.GroupBy(p => p.enumName).Where(g => g.Count() > 1))
            {
                var message = $"Duplicate Command value '{duplicate.Key}' among: {string.Join(", ", duplicate.Select(p => p.fqTypeName))}";
                diagnostics.Add(Diagnostic.Create(Diagnostics.DuplicateCommandValue, Location.None, duplicate.Key, message));
            }

            foreach (var diagnostic in diagnostics)
                context.ReportDiagnostic(diagnostic);

            var source = EmitFactories(pairs.OrderBy(p => p.enumName));
            context.AddSource("CommandFactory.g.cs", source);
        }

        private static bool InheritsFrom(INamedTypeSymbol t, INamedTypeSymbol baseType)
        {
            for (var current = t.BaseType; current is not null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType)) 
                    return true;
            }
            
            return false;
        }

        private static (bool isOk, string enumName, Diagnostic diag) TryGetCommandEnumName(INamedTypeSymbol type)
        {
            // Look for: public override Command Command => Command.XXXXX;
            var typeProperty = type.GetMembers().OfType<IPropertySymbol>()
                .FirstOrDefault(p =>
                    p.Name == "Command" &&
                    p.IsOverride &&
                    p.SetMethod is null && // getter-only
                    p.GetMethod is not null &&
                    p.Type.Name == "Command");

            if (typeProperty is null)
            {
                return (false, null, Diagnostic.Create(
                    Diagnostics.MissingOverride, Location.None, type.ToDisplayString()));
            }

            var node = typeProperty.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

            // Expression-bodied property:  public override Command Command => Command.XXXX;
            if (node is PropertyDeclarationSyntax { ExpressionBody.Expression: MemberAccessExpressionSyntax propertySyntax })
                return (true, propertySyntax.Name.Identifier.Text, null);

            // Accessor with expression body: get => Command.XXXX;
            if (node is AccessorDeclarationSyntax { ExpressionBody.Expression: MemberAccessExpressionSyntax accessorSyntax })
                return (true, accessorSyntax.Name.Identifier.Text, null);

            return (false, null, Diagnostic.Create(Diagnostics.UnsupportedGetter, Location.None, type.ToDisplayString()));
        }

        private static string EmitFactories(IEnumerable<(string enumName, string fqTypeName)> pairs)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// This file has been autogenerated. Please do not edit manually.");
            sb.AppendLine("namespace RenovationRumble.Logic.Serialization");
            sb.AppendLine("{");
            sb.AppendLine("    using System;");
            sb.AppendLine("    using System.Collections.Generic;");
            sb.AppendLine("    using RenovationRumble.Logic.Data.Commands;");
            sb.AppendLine();
            sb.AppendLine("    public static partial class CommandFactory");
            sb.AppendLine("    {");
            sb.AppendLine("        public static readonly Dictionary<Command, Func<CommandDataModel>> Factories =");
            sb.AppendLine("            new Dictionary<Command, Func<CommandDataModel>>");
            sb.AppendLine("            {");
            
            foreach (var (name, type) in pairs)
                sb.AppendLine($"                [Command.{name}] = () => new {type}(),");
            
            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine("        public static CommandDataModel Create(Command command)");
            sb.AppendLine("            => Factories.TryGetValue(command, out var factory) ? factory() : throw new ArgumentOutOfRangeException(nameof(command));");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private static class Diagnostics
        {
            public static readonly DiagnosticDescriptor MissingOverride = new(
                id: "RRGEN001",
                title: "Command subclass must override Command property",
                messageFormat: "Type '{0}' must override 'public override Command Command => Command.X;'.",
                category: "Generation",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            public static readonly DiagnosticDescriptor UnsupportedGetter = new(
                id: "RRGEN002",
                title: "Unsupported Command getter",
                messageFormat: "Type '{0}' has a 'Command' getter the generator cannot evaluate. Use '=> Command.X'.",
                category: "Generation",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            public static readonly DiagnosticDescriptor DuplicateCommandValue = new(
                id: "RRGEN003",
                title: "Duplicate Command enum value",
                messageFormat: "Command '{0}' is assigned to multiple types: {1}",
                category: "Generation",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);
        }

        private sealed class Receiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> Candidates { get; } = [];
            
            public void OnVisitSyntaxNode(SyntaxNode node)
            {
                if (node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.SealedKeyword)))
                {
                    Candidates.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
