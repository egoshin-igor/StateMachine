using Lekser;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums;
using System;
using System.Collections.Generic;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public class ASTGenerator
    {
        private Stack<TermType> _signStack;
        private Stack<IASTNode> _nodesStack;

        public IASTNode RootNode => _nodesStack.Peek();

        public ASTGenerator()
        {
            _signStack = new Stack<TermType>();
            _nodesStack = new Stack<IASTNode>();
        }

        public void CreateLeafNode( Term number )
        {
            _nodesStack.Push( new LeafNode( number.Type, number.Value ) );
        }

        public bool IsSuccessfulyCreated()
        {
            return _nodesStack.Count == 1;
        }

        public bool CreateOperationNode( Term number )
        {
            if ( _signStack.Count == 0 )
                throw new ApplicationException( $"Error in sign sign stack empty: { number.Value } in row { number.RowPosition }." );

            IASTNode rightNode;
            IASTNode leftNode;
			var nodes = new List<IASTNode>();
			switch ( _signStack.Pop() )
            {
                case TermType.Plus:
                    rightNode = _nodesStack.Pop();
                    leftNode = _nodesStack.Pop();
					nodes.Add(leftNode);
					nodes.Add(rightNode);
					_nodesStack.Push( new TreeNode( NodeType.PlusNode, TermType.Plus, nodes ) );
                    break;
                case TermType.Minis:
                    rightNode = _nodesStack.Pop();
                    leftNode = _nodesStack.Pop();
					nodes.Add(leftNode);
					nodes.Add(rightNode);
					_nodesStack.Push( new TreeNode( NodeType.BinaryMinusNode, TermType.Minis, nodes ) );
                    break;
				case TermType.Multiple:
					rightNode = _nodesStack.Pop();
					leftNode = _nodesStack.Pop();
					nodes.Add(leftNode);
					nodes.Add(rightNode);
					_nodesStack.Push( new TreeNode( NodeType.MultipleNode, TermType.Multiple, nodes ) );
					break;
				case TermType.Division:
					rightNode = _nodesStack.Pop();
					leftNode = _nodesStack.Pop();
					nodes.Add(leftNode);
					nodes.Add(rightNode);
					_nodesStack.Push( new TreeNode( NodeType.DivisionNode, TermType.Division, nodes ) );
					break;
                default:
                    throw new ApplicationException( $"Operation not recognized. After:{ number.Value } in row { number.RowPosition }." );
            }

            return true;
        }

        public void AddSign( Term number )
        {
            _signStack.Push( number.Type );
        }
    }
}
