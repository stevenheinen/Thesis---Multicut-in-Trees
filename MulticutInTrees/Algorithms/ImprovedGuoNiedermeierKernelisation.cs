// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// <see cref="GuoNiedermeierKernelisation"/> kernelisation with improved reduction rules.
    /// </summary>
    public class ImprovedGuoNiedermeierKernelisation : GuoNiedermeierKernelisation
    {
        /// <summary>
        /// Constructor for <see cref="GuoNiedermeierKernelisation"/>.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public ImprovedGuoNiedermeierKernelisation(MulticutInstance instance) : base(instance, AlgorithmType.ImprovedGuoNiedermeierKernelisation)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of a the Guo-Niedermeier FPT algorithm, but the problem instance is null!");
#endif
            Preprocess();
        }

        /// <inheritdoc/>
        protected override ReadOnlyCollection<ReductionRule> CreateReductionRules()
        {
            List<ReductionRule> reductionRules = new List<ReductionRule>();

            IdleEdge idleEdge = new IdleEdge(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(idleEdge);

            UnitPath unitPath = new UnitPath(Tree, DemandPairs, this);
            reductionRules.Add(unitPath);

            ImprovedDominatedEdge improvedDominatedEdge = new ImprovedDominatedEdge(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(improvedDominatedEdge);

            DominatedPath dominatedPath = new DominatedPath(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(dominatedPath);

            DisjointPaths disjointPaths = new DisjointPaths(Tree, DemandPairs, this, PartialSolution, K);
            reductionRules.Add(disjointPaths);

            OverloadedEdge overloadedEdge = new OverloadedEdge(Tree, DemandPairs, this, PartialSolution, K, DemandPairsPerEdge);
            reductionRules.Add(overloadedEdge);

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(Tree, DemandPairs, this, DemandPairsPerNode, CaterpillarComponentPerNode, PartialSolution, K);
            reductionRules.Add(overloadedCaterpillar);

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(Tree, DemandPairs, this, DemandPairsPerNode, PartialSolution, K);
            reductionRules.Add(overloadedL3Leaves);

            return new ReadOnlyCollection<ReductionRule>(reductionRules);
        }
    }
}
