namespace MSILGenerator.Resources
{
    static class ResourceFileRouter
    {
        private static readonly string RESOURCE_DIRECTORY = "..\\..\\..\\MSILGenerator\\Resources\\MSILResource";

        public static readonly string INITIALIZE_ROUTE = $"{RESOURCE_DIRECTORY}\\Initialize.txt";
        public static readonly string ADD_OPERATION_ROUTE = $"{RESOURCE_DIRECTORY}\\AddOperation.txt";
        public static readonly string SUBTRACTION_OPERATION_ROUTE = $"{RESOURCE_DIRECTORY}\\SubstractionOperation.txt";
        public static readonly string WRITE_LINE_FUNCTION_ROUTE = $"{RESOURCE_DIRECTORY}\\WriteLine.txt";
        public static readonly string MAIN_FUNCTION_ROUTE = $"{RESOURCE_DIRECTORY}\\MainFunction.txt";
        public static readonly string STACK_CAPACITY_ROUTE = $"{RESOURCE_DIRECTORY}\\StackCapacity.txt";
        public static readonly string VARIABLE_DECLARATION_OPERATOR = $"{RESOURCE_DIRECTORY}\\VariableDeclarationOperator.txt";
        public static readonly string ASSIGNMENT_OPERATOR_FOR_INTEGER = $"{RESOURCE_DIRECTORY}\\AssignmentOperatorForInteger.txt";
        public static readonly string PUSH_TO_STACK_INTEGER = $"{RESOURCE_DIRECTORY}\\PushToStackInt.txt";
        public static readonly string PUSH_TO_STACK_VARIABLE_VALUE = $"{RESOURCE_DIRECTORY}\\PushToStackVariableValue.txt";
    }
}
