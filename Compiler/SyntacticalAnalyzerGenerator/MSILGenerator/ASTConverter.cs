using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Functions;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators.IfOperator;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Utils;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticalAnalyzerGenerator.MSILGenerator
{
    public class ASTConverter
    {
        private List<IMSILConstruction> _mSILConstructions;
        private List<IMSILConstruction> _body;
        private List<IASTNode> _nodes;
        private int _nodeCounter;
        private int _metkaCounter;
        private List<string> _metkaList;
        private string _beginElseMetka;

        public ASTConverter()
        {
            _mSILConstructions = new List<IMSILConstruction>();
            _body = new List<IMSILConstruction>();
            _metkaList = new List<string>();
            _metkaCounter = 0;
        }

        public List<IMSILConstruction> GenerateMSILConstructions( List<IASTNode> nodes )
        {
            _nodes = nodes;
            _mSILConstructions.Add( new Initializator() );
            for ( _nodeCounter = 0; _nodeCounter < _nodes.Count; _nodeCounter++ )
            {
                ProccessASTNode( _nodes [ _nodeCounter ] );
            }
            _mSILConstructions.Add( new MainFunction( _body ) );
            return _mSILConstructions;
        }

        private void ProccessASTNode( IASTNode node )
        {
            if ( node.Nodes.Count >= 1 && node.Nodes [ 0 ].NodeType != NodeType.Leaf )
            {
                ProccessASTNode( node.Nodes [ 0 ] );
            }
            else if ( node.Nodes.Count >= 1 && IsOperationNeedToPushStack( node ) )
            {
                _body.Add( CreateMSILCodeForLeaf( node.Nodes [ 0 ] ) );
            }

            if ( node.Nodes.Count > 1 && node.Nodes [ 1 ].NodeType != NodeType.Leaf )
            {
                ProccessASTNode( node.Nodes [ 1 ] );
            }
            else if ( IsOperationNeedToPushStack( node ) && node.Nodes.Count > 1 )
            {
                _body.Add( CreateMSILCodeForLeaf( node.Nodes [ 1 ] ) );
            }

            _body.Add( CreateMSILConstruction( node ) );
        }

        private IMSILConstruction CreateMSILCodeForLeaf( IASTNode node )
        {
            if ( IsValue( node ) )
            {
                return CreateMSILConstructionForConstValue( node );
            }
            else
            {
                return CreatePushVariableToStack( node );
            }
        }

        private IMSILConstruction CreateMSILConstructionForConstValue( IASTNode node )
        {
            switch ( node.TermType )
            {
                case TermType.DecimalWholeNumber:
                case TermType.Int:
                    return CreatePushIntToStack( node );
                case TermType.DecimalFixedPointNumber:
                case TermType.Double:
                    return CreatePushDoubleToStack( node );
                case TermType.Bool:
                    return CreatePushBoolToStack( node );
                default:
                    throw new Exception( "Шэф всё пропало, непонятный тип переменной" );
            }
        }

        private IMSILConstruction CreatePushVariableToStack( IASTNode node )
        {
            var pushToStackFun = new PushToStack();
            pushToStackFun.VariableName = node.Value;

            return pushToStackFun;
        }

        private IMSILConstruction CreatePushIntToStack( IASTNode node )
        {
            var pushToStackFun = new PushToStack();
            pushToStackFun.IntValue = Convert.ToInt32( node.Value );
            return pushToStackFun;
        }

        private IMSILConstruction CreatePushDoubleToStack( IASTNode node )
        {
            var pushToStackFun = new PushToStack();
            pushToStackFun.DoubleValue = Convert.ToDouble( node.Value );
            return pushToStackFun;
        }

        private IMSILConstruction CreatePushBoolToStack( IASTNode node )
        {
            var pushToStackFun = new PushToStack
            {
                BoolValue = node.Value
            };
            return pushToStackFun;
        }

        private bool IsSimpleNode( IASTNode node )
        {
            return node.Nodes.All( childNode => childNode.NodeType == NodeType.Leaf );
        }


        private IMSILConstruction CreateMSILConstruction( IASTNode node )
        {
            switch ( node.NodeType )
            {
                case NodeType.PlusNode:
                    return CreatePlusOperator( node );
                case NodeType.UnaryMinusNode:
                    return CreateUnaryMinusNode( node );
                case NodeType.DivisionNode:
                    return CreateDivOperation( node );
                case NodeType.MultipleNode:
                    return CreateMulOperator( node );
                case NodeType.BinaryMinusNode:
                    return CreateSubOperation( node );
                case NodeType.Print:
                    return CreateWriteLineFunction( node );
                case NodeType.DefineNewType:
                    return CreateVariableDeclarationOperator( node );
                case NodeType.Equality:
                    return CreateAssignmentOperator( node );
                case NodeType.Equal:
                    return CreateComparisonOperator( node );
                case NodeType.LogicAnd:
                    return CreateAndOperator( node );
                case NodeType.LogicOr:
                    return CreateOrOperator( node );
                case NodeType.LogicNot:
                    return CreateNotOperator( node );
                case NodeType.Less:
                    return CreateSmallerOperation( node );
                case NodeType.More:
                    return CreateBiggerOperation( node );
                case NodeType.NotEqual:
                    return CreateNotComparisionOperation( node );
                case NodeType.IfTerm:
                    return CreateIfBegin( node );
                case NodeType.IfThenEnd:
                    return CreateEndIf( node );
                case NodeType.IfElseBegin:
                    return CreateElseBegin( node );
                case NodeType.IfElseEnd:
                    return CreateEndElse( node );
                default:
                    return null;
            }
        }

        private IMSILConstruction CreateIfBegin( IASTNode node )
        {
            _metkaList.Add( GetMetkaName() );
            return new BeginIf( _metkaList.Last() );
        }

        private IMSILConstruction CreateEndIf( IASTNode node )
        {
            bool isIfElseOperator = ( _nodeCounter < _nodes.Count - 1 ) && isElseBegin( _nodes [ _nodeCounter + 1 ] );
            if ( isIfElseOperator )
            {
                _beginElseMetka = _metkaList.Last();
                _metkaList.RemoveAt( _metkaList.Count - 1 );
                _metkaList.Add( GetMetkaName() );
                return new EndIf( _metkaList.Last(), true );
            }
            var result = new EndIf( _metkaList.Last() );
            _metkaList.RemoveAt( _metkaList.Count - 1 );
            return result;
        }

        private IMSILConstruction CreateElseBegin( IASTNode node )
        {
            if ( string.IsNullOrEmpty( _beginElseMetka ) )
            {
                throw new Exception( "Шэф, всё пропало у начала else название метки нет" );
            }
            return new BeginElse( _beginElseMetka );
        }

        private IMSILConstruction CreateEndElse( IASTNode node )
        {
            var result = new EndElse( _metkaList.Last() );
            _metkaList.RemoveAt( _metkaList.Count - 1 );
            return result;
        }

        private IMSILConstruction CreateNotComparisionOperation( IASTNode node )
        {
            return new NotComparisonOperation();
        }

        private IMSILConstruction CreateBiggerOperation( IASTNode node )
        {
            return new BiggerOperation();
        }

        private IMSILConstruction CreateSmallerOperation( IASTNode node )
        {
            return new SmallerOperation();
        }

        private IMSILConstruction CreateNotOperator( IASTNode node )
        {
            return new NotBoolOperation();
        }

        private IMSILConstruction CreateAndOperator( IASTNode node )
        {
            return new AndOperation();
        }

        private IMSILConstruction CreateOrOperator( IASTNode node )
        {
            return new OrOperation();
        }

        private IMSILConstruction CreateComparisonOperator( IASTNode node )
        {
            return new ComparisonOperator();
        }

        private IMSILConstruction CreateUnaryMinusNode( IASTNode node )
        {
            var mulOperator = new MulOperator();
            mulOperator.FirstIntValue = -1;

            return mulOperator;
        }

        private IMSILConstruction CreateDivOperation( IASTNode node )
        {
            var divOperator = new DivOperation();
            return divOperator;
        }

        private IMSILConstruction CreateSubOperation( IASTNode node )
        {
            var subOperator = new SubOperation();
            return subOperator;
        }

        private IMSILConstruction CreateMulOperator( IASTNode node )
        {
            var mulOperator = new MulOperator();
            return mulOperator;
        }

        private IMSILConstruction CreateVariableDeclarationOperator( IASTNode node )
        {
            return new VariableDeclarationOperator( new List<string> { node.Nodes [ 0 ].Value }, TermTypeToVariableType( node.TermType ) );
        }

        private IMSILConstruction CreateWriteLineFunction( IASTNode node )
        {
            return new WriteLineFunction( new Variable( node.Nodes [ 1 ].Value, TermTypeToVariableType( node.Nodes [ 0 ].TermType ) ) );
        }

        private IMSILConstruction CreateAssignmentOperator( IASTNode node )
        {
            if ( IsValue( node.Nodes [ 1 ] ) )
            {
                if ( node.Nodes [ 1 ].TermType == TermType.Bool || node.Nodes [ 1 ].TermType == TermType.String )
                {
                    return new AssignmentOperator( node.Nodes [ 0 ].Value, node.Nodes [ 1 ].Value );
                }
                return new AssignmentOperator( node.Nodes [ 0 ].Value, int.Parse( node.Nodes [ 1 ].Value ) );
            }
            else
            {
                return new AssignmentOperator( node.Nodes [ 0 ].Value );
            }
        }

        private IMSILConstruction CreatePlusOperator( IASTNode node )
        {
            var addingOperator = new AddingOperator();
            return addingOperator;
        }

        private VariableType TermTypeToVariableType( TermType type )
        {
            switch ( type )
            {
                case TermType.Int:
                    return VariableType.Integer;
                case TermType.Bool:
                    return VariableType.Bool;
                case TermType.DecimalFixedPointNumber:
                case TermType.DecimalWholeNumber:
                    return VariableType.Integer;
                case TermType.DecimalFloatingPointNumber:
                    return VariableType.Float;
                case TermType.String:
                    return VariableType.String;
            }
            throw new Exception( "Invalid type of VariableType" );
        }

        private bool IsValue( IASTNode node )
        {
            switch ( node.TermType )
            {
                case TermType.Int:
                case TermType.Bool:
                case TermType.Char:
                case TermType.Double:
                case TermType.String:
                case TermType.DecimalFixedPointNumber:
                case TermType.DecimalWholeNumber:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsVariable( IASTNode node )
        {
            return node.TermType == TermType.Identifier;
        }

        private bool IsInt( IASTNode node )
        {
            switch ( node.TermType )
            {
                case TermType.Int:
                case TermType.DecimalFixedPointNumber:
                case TermType.DecimalWholeNumber:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsDouble( IASTNode node )
        {
            return node.TermType == TermType.DecimalFixedPointNumber || node.TermType == TermType.DecimalFloatingPointNumber;
        }

        private bool IsOperationNeedToPushStack( IASTNode node )
        {
            switch ( node.NodeType )
            {
                case NodeType.Print:
                case NodeType.Println:
                case NodeType.DefineNewType:
                case NodeType.Equality:
                    return false;
                default:
                    return true;
            }
        }

        private string GetMetkaName()
        {
            return $"{Constants.METKA}_{_metkaCounter++}";
        }

        private bool isElseBegin( IASTNode node )
        {
            return node.NodeType == NodeType.IfElseBegin;
        }
    }
}
