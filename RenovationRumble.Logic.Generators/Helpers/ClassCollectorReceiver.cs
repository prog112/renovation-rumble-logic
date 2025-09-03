namespace RenovationRumble.Logic.Generators.Helpers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Collects all class declarations. Reuse across generators.
    /// </summary>
    public sealed class ClassCollectorReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Candidates { get; } = [];

        public void OnVisitSyntaxNode(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax classDeclarationSyntax)
                Candidates.Add(classDeclarationSyntax);
        }
    }
}