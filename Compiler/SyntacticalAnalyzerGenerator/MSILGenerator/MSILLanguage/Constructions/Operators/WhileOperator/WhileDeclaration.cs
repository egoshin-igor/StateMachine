using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators.WhileOperator
{
    public class WhileDeclaration : IMSILConstruction
    {
        public string Metka { get; private set; }

        public WhileDeclaration( string metka )
        {
            Metka = metka;
        }

        public string ToMSILCode()
        {
            return ResourceManager.GetDeclareMetkaResource().Replace( Constants.RESOURCE_METKA, Metka );
        }
    }
}
