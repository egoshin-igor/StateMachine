using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class AddingOperator : ArithmeticOperation
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetAddOperationResource() + GetResultParam();
        }
    }
}
