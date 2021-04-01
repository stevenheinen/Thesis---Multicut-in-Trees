// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.ObjectModel;
using MulticutInTrees.Algorithms;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.Experiments
{
    /// <summary>
    /// Class containing the output of an experiment.
    /// </summary>
    public class ExperimentOutput
    {
        /// <summary>
        /// The original number of nodes in the instance.
        /// </summary>
        public int Nodes { get; }

        /// <summary>
        /// The original number of <see cref="DemandPair"/>s in the instance.
        /// </summary>
        public int DemandPairs { get; }

        /// <summary>
        /// The <see cref="InputTreeType"/> used to generate the tree in the instance.
        /// </summary>
        public InputTreeType TreeType { get; }

        /// <summary>
        /// The <see cref="InputDemandPairsType"/> used to generate the <see cref="DemandPair"/>s in the instance.
        /// </summary>
        public InputDemandPairsType DPType { get; }

        /// <summary>
        /// The <see cref="AlgorithmType"/> used to solve the instance.
        /// </summary>
        public AlgorithmType Algorithm { get; }

        /// <summary>
        /// The seed used for the random number generator in the instance.
        /// </summary>
        public int Seed { get; }

        /// <summary>
        /// Whether the instance is solvable.
        /// </summary>
        public bool Solvable { get; }

        /// <summary>
        /// The resulting size of the kernel.
        /// </summary>
        public int KernelSize { get; }

        /// <summary>
        /// The <see cref="PerformanceMeasurements"/> the <see cref="Algorithm"/> itself used.
        /// </summary>
        public PerformanceMeasurements AlgorithmOperations { get; }

        /// <summary>
        /// <see cref="ReadOnlyCollection{T}"/> of the <see cref="PerformanceMeasurements"/> per <see cref="ReductionRules.ReductionRule"/> in the algorithm.
        /// </summary>
        public ReadOnlyCollection<PerformanceMeasurements> ReductionRulesOperations { get; }

        /// <summary>
        /// Constructor for a <see cref="ExperimentOutput"/>.
        /// </summary>
        /// <param name="nodes">The original number of nodes in the instance.</param>
        /// <param name="demandPairs">The original number of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="treeType">The <see cref="InputTreeType"/> used to generate the tree in the instance.</param>
        /// <param name="dpType">The <see cref="InputDemandPairsType"/> used to generate the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="AlgorithmType"/> used to solve the instance.</param>
        /// <param name="seed">The seed used for the random number generator in the instance.</param>
        /// <param name="solvable">Whether the instance is solvable.</param>
        /// <param name="kernelSize">The resulting size of the kernel.</param>
        /// <param name="algorithmOperations">The <see cref="PerformanceMeasurements"/> the <see cref="Algorithm"/> itself used.</param>
        /// <param name="reductionRulesOperations"><see cref="ReadOnlyCollection{T}"/> of the <see cref="PerformanceMeasurements"/> per <see cref="ReductionRules.ReductionRule"/> in the algorithm.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="algorithmOperations"/> or <paramref name="reductionRulesOperations"/> is <see langword="null"/>.</exception>
        public ExperimentOutput(int nodes, int demandPairs, InputTreeType treeType, InputDemandPairsType dpType, AlgorithmType algorithm, int seed, bool solvable, int kernelSize, PerformanceMeasurements algorithmOperations, ReadOnlyCollection<PerformanceMeasurements> reductionRulesOperations)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(algorithmOperations, nameof(algorithmOperations), "Trying to creat an experiment output, but the performance measures of the algorithm is null!"); 
            Utilities.Utils.NullCheck(reductionRulesOperations, nameof(reductionRulesOperations), "Trying to creat an experiment output, but the list with performance measures of the reduction rules is null!");
#endif
            Nodes = nodes;
            DemandPairs = demandPairs;
            TreeType = treeType;
            DPType = dpType;
            Algorithm = algorithm;
            Seed = seed;
            Solvable = solvable;
            KernelSize = kernelSize;
            AlgorithmOperations = algorithmOperations;
            ReductionRulesOperations = reductionRulesOperations;
        }
    }
}
