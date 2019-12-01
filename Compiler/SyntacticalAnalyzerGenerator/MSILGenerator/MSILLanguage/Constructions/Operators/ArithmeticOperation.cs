using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public abstract class ArithmeticOperation : IMSILConstruction
    {
        public string FirstVariableName { get; set; }
        public string SecondVariableName { get; set; }
        public string ResultVariableName { get; set; }
        public int? FirstIntValue { get; set; }
        public int? SecondIntValue { get; set; }
        public double? FirstDoubleValue { get; set; }
        public double? SecondDoubleValue { get; set; }

        public abstract string ToMSILCode();

        protected string GetInputParam()
        {
            string inputParam = "";
            if ( FirstIntValue.HasValue )
            {
                inputParam += CreatePushToStackIntCode( FirstIntValue.Value );
            }
            if ( FirstDoubleValue.HasValue )
            {
                inputParam += CreatePushToStackDoubleCode( FirstDoubleValue.Value );
            }
            if ( !string.IsNullOrEmpty( FirstVariableName ) )
            {
                inputParam += CreatePushToStackVariableValue( FirstVariableName );
            }
            if ( SecondIntValue.HasValue )
            {
                inputParam += CreatePushToStackIntCode( SecondIntValue.Value );
            }
            if ( SecondDoubleValue.HasValue )
            {
                inputParam += CreatePushToStackDoubleCode( SecondDoubleValue.Value );
            }
            if ( !string.IsNullOrEmpty( SecondVariableName ) )
            {
                inputParam += CreatePushToStackVariableValue( SecondVariableName );
            }

            return inputParam;
        }

        private string CreatePushToStackIntCode( int value )
        {
            var code = ResourceManager.GetPushToStackIntegerResource();
            return code.Replace( Constants.RESOURCE_VALUE_PARAMETER, value.ToString() );
        }

        private string CreatePushToStackDoubleCode( double value )
        {
            return null;
        }

        private string CreatePushToStackVariableValue( string value )
        {
            var code = ResourceManager.GetPushToStackVariableValueResource();
            return code.Replace( Constants.RESOURCE_VALUE_PARAMETER, value );
        }


        protected string GetResultParam()
        {
            return !string.IsNullOrEmpty( ResultVariableName )
                ? ResourceManager.GetGetFromStackResource().Replace( Constants.RESOURCE_VALUE_PARAMETER, ResultVariableName )
                : "";
        }
    }
}
