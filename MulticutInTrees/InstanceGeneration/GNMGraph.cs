// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Generator for a random graph using the G(n, p) model.
    /// </summary>
    public static class GNMGraph
    {
        /// <summary>
        /// Creates a random graph using the G(n, m) model.
        /// </summary>
        /// <param name="numberOfNodes">The number of nodes that should be in the graph. Should be zero or greater.</param>
        /// <param name="numberOfEdges">The number of edges that should be in the graph. Should be zero or greater.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <returns>A randomly generated <see cref="Graph"/> with <paramref name="numberOfNodes"/> <see cref="Node"/>s and <paramref name="numberOfEdges"/> <see cref="Edge{TNode}"/>s.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfNodes"/> or <paramref name="numberOfEdges"/> is negative.</exception>
        public static Graph CreateGNMGraph(int numberOfNodes, int numberOfEdges, Random random)
        {
#if !EXPERIMENT
            Utils.NullCheck(random, nameof(random), "Trying to create a GNP graph, but the random is null!");
            if (numberOfNodes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfNodes), "Trying to create a GNP graph with less than zero nodes!");
            }
            if (numberOfEdges < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfEdges), "Trying to create a GNP graph with less than zero edges!");
            }
#endif
            Counter mockCounter = new();
            Graph graph = new();

            for (uint i = 0; i < numberOfNodes; i++)
            {
                graph.AddNode(new Node(i), mockCounter);
            }

            int maxNumberOfEdges = numberOfNodes * (numberOfNodes - 1) / 2;
            List<(Node, Node)> possibleEdges = new(maxNumberOfEdges);
            foreach (Node node1 in graph.Nodes(mockCounter))
            {
                foreach (Node node2 in graph.Nodes(mockCounter))
                {
                    if (node2.ID <= node1.ID)
                    {
                        continue;
                    }

                    possibleEdges.Add((node1, node2));
                }
            }

            int actualNumberOfEdges = Math.Min(numberOfEdges, maxNumberOfEdges);
            possibleEdges.Shuffle(random);
            for (int i = 0; i < actualNumberOfEdges; i++)
            {
                Edge<Node> edge = new(possibleEdges[i].Item1, possibleEdges[i].Item2);
                graph.AddEdge(edge, mockCounter);
            }

            return graph;
        }
    }
}
