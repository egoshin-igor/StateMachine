using System;
using System.IO;
using System.Text;
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
            using ( var streamReader = new StreamReader( args[ 0 ], Encoding.Default ) )
            {
                SentencesReader reader = new SentencesReader( streamReader );
                FirstCreator creator = new FirstCreator( reader.Sentences );
                using ( var writer = new StreamWriter( "../../../1.html" ) )
                {
                    SlrTableToHtmlVisualizer.Write( writer, creator );
                }

                // creator.WriteResult( Console.Out );
            }
        }
    }
}
