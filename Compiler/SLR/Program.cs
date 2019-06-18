using System;
using System.IO;
using System.Text;
using Lekser;
using SLR.Table;
using SLR.Utils;

namespace SLR
{
    class Program
    {
        static void Main( string[] args )
        {
            Run( args );
            try
            {
                
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.Message );
            }
        }

        private static void Run( string[] args )
        {
            SentencesReader reader = null;
            using ( var streamReader = new StreamReader( args[ 0 ], Encoding.Default ) )
            {
                reader = new SentencesReader( streamReader );
            }
            if ( !reader.Sentences.IsValid() )
                throw new ApplicationException( "Cycles exist" );

            FirstCreator creator = new FirstCreator( reader.Sentences );
            var lexer = new ProgramLekser( new StreamReader( "../../../input.txt" ) );
            var runner = new Runner.Runner( lexer, creator.TableOfFirsts, creator.Sentences );
            Console.WriteLine( runner.IsCorrectSentence().Result );

            using ( var writer = new StreamWriter( "../../../1.html" ) )
            {
                SlrTableToHtmlVisualizer.Write( writer, creator );
            }
        }
    }
}
