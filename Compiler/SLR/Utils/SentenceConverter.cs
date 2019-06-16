using System.Collections.Generic;
using SLR.Table;

namespace SLR.Utils
{
    public static class SentenceConverter
    {
        public static List<Sentence> ConvertRawSentences( List<RawSentence> rawSentences )
        {
            var sentences = new List<Sentence>();
            sentences.Add( new Sentence( "[START]", new List<Token> { new Token( $"{ rawSentences[ 0 ].MainToken }", 0, 0 ), new Token( "[END]", 1, 0 ) } ) );
            for ( var i = 0; i < rawSentences.Count; ++i )
            {
                var tokens = new List<Token>();
                for ( var j = 0; j < rawSentences[ i ].Tokens.Count; ++j )
                {
                    tokens.Add( new Token( rawSentences[ i ].Tokens[ j ], j, i + 1 ) );
                }

                sentences.Add( new Sentence( rawSentences[ i ].MainToken, tokens ) );
            }

            return sentences;
        }
    }
}
