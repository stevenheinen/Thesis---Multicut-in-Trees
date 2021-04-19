// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;
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
        /// Find the shortest path from an <typeparamref name="TNode"/> to any <typeparamref name="TNode"/> in <paramref name="targetSet"/>.
        /// </summary>
        /// <typeparam name="TNode">The type of node we are using.</typeparam>
        /// <param name="startNode">The <typeparamref name="TNode"/> to start with.</param>
        /// <param name="targetSet">The <see cref="HashSet{T}"/> of <typeparamref name="TNode"/>s that need to be found.</param>
        /// <param name="treeCounter">The <see cref="Counter"/> for graph operations that should count this BFS.</param>
        /// <param name="seen">Optional. Nodes in this <see cref="HashSet{T}"/> will be skipped during the BFS.</param>
        /// <returns>A <see cref="List{T}"/> with the shortest path from <paramref name="startNode"/> to any <typeparamref name="TNode"/> in <paramref name="targetSet"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="startNode"/>, <paramref name="targetSet"/> or <paramref name="treeCounter"/> is <see langword="null"/>.</exception>
        public static List<TNode> FindShortestPath<TNode>(TNode startNode, HashSet<TNode> targetSet, Counter treeCounter, HashSet<TNode> seen = null) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(startNode, nameof(startNode), "Trying to find the shortest path to a set, but the start node is null!");
            Utils.NullCheck(targetSet, nameof(targetSet), "Trying to find the shortest path to a set, but the set is null!");
            Utils.NullCheck(treeCounter, nameof(treeCounter), "Trying to find the shortest path to a set, but the counter is null!");
            if (targetSet.Count == 0)
            {
                throw new InvalidOperationException("Trying to find the shortest path to a set, but the set has no elements!");
            }
            if (targetSet.Contains(startNode))
            {
                throw new InvalidOperationException("Trying to find the shortest path to a set, but the startnode is already part of the set!");
            }
#endif
            Counter mockCounter = new Counter();
            seen ??= new HashSet<TNode>();
            Queue<TNode> queue = new Queue<TNode>();
            queue.Enqueue(startNode);
            seen.Add(startNode);

            // Keep track of which node pushed which node onto the stack to test for cycles.
            Dictionary<TNode, TNode> pushingNode = new Dictionary<TNode, TNode>
            {
                [startNode] = startNode
            };

            while (queue.Count > 0)
            {
                _ = treeCounter++;
                TNode node = queue.Dequeue();

                // Potentially push this node's neighbours onto the stack.
                foreach (TNode neighbour in node.Neighbours(mockCounter))
                {
                    if (seen.Contains(neighbour))
                    {
                        continue;
                    }

                    if (targetSet.Contains(neighbour))
                    {
                        List<TNode> path = new List<TNode>
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
