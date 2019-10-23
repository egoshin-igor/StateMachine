using System.Collections.Generic;
using System.Linq;

namespace Common.Data.Words
{
    public class Expression
    {
        public Word NoTerm { get; set; }
        public List<Word> Words { get; set; }

        public string ToStringWithoutSet()
        {
            return $"{NoTerm.Name} -> {WordsToString()}";
        }

        private string WordsToString()
        {
            return string.Join( " ", Words.Select( w => w.Name ) );
        }
    }
}
