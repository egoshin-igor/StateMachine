using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class NotComparisonOperation : IMSILConstruction
    {
        private readonly ComparisonOperator _compareWithZeroOperation;
        private readonly ComparisonOperator _comparisonOperator;

        public NotComparisonOperation()
        {
            _compareWithZeroOperation = new ComparisonOperator();
            _comparisonOperator = new ComparisonOperator();
        }

        public string ToMSILCode()
        {
            _compareWithZeroOperation.FirstValue = Constants.FALSE_VALUE;
            return _comparisonOperator.ToMSILCode() + _compareWithZeroOperation.ToMSILCode();
        }
    }
}
