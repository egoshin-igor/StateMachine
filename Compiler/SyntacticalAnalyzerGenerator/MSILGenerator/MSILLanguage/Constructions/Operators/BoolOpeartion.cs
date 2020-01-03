using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public abstract class BoolOpeartion : IMSILConstruction
    {
        public string FirstVariableName { get; set; }
        public string SecondVariableName { get; set; }
        public string ResultVariableName { get; set; }
        public string FirstValue { get; set; }
        public string SecondValue { get; set; }

        public abstract string ToMSILCode();

        protected string GetInputParam()
        {
            string inputParam = "";
            if ( !string.IsNullOrEmpty( FirstValue ) )
            {
                inputParam += CreatePushToStackIntCode( FirstValue );
            }
            if ( !string.IsNullOrEmpty( SecondValue ) )
            {
                inputParam += CreatePushToStackIntCode( SecondValue );
            }
            if ( !string.IsNullOrEmpty( FirstVariableName ) )
            {
                inputParam += CreatePushToStackVariableValue( FirstVariableName );
            }
            if ( !string.IsNullOrEmpty( SecondVariableName ) )
            {
                inputParam += CreatePushToStackVariableValue( SecondVariableName );
            }
            return inputParam;
        }

        protected string GetResultParam()
        {
            return !string.IsNullOrEmpty( ResultVariableName )
                ? ResourceManager.GetGetFromStackResource().Replace( Constants.RESOURCE_VALUE_PARAMETER, ResultVariableName )
                : "";
        }

        private string CreatePushToStackIntCode( string value )
        {
            int intValue = value == Constants.TRUE_VALUE ? 1 : 0;
            var code = ResourceManager.GetPushToStackIntegerResource();
            return code.Replace( Constants.RESOURCE_VALUE_PARAMETER, intValue.ToString() );
        }

        private string CreatePushToStackVariableValue( string value )
        {
            var code = ResourceManager.GetPushToStackVariableValueResource();
            return code.Replace( Constants.RESOURCE_VALUE_PARAMETER, value );
        }
    }
}
