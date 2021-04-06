// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// Improved version of the <see cref="DominatedEdge"/> rule. This rule only compares edges to their direct neighbours instead of every other edge in the tree.
    /// </summary>
    public class ImprovedDominatedEdge : DominatedEdge
    {
        /// <summary>
        /// Constructor for <see cref="ImprovedDominatedEdge"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="IdleEdge"/> is part of.</param>
        /// <param name="demandPairsPerEdge">The <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public ImprovedDominatedEdge(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm, demandPairsPerEdge)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the ImprovedDominatedEdge reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the ImprovedDominatedEdge reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the ImprovedDominatedEdge reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), "Trying to create an instance of the ImprovedDominatedEdge reduction rule, but the dictionary with demand paths per edge is null!");
#endif
        }

        /// <inheritdoc cref="ReductionRule.AfterDemandPathRemove(CountedList{DemandPair})"/>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Improved Dominated Edge rule after a demand path was removed, but the IEnumerable of removed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Improved Dominated Edge rule after a demand path was removed...");
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

            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = DetermineEdgesToBeContracted(edgesToBeChecked);

            Measurements.TimeSpentCheckingApplicability.Stop();
            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc cref="ReductionRule.AfterDemandPathChanged(CountedList{ValueTuple{CountedList{ValueTuple{TreeNode, TreeNode}}, DemandPair}})"/>
        internal override bool AfterDemandPathChanged(CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
#if !EXPERIMENT
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Improved Dominated Edge rule after a demand path was changed, but the IEnumerable of changed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Improved Dominated Edge rule after a demand path was changed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeChecked = new HashSet<(TreeNode, TreeNode)>();
            foreach ((CountedList<(TreeNode, TreeNode)> edges, DemandPair _) in changedEdgesPerDemandPairList.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode, TreeNode) edge in edges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = DetermineEdgesToBeContracted(edgesToBeChecked);

            Measurements.TimeSpentCheckingApplicability.Stop();
            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc cref="ReductionRule.RunFirstIteration"/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine("Applying Improved Dominated Edge rule for the first time...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = DetermineEdgesToBeContracted(Tree.Edges(Measurements.TreeOperationsCounter).Select(e => Utils.OrderEdgeSmallToLarge(e)));

            Measurements.TimeSpentCheckingApplicability.Stop();
            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <summary>
        /// Determine the edges in <paramref name="possibleEdges"/> that can be contracted.
        /// </summary>
        /// <param name="possibleEdges">The edges we will potentially contract.</param>
        /// <returns>A <see cref="HashSet{T}"/> with edges from <paramref name="possibleEdges"/> that will be contracted.</returns>
        private HashSet<(TreeNode, TreeNode)> DetermineEdgesToBeContracted(IEnumerable<(TreeNode, TreeNode)> possibleEdges)
        {
            Dictionary<(TreeNode, TreeNode), (TreeNode, TreeNode)> comparedEdge = new Dictionary<(TreeNode, TreeNode), (TreeNode, TreeNode)>();
            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();

            foreach ((TreeNode, TreeNode) edge in possibleEdges)
            {
                if (edgesToBeContracted.Contains(edge) || !Tree.HasNode(edge.Item1, Measurements.TreeOperationsCounter) || !Tree.HasNode(edge.Item2, Measurements.TreeOperationsCounter) || !Tree.HasEdge(edge, Measurements.TreeOperationsCounter))
                {
                    continue;
                }

                // Compare the edge to its neighbours. If we contracted it this way, go to the next edge.
                if (CompareEdgeToNeighbours(edge, edgesToBeContracted, comparedEdge))
                {
                    continue;
                }

                // We have not contracted this edge yet, so we will look at all the edges (that are not yet contracted) on the shortest demand path through this edge and compare this edge to those.
                IEnumerable<(TreeNode, TreeNode)> demandPairEdges = FindEdgesOnShortestDemandPathThroughEdge(edge).Select(e => Utils.OrderEdgeSmallToLarge(e)).Where(e => e != edge && !edgesToBeContracted.Contains(e));
                foreach ((TreeNode, TreeNode) otherEdge in demandPairEdges)
                {
                    if (AllDemandPairsPassThroughAnotherEdge(edge, otherEdge))
                    {
                        edgesToBeContracted.Add(edge);
                        comparedEdge[edge] = otherEdge;
                        break;
                    }
                }
            }

            return edgesToBeContracted;
        }

        /// <summary>
        /// Compare <paramref name="edge"/> to its direct neighbours and see if we can contract it.
        /// </summary>
        /// <param name="edge">The edge we want to contract.</param>
        /// <param name="edgesToBeContracted">The <see cref="HashSet{T}"/> containing all edges that will be contracted.</param>
        /// <param name="comparedEdge">The <see cref="Dictionary{TKey, TValue}"/> with per contracted edge the edge it was compared to that led to the contraction.</param>
        /// <returns><see langword="true"/> if we are able to contract <paramref name="edge"/> when looking at its direct neighbours, <see langword="false"/> otherwise.</returns>
        private bool CompareEdgeToNeighbours((TreeNode, TreeNode) edge, HashSet<(TreeNode, TreeNode)> edgesToBeContracted, Dictionary<(TreeNode, TreeNode), (TreeNode, TreeNode)> comparedEdge)
        {
            foreach ((TreeNode, TreeNode) neighbour in NeighbouringEdges(edge))
            {
                (TreeNode, TreeNode) compareEdge = neighbour;
                if (edgesToBeContracted.Contains(neighbour))
                {
                    if (comparedEdge[neighbour] == edge)
                    {
                        continue;
                    }
                    compareEdge = comparedEdge[neighbour];
                }

                if (AllDemandPairsPassThroughAnotherEdge(edge, compareEdge))
                {
                    edgesToBeContracted.Add(edge);
                    comparedEdge[edge] = compareEdge;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determine and return all edges that are a neighbour of <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">The edge for which we want to determine its neighbours.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all edges that are a neighbour of <paramref name="edge"/>.</returns>
        private IEnumerable<(TreeNode, TreeNode)> NeighbouringEdges((TreeNode, TreeNode) edge)
        {
            IEnumerable<(TreeNode, TreeNode)> part1 = edge.Item1.Neighbours(Measurements.TreeOperationsCounter).Where(n => n != edge.Item2).Select(n => Utils.OrderEdgeSmallToLarge((edge.Item1, n)));
            IEnumerable<(TreeNode, TreeNode)> part2 = edge.Item2.Neighbours(Measurements.TreeOperationsCounter).Where(n => n != edge.Item1).Select(n => Utils.OrderEdgeSmallToLarge((edge.Item2, n)));
            return part1.Concat(part2);
        }
    }
}
