﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// Implementation of the Disjoint Paths reduction rule.
    /// <br/>
    /// <b>Rule:</b> If there are more than k pairwise edge-disjoint demand paths, then there is no solution with parameter value k.
    /// </summary>
    public class DisjointPaths : ReductionRule
    {
        /// <summary>
        /// The maximum size the solution is allowed to have.
        /// </summary>
        private int MaxSolutionSize { get; }

        /// <summary>
        /// The part of the solution that has been found thus far.
        /// </summary>
        private List<(TreeNode, TreeNode)> PartialSolution { get; }

        /// <summary>
        /// Constructor for the <see cref="DisjointPaths"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input tree.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public DisjointPaths(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, List<(TreeNode, TreeNode)> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm, true)
        {
#if !Experiment
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the Disjoint Paths reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the Disjoint Paths reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the Disjoint Paths reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(partialSolution, nameof(partialSolution), "Trying to create an instance of the Disjoint Paths reduction rule, but the list with the partial solution is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException("Trying to create an instance of the Disjoint Paths reduction rule, but the maximum solution size parameter is smaller than zero!");
            }
#endif
            PartialSolution = partialSolution;
            MaxSolutionSize = maxSolutionSize;
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="changedEdgesPerDemandPairList"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathChanged(CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
#if !EXPERIMENT
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Disjoint Paths reduction rule after a demand path was changed, but the list with information about changed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Executing Disjoint Paths rule after a demand path was changed.");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            return DisjointPathsGreaterThanK();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Disjoint Paths reduction rule after a demand path was removed, but the list with information about removeds demand paths is null!");
#endif
            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdgeNodeTupleList"/> is <see langword="null"/>.</exception>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to apply the Disjoint Paths reduction rule after an edge was contracted, but the list with information about contracted edges is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Executing Disjoint Paths rule after an edge was contracted.");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            return DisjointPathsGreaterThanK();
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine("Executing Disjoint Paths rule for the first time.");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            return DisjointPathsGreaterThanK();
        }

        /// <summary>
        /// Returns whether the current number of edge-disjoint demand pairs in the instance is larger than what can be added to the solution.
        /// </summary>
        /// <returns><see langword="true"/> if the current number of edge-disjoint demand pairs in the instance is larger than the amount of edges that can still be added to the solution, <see langword="false"/> otherwise.</returns>
        private bool DisjointPathsGreaterThanK()
        {
            List<(TreeNode, TreeNode)> commodities = DemandPairToCommodity();
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlow(Tree, commodities, Measurements);
            Measurements.TimeSpentCheckingApplicability.Stop();
            return flow > (MaxSolutionSize - PartialSolution.Count);
        }

        /// <summary>
        /// Transforms <see cref="ReductionRule.DemandPairs"/> into a list of pairs of endpoints that can be used as commodities for the disjoint-path algorithm.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> with, for each <see cref="DemandPair"/> in the instance, a tuple of two <see cref="TreeNode"/>s that are the endpoints of that <see cref="DemandPair"/>.</returns>
        private List<(TreeNode, TreeNode)> DemandPairToCommodity()
        {
            List<(TreeNode, TreeNode)> result = new List<(TreeNode, TreeNode)>();
            foreach (DemandPair demandPair in DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                result.Add((demandPair.Node1, demandPair.Node2));
            }
            return result;
        }
    }
}