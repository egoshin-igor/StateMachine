namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public static class SourceActionName
    {
        public const string VtcCreateTable = "CreateTable";
        public const string VtcDefineNewType = "DefineNewType";
        public const string VtcDefineIdentifier = "DefineIdentifier";
        public const string VtcDestroyLastTable = "DestroyLastTable";
        public const string VtcGetTerm = "GetVariable";

        public const string TcSaveLastTerm = "SaveLastTerm";
        public const string TcCheckLeftRight = "CheckLeftRight";
        public const string TcSaveLeftTerm = "SaveLeftTerm";
        public const string TcSaveRightTerm = "SaveRightTerm";
        public const string TcDefineArrElemType = "DefineArrElemType";

        public const string CommonIsIntIndex = "IsIntIndex";

        public const string AoActionAfterNumber = "ActionAfterNumber";
        public const string AoActionAfterSign = "ActionAfterSign";
        public const string AoActionAfterOperation = "ActionAfterOperation";


        public const string AoClear = "Clear";
    }
}
