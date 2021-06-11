// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Equal to <see cref="GuoNiedermeierKernelisation"/>, except that rules <see cref="DominatedEdge"/> and <see cref="DominatedPath"/> are swapped.
    /// </summary>
    public class GuoNiedermeierKernelisationSwap34 : GuoNiedermeierKernelisation
    {
        /// <summary>
        /// Constructor for <see cref="GuoNiedermeierKernelisation"/>.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public GuoNiedermeierKernelisationSwap34(MulticutInstance instance) : base(instance, AlgorithmType.GuoNiedermeierKernelisationSwap34)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of a the Guo-Niedermeier FPT algorithm, but the problem instance is null!");
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

            DominatedPath dominatedPath = new(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(dominatedPath);

            DominatedEdge dominatedEdge = new(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(dominatedEdge);

            DisjointPaths disjointPaths = new(Tree, DemandPairs, this, PartialSolution, K);
            reductionRules.Add(disjointPaths);

            OverloadedEdge overloadedEdge = new(Tree, DemandPairs, this, PartialSolution, K, DemandPairsPerEdge);
            reductionRules.Add(overloadedEdge);

            OverloadedCaterpillar overloadedCaterpillar = new(Tree, DemandPairs, this, DemandPairsPerNode, CaterpillarComponentPerNode, PartialSolution, K);
            reductionRules.Add(overloadedCaterpillar);

            OverloadedL3Leaves overloadedL3Leaves = new(Tree, DemandPairs, this, DemandPairsPerNode, PartialSolution, K);
            reductionRules.Add(overloadedL3Leaves);

            ReductionRules = new ReadOnlyCollection<ReductionRule>(reductionRules);
        }
    }
}
