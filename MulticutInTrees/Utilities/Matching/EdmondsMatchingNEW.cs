// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities.Matching
{
    /// <summary>
    /// Class that uses Edmond's Blossom algorithm to find a maximum matching in a graph.
    /// </summary>
    public static class EdmondsMatchingNEW
    {
        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not affect performance.
        /// </summary>
        private static readonly Counter MockCounter = new();

        /// <summary>
        /// Finds a maximum matching in <paramref name="graph"/>.
        /// <br/>
        /// <b>NOTE:</b> This implementation is not bug-free. It only works on acyclic graphs (and mostly on general graphs, but no guarantees!).
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="TEdge"/>s that represent the edges in the maximum matching.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="graph"/> is not acyclic.</exception>
        public static List<TEdge> FindMaximumMatching<TGraph, TEdge, TNode>(TGraph graph, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find a maximum matching in a graph, but the graph is null!");
#endif
            List<TEdge> matching = GreedyMatching.FindGreedyMaximalMatching<TGraph, TEdge, TNode>(graph);
            return RecursiveFindMaximumMatching<TGraph, TEdge, TNode>(graph, matching, graphCounter);
        }

        /// <summary>
        /// Finds whether a matching of size at least <paramref name="requiredSize"/> exists in <paramref name="graph"/>.
        /// <br/>
        /// <b>NOTE:</b> This implementation is not bug-free. It only works on acyclic graphs (and mostly on general graphs, but no guarantees!).
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <param name="requiredSize">The required size of the matching.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns><see langword="true"/> if a matching of size <paramref name="requiredSize"/> exists in <paramref name="graph"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="graph"/> is not acyclic.</exception>
        public static bool HasMatchingOfAtLeast<TGraph, TEdge, TNode>(TGraph graph, int requiredSize, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), $"Trying to find a matching of size at least {requiredSize} in a graph, but the graph is null!");
            if (!graph.IsAcyclic(MockCounter))
            {
                throw new NotSupportedException("This implementation of the blossom matching algorithm is not bug-free! Please only use it on acyclic graphs!");
            }
#endif
            List<TEdge> matching = GreedyMatching.FindGreedyMaximalMatching<TGraph, TEdge, TNode>(graph);
            return RecursiveHasMatchingOfAtLeast<TGraph, TEdge, TNode>(graph, requiredSize, matching, graphCounter);
        }

        /// <summary>
        /// Tries to increase a matching in a graph to ultimately find a maximum matching.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the matching.</param>
        /// <param name="currentMatching">The <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges currently in the matching.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="TEdge"/>s that represent the edges in the maximum matching.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> or <paramref name="currentMatching"/> is <see langword="null"/>.</exception>
        private static List<TEdge> RecursiveFindMaximumMatching<TGraph, TEdge, TNode>(TGraph graph, List<TEdge> currentMatching, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to recursively find a maximum matching in a graph, but the graph is null!");
            Utils.NullCheck(currentMatching, nameof(currentMatching), "Trying to recursively find a maximum matching in a graph, but the current matching is null!");
#endif
            List<TEdge> augmentingPath = FindAugmentingPath<TGraph, TEdge, TNode>(graph, currentMatching, graphCounter);
            if (augmentingPath.Count > 0)
            {
                AugmentMatchingAlongPath<TEdge, TNode>(currentMatching, augmentingPath);
                return RecursiveFindMaximumMatching<TGraph, TEdge, TNode>(graph, currentMatching, graphCounter);
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
        /// <param name="graphCounter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns><see langword="true"/> if a matching of size <paramref name="requiredSize"/> exists in <paramref name="graph"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> or <paramref name="currentMatching"/> is <see langword="null"/>.</exception>
        private static bool RecursiveHasMatchingOfAtLeast<TGraph, TEdge, TNode>(TGraph graph, int requiredSize, List<TEdge> currentMatching, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), $"Trying to recursively find a matching of size at least {requiredSize} in a graph, but the graph is null!");
            Utils.NullCheck(currentMatching, nameof(currentMatching), $"Trying to recursively find a matching of size at least {requiredSize} in a graph, but the current matching is null!");
#endif
            if (currentMatching.Count >= requiredSize)
            {
                return true;
            }

            List<TEdge> augmentingPath = FindAugmentingPath<TGraph, TEdge, TNode>(graph, currentMatching, graphCounter);
            if (augmentingPath.Count > 0)
            {
                AugmentMatchingAlongPath<TEdge, TNode>(currentMatching, augmentingPath);
                return RecursiveHasMatchingOfAtLeast<TGraph, TEdge, TNode>(graph, requiredSize, currentMatching, graphCounter);
            }

            return false;
        }

        /// <summary>
        /// Augments a matching along a path.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <param name="matching">The current matching.</param>
        /// <param name="path">The augmenting path to augment the matching along.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="matching"/> or <paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="path"/> is not an augmenting path.</exception>
        private static void AugmentMatchingAlongPath<TEdge, TNode>(List<TEdge> matching, List<TEdge> path) where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(matching, nameof(matching), "Trying to augment a matching along a path, but the matching is null!");
            Utils.NullCheck(path, nameof(path), "Trying to augment a matching along a path, but the path is null!");
            if (!IsAugmentingPath<TEdge, TNode>(path, matching))
            {
                throw new InvalidOperationException("Trying to augment along a path, but the path is not an augmenting path!");
            }
#endif
            for (int i = 1; i < path.Count - 1; i += 2)
            {
                matching.Remove(path[i]);
            }
            for (int i = 0; i < path.Count; i += 2)
            {
                matching.Add(path[i]);
            }
        }

        /// <summary>
        /// Checks whether <paramref name="path"/> is an augmenting path given <paramref name="matching"/>.
        /// </summary>
        /// <typeparam name="TNode">The types of nodes used.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <param name="path">The <see cref="List{T}"/> for which we want to know if it is an augmenting path.</param>
        /// <param name="matching">The <see cref="List{T}"/> with the current matching.</param>
        /// <returns><see langword="true"/> if <paramref name="path"/> is an augmenting path given <paramref name="matching"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static bool IsAugmentingPath<TEdge, TNode>(List<TEdge> path, List<TEdge> matching) where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
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

            HashSet<TEdge> hashedMatching = new(matching);

            // Check if the alternating no-yes-no-yes-...-yes-no path is correct given the matching.
            for (int i = 1; i < path.Count - 1; i += 2)
            {
                if (!hashedMatching.Contains(path[i]))
                {
                    return false;
                }
            }
            for (int i = 0; i < path.Count; i += 2)
            {
                if (hashedMatching.Contains(path[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Find an augmenting path in <paramref name="graph"/> given <paramref name="matching"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to find the matching in.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <paramref name="graph"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> in which to find the augmenting path.</param>
        /// <param name="matching">The <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges currently in the matching.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns>An empty <see cref="List{T}"/> if no augmenting path can be found, or a <see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the edges on the augmenting path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> or <paramref name="matching"/> is <see langword="null"/>.</exception>
        private static List<TEdge> FindAugmentingPath<TGraph, TEdge, TNode>(TGraph graph, List<TEdge> matching, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find an augmenting path in a graph, but the graph is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to find an augmenting path in a graph, but the current matching is null!");
#endif
            HashSet<TNode> unmatchedVertices = new(graph.Nodes(graphCounter));
            foreach (TEdge edge in matching)
            {
                unmatchedVertices.Remove(edge.Endpoint1);
                unmatchedVertices.Remove(edge.Endpoint2);
            }

            HashSet<TNode> adjacentVertices = new();
            foreach (TNode node in unmatchedVertices)
            {
                foreach (TNode neighbour in node.Neighbours(graphCounter))
                {
                    adjacentVertices.Add(neighbour);
                }
            }

            Graph d = CreateGraphD(graph, matching, graphCounter, out Dictionary<Node, TNode> nodesInDToOriginalNodes, out Dictionary<TNode, Node> originalNodesToNodesInD, out Dictionary<Edge<Node>, (TEdge, TEdge)> edgesInDToOriginalEdges);

            Node startNode = originalNodesToNodesInD[unmatchedVertices.First(n => originalNodesToNodesInD.ContainsKey(n))];
            HashSet<Node> targetSet = adjacentVertices.Where(n => originalNodesToNodesInD.ContainsKey(n)).Select(n => originalNodesToNodesInD[n]).Where(n => n != startNode).ToHashSet();
            List<Edge<Node>> pathP = BFS.FindShortestPath<Graph, Edge<Node>, Node>(d, startNode, targetSet, graphCounter);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the digraph D with an arc that represents an unmatched edge followed by a matched edge in the original graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of original graph.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the original graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the original graph.</typeparam>
        /// <param name="graph">The original <typeparamref name="TGraph"/>.</param>
        /// <param name="matching">The current matching.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <param name="nodesInDToOriginalNodes"><see cref="Dictionary{TKey, TValue}"/> from nodes in D to the original <typeparamref name="TNode"/>s.</param>
        /// <param name="originalNodesToNodesInD"><see cref="Dictionary{TKey, TValue}"/> from original <typeparamref name="TNode"/>s to the nodes in D.</param>
        /// <param name="edgesInDToOriginalEdges"><see cref="Dictionary{TKey, TValue}"/> from edges in D to the tuple of <typeparamref name="TEdge"/>s it was formed by.</param>
        /// <returns>A directed <see cref="Graph"/> with an arc for each pair of an unmatched <typeparamref name="TEdge"/> followed by a matched <typeparamref name="TEdge"/>.</returns>
        private static Graph CreateGraphD<TGraph, TEdge, TNode>(TGraph graph, List<TEdge> matching, Counter graphCounter, out Dictionary<Node, TNode> nodesInDToOriginalNodes, out Dictionary<TNode, Node> originalNodesToNodesInD, out Dictionary<Edge<Node>, (TEdge, TEdge)> edgesInDToOriginalEdges) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
            nodesInDToOriginalNodes = new();
            originalNodesToNodesInD = new();
            edgesInDToOriginalEdges = new();

            HashSet<TEdge> matchedEdges = new(matching);
            IEnumerable<TEdge> unmatchedEdges = graph.Edges(graphCounter).Where(e => !matchedEdges.Contains(e));

            List<Node> nodesInD = new();
            List<Edge<Node>> edgesInD = new();

            foreach (TEdge unmatchedEdge in unmatchedEdges)
            {
                foreach (TEdge matchedEdge in matching)
                {
                    if (unmatchedEdge.Endpoint1 == matchedEdge.Endpoint1)
                    {
                        AddArcToD(unmatchedEdge.Endpoint2, matchedEdge.Endpoint2, unmatchedEdge, matchedEdge, nodesInDToOriginalNodes, originalNodesToNodesInD, edgesInDToOriginalEdges, nodesInD, edgesInD);
                    }
                    else if (unmatchedEdge.Endpoint1 == matchedEdge.Endpoint2)
                    {
                        AddArcToD(unmatchedEdge.Endpoint2, matchedEdge.Endpoint1, unmatchedEdge, matchedEdge, nodesInDToOriginalNodes, originalNodesToNodesInD, edgesInDToOriginalEdges, nodesInD, edgesInD);
                    }
                    else if (unmatchedEdge.Endpoint2 == matchedEdge.Endpoint1)
                    {
                        AddArcToD(unmatchedEdge.Endpoint1, matchedEdge.Endpoint2, unmatchedEdge, matchedEdge, nodesInDToOriginalNodes, originalNodesToNodesInD, edgesInDToOriginalEdges, nodesInD, edgesInD);
                    }
                    else if (unmatchedEdge.Endpoint2 == matchedEdge.Endpoint2)
                    {
                        AddArcToD(unmatchedEdge.Endpoint1, matchedEdge.Endpoint1, unmatchedEdge, matchedEdge, nodesInDToOriginalNodes, originalNodesToNodesInD, edgesInDToOriginalEdges, nodesInD, edgesInD);
                    }
                }
            }

            Graph d = new();
            d.AddNodes(nodesInD, MockCounter);
            d.AddEdges(edgesInD, MockCounter);

            return d;
        }

        /// <summary>
        /// Create an arc for the digraph D.
        /// </summary>
        /// <typeparam name="TEdge">The type of edges in the original graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the original graph.</typeparam>
        /// <param name="unmatchedEdgeEndpoint">The endpoint of the unmatched edge that should be a node in D.</param>
        /// <param name="matchedEdgeEndpoint">The endpoint of the matched edge that should be a node in D.</param>
        /// <param name="unmatchedEdge">The unmatched edge part of the arc in D.</param>
        /// <param name="matchedEdge">The matched edge part of the arc in D.</param>
        /// <param name="nodesInDToOriginalNodes"><see cref="Dictionary{TKey, TValue}"/> from nodes in D to the original <typeparamref name="TNode"/>s.</param>
        /// <param name="originalNodesToNodesInD"><see cref="Dictionary{TKey, TValue}"/> from original <typeparamref name="TNode"/>s to the nodes in D.</param>
        /// <param name="edgesInDToOriginalEdges"><see cref="Dictionary{TKey, TValue}"/> from edges in D to the tuple of <typeparamref name="TEdge"/>s it was formed by.</param>
        /// <param name="nodesInD"><see cref="List{T}"/> of nodes in D.</param>
        /// <param name="edgesInD"><see cref="List{T}"/> of edges in D.</param>
        private static void AddArcToD<TEdge, TNode>(TNode unmatchedEdgeEndpoint, TNode matchedEdgeEndpoint, TEdge unmatchedEdge, TEdge matchedEdge, Dictionary<Node, TNode> nodesInDToOriginalNodes, Dictionary<TNode, Node> originalNodesToNodesInD, Dictionary<Edge<Node>, (TEdge, TEdge)> edgesInDToOriginalEdges, List<Node> nodesInD, List<Edge<Node>> edgesInD) where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
            if (!originalNodesToNodesInD.TryGetValue(unmatchedEdgeEndpoint, out Node n1))
            {
                n1 = new Node(unmatchedEdgeEndpoint.ID);
                nodesInD.Add(n1);
                nodesInDToOriginalNodes[n1] = unmatchedEdgeEndpoint;
                originalNodesToNodesInD[unmatchedEdgeEndpoint] = n1;
            }

            if (!originalNodesToNodesInD.TryGetValue(matchedEdgeEndpoint, out Node n2))
            {
                n2 = new Node(matchedEdgeEndpoint.ID);
                nodesInD.Add(n2);
                nodesInDToOriginalNodes[n2] = matchedEdgeEndpoint;
                originalNodesToNodesInD[matchedEdgeEndpoint] = n2;
            }

            Edge<Node> edge = new(n1, n2, true);
            edgesInDToOriginalEdges[edge] = (unmatchedEdge, matchedEdge);
            edgesInD.Add(edge);
        }
    }
}
