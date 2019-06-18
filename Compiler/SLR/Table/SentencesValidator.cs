using System;
using System.Collections.Generic;
using System.Linq;
using SLR.Utils;

namespace SLR.Table
{
    public static class SentencesValidator
    {
        private const string NoTermBegining = "<";
        private const string NoTermEnding = ">";

        public static bool IsValid( this List<RawSentence> sentences )
        {
            bool result = true;
            Dictionary<string, List<RawSentence>> sentensesByName = sentences.GroupBy( s => s.MainToken ).ToDictionary( g => g.Key, g => g.ToList() );
            var validationInfo = new ValidationInfo { SentensesByName = sentensesByName, UncoveredNoTerms = new HashSet<string>() };
            foreach ( RawSentence sentence in sentences )
            {
                if ( !sentence.Tokens.First().StartsWith( NoTermBegining ) || !sentence.Tokens.First().EndsWith( NoTermEnding ) )
                    continue;
                if ( sentence.Tokens.Count == 0 )
                    continue;

                string firstToken = sentence.Tokens.First();
                if ( sentence.MainToken == firstToken )
                    continue;

                validationInfo.UncoveredNoTerms.Clear();
                if ( !IsValidSentences( validationInfo, sentence.MainToken, firstToken ) )
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private static bool IsValidSentences( ValidationInfo validationInfo, string mainToken, string currentToken )
        {
            validationInfo.UncoveredNoTerms.Add( currentToken );
            Dictionary<string, List<RawSentence>> sentensesByName = validationInfo.SentensesByName;

            if ( mainToken == currentToken )
                return false;

            var sentences = sentensesByName[ currentToken ];
            foreach ( RawSentence sentence in sentences )
            {
                if ( sentence.Tokens.Count == 0 )
                    continue;
                if ( !sentence.Tokens.First().StartsWith( NoTermBegining ) || !sentence.Tokens.First().EndsWith( NoTermBegining ) )
                    continue;

                string firstToken = sentence.Tokens.First();
                if ( validationInfo.UncoveredNoTerms.Contains( firstToken ) )
                    continue;

                if ( !IsValidSentences( validationInfo, mainToken, firstToken ) )
                    return false;
            }

            return true;
        }

        private class ValidationInfo
        {
            public Dictionary<string, List<RawSentence>> SentensesByName { get; set; }
            public HashSet<string> UncoveredNoTerms { get; set; }
        }
    }
}
