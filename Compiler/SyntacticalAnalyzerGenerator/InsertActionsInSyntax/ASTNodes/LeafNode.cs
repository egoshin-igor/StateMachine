using System.Collections.Generic;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes
{
    public class LeafNode : IASTNode
    {
        public string Value { get; }
        public TermType Type { get; }
        public List<IASTNode> Nodes { get; }

        public LeafNode( TermType type, string value )
        {
            Type = type;
            Value = value;
            Nodes = new List<IASTNode>();
        }
    }
}
