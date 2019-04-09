using System;
using System.IO;
using System.Threading.Tasks;

namespace Lekser
{
    class Program
    {
        static async Task Main( string[] args )
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
                ILekser programLekser = new ProgramLekser( tw );
                await PrintAllTermsAsync( programLekser );
            }
        }

        private static async Task PrintAllTermsAsync( ILekser programLekser )
        {
            Term term = null;
            do
            {
                term = await programLekser.GetTermAsync();
                if ( term != null )
                {
                    Console.Write( $"Id:{term.Id}, Type:{term.Type}, row/column:{term.RowPosition}:{term.ColumnPosition}" );
                    if ( term.NumberString != null )
                    {
                        Console.Write( $" number:{term.NumberString}" );
                    }
                    Console.WriteLine();
                }
            } while ( term != null );
        }
    }


}
