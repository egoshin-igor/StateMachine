using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SyntacticalAnalyzerGenerator.Words;

namespace SyntacticalAnalyzerGenerator
{
    class Program
    {
        private const string LangFileName = "../../../lang.txt";

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
            List<Expression> expressions = LangParser.Parse( LangFileName );
            var generator = new SyntacticalAnalyzerGenerator( expressions, expressions.First().NoTerm.Name );
            List<ResultTableRow> rows = generator.Generate();

            var words = new List<Word>();
            using ( var sw = new StreamReader( "../../../in.txt" ) )
            {
                words = sw.ReadLine().Split( " " ).Select( s => new Word { Name = s } ).ToList();
            }

            var runner = new Runner();
            var result = runner.IsCorrectSentence( rows, words );
            Console.WriteLine( result );
        }


    }
}
