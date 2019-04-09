using System;
using System.IO;

namespace SyntacticalAnalyzer
{
    class Program
    {
        static void Main( string[] args )
        {
            if ( args.Length != 1 )
            {
                Console.WriteLine( "run so: program.exe <infile>" );
                return;
            }
            if ( !File.Exists( args[ 0 ] ) )
            {
                Console.WriteLine( $"{args[ 0 ]} not exist" );
                return;
            }
            using ( TextReader tw = new StreamReader( args[ 0 ] ) )
            {
                var syntactialAnalyzer = new SyntactialAnalyzer( tw );
                Console.WriteLine( syntactialAnalyzer.IsVar() );
            }
        }
    }
}
