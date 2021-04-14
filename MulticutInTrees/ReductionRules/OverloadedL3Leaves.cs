// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// Implementation of the Overloaded L3-Leaves <see cref="ReductionRule"/>.
    /// <br/>
    /// <b>Rule:</b> If there are k+1 demand pairs (v,u1), (v,u2), ..., (v,u(k+1)) such that nodes u1, ..., u(k+1) are all L3-leaves of an I3-node u, then remove all these demand paths and add a new demand path between v and u.
    /// </summary>
    public class OverloadedL3Leaves : ReductionRule
    {
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="TreeNode"/>.
        /// </summary>
        private CountedDictionary<TreeNode, CountedCollection<DemandPair>> DemandPairsPerNode { get; }

        /// <summary>
        /// The maximum size the solution is allowed to have.
        /// </summary>
        private int MaxSolutionSize { get; }

        /// <summary>
        /// The part of the solution that has been found thus far.
        /// </summary>
        private List<(TreeNode, TreeNode)> PartialSolution { get; }

        /// <summary>
        /// Constructor for <see cref="OverloadedL3Leaves"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/>.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> with <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="demandPairsPerNode"><see cref="CountedDictionary{TKey, TValue}"/> with for each <see cref="TreeNode"/> all <see cref="DemandPair"/>s that start at that <see cref="TreeNode"/>.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="demandPairsPerNode"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public OverloadedL3Leaves(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode, List<(TreeNode, TreeNode)> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utilities.Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with demand pairs is null!");
            Utilities.Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utilities.Utils.NullCheck(demandPairsPerNode, nameof(demandPairsPerNode), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per node is null!");
            Utilities.Utils.NullCheck(partialSolution, nameof(partialSolution), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with the partial solution is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException($"Trying to create an instance of the {GetType().Name} reduction rule, but the maximum solution size parameter is smaller than zero!");
            }
#endif
            DemandPairsPerNode = demandPairsPerNode;
            PartialSolution = partialSolution;
            MaxSolutionSize = maxSolutionSize;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Tells the algorithm to remove the <see cref="DemandPair"/>s that correspond to the overloaded leaves and to replace it by one between the other node and the parent of the overloaded leaves.
        /// </summary>
        /// <param name="overloadedLeaves"><see cref="CountedList{T}"/> with the parent, overloaded <see cref="DemandPair"/>s and the other endpoint of the <see cref="DemandPair"/>s.</param>
        /// <returns><see langword="true"/> if any modification to the instance were made, <see langword="false"/> otherwise.</returns>
        private bool HandleOverloadedL3Leaves(CountedList<(TreeNode, CountedList<DemandPair>, TreeNode)> overloadedLeaves)
        {
            if (overloadedLeaves.Count(MockCounter) == 0)
            {
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }

            List<(DemandPair, TreeNode, TreeNode)> dpsToBeChanged = new List<(DemandPair, TreeNode, TreeNode)>();
            List<DemandPair> dpsToBeRemoved = new List<DemandPair>();

            foreach ((TreeNode parent, CountedList<DemandPair> affectedDemandPairs, TreeNode otherNode) in overloadedLeaves.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                DemandPair changedDemandPair = affectedDemandPairs[0, Measurements.DemandPairsOperationsCounter];
                TreeNode leaf = changedDemandPair.Node1 == otherNode ? changedDemandPair.Node2 : changedDemandPair.Node1;
                dpsToBeChanged.Add((changedDemandPair, leaf, parent));
                dpsToBeRemoved.AddRange(affectedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).Skip(1));
            }

            Measurements.TimeSpentCheckingApplicability.Stop();
            Measurements.TimeSpentModifyingInstance.Start();
            Algorithm.RemoveDemandPairs(new CountedList<DemandPair>(dpsToBeRemoved, Measurements.DemandPairsOperationsCounter), Measurements);
            Algorithm.ChangeEndpointOfDemandPairs(new CountedList<(DemandPair, TreeNode, TreeNode)>(dpsToBeChanged, Measurements.DemandPairsOperationsCounter), Measurements);
            Measurements.TimeSpentModifyingInstance.Stop();
            return true;
        }

        /// <summary>
        /// Determines the groups of L3-Leaves on which this reduction rule is applicable.
        /// </summary>
        /// <returns>A <see cref="CountedList{T}"/> with the parent, overloaded <see cref="DemandPair"/>s and the other endpoint of the <see cref="DemandPair"/>s.</returns>
        private CountedList<(TreeNode, CountedList<DemandPair>, TreeNode)> FindOverloadedL3Leaves()
        {
            int k = MaxSolutionSize - PartialSolution.Count;
            CountedList<(TreeNode, CountedList<DemandPair>, TreeNode)> result = new CountedList<(TreeNode, CountedList<DemandPair>, TreeNode)>();
            foreach (TreeNode node in Tree.Nodes(Measurements.TreeOperationsCounter))
            {
                DemandPairsPerNode.TryGetValue(node, out CountedCollection<DemandPair> demandPairsAtNode, Measurements.DemandPairsPerEdgeKeysCounter);
                if (demandPairsAtNode is null || demandPairsAtNode.Count(Measurements.DemandPairsOperationsCounter) <= k)
                {
                    continue;
                }
                CountedDictionary<TreeNode, CountedList<DemandPair>> i3NodeToDemandPairsInChildren = new CountedDictionary<TreeNode, CountedList<DemandPair>>();
                foreach (DemandPair demandPair in demandPairsAtNode.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    TreeNode otherEndpoint = demandPair.Node1 == node ? demandPair.Node2 : demandPair.Node1;
                    if (otherEndpoint.Type != NodeType.L3)
                    {
                        continue;
                    }
                    TreeNode parent = otherEndpoint.GetParent(Measurements.TreeOperationsCounter) ?? otherEndpoint.Children(Measurements.TreeOperationsCounter).First();
                    if (!i3NodeToDemandPairsInChildren.ContainsKey(parent, MockCounter))
                    {
                        i3NodeToDemandPairsInChildren[parent, MockCounter] = new CountedList<DemandPair>();
                    }
                    i3NodeToDemandPairsInChildren[parent, Measurements.TreeOperationsCounter].Add(demandPair, Measurements.DemandPairsOperationsCounter);
                }
                foreach (KeyValuePair<TreeNode, CountedList<DemandPair>> kv in i3NodeToDemandPairsInChildren.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    if (kv.Value.Count(Measurements.DemandPairsOperationsCounter) <= k)
                    {
                        continue;
                    }
                    result.Add((kv.Key, kv.Value, node), Measurements.TreeOperationsCounter);
                }
            }
            return result;
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying the {GetType().Name} reduction rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            if (LastContractedEdges.Count(MockCounter) == 0)
            {
                LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
                LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }

            bool containsI3OrL3Node = false;
            foreach (((TreeNode, TreeNode) _, TreeNode newNode, CountedCollection<DemandPair> _) in LastContractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                if (newNode.Type == NodeType.I3 || newNode.Type == NodeType.L3)
                {
                    containsI3OrL3Node = true;
                    break;
                }
            }
            if (!containsI3OrL3Node)
            {
                LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
                LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
                LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }
            CountedList<(TreeNode, CountedList<DemandPair>, TreeNode)> overloadedLeaves = FindOverloadedL3Leaves();
            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            return HandleOverloadedL3Leaves(overloadedLeaves);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying the {GetType().Name} reduction rule for the first time");
#endif
            HasRun = true;
            Measurements.TimeSpentCheckingApplicability.Start();
            CountedList<(TreeNode, CountedList<DemandPair>, TreeNode)> overloadedLeaves = FindOverloadedL3Leaves();
            return HandleOverloadedL3Leaves(overloadedLeaves);
        }
    }
}
