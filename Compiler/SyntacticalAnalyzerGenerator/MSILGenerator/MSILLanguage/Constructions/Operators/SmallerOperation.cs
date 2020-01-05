using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class SmallerOperation : IMSILConstruction
    {
        public string ToMSILCode()
        {
            return ResourceManager.GetSmallerOperationResource();
        }
    }
}
