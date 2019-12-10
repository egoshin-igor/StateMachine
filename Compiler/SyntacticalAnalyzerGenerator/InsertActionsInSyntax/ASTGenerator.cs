using Lekser;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public class ASTGenerator
    {
        private Stack<TermType> _signStack;
        private Stack<IASTNode> _nodesStack;

        private int _predictedUnaryMinusesCount = 0;
        private int _minusesWithBracketsCount = 0;

        public IASTNode RootNode => _nodesStack.Peek();

        private List<IASTNode> _rootNodes = new List<IASTNode>();
        public IReadOnlyCollection<IASTNode> RootNodes => _rootNodes;

        public ASTGenerator()
        {
            _signStack = new Stack<TermType>();
            _nodesStack = new Stack<IASTNode>();
        }

        public void SaveAndClear()
        {
            if ( _nodesStack.Any() )
            {
                _rootNodes.Add( RootNode );
            }
            _nodesStack.Clear();
            _signStack.Clear();
            _predictedUnaryMinusesCount = 0;
            _minusesWithBracketsCount = 0;
        }

        public void CreateLeafNode( Term number )
        {
            _nodesStack.Push( new LeafNode( number.Type, number.Value ) );
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
                    nodes.Add( leftNode );
                    nodes.Add( rightNode );
                    _nodesStack.Push( new TreeNode( NodeType.PlusNode, TermType.Plus, nodes ) );
                    break;
                case TermType.Minis:
                    rightNode = _nodesStack.Pop();
                    leftNode = _nodesStack.Pop();
                    nodes.Add( leftNode );
                    nodes.Add( rightNode );
                    _nodesStack.Push( new TreeNode( NodeType.BinaryMinusNode, TermType.Minis, nodes ) );
                    break;
                case TermType.Multiple:
                    rightNode = _nodesStack.Pop();
                    leftNode = _nodesStack.Pop();
                    nodes.Add( leftNode );
                    nodes.Add( rightNode );
                    _nodesStack.Push( new TreeNode( NodeType.MultipleNode, TermType.Multiple, nodes ) );
                    break;
                case TermType.Division:
                    rightNode = _nodesStack.Pop();
                    leftNode = _nodesStack.Pop();
                    nodes.Add( leftNode );
                    nodes.Add( rightNode );
                    _nodesStack.Push( new TreeNode( NodeType.DivisionNode, TermType.Division, nodes ) );
                    break;
                default:
                    throw new ApplicationException( $"Operation not recognized. After:{ number.Value } in row { number.RowPosition }." );
            }

            return true;
        }

        public void CreateUnaryMinusNode()
        {
            int unaryMinusesCountWithSingleNumber = _predictedUnaryMinusesCount - _minusesWithBracketsCount;
            if ( unaryMinusesCountWithSingleNumber > 0 )
            {
                _predictedUnaryMinusesCount--;
                var nodes = new List<IASTNode>();
                nodes.Add( _nodesStack.Pop() );

                _nodesStack.Push( new TreeNode( NodeType.UnaryMinusNode, TermType.Minis, nodes, "-" ) );
            }
        }

        public void AddDeclarationNode( Term term )
        {
            switch ( term.Type )
            {
                case TermType.Int:
                case TermType.Float:
                case TermType.Bool:
                case TermType.Char:
                case TermType.Double:
                    _nodesStack.Push( new TreeNode( NodeType.DefineNewType, term.Type, new List<IASTNode>(), term.Value ) );
                    break;
                case TermType.Identifier:
                    IASTNode typeNode = _nodesStack.Pop();
                    typeNode.Nodes.Add( new LeafNode( TermType.Identifier, term.Value ) );
                    _nodesStack.Push( typeNode );
                    break;
                default:
                    break;
            }

        }

        public void AddLeafNode( Term term )
        {
            _nodesStack.Push( new LeafNode( term.Type, term.Value ) );
        }

        public void AddPrintNode()
        {
            var nodes = new List<IASTNode>();
            while ( _nodesStack.Any() )
            {
                nodes.Add( _nodesStack.Pop() );
            }
            if ( nodes.Count == 1 )
            {
                _nodesStack.Push( new TreeNode(
                    NodeType.Println,
                    TermType.Println,
                    new List<IASTNode>() )
                );
            }

            _nodesStack.Push( new TreeNode(
                NodeType.Print,
                TermType.Print,
                new List<IASTNode> { nodes[ 1 ], nodes[ 0 ] } )
            );
        }

        public void AddEqualityNode()
        {
            var nodes = new List<IASTNode>();
            while ( _nodesStack.Any() )
            {
                IASTNode node = _nodesStack.Pop();
                if ( _nodesStack.Any() )
                {
                    nodes.Add( node );
                }
                else
                {
                    nodes.Insert( 0, node );
                }
            }

            _nodesStack.Push( new TreeNode( NodeType.Equality, TermType.Equally, nodes, "=" ) );
        }

        public void UnaryMinusFound()
        {
            _predictedUnaryMinusesCount++;
        }

        public void AddSign( Term number )
        {
            _signStack.Push( number.Type );
        }

        public void OpenBracketFound()
        {
            if ( _predictedUnaryMinusesCount > 0 )
            {
                _minusesWithBracketsCount++;
            }
        }

        public void CloseBracketFound()
        {
            if ( _minusesWithBracketsCount > 0 )
            {
                _minusesWithBracketsCount--;
                _predictedUnaryMinusesCount--;
                var nodes = new List<IASTNode>();
                nodes.Add( _nodesStack.Pop() );

                _nodesStack.Push( new TreeNode( NodeType.UnaryMinusNode, TermType.Minis, nodes, "-" ) );
            }
        }
    }
}
