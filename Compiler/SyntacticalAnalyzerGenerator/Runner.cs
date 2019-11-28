using Lekser;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;
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
        private IASTNode _tree;
        private bool _isIncorrectTerms = false;

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

        public async Task<IASTNode> GetTree()
        {
            if ( _tree != null )
                return _tree;
            if ( _isIncorrectTerms )
                return null;

            _currentTableIndex = 0;
            _currentTerm = await _programLekser.GetTermAsync();

            bool result = await CheckWordsAsync( _table );
            if ( result )
            {
                _tree = GetGeneratedAST();
                return _tree;
            }
            else
            {
                _isIncorrectTerms = true;
                return null;
            }
        }

        public IASTNode GetGeneratedAST()
        {
            if ( _aSTGenerator.IsSuccessfulyCreated() )
            {
                return _aSTGenerator.RootNode;
            }

            throw new ApplicationException( $"root nodes more or less than 1!" );
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
                    _currentTableIndex = table[ _currentTableIndex ].GoTo;
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
                return false;
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
                case ActionSourceType.Common:
                    DoCommonAction( actionNameData[ 1 ] );
                    break;
                case ActionSourceType.AriphmeticalOperation:
                    DoAoAction( actionNameData[ 1 ] );
                    break;
                default:
                    throw new NotImplementedException();
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
                    break;
                case SourceActionName.VtcDefineIdentifier:
                    _variablesTableController.DefineIdentifier( _currentTerm );
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }

        private void DoTcAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.TcSaveLastTerm:
                    _typeController.SaveLastTerm( _currentTerm );
                    break;
                case SourceActionName.TcCheckLeftRight:
                    Term term = null;
                    if ( _typeController.LastTerm.Type == TermType.Identifier )
                    {
                        term = _variablesTableController.GetVariable( _typeController.LastTerm.Id ).Type;
                    }
                    else
                    {
                        term = _typeController.LastTerm;
                    }
                    _typeController.SaveRightTerm( term );
                    _typeController.CheckLeftRight( _currentTerm.RowPosition );
                    break;
                case SourceActionName.TcSaveLeftTerm:
                    Variable variable = _variablesTableController.GetVariable( _typeController.LastTerm.Id );
                    if ( variable == null )
                        throw new ApplicationException( $"Variable not declated on row {_typeController.LastTerm.RowPosition}" );
                    _typeController.SaveLeftTerm( variable.Type );
                    break;
                case SourceActionName.TcDefineArrElemType:
                    variable = _variablesTableController.GetVariable( _typeController.LastTerm.Id );
                    _typeController.DefineArrElemType( variable.Type );
                    break;
                default:
                    throw new NotImplementedException( $"action: {actionName} not found" );
            }
        }

        private void DoCommonAction( string actionName )
        {
            switch ( actionName )
            {
                case SourceActionName.CommonIsIntIndex:
                    Variable variable = _variablesTableController.GetVariable( _currentTerm.Id );
                    if ( variable.Type.Type != TermType.Int )
                        throw new ApplicationException( $"Index must be int on row { _currentTerm.RowPosition }" );
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
                    }
                    else
                    {
                        _ariphmeticalOperationsController.AddNewNumber( _currentTerm );
                    }

                    _aSTGenerator.CreateLeafNode( _currentTerm );
                    //_aSTGenerator.CreateOperationNode( _currentTerm );
                    break;
                case SourceActionName.AoActionAfterSign:
                    _aSTGenerator.AddSign( _currentTerm );
                    break;
                case SourceActionName.AoActionAfterOperation:
                    _aSTGenerator.CreateOperationNode( _currentTerm );
                    break;
                case SourceActionName.AoClear:
                    _ariphmeticalOperationsController.Clear();
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
    }
}
