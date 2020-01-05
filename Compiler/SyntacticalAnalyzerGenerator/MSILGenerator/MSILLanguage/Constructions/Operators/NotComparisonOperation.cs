using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class NotComparisonOperation : IMSILConstruction
    {
        public string Value { get; set; }
        public string VaribaleName { get; set; }
        private readonly ComparisonOperator _compareWithZeroOperation;

        public NotComparisonOperation()
        {
            _compareWithZeroOperation = new ComparisonOperator();
        }

        public string ToMSILCode()
        {
            _compareWithZeroOperation.FirstValue = GetValue();
            _compareWithZeroOperation.SecondValue = Constants.FALSE_VALUE;
            return _compareWithZeroOperation.ToMSILCode();
        }

        private string GetValue()
        {
            if ( !string.IsNullOrEmpty( Value ) )
            {
                return Value;
            }

            if ( !string.IsNullOrEmpty( VaribaleName ) )
            {
                return VaribaleName;
            }
            return "";
        }
    }
}
