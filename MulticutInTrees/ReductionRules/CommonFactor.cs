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
        private CountedDictionary<(DemandPair, DemandPair), CountedCollection<(TreeNode, TreeNode)>> DemandPairIntersections { get; }

        // todo; give each demand pair its own id
        /// <summary>
        /// <see cref="Dictionary{TKey, TValue}"/> with a unique identifier per <see cref="DemandPair"/>.
        /// </summary>
        private Dictionary<DemandPair, int> DemandPairIDs { get; }

        /// <summary>
        /// The maximum size the solution is allowed to have.
        /// </summary>
        private int MaxSolutionSize { get; }

        /// <summary>
        /// The part of the solution that has been found thus far.
        /// </summary>
        private List<(TreeNode, TreeNode)> PartialSolution { get; }

        // todo: give RR itself the mockcounter
        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not influence the performance of this <see cref="ReductionRule"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for <see cref="CommonFactor"/>.
        /// </summary>
        /// <param name="tree">The input tree.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>"
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public CommonFactor(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, List<(TreeNode, TreeNode)> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm)
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
            DemandPairIDs = new Dictionary<DemandPair, int>();
            PartialSolution = partialSolution;
            MaxSolutionSize = maxSolutionSize;
            IntersectingDemandPairs = new CountedDictionary<DemandPair, CountedCollection<DemandPair>>();
            DemandPairIntersections = new CountedDictionary<(DemandPair, DemandPair), CountedCollection<(TreeNode, TreeNode)>>();
            FillDemandPairIDs();
        }

        /// <summary>
        /// Fills <see cref="DemandPairIDs"/> with the unique identifier for each <see cref="DemandPair"/>.
        /// </summary>
        private void FillDemandPairIDs()
        {
            for (int i = 0; i < DemandPairs.Count(MockCounter); i++)
            {
                DemandPairIDs[DemandPairs[i, MockCounter]] = i;
            }
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
            IEnumerable<(TreeNode, TreeNode)> p0Path = p0.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter).Select(Utils.OrderEdgeSmallToLarge);
            int subsetFailCount = 0;
            HashSet<(DemandPair, DemandPair)> checkedPairs = new HashSet<(DemandPair, DemandPair)>();
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
                    if (!DemandPairIntersections.TryGetValue(key, out CountedCollection<(TreeNode, TreeNode)> intersection, Measurements.DemandPairsOperationsCounter))
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
        /// Uses <see cref="DemandPairIDs"/> to sort <paramref name="dp1"/> and <paramref name="dp2"/> in order to be used as key for <see cref="DemandPairIntersections"/>.
        /// </summary>
        /// <param name="dp1">The first <see cref="DemandPair"/>.</param>
        /// <param name="dp2">The second <see cref="DemandPair"/>.</param>
        /// <returns>A tuple with <paramref name="dp1"/> and <paramref name="dp2"/> sorted on their unique ID to be used as key for <see cref="DemandPairIntersections"/>.</returns>
        private (DemandPair, DemandPair) GetIntersectionKey(DemandPair dp1, DemandPair dp2)
        {
            return DemandPairIDs[dp1] < DemandPairIDs[dp2] ? (dp1, dp2) : (dp2, dp1);
        }

        /// <summary>
        /// Checks whether this <see cref="ReductionRule"/> is applicable on any of the <see cref="DemandPair"/>s in <paramref name="demandPairsToCheck"/>, and applies it.
        /// </summary>
        /// <param name="demandPairsToCheck">The <see cref="DemandPair"/>s we want to try this <see cref="ReductionRule"/> on.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/>, <see langword="false"/> otherwise.</returns>
        private bool CheckApplicability(IEnumerable<DemandPair> demandPairsToCheck)
        {
            List<DemandPair> pairsToBeRemoved = new List<DemandPair>();

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

        // todo: remove from RR, move this code to own method.
        /// <inheritdoc/>
        protected override void Preprocess()
        {
            int numberOfDPs = DemandPairs.Count(MockCounter);
            for (int i = 0; i < numberOfDPs - 1; i++)
            {
                DemandPair dp1 = DemandPairs[i, Measurements.DemandPairsOperationsCounter];
                IEnumerable<(TreeNode, TreeNode)> path1 = dp1.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter).Select(Utils.OrderEdgeSmallToLarge);
                for (int j = i + 1; j < numberOfDPs; j++)
                {
                    DemandPair dp2 = DemandPairs[j, Measurements.DemandPairsOperationsCounter];
                    IEnumerable<(TreeNode, TreeNode)> path2 = dp2.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter).Select(Utils.OrderEdgeSmallToLarge);
                    IEnumerable<(TreeNode, TreeNode)> intersection = path1.Intersect(path2);

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

                    CountedCollection<(TreeNode, TreeNode)> intersectionCollection = new CountedCollection<(TreeNode, TreeNode)>(intersection, MockCounter);
                    DemandPairIntersections[GetIntersectionKey(dp1, dp2), MockCounter] = intersectionCollection;
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdges"/>, <paramref name="removedDemandPairs"/> or <paramref name="changedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool RunLaterIteration(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdges, CountedList<DemandPair> removedDemandPairs, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdges, nameof(contractedEdges), $"Trying to apply the {GetType().Name} reduction rule after an edge was contracted, but the list with information about contracted edges is null!");
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), $"Trying to apply the {GetType().Name} reduction rule after a demand pair was removed, but the list with information about removed demand pairs is null!");
            Utils.NullCheck(changedDemandPairs, nameof(changedDemandPairs), $"Trying to apply the {GetType().Name} reduction rule after a demand pair was changed, but the list with information about changed demand pairs is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Executing {GetType().Name} rule in a later iterations");
#endif
            // TODO: WIP!! INTERSECTIONS STILL NEED TO BE UPDATED
            // TODO: WIP!! ALSO JUST THE IMPLEMENTATION

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Executing {GetType().Name} rule for the first time");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            return CheckApplicability(DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter));
        }
    }
}
