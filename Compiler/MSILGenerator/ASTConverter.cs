using Lekser.Enums;
using MSILGenerator.MSILLanguage.Constructions;
using MSILGenerator.MSILLanguage.Constructions.Functions;
using MSILGenerator.MSILLanguage.Constructions.Operators;
using MSILGenerator.MSILLanguage.Constructions.Utils;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums;
using System;
using System.Collections.Generic;

namespace MSILGenerator
{
    public class ASTConverter
    {
        private List<IMSILConstruction> _mSILConstructions;

        public ASTConverter()
        {
            _mSILConstructions = new List<IMSILConstruction>();
        }

        public List<IMSILConstruction> GenerateMSILConstructions( List<IASTNode> nodes )
        {
            foreach ( var node in nodes )
            {
                ProccessASTNode( node );
            }
            return _mSILConstructions;
        }

        public List<IMSILConstruction> GetTestProgram()
        {
            var msilConstructions = new List<IMSILConstruction>();
            msilConstructions.Add( new Initializator() );
            var firstVariable = new Variable( "firstValue", VariableType.Integer );
            var secondVariable = new Variable( "secondValue", VariableType.Integer );
            var resultVariable = new Variable( "resultValue", VariableType.Integer );
            var variableNames = new List<string>
            {
                firstVariable.Name,
                secondVariable.Name,
                resultVariable.Name,
                "result"
            };
            var mainBody = new List<IMSILConstruction>
            {
                new StackCapacityFunction( 10 ),
                new WriteLineFunction( "Ого" ),
                new WriteLineFunction( "это" ),
                new WriteLineFunction( "работает" ),
                new VariableDeclarationOperator( variableNames, VariableType.Integer ),
                new AssignmentOperator(VariableType.Integer, firstVariable, 100),
                new AssignmentOperator(VariableType.Integer, secondVariable, 200),
                new AddingOperator( 10, 15, "result" ),
                new AddingOperator( firstVariable, secondVariable, resultVariable ),
                new WriteLineFunction( new Variable( "result", VariableType.Integer ) ),
                new WriteLineFunction( resultVariable )
            };
            msilConstructions.Add( new MainFunction( mainBody ) );
            return msilConstructions;
        }

        private void ProccessASTNode( IASTNode node )
        {
         //   object leftNodeValue = node.Nodes [ 0 ].Value;
          //  object rightNodeValue = node.Nodes [ 1 ].Value;
            if ( node.Nodes [ 0 ].NodeType != NodeType.Leaf )
            {
                //  leftNodeValue = ProccessASTNode( node.Nodes [ 0 ] );
                ProccessASTNode( node.Nodes [ 0 ] );
            }
            if ( node.Nodes [ 1 ].NodeType != NodeType.Leaf )
            {
                //rightNodeValue = ProccessASTNode( node.Nodes [ 1 ] );
                ProccessASTNode( node.Nodes [ 1 ] );
            }

            _mSILConstructions.Add( CreateMSILConstruction( node ) );

        }


        private IMSILConstruction CreateMSILConstruction( IASTNode node )
        {
            switch ( node.NodeType )
            {
                case NodeType.PlusNode:
                    return CreatePlusOperator( node );
                case NodeType.Print:
                    return CreateWriteLineFunction( node );
                default:
                    return null;
            }

        }

        private IMSILConstruction CreateWriteLineFunction( IASTNode node )
        {
            return null;
        }

        private IMSILConstruction CreateAssignmentOperator( IASTNode node )
        {
            return new AssignmentOperator( node.Nodes [ 0 ].Value, int.Parse( node.Nodes [ 1 ].Value ) );
        }

        private IMSILConstruction CreatePlusOperator(IASTNode node)
        {
            if (IsValue(node.Nodes[0]) && IsValue(node.Nodes[1]))
            {
                if ( int.TryParse( node.Nodes [ 0 ].Value, out var numberOne ) && int.TryParse( node.Nodes [ 1 ].Value, out var numberTwo ) )
                {
                    return new AddingOperator( numberOne, numberTwo );
                }
            }

            if ( IsValue( node.Nodes [ 0 ] ) && IsVariable( node.Nodes [ 1 ] ) )
            {
                if ( int.TryParse( node.Nodes [ 0 ].Value, out var numberOne ) )
                {
                    return new AddingOperator( numberOne, node.Nodes [ 1 ].Value );
                }
            }

            if ( IsVariable( node.Nodes [ 0 ] ) && IsValue( node.Nodes [ 1 ] ) )
            {
                if ( int.TryParse( node.Nodes [ 1 ].Value, out var numberTwo ) )
                {
                    return new AddingOperator( node.Nodes [ 0 ].Value, numberTwo);
                }
            }

            if ( IsVariable( node.Nodes [ 0 ] ) && IsVariable( node.Nodes [ 1 ] ) )
            {
                return new AddingOperator( node.Nodes [ 0 ].Value, node.Nodes [ 1 ].Value );
            }
            return null;
        }

        private bool IsValue(IASTNode node)
        {
            switch (node.TermType)
            {
                case TermType.Int:
                case TermType.Bool:
                case TermType.Char:
                case TermType.Double:
                case TermType.String:
                case TermType.DecimalFixedPointNumber:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsVariable( IASTNode node )
        {
            return node.TermType == TermType.Identifier;
        }
    }
}
