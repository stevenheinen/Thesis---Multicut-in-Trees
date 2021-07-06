// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that generates a binary tree, that is, a tree with bounded degree 3.
    /// </summary>
    public static class BinaryTreeGenerator
    {
        /// <summary>
        /// Creates a tree with a bounded degree 3.
        /// </summary>
        /// <param name="numberOfNodes">The number of nodes in the tree.</param>
        /// <returns>A <see cref="Graph"/> that is a tree with bounded degree 3.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfNodes"/> is smaller than 1.</exception>
        public static Graph CreateBinaryTree(int numberOfNodes)
        {
#if !EXPERIMENT
            if (numberOfNodes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfNodes), $"Trying to create a binary tree, but the number of nodes ({numberOfNodes}) is smaller than 1!");
            }    
#endif
            Counter mockCounter = new();
            Graph tree = new();
            Queue<Node> parents = new();
            uint nodeCounter = 0;
            Node node = new(nodeCounter++);
            tree.AddNode(node, mockCounter);
            parents.Enqueue(node);
            if (nodeCounter < numberOfNodes)
            {
                Node child = new(nodeCounter++);
                tree.AddNode(child, mockCounter);
                tree.AddEdge(new Edge<Node>(node, child), mockCounter);
                parents.Enqueue(child);
            }
            while (parents.Count > 0 && nodeCounter < numberOfNodes)
            {
                Node parent = parents.Dequeue();
                Node child1 = new(nodeCounter++);
                tree.AddNode(child1, mockCounter);
                tree.AddEdge(new Edge<Node>(parent, child1), mockCounter);
                parents.Enqueue(child1);

                if (nodeCounter < numberOfNodes)
                {
                    Node child2 = new(nodeCounter++);
                    tree.AddNode(child2, mockCounter);
                    tree.AddEdge(new Edge<Node>(parent, child2), mockCounter);
                    parents.Enqueue(child2);
                }
            }
            tree.UpdateNodeTypes();
            return tree;
        }
    }
}
