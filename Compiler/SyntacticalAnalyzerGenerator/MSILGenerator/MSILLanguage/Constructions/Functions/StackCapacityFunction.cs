using SyntacticalAnalyzerGenerator.MSILGenerator.Resources;
using SyntacticalAnalyzerGenerator.MSILGenerator.Utils;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Functions
{
    public class StackCapacityFunction : IMSILConstruction
    {
        private int _capacity;

        public StackCapacityFunction( int capacity )
        {
            _capacity = capacity;
        }

        public string ToMSILCode()
        {
            var commandCode = ResourceManager.GetStackCapacityFunctionResource();
            return commandCode.Replace( Constants.RESOURCE_VALUE_PARAMETER, _capacity.ToString() );
        }
    }
}
