// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

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
    /// <see cref="ReductionRule"/> that contracts edges that are not used by any <see cref="DemandPair"/>s.
    /// <br/>
    /// Rule: If there is a tree edge with no demand path passing through it, contract this edge.
    /// </summary>
    public class IdleEdge : ReductionRule
    {
        /// <summary>
        /// A <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.
        /// The value is all the <see cref="DemandPair"/>s whose path passes through the key.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for <see cref="IdleEdge"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="IdleEdge"/> is part of.</param>
        /// <param name="demandPairsPerEdge">The <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public IdleEdge(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm, nameof(IdleEdge))
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the IdleEdge rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the IdleEdge rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the IdleEdge rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), "Trying to create an instance of the IdleEdge rule, but the dictionary with demand paths per edge is null!");
#endif
            DemandPairsPerEdge = demandPairsPerEdge;
        }

        /// <summary>
        /// Checks whether an edge can be contracted. This is the case when no demand pairs use this edge.
        /// </summary>
        /// <param name="edge">The tuple of two <see cref="TreeNode"/>s for which we want to know if it can be contracted.</param>
        /// <returns><see langword="true"/> if we can contract <paramref name="edge"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <see cref="TreeNode"/> of <paramref name="edge"/> is <see langword="null"/>.</exception>
        private bool CanEdgeBeContracted((TreeNode, TreeNode) edge)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to see if an edge can be contracted, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to see if an edge can be contracted, but the second endpoint of the edge is null!");
#endif
            (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);

            // If this edge is not in the dictionary with demand paths per edge, or it is, but there are no demand paths going through this edge, we can contract this edge.
            if (Tree.HasNode(edge.Item1, Measurements.TreeOperationsCounter) && Tree.HasNode(edge.Item2, Measurements.TreeOperationsCounter) && Tree.HasEdge(edge, Measurements.TreeOperationsCounter) && (!DemandPairsPerEdge.ContainsKey(usedEdge, Measurements.DemandPairsPerEdgeKeysCounter) || DemandPairsPerEdge[usedEdge, Measurements.DemandPairsPerEdgeKeysCounter].Count(Measurements.DemandPairsPerEdgeValuesCounter) == 0))
            {
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {

        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine("Applying Idle Edge rule for the first time...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            // In the first iteration, check all edges in the input tree.
            List<(TreeNode, TreeNode)> edgesToBeContracted = new List<(TreeNode, TreeNode)>();
            foreach ((TreeNode, TreeNode) edge in Tree.Edges(Measurements.TreeOperationsCounter))
            {
                if (CanEdgeBeContracted(edge))
                {
                    edgesToBeContracted.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();

            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdgeNodeTupleList"/> is <see langword="null"/>.</exception>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to execute the IdleEdge rule after edges were contracted, but the IEnumerable with contracted edge information is null!");
#endif
            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to execute the IdleEdge rule after a demand pair was removed, but the list of removed demand pairs is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Idle Edge rule after a demand path was removed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            // Find all edges that were on the removed demand path, and check if they can be contracted.
            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();
            foreach (DemandPair demandPair in removedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter))
                {
                    if (CanEdgeBeContracted(edge))
                    {
                        edgesToBeContracted.Add(Utils.OrderEdgeSmallToLarge(edge));
                    }
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();

            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="changedEdgesPerDemandPairList"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathChanged(CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
#if !EXPERIMENT
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to execute the IdleEdge rule after a demand pair was changed, but the list of changed demand pairs is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Idle Edge rule after a demand path was changed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();
            foreach ((CountedList<(TreeNode, TreeNode)>, DemandPair) tuple in changedEdgesPerDemandPairList.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode, TreeNode) edge in tuple.Item1.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    if (CanEdgeBeContracted(edge))
                    {
                        edgesToBeContracted.Add(Utils.OrderEdgeSmallToLarge(edge));
                    }
                }
            }

            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <summary>
        /// Contract all edges in <paramref name="edgesToBeContracted"/>.
        /// </summary>
        /// <param name="edgesToBeContracted">The <see cref="CountedList{T}"/> with all edges to be contracted.</param>
        /// <returns><see langword="true"/> if <paramref name="edgesToBeContracted"/> has any elements, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edgesToBeContracted"/> is <see langword="null"/>.</exception>
        private bool TryContractEdges(CountedList<(TreeNode, TreeNode)> edgesToBeContracted)
        {
#if !EXPERIMENT
            Utils.NullCheck(edgesToBeContracted, nameof(edgesToBeContracted), "Trying to contract edges, but the List with edges is null!");
#endif            
            if (edgesToBeContracted.Count(Measurements.TreeOperationsCounter) == 0)
            {
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }

            Measurements.TimeSpentCheckingApplicability.Stop();
            Measurements.TimeSpentModifyingInstance.Start();
            Algorithm.ContractEdges(edgesToBeContracted, Measurements);
            Measurements.TimeSpentModifyingInstance.Stop();
            return true;
        }
    }
}
