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
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per <see cref="TreeNode"/>.
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerNode"/> is <see langword="null"/>.</exception>
        public UniqueDirection(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the dominated path reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the dominated path reduction rule, but the list with demand paths is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the dominated path reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerNode, nameof(demandPairsPerNode), "Trying to create an instance of the dominated path reduction rule, but the dictionary with demand pairs per node is null!");
#endif
            DemandPairsPerNode = demandPairsPerNode;
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
            (TreeNode, TreeNode) firstEdge = (null, null);

            foreach (DemandPair dp in dpsAtNode.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                if (dp.LengthOfPath(Measurements.DemandPairsOperationsCounter) < 2)
                {
                    return;
                }
                
                (TreeNode, TreeNode) edge;
                bool allOtherEdgesOnPathWillBeContracted = true;
                
                if (dp.Node1 == node)
                {
                    edge = Utils.OrderEdgeSmallToLarge(dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(0));

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
                    edge = Utils.OrderEdgeSmallToLarge(dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(length - 1));

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

                if (firstEdge.Item1 is null)
                {
                    firstEdge = edge;
                }
                else if (firstEdge != edge)
                {
                    return;
                }
            }

            TreeNode otherNode = firstEdge.Item1 == node ? firstEdge.Item2 : firstEdge.Item1;
            edgesToBeContracted.Add(Utils.OrderEdgeSmallToLarge((node, node.Neighbours(Measurements.TreeOperationsCounter).First(n => n != otherNode))));
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
        private bool GetEdgesThatCanBeContracted(IEnumerable<TreeNode> nodesToCheck)
        {
            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();
            foreach (TreeNode node in nodesToCheck)
            {
                if (node.ID == 295 || node.ID == 422)
                {

                }

                CheckApplicabilityNode(node, edgesToBeContracted);
            }

            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {

        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathChanged(CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
#if !EXPERIMENT
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Unique Direction rule after a demand path was changed, but the IEnumerable of removed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Unique Direction rule after a demand path was changed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<TreeNode> nodesToCheck = new HashSet<TreeNode>();
            foreach ((CountedList<(TreeNode, TreeNode)> edges, DemandPair _) in changedEdgesPerDemandPairList.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode n1, TreeNode n2) in edges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    nodesToCheck.Add(n1);
                    nodesToCheck.Add(n2);
                }
            }

            return GetEdgesThatCanBeContracted(nodesToCheck);
        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Unique Direction rule after a demand path was removed, but the IEnumerable of removed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Unique Direction rule after a demand pair was removed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<TreeNode> nodesToCheck = new HashSet<TreeNode>();
            foreach (DemandPair dp in removedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                nodesToCheck.Add(dp.Node1);
                nodesToCheck.Add(dp.Node2);
            }

            return GetEdgesThatCanBeContracted(nodesToCheck);
        }

        /// <inheritdoc/>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to apply the Unique Direction rule after an edge was contracted, but the IEnumerable of contracted edges is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Unique Direction rule after an edge was contracted...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<TreeNode> nodesToCheck = new HashSet<TreeNode>(contractedEdgeNodeTupleList.GetCountedEnumerable(Measurements.TreeOperationsCounter).Select(t => t.Item2));
            return GetEdgesThatCanBeContracted(nodesToCheck);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine("Applying Unique Direction rule for the first time...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            return GetEdgesThatCanBeContracted(Tree.Nodes(Measurements.TreeOperationsCounter));
        }
    }
}
