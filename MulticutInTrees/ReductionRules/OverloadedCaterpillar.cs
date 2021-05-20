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
    /// Implementation of the Overloaded Caterpillar <see cref="ReductionRule"/>.
    /// <br/>
    /// <b>Rule:</b> If there are k+1 demand pairs (v,u1), (v,u2), ..., (v,u(k+1)) such that nodes u1, ..., u(k+1) belong to the same caterpillar component that does not contain v, then (one of) the longest of these demand paths can be deleted.
    /// </summary>
    public class OverloadedCaterpillar : ReductionRule
    {
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="Node"/>.
        /// </summary>
        private CountedDictionary<Node, CountedCollection<DemandPair>> DemandPairsPerNode { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> with the identifier of the caterpillar component each <see cref="Node"/> is part of, or -1 if it is not part of any caterpillar component.
        /// </summary>
        private CountedDictionary<Node, int> CaterpillarComponentPerNode { get; }

        /// <summary>
        /// The maximum size the solution is allowed to have.
        /// </summary>
        private int MaxSolutionSize { get; }

        /// <summary>
        /// The part of the solution that has been found thus far.
        /// </summary>
        private List<Edge<Node>> PartialSolution { get; }

        /// <summary>
        /// Constructor for <see cref="OverloadedCaterpillar"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="demandPairsPerNode"><see cref="CountedDictionary{TKey, TValue}"/> with for each <see cref="Node"/> all <see cref="DemandPair"/>s that start at that <see cref="Node"/>.</param>
        /// <param name="caterpillarComponentPerNode"><see cref="CountedDictionary{TKey, TValue}"/> with the identifier of the caterpillar component per node.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="demandPairsPerNode"/>, <paramref name="caterpillarComponentPerNode"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public OverloadedCaterpillar(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode, CountedDictionary<Node, int> caterpillarComponentPerNode, List<Edge<Node>> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerNode, nameof(demandPairsPerNode), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per node is null!");
            Utils.NullCheck(caterpillarComponentPerNode, nameof(caterpillarComponentPerNode), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with the caterpillar component per node is null!");
            Utils.NullCheck(partialSolution, nameof(partialSolution), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with the partial solution is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException($"Trying to create an instance of the {GetType().Name} reduction rule, but the maximum solution size parameter is smaller than zero!");
            }
#endif
            DemandPairsPerNode = demandPairsPerNode;
            CaterpillarComponentPerNode = caterpillarComponentPerNode;
            PartialSolution = partialSolution;
            MaxSolutionSize = maxSolutionSize;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Determine the <see cref="DemandPair"/>s that can be deleted. A <see cref="DemandPair"/> can be deleted when there are at least k+1 <see cref="DemandPair"/>s starting at a node v and going to nodes in the same caterpillar component that does not contain v. We delete the longest of these paths.
        /// </summary>
        /// <returns>A <see cref="CountedList{T}"/> with <see cref="DemandPair"/>s that can be deleted.</returns>
        private CountedList<DemandPair> DeterminePairsToBeDeleted()
        {
            int k = MaxSolutionSize - PartialSolution.Count;
            CountedList<DemandPair> result = new();
            foreach (Node node in DemandPairsPerNode.GetKeys(Measurements.DemandPairsPerEdgeKeysCounter))
            {
                CountedCollection<DemandPair> demandPairs = DemandPairsPerNode[node, Measurements.DemandPairsPerEdgeKeysCounter];
                if (demandPairs.Count(Measurements.DemandPairsOperationsCounter) <= k)
                {
                    continue;
                }
                int caterpillarComponent = CaterpillarComponentPerNode[node, Measurements.TreeOperationsCounter];
                CountedDictionary<int, CountedList<DemandPair>> pairsPerComponent = new();
                foreach (DemandPair demandPair in demandPairs.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                {
                    Node endpoint = demandPair.Node1 == node ? demandPair.Node2 : demandPair.Node1;
                    int otherComponent = CaterpillarComponentPerNode[endpoint, Measurements.TreeOperationsCounter];
                    if (otherComponent == -1 || otherComponent == caterpillarComponent)
                    {
                        continue;
                    }
                    if (!pairsPerComponent.ContainsKey(otherComponent, MockCounter))
                    {
                        pairsPerComponent[otherComponent, MockCounter] = new CountedList<DemandPair>();
                    }
                    pairsPerComponent[otherComponent, MockCounter].Add(demandPair, Measurements.DemandPairsOperationsCounter);
                }
                foreach (CountedList<DemandPair> pairsToComponent in pairsPerComponent.GetValues(Measurements.DemandPairsOperationsCounter))
                {
                    if (pairsToComponent.Count(Measurements.DemandPairsOperationsCounter) <= k)
                    {
                        continue;
                    }
                    result.Add(pairsToComponent.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).Aggregate((n, m) => n.LengthOfPath(Measurements.DemandPairsOperationsCounter) > m.LengthOfPath(Measurements.DemandPairsOperationsCounter) ? n : m), Measurements.DemandPairsOperationsCounter);
                }
            }
            return result;
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
            foreach (KeyValuePair<Node, int> kv in DFS.DetermineCaterpillarComponents(Tree.Nodes(Measurements.TreeOperationsCounter), Measurements.TreeOperationsCounter))
            {
                CaterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            Measurements.TimeSpentCheckingApplicability.Start();
            CountedList<DemandPair> pairsToBeDeleted = DeterminePairsToBeDeleted();
            return TryRemoveDemandPairs(pairsToBeDeleted);
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying the {GetType().Name} reduction rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            if (LastContractedEdges.Count(MockCounter) == 0 && LastChangedDemandPairs.Count(MockCounter) == 0)
            {
                LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }

            CountedList<DemandPair> pairsToBeDeleted = DeterminePairsToBeDeleted();

            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return TryRemoveDemandPairs(pairsToBeDeleted);
        }
    }
}
