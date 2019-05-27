using System;
using System.Collections.Generic;
using System.Text;

namespace GuideSetsDeterminant.Creator
{
    public sealed class Sentence
    {
        public string MainToken { get; set; }
        public List<string> Tokens { get; set; }
        public List<string> ForwardSet { get; private set; }

        public Sentence( string main, List<string> tokens )
        {
            ForwardSet = new List<string>();
            Tokens = tokens;
            MainToken = main;
        }

        public Sentence( string main, string[] tokens )
        {
            Tokens = new List<string>( tokens );
            MainToken = main;
        }

        public void AddInSet( string token )
        {
            if ( !ForwardSet.Contains( token ) )
            {
                ForwardSet.Add( token );
            }
        }

        public void AddInSet( List<string> tokens )
        {
            foreach ( var token in tokens )
            {
                AddInSet( token );
            }
        }
    }
}
