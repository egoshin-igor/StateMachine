namespace Common.Util
{
    public class Graph
    {
        private string _grapf = "";

        public void AddToGraph( string from, string to )
        {
            _grapf += $"\"{from}\"->\"{to}\";";
        }

        public void AddType( string phigure, string type )
        {
            _grapf += $"\"{phigure}\" [shape = {type}];";
        }

        public string GetGraph()
        {
            return "digraph G{" + _grapf + "}";
        }
    }
}
