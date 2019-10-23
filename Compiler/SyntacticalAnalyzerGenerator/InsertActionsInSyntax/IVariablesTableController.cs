using Lekser;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public interface IVariablesTableController
    {
        void CreateTable();
        void DefineNewType( Term type );
        void DefineIdentifier( Term identifier );
        Term GetTerm( int id );
    }
}
