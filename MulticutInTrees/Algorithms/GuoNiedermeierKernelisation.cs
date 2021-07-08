// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Implementation of the kernelisation algorithm by Guo and Niedermeier.
    /// <br/>
    /// Source: <see href="https://doi.org/10.1002/net.20081"/>
    /// </summary>
    public class GuoNiedermeierKernelisation : Algorithm
    {
        /// <summary>
        /// Constructor for <see cref="GuoNiedermeierKernelisation"/>.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public GuoNiedermeierKernelisation(MulticutInstance instance) : this(instance, AlgorithmType.GuoNiedermeierKernelisation)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of the Guo-Niedermeier FPT algorithm, but the problem instance is null!");
#endif
            // Performs a call to the protected constructor.
        }

        /// <summary>
        /// Constructor for <see cref="GuoNiedermeierKernelisation"/> with an overwritten <see cref="AlgorithmType"/> for subclasses of this algorithm.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <param name="overwrittenAlgorithmType">The <see cref="AlgorithmType"/> of the current algorithm.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> or <paramref name="overwrittenAlgorithmType"/> is <see langword="null"/>.</exception>
        protected GuoNiedermeierKernelisation(MulticutInstance instance, AlgorithmType overwrittenAlgorithmType) : base(instance, overwrittenAlgorithmType)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of the Guo-Niedermeier FPT algorithm, but the problem instance is null!");
            Utilities.Utils.NullCheck(overwrittenAlgorithmType, nameof(overwrittenAlgorithmType), "Trying to create an instance of the Guo-Niedermeier FPT algorithm, but the AlgorithmType of the algorithm is null!");
#endif
        }

        /// <inheritdoc cref="Algorithm.CreateReductionRules"/>
        protected override void CreateReductionRules()
        {
            List<ReductionRule> reductionRules = new();

            IdleEdge idleEdge = new(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(idleEdge);

            UnitPath unitPath = new(Tree, DemandPairs, this);
            reductionRules.Add(unitPath);

            DominatedEdge dominatedEdge = new(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(dominatedEdge);

            DominatedPath dominatedPath = new(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(dominatedPath);

            DisjointPaths disjointPaths = new(Tree, DemandPairs, this, PartialSolution, K);
            reductionRules.Add(disjointPaths);

            OverloadedEdge overloadedEdge = new(Tree, DemandPairs, this, PartialSolution, K);
            reductionRules.Add(overloadedEdge);

            OverloadedCaterpillar overloadedCaterpillar = new(Tree, DemandPairs, this, DemandPairsPerNode, CaterpillarComponentPerNode, PartialSolution, K);
            reductionRules.Add(overloadedCaterpillar);

            OverloadedL3Leaves overloadedL3Leaves = new(Tree, DemandPairs, this, DemandPairsPerNode, PartialSolution, K);
            reductionRules.Add(overloadedL3Leaves);

            ReductionRules = reductionRules.AsReadOnly();
        }
    }
}
