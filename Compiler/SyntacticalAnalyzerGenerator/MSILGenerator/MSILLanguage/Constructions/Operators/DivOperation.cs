using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class DivOperation : ArithmeticOperation
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetDivOperation();
        }
    }
}
