using System.IO;
using System.Linq;

namespace SyntacticalAnalyzerGenerator.MSILGenerator.Resources
{
    public static class ResourceManager
    {
        public static string GetInitializeResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.INITIALIZE_ROUTE );
        }

        public static string GetAddOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.ADD_OPERATION_ROUTE );
        }

        public static string GetSubstractionOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.SUBTRACTION_OPERATION_ROUTE );
        }

        public static string GetWriteLineFunctionResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.WRITE_LINE_FUNCTION_ROUTE );
        }

        public static string GetMainFunctionResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.MAIN_FUNCTION_ROUTE );
        }

        public static string GetStackCapacityFunctionResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.STACK_CAPACITY_ROUTE );
        }

        public static string GetVariableDeclarationOperatorResouce()
        {
            return ReadResourceFromFile( ResourceFileRouter.VARIABLE_DECLARATION_OPERATOR_ROUTE );
        }

        public static string GetAssignmentOperatorForIntegerResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.ASSIGNMENT_OPERATOR_FOR_INTEGER_ROUTE );
        }

        public static string GetPushToStackIntegerResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.PUSH_TO_STACK_INTEGER_ROUTE );
        }

        public static string GetPushToStackVariableValueResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.PUSH_TO_STACK_VARIABLE_VALUE_ROUTE );
        }

        public static string GetAssignmentOperatorResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.ASSIGNMENT_OPERATOR_ROUTE );
        }

        public static string GetAddOperationWithoutResultResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.ADD_OPERATION_WITHOUT_RESULT_ROUTE );
        }

        public static string GetMulOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.MUL_OPERATION_ROUTE );
        }

        public static string GetGetFromStackResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.GET_FROM_STACK_ROUTE );
        }

        public static string GetSubOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.SUB_OPERATION_ROUTE );
        }

        public static string GetDivOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.DIV_OPEARTION_ROUTE );
        }

        public static string GetPushToStackDoubleResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.PUSH_TO_STACK_DOUBLE_ROUTE );
        }

        public static string GetCeqOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.CEQ_OPERATION_ROUTE );
        }

        public static string GetAndOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.AND_OPERATION_ROUTE );
        }

        public static string GetOrOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.OR_OPERATION_ROUTE );
        }

        public static string GetBiggerOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.BIGGER_OPERATION_ROUTE );
        }

        public static string GetSmallerOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.SMALLER_OPERATION_ROUTE );
        }

        public static string GetBrFalseOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.IF_OPERATION_ROUTE );
        }

        public static string GetGotoOperationResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.GOTO_OPERATION );
        }

        public static string GetDeclareMetkaResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.DECLARE_METKA_ROUTE );
        }

        private static string ReadResourceFromFile( string path )
        {
            if ( !File.Exists( path ) )
            {
                string resourceFileName = path.Split( '\\' ).Last();
                throw new FileNotFoundException( $"Ресурсный файл {resourceFileName} не обнаружен" );
            }

            string resourceData = "";
            using ( FileStream fstream = File.OpenRead( path ) )
            {
                byte [] array = new byte [ fstream.Length ];
                fstream.Read( array, 0, array.Length );
                resourceData = System.Text.Encoding.Default.GetString( array );
            }
            return resourceData;
        }
    }
}
