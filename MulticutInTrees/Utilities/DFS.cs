// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using MulticutInTrees.Exceptions;
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
        /// <typeparam name="N">Implementation of <see cref="INode{N}"/>.</typeparam>
        /// <param name="startNode">The node to start with.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> with all <typeparamref name="N"/>s that are connected to <paramref name="startNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="startNode"/> is <see langword="null"/>.</exception>
        public static List<N> FindConnectedComponent<N>(N startNode, HashSet<N> seen = null) where N : INode<N>
        {
            Utils.NullCheck(startNode, nameof(startNode), "Trying to find a connected component of an INode, but the start node is null!");

            return FindConnectedComponent(startNode, default, seen);
        }

        /// <summary>
        /// Checks if a given graph of type <typeparamref name="G"/> is acyclic.
        /// </summary>
        /// <typeparam name="G">The type of graph.</typeparam>
        /// <typeparam name="N">The type of nodes in the graph.</typeparam>
        /// <param name="inputGraph">The <typeparamref name="G"/> for which we want to know if it is acyclic.</param>
        /// <returns><see langword="true"/> if <paramref name="inputGraph"/> is acyclic, <see langword="false"/> if it is cyclic.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputGraph"/> is <see langword="null"/>.</exception>
        public static bool IsAcyclicGraph<G, N>(G inputGraph) where G : IGraph<N> where N : INode<N>
        {
            Utils.NullCheck(inputGraph, nameof(inputGraph), $"Trying to see if a graph is acyclic, but the graph is null!");

            if (inputGraph.NumberOfNodes < 2)
            {
                return true;
            }
            return FindConnectedComponent(inputGraph.Nodes[0], default, null, true).Count != 0;
        }

        /// <summary>
        /// Checks if a given tree of type <typeparamref name="T"/> is acyclic.
        /// </summary>
        /// <typeparam name="T">The type of tree.</typeparam>
        /// <typeparam name="N">The type of nodes in the tree.</typeparam>
        /// <param name="inputTree">The <typeparamref name="T"/> for which we want to know if it is acyclic.</param>
        /// <returns><see langword="true"/> if <paramref name="inputTree"/> is acyclic, <see langword="false"/> if it is cyclic.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputTree"/> is <see langword="null"/>.</exception>
        /// <exception cref="NoRootException">Thrown when the <see cref="ITree{N}.Root"/> of <paramref name="inputTree"/> is <see langword="null"/>.</exception>
        public static bool IsAcyclicTree<T, N>(T inputTree) where T : ITree<N> where N : ITreeNode<N>
        {
            Utils.NullCheck(inputTree, nameof(inputTree), $"Trying to see if a tree is acyclic, but the tree is null!");

            if (inputTree.Root is null)
            {
                throw new NoRootException($"Trying to see if {inputTree} is acyclic, but it has no root!");
            }

            if (inputTree.NumberOfNodes < 2)
            {
                return true;
            }
            return FindConnectedComponent(inputTree.Root, default, null, true).Count != 0;
        }

        /// <summary>
        /// Find all nodes that are connected to the given startnode.
        /// </summary>
        /// <typeparam name="N">Implementation of <see cref="INode{N}"/>.</typeparam>
        /// <param name="startNode">The node to start with.</param>
        /// <param name="findNode">The node that needs to be found, or <see langword="default"/> if there is none.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <param name="acyclicCheck">Optional. If <see langword="true"/> and a cycle is encountered, an empty list will be returned. Ignored if <see langword="false"/>.</param>
        /// <param name="findPath">Optional. If <see langword="true"/>, <paramref name="findNode"/> should be given as well. The method will then return a list with all <typeparamref name="N"/>s on the path from <paramref name="startNode"/> to <paramref name="findNode"/>.</param>
        /// <returns>A <see cref="List{T}"/> with all <typeparamref name="N"/>s that are connected to <paramref name="startNode"/>.</returns>
        private static List<N> FindConnectedComponent<N>(N startNode, N findNode, HashSet<N> seen = null, bool acyclicCheck = false, bool findPath = false) where N : INode<N>
        {
            List<N> result = new List<N>();
            if (seen is null)
            {
                seen = new HashSet<N>();
            }
            Stack<N> stack = new Stack<N>();
            stack.Push(startNode);
            seen.Add(startNode);
            result.Add(startNode);

            // Keep track of which node pushed which node onto the stack to test for cycles.
            Dictionary<N, N> pushingNode = new Dictionary<N, N>();
            if (acyclicCheck || findPath)
            {
                pushingNode[startNode] = startNode;
            }

            while (stack.Count > 0)
            {
                N node = stack.Pop();

                // Potentially push this node's neighbours onto the stack.
                foreach (N neighbour in node.Neighbours)
                {
                    if (!seen.Contains(neighbour))
                    {
                        // If we are looking for a specific node, and we find it, clear the rest of the result and return a list consisting only of the node we are looking for.
                        if (!(findNode is null) && neighbour.Equals(findNode))
                        {
                            if (findPath)
                            {
                                List<N> path = new List<N>
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
                        return new List<N>();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Find all connected components for an <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s.
        /// </summary>
        /// <typeparam name="N">Imlementation of <see cref="INode{N}"/>.</typeparam>
        /// <param name="allNodes"><see cref="IEnumerable{T}"/> with all <typeparamref name="N"/>s.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> of connected components, where each connected component is also a <see cref="List{T}"/> by itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="allNodes"/> is <see langword="null"/>.</exception>
        public static List<List<N>> FindAllConnectedComponents<N>(IEnumerable<N> allNodes, HashSet<N> seen = null) where N : INode<N>
        {
            Utils.NullCheck(allNodes, nameof(allNodes), "Trying to find all connected components of an IEnumerable with nodes, but the IEnumberable is null!");

            List<List<N>> result = new List<List<N>>();
            if (seen is null)
            {
                seen = new HashSet<N>();
            }
            foreach (N node in allNodes)
            {
                if (!seen.Contains(node))
                {
                    seen.Add(node);
                    List<N> component = FindConnectedComponent(node, seen);
                    result.Add(component);
                    foreach (N found in component)
                    {
                        seen.Add(found);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks whether two <typeparamref name="N"/>s are connected to each other.
        /// </summary>
        /// <typeparam name="N">Implementation of <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="node1">The first <typeparamref name="N"/>.</param>
        /// <param name="node2">The second <typeparamref name="N"/>.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns><see langword="true"/> if <paramref name="node1"/> and <paramref name="node2"/> are connected, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node1"/> or <paramref name="node2"/> is <see langword="null"/>.</exception>
        public static bool AreConnected<N>(N node1, N node2, HashSet<N> seen = null) where N : INode<N>
        {
            Utils.NullCheck(node1, nameof(node1), "Trying to find whether two nodes are connected, but the first of these nodes is null!");
            Utils.NullCheck(node2, nameof(node2), "Trying to find whether two nodes are connected, but the second of these nodes is null!");

            List<N> connectedComponent = FindConnectedComponent(node1, node2, seen);
            return connectedComponent.Count == 1;
        }

        /// <summary>
        /// Finds all <typeparamref name="N"/>s on the path from <paramref name="node1"/> to <paramref name="node2"/>.
        /// </summary>
        /// <typeparam name="N">Implementation of <see cref="INode{N}"/>.</typeparam>
        /// <param name="node1">The start <typeparamref name="N"/>.</param>
        /// <param name="node2">The goal <typeparamref name="N"/>.</param>
        /// <param name="seen">Optional. <typeparamref name="N"/>s in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="N"/>s with all <typeparamref name="N"/>s on the path from <paramref name="node1"/> to <paramref name="node2"/>/</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node1"/> or <paramref name="node2"/> is <see langword="null"/>.</exception>
        public static List<N> FindPathBetween<N>(N node1, N node2, HashSet<N> seen = null) where N : INode<N>
        {
            Utils.NullCheck(node1, nameof(node1), "Trying to find whether two nodes are connected, but the first of these nodes is null!");
            Utils.NullCheck(node2, nameof(node2), "Trying to find whether two nodes are connected, but the second of these nodes is null!");

            List<N> result = FindConnectedComponent(node1, node2, seen, false, true);
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Finds all edges in <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="G">The type of graph. Must implement <see cref="IGraph{N}"/>.</typeparam>
        /// <typeparam name="N">The type of nodes in the graph. Must implement <see cref="INode{N}"/>.</typeparam>
        /// <param name="graph">The <typeparamref name="G"/> in which we want to find all edges.</param>
        /// <returns>A <see cref="List{T}"/> of tuples of <typeparamref name="N"/>s that represent the edges.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> is <see langword="null"/>.</exception>
        public static List<(N, N)> FindAllEdgesGraph<G, N>(G graph) where G : IGraph<N> where N : INode<N>
        {
            Utils.NullCheck(graph, nameof(graph), "Trying to find all edges in a graph, but the graph is null!");

            List<(N, N)> result = new List<(N, N)>();
            if (graph.Nodes.Count == 0)
            {
                return result;
            }
            Stack<N> stack = new Stack<N>();
            stack.Push(graph.Nodes[0]);
            HashSet<(N, N)> seen = new HashSet<(N, N)>();
            while (stack.Count > 0)
            {
                N node = stack.Pop();
                foreach (N child in node.Neighbours)
                {
                    if (seen.Contains((node, child)) || seen.Contains((child, node)))
                    {
                        continue;
                    }
                    result.Add((node, child));
                    seen.Add((node, child));
                    seen.Add((child, node));
                    stack.Push(child);
                }
            }
            return result;
        }

        /// <summary>
        /// Finds all edges in <paramref name="tree"/>.
        /// </summary>
        /// <typeparam name="T">The type of tree. Must implement <see cref="ITree{N}"/>.</typeparam>
        /// <typeparam name="N">The type of nodes in the tree. Must implement <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="tree">The <typeparamref name="T"/> in which we want to find all edges.</param>
        /// <returns>A <see cref="List{T}"/> of tuples of <typeparamref name="N"/>s that represent the edges.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> is <see langword="null"/>.</exception>
        public static List<(N, N)> FindAllEdgesTree<T, N>(T tree) where T : ITree<N> where N : ITreeNode<N>
        {
            Utils.NullCheck(tree, nameof(tree), "Trying to find all edges in a tree, but the tree is null!");

            List<(N, N)> result = new List<(N, N)>();
            if (tree.Root is null)
            {
                return result;
            }
            Stack<N> stack = new Stack<N>();
            stack.Push(tree.Root);
            while (stack.Count > 0)
            {
                N node = stack.Pop();
                foreach (N child in node.Children)
                {
                    result.Add((node, child));
                    stack.Push(child);
                }
            }
            return result;
        }
    }
}
