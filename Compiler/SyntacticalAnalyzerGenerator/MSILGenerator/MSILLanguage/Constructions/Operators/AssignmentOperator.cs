using SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Utils;
using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;
using System;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class AssignmentOperator : IMSILConstruction
    {
        private VariableType _variableType;
        private Variable _resultVariable;
        private string _variableName;
        private int? _intValue = null;
        private float? _floatValue = null;
        private int? _boolValue = null;
        private bool _isAssignmentFromStack = false;

        public AssignmentOperator( VariableType variableType, Variable variable, int value )
        {
            _variableType = variableType;
            _resultVariable = variable;
            _intValue = value;
        }

        public AssignmentOperator( string variableName, int value )
        {
            _variableName = variableName;
            _intValue = value;
        }

        public AssignmentOperator( string variableName, float value )
        {
            _variableName = variableName;
            _floatValue = value;
        }

        public AssignmentOperator( string variblaName, string boolValue )
        {
            if ( boolValue != Constants.TRUE_VALUE && boolValue != Constants.FALSE_VALUE )
            {
                throw new Exception( "Шеф, все пропало, какую-то дичь в бул пихают!!!" );
            }
            _variableName = variblaName;
            _boolValue = boolValue == Constants.TRUE_VALUE ? 1 : 0;
        }

        public AssignmentOperator( string resultVariableName )
        {
            _isAssignmentFromStack = true;
            _variableName = resultVariableName;
        }

        public string ToMSILCode()
        {
            string commandCode = "";
            if ( _isAssignmentFromStack )
            {
                commandCode = ResourceManager.GetAssignmentOperatorResource();
            }
            else
            {
                commandCode = GetAssigmentResource();
                commandCode = commandCode.Replace( Constants.RESOURCE_VALUE_PARAMETER, GetValue() );
            }
            return commandCode.Replace( Constants.RESOURCE_RESULT, string.IsNullOrEmpty( _variableName ) ? _resultVariable.Name : _variableName );
        }

        private string GetValue()
        {
            if (_intValue.HasValue)
            {
                return _intValue.ToString();
            }

            if (_floatValue.HasValue)
            {
                return _floatValue.ToString();
            }

            if (_boolValue.HasValue)
            {
                return _boolValue.ToString();
            }

            throw new Exception( "Шеф, все пропало, нечего присваивать" );
        }

        private string GetAssigmentResource()
        {
            if ( _floatValue.HasValue )
            {
                return ResourceManager.GetAssignmentOperatorForFloatResource();
            }
            return ResourceManager.GetAssignmentOperatorForIntegerResource();
        }
    }
}
