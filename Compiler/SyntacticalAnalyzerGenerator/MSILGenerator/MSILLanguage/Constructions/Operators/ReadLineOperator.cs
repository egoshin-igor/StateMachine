using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Operators
{
    public class ReadLineOperator : IMSILConstruction
    {
        private string _type;
        private string _variableName;
        private string _systemType;

        public ReadLineOperator( string type, string variableName )
        {
            _type = GetMSILType( type );

            _variableName = variableName;
        }

        public string ToMSILCode()
        {
            var code = ResourceManager.GetReadLineResource();
            if ( IsNeedToParseStringCommand() )
            {
                code += ResourceManager.GetParseStringResource().Replace( Constants.RESOURCE_TYPE, _type ).Replace( Constants.RESOURCE_U_TYPE, GetSystemType( _type ) );
            }
            return code + ResourceManager.GetGetFromStackResource().Replace( Constants.RESOURCE_VALUE_PARAMETER, _variableName );
        }

        private string GetMSILType( string type )
        {
            switch ( type )
            {
                case "int":
                    return "int32";
                case "string":
                    return "string";
                case "bool":
                    return "int32";
                default:
                    return "";
            }
        }

        private string GetSystemType( string type )
        {
            return "Int32";
        }

        private bool IsNeedToParseStringCommand()
        {
            return _type == "int32";
        }
    }
}
