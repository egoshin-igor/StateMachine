using Lekser;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public interface IVariablesTableController
    {
        void CreateTable();
        void InsertToCurrentTable( Term term );
        void DestroyLastTable();
        Term GetTerm( int id );
    }
}
