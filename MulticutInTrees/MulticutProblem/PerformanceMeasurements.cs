// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Diagnostics;
using System.Text;
using MulticutInTrees.CountedDatastructures;

namespace MulticutInTrees.MulticutProblem
{
    /// <summary>
    /// Class that contains all information about the (running time) performance of an <see cref="Algorithms.Algorithm"/> or <see cref="ReductionRules.ReductionRule"/>.
    /// </summary>
    public class PerformanceMeasurements
    {
        /// <summary>
        /// The name of the user of this <see cref="PerformanceMeasurements"/>.
        /// </summary>
        public string Owner { get; }

        /// <summary>
        /// <see cref="Counter"/> for the number of operations on the <see cref="Graphs.AbstractGraph{TEdge, TNode}"/> in the instance.
        /// </summary>
        public Counter TreeOperationsCounter { get; set; }

        /// <summary>
        /// <see cref="Counter"/> for the number of operations on the <see cref="DemandPair"/>s in the instance.
        /// </summary>
        public Counter DemandPairsOperationsCounter { get; }

        /// <summary>
        /// <see cref="Counter"/> for the number of operations on the keys of the <see cref="CountedDictionary{TKey, TValue}"/> with information about the <see cref="DemandPair"/>s per edge.
        /// </summary>
        public Counter DemandPairsPerEdgeKeysCounter { get; }

        /// <summary>
        /// <see cref="Counter"/> for the number of operations on the values of the <see cref="CountedDictionary{TKey, TValue}"/> with information about the <see cref="DemandPair"/>s per edge.
        /// </summary>
        public Counter DemandPairsPerEdgeValuesCounter { get; }

        /// <summary>
        /// <see cref="Counter"/> for the total number of edges contracted by the owner of this <see cref="PerformanceMeasurements"/>.
        /// </summary>
        public Counter NumberOfContractedEdgesCounter { get; set; }

        /// <summary>
        /// <see cref="Counter"/> for the total number of <see cref="DemandPair"/>s that were changed by the owner of this <see cref="PerformanceMeasurements"/>.
        /// </summary>
        public Counter NumberOfChangedDemandPairsCounter { get; set; }

        /// <summary>
        /// <see cref="Counter"/> for the total number of <see cref="DemandPair"/>s that were removed by the owner of this <see cref="PerformanceMeasurements"/>.
        /// </summary>
        public Counter NumberOfRemovedDemandPairsCounter { get; set; }

        /// <summary>
        /// <see cref="Stopwatch"/> that counts the time the owner of this <see cref="PerformanceMeasurements"/> spent on checking whether it could be applied.
        /// </summary>
        public Stopwatch TimeSpentCheckingApplicability { get; }

        /// <summary>
        /// <see cref="Stopwatch"/> that counts the time the owner of this <see cref="PerformanceMeasurements"/> spent on modifying the instance.
        /// </summary>
        public Stopwatch TimeSpentModifyingInstance { get; }

        /// <summary>
        /// Constructor for a <see cref="PerformanceMeasurements"/>.
        /// </summary>
        /// <param name="ownerName">The name of the owner of this <see cref="PerformanceMeasurements"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ownerName"/> is <see langword="null"/>.</exception>
        public PerformanceMeasurements(string ownerName)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(ownerName, nameof(ownerName), "Trying to create an instance of PerformanceMeasurements, but the name of the owner is null!");
#endif
            Owner = ownerName;
            TreeOperationsCounter = new Counter();
            DemandPairsOperationsCounter = new Counter();
            DemandPairsPerEdgeKeysCounter = new Counter();
            DemandPairsPerEdgeValuesCounter = new Counter();
            NumberOfContractedEdgesCounter = new Counter();
            NumberOfChangedDemandPairsCounter = new Counter();
            NumberOfRemovedDemandPairsCounter = new Counter();
            TimeSpentCheckingApplicability = new Stopwatch();
            TimeSpentModifyingInstance = new Stopwatch();
        }

        /// <summary>
        /// Returns a <see cref="string"/> with the performance measured by this <see cref="PerformanceMeasurements"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> with the performance measured by this <see cref="PerformanceMeasurements"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append('\n');
            sb.Append("==============================================================\n");
            sb.Append("==============================================================\n");
            sb.Append($"{Owner} instance modifications\n");
            sb.Append("==============================================================\n");
            sb.Append($"Number of contracted edges:    {NumberOfContractedEdgesCounter}\n");
            sb.Append($"Number of changed demandpairs: {NumberOfChangedDemandPairsCounter}\n");
            sb.Append($"Number of removed demandpairs: {NumberOfRemovedDemandPairsCounter}\n");
            sb.Append("==============================================================\n");
            sb.Append($"{Owner} operations\n");
            sb.Append("==============================================================\n");
            sb.Append($"Operations on the input tree:              {TreeOperationsCounter}\n");
            sb.Append($"Operations on demandpairs:                 {DemandPairsOperationsCounter}\n");
            sb.Append($"Operations on demandpairs per edge keys:   {DemandPairsPerEdgeKeysCounter}\n");
            sb.Append($"Operations on demandpairs per edge values: {DemandPairsPerEdgeValuesCounter}\n");
            sb.Append("==============================================================\n");
            sb.Append($"{Owner} time\n");
            sb.Append("==============================================================\n");
            sb.Append($"Time spent checking applicability (in ticks): {TimeSpentCheckingApplicability.ElapsedTicks}\n");
            sb.Append($"Time spent modifying the instance (in ticks): {TimeSpentModifyingInstance.ElapsedTicks}\n");
            sb.Append($"Time spent checking applicability (TimeSpan): {TimeSpentCheckingApplicability.Elapsed:hh\\:mm\\:ss\\.fffffff}\n");
            sb.Append($"Time spent modifying the instance (TimeSpan): {TimeSpentModifyingInstance.Elapsed:hh\\:mm\\:ss\\.fffffff}\n");
            sb.Append("==============================================================\n");
            sb.Append('\n');

            return sb.ToString();
        }
    }
}
