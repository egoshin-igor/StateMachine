using System.Collections.Generic;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes
{
    public interface IASTNode
    {
		NodeType NodeType { get; }
        TermType TermType { get; }
        string Value { get; }
        List<IASTNode> Nodes { get; }
    }
}
