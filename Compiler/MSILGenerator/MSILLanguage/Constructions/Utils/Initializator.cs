using MSILGenerator.Resources;

namespace MSILGenerator.MSILLanguage.Constructions.Utils
{
    public class Initializator : IMSILConstruction
    {
        public string ToMSILCode()
        {
            return ResourceManager.GetInitializeResource();
        }
    }
}
