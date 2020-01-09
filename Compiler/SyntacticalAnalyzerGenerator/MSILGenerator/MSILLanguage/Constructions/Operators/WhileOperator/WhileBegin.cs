using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators.WhileOperator
{
    public class WhileBegin : IMSILConstruction
    {
        public string Metka { get; private set; }

        public WhileBegin( string metka )
        {
            Metka = metka;
        }

        public string ToMSILCode()
        {
            return ResourceManager.GetBrFalseOperationResource().Replace( Constants.RESOURCE_METKA, Metka );
        }
    }
}
