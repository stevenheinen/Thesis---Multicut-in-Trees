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
    /// Implementation of the Disjoint Paths reduction rule.
    /// <br/>
    /// <b>Rule:</b> If there are more than k pairwise edge-disjoint demand paths, then there is no solution with parameter value k.
    /// </summary>
    public class DisjointPaths : ReductionRule
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
        /// Constructor for the <see cref="DisjointPaths"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public DisjointPaths(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, List<Edge<Node>> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm, true)
        {
#if !Experiment
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(partialSolution, nameof(partialSolution), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with the partial solution is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException($"Trying to create an instance of the {GetType().Name} reduction rule, but the maximum solution size parameter is smaller than zero!");
            }
#endif
            PartialSolution = partialSolution;
            MaxSolutionSize = maxSolutionSize;
        }

        /// <summary>
        /// Returns whether the current number of edge-disjoint demand pairs in the instance is larger than what can be added to the solution.
        /// </summary>
        /// <returns><see langword="true"/> if the current number of edge-disjoint demand pairs in the instance is larger than the amount of edges that can still be added to the solution, <see langword="false"/> otherwise.</returns>
        private bool DisjointPathsGreaterThanK()
        {
            IEnumerable<(Node, Node)> commodities = DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).Select(dp => (dp.Node1, dp.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowGraph<Graph, Edge<Node>, Node>(Tree, commodities, Measurements);
            Measurements.TimeSpentCheckingApplicability.Stop();
            return flow > MaxSolutionSize - PartialSolution.Count;
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Executing {GetType().Name} rule for the first time");
#endif
            HasRun = true;
            Measurements.TimeSpentCheckingApplicability.Start();
            return DisjointPathsGreaterThanK();
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Executing {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return DisjointPathsGreaterThanK();
        }
    }
}
