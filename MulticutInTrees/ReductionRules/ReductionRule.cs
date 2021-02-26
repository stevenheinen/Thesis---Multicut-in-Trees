// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// Abstract class to be used as baseclass for each reduction rule.
    /// </summary>
    public abstract class ReductionRule
    {        
        /// <summary>
        /// The input <see cref="Tree{N}"/>.
        /// </summary>
        protected Tree<TreeNode> Tree { get; }

        /// <summary>
        /// The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.
        /// </summary>
        protected CountedList<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.
        /// </summary>
        protected Algorithm Algorithm { get; }

        /// <summary>
        /// <see cref="Counter"/> that counts how many edges this <see cref="ReductionRule"/> contracted.
        /// </summary>
        protected Counter ContractedEdgesCounter { get; set; }

        /// <summary>
        /// <see cref="Counter"/> that counts how many <see cref="DemandPair"/>s this <see cref="ReductionRule"/> removed.
        /// </summary>
        protected Counter RemovedDemandPairsCounter { get; set; }

        /// <summary>
        /// <see cref="Counter"/> that counts how many <see cref="DemandPair"/>s this <see cref="ReductionRule"/> changed.
        /// </summary>
        protected Counter ChangedDemandPairsCounter { get; set; }

        /// <summary>
        /// <see cref="Stopwatch"/> that keeps track of the time spent checking whether this <see cref="ReductionRule"/> is applicable.
        /// </summary>
        protected Stopwatch TimeSpentCheckingApplicability { get; }

        /// <summary>
        /// The <see cref="System.Random"/> used for random number generation.
        /// </summary>
        protected Random Random { get; }

        /// <summary>
        /// Constructor for a <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithms.Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        /// <param name="random">The <see cref="System.Random"/> used for random number generation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        protected ReductionRule(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, Random random)
        {
            Utils.NullCheck(tree, nameof(tree), "Trying to create a reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create a reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create a reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(random, nameof(random), "Trying to create a reduction rule, but the random is null!");

            Tree = tree;
            DemandPairs = new CountedList<DemandPair>(demandPairs);
            Algorithm = algorithm;
            Random = random;

            ContractedEdgesCounter = new Counter();
            RemovedDemandPairsCounter = new Counter();
            ChangedDemandPairsCounter = new Counter();
            TimeSpentCheckingApplicability = new Stopwatch();

            Preprocess();
        }

        /// <summary>
        /// Everything that needs to happen before the reduction rule can be executed.
        /// </summary>
        protected abstract void Preprocess();

        /// <summary>
        /// Print all counters that this <see cref="ReductionRule"/> needed.
        /// </summary>
        internal virtual void PrintCounters()
        {
            Console.WriteLine($"DemandPairs:          {DemandPairs.OperationsCounter}");
            Console.WriteLine($"Contracted edges:     {ContractedEdgesCounter}");
            Console.WriteLine($"Removed demand pairs: {RemovedDemandPairsCounter}");
            Console.WriteLine($"Changed demand pairs: {ChangedDemandPairsCounter}");
            Console.WriteLine($"Time spent checking whether this rule is applicable: {TimeSpentCheckingApplicability.ElapsedTicks} ticks");
        }

        /// <summary>
        /// First iteration of this <see cref="ReductionRule"/>. There is no information about last iterations available.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool RunFirstIteration();

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after one or more edges have been contracted in the last iteration.
        /// </summary>
        /// <param name="contractedEdgeNodeTupleList">An <see cref="CountedList{T}"/> with tuples consisting of a tuple of <see cref="TreeNode"/>s (the contracted edge), a <see cref="TreeNode"/> (the result of the edge contraction), and a <see cref="List{T}"/> of <see cref="DemandPair"/>s (the <see cref="DemandPair"/>s on the contracted edge).</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)> contractedEdgeNodeTupleList);

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after one or more <see cref="DemandPair"/>s have been removed in the last iteration.
        /// </summary>
        /// <param name="removedDemandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s that were removed in the last iteration.</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs);

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after the endpoint of one or more <see cref="DemandPair"/>s were changed in the last iteration.
        /// </summary>
        /// <param name="changedEdgesPerDemandPairList">The <see cref="CountedList{T}"/> with tuples with a <see cref="List{T}"/> with tuples of <see cref="TreeNode"/>s that represent the edges that were removed from the demand path, and a <see cref="DemandPair"/>.</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterDemandPathChanged(CountedList<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList);
    }
}
