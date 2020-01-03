using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class OrOperation : BoolOpeartion
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetOrOperationResource();
        }
    }
}
