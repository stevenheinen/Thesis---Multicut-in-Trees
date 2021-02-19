// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.Algorithms;
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
        /// <see cref="Dictionary{TKey, TValue}"/> containing a <see cref="List{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        private Dictionary<(TreeNode, TreeNode), List<DemandPair>> DemandPairsPerEdge { get; set; }

        /// <summary>
        /// Constructor for the <see cref="DominatedPath"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="List{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <param name="demandPairsPerEdge"><see cref="Dictionary{TKey, TValue}"/> containing a <see cref="List{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="random"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public DominatedPath(Tree<TreeNode> tree, List<DemandPair> demandPairs, Algorithm algorithm, Random random, Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm, random)
        {
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the dominated path reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the dominated path reduction rule, but the list with demand paths is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the dominated path reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(random, nameof(random), "Trying to create an instance of the dominated path reduction rule, but the random is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), "Trying to create an instance of the dominated path reduction rule, but the dictionary with demand pairs per edge is null!");

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
            Utils.NullCheck(subsetPair, nameof(subsetPair), "Trying to see if the path of a demand pair is contained in the path of another demand pair, but the first demand pair is null!");
            Utils.NullCheck(largerPair, nameof(largerPair), "Trying to see if the path of a demand pair is contained in the path of another demand pair, but the second demand pair is null!");

            return largerPair.EdgeIsPartOfPath(subsetPair.EdgesOnDemandPath[0]) && largerPair.EdgeIsPartOfPath(subsetPair.EdgesOnDemandPath[^1]);
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            return;
        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathChanged(IEnumerable<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Dominated Path reduction rule after a demand path was changed, but the IEnumerable with changed demand paths is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Dominated Path rule after a demand path changed...");
            }

            List<DemandPair> result = new List<DemandPair>();

            foreach ((List<(TreeNode, TreeNode)>, DemandPair) changedPath in changedEdgesPerDemandPairList)
            {
                if (result.Contains(changedPath.Item2))
                {
                    continue;
                }
                foreach (DemandPair otherDemandPair in DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(changedPath.Item2.EdgesOnDemandPath[0])])
                {
                    if (changedPath.Item2 == otherDemandPair || result.Contains(otherDemandPair))
                    {
                        continue;
                    }
                    if (PathIsContainedIn(changedPath.Item2, otherDemandPair))
                    {
                        result.Add(otherDemandPair);
                        break;
                    }
                }
                foreach (DemandPair otherDemandPair in DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(changedPath.Item2.EdgesOnDemandPath[^1])])
                {
                    if (changedPath.Item2 == otherDemandPair || result.Contains(otherDemandPair))
                    {
                        continue;
                    }
                    if (PathIsContainedIn(changedPath.Item2, otherDemandPair))
                    {
                        result.Add(otherDemandPair);
                        break;
                    }
                }
            }

            if (result.Count == 0)
            {
                return false;
            }

            Algorithm.RemoveDemandPairs(result);
            return true;
        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathRemove(IEnumerable<DemandPair> removedDemandPairs)
        {
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Dominated Path reduction rule after a demand path was removed, but the IEnumerable with removed demand paths is null!");

            return false;
        }

        /// <inheritdoc/>
        internal override bool AfterEdgeContraction(IEnumerable<((TreeNode, TreeNode), TreeNode, List<DemandPair>)> contractedEdgeNodeTupleList)
        {
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to apply the Dominated Path reduction rule after an edge was contracted, but the IEnumerable with contracted edges is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Dominated Path rule after an edge was contracted...");
            }

            HashSet<DemandPair> result = new HashSet<DemandPair>();

            foreach (((TreeNode, TreeNode), TreeNode, List<DemandPair>) contraction in contractedEdgeNodeTupleList) 
            {
                HashSet<DemandPair> otherDemandPairs = new HashSet<DemandPair>();
                foreach (TreeNode neighbour in contraction.Item2.Neighbours)
                {
                    if (!DemandPairsPerEdge.TryGetValue(Utils.OrderEdgeSmallToLarge((contraction.Item2, neighbour)), out List<DemandPair> pairsOnEdge))
                    {
                        continue;
                    }

                    foreach (DemandPair pair in pairsOnEdge)
                    {
                        otherDemandPairs.Add(pair);
                    }
                }
                foreach (DemandPair demandPair in contraction.Item3)
                {
                    if (result.Contains(demandPair))
                    {
                        continue;
                    }
                    foreach (DemandPair otherDemandPair in otherDemandPairs)
                    {
                        if (demandPair == otherDemandPair || result.Contains(otherDemandPair))
                        {
                            continue;
                        }
                        if (PathIsContainedIn(demandPair, otherDemandPair))
                        {
                            result.Add(otherDemandPair);
                        }
                    }
                }
            }

            if (result.Count == 0)
            {
                return false;
            }

            Algorithm.RemoveDemandPairs(result.ToList());
            return true;
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Dominated Path rule for the first time...");
            }

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

            if (pairsToBeRemoved.Count == 0)
            {
                return false;
            }

            Algorithm.RemoveDemandPairs(pairsToBeRemoved.ToList());
            return true;
        }
    }
}
