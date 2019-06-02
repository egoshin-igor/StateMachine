using System.Collections.Generic;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.Words
{
    public partial class Word
    {
        public const string Epsilant = "[EPS]";
        public const string End = "[END]";
    }

    public partial class Word
    {
        public string Name { get; set; }
        public TermType? TermType { get; set; } = null;
        public WordType Type { get; set; }
        public HashSet<string> DirectingSet { get; set; }
    }
}
