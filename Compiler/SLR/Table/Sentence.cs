using System.Collections.Generic;
using System.Linq;

namespace SLR.Table
{
    public sealed class Sentence
    {
        /// <summary>
        /// left side of sentence
        /// </summary>
        public string MainToken { get; set; }

        /// <summary>
        /// right side of sentence
        /// </summary>
        public List<Token> Tokens { get; set; }

        public Sentence( string main, List<Token> tokens )
        {
            Tokens = tokens;
            MainToken = main;
        }

        public Sentence( string main, Token[] tokens )
        {
            Tokens = new List<Token>( tokens );
            MainToken = main;
        }

        public override string ToString()
        {
            return $"{MainToken} -> {TokensToString()}";
        }

        private string TokensToString()
        {
            return string.Join( " ", Tokens.Select( w => w.Value ) );
        }
    }
}
