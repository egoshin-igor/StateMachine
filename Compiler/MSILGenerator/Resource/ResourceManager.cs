using System.IO;
using System.Linq;

namespace MSILGenerator.Resources
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
            return ReadResourceFromFile( ResourceFileRouter.VARIABLE_DECLARATION_OPERATOR );
        }

        public static string GetAssignmentOperatorForIntegerResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.ASSIGNMENT_OPERATOR_FOR_INTEGER );
        }

        public static string GetPushToStackIntegerResource()
        {
            return ReadResourceFromFile( ResourceFileRouter.PUSH_TO_STACK_INTEGER );
        }

        public static string GetPushToStackVariableValue()
        {
            return ReadResourceFromFile( ResourceFileRouter.PUSH_TO_STACK_VARIABLE_VALUE );
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
