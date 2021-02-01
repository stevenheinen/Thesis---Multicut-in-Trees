// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;
using MulticutInTrees.Algorithms;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// <see cref="ReductionRule"/> that contracts edges that are not used by any <see cref="DemandPair"/>s.
    /// <br/>
    /// Rule: If there is a tree edge with no demand path passing through it, contract this edge.
    /// </summary>
    public class IdleEdge : ReductionRule
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="List{T}"/> of <see cref="DemandPair"/>s as value.
        /// The value is all the <see cref="DemandPair"/>s whose path passes through the key.
        /// </summary>
        private Dictionary<(TreeNode, TreeNode), List<DemandPair>> DemandPathsPerEdge { get; }

        /// <summary>
        /// Constructor for <see cref="IdleEdge"/>.
        /// </summary>
        /// <param name="input">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="List{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="IdleEdge"/> is part of.</param>
        /// <param name="demandPathsPerEdge">The <see cref="Dictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="List{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPathsPerEdge"/> is <see langword="null"/>.</exception>
        public IdleEdge(Tree<TreeNode> input, List<DemandPair> demandPairs, Algorithm algorithm, Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPathsPerEdge) : base(input, demandPairs, algorithm)
        {
            Utils.NullCheck(input, nameof(input), $"Trying to create an instance of the IdleEdge rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the IdleEdge rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the IdleEdge rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPathsPerEdge, nameof(demandPathsPerEdge), $"Trying to create an instance of the IdleEdge rule, but the dictionary with demand paths per edge is null!");

            DemandPathsPerEdge = demandPathsPerEdge;
        }

        /// <summary>
        /// Checks whether an edge can be contracted. This is the case when no demand pairs use this edge.
        /// </summary>
        /// <param name="edge">The tuple of two <see cref="TreeNode"/>s for which we want to know if it can be contracted.</param>
        /// <returns><see langword="true"/> if we can contract <paramref name="edge"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <see cref="TreeNode"/> of <paramref name="edge"/> is <see langword="null"/>.</exception>
        private bool CanEdgeBeContracted((TreeNode, TreeNode) edge)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), $"Trying to see if an edge can be contracted, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), $"Trying to see if an edge can be contracted, but the second endpoint of the edge is null!");

            (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);

            // If this edge is not in the dictionary with demand paths per edge, or it is, but there are no demand paths going through this edge, we can contract this edge.
            if ((!DemandPathsPerEdge.ContainsKey(usedEdge) || DemandPathsPerEdge[usedEdge].Count == 0) && Input.HasNode(edge.Item1) && Input.HasNode(edge.Item2) && Input.HasEdge(edge))
            {
                return true;
            }
            return false; 
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            return;
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
            // In the first iteration, check all edges in the input tree.
            List<(TreeNode, TreeNode)> edgesToBeContracted = new List<(TreeNode, TreeNode)>();
            foreach ((TreeNode, TreeNode) edge in Input.Edges)
            {
                if (CanEdgeBeContracted(edge))
                {
                    edgesToBeContracted.Add(edge);
                }
            }

            if (edgesToBeContracted.Count == 0)
            {
                return false;
            }

            Algorithm.ContractEdges(edgesToBeContracted);
            return true;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdgeNodeTupleList"/> is <see langword="null"/>.</exception>
        internal override bool AfterEdgeContraction(IEnumerable<((TreeNode, TreeNode), TreeNode, List<DemandPair>)> contractedEdgeNodeTupleList)
        {
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), $"Trying to execute the IdleEdge rule after edges were contracted, but the IEnumerable with contracted edge information is null!");

            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathRemove(IEnumerable<DemandPair> removedDemandPairs)
        {
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), $"Trying to execute the IdleEdge rule after a demand pair was removed, but the list of removed demand pairs is null!");

            // Find all edges that were on the removed demand path, and check if they can be contracted.
            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();
            foreach (DemandPair demandPair in removedDemandPairs)
            {
                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath)
                {
                    if (CanEdgeBeContracted(edge))
                    {
                        edgesToBeContracted.Add(edge);
                    }
                }
            }

            // If we cannot contract any edges, return false.
            if (edgesToBeContracted.Count == 0)
            {
                return false;
            }

            // Contract the edges and return true.
            Algorithm.ContractEdges(edgesToBeContracted.ToList());
            return true;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="changedEdgesPerDemandPairList"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathChanged(IEnumerable<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), $"Trying to execute the IdleEdge rule after a demand pair was changed, but the list of changed demand pairs is null!");

            HashSet<(TreeNode, TreeNode)> contractableEdges = new HashSet<(TreeNode, TreeNode)>();
            foreach ((IEnumerable<(TreeNode, TreeNode)>, DemandPair) tuple in changedEdgesPerDemandPairList)
            {
                foreach ((TreeNode, TreeNode) edge in tuple.Item1)
                {
                    if (CanEdgeBeContracted(edge))
                    {
                        contractableEdges.Add(edge);
                    }
                }
            }

            if (contractableEdges.Count == 0)
            {
                return false;
            }

            Algorithm.ContractEdges(contractableEdges.ToList());
            return true;
        }
    }
}
