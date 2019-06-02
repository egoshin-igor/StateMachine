
using System.Collections.Generic;
using System.Linq;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.Words
{
    public class ResultTableRow
    {
        public int N { get; set; }
        public string Name { get; set; }
        public TermType? TermType { get; set; }
        public HashSet<string> DirectingSet { get; set; }
        public bool IsShift { get; set; } = false;
        public int ShiftOnError { get; set; } = -1;
        public bool IsPushToStack { get; set; } = false;
        public int GoTo { get; set; }
        public bool IsEnd { get; set; } = false;

        public override string ToString()
        {
            return $"N: {N};\tName:{Name};\tSet:{SetToString()};\tShift:{IsShift};\tOnErr:{ShiftOnError};\tStack:{IsPushToStack};\tGoTo:{GoTo};\tIsEnd:{IsEnd}";
        }

        private string SetToString()
        {
            var result = "";
            foreach ( var direct in DirectingSet )
            {
                result += direct + " ";
            }

            return string.Join( ", ", DirectingSet.ToArray() );
        }
    }
}
