// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
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
        /// A <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.
        /// The value is all the <see cref="DemandPair"/>s whose path passes through the key.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for <see cref="DominatedEdge"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="IdleEdge"/> is part of.</param>
        /// <param name="demandPairsPerEdge">The <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public DominatedEdge(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand paths per edge is null!");
#endif
            DemandPairsPerEdge = demandPairsPerEdge;
        }

        /// <summary>
        /// Checks whether we can contract <paramref name="contractEdge"/>. We can do this if all <see cref="DemandPair"/>s passing through it also pass through <paramref name="otherEdge"/>.
        /// </summary>
        /// <param name="contractEdge">The edge for which we want to know whether we can contract it.</param>
        /// <param name="otherEdge">The edge we are comparing <paramref name="contractEdge"/> against.</param>
        /// <returns><see langword="true"/> if all <see cref="DemandPair"/>s that pas through <paramref name="contractEdge"/> also pass through <paramref name="otherEdge"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="contractEdge"/> or <paramref name="otherEdge"/> is <see langword="null"/>.</exception>
        protected bool AllDemandPairsPassThroughAnotherEdge((TreeNode, TreeNode) contractEdge, (TreeNode, TreeNode) otherEdge)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractEdge.Item1, nameof(contractEdge.Item1), "Trying to see whether all demand paths that pass through an edge also pass through another, but the first endpoint of the first edge is null!");
            Utils.NullCheck(contractEdge.Item2, nameof(contractEdge.Item2), "Trying to see whether all demand paths that pass through an edge also pass through another, but the second endpoint of the first edge is null!");
            Utils.NullCheck(otherEdge.Item1, nameof(otherEdge.Item1), "Trying to see whether all demand paths that pass through an edge also pass through another, but the first endpoint of the second edge is null!");
            Utils.NullCheck(otherEdge.Item2, nameof(otherEdge.Item2), "Trying to see whether all demand paths that pass through an edge also pass through another, but the second endpoint of the second edge is null!");
#endif
            return DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(contractEdge), Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter).IsSubsetOf(DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(otherEdge), Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter));
        }

        /// <summary>
        /// Finds all edges that are on the shortest <see cref="DemandPair"/> through <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all edges on the shortest <see cref="DemandPair"/> through <paramref name="edge"/>.</returns>
        protected IEnumerable<(TreeNode, TreeNode)> FindEdgesOnShortestDemandPathThroughEdge((TreeNode, TreeNode) edge)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to find all edges on the shortest demand path through this edge, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to find all edges on the shortest demand path through this edge, but the second endpoint of the edge is null!");
#endif
            if (!DemandPairsPerEdge.TryGetValue(edge, out CountedCollection<DemandPair> demandPathsOnEdge, Measurements.DemandPairsPerEdgeKeysCounter))
            {
                return new List<(TreeNode, TreeNode)>();
            }

            DemandPair shortestPathThroughThisEdge = demandPathsOnEdge.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter).Aggregate((n, m) => n.LengthOfPath(Measurements.DemandPairsOperationsCounter) < m.LengthOfPath(Measurements.DemandPairsOperationsCounter) ? n : m);
            return shortestPathThroughThisEdge.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Select(Utils.OrderEdgeSmallToLarge);
        }

        /// <summary>
        /// Try to apply this <see cref="ReductionRule"/> on the edges in <paramref name="edgesToCheck"/>.
        /// </summary>
        /// <param name="edgesToCheck">The <see cref="IEnumerable{T}"/> with edges we want to check.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/> successfully, <see langword="false"/> otherwise.</returns>
        protected virtual bool TryApplyReductionRule(IEnumerable<(TreeNode, TreeNode)> edgesToCheck)
        {
            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();
            foreach ((TreeNode, TreeNode) edge1 in edgesToCheck)
            {
                foreach ((TreeNode, TreeNode) edge2 in FindEdgesOnShortestDemandPathThroughEdge(edge1))
                {
                    if (edge1 == edge2 || edgesToBeContracted.Contains(edge2))
                    {
                        continue;
                    }

                    if (AllDemandPairsPassThroughAnotherEdge(edge1, edge2))
                    {
                        edgesToBeContracted.Add(edge1);
                        break;
                    }
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();

            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {

        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdges"/>, <paramref name="removedDemandPairs"/> or <paramref name="changedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool RunLaterIteration(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdges, CountedList<DemandPair> removedDemandPairs, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdges, nameof(contractedEdges), $"Trying to apply the {GetType().Name} rule after an edge was contracted, but the IEnumerable of contracted edges is null!");
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), $"Trying to apply the {GetType().Name} rule after a demand path was removed, but the IEnumerable of removed demand paths is null!");
            Utils.NullCheck(changedDemandPairs, nameof(changedDemandPairs), $"Trying to apply the {GetType().Name} rule after a demand path was changed, but the IEnumerable of changed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeChecked = new HashSet<(TreeNode, TreeNode)>();

            foreach (DemandPair demandPair in removedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter))
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            foreach ((CountedList<(TreeNode, TreeNode)> edges, DemandPair _) in changedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode, TreeNode) edge in edges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            contractedEdges.Clear(Measurements.TreeOperationsCounter);
            removedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            changedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return TryApplyReductionRule(edgesToBeChecked);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule for the first time");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            return TryApplyReductionRule(Tree.Edges(Measurements.TreeOperationsCounter).Select(Utils.OrderEdgeSmallToLarge));
        }
    }
}