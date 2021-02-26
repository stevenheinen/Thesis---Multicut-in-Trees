// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
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
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="List{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> DemandPairsPerEdge { get; set; }

        /// <summary>
        /// Constructor for the <see cref="DominatedPath"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <param name="demandPairsPerEdge"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="List{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="random"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public DominatedPath(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, Random random, CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm, random)
        {
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the dominated path reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the dominated path reduction rule, but the list with demand paths is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the dominated path reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(random, nameof(random), "Trying to create an instance of the dominated path reduction rule, but the random is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), "Trying to create an instance of the dominated path reduction rule, but the dictionary with demand pairs per edge is null!");

            DemandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>(demandPairsPerEdge);
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
            Utils.NullCheck(subsetPair, nameof(subsetPair), "Trying to see if the path of a demand pair is contained in the path of another demand pair, but the first demand pair is null!");
            Utils.NullCheck(largerPair, nameof(largerPair), "Trying to see if the path of a demand pair is contained in the path of another demand pair, but the second demand pair is null!");

            return largerPair.EdgeIsPartOfPath(subsetPair.EdgesOnDemandPath.First) && largerPair.EdgeIsPartOfPath(subsetPair.EdgesOnDemandPath.Last);
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            return;
        }

        /// <inheritdoc/>
        internal override void PrintCounters()
        {
            Console.WriteLine();
            Console.WriteLine($"Dominated Path counters");
            Console.WriteLine("==============================");
            Console.WriteLine($"DemandPairsPerEdge: {DemandPairsPerEdge.OperationsCounter}");
            base.PrintCounters();
            Console.WriteLine();
        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathChanged(CountedList<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Dominated Path reduction rule after a demand path was changed, but the IEnumerable with changed demand paths is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Dominated Path rule after a demand path changed...");
            }

            TimeSpentCheckingApplicability.Start();

            List<DemandPair> pairsToBeRemoved = new List<DemandPair>();

            foreach ((List<(TreeNode, TreeNode)>, DemandPair) changedPath in changedEdgesPerDemandPairList.GetCountedEnumerable(new Counter()))
            {
                if (pairsToBeRemoved.Contains(changedPath.Item2))
                {
                    continue;
                }
                foreach (DemandPair otherDemandPair in DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(changedPath.Item2.EdgesOnDemandPath.First)].GetCountedEnumerable(new Counter()))
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
                foreach (DemandPair otherDemandPair in DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(changedPath.Item2.EdgesOnDemandPath.Last)].GetCountedEnumerable(new Counter()))
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

            TimeSpentCheckingApplicability.Stop();

            return TryRemoveDemandPairs(pairsToBeRemoved);
        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Dominated Path reduction rule after a demand path was removed, but the IEnumerable with removed demand paths is null!");

            return false;
        }

        /// <inheritdoc/>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)> contractedEdgeNodeTupleList)
        {
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to apply the Dominated Path reduction rule after an edge was contracted, but the IEnumerable with contracted edges is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Dominated Path rule after an edge was contracted...");
            }

            TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> pairsToBeRemoved = new HashSet<DemandPair>();

            foreach (((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>) contraction in contractedEdgeNodeTupleList.GetCountedEnumerable(new Counter())) 
            {
                HashSet<DemandPair> otherDemandPairs = new HashSet<DemandPair>();
                foreach (TreeNode neighbour in contraction.Item2.Neighbours)
                {
                    if (!DemandPairsPerEdge.TryGetValue(Utils.OrderEdgeSmallToLarge((contraction.Item2, neighbour)), out CountedList<DemandPair> pairsOnEdge))
                    {
                        continue;
                    }

                    foreach (DemandPair pair in pairsOnEdge.GetCountedEnumerable(new Counter()))
                    {
                        otherDemandPairs.Add(pair);
                    }
                }
                foreach (DemandPair demandPair in contraction.Item3.GetCountedEnumerable(new Counter()))
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

            TimeSpentCheckingApplicability.Stop();

            return TryRemoveDemandPairs(pairsToBeRemoved.ToList());
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Dominated Path rule for the first time...");
            }

            TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> pairsToBeRemoved = new HashSet<DemandPair>();
            for (int i = 0; i < DemandPairs.Count - 1; i++)
            {
                if (pairsToBeRemoved.Contains(DemandPairs[i]))
                {
                    continue;
                }
                for (int j = i + 1; j < DemandPairs.Count; j++)
                {
                    if (pairsToBeRemoved.Contains(DemandPairs[j]))
                    {
                        continue;
                    }

                    if (PathIsContainedIn(DemandPairs[j], DemandPairs[i]))
                    {
                        pairsToBeRemoved.Add(DemandPairs[i]);
                        break;
                    }
                    else if (PathIsContainedIn(DemandPairs[i], DemandPairs[j]))
                    {
                        pairsToBeRemoved.Add(DemandPairs[j]);
                    }
                }
            }

            TimeSpentCheckingApplicability.Stop();

            return TryRemoveDemandPairs(pairsToBeRemoved.ToList());
        }

        /// <summary>
        /// Remove all <see cref="DemandPair"/>s in <paramref name="pairsToBeRemoved"/>.
        /// </summary>
        /// <param name="pairsToBeRemoved">The <see cref="List{T}"/> with all <see cref="DemandPair"/>s to be removed.</param>
        /// <returns><see langword="true"/> if <paramref name="pairsToBeRemoved"/> has any elements, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pairsToBeRemoved"/> is <see langword="null"/>.</exception>
        private bool TryRemoveDemandPairs(List<DemandPair> pairsToBeRemoved)
        {
            Utils.NullCheck(pairsToBeRemoved, nameof(pairsToBeRemoved), $"Trying to remove demand pairs, but the List with demand pairs is null!");

            if (pairsToBeRemoved.Count == 0)
            {
                return false;
            }

            RemovedDemandPairsCounter += pairsToBeRemoved.Count;
            Algorithm.RemoveDemandPairs(pairsToBeRemoved);
            return true;
        }
    }
}
