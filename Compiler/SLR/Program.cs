using System;
using System.Collections.Generic;
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
            SentencesReader reader = null;
            using ( var streamReader = new StreamReader( args[ 0 ], Encoding.Default ) )
            {
                reader = new SentencesReader( streamReader );
            }
            FirstCreator creator = new FirstCreator( reader.Sentences );
            if ( !creator.Sentences.IsValid() )
            {
            }

            using ( var writer = new StreamWriter( "../../../1.html" ) )
            {
                SlrTableToHtmlVisualizer.Write( writer, creator );
            }
        }
    }
}
