// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using MulticutInTrees.Algorithms;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.Experiments
{
    /// <summary>
    /// Class that writes a list of <see cref="ExperimentOutput"/> to a CSV file.
    /// </summary>
    internal static class ExperimentOutputWriter
    {
        /// <summary>
        /// Create a CSV file with all the <see cref="ExperimentOutput"/>s in <paramref name="list"/> in the <paramref name="outputDirectory"/> directory. The file includes the current date and time in its name.
        /// </summary>
        /// <param name="list">The <see cref="IEnumerable{T}"/> of <see cref="ExperimentOutput"/>s to write to the CSV file.</param>
        /// <param name="outputDirectory">The filepath to the directory to place the CSV file in.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="outputDirectory"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="list"/> contains no elements.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the file to write to cannot be opened.</exception>
        internal static void WriteOutput(this IEnumerable<ExperimentOutput> list, string outputDirectory)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(list, nameof(list), "Trying to write a list of experiment outputs, but the list is null!");
            Utilities.Utils.NullCheck(outputDirectory, nameof(outputDirectory), "Trying to write a list of experiment outputs, but the output directory is null!");
            if (!list.Any())
            {
                throw new ArgumentException($"Cannot write the output of less than 1 ExperimentOutputs! (List contains {list.Count()} ExperimentOutputs)", nameof(list));
            }
#endif
            try
            {
                string fileName = $@"{outputDirectory}\Experiments_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.csv";
                using StreamWriter streamWriter = new StreamWriter(fileName);
                using CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                CreateHeader(csvWriter, list.First().ReductionRulesOperations.Count);
                foreach (ExperimentOutput output in list)
                {
                    WriteSingleOutput(csvWriter, output);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("An UnauthorizedAccessException was thrown! Message: " + e.Message);
                Console.WriteLine("Do you have enough permissions to write to this file? Is your antivirus software blocking access?");
                Console.WriteLine("File is not written, continuing...");
            }
        }

        /// <summary>
        /// Create the header in the CSV file.
        /// </summary>
        /// <param name="writer">The <see cref="CsvWriter"/> to use.</param>
        /// <param name="numberOfReductionRules">The number of <see cref="ReductionRules.ReductionRule"/> the <see cref="Algorithm"/> in the results used.</param>
        private static void CreateHeader(CsvWriter writer, int numberOfReductionRules)
        {
            writer.WriteField("Nodes");
            writer.WriteField("DemandPairs");
            writer.WriteField("TreeType");
            writer.WriteField("DPType");
            writer.WriteField("Algorithm");
            writer.WriteField("Seed");
            writer.WriteField("Optimal K");
            writer.WriteField("K used");
            writer.WriteField("Solvable");
            writer.WriteField("Remaining nodes");
            writer.WriteField("Remaining DPs");
            writer.WriteField("AlgTreeOps");
            writer.WriteField("AlgDPOps");
            writer.WriteField("AlgDPEdgeKeyOps");
            writer.WriteField("AlgDPEdgeValueOps");
            writer.WriteField("AlgTicksApplicability");
            writer.WriteField("AlgTicksModifying");
            writer.WriteField("AlgTimeApplicability");
            writer.WriteField("AlgTimeModifying");

            for (int i = 0; i < numberOfReductionRules; i++)
            {
                int num = i + 1;
                writer.WriteField($"RR{num}Name");
                writer.WriteField($"RR{num}ContractedEdges");
                writer.WriteField($"RR{num}RemovedDPs");
                writer.WriteField($"RR{num}ChangedDPs");
                writer.WriteField($"RR{num}TreeOps");
                writer.WriteField($"RR{num}DPOps");
                writer.WriteField($"RR{num}DPEdgeKeyOps");
                writer.WriteField($"RR{num}DPEdgeValueOps");
                writer.WriteField($"RR{num}TicksApplicability");
                writer.WriteField($"RR{num}TicksModifying");
                writer.WriteField($"RR{num}TimeApplicability");
                writer.WriteField($"RR{num}TimeModifying");
            }

            writer.NextRecord();
        }

        /// <summary>
        /// Write the row for a single <see cref="ExperimentOutput"/> to the CSV file.
        /// </summary>
        /// <param name="writer">The <see cref="CsvWriter"/> to use.</param>
        /// <param name="output">The <see cref="ExperimentOutput"/> to be written.</param>
        private static void WriteSingleOutput(CsvWriter writer, ExperimentOutput output)
        {
            writer.WriteField(output.Nodes);
            writer.WriteField(output.DemandPairs);
            writer.WriteField(output.TreeType);
            writer.WriteField(output.DPType);
            writer.WriteField(output.Algorithm);
            writer.WriteField(output.Seed);
            writer.WriteField(output.OptimalMaxSolutionSize);
            writer.WriteField(output.MaxSolutionSize);
            writer.WriteField(output.Solvable);
            writer.WriteField(output.RemainingNodes);
            writer.WriteField(output.RemainingDPs);
            writer.WriteField(output.AlgorithmOperations.TreeOperationsCounter);
            writer.WriteField(output.AlgorithmOperations.DemandPairsOperationsCounter);
            writer.WriteField(output.AlgorithmOperations.DemandPairsPerEdgeKeysCounter);
            writer.WriteField(output.AlgorithmOperations.DemandPairsPerEdgeValuesCounter);
            writer.WriteField(output.AlgorithmOperations.TimeSpentCheckingApplicability.ElapsedTicks);
            writer.WriteField(output.AlgorithmOperations.TimeSpentModifyingInstance.ElapsedTicks);
            writer.WriteField(output.AlgorithmOperations.TimeSpentCheckingApplicability.Elapsed.ToString(@"hh\:mm\:ss\.fffffff"));
            writer.WriteField(output.AlgorithmOperations.TimeSpentModifyingInstance.Elapsed.ToString(@"hh\:mm\:ss\.fffffff"));

            foreach (PerformanceMeasurements reductionRuleMeasurements in output.ReductionRulesOperations)
            {
                writer.WriteField(reductionRuleMeasurements.Owner);
                writer.WriteField(reductionRuleMeasurements.NumberOfContractedEdgesCounter);
                writer.WriteField(reductionRuleMeasurements.NumberOfRemovedDemandPairsCounter);
                writer.WriteField(reductionRuleMeasurements.NumberOfChangedDemandPairsCounter);
                writer.WriteField(reductionRuleMeasurements.TreeOperationsCounter);
                writer.WriteField(reductionRuleMeasurements.DemandPairsOperationsCounter);
                writer.WriteField(reductionRuleMeasurements.DemandPairsPerEdgeKeysCounter);
                writer.WriteField(reductionRuleMeasurements.DemandPairsPerEdgeValuesCounter);
                writer.WriteField(reductionRuleMeasurements.TimeSpentCheckingApplicability.ElapsedTicks);
                writer.WriteField(reductionRuleMeasurements.TimeSpentModifyingInstance.ElapsedTicks);
                writer.WriteField(reductionRuleMeasurements.TimeSpentCheckingApplicability.Elapsed.ToString(@"hh\:mm\:ss\.fffffff"));
                writer.WriteField(reductionRuleMeasurements.TimeSpentModifyingInstance.Elapsed.ToString(@"hh\:mm\:ss\.fffffff"));
            }

            writer.NextRecord();
        }
    }
}