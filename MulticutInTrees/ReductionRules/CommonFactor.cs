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
    /// <see cref="ReductionRule"/> that removes <see cref="DemandPair"/>s when they intersect enough other <see cref="DemandPair"/>s to be automatically separated.
    /// <br/>
    /// <b>Rule:</b> Let P0 be a demand path. If k + 1 demand paths P1, ..., P(k + 1) different from P0 but intersecting P0 are such that for every i != j, the common factor of Pi and Pj is a subset of P0, i.e. (Pi ∩ Pj) ⊆ P0, then delete P0 from the set of demand paths.
    /// </summary>
    public class CommonFactor : ReductionRule
    {
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> with per <see cref="DemandPair"/> the <see cref="DemandPair"/>s it intersects with.
        /// </summary>
        private CountedDictionary<DemandPair, CountedCollection<DemandPair>> IntersectingDemandPairs { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> with the edges that are the intersection between two <see cref="DemandPair"/>s.
        /// </summary>
        private CountedDictionary<(DemandPair, DemandPair), CountedCollection<Edge<Node>>> DemandPairIntersections { get; }

        /// <summary>
        /// The maximum size the solution is allowed to have.
        /// </summary>
        private int MaxSolutionSize { get; }

        /// <summary>
        /// The part of the solution that has been found thus far.
        /// </summary>
        private List<Edge<Node>> PartialSolution { get; }

        /// <summary>
        /// Constructor for <see cref="CommonFactor"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>"
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public CommonFactor(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, List<Edge<Node>> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(partialSolution, nameof(partialSolution), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with the partial solution is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException($"Trying to create an instance of the {GetType().Name} reduction rule, but the maximum solution size parameter is smaller than zero!");
            }
#endif
            MockCounter = new Counter();
            PartialSolution = partialSolution;
            MaxSolutionSize = maxSolutionSize;
            IntersectingDemandPairs = new CountedDictionary<DemandPair, CountedCollection<DemandPair>>();
            DemandPairIntersections = new CountedDictionary<(DemandPair, DemandPair), CountedCollection<Edge<Node>>>();
        }

        /// <summary>
        /// Checks whether this <see cref="ReductionRule"/> is applicable on <paramref name="p0"/>.
        /// </summary>
        /// <param name="p0">The <see cref="DemandPair"/> for which we want to know if this rule is applicable.</param>
        /// <param name="intersectingPairs">The <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s that intersect <paramref name="p0"/>.</param>
        /// <param name="intersectingPairCount">The number of <see cref="DemandPair"/>s that intersect with <paramref name="p0"/>.</param>
        /// <param name="k">The remaining number of edges that can be cut.</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> can be applied on <paramref name="p0"/>, <see langword="false"/> otherwise.</returns>
        private bool CheckApplicabilitySinglePair(DemandPair p0, CountedCollection<DemandPair> intersectingPairs, int intersectingPairCount, int k)
        {
            IEnumerable<Edge<Node>> p0Path = p0.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter);
            int subsetFailCount = 0;
            HashSet<(DemandPair, DemandPair)> checkedPairs = new();
            foreach (DemandPair pi in intersectingPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach (DemandPair pj in intersectingPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    if (pi == pj)
                    {
                        continue;
                    }

                    (DemandPair, DemandPair) key = GetIntersectionKey(pi, pj);
                    if (checkedPairs.Contains(key))
                    {
                        continue;
                    }

                    checkedPairs.Add(key);

                    // If key is not in the dictionary, pi and pj do not intersect, and since the empty set is a subset of every set, this is a correct case.
                    if (!DemandPairIntersections.TryGetValue(key, out CountedCollection<Edge<Node>> intersection, Measurements.DemandPairsOperationsCounter))
                    {
                        continue;
                    }

                    if (!intersection.GetCountedEnumerable(Measurements.TreeOperationsCounter).IsSubsetOf(p0Path))
                    {
                        subsetFailCount++;
                    }

                    if (intersectingPairCount - subsetFailCount <= k)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Uses <see cref="DemandPair.ID"/> to sort <paramref name="dp1"/> and <paramref name="dp2"/> in order to be used as key for <see cref="DemandPairIntersections"/>.
        /// </summary>
        /// <param name="dp1">The first <see cref="DemandPair"/>.</param>
        /// <param name="dp2">The second <see cref="DemandPair"/>.</param>
        /// <returns>A tuple with <paramref name="dp1"/> and <paramref name="dp2"/> sorted on their unique ID to be used as key for <see cref="DemandPairIntersections"/>.</returns>
        private static (DemandPair, DemandPair) GetIntersectionKey(DemandPair dp1, DemandPair dp2)
        {
            return dp1.ID < dp2.ID ? (dp1, dp2) : (dp2, dp1);
        }

        /// <summary>
        /// Checks whether this <see cref="ReductionRule"/> is applicable on any of the <see cref="DemandPair"/>s in <paramref name="demandPairsToCheck"/>, and applies it.
        /// </summary>
        /// <param name="demandPairsToCheck">The <see cref="DemandPair"/>s we want to try this <see cref="ReductionRule"/> on.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/>, <see langword="false"/> otherwise.</returns>
        private bool CheckApplicability(IEnumerable<DemandPair> demandPairsToCheck)
        {
            List<DemandPair> pairsToBeRemoved = new();

            int k = MaxSolutionSize - PartialSolution.Count;
            foreach (DemandPair p0 in demandPairsToCheck)
            {
                if (!IntersectingDemandPairs.TryGetValue(p0, out CountedCollection<DemandPair> intersectingPairs, Measurements.DemandPairsOperationsCounter))
                {
                    continue;
                }

                int intersectingPairCount = intersectingPairs.Count(Measurements.DemandPairsOperationsCounter);
                if (intersectingPairCount <= k)
                {
                    continue;
                }

                if (CheckApplicabilitySinglePair(p0, intersectingPairs, intersectingPairCount, k))
                {
                    pairsToBeRemoved.Add(p0);
                }
            }

            return TryRemoveDemandPairs(new CountedList<DemandPair>(pairsToBeRemoved, Measurements.DemandPairsOperationsCounter));
        }

        /// <summary>
        /// Computes the intersections of every two <see cref="DemandPair"/>s and saves information in <see cref="IntersectingDemandPairs"/> and <see cref="DemandPairIntersections"/>.
        /// </summary>
        private void FindInitialIntersections()
        {
            int numberOfDPs = DemandPairs.Count(MockCounter);
            List<DemandPair> temporaryDPs = DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).ToList();
            for (int i = 0; i < numberOfDPs - 1; i++)
            {
                DemandPair dp1 = temporaryDPs[i];
                IEnumerable<Edge<Node>> path1 = dp1.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter);
                for (int j = i + 1; j < numberOfDPs; j++)
                {
                    DemandPair dp2 = temporaryDPs[j];
                    IEnumerable<Edge<Node>> path2 = dp2.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter);
                    IEnumerable<Edge<Node>> intersection = path1.Intersect(path2);

                    if (!intersection.Any())
                    {
                        continue;
                    }

                    if (!IntersectingDemandPairs.ContainsKey(dp1, MockCounter))
                    {
                        IntersectingDemandPairs[dp1, MockCounter] = new CountedCollection<DemandPair>();
                    }
                    IntersectingDemandPairs[dp1, Measurements.DemandPairsOperationsCounter].Add(dp2, Measurements.DemandPairsOperationsCounter);

                    if (!IntersectingDemandPairs.ContainsKey(dp2, MockCounter))
                    {
                        IntersectingDemandPairs[dp2, MockCounter] = new CountedCollection<DemandPair>();
                    }
                    IntersectingDemandPairs[dp2, Measurements.DemandPairsOperationsCounter].Add(dp1, Measurements.DemandPairsOperationsCounter);

                    DemandPairIntersections[GetIntersectionKey(dp1, dp2), MockCounter] = new CountedCollection<Edge<Node>>(intersection, MockCounter);
                }
            }
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Executing {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> pairsToCheck = new();

            foreach (DemandPair dp in LastRemovedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach (DemandPair intersectingDemandPair in IntersectingDemandPairs[dp, Measurements.DemandPairsOperationsCounter].GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    DemandPairIntersections.Remove(GetIntersectionKey(dp, intersectingDemandPair), Measurements.DemandPairsOperationsCounter);
                }

                IntersectingDemandPairs.Remove(dp, Measurements.DemandPairsOperationsCounter);
            }

            foreach ((Edge<Node> edge, Node _, CountedCollection<DemandPair> dps) in LastContractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                List<DemandPair> pairsOnEdge = dps.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).ToList();
                for (int i = 0; i < pairsOnEdge.Count - 1; i++)
                {
                    if (!IntersectingDemandPairs.TryGetValue(pairsOnEdge[i], out CountedCollection<DemandPair> intersectionsI, Measurements.DemandPairsOperationsCounter))
                    {
                        continue;
                    }

                    for (int j = i + 1; j < pairsOnEdge.Count; j++)
                    {
                        if (!IntersectingDemandPairs.TryGetValue(pairsOnEdge[j], out CountedCollection<DemandPair> intersectionsJ, Measurements.DemandPairsOperationsCounter))
                        {
                            continue;
                        }
                        
                        foreach (DemandPair candidateDP in intersectionsI.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).Intersect(intersectionsJ.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter)))
                        {
                            pairsToCheck.Add(candidateDP);
                        }

                        (DemandPair, DemandPair) key = GetIntersectionKey(pairsOnEdge[i], pairsOnEdge[j]);
                        DemandPairIntersections[key, Measurements.DemandPairsOperationsCounter].Remove(edge, Measurements.TreeOperationsCounter);
                        if (DemandPairIntersections[key, MockCounter].Count(MockCounter) == 0)
                        {
                            DemandPairIntersections.Remove(key, MockCounter);
                            IntersectingDemandPairs[pairsOnEdge[i], MockCounter].Remove(pairsOnEdge[j], MockCounter);
                            IntersectingDemandPairs[pairsOnEdge[j], MockCounter].Remove(pairsOnEdge[i], MockCounter);
                            if (IntersectingDemandPairs[pairsOnEdge[i], MockCounter].Count(MockCounter) == 0)
                            {
                                IntersectingDemandPairs.Remove(pairsOnEdge[i], MockCounter);
                            }
                            if (IntersectingDemandPairs[pairsOnEdge[j], MockCounter].Count(MockCounter) == 0)
                            {
                                IntersectingDemandPairs.Remove(pairsOnEdge[j], MockCounter);
                            }
                        }
                    }
                }
            }

            foreach ((CountedList<Edge<Node>> edges, DemandPair dp) in LastChangedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                pairsToCheck.Add(dp);

                foreach (DemandPair intersectingDemandPair in IntersectingDemandPairs[dp, Measurements.DemandPairsOperationsCounter].GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    pairsToCheck.Add(intersectingDemandPair);
                    (DemandPair, DemandPair) key = GetIntersectionKey(dp, intersectingDemandPair);
                    foreach (Edge<Node> edge in edges.GetCountedEnumerable(Measurements.TreeOperationsCounter)) 
                    {
                        DemandPairIntersections[key, Measurements.DemandPairsOperationsCounter].Remove(edge, Measurements.TreeOperationsCounter);
                    }
                    if (DemandPairIntersections[key, MockCounter].Count(MockCounter) == 0)
                    {
                        DemandPairIntersections.Remove(key, MockCounter);
                        IntersectingDemandPairs[dp, MockCounter].Remove(intersectingDemandPair, MockCounter);
                        IntersectingDemandPairs[intersectingDemandPair, MockCounter].Remove(dp, MockCounter);
                        if (IntersectingDemandPairs[dp, MockCounter].Count(MockCounter) == 0)
                        {
                            IntersectingDemandPairs.Remove(dp, MockCounter);
                        }
                        if (IntersectingDemandPairs[intersectingDemandPair, MockCounter].Count(MockCounter) == 0)
                        {
                            IntersectingDemandPairs.Remove(intersectingDemandPair, MockCounter);
                        }
                    }
                }
            }

            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return CheckApplicability(pairsToCheck);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Executing {GetType().Name} rule for the first time");
#endif
            HasRun = true;
            LastContractedEdges.Clear(MockCounter);
            LastRemovedDemandPairs.Clear(MockCounter);
            LastChangedDemandPairs.Clear(MockCounter);
            FindInitialIntersections();
            Measurements.TimeSpentCheckingApplicability.Start();
            return CheckApplicability(DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter));
        }
    }
}
