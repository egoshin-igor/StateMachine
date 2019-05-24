using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SyntacticalAnalyzerGenerator.Words;

namespace SyntacticalAnalyzerGenerator
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {
                Run();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.Message );
            }
        }

        private static void Run()
        {
            List<Expression> expressions = GetReadiedExpressions();
            var generator = new SyntacticalAnalyzerGenerator( expressions, expressions.First().NoTerm.Name );
            List<ResultTableRow> rows = generator.Generate();
            using ( var sw = new StreamWriter( "../../../out.txt" ) )
            {
                foreach ( ResultTableRow row in rows )
                {
                    sw.WriteLine( row.ToString() );
                }
            }
        }

        private static List<Expression> GetReadiedExpressions()
        {
            List<Expression> result = GetNotReadiedExpressions();
            Dictionary<string, HashSet<string>> directingSetByName = result
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
                        DirectingSet = new HashSet<string>(),
                        Name = "[end]",
                        Type = WordType.EndOfLang
                    }
                }
            };
            result.Insert( 0, expr );

            return result;
        }

        private static List<Expression> GetNotReadiedExpressions()
        {
            var result = new List<Expression>();
            using ( StreamReader sr = new StreamReader( "in.txt" ) )
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
            HashSet<string> mainDirectingSet = new HashSet<string>( splited[ 1 ].Split( "," ).Select( s => s.Trim() ) );

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
                var word = new Word { Name = trimmedOther };
                if ( trimmedOther == "e" )
                {
                    word.DirectingSet = mainDirectingSet;
                    word.Type = WordType.Epsilant;
                }
                else if ( trimmedOther.Length < 2 || ( trimmedOther[ 0 ] != '<' || trimmedOther[ trimmedOther.Length - 1 ] != '>' ) )
                {
                    word.DirectingSet = new HashSet<string> { trimmedOther };
                    word.Type = WordType.Term;
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
