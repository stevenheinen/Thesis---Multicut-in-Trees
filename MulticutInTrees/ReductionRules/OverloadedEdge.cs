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
        private List<(TreeNode, TreeNode)> PartialSolution { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for things that should not impact performance.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for the <see cref="OverloadedEdge"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input tree.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <param name="demandPairsPerEdge"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="partialSolution"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public OverloadedEdge(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, List<(TreeNode, TreeNode)> partialSolution, int maxSolutionSize, CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the Overloaded Edge reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the Overloaded Edge reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the Overloaded Edge reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(partialSolution, nameof(partialSolution), "Trying to create an instance of the Overloaded Edge reduction rule, but the partial solution is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), "Trying to create an instance of the Overloaded Edge reduction rule, but the dictionary with demand pairs per edge is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSolutionSize), "Trying to create an instance of the Overloaded Edge reduction rule, but the maximum number of edges that can be cut is smaller than zero!");
            }
#endif
            MaxSolutionSize = maxSolutionSize;
            PartialSolution = partialSolution;
            DemandPairsPerEdge = demandPairsPerEdge;
            MockCounter = new Counter();
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            
        }

        /// <summary>
        /// Checks if a given edge is overloaded.
        /// </summary>
        /// <param name="edge">The edge to be checked.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> has more than k length-2 demand paths passing through it, <see langword="false"/> otherwise.</returns>
        private bool CheckEdgeForOverload((TreeNode, TreeNode) edge)
        {
            return DemandPairsPerEdge[edge, Measurements.DemandPairsPerEdgeKeysCounter].Count(dp => dp.LengthOfPath(Measurements.DemandPairsOperationsCounter) == 2, Measurements.DemandPairsPerEdgeValuesCounter) > MaxSolutionSize - PartialSolution.Count;
        }

        /// <summary>
        /// Checks for a set of changed(!) <see cref="DemandPair"/>s whether one of their edges is now overloaded.
        /// </summary>
        /// <param name="demandPairs">The changed <see cref="DemandPair"/>s.</param>
        /// <returns>A <see cref="HashSet{T}"/> of overloaded edges.</returns>
        private HashSet<(TreeNode, TreeNode)> CheckForOverloadedEdges(IEnumerable<DemandPair> demandPairs)
        {
            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach (DemandPair demandPair in demandPairs)
            {
                if (demandPair.LengthOfPath(Measurements.DemandPairsOperationsCounter) != 2)
                {
                    continue;
                }
                (TreeNode, TreeNode) edge1 = Utils.OrderEdgeSmallToLarge(demandPair.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter).First());
                (TreeNode, TreeNode) edge2 = Utils.OrderEdgeSmallToLarge(demandPair.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter).Last());
                if (CheckEdgeForOverload(edge1))
                {
                    edgesToBeCut.Add(edge1);
                }
                if (CheckEdgeForOverload(edge2))
                {
                    edgesToBeCut.Add(edge2);
                }
            }
            return edgesToBeCut;
        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathChanged(CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
#if !EXPERIMENT
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to execute the Overloaded Edge rule after a demand pair was changed, but the list with changed demand pairs is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Overloaded Edge rule after a demand path was changed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            HashSet<(TreeNode, TreeNode)> edgesToBeCut = CheckForOverloadedEdges(changedEdgesPerDemandPairList.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).Select(i => i.Item2));
            return TryCutEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeCut, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to execute the Overloaded Edge rule after a demand pair was removed, but the list with removed demand pairs is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Overloaded Edge rule after a demand path was removed...");
#endif
            return false;
        }

        /// <inheritdoc/>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to execute the Overloaded Edge rule after an edge was contracted, but the list with contracted edges is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Overloaded Edge rule after an edge was contracted...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            HashSet<(TreeNode, TreeNode)> edgesToBeCut = new HashSet<(TreeNode, TreeNode)>();
            foreach (((TreeNode, TreeNode) _, TreeNode _, CountedCollection<DemandPair> demandPairs) in contractedEdgeNodeTupleList.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                foreach ((TreeNode, TreeNode) overloadedEdge in CheckForOverloadedEdges(demandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter)))
                {
                    edgesToBeCut.Add(overloadedEdge);
                }
            }
            return TryCutEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeCut, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine("Applying Overloaded Edge rule for the first time...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            CountedDictionary<(TreeNode, TreeNode), int> edgeOccurrences = new CountedDictionary<(TreeNode, TreeNode), int>();
            foreach (DemandPair demandPair in DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                if (demandPair.LengthOfPath(Measurements.DemandPairsOperationsCounter) != 2)
                {
                    continue;
                }

                (TreeNode, TreeNode) edge1 = Utils.OrderEdgeSmallToLarge(demandPair.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter).First());
                (TreeNode, TreeNode) edge2 = Utils.OrderEdgeSmallToLarge(demandPair.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter).Last());
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

            CountedList<(TreeNode, TreeNode)> edgesToBeCut = DetermineOverloadedEdges(edgeOccurrences);
            return TryCutEdges(edgesToBeCut);
        }

        /// <summary>
        /// Checks, given a <see cref="Dictionary{TKey, TValue}"/> with per edge how many length-2 <see cref="DemandPair"/>s pass through it, which edges are overloaded.
        /// </summary>
        /// <param name="edgeOccurrences"><see cref="Dictionary{TKey, TValue}"/> with the number of length-2 <see cref="DemandPair"/>s per edge.</param>
        /// <returns>A <see cref="CountedList{T}"/> of edges that can be cut.</returns>
        private CountedList<(TreeNode, TreeNode)> DetermineOverloadedEdges(CountedDictionary<(TreeNode, TreeNode), int> edgeOccurrences)
        {
            CountedList<(TreeNode, TreeNode)> overloadedEdges = new CountedList<(TreeNode, TreeNode)>();
            int threshold = MaxSolutionSize - PartialSolution.Count;
            foreach (KeyValuePair<(TreeNode, TreeNode), int> edge in edgeOccurrences.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                if (edge.Value > threshold)
                {
                    overloadedEdges.Add(edge.Key, Measurements.TreeOperationsCounter);
                }
            }
            return overloadedEdges;
        }
    }
}
