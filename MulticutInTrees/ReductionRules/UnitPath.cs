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
        /// <param name="tree">The <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="UnitPath"/> rule is part of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/> or <paramref name="algorithm"/> is <see langword="null"/>.</exception>
        public UnitPath(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm) : base(tree, demandPairs, algorithm)
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
        private bool TryApplyReductionRule(IEnumerable<DemandPair> demandPairsToCheck)
        {
            HashSet<Edge<Node>> edgesToBeCut = new();
            foreach (DemandPair dp in demandPairsToCheck)
            {
                if (DemandPathHasLengthOne(dp))
                {
                    edgesToBeCut.Add(dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First());
                }
            }

            return TryCutEdges(new CountedList<Edge<Node>>(edgesToBeCut, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<DemandPair> demandPairsToCheck = new();

            foreach ((Edge<Node> _, Node _, CountedCollection<DemandPair> demandPairs) in LastContractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                foreach (DemandPair demandPair in demandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
                {
                    demandPairsToCheck.Add(demandPair);
                }
            }

            foreach ((CountedList<Edge<Node>> _, DemandPair demandPair) in LastChangedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
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
            LastContractedEdges.Clear(MockCounter);
            LastRemovedDemandPairs.Clear(MockCounter);
            LastChangedDemandPairs.Clear(MockCounter);
            Measurements.TimeSpentCheckingApplicability.Start();
            return TryApplyReductionRule(DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter));
        }
    }
}
