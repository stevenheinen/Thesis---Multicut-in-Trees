// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Implementation of the FPT algorithm by Bousquet et al.
    /// <br/>
    /// Source: <see href="https://arxiv.org/abs/0902.1047"/>
    /// </summary>
    public class BousquetKernelisation : Algorithm
    {
        /// <summary>
        /// Constructor for the <see cref="BousquetKernelisation"/> algorithm.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> the algorithm should run on.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public BousquetKernelisation(MulticutInstance instance) : this(instance, AlgorithmType.BousquetKernelisation)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of the Bousquet et al. kernelisation algorithm, but the problem instance is null!");
#endif
            // Calls the internal constructor with the correct parameterss
        }

        /// <summary>
        /// Constructor for <see cref="BousquetKernelisation"/> with an overwritten <see cref="AlgorithmType"/> for subclasses of this algorithm.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <param name="overwrittenAlgorithmType">The <see cref="AlgorithmType"/> of the current algorithm.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> or <paramref name="overwrittenAlgorithmType"/> is <see langword="null"/>.</exception>
        internal BousquetKernelisation(MulticutInstance instance, AlgorithmType overwrittenAlgorithmType) : base(instance, overwrittenAlgorithmType)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of the Bousquet et al. kernelisation algorithm, but the problem instance is null!");
            Utilities.Utils.NullCheck(overwrittenAlgorithmType, nameof(overwrittenAlgorithmType), "Trying to create an instance of the Bousquet et al. kernelisation algorithm, but the AlgorithmType of the algorithm is null!");
#endif
        }

        /// <inheritdoc cref="Algorithm.CreateReductionRules"/>
        protected override void CreateReductionRules()
        {
            List<ReductionRule> reductionRules = new();

            UnitPath unitPath = new(Tree, DemandPairs, this);
            reductionRules.Add(unitPath);

            DisjointPaths disjointPaths = new(Tree, DemandPairs, this, PartialSolution, K);
            reductionRules.Add(disjointPaths);
            
            UniqueDirection uniqueDirection = new(Tree, DemandPairs, this, DemandPairsPerNode, DemandPairsPerEdge, false);
            reductionRules.Add(uniqueDirection);

            DominatedPath dominatedPath = new(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(dominatedPath);

            CommonFactor commonFactor = new(Tree, DemandPairs, this, PartialSolution, K);
            reductionRules.Add(commonFactor);

            // todo: bidimensional dominating wingspan reduction rule

            // todo: generalised dominating wingspan reduction rule

            ReductionRules = reductionRules.AsReadOnly();
        }
    }
}
