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
    internal sealed class NewArrayMustBeInReadOnlySpanAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "MY0005";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, "Only allowed in static ReadOnlySpan property initialization", "Array creation (`new T[]`) is only allowed in static ReadOnlySpan<T> property initializers", "Safety", DiagnosticSeverity.Error, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeArrayCreation, SyntaxKind.ArrayCreationExpression);
        }

        private static void AnalyzeArrayCreation(SyntaxNodeAnalysisContext context)
        {
            ArrayCreationExpressionSyntax arrayCreation = (ArrayCreationExpressionSyntax)context.Node;
            PropertyDeclarationSyntax? property = arrayCreation.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            if (property != null)
            {
                SemanticModel model = context.SemanticModel;
                IPropertySymbol? propSymbol = model.GetDeclaredSymbol(property);
                if (propSymbol != null && propSymbol.IsStatic && propSymbol.Type.OriginalDefinition.ToDisplayString() == "System.ReadOnlySpan<T>")
                {
                    if (property.ExpressionBody != null)
                        return;
                    if (property.AccessorList != null)
                    {
                        foreach (AccessorDeclarationSyntax accessor in property.AccessorList.Accessors)
                        {
                            if (accessor.ExpressionBody != null)
                                return;
                        }
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, arrayCreation.GetLocation()));
        }
    }
}