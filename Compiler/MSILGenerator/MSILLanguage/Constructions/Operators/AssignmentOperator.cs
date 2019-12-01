using MSILGenerator.MSILLanguage.Constructions.Utils;
using MSILGenerator.Resources;
using MSILGenerator.Utils;

namespace MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class AssignmentOperator : IMSILConstruction
    {
        private VariableType _variableType;
        private Variable _resultVariable;
        private string _variableName;
        private int _intValue;

        public AssignmentOperator( VariableType variableType, Variable variable, int value )
        {
            _variableType = variableType;
            _resultVariable = variable;
            _intValue = value;
        }

        public AssignmentOperator(string variableName, int value)
        {
            _variableName = variableName;
            _intValue = value;
        }

        public string ToMSILCode()
        {
            var commandCode = ResourceManager.GetAssignmentOperatorForIntegerResource();
            commandCode = commandCode.Replace( Constants.RESOURCE_VALUE_PARAMETER, _intValue.ToString() );
            return commandCode.Replace( Constants.RESOURCE_RESULT, string.IsNullOrEmpty( _variableName ) ? _resultVariable.Name : _variableName );
        }
    }
}