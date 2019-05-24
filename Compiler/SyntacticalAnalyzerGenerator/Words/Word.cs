using System.Collections.Generic;

namespace SyntacticalAnalyzerGenerator.Words
{
    public partial class Word
    {
        public const string Epsilant = "e";
    }

    public partial class Word
    {
        public string Name { get; set; }
        public WordType Type { get; set; }
        public HashSet<string> DirectingSet { get; set; }
    }
}
