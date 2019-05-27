using System;
using System.Collections.Generic;
using System.IO;
using GuideSetsDeterminant.Creator;

namespace GuideSetsDeterminant.Utils
{
    public sealed class SentencesReader
    {
        public List<Sentence> Sentences { get; private set; }

        public SentencesReader( StreamReader input )
        {
            Sentences = new List<Sentence>();

            while ( !input.EndOfStream )
            {
                ParseStringToSentense( input.ReadLine() );
            }
        }

        private void ParseStringToSentense( string str )
        {
            var parsedStr = str.Split( ' ' );
            var mainToken = parsedStr[ 0 ];
            var list = new List<string>();
            for ( var i = 2; i < parsedStr.Length; ++i )
            {
                var s = parsedStr[ i ];
                if ( s != "" )
                {
                    list.Add( parsedStr[ i ] );
                }
            }

            Sentences.Add( new Sentence( mainToken, list ) );
        }
    }
}
