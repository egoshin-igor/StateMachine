using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class AndOperation : BoolOpeartion
    {
        public override string ToMSILCode()
        {
            return GetInputParam() + ResourceManager.GetAndOperationResource();
        }
    }
}
