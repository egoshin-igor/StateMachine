using System;
using System.Collections.Generic;
using System.Linq;

namespace SLR.Table
{
    public static class SentencesValidator
    {
        private const string NoTermBegining = "<";

        public static bool IsValid( this List<Sentence> sentences )
        {
            bool result = true;
            Dictionary<string, List<Sentence>> sentensesByName = sentences.GroupBy( s => s.MainToken ).ToDictionary( g => g.Key, g => g.ToList() );
            foreach ( Sentence sentence in sentences )
            {
                if ( !sentence.Tokens.First().Value.StartsWith( NoTermBegining ) )
                    continue;
                if ( sentence.Tokens.Count == 0 )
                    continue;
                if ( sentence.MainToken == sentence.Tokens.First().Value )
                    continue;

                if ( !IsValidSentences( sentensesByName, sentence.MainToken, sentence.Tokens.First().Value ) )
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private static bool IsValidSentences( Dictionary<string, List<Sentence>> sentensesByName, string mainToken, string currentToken )
        {
            if ( mainToken == currentToken )
                return false;

            var sentences = sentensesByName[ currentToken ];
            foreach ( Sentence sentence in sentences )
            {
                if ( sentence.Tokens.Count == 0 )
                    continue;
                if ( !sentence.Tokens.First().Value.StartsWith( NoTermBegining ) )
                    continue;
                if ( !IsValidSentences( sentensesByName, mainToken, sentence.Tokens.First().Value ) )
                    return false;
            }

            return true;
        }
    }
}
