using Lekser;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax;
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
        private Term _currentTerm;

        public Runner( ProgramLekser programLekser, IVariablesTableController variablesTableController )
        {
            _programLekser = programLekser;
            _variablesTableController = variablesTableController;
            _indexStack = new List<int>();
        }

        public async Task<bool> IsCorrectSentenceAsync( List<ResultTableRow> table )
        {
            _currentTableIndex = 0;
            _currentTerm = await _programLekser.GetTermAsync();
            //   _currentWordIndex = 0;
            return await CheckWordsAsync( table );
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
                return _indexStack.Count == 0 && table[ _currentTableIndex ].IsEnd;
            }
            else
            {
                if ( table[ _currentTableIndex ].ShiftOnError != -1 )  // переходим по onError, если возможно и нельзя обработать строку
                {
                    _currentTableIndex = table[ _currentTableIndex ].ShiftOnError;
                    return await CheckWordsAsync( table );
                }
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
                throw new Exception();

            switch ( actionNameData[ 0 ] )
            {
                case ActionSourceType.VariablesTableController:
                    DoVTCAction( actionNameData[ 1 ] );
                    break;
                default:
                    throw new Exception();
            }
        }

        private void DoVTCAction( string actionName )
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
                    throw new Exception( $"action: {actionName} not found" );
            }
        }
    }
}
