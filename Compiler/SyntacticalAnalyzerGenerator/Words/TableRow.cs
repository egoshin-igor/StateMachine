﻿using System.Collections.Generic;
using System.Linq;

namespace SyntacticalAnalyzerGenerator.Words
{
    public class TableRow
    {
        public int N { get; set; }
        public string Name { get; set; }
        public HashSet<string> DirectingSet { get; set; }
        public bool IsShift { get; set; } = false;
        public int ShiftOnError { get; set; } = -1;
        public bool IsPushToStack { get; set; } = false;
        public int GoTo { get; set; }
        public bool IsEnd { get; set; } = false;

        public Word Word { get; set; }
        public Word Parent { get; set; }
        public int NextParallelRow { get; set; } = -1;
        public bool IsFirst { get; set; } = false;
        public bool IsLast { get; set; } = false;
    }
}