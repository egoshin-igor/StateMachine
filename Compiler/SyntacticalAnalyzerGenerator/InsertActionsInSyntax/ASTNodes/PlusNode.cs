using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes
{
	public class PlusNode : IASTNode
	{
		public IASTNode LeftValue { get; }
		public IASTNode RightValue { get; }
		public TermType Type { get; }

		public PlusNode(TermType type, IASTNode left, IASTNode right)
		{
			Type = type;
			LeftValue = left;
			RightValue = right;
		}
	}
}
