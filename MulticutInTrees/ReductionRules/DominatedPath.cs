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
    /// <see cref="ReductionRule"/> that gets rid of <see cref="DemandPair"/>s with a path that is a superset of the path of another <see cref="DemandPair"/>.
    /// <br/>
    /// Rule: If Pi is a subset of Pj for two distinct demand pairs Pi, Pj, then delete Pj.
    /// </summary>
    public class DominatedPath : ReductionRule
    {
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge.
        /// </summary>
        private CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for the <see cref="DominatedPath"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        /// <param name="demandPairsPerEdge">The <see cref="CountedDictionary{TKey, TValue}"/> with <see cref="Edge{TNode}"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public DominatedPath(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with demand paths is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per edge is null!");
#endif
            DemandPairsPerEdge = demandPairsPerEdge;
        }

        /// <summary>
        /// Try to apply this <see cref="ReductionRule"/> on the <see cref="DemandPair"/>s in <paramref name="demandPairsToCheck"/>.
        /// </summary>
        /// <param name="demandPairsToCheck">The <see cref="IEnumerable{T}"/> with edges we want to check.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/> successfully, <see langword="false"/> otherwise.</returns>
        private bool TryApplyReductionRule(IEnumerable<DemandPair> demandPairsToCheck)
        {
            HashSet<DemandPair> demandPairsToRemove = new();

            foreach (DemandPair demandPair in demandPairsToCheck)
            {
                if (demandPairsToRemove.Contains(demandPair))
                {
                    continue;
                }

                Edge<Node> firstEdge = demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First();
                Edge<Node> lastEdge = demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Last();
                foreach (DemandPair otherPair in DemandPairsPerEdge[firstEdge, Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                {
                    if (demandPairsToRemove.Contains(otherPair) || demandPair == otherPair)
                    {
                        continue;
                    }

                    if (otherPair.EdgeIsPartOfPath(lastEdge, Measurements.TreeOperationsCounter))
                    {
                        demandPairsToRemove.Add(otherPair);
                    }
                }
            }

            return TryRemoveDemandPairs(new CountedList<DemandPair>(demandPairsToRemove, Measurements.DemandPairsOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> demandPairsToCheck = new();
            HashSet<DemandPair> removedPairs = new(LastRemovedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter));

            foreach ((Edge<Node> _, Node _, CountedCollection<DemandPair> demandPairs) in LastContractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                foreach (DemandPair demandPair in demandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    if (removedPairs.Contains(demandPair))
                    {
                        continue;
                    }

                    demandPairsToCheck.Add(demandPair);
                }
            }

            foreach ((CountedList<Edge<Node>> _, DemandPair demandPair) in LastChangedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                if (removedPairs.Contains(demandPair))
                {
                    continue;
                }

                demandPairsToCheck.Add(demandPair);
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
