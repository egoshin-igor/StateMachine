using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Functions;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators;
using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticalAnalyzerGenerator.MSILGenerator
{
    public class ASTConverter
    {
        private List<IMSILConstruction> _mSILConstructions;
        private List<IMSILConstruction> _body;

        public ASTConverter()
        {
            _mSILConstructions = new List<IMSILConstruction>();
            _body = new List<IMSILConstruction>();
        }

        public List<IMSILConstruction> GenerateMSILConstructions( List<IASTNode> nodes )
        {
            _mSILConstructions.Add( new Initializator() );
            foreach ( var node in nodes )
            {
                ProccessASTNode( node );
            }
            _mSILConstructions.Add( new MainFunction( _body ) );
            return _mSILConstructions;
        }

      /*  public List<IMSILConstruction> GetTestProgram()
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
        }*/

        private void ProccessASTNode( IASTNode node )
        {
            if ( node.Nodes [ 0 ].NodeType != NodeType.Leaf )
            {
                ProccessASTNode( node.Nodes [ 0 ] );
            }

            if ( node.Nodes.Count > 1 && node.Nodes [ 1 ].NodeType != NodeType.Leaf )
            {
                ProccessASTNode( node.Nodes [ 1 ] );
            }

            _body.Add( CreateMSILConstruction( node ) );

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
                case NodeType.MultipleNode:
                    return CreateMulOperator( node );
                case NodeType.Print:
                    return CreateWriteLineFunction( node );
                case NodeType.DefineNewType:
                    return CreateVariableDeclarationOperator( node );
                case NodeType.Equality:
                    return CreateAssignmentOperator( node );
                default:
                    return null;
            }

        }

        private IMSILConstruction CreateMulOperator( IASTNode node )
        {
            var mulOperator = new MulOperator();
            if ( node.Nodes [ 0 ].NodeType == NodeType.Leaf )
            {
                if ( IsValue( node.Nodes [ 0 ] ) )
                {
                    if ( IsInt( node.Nodes [ 0 ] ) )
                    {
                        mulOperator.FirstIntValue = Convert.ToInt32( node.Nodes [ 0 ].Value );
                    }
                    else if ( IsDouble( node.Nodes [ 0 ] ) )
                    {
                        mulOperator.FirstDoubleValue = Convert.ToDouble( node.Nodes [ 0 ].Value );
                    }
                }
                else if (IsVariable(node.Nodes[0]))
                {
                    mulOperator.FirstVariableName = node.Nodes [ 0 ].Value;
                }
            }

            if ( node.Nodes [ 1 ].NodeType == NodeType.Leaf )
            {
                if ( IsValue( node.Nodes [ 1 ] ) )
                {
                    if ( IsInt( node.Nodes [ 1 ] ) )
                    {
                        mulOperator.SecondIntValue = Convert.ToInt32( node.Nodes [ 1 ].Value );
                    }
                    else if ( IsDouble( node.Nodes [ 1 ] ) )
                    {
                        mulOperator.SecondDoubleValue = Convert.ToDouble( node.Nodes [ 1 ].Value );
                    }
                }
                else if ( IsVariable( node.Nodes [ 1 ] ) )
                {
                    mulOperator.SecondVariableName = node.Nodes [ 1 ].Value;
                }
            }
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
            if ( node.Nodes [ 0 ].NodeType == NodeType.Leaf )
            {
                if ( IsValue( node.Nodes [ 0 ] ) )
                {
                    if ( IsInt( node.Nodes [ 0 ] ) )
                    {
                        addingOperator.FirstIntValue = Convert.ToInt32( node.Nodes [ 0 ].Value );
                    }
                    else if ( IsDouble( node.Nodes [ 0 ] ) )
                    {
                        addingOperator.FirstDoubleValue = Convert.ToDouble( node.Nodes [ 0 ].Value );
                    }
                }
                else if ( IsVariable( node.Nodes [ 0 ] ) )
                {
                    addingOperator.FirstVariableName = node.Nodes [ 0 ].Value;
                }
            }

            if ( node.Nodes [ 1 ].NodeType == NodeType.Leaf )
            {
                if ( IsValue( node.Nodes [ 1 ] ) )
                {
                    if ( IsInt( node.Nodes [ 1 ] ) )
                    {
                        addingOperator.SecondIntValue = Convert.ToInt32( node.Nodes [ 1 ].Value );
                    }
                    else if ( IsDouble( node.Nodes [ 1 ] ) )
                    {
                        addingOperator.SecondDoubleValue = Convert.ToDouble( node.Nodes [ 1 ].Value );
                    }
                }
                else if ( IsVariable( node.Nodes [ 1 ] ) )
                {
                    addingOperator.SecondVariableName = node.Nodes [ 1 ].Value;
                }
            }
            return addingOperator;
            /*     if ( IsValue( node.Nodes [ 0 ] ) && IsValue( node.Nodes [ 1 ] ) )
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
                         return new AddingOperator( node.Nodes [ 0 ].Value, numberTwo );
                     }
                 }

                 if ( IsVariable( node.Nodes [ 0 ] ) && IsVariable( node.Nodes [ 1 ] ) )
                 {
                     return new AddingOperator( node.Nodes [ 0 ].Value, node.Nodes [ 1 ].Value );
                 }

                 if ( string.IsNullOrEmpty( node.Nodes [ 0 ].Value ) && IsValue( node.Nodes [ 1 ] ) )
                 {
                     if ( int.TryParse( node.Nodes [ 1 ].Value, out var numberOne ) )
                     {
                         return new AddingOperator( numberOne );
                     }
                 }

                 if ( string.IsNullOrEmpty( node.Nodes [ 1 ].Value ) && IsValue( node.Nodes [ 0 ] ) )
                 {
                     if ( int.TryParse( node.Nodes [ 0 ].Value, out var numberOne ) )
                     {
                         return new AddingOperator( numberOne );
                     }
                 }

                 if ( string.IsNullOrEmpty( node.Nodes [ 0 ].Value ) && IsVariable( node.Nodes [ 1 ] ) )
                 {
                     return new AddingOperator( node.Nodes [ 1 ].Value );
                 }

                 if ( string.IsNullOrEmpty( node.Nodes [ 1 ].Value ) && IsVariable( node.Nodes [ 0 ] ) )
                 {
                     return new AddingOperator( node.Nodes [ 0 ].Value );
                 }

                 throw new Exception( "Не удалось создать оператор сложения" );
                 return null;*/
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
    }
}
