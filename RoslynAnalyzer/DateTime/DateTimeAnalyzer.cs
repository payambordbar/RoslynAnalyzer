using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using System;
using System.Collections.Immutable;
using System.Linq;

namespace RoslynAnalyzer.DateTime
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UsingDateTime";
        private static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, "DateTime", $"Using {nameof(DateTimeOffset)} Instead of {nameof(DateTime)}", "DateTime", DiagnosticSeverity.Warning, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(x =>
            {
                var node = (PropertyDeclarationSyntax)x.Node;
                var type = node.Type.ToString();

                if (type == "DateTime" || type == "DateTime?")
                {
                    x.ReportDiagnostic(Diagnostic.Create(rule, node.ChildNodes().FirstOrDefault(n => n.IsKind(SyntaxKind.IdentifierName) || n.IsKind(SyntaxKind.NullableType)).GetLocation()));
                }
            }, SyntaxKind.PropertyDeclaration);
        }
    }
}
