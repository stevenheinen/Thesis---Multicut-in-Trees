// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MulticutInTrees.Algorithms;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// <see cref="ReductionRule"/> that finds paths of <see cref="DemandPair"/>s of length one and cuts the corresponding edges.
    /// <br/>
    /// Rule: If a demand path has length one, cut its edge e.
    /// </summary>
    public class UnitPath : ReductionRule
    {
        /// <summary>
        /// Constructor for the <see cref="UnitPath"/> rule.
        /// </summary>
        /// <param name="input">The <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="List{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="UnitPath"/> rule is part of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>, <paramref name="demandPairs"/> or <paramref name="algorithm"/> is <see langword="null"/>.</exception>
        public UnitPath(Tree<TreeNode> input, List<DemandPair> demandPairs, Algorithm algorithm) : base(input, demandPairs, algorithm)
        {
            Utils.NullCheck(input, nameof(input), $"Trying to create an instance of the Unit Path rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the Unit Path rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the Unit Path rule, but the algorithm it is part of is null!");
        }

        /// <summary>
        /// Returns whether a given <see cref="DemandPair"/> has a path of length one.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> for which we want to know if its path has length one.</param>
        /// <returns><see langword="true"/> if the path of <paramref name="demandPair"/> has length one, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/> is <see langword="null"/>.</exception>
        private bool DemandPathHasLengthOne(DemandPair demandPair)
        {
            Utils.NullCheck(demandPair, nameof(demandPair), $"Trying to check whether a demand pair has a path of length 1, but the demand pair is null!");

            return demandPair.EdgesOnDemandPath.Count == 1;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="changedEdgesPerDemandPairList"/> is <see langword="null"/>.</exception>
        public override bool AfterDemandPathChanged(IEnumerable<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), $"Trying to apply the Unit Path rule after a demand path was changed, but the list of changed demand pairs is null!");

            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach ((List<(TreeNode, TreeNode)> _, DemandPair path) in changedEdgesPerDemandPairList)
            {
                if (DemandPathHasLengthOne(path))
                {
                    edgesToBeCut.Add(path.EdgesOnDemandPath[0]);
                }
            }

            if (edgesToBeCut.Count == 0)
            {
                return false;
            }

            Algorithm.CutEdges(edgesToBeCut.ToList());
            return true;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removedDemandPairs"/> is <see langword="null"/>.</exception>
        public override bool AfterDemandPathRemove(IEnumerable<DemandPair> removedDemandPairs)
        {
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), $"Trying to apply the Unit Path rule after a demand path was removed, but the list of removed demand pairs is null!");

            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdgeNodeTupleList"/> is <see langword="null"/>.</exception>
        public override bool AfterEdgeContraction(IEnumerable<((TreeNode, TreeNode), TreeNode, List<DemandPair>)> contractedEdgeNodeTupleList)
        {
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), $"Trying to apply the Unit Path rule after an edge was contracted, but the list of contracted edges is null!");

            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach (((TreeNode, TreeNode) _, TreeNode _, List<DemandPair> paths) in contractedEdgeNodeTupleList)
            {
                foreach (DemandPair path in paths)
                {
                    if (DemandPathHasLengthOne(path))
                    {
                        edgesToBeCut.Add(path.EdgesOnDemandPath[0]);
                    }
                }
            }

            if (edgesToBeCut.Count == 0)
            {
                return false;
            }

            Algorithm.CutEdges(edgesToBeCut.ToList());
            return true;
        }

        /// <inheritdoc/>
        public override bool RunFirstIteration()
        {
            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach (DemandPair dp in DemandPairs)
            {
                if (DemandPathHasLengthOne(dp))
                {
                    edgesToBeCut.Add(dp.EdgesOnDemandPath[0]);
                }
            }

            if (edgesToBeCut.Count == 0)
            {
                return false;
            }

            Algorithm.CutEdges(edgesToBeCut.ToList());
            return true;
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            return;
        }
    }
}
