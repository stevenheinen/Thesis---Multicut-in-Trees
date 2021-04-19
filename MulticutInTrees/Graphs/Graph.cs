// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Implementation of a graph that uses <see cref="Node"/>s as nodes.
    /// </summary>
    public class Graph : AbstractGraph<Edge<Node>, Node>
    {
        /// <summary>
        /// Constructor for a <see cref="Graph"/>.
        /// </summary>
        public Graph() : base()
        {

        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Graph with {NumberOfNodes(MockCounter)} nodes and {NumberOfEdges(MockCounter)} edges";
        }
    }
}
