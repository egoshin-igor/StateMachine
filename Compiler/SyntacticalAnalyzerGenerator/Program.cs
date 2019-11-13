﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lekser;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax;
using SyntacticalAnalyzerGenerator.Utils;
using SyntacticalAnalyzerGenerator.Words;

namespace SyntacticalAnalyzerGenerator
{
    class Program
    {
        private const string PathToLangFiles = "../../../LangFiles";
        private const string LangFileName = PathToLangFiles + "/lang.txt";
        private const string LlOneLangFileName = PathToLangFiles + "/llOneLang.txt";

        static void Main( string[] args )
        {
            try
            {
                RunAsync( args ).Wait();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex.Message );
            }
        }

        private static async Task RunAsync( string[] args )
        {
            if ( args.Length > 0 && args[ 0 ] == "GenerateLlOneTable" )
            {
                LangParser.ConvertToLlOneTable( LangFileName, LlOneLangFileName );
                Console.WriteLine( "Ll one table generated" );
                return;
            }

            List<Expression> expressions = LangParser.Parse( LlOneLangFileName );
            var generator = new SyntacticalAnalyzerGenerator( expressions, expressions.First().NoTerm.Name );
            List<ResultTableRow> rows = generator.Generate();

            using ( TextWriter tw = new StreamWriter( $"{PathToLangFiles}/table.html" ) )
            {
                LlTableToHtmlVisualizer.Write( tw, rows );
            }

            ProgramLekser programLekser;
            using ( TextReader tr = new StreamReader( $"{PathToLangFiles}/input.txt" ) )
            {
                programLekser = new ProgramLekser( tr );
                var runner = new Runner(
                    programLekser,
                    new VariablesTableController(),
                    new TypeController(),
                    new AriphmeticalOperationsController(),
                    rows
                );

                InsertActionsInSyntax.ASTNodes.IASTNode astTree = await runner.GetTree();
                await AstTreeVisualizer.VisualizeAsync( astTree, $"{PathToLangFiles}/astTree.dot" );
                Console.WriteLine( astTree != null ? "Success" : "Error" );
            }

            using ( TextWriter tw = new StreamWriter( $"{PathToLangFiles}/table.txt" ) )
            {
                foreach ( ResultTableRow row in rows )
                {
                    tw.WriteLine( row.ToString() );
                }
            }
        }
    }
}
