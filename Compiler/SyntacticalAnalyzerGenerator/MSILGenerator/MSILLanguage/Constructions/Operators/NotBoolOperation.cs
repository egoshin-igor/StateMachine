using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class NotBoolOperation : IMSILConstruction
    {
        public string Value { get; set; }
        public string VaribaleName { get; set; }
        private readonly ComparisonOperator _compareWithTrueOperation;
        private readonly ComparisonOperator _compareWithFalseOperation;

        public NotBoolOperation()
        {
            _compareWithTrueOperation = new ComparisonOperator();
            _compareWithFalseOperation = new ComparisonOperator();
        }

        public string ToMSILCode()
        {
            _compareWithTrueOperation.FirstValue = GetValue();
            _compareWithTrueOperation.SecondValue = Constants.TRUE_VALUE;
            var code = _compareWithTrueOperation.ToMSILCode();
            _compareWithFalseOperation.FirstValue = Constants.FALSE_VALUE;
            code += _compareWithFalseOperation.ToMSILCode();
            return code;
        }

        private string GetValue()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                return Value;
            }

            if (!string.IsNullOrEmpty(VaribaleName))
            {
                return VaribaleName;
            }
            return "";
        }
    }
}
