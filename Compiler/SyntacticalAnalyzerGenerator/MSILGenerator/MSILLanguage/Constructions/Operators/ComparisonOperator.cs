using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class ComparisonOperator : BoolOpeartion
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetCeqOperationResource();
        }
    }
}
