using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Utils
{
    public class Initializator : IMSILConstruction
    {
        public string ToMSILCode()
        {
            return ResourceManager.GetInitializeResource();
        }
    }
}
