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
    internal sealed class StringFormatToInterpolatedStringAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "MY0002";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, "Use string interpolation instead of string.Format", "Use string interpolation instead of string.Format", "Style", DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess && memberAccess.Name.Identifier.Text == "Format")
            {
                IMethodSymbol? symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
                if (symbol == null || symbol.ContainingType?.SpecialType != SpecialType.System_String)
                    return;
                Diagnostic diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}