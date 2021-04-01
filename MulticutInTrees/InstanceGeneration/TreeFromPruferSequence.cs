// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that is able to generate a random tree from a Prüfer sequence.
    /// </summary>
    public static class TreeFromPruferSequence
    {
        /// <summary>
        /// Generates a random <see cref="Tree{N}"/> with <paramref name="numberOfNodes"/> <see cref="TreeNode"/>s using a random Prüfer sequence.
        /// </summary>
        /// <param name="numberOfNodes">The required number of nodes in the resulting tree. Should be at least 3.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <returns>A <see cref="Tree{N}"/> with <see cref="TreeNode"/>s that is randomly generated using a Prüfer sequence.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfNodes"/> is less than three.</exception>
        public static Tree<TreeNode> GenerateTree(int numberOfNodes, Random random)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(random, nameof(random), "Trying to generate a random tree from a Prüfer sequence, but the random is null!");
            if (numberOfNodes < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfNodes), "A tree generated with a Prüfer sequence should have at least 3 nodes!");
            }
#endif
            Counter counter = new Counter();

            List<(int, int)> edges = GenerateEdgesInTree(numberOfNodes, random);

            return Utils.CreateTreeWithEdges(numberOfNodes, edges);
        }

        /// <summary>
        /// Generate a <see cref="List{T}"/> with <see cref="int"/> tuples representing the edges in a tree with <paramref name="numberOfNodes"/> nodes.
        /// </summary>
        /// <param name="numberOfNodes">The required number of nodes in the resulting tree.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <returns>A <see cref="List{T}"/> with tuples of <see cref="int"/>s that represent the edges in the tree.</returns>
        private static List<(int, int)> GenerateEdgesInTree(int numberOfNodes, Random random)
        {
            int[] prufer = GeneratePruferSequence(numberOfNodes - 2, random);
            int[] vertices = new int[numberOfNodes];

            for (int i = 0; i < numberOfNodes - 2; i++)
            {
                vertices[prufer[i]]++;
            }

            List<(int, int)> edges = new List<(int, int)>();
            for (int i = 0; i < numberOfNodes - 2; i++)
            {
                for (int k = 0; k < numberOfNodes; k++)
                {
                    if (vertices[k] == 0)
                    {
                        vertices[k] = -1;
                        edges.Add((k, prufer[i]));
                        vertices[prufer[i]]--;
                        break;
                    }
                }
            }

            int j = 0;
            (int, int) edge = (-1, -1);
            for (int i = 0; i < numberOfNodes; i++)
            {
                if (vertices[i] == 0 && j == 0)
                {
                    edge = (i, edge.Item2);
                    j++;
                }
                else if (vertices[i] == 0 && j == 1)
                {
                    edge = (edge.Item1, i);
                }
            }
            edges.Add(edge);

            return edges;
        }

        /// <summary>
        /// Generate a random Prüfer sequence with length <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The required length the sequence should be.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <returns>A randomly generated Prüfer sequence of length <paramref name="length"/>.</returns>
        private static int[] GeneratePruferSequence(int length, Random random)
        {
            int[] prufer = new int[length];
            for (int i = 0; i < length; i++)
            {
                prufer[i] = random.Next(length + 2);
            }
            return prufer;
        }
    }
}
