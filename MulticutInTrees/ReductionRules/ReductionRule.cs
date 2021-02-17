// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Text;
using MulticutInTrees.Algorithms;
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
        /// The <see cref="List{T}"/> of <see cref="DemandPair"/>s in the instance.
        /// </summary>
        protected List<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.
        /// </summary>
        protected Algorithm Algorithm { get; }

        /// <summary>
        /// Constructor for a <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="List{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        public ReductionRule(Tree<TreeNode> tree, List<DemandPair> demandPairs, Algorithm algorithm)
        {
            Tree = tree;
            DemandPairs = demandPairs;
            Algorithm = algorithm;
            Preprocess();
        }

        /// <summary>
        /// Everything that needs to happen before the reduction rule can be executed.
        /// </summary>
        protected abstract void Preprocess();

        /// <summary>
        /// First iteration of this <see cref="ReductionRule"/>. There is no information about last iterations available.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool RunFirstIteration();

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after one or more edges have been contracted in the last iteration.
        /// </summary>
        /// <param name="contractedEdgeNodeTupleList">An <see cref="IEnumerable{T}"/> with tuples consisting of a tuple of <see cref="TreeNode"/>s (the contracted edge), a <see cref="TreeNode"/> (the result of the edge contraction), and a <see cref="List{T}"/> of <see cref="DemandPair"/>s (the <see cref="DemandPair"/>s on the contracted edge).</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterEdgeContraction(IEnumerable<((TreeNode, TreeNode), TreeNode, List<DemandPair>)> contractedEdgeNodeTupleList);

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after one or more <see cref="DemandPair"/>s have been removed in the last iteration.
        /// </summary>
        /// <param name="removedDemandPairs">The <see cref="IEnumerable{T}"/> of <see cref="DemandPair"/>s that were removed in the last iteration.</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterDemandPathRemove(IEnumerable<DemandPair> removedDemandPairs);

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after the endpoint of one or more <see cref="DemandPair"/>s were changed in the last iteration.
        /// </summary>
        /// <param name="changedEdgesPerDemandPairList">The <see cref="IEnumerable{T}"/> with tuples with a <see cref="List{T}"/> with tuples of <see cref="TreeNode"/>s that represent the edges that were removed from the demand path, and a <see cref="DemandPair"/>.</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterDemandPathChanged(IEnumerable<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList);
    }
}
