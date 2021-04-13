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
    /// <see cref="ReductionRule"/> that contracts edges when all <see cref="DemandPair"/>s starting at a node go in the same direction.
    /// <br/>
    /// <b>Rule:</b> If all the demand paths starting at a leaf u have the same direction (for at least 2 edges), then contract the edge that is connected to u. If all the demand paths starting at an inner node u have the same direction (for at least 1 edge), then contract the edge e adjacent to u which does not belong to any demand path starting at u.
    /// </summary>
    public class UniqueDirection : ReductionRule
    {
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per <see cref="TreeNode"/>.
        /// </summary>
        private CountedDictionary<TreeNode, CountedCollection<DemandPair>> DemandPairsPerNode { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not influence the performance.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for the <see cref="DominatedPath"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        /// <param name="demandPairsPerNode"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per <see cref="TreeNode"/>.</param>
        /// <param name="demandPairsPerEdge"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge.</param>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per <see cref="TreeNode"/>.
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="demandPairsPerNode"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public UniqueDirection(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode, CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with demand paths is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerNode, nameof(demandPairsPerNode), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per node is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per edge is null!");
#endif
            DemandPairsPerNode = demandPairsPerNode;
            DemandPairsPerEdge = demandPairsPerEdge;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Determine for a given <see cref="TreeNode"/> which edges can be contracted according to this <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> for which we want to determine which edges can be contracted.</param>
        /// <param name="edgesToBeContracted"><see cref="HashSet{T}"/> containing all edges that can be contracted in this application of this <see cref="ReductionRule"/>.</param>
        private void CheckApplicabilityNode(TreeNode node, HashSet<(TreeNode, TreeNode)> edgesToBeContracted)
        {
            if (!DemandPairsPerNode.TryGetValue(node, out CountedCollection<DemandPair> dpsAtNode, Measurements.DemandPairsPerEdgeKeysCounter))
            {
                return;
            }

            if (node.Degree(Measurements.TreeOperationsCounter) == 1)
            {
                CheckApplicabilityLeaf(node, dpsAtNode, edgesToBeContracted);
            }
            else
            {
                CheckApplicabilityInternalNode(node, dpsAtNode, edgesToBeContracted);
            }
        }

        /// <summary>
        /// Determine for a given internal <see cref="TreeNode"/> which edges can be contracted according to this <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> for which we want to determine which edges can be contracted.</param>
        /// <param name="dpsAtNode">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s that start at <paramref name="node"/>.</param>
        /// <param name="edgesToBeContracted"><see cref="HashSet{T}"/> containing all edges that can be contracted in this application of this <see cref="ReductionRule"/>.</param>
        private void CheckApplicabilityInternalNode(TreeNode node, CountedCollection<DemandPair> dpsAtNode, HashSet<(TreeNode, TreeNode)> edgesToBeContracted)
        {
            (TreeNode, TreeNode) firstEdge = DetermineCommonEdge(node, dpsAtNode);

            if (firstEdge.Item1 is null)
            {
                return;
            }

            // For the other edges connected to this node, check if we can contract it. We can contract it if, for each demand path on it, not all edges on that demand path are being contracted.
            foreach ((TreeNode, TreeNode) edge in node.Neighbours(Measurements.TreeOperationsCounter).Select(n => Utils.OrderEdgeSmallToLarge((node, n))).Where(e => e != firstEdge))
            {
                if (CanContractNeighbouringEdge(edge, edgesToBeContracted))
                {
                    edgesToBeContracted.Add(edge);
                }
            }
        }

        /// <summary>
        /// Checks for a given edge whether it can be contracted. This is the case when it is not used by a <see cref="DemandPair"/> whose all other edges will be contracted.
        /// </summary>
        /// <param name="edge">The edge we will potentially contract.</param>
        /// <param name="edgesToBeContracted">The <see cref="HashSet{T}"/> with all edges that will be contracted.</param>
        /// <returns><see langword="true"/> if we can contract <paramref name="edge"/>, <see langword="false"/> otherwise.</returns>
        private bool CanContractNeighbouringEdge((TreeNode, TreeNode) edge, HashSet<(TreeNode, TreeNode)> edgesToBeContracted)
        {
            if (!DemandPairsPerEdge.TryGetValue(edge, out CountedCollection<DemandPair> dps, Measurements.DemandPairsPerEdgeKeysCounter))
            {
                // No demand pairs use this edge, so we can safely contract it.
                return true;
            }

            bool canBeContracted = true;
            foreach (DemandPair dp in dps.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                bool allOtherEdgesOnPathWillBeContracted = true;
                foreach ((TreeNode, TreeNode) e in dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter))
                {
                    if (edge == Utils.OrderEdgeSmallToLarge(e))
                    {
                        continue;
                    }

                    if (!edgesToBeContracted.Contains(Utils.OrderEdgeSmallToLarge(e)))
                    {
                        allOtherEdgesOnPathWillBeContracted = false;
                        break;
                    }
                }

                if (allOtherEdgesOnPathWillBeContracted)
                {
                    canBeContracted = false;
                    break;
                }
            }

            return canBeContracted;
        }

        /// <summary>
        /// Determine the edge that all <see cref="DemandPair"/>s that start at <paramref name="node"/> have in common, if it exists.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> at which all <see cref="DemandPair"/>s start.</param>
        /// <param name="dpsAtNode">The <see cref="DemandPair"/>s that start at <paramref name="node"/>.</param>
        /// <returns>The edge connected to <paramref name="node"/> that all <see cref="DemandPair"/>s in <paramref name="dpsAtNode"/> use, or a tuple of two times <see langword="null"/> if that edge does not exist.</returns>
        private (TreeNode, TreeNode) DetermineCommonEdge(TreeNode node, CountedCollection<DemandPair> dpsAtNode)
        {
            (TreeNode, TreeNode) firstEdge = (null, null);

            // Determine whether the demand pairs that start at this node go in the same direction.
            foreach (DemandPair dp in dpsAtNode.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                (TreeNode, TreeNode) edge;

                if (dp.Node1 == node)
                {
                    edge = Utils.OrderEdgeSmallToLarge(dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(0));
                }
                else
                {
                    int length = dp.LengthOfPath(MockCounter);
                    edge = Utils.OrderEdgeSmallToLarge(dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(length - 1));
                }

                if (firstEdge.Item1 is null)
                {
                    firstEdge = edge;
                }
                else if (firstEdge != edge)
                {
                    return (null, null);
                }
            }

            return firstEdge;
        }

        /// <summary>
        /// Determine for a given leaf <see cref="TreeNode"/> which edges can be contracted according to this <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="leaf">The <see cref="TreeNode"/> for which we want to determine which edges can be contracted.</param>
        /// <param name="dpsAtNode">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s that start at <paramref name="leaf"/>.</param>
        /// <param name="edgesToBeContracted"><see cref="HashSet{T}"/> containing all edges that can be contracted in this application of this <see cref="ReductionRule"/>.</param>
        private void CheckApplicabilityLeaf(TreeNode leaf, CountedCollection<DemandPair> dpsAtNode, HashSet<(TreeNode, TreeNode)> edgesToBeContracted)
        {
            (TreeNode, TreeNode) secondEdge = (null, null);

            foreach (DemandPair dp in dpsAtNode.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                if (dp.LengthOfPath(Measurements.DemandPairsOperationsCounter) < 2)
                {
                    return;
                }

                (TreeNode, TreeNode) edge;
                bool allOtherEdgesOnPathWillBeContracted = true;

                if (dp.Node1 == leaf)
                {
                    edge = Utils.OrderEdgeSmallToLarge(dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(1));

                    // Check whether all other edges on this demand path are already being contracted. If so, we cannot contract this edge.
                    foreach ((TreeNode, TreeNode) e in dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Skip(1))
                    {
                        if (!edgesToBeContracted.Contains(Utils.OrderEdgeSmallToLarge(e)))
                        {
                            allOtherEdgesOnPathWillBeContracted = false;
                            break;
                        }
                    }
                }
                else
                {
                    int length = dp.LengthOfPath(MockCounter);
                    edge = Utils.OrderEdgeSmallToLarge(dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(length - 2));

                    // Check whether all other edges on this demand path are already being contracted. If so, we cannot contract this edge.
                    foreach ((TreeNode, TreeNode) e in dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Take(length - 1))
                    {
                        if (!edgesToBeContracted.Contains(Utils.OrderEdgeSmallToLarge(e)))
                        {
                            allOtherEdgesOnPathWillBeContracted = false;
                            break;
                        }
                    }
                }

                if (allOtherEdgesOnPathWillBeContracted)
                {
                    return;
                }

                if (secondEdge.Item1 is null)
                {
                    secondEdge = edge;
                }
                else if (secondEdge != edge)
                {
                    return;
                }
            }

            edgesToBeContracted.Add(Utils.OrderEdgeSmallToLarge((leaf, leaf.Neighbours(Measurements.TreeOperationsCounter).First())));
        }

        /// <summary>
        /// Finds and contracts all edges that can be contracted by checking all <see cref="TreeNode"/>s in <paramref name="nodesToCheck"/>.
        /// </summary>
        /// <param name="nodesToCheck">The <see cref="IEnumerable{T}"/> with <see cref="TreeNode"/>s to check.</param>
        /// <returns>Whether this <see cref="ReductionRule"/> was applicable.</returns>
        private bool TryApplyReductionRule(IEnumerable<TreeNode> nodesToCheck)
        {
            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();
            foreach (TreeNode node in nodesToCheck)
            {
                CheckApplicabilityNode(node, edgesToBeContracted);
            }

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
            Utils.NullCheck(changedDemandPairs, nameof(changedDemandPairs), $"Trying to apply the {GetType().Name} rule after a demand path was changed, but the IEnumerable of removed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            HashSet<TreeNode> nodesToCheck = new HashSet<TreeNode>();

            foreach (((TreeNode, TreeNode) _, TreeNode node, CountedCollection<DemandPair> _) in contractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                nodesToCheck.Add(node);
            }

            foreach (DemandPair dp in removedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                nodesToCheck.Add(dp.Node1);
                nodesToCheck.Add(dp.Node2);
            }

            foreach ((CountedList<(TreeNode, TreeNode)> edges, DemandPair _) in changedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode n1, TreeNode n2) in edges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    nodesToCheck.Add(n1);
                    nodesToCheck.Add(n2);
                }
            }

            contractedEdges.Clear(Measurements.TreeOperationsCounter);
            removedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            removedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return TryApplyReductionRule(nodesToCheck);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule for the first time");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            return TryApplyReductionRule(Tree.Nodes(Measurements.TreeOperationsCounter));
        }
    }
}
