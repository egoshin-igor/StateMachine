using Lekser;
using Lekser.Enums;
using SLR.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLR.Runner
{
    public class Runner
    {
        private ILekser _lekser;
        private TableOfFirsts _tableOfFirsts;
        private List<Sentence> _sentences;
        private List<InputWord> _inputWords;
        private List<int> _rowIndexStack;
        private List<Cell> _rowStack;
        private List<InputWord> _inputWordStack;

        public Runner( ILekser lekser, TableOfFirsts tableOfFirsts, List<Sentence> sentences )
        {
            _lekser = lekser;
            _tableOfFirsts = tableOfFirsts;
            _sentences = sentences;
            _inputWords = new List<InputWord>();
            _rowIndexStack = new List<int> { 0 };
            _inputWordStack = new List<InputWord>();
            _rowStack = new List<Cell> { _tableOfFirsts.Row [ 0 ] };
        }

        public async Task<bool> IsCorrectSentence()
        {
            try
            {
                await FillInputWords();

                return CheckSentence();
            }
            catch
            {
                return false;
            }
        }

        private async Task FillInputWords()
        {
            var currentTerm = await _lekser.GetTermAsync();
            while ( currentTerm != null )
            {
                _inputWords.Add( new InputWord
                {
                    TermType = currentTerm.Type,
                    Value = null
                } );
                currentTerm = await _lekser.GetTermAsync();
            }
            _inputWords.Reverse();
        }

        private bool CheckSentence()
        {
            Cell currentCell;
            while ( !( _inputWords.Count == 1 && _inputWords [ 0 ].Value == SpecialWords.Start ) && _inputWords.Count > 0 )
            {
                DisplaceIfPossible();
                var currentInputWord = _inputWords [ _inputWords.Count - 1 ];
                _inputWords.RemoveAt( _inputWords.Count - 1 );
                int rowIndex = GetRowIndex();
                int columnIndex = GetColumnIndex( currentInputWord );
                currentCell = _tableOfFirsts.Table [ rowIndex ] [ columnIndex ];
                _rowStack.Add( currentCell );
                _inputWordStack.Add( currentInputWord );
                DisplaceIfPossible();

            }
            if ( _inputWords.Count == 1 && _rowStack.Count == 1 && _rowStack [ 0 ].Value == SpecialWords.Start )
            {
                return true;
            }
            return false;
        }

        private int GetRowIndex()
        {
            Cell cell = _rowStack [ _rowStack.Count - 1 ];
            return _tableOfFirsts.Row.FindIndex( t =>
            {
                if ( t.Values.Count != cell.Values.Count )
                    return false;
                List<Token> orderedCell = cell.Values.OrderBy( c => c.Value ).ToList();
                List<Token> orderedRow = t.Values.OrderBy( x => x.Value ).ToList();
                for ( int i = 0; i < orderedCell.Count; i++ )
                {
                    if ( orderedCell [ i ].Value != orderedRow [ i ].Value )
                        return false;
                    if ( orderedCell [ i ].ColIndex != orderedRow [ i ].ColIndex )
                        return false;
                    if ( orderedCell [ i ].RowIndex != orderedRow [ i ].RowIndex )
                        return false;
                }

                return true;
            } );
        }

        private int GetColumnIndex( InputWord inputWord )
        {
            if ( inputWord.TermType == null )
            {
                 return _tableOfFirsts.TokensColumn.FindIndex( tc => tc.Value == inputWord.Value );
            }
            else
            {
                return _tableOfFirsts.TokensColumn.FindIndex( tc => tc.LekserTermType == inputWord.TermType );
            }
            /*  var arr = _tableOfFirsts.Column.Select( item => TermRecognizer.GetTypeByTermString( item ) ).ToList();
            string str = "";
            foreach ( var item in _tableOfFirsts.Row )
            {
                if ( item.Values.FirstOrDefault( it => it.LekserTermType == inputWord.TermType ) != null)
                {
                    str = item.Values.FirstOrDefault( it => it.LekserTermType == inputWord.TermType ).Value;
                }
            }
            return inputWord.Value == null ? _tableOfFirsts.Column.IndexOf( str ) : _tableOfFirsts.Column.IndexOf( inputWord.Value );*/
        }

        private void DisplaceIfPossible()
        {
            // var currentInputWord = _inputWordStack [ _inputWordStack.Count - 1 ];
            var currentInputWord = _inputWords.Count == 0 ? new InputWord { TermType = TermType.End } : _inputWords [ _inputWords.Count - 1 ];
            var currentRow = _rowStack [ _rowStack.Count - 1 ];
            int rowIndex = GetRowIndex();
            int columnIndex = GetColumnIndex( currentInputWord );
            var currentCell = _tableOfFirsts.Table [ rowIndex ] [ columnIndex ];

            if (  currentCell.Values [ 0 ].Type != TokenType.Rule )
            {
                return;
            }

            Token token = _rowStack [ _rowStack.Count - 1 ].Values [ 0 ];
            int elementsCount = _rowStack [ _rowStack.Count - 1 ].Values [ 0 ].ColIndex + 1;
            int sentenceIterator = _sentences [ token.RowIndex ].Tokens.Count - 1;
            for ( int i = _rowStack.Count - 1; i >= _rowStack.Count - elementsCount; i-- )
            {
                if ( token.RowIndex == 0 )
                {
                    if ( _sentences [ token.RowIndex ].Tokens [ sentenceIterator - 1 ].Value != _rowStack [ i ].Value )
                        throw new ApplicationException( "False" );
                }
                else if ( _sentences [ token.RowIndex].Tokens[sentenceIterator].Value != _rowStack[i].Value)
                    throw new ApplicationException( "False" );
                sentenceIterator--;
            }
            
            var rule = _sentences [ token.RowIndex ];
            for ( int i = 0; i < elementsCount; i++ )
            {
                _rowStack.RemoveAt( _rowStack.Count - 1 );
                _inputWordStack.RemoveAt( _inputWordStack.Count - 1 );
            }
            _inputWords.Add( new InputWord
            {
                Value = rule.MainToken
            } );
        }
    }
}
