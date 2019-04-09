using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lekser.Enums;

namespace Lekser
{
    /*
    public class ProgramLekserCopy
    {
        private int _linePosition;
        private int _columnPosition;

        public async Task<List<Term>> GetTermsAsync( TextReader textReader )
        {
            NullifyPosition();
            var result = new List<Term>();

            string tempFileName = "qwwqq.txt";
            using ( TextWriter tw = new StreamWriter( new FileStream( tempFileName, FileMode.Create ) ) )
            {
                await WriteWithoutCommentsAsync( textReader, tw );
                textReader.Close();
            }
            textReader = new StreamReader( tempFileName );

            string line = await textReader.ReadLineAsync();
            while ( line != null )
            {
                Task<string> lineTask = textReader.ReadLineAsync();
                List<Term> terms = GetTermsByLine( line );
                result.AddRange( terms );

                line = await lineTask;
                _linePosition++;
            }

            textReader.Close();
            return result;
        }

        private List<Term> GetTermsByLine( string line )
        {
            var result = new List<Term>();

            List<string> words = SplitToStringTerms( line );

            foreach ( string word in words )
            {
                Term term = GetTermByWord( word );
                if ( term != null )
                {
                    result.Add( term );
                }
            }

            return result;
        }


        private async Task WriteWithoutCommentsAsync( TextReader from, TextWriter to )
        {
            string line = await from.ReadLineAsync();
            CommentState commitState = CommentState.NormalText;
            var resultLineBuilder = new StringBuilder();
            while ( line != null )
            {
                Task<string> lineTask = from.ReadLineAsync();
                resultLineBuilder.Clear();
                for ( int i = 0; i < line.Length; i++ )
                {
                    var letter = line[ i ];
                    var stateKey = KeyValuePair.Create( commitState, letter );
                    if ( TermRecognizer.CommentStateMachine.ContainsKey( stateKey ) )
                    {
                        commitState = TermRecognizer.CommentStateMachine[ stateKey ];
                    }
                    else
                    {
                        if ( commitState == CommentState.OneLineCommit )
                        {
                            break;
                        }
                        if ( commitState == CommentState.CommitBegining )
                        {
                            commitState = CommentState.NormalText;
                            resultLineBuilder.Append( '/' );
                        }
                        if ( commitState == CommentState.CommitEnding )
                        {
                            commitState = CommentState.NormalText;
                            resultLineBuilder.Append( '*' );
                        }
                        if ( commitState == CommentState.CommitEnd )
                        {
                            commitState = CommentState.NormalText;
                        }
                        if ( commitState == CommentState.NormalText )
                        {
                            resultLineBuilder.Append( letter );
                        }
                    }

                }
                resultLineBuilder.AppendLine();
                Task toFileWriting = to.WriteAsync( resultLineBuilder.ToString() );
                line = await lineTask;
                await toFileWriting;
            }
        }

        private Term GetTermByWord( string word )
        {
            if ( word.Length == 0 )
            {
                throw new ArgumentException( "term cant be empty" );
            }

            if ( word == " " )
            {
                _columnPosition++;
                return null;
            }

            int wordStart = _columnPosition;
            if ( word.Length == 1 )
            {
                if ( !TermRecognizer.TermByString.ContainsKey( word ) )
                {
                    Console.WriteLine( $"Error on line {_linePosition}, column {wordStart}" );
                    _columnPosition++;

                    return null;
                }

                return new Term( 1111, TermRecognizer.TermByString[ word ], _linePosition, _columnPosition );
            }

            if ( TermRecognizer.IsEnglishLetter( word[ 0 ] ) && TermRecognizer.IsIdentifier( word ) )
            {
                return new Term( 1111, TermRecognizer.TermByString[ word ], _linePosition, _columnPosition );
            }

            return null;
        }

        private List<string> SplitToStringTerms( string line )
        {
            var delimeters = new HashSet<char> { '(', ')', '{', '}', ';', ' ' };

            var result = new List<string>();
            var wordBuilder = new StringBuilder();
            string word = "";
            for ( int i = 0; i < line.Length; i++ )
            {
                char letter = line[ i ];
                if ( delimeters.Contains( letter ) )
                {
                    word = wordBuilder.ToString();

                    if ( word.Length != 0 )
                    {
                        result.Add( word );
                        wordBuilder.Clear();
                    }

                    result.Add( letter.ToString() );
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
                result.Add( word );
            }

            return result;
        }

        private void NullifyPosition()
        {
            _columnPosition = 1;
            _linePosition = 1;
        }
    }
    */
}
