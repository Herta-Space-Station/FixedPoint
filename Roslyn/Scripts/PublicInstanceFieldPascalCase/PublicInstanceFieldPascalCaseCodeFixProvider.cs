using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable CS8604
#pragma warning disable CS8631
#pragma warning disable CS8602
#pragma warning disable RS1038
#pragma warning disable RS2008

// ReSharper disable ALL

namespace Herta.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PublicInstanceFieldPascalCaseCodeFixProvider)), Shared]
    internal sealed class PublicInstanceFieldPascalCaseCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => [PublicInstanceFieldPascalCaseAnalyzer.DIAGNOSTIC_ID];

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null)
                return;
            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;
            VariableDeclaratorSyntax? variableDeclarator = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            if (variableDeclarator == null)
                return;
            string fieldName = variableDeclarator.Identifier.Text;
            if (string.IsNullOrEmpty(fieldName))
                return;
            string newName = char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1);
            context.RegisterCodeFix(CodeAction.Create(title: $"Rename to '{newName}'", createChangedSolution: c => RenameFieldAsync(context.Document, variableDeclarator, newName, c), equivalenceKey: "RenameToPascalCase"), diagnostic);
        }

        private static async Task<Solution> RenameFieldAsync(Document document, VariableDeclaratorSyntax variable, string newName, CancellationToken cancellationToken)
        {
            SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null)
                return document.Project.Solution;
            IFieldSymbol? fieldSymbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken) as IFieldSymbol;
            if (fieldSymbol == null)
                return document.Project.Solution;
            Solution solution = document.Project.Solution;
            Solution? newSolution = await Renamer.RenameSymbolAsync(solution, fieldSymbol, default, newName, cancellationToken).ConfigureAwait(false);
            return newSolution;
        }
    }
}