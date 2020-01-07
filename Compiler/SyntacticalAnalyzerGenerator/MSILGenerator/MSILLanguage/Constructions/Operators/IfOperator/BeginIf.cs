using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators.IfOperator
{
    public class BeginIf : IMSILConstruction
    {
        public string Metka { get; private set; }

        public BeginIf( string metka )
        {
            Metka = metka;
        }

        public string ToMSILCode()
        {
            return ResourceManager.GetIfOperationResource().Replace( Constants.RESOURCE_METKA, Metka );
        }
    }
}
