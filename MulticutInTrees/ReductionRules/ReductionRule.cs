// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// Abstract class to be used as baseclass for each reduction rule.
    /// </summary>
    public abstract class ReductionRule
    {
        /// <summary>
        /// The input <see cref="Tree{N}"/>.
        /// </summary>
        protected Tree<TreeNode> Tree { get; }

        /// <summary>
        /// The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.
        /// </summary>
        protected CountedList<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.
        /// </summary>
        protected Algorithm Algorithm { get; }

        /// <summary>
        /// The <see cref="PerformanceMeasurements"/> used to measure the performance of this <see cref="ReductionRule"/>.
        /// </summary>
        public PerformanceMeasurements Measurements { get; }

        /// <summary>
        /// If a <see cref="ReductionRule"/> returns <see langword="true"/> when it is checked for applicability, and this value is also <see langword="true"/>, we have an instance that cannot be solved.
        /// </summary>
        public bool TrueMeansInfeasibleInstance { get; }

        /// <summary>
        /// Constructor for a <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithms.Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        /// <param name="trueMeansInfeasibleInstance">If a <see cref="ReductionRule"/> returns <see langword="true"/> when it is checked for applicability, and this value is also <see langword="true"/>, we have an instance that cannot be solved. When a <see cref="ReductionRule"/> cannot determine that an instance is infeasible, pass <see langword="false"/> to this constructor.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/> or <paramref name="algorithm"/> is <see langword="null"/>.</exception>
        protected ReductionRule(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, bool trueMeansInfeasibleInstance = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(tree, nameof(tree), "Trying to create a reduction rule, but the input tree is null!");
            Utilities.Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create a reduction rule, but the list of demand pairs is null!");
            Utilities.Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create a reduction rule, but the algorithm it is part of is null!");
#endif
            Tree = tree;
            DemandPairs = demandPairs;
            Algorithm = algorithm;
            Measurements = new PerformanceMeasurements(GetType().Name);
            TrueMeansInfeasibleInstance = trueMeansInfeasibleInstance;

            Preprocess();
        }

        /// <summary>
        /// Cut all edges in <paramref name="edgesToBeCut"/>.
        /// </summary>
        /// <param name="edgesToBeCut">The <see cref="CountedList{T}"/> with all edges to be cut.</param>
        /// <returns><see langword="true"/> if <paramref name="edgesToBeCut"/> has any elements, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edgesToBeCut"/> is <see langword="null"/>.</exception>
        protected bool TryCutEdges(CountedList<(TreeNode, TreeNode)> edgesToBeCut)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(edgesToBeCut, nameof(edgesToBeCut), "Trying to cut edges, but the Hashset with edges is null!");
#endif
            if (edgesToBeCut.Count(Measurements.TreeOperationsCounter) == 0)
            {
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }

            Measurements.TimeSpentCheckingApplicability.Stop();
            Measurements.TimeSpentModifyingInstance.Start();
            Algorithm.CutEdges(edgesToBeCut, Measurements);
            Measurements.TimeSpentModifyingInstance.Stop();
            return true;
        }

        /// <summary>
        /// Remove all <see cref="DemandPair"/>s in <paramref name="pairsToBeRemoved"/>.
        /// </summary>
        /// <param name="pairsToBeRemoved">The <see cref="CountedList{T}"/> with all <see cref="DemandPair"/>s to be removed.</param>
        /// <returns><see langword="true"/> if <paramref name="pairsToBeRemoved"/> has any elements, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pairsToBeRemoved"/> is <see langword="null"/>.</exception>
        protected bool TryRemoveDemandPairs(CountedList<DemandPair> pairsToBeRemoved)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(pairsToBeRemoved, nameof(pairsToBeRemoved), "Trying to remove demand pairs, but the List with demand pairs is null!");
#endif
            if (pairsToBeRemoved.Count(Measurements.DemandPairsOperationsCounter) == 0)
            {
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }

            Measurements.TimeSpentCheckingApplicability.Stop();
            Measurements.TimeSpentModifyingInstance.Start();
            Algorithm.RemoveDemandPairs(pairsToBeRemoved, Measurements);
            Measurements.TimeSpentModifyingInstance.Stop();
            return true;
        }

        /// <summary>
        /// Contract all edges in <paramref name="edgesToBeContracted"/>.
        /// </summary>
        /// <param name="edgesToBeContracted">The <see cref="CountedList{T}"/> with all edges to be contracted.</param>
        /// <returns><see langword="true"/> if <paramref name="edgesToBeContracted"/> has any elements, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edgesToBeContracted"/> is <see langword="null"/>.</exception>
        protected bool TryContractEdges(CountedList<(TreeNode, TreeNode)> edgesToBeContracted)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(edgesToBeContracted, nameof(edgesToBeContracted), "Trying to contract edges, but the List with edges is null!");
#endif
            if (edgesToBeContracted.Count(Measurements.TreeOperationsCounter) == 0)
            {
                Measurements.TimeSpentCheckingApplicability.Stop();
                return false;
            }

            Measurements.TimeSpentCheckingApplicability.Stop();
            Measurements.TimeSpentModifyingInstance.Start();
            Algorithm.ContractEdges(edgesToBeContracted, Measurements);
            Measurements.TimeSpentModifyingInstance.Start();
            return true;
        }

        /// <summary>
        /// Everything that needs to happen before the reduction rule can be executed.
        /// </summary>
        protected abstract void Preprocess();

        /// <summary>
        /// First iteration of this <see cref="ReductionRule"/>. There is no information about last iterations available.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool RunFirstIteration();

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after one or more edges have been contracted in the last iteration.
        /// </summary>
        /// <param name="contractedEdgeNodeTupleList">An <see cref="CountedList{T}"/> with tuples consisting of a tuple of <see cref="TreeNode"/>s (the contracted edge), a <see cref="TreeNode"/> (the result of the edge contraction), and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s (the <see cref="DemandPair"/>s on the contracted edge).</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList);

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after one or more <see cref="DemandPair"/>s have been removed in the last iteration.
        /// </summary>
        /// <param name="removedDemandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s that were removed in the last iteration.</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs);

        /// <summary>
        /// Executed when this <see cref="ReductionRule"/> is applied after the endpoint of one or more <see cref="DemandPair"/>s were changed in the last iteration.
        /// </summary>
        /// <param name="changedEdgesPerDemandPairList">The <see cref="CountedList{T}"/> with tuples with a <see cref="CountedList{T}"/> with tuples of <see cref="TreeNode"/>s that represent the edges that were removed from the demand path, and a <see cref="DemandPair"/>.</param>
        /// <returns><see langword="true"/> if this <see cref="ReductionRule"/> was applied successfully, <see langword="false"/> otherwise.</returns>
        internal abstract bool AfterDemandPathChanged(CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList);
    }
}
