// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MulticutInTrees.Algorithms;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// <see cref="ReductionRule"/> that contracts an edge if all <see cref="DemandPair"/>s whose path go over this edge also go over another edge.
    /// <br/>
    /// Rule: If all demand paths that pass through edge e_1 also pass through edge e_2, then contract e_1.
    /// </summary>
    public class DominatedEdge : ReductionRule
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
        public DominatedEdge(Tree<TreeNode> input, List<DemandPair> demandPairs, Algorithm algorithm, Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPathsPerEdge) : base(input, demandPairs, algorithm)
        {
            Utils.NullCheck(input, nameof(input), $"Trying to create an instance of the DominatedEdge reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the DominatedEdge reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the DominatedEdge reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPathsPerEdge, nameof(demandPathsPerEdge), $"Trying to create an instance of the DominatedEdge reduction rule, but the dictionary with demand paths per edge is null!");

            DemandPathsPerEdge = demandPathsPerEdge;
        }

        /// <summary>
        /// Checks whether we can contract <paramref name="contractEdge"/>. We can do this if all <see cref="DemandPair"/>s passing through it also pass through <paramref name="otherEdge"/>.
        /// </summary>
        /// <param name="contractEdge">The edge for which we want to know whether we can contract it.</param>
        /// <param name="otherEdge">The edge we are comparing <paramref name="contractEdge"/> against.</param>
        /// <returns><see langword="true"/> if all <see cref="DemandPair"/>s that pas through <paramref name="contractEdge"/> also pass through <paramref name="otherEdge"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="contractEdge"/> or <paramref name="otherEdge"/> is <see langword="null"/>.</exception>
        private bool AllDemandPairsPassThroughAnotherEdge((TreeNode, TreeNode) contractEdge, (TreeNode, TreeNode) otherEdge)
        {
            Utils.NullCheck(contractEdge.Item1, nameof(contractEdge.Item1), $"Trying to see whether all demand paths that pass through an edge also pass through another, but the first endpoint of the first edge is null!");
            Utils.NullCheck(contractEdge.Item2, nameof(contractEdge.Item2), $"Trying to see whether all demand paths that pass through an edge also pass through another, but the second endpoint of the first edge is null!");
            Utils.NullCheck(otherEdge.Item1, nameof(otherEdge.Item1), $"Trying to see whether all demand paths that pass through an edge also pass through another, but the first endpoint of the second edge is null!");
            Utils.NullCheck(otherEdge.Item2, nameof(otherEdge.Item2), $"Trying to see whether all demand paths that pass through an edge also pass through another, but the second endpoint of the second edge is null!");

            return DemandPathsPerEdge[contractEdge].IsSubsetOf(DemandPathsPerEdge[otherEdge]);
        }

        /// <summary>
        /// Finds all edges that are on the shortest <see cref="DemandPair"/> through <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        private ReadOnlyCollection<(TreeNode, TreeNode)> FindEdgesOnShortestDemandPathThroughEdge((TreeNode, TreeNode) edge)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), $"Trying to find all edges on the shortest demand path through this edge, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), $"Trying to find all edges on the shortest demand path through this edge, but the second endpoint of the edge is null!");

            DemandPair shortestPathThroughThisEdge = DemandPathsPerEdge[edge].Aggregate((n, m) => n.EdgesOnDemandPath.Count < m.EdgesOnDemandPath.Count ? n : m);
            return shortestPathThroughThisEdge.EdgesOnDemandPath.Select(n => Utils.OrderEdgeSmallToLarge(n)).ToList().AsReadOnly();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="changedEdgesPerDemandPairList"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathChanged(IEnumerable<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), $"Trying to apply the Dominated Edge rule after a demand path was changed, but the IEnumerable of changed demand paths is null!");

            HashSet<(TreeNode, TreeNode)> edgesToBeChecked = new HashSet<(TreeNode, TreeNode)>();
            foreach ((List<(TreeNode, TreeNode)> edges, DemandPair _) in changedEdgesPerDemandPairList)
            {
                foreach ((TreeNode, TreeNode) edge in edges)
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            List<(TreeNode, TreeNode)> edgesToBeContracted = new List<(TreeNode, TreeNode)>();

            foreach ((TreeNode, TreeNode) edge in edgesToBeChecked)
            {
                foreach ((TreeNode, TreeNode) otherEdge in FindEdgesOnShortestDemandPathThroughEdge(edge))
                {
                    if (edge == otherEdge)
                    {
                        continue;
                    }
                    if (AllDemandPairsPassThroughAnotherEdge(edge, otherEdge))
                    {
                        edgesToBeContracted.Add(edge);
                        break;
                    }
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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathRemove(IEnumerable<DemandPair> removedDemandPairs)
        {
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), $"Trying to apply the Dominated Edge rule after a demand path was removed, but the IEnumerable of removed demand paths is null!");

            HashSet<(TreeNode, TreeNode)> edgesToBeChecked = new HashSet<(TreeNode, TreeNode)>();
            foreach (DemandPair demandPair in removedDemandPairs)
            {
                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath)
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            List<(TreeNode, TreeNode)> edgesToBeContracted = new List<(TreeNode, TreeNode)>();

            foreach ((TreeNode, TreeNode) edge in edgesToBeChecked)
            {
                foreach ((TreeNode, TreeNode) otherEdge in FindEdgesOnShortestDemandPathThroughEdge(edge))
                {
                    if (edge == otherEdge)
                    {
                        continue;
                    }
                    if (AllDemandPairsPassThroughAnotherEdge(edge, otherEdge))
                    {
                        edgesToBeContracted.Add(edge);
                        break;
                    }
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
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), $"Trying to apply the Dominated Edge rule after an edge was contracted, but the IEnumerable of contracted edges is null!");

            return false;
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();

            for (int i = 0; i < Input.Edges.Count - 1; i++)
            {
                for (int j = i + 1; j < Input.Edges.Count; j++)
                {
                    if (edgesToBeContracted.Contains(Input.Edges[i]) || edgesToBeContracted.Contains(Input.Edges[j]))
                    {
                        continue;
                    }
                    if (AllDemandPairsPassThroughAnotherEdge(Input.Edges[i], Input.Edges[j]))
                    {
                        edgesToBeContracted.Add(Input.Edges[i]);
                    }
                    else if (AllDemandPairsPassThroughAnotherEdge(Input.Edges[j], Input.Edges[i]))
                    {
                        edgesToBeContracted.Add(Input.Edges[j]);
                    }
                }
            }

            if (edgesToBeContracted.Count == 0)
            {
                return false;
            }

            Algorithm.ContractEdges(edgesToBeContracted.ToList());
            return true;
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            return;
        }
    }
}