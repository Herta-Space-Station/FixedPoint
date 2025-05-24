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
using Microsoft.CodeAnalysis.Editing;

#pragma warning disable CS8604
#pragma warning disable CS8631
#pragma warning disable CS8602
#pragma warning disable RS1038
#pragma warning disable RS2008

namespace Herta.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StaticGetPropertyCodeFixProvider))]
    [Shared]
    internal sealed class StaticGetPropertyCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => [StaticGetPropertyAnalyzer.DIAGNOSTIC_ID];

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics[0];
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var property = root.FindNode(diagnostic.Location.SourceSpan) as PropertyDeclarationSyntax;
            if (property == null)
                return;
            context.RegisterCodeFix(CodeAction.Create("Convert to static readonly field", c => ConvertToFieldAsync(context.Document, property, c), "ConvertToField"), diagnostic);
        }

        private async Task<Document> ConvertToFieldAsync(Document document, PropertyDeclarationSyntax property, CancellationToken cancellationToken)
        {
            var generator = SyntaxGenerator.GetGenerator(document);
            var fieldName = property.Identifier.Text;
            var fieldType = property.Type;
            var fieldValue = generator.DefaultExpression(fieldType);
            if (property.ExpressionBody != null)
            {
                fieldValue = property.ExpressionBody.Expression;
            }
            else
            {
                var getter = property.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));
                if (getter != null)
                {
                    if (getter.ExpressionBody != null)
                    {
                        fieldValue = getter.ExpressionBody.Expression;
                    }
                    else if (getter.Body != null)
                    {
                        var returnStatement = getter.Body.Statements.OfType<ReturnStatementSyntax>().FirstOrDefault();
                        if (returnStatement != null)
                            fieldValue = returnStatement.Expression ?? fieldValue;
                    }
                }
            }

            var fieldDeclaration = generator.FieldDeclaration(fieldName, fieldType, Accessibility.Public, DeclarationModifiers.Static | DeclarationModifiers.ReadOnly, fieldValue);
            var leadingTrivia = property.GetLeadingTrivia();
            if (leadingTrivia.Any())
            {
                var fieldSyntax = (FieldDeclarationSyntax)fieldDeclaration;
                fieldDeclaration = fieldSyntax.WithLeadingTrivia(leadingTrivia);
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root.ReplaceNode(property, fieldDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}