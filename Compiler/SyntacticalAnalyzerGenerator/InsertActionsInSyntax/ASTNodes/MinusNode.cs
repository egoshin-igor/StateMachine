using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes
{
	public class MinusNode : IASTNode
	{
		public IASTNode LeftValue { get; }
		public IASTNode RightValue { get; }
		public TermType Type { get; }

		public MinusNode(TermType type, IASTNode left, IASTNode right)
		{
			Type = type;
			LeftValue = left;
			RightValue = right;
		}
	}
}
