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
    /// Implementation of the Overloaded Edge <see cref="ReductionRule"/>.
    /// <br/>
    /// <b>Rule:</b> If more than k length-two demand paths pass through an edge e, then cut e.
    /// </summary>
    public class OverloadedEdge : ReductionRule
    {
        /// <summary>
        /// The maximum size the solution is allowed to have.
        /// </summary>
        private int MaxSolutionSize { get; }

        /// <summary>
        /// The part of the solution that has been found thus far.
        /// </summary>
        private List<Edge<Node>> PartialSolution { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge.
        /// </summary>
        private CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for the <see cref="OverloadedEdge"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <param name="demandPairsPerEdge"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="partialSolution"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public OverloadedEdge(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, List<Edge<Node>> partialSolution, int maxSolutionSize, CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(partialSolution, nameof(partialSolution), $"Trying to create an instance of the {GetType().Name} reduction rule, but the partial solution is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per edge is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSolutionSize), $"Trying to create an instance of the {GetType().Name} reduction rule, but the maximum number of edges that can be cut is smaller than zero!");
            }
#endif
            MaxSolutionSize = maxSolutionSize;
            PartialSolution = partialSolution;
            DemandPairsPerEdge = demandPairsPerEdge;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Checks, given a <see cref="Dictionary{TKey, TValue}"/> with per edge how many length-2 <see cref="DemandPair"/>s pass through it, which edges are overloaded.
        /// </summary>
        /// <param name="edgeOccurrences"><see cref="Dictionary{TKey, TValue}"/> with the number of length-2 <see cref="DemandPair"/>s per edge.</param>
        /// <returns>A <see cref="CountedList{T}"/> of edges that can be cut.</returns>
        private CountedList<Edge<Node>> DetermineOverloadedEdges(CountedDictionary<Edge<Node>, int> edgeOccurrences)
        {
            CountedList<Edge<Node>> overloadedEdges = new CountedList<Edge<Node>>();
            int k = MaxSolutionSize - PartialSolution.Count;
            foreach (KeyValuePair<Edge<Node>, int> edge in edgeOccurrences.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                if (edge.Value > k)
                {
                    overloadedEdges.Add(edge.Key, Measurements.TreeOperationsCounter);
                }
            }
            return overloadedEdges;
        }

        /// <summary>
        /// Try to apply this <see cref="ReductionRule"/> on the <see cref="DemandPair"/>s in <paramref name="demandPairsToCheck"/>.
        /// </summary>
        /// <param name="demandPairsToCheck">The <see cref="IEnumerable{T}"/> with edges we want to check.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/> successfully, <see langword="false"/> otherwise.</returns>
        private bool TryApplyReductionRule(IEnumerable<DemandPair> demandPairsToCheck)
        {
            CountedDictionary<Edge<Node>, int> edgeOccurrences = new CountedDictionary<Edge<Node>, int>();
            foreach (DemandPair demandPair in demandPairsToCheck)
            {
                if (demandPair.LengthOfPath(Measurements.DemandPairsOperationsCounter) != 2)
                {
                    continue;
                }

                Edge<Node> edge1 = demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First();
                Edge<Node> edge2 = demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Last();
                if (!edgeOccurrences.ContainsKey(edge1, MockCounter))
                {
                    edgeOccurrences[edge1, MockCounter] = 0;
                }
                if (!edgeOccurrences.ContainsKey(edge2, MockCounter))
                {
                    edgeOccurrences[edge2, MockCounter] = 0;
                }

                edgeOccurrences[edge1, Measurements.TreeOperationsCounter]++;
                edgeOccurrences[edge2, Measurements.TreeOperationsCounter]++;
            }

            CountedList<Edge<Node>> edgesToBeCut = DetermineOverloadedEdges(edgeOccurrences);
            return TryCutEdges(edgesToBeCut);
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> demandPairsToCheck = new HashSet<DemandPair>();

            foreach ((Edge<Node> _, Node _, CountedCollection<DemandPair> demandPairs) in LastContractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                foreach (DemandPair demandPair in demandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    demandPairsToCheck.Add(demandPair);
                }
            }

            foreach ((CountedList<Edge<Node>> _, DemandPair demandPair) in LastChangedDemandPairs.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                if (demandPair.LengthOfPath(Measurements.DemandPairsOperationsCounter) != 2)
                {
                    continue;
                }

                if (DemandPairsPerEdge.TryGetValue(demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First(), out CountedCollection<DemandPair> dpsFirst, Measurements.DemandPairsPerEdgeKeysCounter))
                {
                    foreach (DemandPair dp in dpsFirst.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                    {
                        demandPairsToCheck.Add(dp);
                    }
                }
                if (DemandPairsPerEdge.TryGetValue(demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Last(), out CountedCollection<DemandPair> dpsLast, Measurements.DemandPairsPerEdgeKeysCounter))
                {
                    foreach (DemandPair dp in dpsLast.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                    {
                        demandPairsToCheck.Add(dp);
                    }
                }
            }

            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return TryApplyReductionRule(demandPairsToCheck);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule for the first time");
#endif
            HasRun = true;
            Measurements.TimeSpentCheckingApplicability.Start();
            return TryApplyReductionRule(DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter));
        }
    }
}
