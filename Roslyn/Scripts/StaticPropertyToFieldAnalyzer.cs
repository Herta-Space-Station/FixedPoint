using System.Collections.Immutable;
using System.Linq;
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
    internal sealed class StaticGetPropertyAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "MY0001";

        private static readonly DiagnosticDescriptor Rule = new(DIAGNOSTIC_ID, "Use static readonly field instead of static get-only property", "Property '{0}' can be converted to static readonly field", "Performance", DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        }

        private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax property = (PropertyDeclarationSyntax)context.Node;
            IPropertySymbol? symbol = context.SemanticModel.GetDeclaredSymbol(property);
            if (symbol == null)
                return;
            if (!symbol.IsStatic || symbol.GetMethod == null || symbol.SetMethod != null)
                return;
            ITypeSymbol propertyType = symbol.Type;
            ITypeSymbol? getReturnType = null;
            if (property.ExpressionBody != null)
            {
                ExpressionSyntax expr = property.ExpressionBody.Expression;
                getReturnType = context.SemanticModel.GetTypeInfo(expr).Type;
            }
            else if (property.AccessorList != null)
            {
                AccessorDeclarationSyntax? getter = property.AccessorList.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));
                if (getter != null)
                {
                    if (getter.ExpressionBody != null)
                    {
                        getReturnType = context.SemanticModel.GetTypeInfo(getter.ExpressionBody.Expression).Type;
                    }
                    else if (getter.Body != null)
                    {
                        ReturnStatementSyntax? ret = getter.Body.Statements.OfType<ReturnStatementSyntax>().FirstOrDefault();
                        if (ret != null && ret.Expression != null)
                            getReturnType = context.SemanticModel.GetTypeInfo(ret.Expression).Type;
                    }
                }
            }

            if (getReturnType == null || !getReturnType.IsValueType)
                return;
            if (!SymbolEqualityComparer.Default.Equals(propertyType, getReturnType))
                return;
            Diagnostic diagnostic = Diagnostic.Create(Rule, property.GetLocation(), symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}