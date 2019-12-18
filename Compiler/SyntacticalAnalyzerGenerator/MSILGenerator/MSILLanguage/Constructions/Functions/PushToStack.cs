using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Functions
{
    public class PushToStack : IMSILConstruction
    {
        public int? IntValue { get; set; }
        public double? DoubleValue { get; set; }
        public string VariableName { get; set; }

        public string ToMSILCode()
        {
            var msilCode = GetResource();
            return msilCode.Replace( Constants.RESOURCE_VALUE_PARAMETER, GetValue() );
        }

        private string GetResource()
        {
            if ( IntValue.HasValue )
            {
                return ResourceManager.GetPushToStackIntegerResource();
            }

            if ( DoubleValue.HasValue )
            {
                return ResourceManager.GetPushToStackDoubleResource();
            }

            if ( !string.IsNullOrEmpty( VariableName ) )
            {
                return ResourceManager.GetPushToStackVariableValueResource();
            }
            return "";
        }

        private string GetValue()
        {
            if ( IntValue.HasValue )
            {
                return IntValue.ToString();
            }

            if ( DoubleValue.HasValue )
            {
                return DoubleValue.ToString();
            }

            if ( !string.IsNullOrEmpty( VariableName ) )
            {
                return VariableName;
            }
            return "";
        }
    }
}
