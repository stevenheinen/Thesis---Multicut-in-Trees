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
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="Node"/>.
        /// </summary>
        private CountedDictionary<Node, CountedCollection<DemandPair>> DemandPairsPerNode { get; }

        /// <summary>
        /// The maximum size the solution is allowed to have.
        /// </summary>
        private int MaxSolutionSize { get; }

        /// <summary>
        /// The part of the solution that has been found thus far.
        /// </summary>
        private List<Edge<Node>> PartialSolution { get; }

        /// <summary>
        /// Constructor for <see cref="OverloadedL3Leaves"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="demandPairsPerNode"><see cref="CountedDictionary{TKey, TValue}"/> with for each <see cref="Node"/> all <see cref="DemandPair"/>s that start at that <see cref="Node"/>.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="demandPairsPerNode"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public OverloadedL3Leaves(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode, List<Edge<Node>> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm)
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
        private bool HandleOverloadedL3Leaves(CountedList<(Node, CountedList<DemandPair>, Node)> overloadedLeaves)
        {
            if (overloadedLeaves.Count(MockCounter) == 0)
            {
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }

            List<(DemandPair, Node, Node)> dpsToBeChanged = new();
            List<DemandPair> dpsToBeRemoved = new();

            foreach ((Node parent, CountedList<DemandPair> affectedDemandPairs, Node otherNode) in overloadedLeaves.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                DemandPair changedDemandPair = affectedDemandPairs[0, Measurements.DemandPairsOperationsCounter];
                Node leaf = changedDemandPair.Node1 == otherNode ? changedDemandPair.Node2 : changedDemandPair.Node1;
                dpsToBeChanged.Add((changedDemandPair, leaf, parent));
                dpsToBeRemoved.AddRange(affectedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).Skip(1));
            }

            Measurements.TimeSpentCheckingApplicability.Stop();
            Measurements.TimeSpentModifyingInstance.Start();
            Algorithm.RemoveDemandPairs(new CountedList<DemandPair>(dpsToBeRemoved, Measurements.DemandPairsOperationsCounter), Measurements);
            Algorithm.ChangeEndpointOfDemandPairs(new CountedList<(DemandPair, Node, Node)>(dpsToBeChanged, Measurements.DemandPairsOperationsCounter), Measurements);
            Measurements.TimeSpentModifyingInstance.Stop();
            return true;
        }

        /// <summary>
        /// Determines the first group of L3-Leaves on which this reduction rule is applicable.
        /// </summary>
        /// <returns>A <see cref="CountedList{T}"/> with the parent, overloaded <see cref="DemandPair"/>s and the other endpoint of the <see cref="DemandPair"/>s.</returns>
        private CountedList<(Node, CountedList<DemandPair>, Node)> FindOverloadedL3Leaves()
        {
            int k = MaxSolutionSize - PartialSolution.Count;
            foreach (Node node in Tree.Nodes(Measurements.TreeOperationsCounter))
            {
                if (!DemandPairsPerNode.TryGetValue(node, out CountedCollection<DemandPair> demandPairsAtNode, Measurements.DemandPairsPerEdgeKeysCounter) || demandPairsAtNode.Count(Measurements.DemandPairsOperationsCounter) <= k)
                {
                    continue;
                }
                CountedDictionary<Node, CountedList<DemandPair>> i3NodeToDemandPairsInChildren = new();
                foreach (DemandPair demandPair in demandPairsAtNode.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    Node otherEndpoint = demandPair.Node1 == node ? demandPair.Node2 : demandPair.Node1;
                    if (otherEndpoint.Type != NodeType.L3)
                    {
                        continue;
                    }
                    Node parent = otherEndpoint.Neighbours(Measurements.TreeOperationsCounter).First();
                    if (!i3NodeToDemandPairsInChildren.ContainsKey(parent, MockCounter))
                    {
                        i3NodeToDemandPairsInChildren[parent, MockCounter] = new CountedList<DemandPair>();
                    }
                    i3NodeToDemandPairsInChildren[parent, Measurements.TreeOperationsCounter].Add(demandPair, Measurements.DemandPairsOperationsCounter);
                }
                foreach (KeyValuePair<Node, CountedList<DemandPair>> kv in i3NodeToDemandPairsInChildren.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    if (kv.Value.Count(Measurements.DemandPairsOperationsCounter) <= k)
                    {
                        continue;
                    }
                    CountedList<(Node, CountedList<DemandPair>, Node)> result = new();
                    result.Add((kv.Key, kv.Value, node), Measurements.TreeOperationsCounter);
                    return result;
                }
            }
            return new CountedList<(Node, CountedList<DemandPair>, Node)>();
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying the {GetType().Name} reduction rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            CountedList<(Node, CountedList<DemandPair>, Node)> overloadedLeaves = FindOverloadedL3Leaves();
            return HandleOverloadedL3Leaves(overloadedLeaves);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying the {GetType().Name} reduction rule for the first time");
#endif
            HasRun = true;
            LastContractedEdges.Clear(MockCounter);
            LastRemovedDemandPairs.Clear(MockCounter);
            LastChangedDemandPairs.Clear(MockCounter);
            Measurements.TimeSpentCheckingApplicability.Start();
            CountedList<(Node, CountedList<DemandPair>, Node)> overloadedLeaves = FindOverloadedL3Leaves();
            return HandleOverloadedL3Leaves(overloadedLeaves);
        }
    }
}
