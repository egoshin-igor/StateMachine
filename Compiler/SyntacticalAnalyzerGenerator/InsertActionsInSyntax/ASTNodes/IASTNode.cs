using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes
{
	public interface IASTNode
	{
		TermType Type { get; }
	}
}
