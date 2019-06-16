using System.IO;
using System.Text;
using SLR.Table;

namespace SLR.Utils
{
    public static class SlrTableToHtmlVisualizer
    {
        public static void Write( TextWriter writer, FirstCreator firstCreator )
        {
            var result = GetBeforeBody() + GetBody( firstCreator ) + GetAfterBody();
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

        private static string GetBody( FirstCreator firstCreator )
        {
            var tableOfFirsts = firstCreator.TableOfFirsts;
            var resultBuilder = new StringBuilder();
            resultBuilder.Append( "<body>\n" );
            foreach ( Sentence item in firstCreator.Sentences )
            {
                resultBuilder.Append( "<p>" );
                resultBuilder.Append( item.ToString().GetEscaped() );
                resultBuilder.Append( "</p>" );
            }
            resultBuilder.Append( "<table cellspacing=\"1\">\n" );

            resultBuilder.Append( "<tr>" );
            resultBuilder.Append( $"<th>[     ]</th>" );
            foreach ( var str in tableOfFirsts.Column )
            {
                resultBuilder.Append( $"<th style=\"border:1px solid;\">{ str.GetEscaped() }</th>" );
            }
            resultBuilder.Append( "</tr>" );

            resultBuilder.Append( '\n' );

            for ( var i = 0; i < tableOfFirsts.Row.Count; ++i )
            {
                resultBuilder.Append( "<tr>" );
                resultBuilder.Append( "<td style=\"border:1px solid;\">" );
                foreach ( var x in tableOfFirsts.Row[ i ].Values )
                {
                    resultBuilder.Append( $"{ x.Value.GetEscaped() }<span style=\"font-size: 10px;\">{x.RowIndex},{x.ColIndex}</span> " );
                }
                resultBuilder.Append( "</td>" );
                foreach ( var item in tableOfFirsts.Table[ i ] )
                {
                    resultBuilder.Append( "<td style=\"border:1px solid;\">" );
                    if ( item != null )
                    {
                        foreach ( var token in item.Values )
                        {
                            resultBuilder.Append( $"{ token.Value.GetEscaped() }<span style=\"font-size: 10px;\">{token.RowIndex},{token.ColIndex}</span> " );
                        }
                    }

                    resultBuilder.Append( "</td>" );
                }

                resultBuilder.Append( "</tr>" );
            }

            resultBuilder.Append( "</table>\n" );
            resultBuilder.Append( "</body>\n" );
            return resultBuilder.ToString();
        }
    }
}
