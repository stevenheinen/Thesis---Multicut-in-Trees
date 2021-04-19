// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities
{
    // todo: add counters
    // todo: see if it is possible to use edges here as well instead of node tuples.
    /// <summary>
    /// Class that uses Edmond's Blossom algorithm to find a maximum matching in a graph.
    /// </summary>
    public static class EdmondsMatching
    {
        // todo: replace with correct counters
        private static readonly Counter MockCounter = new Counter();

        /// <summary>
        /// Finds a maximum matching in <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <returns>A <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges in the maximum matching.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> is <see langword="null"/>.</exception>
        public static List<(TNode, TNode)> FindMaximumMatching<TGraph, TEdge, TNode>(TGraph graph) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find a maximum matching in a graph, but the graph is null!");
#endif
            List<(TNode, TNode)> matching = FindGreedyMaximalMatching<TGraph, TEdge, TNode>(graph);
            return RecursiveFindMaximumMatching<TGraph, TEdge, TNode>(graph, matching);
        }

        /// <summary>
        /// Finds whether a matching of size at least <paramref name="requiredSize"/> exists in <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <param name="requiredSize">The required size of the matching.</param>
        /// <returns><see langword="true"/> if a matching of size <paramref name="requiredSize"/> exists in <paramref name="graph"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> is <see langword="null"/>.</exception>
        public static bool HasMatchingOfAtLeast<TGraph, TEdge, TNode>(TGraph graph, int requiredSize) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), $"Trying to find a matching of size at least {requiredSize} in a graph, but the graph is null!");
#endif
            List<(TNode, TNode)> matching = FindGreedyMaximalMatching<TGraph, TEdge, TNode>(graph);
            return RecursiveHasMatchingOfAtLeast<TGraph, TEdge, TNode>(graph, requiredSize, matching);
        }

        /// <summary>
        /// Tries to increase a matching in a graph to ultimately find a maximum matching.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <param name="currentMatching">The <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges currently in the matching.</param>
        /// <returns>A <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges in the maximum matching.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> or <paramref name="currentMatching"/> is <see langword="null"/>.</exception>
        private static List<(TNode, TNode)> RecursiveFindMaximumMatching<TGraph, TEdge, TNode>(TGraph graph, List<(TNode, TNode)> currentMatching) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to recursively find a maximum matching in a graph, but the graph is null!");
            Utils.NullCheck(currentMatching, nameof(currentMatching), "Trying to recursively find a maximum matching in a graph, but the current matching is null!");
#endif
            List<(TNode, TNode)> augmentingPath = FindAugmentingPath<TGraph, TEdge, TNode>(graph, currentMatching);
            if (augmentingPath.Count > 0)
            {
                AugmentMatchingAlongPath(currentMatching, augmentingPath);
                return RecursiveFindMaximumMatching<TGraph, TEdge, TNode>(graph, currentMatching);
            }

            return currentMatching;
        }

        /// <summary>
        /// Tries to increase a matching to ultimately find whether a matching of size at least <paramref name="requiredSize"/> exists in <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <param name="requiredSize">The required size of the matching.</param>
        /// <param name="currentMatching">The <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges currently in the matching.</param>
        /// <returns><see langword="true"/> if a matching of size <paramref name="requiredSize"/> exists in <paramref name="graph"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> or <paramref name="currentMatching"/> is <see langword="null"/>.</exception>
        private static bool RecursiveHasMatchingOfAtLeast<TGraph, TEdge, TNode>(TGraph graph, int requiredSize, List<(TNode, TNode)> currentMatching) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), $"Trying to recursively find a matching of size at least {requiredSize} in a graph, but the graph is null!");
            Utils.NullCheck(currentMatching, nameof(currentMatching), $"Trying to recursively find a matching of size at least {requiredSize} in a graph, but the current matching is null!");
#endif
            if (currentMatching.Count >= requiredSize)
            {
                return true;
            }

            List<(TNode, TNode)> augmentingPath = FindAugmentingPath<TGraph, TEdge, TNode>(graph, currentMatching);
            if (augmentingPath.Count > 0)
            {
                AugmentMatchingAlongPath(currentMatching, augmentingPath);
                return RecursiveHasMatchingOfAtLeast<TGraph, TEdge, TNode>(graph, requiredSize, currentMatching);
            }

            return false;
        }

        /// <summary>
        /// Augments a matching along a path.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="matching">The current matching.</param>
        /// <param name="path">The augmenting path to augment the matching along.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="matching"/> or <paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="path"/> is not an augmenting path.</exception>
        private static void AugmentMatchingAlongPath<TNode>(List<(TNode, TNode)> matching, List<(TNode, TNode)> path) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(matching, nameof(matching), "Trying to augment a matching along a path, but the matching is null!");
            Utils.NullCheck(path, nameof(path), "Trying to augment a matching along a path, but the path is null!");
            if (!IsAugmentingPath(path, matching))
            {
                throw new InvalidOperationException("Trying to augment along a path, but the path is not an augmenting path!");
            }
#endif
            for (int i = 1; i < path.Count - 1; i += 2)
            {
                matching.Remove(Utils.OrderEdgeSmallToLarge(path[i]));
            }
            for (int i = 0; i < path.Count; i += 2)
            {
                matching.Add(Utils.OrderEdgeSmallToLarge(path[i]));
            }
        }

        /// <summary>
        /// Find a maximal matching in <paramref name="graph"/> using a greedy approach.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <returns>A <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges in the maximal matching.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> is <see langword="null"/>.</exception>
        private static List<(TNode, TNode)> FindGreedyMaximalMatching<TGraph, TEdge, TNode>(TGraph graph) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find an initial matching in a graph, but the graph is null!");
#endif
            List<(TNode, TNode)> matching = new List<(TNode, TNode)>();

            // Save we have not matched any of the nodes.
            Dictionary<TNode, bool> matched = new Dictionary<TNode, bool>();
            foreach (TNode node in graph.Nodes(MockCounter))
            {
                matched[node] = false;
            }

            // Try to find an edge incident to each node to add to the mathcing.
            foreach (TNode node in graph.Nodes(MockCounter))
            {
                // If we already matched this node, we cannot include any edges incident to it to the matching.
                if (matched[node])
                {
                    continue;
                }

                // Loop through all neighbours of the node to find an edge that can be added to the matching.
                foreach (TNode neighbour in node.Neighbours(MockCounter))
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

        /// <summary>
        /// Find an augmenting path in <paramref name="graph"/> given <paramref name="matching"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the augmenting path.</param>
        /// <param name="matching">The <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges currently in the matching.</param>
        /// <returns>An empty <see cref="List{T}"/> if no augmenting path can be found, or a <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges on the augmenting path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static List<(TNode, TNode)> FindAugmentingPath<TGraph, TEdge, TNode>(TGraph graph, List<(TNode, TNode)> matching) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find an augmenting path in a graph, but the graph is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to find an augmenting path in a graph, but the current matching is null!");
#endif
            if (graph.NumberOfEdges(MockCounter) == 0)
            {
                return new List<(TNode, TNode)>();
            }

            if (matching.Count == 0)
            {
                return new List<(TNode, TNode)>() { graph.Edges(MockCounter).Select(e => (e.Endpoint1, e.Endpoint2)).First() };
            }

            HashSet<TNode> unmatchedVertices = new HashSet<TNode>(graph.Nodes(MockCounter));
            foreach ((TNode, TNode) edge in matching)
            {
                unmatchedVertices.Remove(edge.Item1);
                unmatchedVertices.Remove(edge.Item2);
            }

            if (unmatchedVertices.Count <= 1)
            {
                return new List<(TNode, TNode)>();
            }

            HashSet<TNode> adjacentVertices = new HashSet<TNode>();
            foreach (TNode vertex in unmatchedVertices)
            {
                foreach (TNode neighbour in vertex.Neighbours(MockCounter))
                {
                    adjacentVertices.Add(neighbour);
                }
            }

            HashSet<(TNode, TNode)> hashedMatching = new HashSet<(TNode, TNode)>(matching);
            List<(TNode, TNode)> unmatchedEdges = graph.Edges(MockCounter).Select(Utils.OrderEdgeSmallToLarge<TEdge, TNode>).Where(n => !hashedMatching.Contains(n) && !hashedMatching.Contains((n.Item2, n.Item1))).ToList();

            if (unmatchedEdges.Count == 0)
            {
                return new List<(TNode, TNode)>();
            }

            HashSet<Node> nodesInD = new HashSet<Node>();
            Dictionary<Node, TNode> originalNodes = new Dictionary<Node, TNode>();
            Dictionary<(TNode, TNode), TNode> nodesInMiddleOfArcs = new Dictionary<(TNode, TNode), TNode>();
            BuildDigraphD(unmatchedEdges, hashedMatching, nodesInD, originalNodes, nodesInMiddleOfArcs);

            List<(TNode, TNode)> pathPPrime = FindPathPPrime(nodesInD, unmatchedEdges, originalNodes, unmatchedVertices, adjacentVertices, nodesInMiddleOfArcs);

            if (Utils.IsSimplePath(pathPPrime))
            {
                return pathPPrime;
            }

            return FindAndContractBlossom<TGraph, TEdge, TNode>(graph, pathPPrime, matching);
        }

        /// <summary>
        /// Find a path in the digraph D from an unmatched vertex to a vertex that is a neighbour of an unmatched vertex.
        /// </summary>
        /// <typeparam name="TNode">The type of node used.</typeparam>
        /// <param name="nodesInD">The <see cref="HashSet{T}"/> of <see cref="Node"/>s in the digraph D.</param>
        /// <param name="unmatchedEdges">The <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent edges not in the matching.</param>
        /// <param name="originalNodes"><see cref="Dictionary{TKey, TValue}"/> from a <see cref="Node"/> in the digraph D to the original <typeparamref name="TNode"/>.</param>
        /// <param name="unmatchedVertices"><see cref="HashSet{T}"/> with <typeparamref name="TNode"/>s that are not yet matched.</param>
        /// <param name="adjacentVertices"><see cref="HashSet{T}"/> with <typeparamref name="TNode"/>s that are adjacent to an <typeparamref name="TNode"/> in <paramref name="unmatchedVertices"/>.</param>
        /// <param name="nodesInMiddleOfArcs"><see cref="Dictionary{TKey, TValue}"/> with, for each arc in the digraph D, the <typeparamref name="TNode"/> that is between the unmatched and the matched edge that was used to create this arc in D.</param>
        /// <returns>A <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges along the path P'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodesInD"/>, <paramref name="unmatchedEdges"/>, <paramref name="originalNodes"/>, <paramref name="unmatchedVertices"/>, <paramref name="adjacentVertices"/> or <paramref name="nodesInMiddleOfArcs"/> is <see langword="null"/>.</exception>
        private static List<(TNode, TNode)> FindPathPPrime<TNode>(HashSet<Node> nodesInD, List<(TNode, TNode)> unmatchedEdges, Dictionary<Node, TNode> originalNodes, HashSet<TNode> unmatchedVertices, HashSet<TNode> adjacentVertices, Dictionary<(TNode, TNode), TNode> nodesInMiddleOfArcs) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(nodesInD, nameof(nodesInD), "Trying to create path P', but the set with nodes in D is null!");
            Utils.NullCheck(unmatchedEdges, nameof(unmatchedEdges), "Trying to create path P', but the list with unmatched edges is null!");
            Utils.NullCheck(originalNodes, nameof(originalNodes), "Trying to create path P', but the dictionary with original nodes is null!");
            Utils.NullCheck(unmatchedVertices, nameof(unmatchedVertices), "Trying to create path P', but the set with unmatched vertices in D is null!");
            Utils.NullCheck(adjacentVertices, nameof(adjacentVertices), "Trying to create path P', but the set with adjacent vertices in D is null!");
            Utils.NullCheck(nodesInMiddleOfArcs, nameof(nodesInMiddleOfArcs), "Trying to create path P', but the dictionary with nodes in the middle of arcs in D is null!");
#endif
            Node start = nodesInD.FirstOrDefault(n => unmatchedVertices.Contains(originalNodes[n]));
            if (start is null)
            {
                return new List<(TNode, TNode)>();
            }
            HashSet<Node> target = new HashSet<Node>(nodesInD.Where(n => n != start && adjacentVertices.Contains(originalNodes[n])));

            if (target.Count == 0)
            {
                return new List<(TNode, TNode)>();
            }

            List<(TNode, TNode)> pathPPrime;
            try
            {
                List<Node> pathInD = BFS.FindShortestPath(start, target, MockCounter);

                pathPPrime = new List<(TNode, TNode)>();
                for (int i = 0; i < pathInD.Count - 1; i++)
                {
                    TNode orig1 = originalNodes[pathInD[i]];
                    TNode orig2 = originalNodes[pathInD[i + 1]];

                    TNode x = nodesInMiddleOfArcs[(orig1, orig2)];
                    pathPPrime.Add((orig1, x));
                    pathPPrime.Add((x, orig2));
                }

                pathPPrime.Add((pathPPrime[^1].Item2, pathPPrime[^1].Item2.Neighbours(MockCounter).Cast<TNode>().First(unmatchedVertices.Contains)));
            }
            catch (NotInGraphException)
            {
                (TNode, TNode) edge = unmatchedEdges.FirstOrDefault(n => unmatchedVertices.Contains(n.Item1) && unmatchedVertices.Contains(n.Item2));
                pathPPrime = edge.Equals(default) ? new List<(TNode, TNode)>() : new List<(TNode, TNode)>() { edge };
            }

            return pathPPrime;
        }

        /// <summary>
        /// Build a digraph D with an arc between u and v if there is an edge (u, x) that is not in the matching and an edge (x, v) that is in the matching.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="unmatchedEdges">The <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent edges not in the matching.</param>
        /// <param name="matching">The <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent edges in the matching.</param>
        /// <param name="nodesInD">The <see cref="HashSet{T}"/> of <see cref="Node"/>s in the digraph D.</param>
        /// <param name="originalNodes"><see cref="Dictionary{TKey, TValue}"/> from a <see cref="Node"/> in the digraph D to the original <typeparamref name="TNode"/>.</param>
        /// <param name="nodesInMiddleOfArcs"><see cref="Dictionary{TKey, TValue}"/> with, for each arc in the digraph D, the <typeparamref name="TNode"/> that is between the unmatched and the matched edge that was used to create this arc in D.</param>
        /// <returns>A directed graph D with arcs between nodes u and v if there is an edge (u, x) that is not in the matching and an edge (x, v) that is in the matching.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="unmatchedEdges"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static AbstractGraph<Edge<Node>, Node> BuildDigraphD<TNode>(List<(TNode, TNode)> unmatchedEdges, HashSet<(TNode, TNode)> matching, HashSet<Node> nodesInD, Dictionary<Node, TNode> originalNodes, Dictionary<(TNode, TNode), TNode> nodesInMiddleOfArcs) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(unmatchedEdges, nameof(unmatchedEdges), "Trying to build the digraph D, but the list with the unmatched edges is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to build the digraph D, but the list with the matched edges is null!");
#endif
            nodesInD ??= new HashSet<Node>();
            originalNodes ??= new Dictionary<Node, TNode>();
            nodesInMiddleOfArcs ??= new Dictionary<(TNode, TNode), TNode>();

            // Build digraph D with an arc (u,v) if there is an x such that (u,x) in E-M and (x,v) in M.
            List<(Node, Node)> arcsInD = new List<(Node, Node)>();
            Dictionary<TNode, Node> originalToD = new Dictionary<TNode, Node>();

            foreach ((TNode, TNode) unmatchedEdge in unmatchedEdges)
            {
                foreach ((TNode, TNode) matchedEdge in matching.Where(n => n.Item1.Equals(unmatchedEdge.Item2)))
                {
                    CreateArcForD(unmatchedEdge, matchedEdge, nodesInD, arcsInD, nodesInMiddleOfArcs, originalNodes, originalToD);
                }
                foreach ((TNode, TNode) matchedEdge in matching.Where(n => n.Item1.Equals(unmatchedEdge.Item1)))
                {
                    CreateArcForD((unmatchedEdge.Item2, unmatchedEdge.Item1), matchedEdge, nodesInD, arcsInD, nodesInMiddleOfArcs, originalNodes, originalToD);
                }
                foreach ((TNode, TNode) matchedEdge in matching.Select(n => (n.Item2, n.Item1)).Where(n => n.Item1.Equals(unmatchedEdge.Item2)))
                {
                    CreateArcForD(unmatchedEdge, matchedEdge, nodesInD, arcsInD, nodesInMiddleOfArcs, originalNodes, originalToD);
                }
                foreach ((TNode, TNode) matchedEdge in matching.Select(n => (n.Item2, n.Item1)).Where(n => n.Item1.Equals(unmatchedEdge.Item1)))
                {
                    CreateArcForD((unmatchedEdge.Item2, unmatchedEdge.Item1), matchedEdge, nodesInD, arcsInD, nodesInMiddleOfArcs, originalNodes, originalToD);
                }
            }

            Graph d = new Graph();
            d.AddNodes(nodesInD, MockCounter);
            d.AddEdges(arcsInD.Select(i => new Edge<Node>(i.Item1, i.Item2, true)), MockCounter);
            return d;
        }

        /// <summary>
        /// Creates an arc for the digraph D.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="unmatchedEdge">The unmatched edge between u and x.</param>
        /// <param name="matchedEdge">The matched edge between x and v.</param>
        /// <param name="nodesInD">The <see cref="HashSet{T}"/> of <see cref="Node"/>s in D.</param>
        /// <param name="arcsInD">The <see cref="List{T}"/> of tuples of two <see cref="Node"/>s representing arcs in D.</param>
        /// <param name="nodesInMiddleOfArcs"><see cref="Dictionary{TKey, TValue}"/> with, for each arc in the digraph D, the <typeparamref name="TNode"/> that is between the unmatched and the matched edge that was used to create this arc in D.</param>
        /// <param name="originalNodes"><see cref="Dictionary{TKey, TValue}"/> from a <see cref="Node"/> in the digraph D to the original <typeparamref name="TNode"/>.</param>
        /// <param name="originalToD"><see cref="Dictionary{TKey, TValue}"/> from an original <typeparamref name="TNode"/> to the <see cref="Node"/> in the digraph D.</param>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="unmatchedEdge"/> or <paramref name="matchedEdge"/>, or <paramref name="nodesInD"/>, <paramref name="arcsInD"/>, <paramref name="nodesInMiddleOfArcs"/>, <paramref name="originalNodes"/> or <paramref name="originalToD"/> is <see langword="null"/>.</exception>
        private static void CreateArcForD<TNode>((TNode, TNode) unmatchedEdge, (TNode, TNode) matchedEdge, HashSet<Node> nodesInD, List<(Node, Node)> arcsInD, Dictionary<(TNode, TNode), TNode> nodesInMiddleOfArcs, Dictionary<Node, TNode> originalNodes, Dictionary<TNode, Node> originalToD) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(unmatchedEdge.Item1, nameof(unmatchedEdge.Item1), "Trying to create an arc for the digraph D, but the first endpoint of the unmatched edge is null!");
            Utils.NullCheck(unmatchedEdge.Item2, nameof(unmatchedEdge.Item2), "Trying to create an arc for the digraph D, but the second endpoint of the unmatched edge is null!");
            Utils.NullCheck(matchedEdge.Item1, nameof(matchedEdge.Item1), "Trying to create an arc for the digraph D, but the first endpoint of the matched edge is null!");
            Utils.NullCheck(matchedEdge.Item2, nameof(matchedEdge.Item2), "Trying to create an arc for the digraph D, but the second endpoint of the matched edge is null!");
            Utils.NullCheck(nodesInD, nameof(nodesInD), "Trying to create an arc for the digraph D, but the set with nodes in D is null!");
            Utils.NullCheck(arcsInD, nameof(arcsInD), "Trying to create an arc for the digraph D, but the list with arcs in D is null!");
            Utils.NullCheck(nodesInMiddleOfArcs, nameof(nodesInMiddleOfArcs), "Trying to create an arc for the digraph D, but the dictionary with nodes in the middle of arcs in D is null!");
            Utils.NullCheck(originalNodes, nameof(originalNodes), "Trying to create an arc for the digraph D, but the dictionary with original nodes is null!");
            Utils.NullCheck(originalToD, nameof(originalToD), "Trying to create an arc for the digraph D, but the dictionary with nodes in D from original nodes is null!");
#endif
            if (!originalToD.TryGetValue(unmatchedEdge.Item1, out Node dNode1))
            {
                dNode1 = new Node(unmatchedEdge.Item1.ID);
            }
            if (!originalToD.TryGetValue(matchedEdge.Item2, out Node dNode2))
            {
                dNode2 = new Node(matchedEdge.Item2.ID);
            }

            nodesInD.Add(dNode1);
            nodesInD.Add(dNode2);
            arcsInD.Add((dNode1, dNode2));
            nodesInMiddleOfArcs[(unmatchedEdge.Item1, matchedEdge.Item2)] = matchedEdge.Item1;

            originalNodes[dNode1] = unmatchedEdge.Item1;
            originalNodes[dNode2] = matchedEdge.Item2;

            originalToD[unmatchedEdge.Item1] = dNode1;
            originalToD[matchedEdge.Item2] = dNode2;
        }

        /// <summary>
        /// Detect the blossom in <paramref name="nonSimpleWalk"/>, contract it, and find a new augmenting path in the reduced graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph used.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which the matching takes place.</param>
        /// <param name="nonSimpleWalk">The augmenting walk that contains a blossom.</param>
        /// <param name="matching">The current matching.</param>
        /// <returns>An augmenting path in <paramref name="graph"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/>, <paramref name="nonSimpleWalk"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static List<(TNode, TNode)> FindAndContractBlossom<TGraph, TEdge, TNode>(TGraph graph, List<(TNode, TNode)> nonSimpleWalk, List<(TNode, TNode)> matching) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find and contract a blossom, but the graph this blossom is in is null!");
            Utils.NullCheck(nonSimpleWalk, nameof(nonSimpleWalk), "Trying to find and contract a blossom, but the list with the non-simple walk is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to find and contract a blossom, but the list with the current matching is null!");
#endif
            List<TNode> blossom = new List<TNode>();
            for (int i = 0; i < nonSimpleWalk.Count - 1; i++)
            {
                for (int j = i + 1; j < nonSimpleWalk.Count; j++)
                {
                    if (nonSimpleWalk[i].Item1.Equals(nonSimpleWalk[j].Item2))
                    {
                        blossom = nonSimpleWalk.Skip(i).Take(j - i + 1).Select(n => n.Item1).ToList();
                    }
                }
            }

            HashSet<TEdge> originalEdges = new HashSet<TEdge>();
            HashSet<TNode> neighbours = new HashSet<TNode>();
            foreach (TNode node in blossom)
            {
                foreach (TNode neighbour in node.Neighbours(MockCounter))
                {
                    originalEdges.Add(graph.Edges(MockCounter).First(e => (e.Endpoint1.Equals(node) && e.Endpoint2.Equals(neighbour)) || (e.Endpoint2.Equals(node) && e.Endpoint1.Equals(neighbour))));

                    if (blossom.Contains(neighbour))
                    {
                        continue;
                    }
                    neighbours.Add(neighbour);
                }
            }

            graph.RemoveNodes(blossom, MockCounter);
            graph.AddNode(blossom[0], MockCounter);
            IEnumerable<TEdge> edges = (IEnumerable<TEdge>)neighbours.Select(n => new Edge<TNode>(blossom[0], n));
            graph.AddEdges(edges, MockCounter);

            List<(TNode, TNode)> contractedPath = FindAugmentingPath<TGraph, TEdge, TNode>(graph, matching.Where(n => !(blossom.Contains(n.Item1) && blossom.Contains(n.Item2))).Select(n => blossom.Contains(n.Item1) ? (blossom[0], n.Item2) : n).Select(n => blossom.Contains(n.Item2) ? (n.Item1, blossom[0]) : n).ToList());

            graph.RemoveNode(blossom[0], MockCounter);
            graph.AddNodes(blossom, MockCounter);
            graph.AddEdges(originalEdges, MockCounter);

            List<(TNode, TNode)> result = ExpandPath<TGraph, TEdge, TNode>(graph, contractedPath, blossom, blossom[0], matching);

            return result;
        }

        /// <summary>
        /// After an alternating path was found in a graph with a contracted blossom, expand this path to walk over the blossom again.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph used.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which we are performing the matching.</param>
        /// <param name="contractedPath">The <see cref="List{T}"/> with the alternating path that should be expanded.</param>
        /// <param name="blossom">The <see cref="List{T}"/> of <typeparamref name="TNode"/>s in the blossom.</param>
        /// <param name="contractedBlossom">The <typeparamref name="TNode"/> that represents the contracted blossom.</param>
        /// <param name="matching">The <see cref="List{T}"/> with the current matching.</param>
        /// <returns>The expanded alternating path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/>, <paramref name="contractedPath"/>, <paramref name="blossom"/>, <paramref name="contractedBlossom"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static List<(TNode, TNode)> ExpandPath<TGraph, TEdge, TNode>(TGraph graph, List<(TNode, TNode)> contractedPath, List<TNode> blossom, TNode contractedBlossom, List<(TNode, TNode)> matching) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to expand a path in a graph with a contracted blossom, but the graph is null!");
            Utils.NullCheck(contractedPath, nameof(contractedPath), "Trying to expand a path in a graph with a contracted blossom, but the path is null!");
            Utils.NullCheck(blossom, nameof(blossom), "Trying to expand a path in a graph with a contracted blossom, but the original blossom is null!");
            Utils.NullCheck(contractedBlossom, nameof(contractedBlossom), "Trying to expand a path in a graph with a contracted blossom, but the contracted blossom is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to expand a path in a graph with a contracted blossom, but the current matching is null!");
#endif
            if (contractedPath.Count == 0)
            {
                return contractedPath;
            }

            // If the path we found does not include the blossom, just return the path.
            if (!contractedPath.Select(n => n.Item1).Contains(contractedBlossom) && !contractedPath[^1].Item2.Equals(contractedBlossom))
            {
                return contractedPath;
            }

            // If the blossom is at the start of the path, reverse the path.
            if (contractedPath[0].Item1.Equals(contractedBlossom))
            {
                contractedPath = contractedPath.Select(n => (n.Item2, n.Item1)).ToList();
                contractedPath.Reverse();
            }
            // If the blossom is at the end of the path (either from the reverse above, or just because it already was), handle that case.
            if (contractedPath[^1].Item2.Equals(contractedBlossom))
            {
                return ExpandPathBlossomOnEnd(contractedPath, blossom, matching);
            }

            // The blossom is somewhere in the middle of the path. Handle that case.
            return ExpandPathBlossomInMiddle<TGraph, TEdge, TNode>(graph, contractedPath, blossom, contractedBlossom, matching);
        }

        /// <summary>
        /// After an alternating path was found in a graph with a contracted blossom on the end of this path, expand this path to walk over the blossom again.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="contractedPath">The <see cref="List{T}"/> with the alternating path that should be expanded.</param>
        /// <param name="blossom">The <see cref="List{T}"/> of <typeparamref name="TNode"/>s in the blossom.</param>
        /// <param name="matching">The <see cref="List{T}"/> with the current matching.</param>
        /// <returns>The expanded alternating path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedPath"/>, <paramref name="blossom"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static List<(TNode, TNode)> ExpandPathBlossomOnEnd<TNode>(List<(TNode, TNode)> contractedPath, List<TNode> blossom, List<(TNode, TNode)> matching) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedPath, nameof(contractedPath), "Trying to expand a path in a graph with a contracted blossom on the end of the path, but the path is null!");
            Utils.NullCheck(blossom, nameof(blossom), "Trying to expand a path in a graph with a contracted blossom somewhere on the end of the path, but the original blossom is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to expand a path in a graph with a contracted blossom somewhere on the end of the path, but the current matching is null!");
#endif
            TNode enterNode = blossom.First(n => n.HasNeighbour(contractedPath[^1].Item1, MockCounter));
            contractedPath[^1] = (contractedPath[^1].Item1, enterNode);
            bool matched = true;
            while (true)
            {
                enterNode = matched ? enterNode.Neighbours(MockCounter).Cast<TNode>().FirstOrDefault(n => matching.Contains((n, enterNode)) || matching.Contains((enterNode, n))) : enterNode.Neighbours(MockCounter).Cast<TNode>().FirstOrDefault(n => !matching.Contains((n, enterNode)) && !matching.Contains((enterNode, n)));

                if (enterNode is null)
                {
                    break;
                }

                contractedPath.Add((contractedPath[^1].Item2, enterNode));
                matched = !matched;
            }

            return contractedPath;
        }

        /// <summary>
        /// After an alternating path was found in a graph with a contracted blossom somewhere in the middle of this path, expand this path to walk over the blossom again.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph used.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which we are performing the matching.</param>
        /// <param name="contractedPath">The <see cref="List{T}"/> with the alternating path that should be expanded.</param>
        /// <param name="blossom">The <see cref="List{T}"/> of <typeparamref name="TNode"/>s in the blossom.</param>
        /// <param name="contractedBlossom">The <typeparamref name="TNode"/> that represents the contracted blossom.</param>
        /// <param name="matching">The <see cref="List{T}"/> with the current matching.</param>
        /// <returns>The expanded alternating path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/>, <paramref name="contractedPath"/>, <paramref name="blossom"/>, <paramref name="contractedBlossom"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static List<(TNode, TNode)> ExpandPathBlossomInMiddle<TGraph, TEdge, TNode>(TGraph graph, List<(TNode, TNode)> contractedPath, List<TNode> blossom, TNode contractedBlossom, List<(TNode, TNode)> matching) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to expand a path in a graph with a contracted blossom somewhere in the middle of the path, but the graph is null!");
            Utils.NullCheck(contractedPath, nameof(contractedPath), "Trying to expand a path in a graph with a contracted blossom somewhere in the middle of the path, but the path is null!");
            Utils.NullCheck(blossom, nameof(blossom), "Trying to expand a path in a graph with a contracted blossom somewhere in the middle of the path, but the original blossom is null!");
            Utils.NullCheck(contractedBlossom, nameof(contractedBlossom), "Trying to expand a path in a graph with a contracted blossom somewhere in the middle of the path, but the contracted blossom is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to expand a path in a graph with a contracted blossom somewhere in the middle of the path, but the current matching is null!");
#endif
            TNode lastNodeBeforeBlossom = contractedPath.SkipWhile(n => !n.Item2.Equals(contractedBlossom)).First().Item1;
            TNode firstNodeAfterBlossom = contractedPath.SkipWhile(n => !n.Item1.Equals(contractedBlossom)).First().Item2;

            HashSet<TNode> seen = new HashSet<TNode>(graph.Nodes(MockCounter));
            seen.RemoveWhere(n => blossom.Contains(n) || matching.Select(m => m.Item1).Contains(n) || matching.Select(m => m.Item2).Contains(n));

            IEnumerable<(TNode, TNode)> beforeBlossom = contractedPath.TakeWhile(n => !n.Item2.Equals(contractedBlossom));
            IEnumerable<(TNode, TNode)> afterBlossom = contractedPath.Skip(contractedPath.Select(n => n.Item2).ToList().IndexOf(firstNodeAfterBlossom) + 1);

            List<(TNode, TNode)> path1 = new List<(TNode, TNode)>(beforeBlossom);
            List<(TNode, TNode)> shortest = Utils.NodePathToEdgePath(BFS.FindShortestPath(lastNodeBeforeBlossom, new HashSet<TNode>() { firstNodeAfterBlossom }, MockCounter, new HashSet<TNode>(seen)));
            path1.AddRange(shortest);
            path1.AddRange(afterBlossom);

            if (IsAugmentingPath(path1, matching))
            {
                return path1;
            }

            seen.Add(shortest[1].Item2);
            List<(TNode, TNode)> path2 = new List<(TNode, TNode)>(beforeBlossom);
            path2.AddRange(Utils.NodePathToEdgePath(BFS.FindShortestPath(lastNodeBeforeBlossom, new HashSet<TNode>() { firstNodeAfterBlossom }, MockCounter, seen)));
            path2.AddRange(afterBlossom);
            return path2;
        }

        /// <summary>
        /// Checks whether <paramref name="path"/> is an augmenting path given <paramref name="matching"/>.
        /// </summary>
        /// <typeparam name="TNode">The types of nodes used.</typeparam>
        /// <param name="path">The <see cref="List{T}"/> for which we want to know if it is an augmenting path.</param>
        /// <param name="matching">The <see cref="List{T}"/> with the current matching.</param>
        /// <returns><see langword="true"/> if <paramref name="path"/> is an augmenting path given <paramref name="matching"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static bool IsAugmentingPath<TNode>(List<(TNode, TNode)> path, List<(TNode, TNode)> matching) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(path, nameof(path), "Trying to see if a path is an augmenting path given a matching, but the path is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to see if a path is an augmenting path given a matching, but the matching is null!");
#endif
            // If the path has even length, it cannot be an augmenting path.
            if (path.Count % 2 == 0)
            {
                return false;
            }

            HashSet<(TNode, TNode)> hashedMatching = new HashSet<(TNode, TNode)>(matching);

            // Check if the alternating no-yes-no-yes-...-yes-no path is correct given the matching.
            for (int i = 1; i < path.Count - 1; i += 2)
            {
                if (!hashedMatching.Contains(Utils.OrderEdgeSmallToLarge(path[i])))
                {
                    return false;
                }
            }
            for (int i = 0; i < path.Count; i += 2)
            {
                if (hashedMatching.Contains(Utils.OrderEdgeSmallToLarge(path[i])))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
