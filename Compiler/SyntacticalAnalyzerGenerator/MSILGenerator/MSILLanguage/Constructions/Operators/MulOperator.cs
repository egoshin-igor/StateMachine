using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class MulOperator : ArithmeticOperation
    {
        public override string ToMSILCode()
        {
            return $"{GetInputParam()}{ResourceManager.GetMulOperationResource()}";
        }
    }
}
