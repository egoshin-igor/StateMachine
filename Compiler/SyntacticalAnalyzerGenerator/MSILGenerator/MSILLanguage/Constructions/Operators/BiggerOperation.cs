using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class BiggerOperation : IMSILConstruction
    {
        public string ToMSILCode()
        {
            return ResourceManager.GetBiggerOperationResource();
        }
    }
}
