// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Text;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Implementation of Breadth First Search.
    /// </summary>
    public static class BFS
    {
        /// <summary>
        /// Find the shortest path from an <typeparamref name="N"/> to any <typeparamref name="N"/> in <paramref name="targetSet"/>.
        /// </summary>
        /// <typeparam name="N">Implementation of <see cref="INode{N}"/>.</typeparam>
        /// <param name="startNode">The <typeparamref name="N"/> to start with.</param>
        /// <param name="targetSet">The <see cref="HashSet{T}"/> of <typeparamref name="N"/>s that need to be found.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the BFS.</param>
        /// <returns>A <see cref="List{T}"/> with the shortest path from <paramref name="startNode"/> to any <typeparamref name="N"/> in <paramref name="targetSet"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="startNode"/> or <paramref name="targetSet"/> is <see langword="null"/>.</exception>
        public static List<N> FindShortestPath<N>(N startNode, HashSet<N> targetSet, HashSet <N> seen = null) where N : INode<N>
        {
            Utils.NullCheck(startNode, nameof(startNode), $"Trying to find the shortest path to a set, but the start node is null!");
            Utils.NullCheck(targetSet, nameof(targetSet), $"Trying to find the shortest path to a set, but the set is null!");
            if (targetSet.Count == 0)
            {
                throw new InvalidOperationException($"Trying to find the shortest path to a set, but the set has no elements!");
            }
            if (targetSet.Contains(startNode))
            {
                throw new InvalidOperationException($"Trying to find the shortest path to a set, but the startnode is already part of the set!");
            }

            if (seen is null)
            {
                seen = new HashSet<N>();
            }
            Queue<N> queue = new Queue<N>();
            queue.Enqueue(startNode);
            seen.Add(startNode);

            // Keep track of which node pushed which node onto the stack to test for cycles.
            Dictionary<N, N> pushingNode = new Dictionary<N, N>
            {
                [startNode] = startNode
            };

            while (queue.Count > 0)
            {
                N node = queue.Dequeue();

                // Potentially push this node's neighbours onto the stack.
                foreach (N neighbour in node.Neighbours)
                {
                    if (seen.Contains(neighbour))
                    {
                        continue;
                    }

                    if (targetSet.Contains(neighbour))
                    {
                        List<N> path = new List<N>
                        {
                            neighbour
                        };
                        while (!node.Equals(startNode))
                        {
                            path.Add(node);
                            node = pushingNode[node];
                        }
                        path.Add(startNode);
                        path.Reverse();
                        return path;
                    }

                    seen.Add(neighbour);
                    queue.Enqueue(neighbour);
                    pushingNode[neighbour] = node;
                }
            }

            throw new NotInGraphException($"There is no path between {startNode} and a node in {targetSet.Print()}!");
        }
    }
}
