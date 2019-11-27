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

		private int _predictedUnaryMinusesCount = 0;

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
					_nodesStack.Push( new TreeNode( NodeType.PlusNode, TermType.Plus, nodes, NodeType.PlusNode.ToString() ) );
                    break;
                case TermType.Minis:
                    rightNode = _nodesStack.Pop();
                    leftNode = _nodesStack.Pop();
					nodes.Add(leftNode);
					nodes.Add(rightNode);
					_nodesStack.Push( new TreeNode( NodeType.BinaryMinusNode, TermType.Minis, nodes, NodeType.BinaryMinusNode.ToString()) );
                    break;
				case TermType.Multiple:
					rightNode = _nodesStack.Pop();
					leftNode = _nodesStack.Pop();
					nodes.Add(leftNode);
					nodes.Add(rightNode);
					_nodesStack.Push( new TreeNode( NodeType.MultipleNode, TermType.Multiple, nodes, NodeType.MultipleNode.ToString()) );
					break;
				case TermType.Division:
					rightNode = _nodesStack.Pop();
					leftNode = _nodesStack.Pop();
					nodes.Add(leftNode);
					nodes.Add(rightNode);
					_nodesStack.Push( new TreeNode( NodeType.DivisionNode, TermType.Division, nodes, NodeType.DivisionNode.ToString()) );
					break;
                default:
                    throw new ApplicationException( $"Operation not recognized. After:{ number.Value } in row { number.RowPosition }." );
            }

            return true;
        }

		public void CreateUnaryMinusNode()
		{
			if (_predictedUnaryMinusesCount > 0)
			{
				_predictedUnaryMinusesCount--;
				var nodes = new List<IASTNode>();
				nodes.Add(_nodesStack.Pop());
				
				_nodesStack.Push(new TreeNode(NodeType.UnaryMinusNode, TermType.Minis, nodes, "-"));
			}
		}

		public void UnaryMinusFound()
		{
			_predictedUnaryMinusesCount++;
		}


		public void AddSign( Term number )
        {
            _signStack.Push( number.Type );
        }
    }
}
