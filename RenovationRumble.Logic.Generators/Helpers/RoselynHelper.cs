namespace RenovationRumble.Logic.Generators.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class RoselynHelper
    {
        public static bool HasPublicParameterlessCtor(INamedTypeSymbol type)
        {
            return type.InstanceConstructors.Any(ctor =>
                ctor.Parameters.Length == 0 &&
                ctor.DeclaredAccessibility == Accessibility.Public);
        }

        public static bool InheritsFrom(INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol baseType)
        {
            for (var current = namedTypeSymbol.BaseType; current is not null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                    return true;
            }

            return false;
        }

        public static Location GetFirstLocation(ISymbol symbol)
        {
            return symbol.Locations.FirstOrDefault() ?? Location.None;
        }

        /// <summary>
        /// Tries to find a concrete implemented interface that matches the provided open generic interface.
        /// Returns true and the realized interface symbol (with type arguments) if found.
        /// </summary>
        public static bool ImplementsInterface(INamedTypeSymbol type, INamedTypeSymbol openGenericInterface, out INamedTypeSymbol implemented, int arity = 0)
        {
            implemented = null;

            foreach (var i in type.AllInterfaces)
            {
                if (!SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, openGenericInterface))
                    continue;

                if (arity > 0 && i.TypeArguments.Length != arity)
                    continue;

                implemented = i;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true and the enum member name when a property/getter is written as an expression-bodied MemberAccess like
        ///   public override EnumType Prop => EnumType.Member;
        /// OR
        ///   inside get accessor as in get => EnumType.Member;
        /// </summary>
        public static bool TryGetExpressionEnumMemberName(INamedTypeSymbol type, string propertyName, out string enumMemberName)
        {
            enumMemberName = null;

            var property = type.GetMembers().OfType<IPropertySymbol>()
                .FirstOrDefault(p => p.Name == propertyName && p.GetMethod is not null);

            if (property is null)
                return false;

            var node = property.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

            if (node is PropertyDeclarationSyntax { ExpressionBody.Expression: MemberAccessExpressionSyntax propertySyntax })
            {
                enumMemberName = propertySyntax.Name.Identifier.Text;
                return true;
            }

            if (node is PropertyDeclarationSyntax pds)
            {
                var getter = pds.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.GetAccessorDeclaration));
                if (getter is AccessorDeclarationSyntax { ExpressionBody.Expression: MemberAccessExpressionSyntax accessorSyntax })
                {
                    enumMemberName = accessorSyntax.Name.Identifier.Text;
                    return true;
                }
            }

            return false;
        }
    }
}
