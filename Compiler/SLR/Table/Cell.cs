using System;
using System.Collections.Generic;
using System.Text;

namespace SLR.Table
{
    public sealed class Cell
    {
        public List<Token> Values { get; } = new List<Token>();

        /// <summary>
        /// get value of the cell by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Token this[ int index ]
        {
            get => Values[ index ];
        }

        /// <summary>
        /// value of the cell
        /// </summary>
        public string Value => Values.Count != 0 ? Values[ 0 ].Value : "";

        public void AddValue( Token token )
        {
            if ( !Values.Contains( token ) )
            {
                Values.Add( token );
            }
        }

        public Cell( Token token )
        {
            Values.Add( token );
        }

        public Cell( Cell cell )
        {
            foreach ( var value in cell.Values )
            {
                Values.Add( new Token( value.Value, value.ColIndex, value.RowIndex ) );
            }
        }

        public Cell( List<Token> tokens )
        {
            Values.AddRange( tokens );
        }
    }
}
