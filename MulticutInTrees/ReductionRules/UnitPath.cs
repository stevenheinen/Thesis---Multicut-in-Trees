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
    /// <see cref="ReductionRule"/> that finds paths of <see cref="DemandPair"/>s of length one and cuts the corresponding edges.
    /// <br/>
    /// Rule: If a demand path has length one, cut its edge e.
    /// </summary>
    public class UnitPath : ReductionRule
    {
        // todo: only necessary for updating the counters
        /// <summary>
        /// <see cref="Dictionary{TKey, TValue}"/> containing a <see cref="List{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> DemandPairsPerEdge { get; set; }

        /// <summary>
        /// Constructor for the <see cref="UnitPath"/> rule.
        /// </summary>
        /// <param name="tree">The <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="UnitPath"/> rule is part of.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, or <paramref name="random"/> is <see langword="null"/>.</exception>
        public UnitPath(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, Random random, CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm, random)
        {
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the Unit Path rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the Unit Path rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the Unit Path rule, but the algorithm it is part of is null!");
            Utils.NullCheck(random, nameof(random), "Trying to create an instance of the Unit Path rule, but the random is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), "Trying to create an instance of the Unit Path rule, but the dictionary with demand pairs per edge is null!");

            DemandPairsPerEdge = demandPairsPerEdge;
        }

        /// <summary>
        /// Returns whether a given <see cref="DemandPair"/> has a path of length one.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> for which we want to know if its path has length one.</param>
        /// <returns><see langword="true"/> if the path of <paramref name="demandPair"/> has length one, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/> is <see langword="null"/>.</exception>
        private bool DemandPathHasLengthOne(DemandPair demandPair)
        {
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to check whether a demand pair has a path of length 1, but the demand pair is null!");

            return demandPair.EdgesOnDemandPath.Count == 1;
        }

        /// <inheritdoc/>
        internal override void PrintCounters()
        {
            Console.WriteLine();
            Console.WriteLine($"Unit Path counters");
            Console.WriteLine("==============================");
            base.PrintCounters();
            Console.WriteLine();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="changedEdgesPerDemandPairList"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathChanged(CountedList<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Unit Path rule after a demand path was changed, but the list of changed demand pairs is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Unit Path rule after a demand path was changed...");
            }

            TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach ((List<(TreeNode, TreeNode)> _, DemandPair path) in changedEdgesPerDemandPairList.GetCountedEnumerable(new Counter()))
            {
                if (DemandPathHasLengthOne(path))
                {
                    edgesToBeCut.Add(path.EdgesOnDemandPath.First);
                }
            }

            TimeSpentCheckingApplicability.Stop();

            return TryCutEdges(edgesToBeCut);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Unit Path rule after a demand path was removed, but the list of removed demand pairs is null!");

            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdgeNodeTupleList"/> is <see langword="null"/>.</exception>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)> contractedEdgeNodeTupleList)
        {
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to apply the Unit Path rule after an edge was contracted, but the list of contracted edges is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Unit Path rule after an edge was contracted...");
            }

            TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach (((TreeNode, TreeNode) _, TreeNode _, CountedList<DemandPair> pairs) in contractedEdgeNodeTupleList.GetCountedEnumerable(new Counter()))
            {
                foreach (DemandPair path in pairs.GetCountedEnumerable(new Counter()))
                {
                    if (DemandPathHasLengthOne(path))
                    {
                        edgesToBeCut.Add(Utils.OrderEdgeSmallToLarge(path.EdgesOnDemandPath.First));
                    }
                }
            }

            TimeSpentCheckingApplicability.Stop();

            return TryCutEdges(edgesToBeCut);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine("Applying Unit Path rule for the first time...");
            }

            TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach (DemandPair dp in DemandPairs.GetCountedEnumerable(new Counter()))
            {
                if (DemandPathHasLengthOne(dp))
                {
                    edgesToBeCut.Add(dp.EdgesOnDemandPath.First);
                }
            }

            TimeSpentCheckingApplicability.Stop();

            return TryCutEdges(edgesToBeCut);
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            return;
        }

        /// <summary>
        /// Cut all edges in <paramref name="edgesToBeCut"/>.
        /// </summary>
        /// <param name="edgesToBeCut">The <see cref="HashSet{T}"/> with all edges to be cut.</param>
        /// <returns><see langword="true"/> if <paramref name="edgesToBeCut"/> has any elements, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edgesToBeCut"/> is <see langword="null"/>.</exception>
        private bool TryCutEdges(HashSet<(TreeNode, TreeNode)> edgesToBeCut)
        {
            Utils.NullCheck(edgesToBeCut, nameof(edgesToBeCut), $"Trying to cut edges, but the Hashset with edges is null!");

            if (edgesToBeCut.Count == 0)
            {
                return false;
            }

            // todo: only necessary for the counters, should not update the number of operations... Maybe move somewhere else?
            HashSet<DemandPair> removedDemandPairs = new HashSet<DemandPair>();
            foreach ((TreeNode, TreeNode) edge in edgesToBeCut)
            {
                foreach (DemandPair demandPair in DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(edge)].GetCountedEnumerable(new Counter()))
                {
                    removedDemandPairs.Add(demandPair);
                }
            }

            RemovedDemandPairsCounter += removedDemandPairs.Count;
            ContractedEdgesCounter += edgesToBeCut.Count();

            Algorithm.CutEdges(edgesToBeCut.ToList());
            return true;
        }
    }
}
