using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

#pragma warning disable RS1038
#pragma warning disable RS2008

// ReSharper disable ALL

namespace Herta.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class NoArrayParameterAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "MY0003";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, "Do not use array parameters", "Parameter '{0}' should be changed to Span<T> or ReadOnlySpan<T>", "Performance", DiagnosticSeverity.Error, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeParameterList, SyntaxKind.Parameter);
        }

        private static void AnalyzeParameterList(SyntaxNodeAnalysisContext context)
        {
            ParameterSyntax parameter = (ParameterSyntax)context.Node;
            if (parameter.Type is ArrayTypeSyntax)
            {
                SyntaxNode? parent = parameter.Parent?.Parent;
                if (parent is MethodDeclarationSyntax || parent is ConstructorDeclarationSyntax || parent is LocalFunctionStatementSyntax || parent is DelegateDeclarationSyntax)
                {
                    IParameterSymbol? paramSymbol = context.SemanticModel.GetDeclaredSymbol(parameter);
                    if (paramSymbol == null)
                        return;
                    Diagnostic diagnostic = Diagnostic.Create(Rule, parameter.GetLocation(), paramSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}