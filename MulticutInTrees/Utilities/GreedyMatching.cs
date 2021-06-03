// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Class that finds a maximal matching in a <see cref="AbstractGraph{TEdge, TNode}"/> using a greedy approach.
    /// </summary>
    public static class GreedyMatching
    {
        /// <summary>
        /// Find a maximal matching in <paramref name="graph"/> using a greedy approach.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> to use for performance measurement.</param>
        /// <returns>A <see cref="List{T}"/> <typeparamref name="TEdge"/>s that represent the edges in the maximal matching.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> or <paramref name="graphCounter"/> is <see langword="null"/>.</exception>
        public static List<(TNode, TNode)> FindGreedyMaximalMatching<TGraph, TEdge, TNode>(TGraph graph, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find an initial matching in a graph, but the graph is null!");
            Utils.NullCheck(graphCounter, nameof(graphCounter), "Trying to find an initial matching in a graph, but the counter is null!");
#endif
            List<(TNode, TNode)> matching = new();

            // Save we have not matched any of the nodes.
            Dictionary<TNode, bool> matched = new();
            foreach (TNode node in graph.Nodes(graphCounter))
            {
                matched[node] = false;
            }

            // Try to find an edge incident to each node to add to the mathcing.
            foreach (TNode node in graph.Nodes(graphCounter))
            {
                // If we already matched this node, we cannot include any edges incident to it to the matching.
                if (matched[node])
                {
                    continue;
                }

                // Loop through all neighbours of the node to find an edge that can be added to the matching.
                foreach (TNode neighbour in node.Neighbours(graphCounter))
                {
                    // If we already matched this neighbour, go to the next one.
                    if (matched[neighbour])
                    {
                        continue;
                    }

                    // We have found an unmatched neighbour. Add the edge between this node and the neighbour to the matching.
                    matching.Add(Utils.OrderEdgeSmallToLarge((node, neighbour)));
                    matched[node] = true;
                    matched[neighbour] = true;
                    break;
                }
            }

            return matching;

        }
    }
}
