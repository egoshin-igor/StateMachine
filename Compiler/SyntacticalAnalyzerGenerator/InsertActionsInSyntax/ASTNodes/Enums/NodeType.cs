using System;
using System.Collections.Generic;
using System.Text;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums
{
    public enum NodeType
    {
        PlusNode,
        BinaryMinusNode,
        UnaryMinusNode,
        MultipleNode,
        DivisionNode,
        Leaf,
        DefineNewType,
        Equality
    }
}
