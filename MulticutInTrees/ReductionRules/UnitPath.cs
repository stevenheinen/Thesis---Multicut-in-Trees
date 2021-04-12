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
    /// <see cref="ReductionRule"/> that finds paths of <see cref="DemandPair"/>s of length one and cuts the corresponding edges.
    /// <br/>
    /// Rule: If a demand path has length one, cut its edge e.
    /// </summary>
    public class UnitPath : ReductionRule
    {
        /// <summary>
        /// Constructor for the <see cref="UnitPath"/> rule.
        /// </summary>
        /// <param name="tree">The <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="UnitPath"/> rule is part of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/> or <paramref name="algorithm"/> is <see langword="null"/>.</exception>
        public UnitPath(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} rule, but the algorithm it is part of is null!");
#endif
        }

        /// <summary>
        /// Returns whether a given <see cref="DemandPair"/> has a path of length one.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> for which we want to know if its path has length one.</param>
        /// <returns><see langword="true"/> if the path of <paramref name="demandPair"/> has length one, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/> is <see langword="null"/>.</exception>
        private bool DemandPathHasLengthOne(DemandPair demandPair)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to check whether a demand pair has a path of length 1, but the demand pair is null!");
#endif
            return demandPair.LengthOfPath(Measurements.DemandPairsOperationsCounter) == 1;
        }

        /// <summary>
        /// Try to apply this <see cref="ReductionRule"/> on the <see cref="DemandPair"/>s in <paramref name="demandPairsToCheck"/>.
        /// </summary>
        /// <param name="demandPairsToCheck">The <see cref="IEnumerable{T}"/> with edges we want to check.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/> successfully, <see langword="false"/> otherwise.</returns>
        protected bool TryApplyReductionRule(IEnumerable<DemandPair> demandPairsToCheck)
        {
            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach (DemandPair dp in demandPairsToCheck)
            {
                if (DemandPathHasLengthOne(dp))
                {
                    edgesToBeCut.Add(Utils.OrderEdgeSmallToLarge(dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First()));
                }
            }

            return TryCutEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeCut, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdges"/>, <paramref name="removedDemandPairs"/> or <paramref name="changedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool RunLaterIteration(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdges, CountedList<DemandPair> removedDemandPairs, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdges, nameof(contractedEdges), $"Trying to apply the {GetType().Name} rule after an edge was contracted, but the list of contracted edges is null!");
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), $"Trying to apply the {GetType().Name} rule after a demand path was removed, but the list of removed demand pairs is null!");
            Utils.NullCheck(changedDemandPairs, nameof(changedDemandPairs), $"Trying to apply the {GetType().Name} rule after a demand path was changed, but the list of changed demand pairs is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            
            HashSet<DemandPair> demandPairsToCheck = new HashSet<DemandPair>();

            foreach (((TreeNode, TreeNode) _, TreeNode _, CountedCollection<DemandPair> demandPairs) in contractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                foreach (DemandPair demandPair in demandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    demandPairsToCheck.Add(demandPair);
                }
            }

            foreach ((CountedList<(TreeNode, TreeNode)> _, DemandPair demandPair) in changedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                demandPairsToCheck.Add(demandPair);
            }

            contractedEdges.Clear(Measurements.TreeOperationsCounter);
            removedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            changedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return TryApplyReductionRule(demandPairsToCheck);
        }

        /*
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="changedEdgesPerDemandPairList"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathChanged(CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
#if !EXPERIMENT
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Unit Path rule after a demand path was changed, but the list of changed demand pairs is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Unit Path rule after a demand path was changed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach ((CountedList<(TreeNode, TreeNode)> _, DemandPair path) in changedEdgesPerDemandPairList.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                if (DemandPathHasLengthOne(path))
                {
                    edgesToBeCut.Add(Utils.OrderEdgeSmallToLarge(path.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First()));
                }
            }

            changedEdgesPerDemandPairList.Clear(Measurements.DemandPairsOperationsCounter);

            return TryCutEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeCut, Measurements.TreeOperationsCounter));
        }
        */

        /*
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Unit Path rule after a demand path was removed, but the list of removed demand pairs is null!");
#endif
            removedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            return false;
        }
        */

        /*
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdgeNodeTupleList"/> is <see langword="null"/>.</exception>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to apply the Unit Path rule after an edge was contracted, but the list of contracted edges is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Unit Path rule after an edge was contracted...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach (((TreeNode, TreeNode) _, TreeNode _, CountedCollection<DemandPair> pairs) in contractedEdgeNodeTupleList.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                foreach (DemandPair path in pairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    if (DemandPathHasLengthOne(path))
                    {
                        edgesToBeCut.Add(Utils.OrderEdgeSmallToLarge(path.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First()));
                    }
                }
            }

            contractedEdgeNodeTupleList.Clear(Measurements.TreeOperationsCounter);

            return TryCutEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeCut, Measurements.TreeOperationsCounter));
        }
        */

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule for the first time");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            return TryApplyReductionRule(DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter));
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {

        }
    }
}
