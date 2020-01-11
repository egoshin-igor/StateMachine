using Lekser;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes.Enums;
using SyntacticalAnalyzerGenerator.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyntacticalAnalyzerGenerator
{
    public class Runner
    {
        private const string END_WORD_NAME = Word.End;

        // private int _currentWordIndex;
        private int _currentTableIndex;
        private List<int> _indexStack;
        private ProgramLekser _programLekser;
        private readonly IVariablesTableController _variablesTableController;
        private readonly TypeController _typeController;
        private readonly AriphmeticalOperationsController _ariphmeticalOperationsController;
        private readonly List<ResultTableRow> _table;
        private readonly ASTGenerator _aSTGenerator;
        private Term _currentTerm;
        private List<IASTNode> _trees;
        private bool _isIncorrectTerms = false;
        private int? _tempGoTo = null;

        public Runner(
            ProgramLekser programLekser,
            IVariablesTableController variablesTableController,
            TypeController typeController,
            AriphmeticalOperationsController ariphmeticalOperationsController,
            List<ResultTableRow> table )
        {
            _aSTGenerator = new ASTGenerator(); // move realisation somewhere else if you want
            _programLekser = programLekser;
            _variablesTableController = variablesTableController;
            _typeController = typeController;
            _ariphmeticalOperationsController = ariphmeticalOperationsController;
            _table = table;
            _indexStack = new List<int>();
        }

        public async Task<List<IASTNode>> GetTrees()
        {
            if ( _trees != null )
                return _trees;
            if ( _isIncorrectTerms )
                return null;

            _currentTableIndex = 0;
            _currentTerm = await _programLekser.GetTermAsync();

            bool result = await CheckWordsAsync( _table );
            if ( result )
            {
                _trees = _aSTGenerator.RootNodes.ToList();
                return _trees;
            }
            else
            {
                _isIncorrectTerms = true;
                return null;
            }
        }

        private async Task<bool> CheckWordsAsync( List<ResultTableRow> table )
        {
            if ( CanProcessRow( table ) )  // проверяем можно ли обрабатывать строку в таблице
            {
                if ( !string.IsNullOrEmpty( table[ _currentTableIndex ].ActionName ) )
                {
                    DoOnAction( table[ _currentTableIndex ].ActionName );
                }
                await ShiftIfEnabledAsync( table );
                PushToStackIfEnabled( table );
                if ( table[ _currentTableIndex ].GoTo == -1 && _indexStack.Count > 0 )  // переходим по стеку, если нельзя по goto
                {
                    _currentTableIndex = _indexStack.Last();
                    _indexStack.RemoveAt( _indexStack.Count - 1 );
                    return await CheckWordsAsync( table );
                }
                if ( table[ _currentTableIndex ].GoTo != -1 )  // переходим по goto
                {
                    _currentTableIndex = _tempGoTo ?? table[ _currentTableIndex ].GoTo;
                    if ( _tempGoTo.HasValue )
                    {
                        _tempGoTo = null;
                    }
                    return await CheckWordsAsync( table );
                }


                bool isCorrectLang = _indexStack.Count == 0 && table[ _currentTableIndex ].IsEnd;
                if ( !isCorrectLang )
                    throw new ApplicationException( $"Error. Incorrect execution of rule:{table[ _currentTableIndex ].Name} on row {_currentTerm.RowPosition}" );

                return isCorrectLang;
            }
            else
            {
                if ( table[ _currentTableIndex ].ShiftOnError != -1 )  // переходим по onError, если возможно и нельзя обработать строку
                {
                    _currentTableIndex = table[ _currentTableIndex ].ShiftOnError;
                    return await CheckWordsAsync( table );
                }

                throw new ApplicationException( $"Error. Incorrect execution of rule:{table[ _currentTableIndex ].Name} on row {_currentTerm.RowPosition}" );
            }
        }

        private bool CanProcessRow( List<ResultTableRow> table )
        {
            var currentTermType = _currentTerm == null ? TermType.End : _currentTerm.Type;

            return table[ _currentTableIndex ].DirectingSet.Contains( currentTermType ) || ( currentTermType == TermType.End && table[ _currentTableIndex ].DirectingSet.Count == 0 );
        }

        private async Task ShiftIfEnabledAsync( List<ResultTableRow> table )
        {
            if ( table[ _currentTableIndex ].IsShift )
            {
                _currentTerm = await _programLekser.GetTermAsync();
            }
        }

        private void PushToStackIfEnabled( List<ResultTableRow> table )
        {
            if ( table[ _currentTableIndex ].IsPushToStack )
            {
                _indexStack.Add( _currentTableIndex + 1 );
            }
        }

        private void DoOnAction( string actionName )
        {
            var actionNameData = actionName.Split( '.' );
            if ( actionNameData.Length < 1 )
                throw new ApplicationException();

            switch ( actionNameData[ 0 ] )
            {
                case ActionSourceType.VariablesTableController:
                    DoVtcAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.TypesController:
                    DoTcAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.PrintOperation:
                    DoPrintAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.AriphmeticalOperation:
                    DoAoAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.BoolOperation:
                    DoBoAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.Common:
                    DoCommonAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.IfOperation:
                    DoIfAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.WhileOperation:
                    DoWhileAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.ReadOperation:
                    DoReadAction( actionNameData[ 1 ] );
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void DoCommonAction( string actionName )
        {
            const string boolRuleNameSubstring = "boolExpression_0";
            const string aoRuleSubstring = "ariphmeticalOperation_rv";

            switch ( actionName )
            {
                case SourceActionName.CommonCheckRightValueDestination:
                    if ( _typeController.LeftTerm.Type == TermType.Bool )
                    {
                        _tempGoTo = _table.First( t => t.Name.Contains( boolRuleNameSubstring ) ).GoTo;
                    }
                    else if ( _typeController.LeftTerm.Type == TermType.Int )
                    {
                        _tempGoTo = _table.First( t => t.Name.Contains( aoRuleSubstring ) ).GoTo;
                    }
                    break;
                default:
                    break;
            }
        }

        private void DoVtcAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.VtcCreateTable:
                    _variablesTableController.CreateTable();
                    break;
                case SourceActionName.VtcDestroyLastTable:
                    _variablesTableController.DestroyLastTable();
                    break;
                case SourceActionName.VtcDefineNewType:
                    _variablesTableController.DefineNewType( _currentTerm );
                    _aSTGenerator.AddDeclarationNode( _currentTerm );
                    break;
                case SourceActionName.VtcDefineIdentifier:
                    _variablesTableController.DefineIdentifier( _currentTerm );
                    _aSTGenerator.AddDeclarationNode( _currentTerm );
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }

        private void DoTcAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.TcCheckLeftRight:
                    _typeController.CheckLeftRight( _currentTerm.RowPosition );
                    _aSTGenerator.AddEqualityNode();
                    _aSTGenerator.SaveAndClear();
                    _ariphmeticalOperationsController.Clear();
                    break;
                case SourceActionName.TcSaveLeftTerm:
                    Variable variable = _variablesTableController.GetVariable( _currentTerm.Id );
                    if ( variable == null )
                        throw new ApplicationException( $"Variable not declarated on row {_currentTerm.RowPosition}" );
                    _typeController.SaveLeftTerm( variable.Type );
                    _aSTGenerator.AddLeafNode( _currentTerm );
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }

        private void DoPrintAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.PrintSave:
                    if ( _currentTerm.Type == TermType.Identifier )
                    {
                        Variable variable = _variablesTableController.GetVariable( _currentTerm.Id );
                        _aSTGenerator.AddLeafNode( variable.Type );
                        _aSTGenerator.AddLeafNode( variable.Identifier );
                    }
                    else if ( _currentTerm.Type == TermType.Print || _currentTerm.Type == TermType.Println )
                    {
                        _aSTGenerator.AddLeafNode( _currentTerm );
                    }
                    else
                    {
                        var term = _currentTerm.Copy();
                        term.Type = TypeController.ConvertToSimpleType( term.Type );
                        term.Value = term.Type.ToString();
                        _aSTGenerator.AddLeafNode( term );
                        _aSTGenerator.AddLeafNode( _currentTerm );
                    }
                    break;
                case SourceActionName.PrintGenerateNode:
                    _aSTGenerator.AddPrintNode();
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }

        private void DoReadAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.ReadSave:
                    if ( _currentTerm.Type == TermType.Identifier )
                    {
                        Variable variable = _variablesTableController.GetVariable( _currentTerm.Id );
                        if ( variable.Type.Type != TermType.Int && variable.Type.Type != TermType.Bool && variable.Type.Type != TermType.Float )
                            throw new Exception( $"Invalid type to read: {variable.Type.Type.ToString()}" );
                        _aSTGenerator.AddLeafNode( variable.Type );
                        _aSTGenerator.AddLeafNode( variable.Identifier );
                    }
                    else
                    {
                        _aSTGenerator.AddLeafNode( _currentTerm );
                    }
                    break;
                case SourceActionName.ReadGenerateNode:
                    _aSTGenerator.AddReadNode();
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }

        private void DoAoAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.AoActionAfterNumber:
                    if ( _currentTerm.Type == TermType.Identifier )
                    {
                        var variable = _variablesTableController.GetVariable( _currentTerm.Id );
                        _ariphmeticalOperationsController.AddNewVariable( variable, _currentTerm );
                        _typeController.SaveRightType( variable.Type );
                    }
                    else
                    {
                        _ariphmeticalOperationsController.AddNewNumber( _currentTerm );
                        _typeController.SaveRightType( _currentTerm );
                    }

                    _aSTGenerator.CreateLeafNode( _currentTerm );
                    break;
                case SourceActionName.AoActionAfterSign:
                    _aSTGenerator.AddSign( _currentTerm );
                    break;
                case SourceActionName.AoActionAfterOperation:
                    _aSTGenerator.CreateUnaryMinusNode();
                    _aSTGenerator.CreateOperationNode( _currentTerm );
                    break;
                case SourceActionName.AoClear:
                    _ariphmeticalOperationsController.Clear();
                    _aSTGenerator.SaveAndClear();
                    break;
                case SourceActionName.AoCreateUnaryMinusNode:
                    _aSTGenerator.CreateUnaryMinusNode();
                    break;
                case SourceActionName.UnaryMinusFound:
                    _aSTGenerator.UnaryMinusFound();
                    break;
                case SourceActionName.AoOpenBracketFound:
                    _aSTGenerator.OpenBracketFound();
                    break;
                case SourceActionName.AoClosedBracketFound:
                    _aSTGenerator.CloseBracketFound();
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }

        private void DoBoAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.BoActionAfterBoolValue:
                    Term term;
                    if ( _currentTerm.Type == TermType.Identifier )
                    {
                        var variable = _variablesTableController.GetVariable( _currentTerm.Id );
                        term = variable.Type;
                    }
                    else
                    {
                        term = _currentTerm;
                    }
                    var type = TypeController.ConvertToSimpleType( term.Type );
                    if ( type == TermType.Bool || type == TermType.Int || type == TermType.Double )
                    {
                        _typeController.SaveRightType( _typeController.LeftTerm );
                    }
                    else
                    {
                        _typeController.SaveRightType( term );
                    }


                    _aSTGenerator.CreateLeafNode( _currentTerm );
                    break;
                case SourceActionName.BoActionAfterCompOperation:
                    _aSTGenerator.CreateNotSignNode();
                    _aSTGenerator.CreateBoolOperationNode( _currentTerm );
                    break;
                case SourceActionName.BoActionAfterCompSign:
                    _aSTGenerator.AddSign( _currentTerm );
                    break;
                case SourceActionName.BoActionAfterLogicSign:
                    _aSTGenerator.CreateNotSignNode();
                    _aSTGenerator.CreateBoolOperationNode( _currentTerm );
                    _aSTGenerator.AddSign( _currentTerm );
                    break;
                case SourceActionName.BoUnaryNotSignFound:
                    _aSTGenerator.UnaryNotSignFound();
                    break;
                case SourceActionName.BoClosedBracketFound:
                    _aSTGenerator.RemoveSign( _currentTerm );
                    _aSTGenerator.CloseBracketInBoolOpFound();
                    break;
                case SourceActionName.BoOpenBracketFound:
                    _aSTGenerator.AddSign( _currentTerm );
                    _aSTGenerator.OpenBracketInBoolOpFound();
                    break;
                default:
                    throw new NotImplementedException( $"action: { actionName } not found" );
            }
        }

        private void DoIfAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.IFAddIfTerm:
                    _aSTGenerator.AddIfNode();
                    _aSTGenerator.SaveAndClear();
                    break;
                case SourceActionName.IFAddThenEnd:
                    _aSTGenerator.AddNode( new TreeNode( NodeType.IfThenEnd, TermType.If, new List<IASTNode>() ) );
                    _aSTGenerator.SaveAndClear();
                    break;
                case SourceActionName.IFAddElseBegin:
                    _aSTGenerator.AddNode( new TreeNode( NodeType.IfElseBegin, TermType.If, new List<IASTNode>() ) );
                    _aSTGenerator.SaveAndClear();
                    break;
                case SourceActionName.IFAddElseEnd:
                    _aSTGenerator.AddNode( new TreeNode( NodeType.IfElseEnd, TermType.If, new List<IASTNode>() ) );
                    _aSTGenerator.SaveAndClear();
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }

        private void DoWhileAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.WhileAddWhile:
                    _aSTGenerator.AddWhileNode();
                    _aSTGenerator.SaveAndClear();
                    break;
                case SourceActionName.WhileAddWhileEnd:
                    _aSTGenerator.AddNode( new TreeNode( NodeType.WhileEnd, TermType.While, new List<IASTNode>() ) );
                    _aSTGenerator.SaveAndClear();
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }
    }
}
