// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that can generate a random caterpillar.
    /// </summary>
    public static class CaterpillarGenerator
    {
        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not impact performance.
        /// </summary>
        private static readonly Counter MockCounter = new Counter();

        /// <summary>
        /// Create a caterpillar with <paramref name="numberOfNodes"/> nodes.
        /// </summary>
        /// <param name="numberOfNodes">The required number of nodes in the caterpillar.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <returns>A randomly generated caterpillar with <paramref name="numberOfNodes"/> nodes.</returns>
        public static Tree<TreeNode> CreateCaterpillar(int numberOfNodes, Random random)
        {
#if !EXPERIMENT
            Utils.NullCheck(random, nameof(random), "Trying to create a random caterpillar, but the random is null!");
            if (numberOfNodes < 4)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfNodes), $"Trying to create a random caterpillar with {numberOfNodes} nodes, but caterpillars should have at least 4 nodes!");
            }
#endif
            // Determine the number of internal nodes and the number of children.
            // Since there are 2 I1-nodes, and at least 2 L1-leaves, we do - 4. (And +1 to make sure no extra leaves is also possible, making - 3.)
            int numberOfInternalNodes = random.Next(numberOfNodes - 3);
            int numberOfLeaves = numberOfNodes - 4 - numberOfInternalNodes;

            // Keep track of the ids we have encountered.
            uint nodeNumber = 0;

            // Create the left and right I1-nodes.
            TreeNode leftI1 = new TreeNode(nodeNumber++);
            TreeNode rightI1 = new TreeNode(nodeNumber++);

            Tree<TreeNode> tree = new Tree<TreeNode>();

            tree.AddRoot(leftI1, MockCounter);

            // List of internal nodes for the leaves to attach to.
            List<TreeNode> internalNodes = new List<TreeNode>() { leftI1, rightI1 };

            // Create all I2-nodes
            TreeNode last = leftI1;
            for (uint i = 0; i < numberOfInternalNodes; i++)
            {
                TreeNode next = new TreeNode(nodeNumber++);
                internalNodes.Add(next);
                tree.AddChild(last, next, MockCounter);
                last = next;
            }
            tree.AddChild(last, rightI1, MockCounter);

            // Give both I1-nodes a child.
            tree.AddChild(leftI1, new TreeNode(nodeNumber++), MockCounter);
            tree.AddChild(rightI1, new TreeNode(nodeNumber++), MockCounter);

            // Attach the required number of leaves uniform randomly to an internal node.
            for (uint i = 0; i < numberOfLeaves; i++)
            {
                TreeNode leaf = new TreeNode(nodeNumber++);
                tree.AddChild(internalNodes.PickRandom(random), leaf, MockCounter);
            }

            tree.UpdateNodeTypes();
            return tree;
        }
    }
}
