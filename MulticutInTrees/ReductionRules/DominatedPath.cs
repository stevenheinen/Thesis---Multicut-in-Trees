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
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for the <see cref="DominatedPath"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        /// <param name="demandPairsPerEdge"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public DominatedPath(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the dominated path reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the dominated path reduction rule, but the list with demand paths is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the dominated path reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), "Trying to create an instance of the dominated path reduction rule, but the dictionary with demand pairs per edge is null!");
#endif
            DemandPairsPerEdge = demandPairsPerEdge;
        }

        /// <summary>
        /// Checks whether the path of <paramref name="subsetPair"/> is contained in the path of <paramref name="largerPair"/>.
        /// </summary>
        /// <param name="subsetPair">The smaller <see cref="DemandPair"/>.</param>
        /// <param name="largerPair">The larger <see cref="DemandPair"/>.</param>
        /// <returns><see langword="true"/> if the path of <paramref name="subsetPair"/> is a subset of the path of <paramref name="largerPair"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="subsetPair"/> or <paramref name="largerPair"/> is <see langword="null"/>.</exception>
        private bool PathIsContainedIn(DemandPair subsetPair, DemandPair largerPair)
        {
#if !EXPERIMENT
            Utils.NullCheck(subsetPair, nameof(subsetPair), "Trying to see if the path of a demand pair is contained in the path of another demand pair, but the first demand pair is null!");
            Utils.NullCheck(largerPair, nameof(largerPair), "Trying to see if the path of a demand pair is contained in the path of another demand pair, but the second demand pair is null!");
#endif
            return largerPair.EdgeIsPartOfPath(subsetPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First(), Measurements.DemandPairsOperationsCounter) && largerPair.EdgeIsPartOfPath(subsetPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Last(), Measurements.DemandPairsOperationsCounter);
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {

        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathChanged(CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
#if !EXPERIMENT
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Dominated Path reduction rule after a demand path was changed, but the IEnumerable with changed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Dominated Path rule after a demand path changed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> pairsToBeRemoved = new HashSet<DemandPair>();

            foreach ((CountedList<(TreeNode, TreeNode)>, DemandPair) changedPath in changedEdgesPerDemandPairList.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                if (pairsToBeRemoved.Contains(changedPath.Item2))
                {
                    continue;
                }
                foreach (DemandPair otherDemandPair in DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(changedPath.Item2.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First()), Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                {
                    if (changedPath.Item2 == otherDemandPair || pairsToBeRemoved.Contains(otherDemandPair))
                    {
                        continue;
                    }
                    if (PathIsContainedIn(changedPath.Item2, otherDemandPair))
                    {
                        pairsToBeRemoved.Add(otherDemandPair);
                        break;
                    }
                }
                foreach (DemandPair otherDemandPair in DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(changedPath.Item2.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Last()), Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                {
                    if (changedPath.Item2 == otherDemandPair || pairsToBeRemoved.Contains(otherDemandPair))
                    {
                        continue;
                    }
                    if (PathIsContainedIn(changedPath.Item2, otherDemandPair))
                    {
                        pairsToBeRemoved.Add(otherDemandPair);
                        break;
                    }
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();

            return TryRemoveDemandPairs(new CountedList<DemandPair>(pairsToBeRemoved, Measurements.DemandPairsOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Dominated Path reduction rule after a demand path was removed, but the IEnumerable with removed demand paths is null!");
#endif
            return false;
        }

        /// <inheritdoc/>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to apply the Dominated Path reduction rule after an edge was contracted, but the IEnumerable with contracted edges is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Dominated Path rule after an edge was contracted...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> pairsToBeRemoved = new HashSet<DemandPair>();

            foreach (((TreeNode, TreeNode) _, TreeNode newNode, CountedCollection<DemandPair> affectedDemandPairs) in contractedEdgeNodeTupleList.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                HashSet<DemandPair> otherDemandPairs = new HashSet<DemandPair>();
                foreach (TreeNode neighbour in newNode.Neighbours(Measurements.TreeOperationsCounter))
                {
                    if (!DemandPairsPerEdge.TryGetValue(Utils.OrderEdgeSmallToLarge((newNode, neighbour)), out CountedCollection<DemandPair> pairsOnEdge, Measurements.DemandPairsPerEdgeKeysCounter))
                    {
                        continue;
                    }

                    foreach (DemandPair pair in pairsOnEdge.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                    {
                        otherDemandPairs.Add(pair);
                    }
                }
                foreach (DemandPair demandPair in affectedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    if (pairsToBeRemoved.Contains(demandPair))
                    {
                        continue;
                    }
                    foreach (DemandPair otherDemandPair in otherDemandPairs)
                    {
                        if (demandPair == otherDemandPair || pairsToBeRemoved.Contains(otherDemandPair))
                        {
                            continue;
                        }
                        if (PathIsContainedIn(demandPair, otherDemandPair))
                        {
                            pairsToBeRemoved.Add(otherDemandPair);
                        }
                    }
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();

            return TryRemoveDemandPairs(new CountedList<DemandPair>(pairsToBeRemoved, Measurements.DemandPairsOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine("Applying Dominated Path rule for the first time...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> pairsToBeRemoved = new HashSet<DemandPair>();
            int nrPairs = DemandPairs.Count(Measurements.DemandPairsOperationsCounter);
            for (int i = 0; i < nrPairs - 1; i++)
            {
                if (pairsToBeRemoved.Contains(DemandPairs[i, Measurements.DemandPairsOperationsCounter]))
                {
                    continue;
                }
                for (int j = i + 1; j < nrPairs; j++)
                {
                    if (pairsToBeRemoved.Contains(DemandPairs[j, Measurements.DemandPairsOperationsCounter]))
                    {
                        continue;
                    }

                    if (PathIsContainedIn(DemandPairs[j, Measurements.DemandPairsOperationsCounter], DemandPairs[i, Measurements.DemandPairsOperationsCounter]))
                    {
                        pairsToBeRemoved.Add(DemandPairs[i, Measurements.DemandPairsOperationsCounter]);
                        break;
                    }
                    else if (PathIsContainedIn(DemandPairs[i, Measurements.DemandPairsOperationsCounter], DemandPairs[j, Measurements.DemandPairsOperationsCounter]))
                    {
                        pairsToBeRemoved.Add(DemandPairs[j, Measurements.DemandPairsOperationsCounter]);
                    }
                }
            }

            return TryRemoveDemandPairs(new CountedList<DemandPair>(pairsToBeRemoved, Measurements.DemandPairsOperationsCounter));
        }
    }
}
