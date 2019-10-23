using System.Collections.Generic;
using System.IO;
using System.Text;
using SyntacticalAnalyzerGenerator.Utils;
using SyntacticalAnalyzerGenerator.Words;

namespace SyntacticalAnalyzerGenerator
{
    public static class LlTableToHtmlVisualizer
    {
        private static List<string> _tableNames = new List<string>
        {
            "N", "Name", "Set", "Shift", "OnErr", "Stack", "GoTo", "IsEnd", "ActionName"
        };

        public static void Write( TextWriter writer, List<ResultTableRow> rows )
        {
            var result = GetBeforeBody() + GetBody( rows ) + GetAfterBody();
            writer.Write( result );
        }

        private static string GetBeforeBody()
        {
            var resultBuilder = new StringBuilder();

            resultBuilder.Append( "<html>\n" );
            resultBuilder.Append( "\t<head>\n" );
            resultBuilder.Append( "\t\t<meta charset=\"utf - 8\">\n" );
            resultBuilder.Append( "\t\t<title>Table</title>\n" );
            resultBuilder.Append( "\t</head>\n" );

            return resultBuilder.ToString();
        }

        private static string GetAfterBody()
        {
            var resultBuilder = new StringBuilder();

            resultBuilder.Append( "</html>\n" );

            return resultBuilder.ToString();
        }

        private static string GetBody( List<ResultTableRow> rows )
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.Append( "<body>\n" );
            resultBuilder.Append( "<table cellspacing=\"1\">\n" );

            resultBuilder.Append( "<tr>" );
            foreach ( string item in _tableNames )
            {
                resultBuilder.Append( $"<th style=\"border:1px solid;\">{ item.GetEscaped() }</th>" );
            }
            resultBuilder.Append( "</tr>" );

            foreach ( ResultTableRow row in rows )
            {
                resultBuilder.Append( "<tr>" );
                foreach ( string item in row.GetItems() )
                {
                    resultBuilder.Append( $"<th style=\"border:1px solid;\">{ item.GetEscaped() }</th>" );
                }
                resultBuilder.Append( "</tr>" );
            }

            resultBuilder.Append( '\n' );

            resultBuilder.Append( "</table>\n" );
            resultBuilder.Append( "</body>\n" );
            return resultBuilder.ToString();
        }
    }
}
