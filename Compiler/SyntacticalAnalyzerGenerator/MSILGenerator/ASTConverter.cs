﻿using Lekser.Enums;
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

        private void ProccessASTNode( IASTNode node )
        {
            if ( node.Nodes [ 0 ].NodeType != NodeType.Leaf )
            {
                ProccessASTNode( node.Nodes [ 0 ] );
            }
            else if ( IsOperationNeedToPushStack( node ) )
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
                return CreateMsilCodeForVariable( node );
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

        private IMSILConstruction CreateMsilCodeForVariable( IASTNode node )
        {
            var pushToStackFun = new PushToStack();
            /*if ( IsValue( node ) )
            {
                if ( IsInt( node ) )
                {
                    pushToStackFun.IntValue = Convert.ToInt32( node );
                }
                else if ( IsDouble( node ) )
                {
                    pushToStackFun.DoubleValue = Convert.ToDouble( node );
                }
            }
            else if ( IsVariable( node ) )
            {
                pushToStackFun.VariableName = node.Value;
            }*/
            pushToStackFun.VariableName = node.Value;

            return pushToStackFun;
        }

        private IMSILConstruction CreatePushIntToStack( IASTNode node )
        {
            var pushToStackFun = new PushToStack();
            /*  if ( IsValue( node ) )
              {
                  if ( IsInt( node ) )
                  {
                      pushToStackFun.IntValue = Convert.ToInt32( node );
                  }
                  else if ( IsDouble( node ) )
                  {
                      pushToStackFun.DoubleValue = Convert.ToDouble( node );
                  }
              }
              else if ( IsVariable( node ) )
              {
                  pushToStackFun.VariableName = node.Value;
              }*/
            pushToStackFun.IntValue = Convert.ToInt32( node.Value );
            return pushToStackFun;
        }

        private IMSILConstruction CreatePushDoubleToStack( IASTNode node )
        {
            var pushToStackFun = new PushToStack();
            /*   if ( IsValue( node ) )
               {
                   if ( IsInt( node ) )
                   {
                       pushToStackFun.IntValue = Convert.ToInt32( node );
                   }
                   else if ( IsDouble( node ) )
                   {
                       pushToStackFun.DoubleValue = Convert.ToDouble( node );
                   }
               }
               else if ( IsVariable( node ) )
               {
                   pushToStackFun.VariableName = node.Value;
               }*/
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
                default:
                    return null;
            }

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
            /*   if ( node.Nodes [ 0 ].NodeType == NodeType.Leaf )
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
                   else if ( IsVariable( node.Nodes [ 0 ] ) )
                   {
                       mulOperator.FirstVariableName = node.Nodes [ 0 ].Value;
                   }
                   mulOperator.SecondIntValue = -1;
               }
               else
               {
                   mulOperator.FirstIntValue = -1;
               }*/
            mulOperator.FirstIntValue = -1;

            return mulOperator;
        }

        private IMSILConstruction CreateDivOperation( IASTNode node )
        {
            var divOperator = new DivOperation();
            /* if ( node.Nodes [ 0 ].NodeType == NodeType.Leaf )
             {
                 if ( IsValue( node.Nodes [ 0 ] ) )
                 {
                     if ( IsInt( node.Nodes [ 0 ] ) )
                     {
                         divOperator.FirstIntValue = Convert.ToInt32( node.Nodes [ 0 ].Value );
                     }
                     else if ( IsDouble( node.Nodes [ 0 ] ) )
                     {
                         divOperator.FirstDoubleValue = Convert.ToDouble( node.Nodes [ 0 ].Value );
                     }
                 }
                 else if ( IsVariable( node.Nodes [ 0 ] ) )
                 {
                     divOperator.FirstVariableName = node.Nodes [ 0 ].Value;
                 }
             }

             if ( node.Nodes [ 1 ].NodeType == NodeType.Leaf )
             {
                 if ( IsValue( node.Nodes [ 1 ] ) )
                 {
                     if ( IsInt( node.Nodes [ 1 ] ) )
                     {
                         divOperator.SecondIntValue = Convert.ToInt32( node.Nodes [ 1 ].Value );
                     }
                     else if ( IsDouble( node.Nodes [ 1 ] ) )
                     {
                         divOperator.SecondDoubleValue = Convert.ToDouble( node.Nodes [ 1 ].Value );
                     }
                 }
                 else if ( IsVariable( node.Nodes [ 1 ] ) )
                 {
                     divOperator.SecondVariableName = node.Nodes [ 1 ].Value;
                 }
             }*/
            return divOperator;
        }

        private IMSILConstruction CreateSubOperation( IASTNode node )
        {
            var subOperator = new SubOperation();
            /* if ( node.Nodes [ 0 ].NodeType == NodeType.Leaf )
             {
                 if ( IsValue( node.Nodes [ 0 ] ) )
                 {
                     if ( IsInt( node.Nodes [ 0 ] ) )
                     {
                         subOperator.FirstIntValue = Convert.ToInt32( node.Nodes [ 0 ].Value );
                     }
                     else if ( IsDouble( node.Nodes [ 0 ] ) )
                     {
                         subOperator.FirstDoubleValue = Convert.ToDouble( node.Nodes [ 0 ].Value );
                     }
                 }
                 else if ( IsVariable( node.Nodes [ 0 ] ) )
                 {
                     subOperator.FirstVariableName = node.Nodes [ 0 ].Value;
                 }
             }

             if ( node.Nodes [ 1 ].NodeType == NodeType.Leaf )
             {
                 if ( IsValue( node.Nodes [ 1 ] ) )
                 {
                     if ( IsInt( node.Nodes [ 1 ] ) )
                     {
                         subOperator.SecondIntValue = Convert.ToInt32( node.Nodes [ 1 ].Value );
                     }
                     else if ( IsDouble( node.Nodes [ 1 ] ) )
                     {
                         subOperator.SecondDoubleValue = Convert.ToDouble( node.Nodes [ 1 ].Value );
                     }
                 }
                 else if ( IsVariable( node.Nodes [ 1 ] ) )
                 {
                     subOperator.SecondVariableName = node.Nodes [ 1 ].Value;
                 }
             }*/
            return subOperator;
        }

        private IMSILConstruction CreateMulOperator( IASTNode node )
        {
            var mulOperator = new MulOperator();
            /* if ( node.Nodes [ 0 ].NodeType == NodeType.Leaf )
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
                 else if ( IsVariable( node.Nodes [ 0 ] ) )
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
             }*/
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
            /*  if ( node.Nodes [ 0 ].NodeType == NodeType.Leaf )
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
              }*/
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
    }
}
