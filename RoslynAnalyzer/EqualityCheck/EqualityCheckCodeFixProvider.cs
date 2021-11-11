using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace RoslynAnalyzer.EqualityCheck
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EqualityCheckCodeFixProvider))]
    [Shared]
    public class EqualityCheckCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(EqualityCheckAnalyzer.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync();
            var node = (BinaryExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan);
            context.RegisterCodeFix(CodeAction.Create($"Use 'is pattern'", ct =>
            {
                SyntaxToken newToken;
                if (node.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken))
                {
                    newToken = SyntaxFactory.Token(SyntaxKind.IsKeyword);

                }
                else
                {
                    newToken = SyntaxFactory.Token(default, SyntaxKind.IsKeyword, "is not ", "is not ", default);
                }

                var updatedNode = root.ReplaceToken(node.OperatorToken, newToken);

                return Task.FromResult(document.WithSyntaxRoot(updatedNode));
            }), diagnostic);
        }
    }
}
