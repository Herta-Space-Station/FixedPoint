using System.Collections.Generic;
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
using Microsoft.CodeAnalysis.Text;

#pragma warning disable CS8604
#pragma warning disable CS8631
#pragma warning disable CS8602
#pragma warning disable RS1038
#pragma warning disable RS2008

// ReSharper disable ALL

namespace Herta.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NoArrayParameterCodeFixProvider))]
    [Shared]
    internal sealed class NoArrayParameterCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => [NoArrayParameterAnalyzer.DIAGNOSTIC_ID];

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            ParameterSyntax? parameterNode = root?.FindNode(diagnosticSpan) as ParameterSyntax;
            if (parameterNode == null)
                return;

            context.RegisterCodeFix(CodeAction.Create(title: "Replace with Span<T> or ReadOnlySpan<T>", createChangedDocument: c => ReplaceWithSpanAsync(context.Document, parameterNode, c), equivalenceKey: "ReplaceWithSpan"), diagnostic);
        }

        private async Task<Document> ReplaceWithSpanAsync(Document document, ParameterSyntax parameter, CancellationToken cancellationToken)
        {
            await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            ParameterListSyntax? parameterList = parameter.Parent as ParameterListSyntax;
            SyntaxNode? parentDecl = parameterList?.Parent;
            bool useWritableSpan = false;
            if (parentDecl is BaseMethodDeclarationSyntax methodDecl)
            {
                if (methodDecl.Body != null)
                {
                    useWritableSpan = ContainsWriteToParameter(methodDecl.Body, parameter.Identifier.Text);
                }
                else if (methodDecl.ExpressionBody != null)
                {
                    useWritableSpan = false;
                }
            }
            else if (parentDecl is LocalFunctionStatementSyntax localFunc)
            {
                if (localFunc.Body != null)
                {
                    useWritableSpan = ContainsWriteToParameter(localFunc.Body, parameter.Identifier.Text);
                }
                else if (localFunc.ExpressionBody != null)
                {
                    useWritableSpan = false;
                }
            }
            else if (parentDecl is ConstructorDeclarationSyntax ctorDecl)
            {
                if (ctorDecl.Body != null)
                {
                    useWritableSpan = ContainsWriteToParameter(ctorDecl.Body, parameter.Identifier.Text);
                }
            }

            TypeSyntax spanType = SyntaxFactory.ParseTypeName(useWritableSpan ? "Span" : "ReadOnlySpan").WithTrailingTrivia(parameter.Type!.GetTrailingTrivia());
            TypeSyntax elementType = ((ArrayTypeSyntax)parameter.Type!).ElementType;
            GenericNameSyntax genericType = SyntaxFactory.GenericName(spanType.ToString()).WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(elementType)));
            ParameterSyntax newParameter = parameter.WithType(genericType);
            SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode newRoot = root!.ReplaceNode(parameter, newParameter);
            return document.WithSyntaxRoot(newRoot);
        }

        private static bool ContainsWriteToParameter(BlockSyntax body, string paramName)
        {
            IEnumerable<ElementAccessExpressionSyntax> writes = body.DescendantNodes().OfType<ElementAccessExpressionSyntax>().Where(e => e.Expression is IdentifierNameSyntax id && id.Identifier.Text == paramName);
            foreach (ElementAccessExpressionSyntax? access in writes)
            {
                SyntaxNode? parent = access.Parent;
                if (parent is AssignmentExpressionSyntax assignment && assignment.Left == access)
                    return true;
            }

            return false;
        }
    }
}