using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lekser;
using SLR.Runner;
using SLR.Table;
using SLR.Utils;

namespace SLR
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {
                Run( args );
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
            var lexer = new ProgramLekser( new StreamReader( "E:\\GITHUB\\SLR\\Новая папка\\StateMachine\\Compiler\\SLR\\input.txt" ) );
            var runner = new Runner.Runner( lexer, creator.TableOfFirsts, creator.Sentences );
            Console.WriteLine( runner.IsCorrectSentence().Result );

            using ( var writer = new StreamWriter( "../../../1.html" ) )
            {
                SlrTableToHtmlVisualizer.Write( writer, creator );
            }
        }
    }
}
