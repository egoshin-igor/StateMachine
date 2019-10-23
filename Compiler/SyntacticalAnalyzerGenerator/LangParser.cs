using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GuideSetsDeterminant.Creator;
using GuideSetsDeterminant.Utils;
using Lekser;
using Lekser.Enums;
using SyntacticalAnalyzerGenerator.Words;

namespace SyntacticalAnalyzerGenerator
{
    public static class LangParser
    {
        private const string NameActionPairSplitString = "~";

        private static readonly Dictionary<string, TermType> _reservedTermByName = new Dictionary<string, TermType>
        {
            { Word.Identifier, TermType.Identifier },
            { Word.DecimalWholeNumber, TermType.DecimalWholeNumber }
        };

        public static void ConvertToLlOneTable( string sourceLangPath, string llOneTablePath )
        {
            string tempFileName = Path.GetTempFileName();

            var result = new List<Expression>();
            List<Expression> expressions = new List<Expression>();
            using ( StreamReader streamReader = new StreamReader( sourceLangPath, Encoding.Default ) )
            {
                while ( !streamReader.EndOfStream )
                {
                    string line = streamReader.ReadLine();
                    if ( line != "" )
                    {
                        expressions.Add( ParseToPartExpression( line ) );
                    }
                }
            }
            using ( var sw = new StreamWriter( tempFileName ) )
            {
                List<Expression> llOneExpressions = LLOneConverter.Convert( expressions );
                foreach ( var llOneExpression in llOneExpressions )
                {
                    sw.WriteLine( llOneExpression.ToStringWithoutSet() );
                }
            }

            using ( StreamReader streamReader = new StreamReader( tempFileName, Encoding.Default ) )
            {
                var reader = new SentencesReader( streamReader );
                GuideSetCreator creator = new GuideSetCreator( reader.Sentences );
                using ( var sw = new StreamWriter( llOneTablePath ) ) { creator.WriteResultToStream( sw ); }
            }
        }

        public static List<Expression> Parse( string llOneTablePath )
        {
            List<Expression> result = GetNotReadMadeExpressions( llOneTablePath );
            Dictionary<string, HashSet<TermType>> directingSetByName = result
                .GroupBy( r => r.NoTerm.Name )
                .ToDictionary( g => g.Key, g => g.ToList().SelectMany( l => l.NoTerm.DirectingSet ).ToHashSet() );
            foreach ( Expression expression in result )
            {
                foreach ( Word word in expression.Words )
                {
                    if ( word.Type == WordType.RightNoTerm )
                    {
                        word.DirectingSet = directingSetByName[ word.Name ];
                    }
                }
            }

            var superTerm = new Word
            {
                Name = "SuperMegaNoTerm",
                DirectingSet = directingSetByName[ result[ 0 ].NoTerm.Name ],
                Type = WordType.LeftNoTerm
            };

            var expr = new Expression
            {
                NoTerm = superTerm,
                Words = new List<Word>
                {
                    new Word
                    {
                        DirectingSet = directingSetByName[ result[ 0 ].NoTerm.Name ],
                        Name = result[0].NoTerm.Name,
                        Type = WordType.RightNoTerm
                    },
                    new Word
                    {
                        DirectingSet = new HashSet<TermType>(),
                        Name = Word.End,
                        Type = WordType.EndOfLang
                    }
                }
            };
            result.Insert( 0, expr );

            return result;
        }

        private static List<Expression> GetNotReadMadeExpressions( string langFileName )
        {
            var result = new List<Expression>();

            using ( StreamReader sr = new StreamReader( langFileName ) )
            {
                while ( !sr.EndOfStream )
                {
                    string line = sr.ReadLine();
                    if ( line != "" )
                    {
                        result.Add( ParseToExpression( line ) );
                    }
                }
            }

            return result;
        }

        private static Expression ParseToExpression( string str )
        {
            string[] splited = str.Split( "/" );
            string[] mainAndOthers = splited[ 0 ].Split( "->" );
            var mainDirectingSet = new HashSet<TermType>(
                splited[ 1 ]
                    .Split( "," )
                    .Select( s => s.Trim() )
                    .Select( s =>
                    {
                        if ( string.IsNullOrWhiteSpace( s ) )
                            return TermType.End;
                        if ( _reservedTermByName.ContainsKey( s ) )
                            return _reservedTermByName[ s ];

                        return TermRecognizer.GetTypeByTermString( s );
                    } )
            );
            var mainWord = new Word
            {
                DirectingSet = mainDirectingSet,
                Name = mainAndOthers[ 0 ].Trim(),
                Type = WordType.LeftNoTerm
            };
            List<string> others = mainAndOthers[ 1 ].Split( " " ).Where( o => o != "" && o != " " ).ToList();
            var words = new List<Word>();
            foreach ( string other in others )
            {
                var trimmedOther = other.Trim();
                string[] nameActionPair = trimmedOther.Split( "~" );
                string wordName = nameActionPair[ 0 ];
                string actionName = nameActionPair.Length > 1 ? nameActionPair[ 1 ] : "";
                var word = new Word { Name = wordName, ActionName = actionName };
                if ( wordName == Word.Epsilant )
                {
                    word.DirectingSet = mainDirectingSet;
                    word.Type = WordType.Epsilant;
                }
                else if ( wordName.Length < 2 || ( wordName[ 0 ] != '<' || wordName[ wordName.Length - 1 ] != '>' ) )
                {
                    TermType termType = _reservedTermByName.ContainsKey( word.Name )
                        ? _reservedTermByName[ word.Name ]
                        : TermRecognizer.GetTypeByTermString( wordName );

                    word.DirectingSet = new HashSet<TermType> { termType };
                    word.Type = WordType.Term;
                    word.TermType = termType;
                }
                else
                {
                    word.Type = WordType.RightNoTerm;
                }

                words.Add( word );
            }

            return new Expression
            {
                NoTerm = mainWord,
                Words = words
            };
        }

        private static Expression ParseToPartExpression( string str )
        {
            string[] mainAndOthers = str.Split( "->" );

            var mainWord = new Word
            {
                Name = mainAndOthers[ 0 ].Trim(),
                Type = WordType.LeftNoTerm
            };
            List<string> others = mainAndOthers[ 1 ].Split( " " ).Where( o => o != "" && o != " " ).ToList();
            var words = new List<Word>();
            foreach ( string other in others )
            {
                var wordName = other.Trim();
                var word = new Word { Name = wordName };
                if ( wordName == Word.Epsilant )
                {
                    word.Type = WordType.Epsilant;
                }
                else if ( wordName.Length < 2 || ( wordName[ 0 ] != '<' || wordName[ wordName.Length - 1 ] != '>' ) )
                {
                    word.Type = WordType.Term;
                    TermType termType = TermRecognizer.GetTypeByTermString( word.Name );
                    if ( !_reservedTermByName.ContainsKey( word.Name ) && ( termType == TermType.Error || _reservedTermByName.Values.Contains( termType ) ) )
                        throw new Exception( $"Unrecognized term {word.Name}" );
                }
                else
                {
                    word.Type = WordType.RightNoTerm;
                }

                words.Add( word );
            }

            return new Expression
            {
                NoTerm = mainWord,
                Words = words
            };
        }
    }
}
