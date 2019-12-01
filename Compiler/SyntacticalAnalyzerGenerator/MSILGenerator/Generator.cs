using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions;
using System.Collections.Generic;
using System.IO;

namespace SyntacticalAnalyzerGenerator.MSILGenerator
{
    public class Generator
    {
        private string ResultPath { get; set; }
        private string ProgramFileName { get; set; }

        public void Generate( List<IMSILConstruction> constructions )
        {
            CreateDirectory();

            using ( FileStream fstream = new FileStream( $"{ResultPath}\\{ProgramFileName}.il", FileMode.OpenOrCreate ) )
            {
                foreach ( var construction in constructions )
                {
                    byte [] array = System.Text.Encoding.Default.GetBytes( construction.ToMSILCode() );
                    fstream.Write( array, 0, array.Length );
                }
            }
        }

        private void CreateDirectory()
        {
            if ( string.IsNullOrEmpty( ResultPath ) )
            {
                ResultPath = "Results";
            }

            if ( string.IsNullOrEmpty( ProgramFileName ) )
            {
                ProgramFileName = "Result";
            }

            DirectoryInfo dirInfo = new DirectoryInfo( $"{ResultPath}" );
            if ( !dirInfo.Exists )
            {
                dirInfo.Create();
            }
            if ( File.Exists( $"{ResultPath}\\{ProgramFileName}.il" ) )
            {
                File.Delete( $"{ResultPath}\\{ProgramFileName}.il" );
            }
        }
    }
}
