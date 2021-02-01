// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MulticutInTrees.Graphs;
using MulticutInTrees.ReductionRules;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Abstract class for an algorithm that solves the Multicut in Trees problem.
    /// </summary>
    public abstract class Algorithm
    {
        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of reduction rules (of type <see cref="ReductionRule"/>) this <see cref="Algorithm"/> uses.
        /// </summary>
        public ReadOnlyCollection<ReductionRule> ReductionRules { get; protected set; }

        /// <summary>
        /// The <see cref="List{T}"/> of <see cref="DemandPair"/>s in the input.
        /// </summary>
        protected List<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The input <see cref="Tree{N}"/>.
        /// </summary>
        protected Tree<TreeNode> Input { get; }

        /// <summary>
        /// The <see cref="List{T}"/> of tuples of <see cref="TreeNode"/>s that contains the already found part of the solution.
        /// </summary>
        protected List<(TreeNode, TreeNode)> PartialSolution { get; }

        /// <summary>
        /// The size the cutset is allowed to be.
        /// </summary>
        protected int K { get; }

        /// <summary>
        /// <see langword="true"/> if in the last iteration an edge was contracted, <see langword="false"/> otherwise.
        /// </summary>
        protected bool LastIterationEdgeContraction { get; set; }

        /// <summary>
        /// <see langword="true"/> if in the last iteration a demand pair was removed, <see langword="false"/> otherwise.
        /// </summary>
        protected bool LastIterationDemandPairRemoval { get; set; }

        /// <summary>
        /// <see langword="true"/> if in the last iteration a demand pair was changed, <see langword="false"/> otherwise.
        /// </summary>
        protected bool LastIterationDemandPairChange { get; set; }

        /// <summary>
        /// A <see cref="List{T}"/> of all edges that were removed in the last iteration, their contracted nodes, and the <see cref="DemandPair"/>s on the contracted edge.
        /// </summary>
        protected List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)> LastContractedEdges { get; set; }

        /// <summary>
        /// A <see cref="List{T}"/> of all <see cref="DemandPair"/>s that were removed in the last iteration.
        /// </summary>
        protected List<DemandPair> LastRemovedDemandPairs { get; set; }

        /// <summary>
        /// A <see cref="List{T}"/> of tuples of changed edges for a <see cref="DemandPair"/> and the <see cref="DemandPair"/> itself.
        /// </summary>
        protected List<(List<(TreeNode, TreeNode)>, DemandPair)> LastChangedEdgesPerDemandPair { get; set; }

        /// <summary>
        /// Constructor for an <see cref="Algorithm"/>.
        /// </summary>
        /// <param name="input">The <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="List{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="k">The size the cutset is allowed to be.</param>
        public Algorithm(Tree<TreeNode> input, List<DemandPair> demandPairs, int k)
        {
            Input = input;
            DemandPairs = demandPairs;
            K = k;
            PartialSolution = new List<(TreeNode, TreeNode)>();

            LastContractedEdges = new List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)>();
            LastRemovedDemandPairs = new List<DemandPair>();
            LastChangedEdgesPerDemandPair = new List<(List<(TreeNode, TreeNode)>, DemandPair)>();
        }

        /// <summary>
        /// Try to solve the instance.
        /// </summary>
        public (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>) Run()
        {
            bool successful = false;
            bool[] appliedReductionRule = new bool[ReductionRules.Count];
            for (int i = 0; i < ReductionRules.Count; i++)
            {
                if (PartialSolution.Count == K)
                {
                    return (Input, PartialSolution, DemandPairs);
                }

                // If we have not executed this rule before, execute it now.
                if (!appliedReductionRule[i])
                {
                    appliedReductionRule[i] = true;
                    if (ReductionRules[i].RunFirstIteration())
                    {
                        // If the first application of the i-th reduction rule was a success, start again at rule 0.
                        i = -1;
                    }
                    continue;
                }

                // We have already applied the i-th rule before. Try to apply it again, depending on what happened in the last iteration.
                if (LastIterationDemandPairChange)
                {
                    List<(List<(TreeNode, TreeNode)>, DemandPair)> oldLastChangedEdgesPerDemandPair = new List<(List<(TreeNode, TreeNode)>, DemandPair)>(LastChangedEdgesPerDemandPair);
                    LastChangedEdgesPerDemandPair = new List<(List<(TreeNode, TreeNode)>, DemandPair)>();
                    LastIterationDemandPairChange = false;
                    successful = ReductionRules[i].AfterDemandPathChanged(oldLastChangedEdgesPerDemandPair);
                }
                if (LastIterationDemandPairRemoval)
                {
                    List<DemandPair> oldLastRemovedDemandPairs = new List<DemandPair>(LastRemovedDemandPairs);
                    LastRemovedDemandPairs = new List<DemandPair>();
                    LastIterationDemandPairRemoval = false;
                    successful = ReductionRules[i].AfterDemandPathRemove(oldLastRemovedDemandPairs);
                }
                if (LastIterationEdgeContraction)
                {
                    List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)> oldLastContractedEdges = new List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)>(LastContractedEdges);
                    LastContractedEdges = new List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)>();
                    LastIterationEdgeContraction = false;
                    successful = ReductionRules[i].AfterEdgeContraction(oldLastContractedEdges);
                }

                // If we applied the rule successfully, go back to rule 0.
                if (successful)
                {
                    i = -1;
                    continue;
                }
            }

            return (Input, PartialSolution, DemandPairs);
        }

        /// <summary>
        /// Computes all information needed prior to the first iteration of the algorithm.
        /// </summary>
        protected abstract void Preprocess();

        /// <summary>
        /// Add this edge to the solution, remove all <see cref="DemandPair"/>s that go over this edge, and contract it.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s representing the edge to be cut.</param>
        public abstract void CutEdge((TreeNode, TreeNode) edge);

        /// <summary>
        /// Add multiple edges to the solution, remove all <see cref="DemandPair"/>s that go over these edges, and contract them.
        /// </summary>
        /// <param name="edges">The <see cref="IList{T}"/> of tuples of <see cref="TreeNode"/>s that represent the edges to be cut.</param>
        public abstract void CutEdges(IList<(TreeNode, TreeNode)> edges);

        /// <summary>
        /// Contract an edge.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s representing the edge to be contracted.</param>
        public abstract void ContractEdge((TreeNode, TreeNode) edge);

        /// <summary>
        /// Contract multiple edges.
        /// </summary>
        /// <param name="edges">The <see cref="IList{T}"/> of tuples of <see cref="TreeNode"/>s representing the edges to be contracted.</param>
        public abstract void ContractEdges(IList<(TreeNode, TreeNode)> edges);

        /// <summary>
        /// Remove a <see cref="DemandPair"/> from the problem instance.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> to be removed.</param>
        public abstract void RemoveDemandPair(DemandPair demandPair);

        /// <summary>
        /// Remove multiple <see cref="DemandPair"/>s from the problem instance.
        /// </summary>
        /// <param name="demandPairs">The <see cref="IList{T}"/> of <see cref="DemandPair"/>s to be removed.</param>
        public abstract void RemoveDemandPairs(IList<DemandPair> demandPairs);

        /// <summary>
        /// Change an endpoint of a <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> whose endpoint changes.</param>
        /// <param name="oldEndpoint">The old endpoint of <paramref name="demandPair"/>.</param>
        /// <param name="newEndpoint">The new endpoint of <paramref name="demandPair"/>.</param>
        public abstract void ChangeEndpointOfDemandPair(DemandPair demandPair, TreeNode oldEndpoint, TreeNode newEndpoint);

        /// <summary>
        /// Change an endpoint of multiple <see cref="DemandPair"/>s.
        /// </summary>
        /// <param name="demandPairEndpointTuples">The <see cref="IList{T}"/> of tuples containing the <see cref="DemandPair"/> that is changed, the <see cref="TreeNode"/> old endpoint and <see cref="TreeNode"/> new endpoint.</param>
        public abstract void ChangeEndpointOfDemandPairs(IList<(DemandPair, TreeNode, TreeNode)> demandPairEndpointTuples);
    }
}
