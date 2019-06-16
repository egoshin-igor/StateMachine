using System;
using System.Collections.Generic;
using System.Text;

namespace SLR.Table
{
    public sealed class Cell
    {
        private readonly List<Token> _values = new List<Token>();

        /// <summary>
        /// get value of the cell by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Token this[ int index ]
        {
            get => _values[ index ];
            set => _values[ index ] = value;
        }

        /// <summary>
        /// value of the cell
        /// </summary>
        public string Value => _values.Count != 0 ? _values[ 0 ].Value : "";

        public Cell( Token token )
        {
            _values.Add( token );
        }

        public Cell( List<Token> tokens )
        {
            _values.AddRange( tokens );
        }
    }
}
