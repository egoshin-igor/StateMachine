using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Utils;
using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Functions
{
    public class WriteLineFunction : IMSILConstruction
    {
        private string _value;
        private string _variableName;
        private bool _isVariable;
        private Variable _variable;

        public WriteLineFunction( string value, bool isVariable = false )
        {
            _isVariable = isVariable;
            if ( _isVariable )
            {
                _variableName = value;
            }
            else
            {
                _value = value;
            }
        }

        public WriteLineFunction( Variable variable )
        {
            _variable = variable;
        }

        public string ToMSILCode()
        {
            var commandCode = ResourceManager.GetWriteLineFunctionResource();

            commandCode = !string.IsNullOrEmpty( _value )
                ? commandCode.Replace( Constants.RESOURCE_COMMAND, "ldstr" )
                : commandCode.Replace( Constants.RESOURCE_COMMAND, "ldloc" );

            commandCode = !string.IsNullOrEmpty( _value )
                ? commandCode.Replace( Constants.RESOURCE_TYPE, VariableTypeHelper.GetMSILRepresentation( VariableType.String ) )
                : commandCode.Replace( Constants.RESOURCE_TYPE, VariableTypeHelper.GetMSILRepresentation( _variable.Type ) );

            return !string.IsNullOrEmpty( _value )
                ? commandCode.Replace( Constants.RESOURCE_VALUE_PARAMETER, $"\"{_value}\"" )
                : commandCode.Replace( Constants.RESOURCE_VALUE_PARAMETER, _variable.Name );
        }
    }
}
