// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.MulticutProblem
{
    /// <summary>
    /// Class that represents an instance for the Multicut problem.
    /// </summary>
    public class MulticutInstance
    {
        /// <summary>
        /// The input <see cref="Tree"/>.
        /// </summary>
        public Tree<TreeNode> Tree { get; }

        /// <summary>
        /// The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.
        /// </summary>
        public CountedList<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The maximum size the cutset is allowed to be.
        /// </summary>
        public int K { get; }

        /// <summary>
        /// Constructor for a <see cref="MulticutInstance"/>.
        /// </summary>
        /// <param name="tree">The <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="k">The size the cutset is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="demandPairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="k"/> is smaller than or equal to zero.</exception>
        public MulticutInstance(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, int k)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create a multicut instance, but the tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create a multicut instance, but the list of demand pairs is null!");
            if (k <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(k), $"Trying to create a multicut instance, but the maximum amount of edges that can be removed is smaller than or equal to zero!");
            }
#endif
            Tree = tree;
            DemandPairs = demandPairs;
            K = k;
        }
    }
}
