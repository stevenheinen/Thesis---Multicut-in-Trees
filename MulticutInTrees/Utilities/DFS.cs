// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Class for Depth First Search
    /// </summary>
    public class DFS
    {
        /// <summary>
        /// Find all nodes that are connected to the given startnode.
        /// </summary>
        /// <typeparam name="N">Implementation of <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="startNode">The node to start with.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> with all <typeparamref name="N"/>s that are connected to <paramref name="startNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="startNode"/> is <see langword="null"/>.</exception>
        public static List<N> FindConnectedComponent<N>(N startNode, HashSet<N> seen = null) where N : ITreeNode<N>
        {
            if (startNode is null)
            {
                throw new ArgumentNullException("startNode", "Trying to find a connected component of an ITreeNode, but the start node is null!");
            }

            return FindConnectedComponent(startNode, default, seen);
        }

        /// <summary>
        /// Find all nodes that are connected to the given startnode.
        /// </summary>
        /// <typeparam name="N">Implementation of <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="startNode">The node to start with.</param>
        /// <param name="findNode">The node that needs to be found, or <see langword="default"/> if there is none.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> with all <typeparamref name="N"/>s that are connected to <paramref name="startNode"/>.</returns>
        private static List<N> FindConnectedComponent<N>(N startNode, N findNode, HashSet<N> seen = null) where N : ITreeNode<N>
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
            while (stack.Count > 0)
            {
                N node = stack.Pop();

                // Potentially push this node's children onto the stack.
                foreach (N child in node.Children)
                {
                    if (!seen.Contains(child))
                    {
                        // If we are looking for a specific node, and we find it, clear the rest of the result and return a list consisting only of the node we are looking for.
                        if (!(findNode is null) && child.Equals(findNode))
                        {
                            result.Clear();
                            result.Add(child);
                            return result;
                        }
                        result.Add(child);
                        seen.Add(child);
                        stack.Push(child);
                    }
                }

                // Besides children, we also have to check the parent.
                if (!(node.Parent is null) && !seen.Contains(node.Parent))
                {
                    // If we are looking for a specific node, and we find it, clear the rest of the result and return a list consisting only of the node we are looking for.
                    if (!(findNode is null) && node.Parent.Equals(findNode))
                    {
                        result.Clear();
                        result.Add(node.Parent);
                        return result;
                    }
                    result.Add(node.Parent);
                    seen.Add(node.Parent);
                    stack.Push(node.Parent);
                }
            }
            return result;
        }

        /// <summary>
        /// Find all connected components for an <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s.
        /// </summary>
        /// <typeparam name="N">Imlementation of <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="allNodes"><see cref="IEnumerable{T}"/> with all <typeparamref name="N"/>s.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the DFS.</param>
        /// <returns>A <see cref="List{T}"/> of connected components, where each connected component is also a <see cref="List{T}"/> by itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="allNodes"/> is <see langword="null"/>.</exception>
        public static List<List<N>> FindAllConnectedComponents<N>(IEnumerable<N> allNodes, HashSet<N> seen = null) where N : ITreeNode<N>
        {
            if (allNodes is null)
            {
                throw new ArgumentNullException("allNodes", "Trying to find all connected components of an IEnumerable with nodes, but the IEnumberable is null!");
            }

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
        public static bool AreConnected<N>(N node1, N node2, HashSet<N> seen = null) where N : ITreeNode<N>
        {
            if (node1 is null)
            {
                throw new ArgumentNullException("node1", "Trying to find whether two nodes are connected, but the first of these nodes is null!");
            }
            if (node2 is null)
            {
                throw new ArgumentNullException("node2", "Trying to find whether two nodes are connected, but the second of these nodes is null!");
            }

            List<N> connectedComponent = FindConnectedComponent(node1, node2, seen);
            return connectedComponent.Count == 1;
        }
    }
}
