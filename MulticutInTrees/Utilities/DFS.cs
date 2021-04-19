// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Class for Depth First Search
    /// </summary>
    public static class DFS
    {
        /// <summary>
        /// Find all nodes that are connected to the given startnode.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="startNode">The node to start with.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> (for graph operations) that should be used during the DFS.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> with all <typeparamref name="TNode"/>s that are connected to <paramref name="startNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="startNode"/> or <paramref name="graphCounter"/> is <see langword="null"/>.</exception>
        public static List<TNode> FindConnectedComponent<TNode>(TNode startNode, Counter graphCounter, HashSet<TNode> seen = null) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(startNode, nameof(startNode), "Trying to find a connected component of an INode, but the start node is null!");
            Utils.NullCheck(graphCounter, nameof(graphCounter), "Trying to find a connected component of an INode, but the counter is null!");
#endif
            return FindConnectedComponent(startNode, default, graphCounter, seen);
        }

        /// <summary>
        /// Checks if a given graph of type <typeparamref name="TGraph"/> is acyclic.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="inputGraph">The <typeparamref name="TGraph"/> for which we want to know if it is acyclic.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> (for graph operations) that should be used during the DFS.</param>
        /// <returns><see langword="true"/> if <paramref name="inputGraph"/> is acyclic, <see langword="false"/> if it is cyclic.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputGraph"/> or <paramref name="graphCounter"/> is <see langword="null"/>.</exception>
        public static bool IsAcyclic<TGraph, TEdge, TNode>(TGraph inputGraph, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(inputGraph, nameof(inputGraph), "Trying to see if a graph is acyclic, but the graph is null!");
            Utils.NullCheck(graphCounter, nameof(graphCounter), "Trying to see if a graph is acyclic, but the counter is null!");
#endif
            if (inputGraph.NumberOfNodes(graphCounter) < 2)
            {
                return true;
            }
            return FindConnectedComponent(inputGraph.Nodes(graphCounter).First(), default, graphCounter, null, true).Count != 0;
        }

        /// <summary>
        /// Find all nodes that are connected to the given startnode.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="startNode">The node to start with.</param>
        /// <param name="findNode">The node that needs to be found, or <see langword="default"/> if there is none.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> (for graph operations) that should be used during the DFS.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <param name="acyclicCheck">Optional. If <see langword="true"/> and a cycle is encountered, an empty list will be returned. Ignored if <see langword="false"/>.</param>
        /// <param name="findPath">Optional. If <see langword="true"/>, <paramref name="findNode"/> should be given as well. The method will then return a list with all <typeparamref name="TNode"/>s on the path from <paramref name="startNode"/> to <paramref name="findNode"/>.</param>
        /// <returns>A <see cref="List{T}"/> with all <typeparamref name="TNode"/>s that are connected to <paramref name="startNode"/>.</returns>
        private static List<TNode> FindConnectedComponent<TNode>(TNode startNode, TNode findNode, Counter graphCounter, HashSet<TNode> seen = null, bool acyclicCheck = false, bool findPath = false) where TNode : AbstractNode<TNode>
        {
            Counter mockCounter = new Counter();

            List<TNode> result = new List<TNode>();
            seen ??= new HashSet<TNode>();
            Stack<TNode> stack = new Stack<TNode>();
            stack.Push(startNode);
            seen.Add(startNode);
            result.Add(startNode);

            // Keep track of which node pushed which node onto the stack to test for cycles.
            Dictionary<TNode, TNode> pushingNode = new Dictionary<TNode, TNode>();
            if (acyclicCheck || findPath)
            {
                pushingNode[startNode] = startNode;
            }

            while (stack.Count > 0)
            {
                TNode node = stack.Pop();

                // Potentially push this node's neighbours onto the stack.
                foreach (TNode neighbour in node.Neighbours(mockCounter))
                {
                    if (!seen.Contains(neighbour))
                    {
                        // If we are looking for a specific node, and we find it, clear the rest of the result and return a list consisting only of the node we are looking for.
                        if (!(findNode is null) && neighbour.Equals(findNode))
                        {
                            if (findPath)
                            {
                                List<TNode> path = new List<TNode>
                                {
                                    findNode
                                };
                                while (!node.Equals(startNode))
                                {
                                    path.Add(node);
                                    node = pushingNode[node];
                                }
                                path.Add(startNode);
                                return path;
                            }

                            result.Clear();
                            result.Add(neighbour);
                            return result;
                        }
                        _ = graphCounter++;
                        result.Add(neighbour);
                        seen.Add(neighbour);
                        stack.Push(neighbour);
                        if (acyclicCheck || findPath)
                        {
                            pushingNode[neighbour] = node;
                        }
                    }
                    // If we have seen this neighbour, are looking for cycles, and this neighbour is not the one that pushed the current node, we have found a cycle.
                    else if (acyclicCheck && !pushingNode[node].Equals(neighbour))
                    {
                        return new List<TNode>();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Computes the different caterpillar components in a set of nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> of nodes for which we want to compute the caterpillar components.</param>
        /// <param name="treeCounter">The <see cref="Counter"/> for tree operations.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> from <typeparamref name="TNode"/> to an identifier of the caterpillar component this <typeparamref name="TNode"/> is in, or -1 if it is not part of a caterpillar component.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> or <paramref name="treeCounter"/> is <see langword="null"/>.</exception>
        public static Dictionary<TNode, int> DetermineCaterpillarComponents<TNode>(IEnumerable<TNode> nodes, Counter treeCounter) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(nodes, nameof(nodes), "Trying to determine caterpillar components in a set of nodes, but the IEnumerable with nodes is null!");
            Utils.NullCheck(treeCounter, nameof(treeCounter), "Trying to determine caterpillar components in a set of nodes, but the counter is null!");
#endif
            Dictionary<TNode, int> result = new Dictionary<TNode, int>();

            if (!nodes.Any())
            {
                return result;
            }

            HashSet<TNode> seen = new HashSet<TNode>();
            Stack<TNode> stack = new Stack<TNode>();
            int caterpillarNumber = 0;
            TNode first = nodes.First();
            stack.Push(first);
            seen.Add(first);

            if (first.Type == NodeType.I2 || first.Type == NodeType.L2)
            {
                result[first] = caterpillarNumber++;
            }
            else
            {
                result[first] = -1;
            }

            while (stack.Count > 0)
            {
                TNode node = stack.Pop();

                foreach (TNode neighbour in node.Neighbours(treeCounter))
                {
                    if (seen.Contains(neighbour))
                    {
                        continue;
                    }

                    if (neighbour.Type == NodeType.I1 || neighbour.Type == NodeType.I3 || neighbour.Type == NodeType.L1 || neighbour.Type == NodeType.L3)
                    {
                        result[neighbour] = -1;
                    }
                    else if (node.Type == NodeType.I1 || node.Type == NodeType.I3)
                    {
                        result[neighbour] = caterpillarNumber++;
                    }
                    else
                    {
                        result[neighbour] = result[node];
                    }

                    seen.Add(neighbour);
                    stack.Push(neighbour);
                }
            }

            return result;
        }

        /// <summary>
        /// Find all connected components for an <see cref="IEnumerable{T}"/> of <typeparamref name="TNode"/>s.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="allNodes"><see cref="IEnumerable{T}"/> with all <typeparamref name="TNode"/>s.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> (for graph operations) that should be used during the DFS.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> of connected components, where each connected component is also a <see cref="List{T}"/> by itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="allNodes"/> or <paramref name="graphCounter"/> is <see langword="null"/>.</exception>
        public static List<List<TNode>> FindAllConnectedComponents<TNode>(IEnumerable<TNode> allNodes, Counter graphCounter, HashSet<TNode> seen = null) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(allNodes, nameof(allNodes), "Trying to find all connected components of an IEnumerable with nodes, but the IEnumberable is null!");
            Utils.NullCheck(graphCounter, nameof(graphCounter), "Trying to find all connected components of an IEnumerable with nodes, but the counter is null!");
#endif
            List<List<TNode>> result = new List<List<TNode>>();
            seen ??= new HashSet<TNode>();
            foreach (TNode node in allNodes)
            {
                if (!seen.Contains(node))
                {
                    seen.Add(node);
                    List<TNode> component = FindConnectedComponent(node, graphCounter, seen);
                    result.Add(component);
                    foreach (TNode found in component)
                    {
                        seen.Add(found);
                    }
                }
            }

            return result;
        }

        // todo: delete?
        /// <summary>
        /// Checks whether two <typeparamref name="TNode"/>s are connected to each other.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="node1">The first <typeparamref name="TNode"/>.</param>
        /// <param name="node2">The second <typeparamref name="TNode"/>.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> (for graph operations) that should be used during the DFS.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns><see langword="true"/> if <paramref name="node1"/> and <paramref name="node2"/> are connected, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node1"/>, <paramref name="node2"/> or <paramref name="graphCounter"/> is <see langword="null"/>.</exception>
        public static bool AreConnected<TNode>(TNode node1, TNode node2, Counter graphCounter, HashSet<TNode> seen = null) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(node1, nameof(node1), "Trying to find whether two nodes are connected, but the first of these nodes is null!");
            Utils.NullCheck(node2, nameof(node2), "Trying to find whether two nodes are connected, but the second of these nodes is null!");
            Utils.NullCheck(graphCounter, nameof(graphCounter), "Trying to find whether two nodes are connected, but the counter is null!");
#endif
            List<TNode> connectedComponent = FindConnectedComponent(node1, node2, graphCounter, seen);
            return connectedComponent.Count == 1;
        }

        /// <summary>
        /// Finds all <typeparamref name="TNode"/>s on the path from <paramref name="node1"/> to <paramref name="node2"/>.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="node1">The start <typeparamref name="TNode"/>.</param>
        /// <param name="node2">The goal <typeparamref name="TNode"/>.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> (for graph operations) that should be used during the DFS.</param>
        /// <param name="seen">Optional. <typeparamref name="TNode"/>s in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="TNode"/>s with all <typeparamref name="TNode"/>s on the path from <paramref name="node1"/> to <paramref name="node2"/>/</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node1"/>, <paramref name="node2"/> or <paramref name="graphCounter"/> is <see langword="null"/>.</exception>
        public static List<TNode> FindPathBetween<TNode>(TNode node1, TNode node2, Counter graphCounter, HashSet<TNode> seen = null) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(node1, nameof(node1), "Trying to find whether two nodes are connected, but the first of these nodes is null!");
            Utils.NullCheck(node2, nameof(node2), "Trying to find whether two nodes are connected, but the second of these nodes is null!");
            Utils.NullCheck(graphCounter, nameof(graphCounter), "Trying to find whether two nodes are connected, but counter is null!");
#endif
            List<TNode> result = FindConnectedComponent(node1, node2, graphCounter, seen, false, true);
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Finds all <typeparamref name="TNode"/>s that are "free" considering <paramref name="unmatchedNodes"/> and <paramref name="matching"/>.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes used.</typeparam>
        /// <param name="unmatchedNodes"><see cref="List{T}"/> of <typeparamref name="TNode"/>s that are unmatched considering <paramref name="matching"/>.</param>
        /// <param name="matching"><see cref="HashSet{T}"/> with tuples of two <typeparamref name="TNode"/>s representing edges in the current matching.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> (for graph operations) that should be used during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="TNode"/>s that are reachable from any <typeparamref name="TNode"/> in <paramref name="unmatchedNodes"/> considering <paramref name="matching"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="unmatchedNodes"/>, <paramref name="matching"/> or <paramref name="graphCounter"/> is <see langword="null"/>.</exception>
        public static List<TNode> FreeNodes<TNode>(List<TNode> unmatchedNodes, HashSet<(TNode, TNode)> matching, Counter graphCounter) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(unmatchedNodes, nameof(unmatchedNodes), "Trying to find all free nodes, but the list with unmatched nodes is null!");
            Utils.NullCheck(matching, nameof(matching), "Trying to find all free nodes, but the list with the current matching is null!");
            Utils.NullCheck(graphCounter, nameof(graphCounter), "Trying to find all free nodes, but the counter is null!");
#endif
            Counter mockCounter = new Counter();

            HashSet<TNode> seen = new HashSet<TNode>();
            List<TNode> result = new List<TNode>();

            // Bool in the stack means: NextShouldBeInMatching
            Stack<(TNode, bool, int)> stack = new Stack<(TNode, bool, int)>();
            foreach (TNode node in unmatchedNodes)
            {
                stack.Push((node, false, 1));
                seen.Add(node);
            }

            while (stack.Count > 0)
            {
                (TNode node, bool nextShouldBeMatched, int pathLength) = stack.Pop();
                foreach (TNode neighbour in node.Neighbours(mockCounter))
                {
                    if (seen.Contains(neighbour))
                    {
                        continue;
                    }
                    if (!nextShouldBeMatched && (matching.Contains((node, neighbour)) || matching.Contains((neighbour, node))))
                    {
                        continue;
                    }
                    if (nextShouldBeMatched && !matching.Contains((node, neighbour)) && !matching.Contains((neighbour, node)))
                    {
                        continue;
                    }
                    _ = graphCounter++;
                    seen.Add(neighbour);
                    stack.Push((neighbour, !nextShouldBeMatched, pathLength + 1));
                    if (pathLength % 2 == 0)
                    {
                        result.Add(neighbour);
                    }
                }
            }

            return result;
        }
    }
}
