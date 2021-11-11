using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;

namespace RoslynAnalyzer.DateTime
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DateTimeCodeFixProvider))]
    [Shared]
    public class DateTimeCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DateTimeAnalyzer.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics[0];
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync();
            var node = (TypeSyntax)root.FindNode(diagnostic.Location.SourceSpan);

            context.RegisterCodeFix(CodeAction.Create($"Use '{nameof(DateTimeOffset)}'", ct =>
            {
                var newNode = SyntaxFactory.IdentifierName(
                    node.ToString().EndsWith("?")
                    ? $"{nameof(DateTimeOffset)}?"
                    : nameof(DateTimeOffset));

                var updatedNode = root.ReplaceNode(node, newNode);
                return Task.FromResult(document.WithSyntaxRoot(updatedNode));
            }), diagnostic);
        }
    }
}
