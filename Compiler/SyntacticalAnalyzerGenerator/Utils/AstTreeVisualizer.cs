using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Util;
using SyntacticalAnalyzerGenerator.InsertActionsInSyntax.ASTNodes;

namespace SyntacticalAnalyzerGenerator.Utils
{
    public static class AstTreeVisualizer
    {
        public async static Task VisualizeAsync( IASTNode aSTNode, string fileUri )
        {
            var graph = new Graph();

            int currentNodeId = 0;
            NodeWithId nodeWithId = NumericNodes( aSTNode, ref currentNodeId );
            Visualize( graph, nodeWithId );
            using ( var streamWriter = new StreamWriter( fileUri ) )
            {
                await streamWriter.WriteAsync( graph.GetGraph() );
            }
        }

        private static NodeWithId NumericNodes( IASTNode aSTNode, ref int currentId )
        {
            currentId++;
            var result = new NodeWithId { ParrentNode = aSTNode, Id = currentId, Childs = new List<NodeWithId>() };
            foreach ( IASTNode node in aSTNode.Nodes )
            {
                NodeWithId child = NumericNodes( node, ref currentId );
                result.Childs.Add( child );
            }

            return result;
        }

        private static void Visualize( Graph graph, NodeWithId aSTNode )
        {
            foreach ( NodeWithId node in aSTNode.Childs )
            {
                graph.AddToGraph( aSTNode.UniqueName, node.UniqueName );
            }

            foreach ( NodeWithId node in aSTNode.Childs )
            {
                Visualize( graph, node );
            }
        }

        private class NodeWithId
        {
            public IASTNode ParrentNode { get; set; }
            public List<NodeWithId> Childs { get; set; }
            public int Id { get; set; }

            public string UniqueName => $"{ParrentNode.Value}(Id:{Id})";
        }
    }
}
