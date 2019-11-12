using Lekser;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;
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

		public void CreateOperationNode( Term number )
		{
			if (_signStack.Count == 0)
				throw new ApplicationException($"Error in sign sign stack empty: {number.Value} in row {number.RowPosition}.");

			IASTNode rightNode;
			IASTNode leftNode;
			switch ( _signStack.Pop() )
			{
				case TermType.Plus:
					rightNode = _nodesStack.Pop();
					leftNode = _nodesStack.Pop();
					_nodesStack.Push( new PlusNode( TermType.Plus, leftNode, rightNode ) );
					break;
				case TermType.Minis:
					rightNode = _nodesStack.Pop();
					leftNode = _nodesStack.Pop();
					_nodesStack.Push( new MinusNode( TermType.Minis, leftNode, rightNode ) );
					break;
				case TermType.Multiple:
					rightNode = _nodesStack.Pop();
					leftNode = _nodesStack.Pop();
					_nodesStack.Push( new MinusNode( TermType.Multiple, leftNode, rightNode ) );
					break;
				default:
					throw new ApplicationException($"Operation not recognized. After:{ number.Value } in row { number.RowPosition }.");
			}
		}

		public void AddSign( Term number )
		{
			_signStack.Push( number.Type );
		}
	}
}
