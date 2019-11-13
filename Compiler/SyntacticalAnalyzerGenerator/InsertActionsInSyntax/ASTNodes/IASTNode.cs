using System.Collections.Generic;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes
{
    public interface IASTNode
    {
        TermType Type { get; }
        string Value { get; }
        List<IASTNode> Nodes { get; }
    }
}
