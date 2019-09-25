using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lekser;
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
                RunAsync().Wait();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.Message );
            }
        }

        private static async Task RunAsync()
        {
            List<Expression> expressions = LangParser.Parse( LangFileName );
            var generator = new SyntacticalAnalyzerGenerator( expressions, expressions.First().NoTerm.Name );
            List<ResultTableRow> rows = generator.Generate();

            ProgramLekser programLekser;
            using ( TextReader tr = new StreamReader( "../../../input.txt" ) )
            {
                programLekser = new ProgramLekser( tr );
                var runner = new Runner( programLekser );
                var result = await runner.IsCorrectSentenceAsync( rows );
                Console.WriteLine( result );
            }

            using ( TextWriter tw = new StreamWriter( "../../../table.txt" ) )
            {
                foreach ( ResultTableRow row in rows )
                {
                    tw.WriteLine( row.ToString() );
                }
            }
        }
    }
}
