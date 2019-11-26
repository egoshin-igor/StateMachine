using System.Collections.Generic;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes
{
    public class LeafNode : IASTNode
    {
        public string Value { get; }
		public NodeType NodeType { get; }
		public TermType TermType { get; }
        public List<IASTNode> Nodes { get; }

		public LeafNode( TermType termType, string value )
        {
			NodeType = NodeType.Leaf;
			TermType = termType;
            Value = value;
            Nodes = new List<IASTNode>();
        }
    }
}
