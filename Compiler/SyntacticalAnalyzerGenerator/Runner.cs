using SyntacticalAnalyzerGenerator.Words;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticalAnalyzerGenerator
{
    public class Runner
    {
        private const string END_WORD_NAME = Word.End;

        private int _currentWordIndex;
        private int _currentTableIndex;
        private List<int> _indexStack;

        public Runner()
        {
            _indexStack = new List<int>();
        }

        public bool IsCorrectSentence( List<ResultTableRow> table, List<Word> words )
        {
            _currentTableIndex = 0;
            _currentWordIndex = 0;
            return CheckWords( table, words );
        }

        private bool CheckWords( List<ResultTableRow> table, List<Word> words )
        {
            if ( CanProcessRow( table, words ) )  // проверяем можно ли обрабатывать строку в таблице
            {
                ShiftIfEnabled( table );
                PushToStackIfEnabled( table );
                if ( table[ _currentTableIndex ].GoTo == -1 && _indexStack.Count > 0 )  // переходим по стеку, если нельзя по goto
                {
                    _currentTableIndex = _indexStack.Last();
                    _indexStack.RemoveAt( _indexStack.Count - 1 );
                    return CheckWords( table, words );
                }
                if ( table[ _currentTableIndex ].GoTo != -1 )  // переходим по goto
                {
                    _currentTableIndex = table[ _currentTableIndex ].GoTo;
                    return CheckWords( table, words );
                }
                return _indexStack.Count == 0 && table[ _currentTableIndex ].IsEnd;
            }
            else
            {
                if ( table[ _currentTableIndex ].ShiftOnError != -1 )  // переходим по onError, если возможно и нельзя обработать строку
                {
                    _currentTableIndex = table[ _currentTableIndex ].ShiftOnError;
                    return CheckWords( table, words );
                }
                return false;
            }
        }

        private bool CanProcessRow( List<ResultTableRow> table, List<Word> words )
        {
            var currentWord = _currentWordIndex == words.Count ? END_WORD_NAME : words[ _currentWordIndex ].Name;
            return table[ _currentTableIndex ].DirectingSet.Contains( currentWord ) || ( currentWord == END_WORD_NAME && table[ _currentTableIndex ].DirectingSet.Count == 0 );
        }

        private void ShiftIfEnabled( List<ResultTableRow> table )
        {
            if ( table[ _currentTableIndex ].IsShift )
            {
                _currentWordIndex++;
            }
        }

        private void PushToStackIfEnabled( List<ResultTableRow> table )
        {
            if ( table[ _currentTableIndex ].IsPushToStack )
            {
                _indexStack.Add( _currentTableIndex + 1 );
            }
        }
    }
}
