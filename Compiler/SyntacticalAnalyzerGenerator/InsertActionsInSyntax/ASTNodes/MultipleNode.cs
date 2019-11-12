using Lekser.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes
{
	public class MultipleNode : IASTNode
	{
		public IASTNode LeftValue { get; }
		public IASTNode RightValue { get; }
		public TermType Type { get; }

		public MultipleNode(TermType type, IASTNode left, IASTNode right)
		{
			Type = type;
			LeftValue = left;
			RightValue = right;
		}
	}
}
