using System.Collections.Generic;
using System.Linq;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.Words
{
    public class TableRow
    {
        public int N { get; set; }
        public string Name { get; set; }
        public TermType? TermType { get; set; }
        public HashSet<TermType> DirectingSet { get; set; }
        public bool IsShift { get; set; } = false;
        public int ShiftOnError { get; set; } = -1;
        public bool IsPushToStack { get; set; } = false;
        public int GoTo { get; set; }
        public bool IsEnd { get; set; } = false;
        public string ActionName { get; set; }

        public Word Word { get; set; }
        public Word Parent { get; set; }
        public int NextParallelRow { get; set; } = -1;
        public bool IsFirst { get; set; } = false;
        public bool IsLast { get; set; } = false;
    }
}
