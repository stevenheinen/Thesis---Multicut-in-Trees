// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Generator for a simple Erdos-Renyi graph.
    /// </summary>
    public static class ErdosRenyiGraph
    {
        private static readonly Counter MockCounter = new Counter();

        /// <summary>
        /// Creates a random Erdos-Renyi graph.
        /// </summary>
        /// <param name="numberOfNodes">The number of nodes that should be in the graph. Should be zero or greater.</param>
        /// <param name="chancePerEdge">The chance for each possible edge to exist in the graph. There will be roughly <paramref name="numberOfNodes"/> * <paramref name="numberOfNodes"/> * <paramref name="chancePerEdge"/> edges. Should be zero or greater.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <returns>A randomly generated <see cref="Graph{N}"/> with <paramref name="numberOfNodes"/> <see cref="Node"/>s according to the Erdos-Renyi model.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfNodes"/> or <paramref name="chancePerEdge"/> is negative.</exception>
        public static Graph<Node> CreateErdosRenyiGraph(int numberOfNodes, double chancePerEdge, Random random)
        {
#if !EXPERIMENT
            Utils.NullCheck(random, nameof(random), "Trying to create an Erdos-Renyi graph, but the random is null!");
            if (numberOfNodes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfNodes), "Trying to create an Erdos-Renyi graph with less than zero nodes!");
            }
            if (chancePerEdge < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chancePerEdge), "Trying to create an Erdos-Renyi graph with a negative chance per possible edge!");
            }
            if (chancePerEdge > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(chancePerEdge), "Trying to create an Erdos-Renyi graph with a change of larger than 1 per possible edge!");
            }
#endif
            Graph<Node> graph = new Graph<Node>();

            for (uint i = 0; i < numberOfNodes; i++)
            {
                graph.AddNode(new Node(i), MockCounter);
            }

            foreach (Node node1 in graph.Nodes(MockCounter))
            {
                foreach (Node node2 in graph.Nodes(MockCounter))
                {
                    if (node2.ID <= node1.ID)
                    {
                        continue;
                    }
                    if (random.NextDouble() < chancePerEdge)
                    {
                        graph.AddEdge(node1, node2, MockCounter);
                    }
                }
            }

            return graph;
        }
    }
}
