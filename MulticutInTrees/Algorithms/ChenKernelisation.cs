// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Implementation of the kernelisation algorithm by Chen et al.
    /// <br/>
    /// Source: <see href="https://doi.org/10.1016/j.jcss.2012.03.001"/>
    /// </summary>
    public class ChenKernelisation : Algorithm
    {
        /// <summary>
        /// Constructor for <see cref="GuoNiedermeierKernelisation"/>.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public ChenKernelisation(MulticutInstance instance) : this(instance, AlgorithmType.ChenKernelisation)
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
        protected ChenKernelisation(MulticutInstance instance, AlgorithmType overwrittenAlgorithmType) : base(instance, overwrittenAlgorithmType)
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

            DisjointPaths disjointPaths = new(Tree, DemandPairs, this, PartialSolution, K);
            reductionRules.Add(disjointPaths);

            UniqueDirection uniqueDirection = new(Tree, DemandPairs, this, DemandPairsPerNode, DemandPairsPerEdge, true);
            reductionRules.Add(uniqueDirection);

            DominatedPath dominatedPath = new(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(dominatedPath);

            // todo: Bound on good leaves

            // todo: Crown reduction

            // todo: Bound on the number of good leaves in a group that form a demand pair with a certain vertex

            // todo: Bound on the number of bad leaves in a group that form a demand pair with internal vertices in another group

            // todo: Bound on the number of bad leaves in a group that form a demand pair with bad leaves in another group

            // todo: Bound on the number of bad leaves in a group that form a demand pair with good leaves in OUT_u for a type-I group γ_i formed by vertex u.

            ReductionRules = reductionRules.AsReadOnly();
        }
    }
}
