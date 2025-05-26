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
    internal sealed class PublicInstanceFieldPascalCaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "MY0006";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, "Public instance field must be PascalCase", "Field '{0}' should be PascalCase", "Naming", DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
        }

        private static void AnalyzeField(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax fieldDeclaration = (FieldDeclarationSyntax)context.Node;
            if (fieldDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword) || !fieldDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
                return;

            foreach (VariableDeclaratorSyntax variable in fieldDeclaration.Declaration.Variables)
            {
                string fieldName = variable.Identifier.Text;

                if (fieldName.Length == 0)
                    continue;

                if (fieldName[0] == '_' && (fieldName.Length == 1 || (fieldName.Length > 1 && char.IsDigit(fieldName[1]))))
                    continue;

                if (!char.IsUpper(fieldName[0]))
                {
                    Diagnostic diagnostic = Diagnostic.Create(Rule, variable.GetLocation(), fieldName);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}