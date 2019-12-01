namespace SyntacticalAnalyzerGenerator.MSILGenerator.MSILLanguage.Constructions.Utils
{
    public class Variable
    {
        public string Name { private set; get; }
        public VariableType Type { private set; get; }

        public Variable( string name, VariableType type )
        {
            Name = name;
            Type = type;
        }

    }
}
