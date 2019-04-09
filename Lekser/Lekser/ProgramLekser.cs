using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lekser.Enums;

namespace Lekser
{
    public interface ILekser
    {
        Task<Term> GetTermAsync();
    }

    public class ProgramLekser : ILekser
    {
        private TextReader _textReader;

        private int _rowPosition = 1;

        private Dictionary<string, int> _idByTerm = new Dictionary<string, int>();

        private string _line = "";
        private Queue<UndefinedTerm> _undefinedTerms = new Queue<UndefinedTerm>();
        private Task<string> _lineTask;

        public ProgramLekser( TextReader textReader )
        {
            _textReader = textReader;
            _lineTask = _textReader.ReadLineAsync();
        }

        public async Task<Term> GetTermAsync()
        {
            if ( _undefinedTerms != null && !_undefinedTerms.Any() )
            {
                _undefinedTerms = await TryGetAnyTermsAsync();
            }

            if ( _undefinedTerms == null )
                return null;

            UndefinedTerm undefinedTerm = _undefinedTerms.Dequeue();
            TermType definedTermType = TermRecognizer.GetTypeByTermString( undefinedTerm.Id );
            var result = new Term( GetIdByTerm( undefinedTerm.Id ), definedTermType, undefinedTerm.RowPosition, undefinedTerm.ColumnPosition );
            if ( TermRecognizer.IsNumber( definedTermType ) )
            {
                result.SetNumberString( undefinedTerm.Id );
            }

            return result;
        }

        private async Task<Queue<UndefinedTerm>> TryGetAnyTermsAsync()
        {
            do
            {
                _line = await _lineTask;
                _lineTask = _textReader.ReadLineAsync();

                if ( _line == null )
                    return null;

                _undefinedTerms = SplitToUndefinedTerms( _line );
                _rowPosition++;
            } while ( !_undefinedTerms.Any() );

            return _undefinedTerms;
        }

        private Queue<UndefinedTerm> SplitToUndefinedTerms( string line )
        {
            var result = new Queue<UndefinedTerm>();
            var wordBuilder = new StringBuilder();
            string word = "";
            for ( int i = 0; i < line.Length; i++ )
            {
                char letter = line[ i ];
                if ( TermRecognizer.Delimeters.Contains( letter ) )
                {
                    word = wordBuilder.ToString();

                    int wordLength = word.Length;
                    if ( wordLength != 0 )
                    {
                        result.Enqueue( new UndefinedTerm( word, _rowPosition, i + 1 - wordLength ) );
                        wordBuilder.Clear();
                    }

                    if ( letter != ' ' && letter != '\t' )
                    {
                        result.Enqueue( new UndefinedTerm( letter.ToString(), _rowPosition, i + 1 ) );
                    }
                }
                else
                {
                    wordBuilder.Append( letter );
                    word = wordBuilder.ToString();
                }
            }

            word = wordBuilder.ToString();
            if ( word.Length != 0 )
            {
                result.Enqueue( new UndefinedTerm( word, _rowPosition, line.Length + 1 - word.Length ) );
            }

            return result;
        }

        private int GetIdByTerm( string key )
        {
            if ( _idByTerm.ContainsKey( key ) )
                return _idByTerm[ key ];

            int lastId = _idByTerm.Any() ? _idByTerm.Last().Value : 0;
            _idByTerm.Add( key, lastId + 1 );

            return lastId + 1;
        }
    }
}
