// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Text;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that can generate random <see cref="DemandPair"/>s.
    /// </summary>
    public static class RandomDemandPairs
    {
        /// <summary>
        /// Generate a <see cref="List{T}"/> of <paramref name="numberOfDemandPairs"/> random <see cref="DemandPair"/>s in <paramref name="tree"/>.
        /// </summary>
        /// <param name="numberOfDemandPairs">The required number of <see cref="DemandPair"/>s.</param>
        /// <param name="tree">The <see cref="Tree{N}"/> in which to generate the <see cref="DemandPair"/>s.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <returns>A <see cref="List{T}"/> of <paramref name="numberOfDemandPairs"/> random <see cref="DemandPair"/>s in <paramref name="tree"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfDemandPairs"/> is negative.</exception>
        public static List<DemandPair> GenerateRandomDemandPairs(int numberOfDemandPairs, Tree<TreeNode> tree, Random random)
        {
            Utils.NullCheck(tree, nameof(tree), $"Trying to generate random demand pairs in a tree, but the tree is null!");
            Utils.NullCheck(random, nameof(random), $"Trying to generate random demand pairs in a tree, but the random is null!");
            if (numberOfDemandPairs < 0)
            {
                throw new ArgumentOutOfRangeException($"Trying to generate random demand pairs in a tree, but the required number of demand pairs is negative!");
            }

            List<DemandPair> demandPairs = new List<DemandPair>();
            for (int i = 0; i < numberOfDemandPairs; i++)
            {
                TreeNode endpoint1 = tree.Nodes[random.Next(tree.NumberOfNodes)];
                TreeNode endpoint2;
                do
                {
                    endpoint2 = tree.Nodes[random.Next(tree.NumberOfNodes)];
                } while (endpoint2 == endpoint1);

                demandPairs.Add(new DemandPair(endpoint1, endpoint2));
            }
            return demandPairs;
        }
    }
}
