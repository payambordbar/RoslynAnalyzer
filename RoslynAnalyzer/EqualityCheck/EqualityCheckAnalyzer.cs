using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Collections.Immutable;

namespace RoslynAnalyzer.EqualityCheck
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EqualityCheckAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "EqualityCheck";
        private static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, "Equality Check", "Use 'is pattern instead of == or !='", "EqualityCheck", DiagnosticSeverity.Warning, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(x =>
            {
                var node = (BinaryExpressionSyntax)x.Node;

                if (node.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken))
                {
                    x.ReportDiagnostic(Diagnostic.Create(rule, node.OperatorToken.GetLocation()));
                }
            }, SyntaxKind.EqualsExpression);

            context.RegisterSyntaxNodeAction(x =>
            {
                var node = (BinaryExpressionSyntax)x.Node;

                if (node.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
                {
                    x.ReportDiagnostic(Diagnostic.Create(rule, node.OperatorToken.GetLocation()));
                }
            }, SyntaxKind.NotEqualsExpression);
        }
    }
}
