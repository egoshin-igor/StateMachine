using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators.IfOperator
{
    public class BeginElse : IMSILConstruction
    {
        public string Metka { get; private set; }

        public BeginElse( string metka )
        {
            Metka = metka;
        }

        public string ToMSILCode()
        {
            return ResourceManager.GetDeclareMetkaResource().Replace( Constants.RESOURCE_METKA, Metka );
        }
    }
}
