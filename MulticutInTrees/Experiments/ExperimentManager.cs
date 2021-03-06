// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Experiments
{
    /// <summary>
    /// Class that manages the experiments that are run. Contains methods to run the correct experiments given the command line options.
    /// </summary>
    internal static class ExperimentManager
    {
        /// <summary>
        /// Run an experiment given the command line options.
        /// </summary>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when there is no experiment that can be executed given these command line options.</exception>
        internal static void RunExperiment(CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to run experiments, but the command line options are null!");
#endif
            List<ExperimentOutput> results = options.AlgorithmType switch
            {
                AlgorithmType.GuoNiedermeierKernelisation or AlgorithmType.BousquetKernelisation or AlgorithmType.ChenKernelisation => RunMultipleExperiments(options, RunKernelisationAlgorithm),
                AlgorithmType.GurobiMIPSolver => RunMultipleExperiments(options, RunGurobiMIPAlgorithm),
                AlgorithmType.GuoNiederMeierBranching => RunMultipleExperiments(options, RunBranchingAlgorithm),
                AlgorithmType.BruteForce => RunMultipleExperiments(options, RunBruteForceAlgorithm),
                AlgorithmType.GenerateInstances => RunMultipleExperiments(options, RunGenerateInstance),
                _ => throw new NotSupportedException($"Trying to run an experiment, but there is no experiment supported with the given command line options. (Algorithm {options.AlgorithmType} is not supported.)"),
            };

#if EXPERIMENT
            if (options.AlgorithmType != AlgorithmType.GenerateInstances)
            {
                results.WriteOutput(options.OutputDirectory);
            }
#endif
        }

        /// <summary>
        /// Runs a number of experiments.
        /// </summary>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <param name="singleExperimentMethod"><see cref="Func{T1, T2, TResult}"/> that will be used to run a single experiment.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="ExperimentOutput"/>s that result from these experiments.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> or <paramref name="singleExperimentMethod"/> is <see langword="null"/>.</exception>
        private static List<ExperimentOutput> RunMultipleExperiments(CommandLineOptions options, Func<int, int, int, int, CommandLineOptions, ExperimentOutput> singleExperimentMethod)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to run multiple experiments, but the options are null!");
            Utils.NullCheck(singleExperimentMethod, nameof(singleExperimentMethod), "Trying to run multiple experiments, but the function that tells how to run a single experiment is null!");
#endif
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput($"Running multiple experiments with the {options.AlgorithmType} algorithm", options.TreeSeed, options.DemandPairSeed, -1, -1, options));
            }

            List<ExperimentOutput> output = new();

            for (int i = 0; i < options.Experiments; i++)
            {
                for (int j = 1; j <= options.Repetitions; j++)
                {
                    ExperimentOutput algorithmOutput = singleExperimentMethod(options.TreeSeed + i, options.DemandPairSeed + i, i + 1, j, options);
                    output.Add(algorithmOutput);
                }
            }

            return output;
        }

        /// <summary>
        /// Formats the input the program got from the command line arguments into a nice message.
        /// </summary>
        /// <param name="experimentMessage">The message that is specific to an experiment.</param>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="currentExperiment">The index of the current experiment in [1..numberOfExperiments].</param>
        /// <param name="currentRepetition">How many times this exact experiment has been executed.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>A <see cref="string"/> with a formatted representation of the command line arguments.</returns>
        private static string FormatParseOutput(string experimentMessage, int treeSeed, int dpSeed, int currentExperiment, int currentRepetition, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(experimentMessage, nameof(experimentMessage), "Trying to format the parse output to a nice readable message, but the experiment specific message is null!");
            Utils.NullCheck(options, nameof(options), "Trying to format the parse output to a nice readable message, but the command line arguments are null!");
#endif
            StringBuilder sb = new();
            sb.Append($"{experimentMessage} on the following instance: {options.Experiments} different experiments, each with {options.Repetitions} repetitions, tree seed {treeSeed}, dp seed {dpSeed}, ");
            sb.Append(options.MaxSolutionSize > 0 ? $"a maximum solution size of {options.MaxSolutionSize}, " : "the optimal solution size, ");

            if (options.InputTreeType == InputTreeType.Prufer || options.InputTreeType == InputTreeType.Caterpillar || options.InputTreeType == InputTreeType.Degree3Tree)
            {
                sb.Append($"a {options.InputTreeType} tree with {options.NumberOfNodes} nodes, ");
                if (options.InputDemandPairsType == InputDemandPairsType.Random)
                {
                    sb.Append($"and {options.NumberOfDemandPairs} randomly generated demand pairs.");
                }
                else if (options.InputDemandPairsType == InputDemandPairsType.LengthDistribution)
                {
                    sb.Append($"and {options.NumberOfDemandPairs} demand pairs generated using {options.DistanceDistribution} as distribution.");
                }
                else if (options.InputDemandPairsType == InputDemandPairsType.ThroughKnownSolution)
                {
                    sb.Append($"and {options.NumberOfDemandPairs} randomly generated through a known solution of size {options.MaxSolutionSize}.");
                }
            }
            else
            {
                sb.Append($"and a tree and demand pairs from the {options.InputTreeType} instance found here \"{options.InstanceFilePath}\".");
            }

            if (currentExperiment != -1)
            {
                sb.Append($" This is experiment {currentExperiment} of {options.Experiments}");
            }
            if (currentRepetition != -1)
            {
                sb.Append($", and repetition {currentRepetition} of {options.Repetitions}");
            }
            if (currentExperiment != -1)
            {
                sb.Append('.');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Does not run any algorithm, but does generate instances for future use. If these instances do not exist already, the Gurobi MIP solver will be used to compute the optimal K value for these instances.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="currentExperiment">The index of the current experiment in [1..numberOfExperiments].</param>
        /// <param name="currentRepetition">How many times this exact experiment has been executed.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>A basically empty <see cref="ExperimentOutput"/>, since no algorithm was run.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private static ExperimentOutput RunGenerateInstance(int treeSeed, int dpSeed, int currentExperiment, int currentRepetition, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to run an experiment with the brute force algorithm, but the command line options are null!");
#endif
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput("Generating the instance", treeSeed, dpSeed, currentExperiment, currentRepetition, options));
            }

            MulticutInstance instance = new(treeSeed, dpSeed, options);

            if (options.Verbose)
            {
                Console.WriteLine("Instance generated!");
            }

            ExperimentOutput output = new(instance.NumberOfNodes, instance.NumberOfDemandPairs, options.InputTreeType, options.InputDemandPairsType, AlgorithmType.GenerateInstances, treeSeed, dpSeed, instance.TreeFileName, instance.DPFileName, instance.K, instance.OptimalK, true, -1, -1, -1, new PerformanceMeasurements("Generate instances"), new ReadOnlyCollection<PerformanceMeasurements>(new List<PerformanceMeasurements>()));

            return output;
        }

        /// <summary>
        /// Runs the Brute Force algorithm on the instance corresponding to the given command line options.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="currentExperiment">The index of the current experiment in [1..numberOfExperiments].</param>
        /// <param name="currentRepetition">How many times this exact experiment has been executed.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>An <see cref="ExperimentOutput"/> with the results of this experiment.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private static ExperimentOutput RunBruteForceAlgorithm(int treeSeed, int dpSeed, int currentExperiment, int currentRepetition, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to run an experiment with the brute force algorithm, but the command line options are null!");
#endif
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput("Running an experiment with the brute force algorithm", treeSeed, dpSeed, currentExperiment, currentRepetition, options));
            }

            MulticutInstance instance = new(treeSeed, dpSeed, options);
            BruteForceAlgorithm algorithm = new(instance);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            bool solved = algorithm.Run();
            stopwatch.Stop();

            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine("Brute force algorithm result:");
                Console.WriteLine($"Solved?                {solved}");
                Console.WriteLine($"Time required (ticks): {stopwatch.ElapsedTicks}");
                Console.WriteLine();
            }

            ExperimentOutput output = new(instance.NumberOfNodes, instance.NumberOfDemandPairs, options.InputTreeType, options.InputDemandPairsType, AlgorithmType.BruteForce, treeSeed, dpSeed, instance.TreeFileName, instance.DPFileName, instance.K, instance.OptimalK, solved, -1, -1, -1, new PerformanceMeasurements(nameof(GurobiMIPAlgorithm)), new ReadOnlyCollection<PerformanceMeasurements>(new List<PerformanceMeasurements>()));

            return output;
        }

        /// <summary>
        /// Uses the Gurobi MIP solver to determine the smallest possible solution size on this instance.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="currentExperiment">The index of the current experiment in [1..numberOfExperiments].</param>
        /// <param name="currentRepetition">How many times this exact experiment has been executed.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>An <see cref="ExperimentOutput"/> that includes the smallest possible solution size found by this algorithm.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private static ExperimentOutput RunGurobiMIPAlgorithm(int treeSeed, int dpSeed, int currentExperiment, int currentRepetition, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to run an experiment with the Gurobi MIP solver, but the command line options are null!");
#endif
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput("Finding the minimum possible value for the maximum solution size using the Gurobi MIP solver", treeSeed, dpSeed, currentExperiment, currentRepetition, options));
            }

            MulticutInstance instance = new(treeSeed, dpSeed, options);
            GurobiMIPAlgorithm algorithm = new(instance);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            int minimumSize = algorithm.Run(options.MIPTimeLimit, options.Verbose);
            stopwatch.Stop();

            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine("Gurobi MIP solver result:");
                Console.WriteLine($"Smallest possible solution size: {minimumSize}");
                Console.WriteLine($"Time required (ticks):           {stopwatch.ElapsedTicks}");
                Console.WriteLine();
            }

            ExperimentOutput output = new(instance.NumberOfNodes, instance.NumberOfDemandPairs, options.InputTreeType, options.InputDemandPairsType, AlgorithmType.GurobiMIPSolver, treeSeed, dpSeed, instance.TreeFileName, instance.DPFileName, options.MaxSolutionSize, minimumSize, true, -1, -1, -1, new PerformanceMeasurements(nameof(GurobiMIPAlgorithm)), new ReadOnlyCollection<PerformanceMeasurements>(new List<PerformanceMeasurements>()));

            return output;
        }

        /// <summary>
        /// Runs an experiment with the branching algorithm.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="currentExperiment">The index of the current experiment in [1..numberOfExperiments].</param>
        /// <param name="currentRepetition">How many times this exact experiment has been executed.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>The <see cref="ExperimentOutput"/> of this algorithm.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private static ExperimentOutput RunBranchingAlgorithm(int treeSeed, int dpSeed, int currentExperiment, int currentRepetition, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to run an experiment with the branching algorithm, but the command line options are null!");
#endif
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput("Running Guo and Niedermeiers branching algorithm", treeSeed, dpSeed, currentExperiment, currentRepetition, options));
            }

            MulticutInstance instance = new(treeSeed, dpSeed, options);
            GuoNiedermeierBranching gnBranching = new(instance);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            (List<Edge<Node>> solution, ExperimentOutput experimentOutput) = gnBranching.Run();
            stopwatch.Stop();

            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine("Branching algorithm result:");
                Console.WriteLine($"Solved?                {experimentOutput.Solvable}");
                Console.WriteLine($"Solution size:         {solution.Count}");
                Console.WriteLine($"Time required (ticks): {stopwatch.ElapsedTicks}");
                Console.WriteLine($"Entire solution:       {solution.Print()}");
                Console.WriteLine();
            }

            return experimentOutput;
        }

        /// <summary>
        /// Runs an experiment with a kernelisation algorithm.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="currentExperiment">The index of the current experiment in [1..numberOfExperiments].</param>
        /// <param name="currentRepetition">How many times this exact experiment has been executed.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>The <see cref="ExperimentOutput"/> of this algorithm.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private static ExperimentOutput RunKernelisationAlgorithm(int treeSeed, int dpSeed, int currentExperiment, int currentRepetition, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to run an experiment with a kernelisation algorithm, but the command line options are null!");
#endif
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput($"Running the {options.AlgorithmType} algorithm", treeSeed, dpSeed, currentExperiment, currentRepetition, options));
            }

            MulticutInstance instance = new(treeSeed, dpSeed, options);
            Algorithm algorithm = CreateAlgorithmInstance(options.AlgorithmType, instance);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            (Graph tree, List<Edge<Node>> partialSolution, List<DemandPair> finalDemandPairs, ExperimentOutput experimentOutput) = algorithm.Run();
            stopwatch.Stop();

            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine($"FPT algorithm ({algorithm.GetType()}) result:");
                Console.WriteLine($"Solved?                           {experimentOutput.Solvable}");
                Console.WriteLine($"Partial solution size:            {partialSolution.Count}");
                Console.WriteLine($"Remaining tree:                   {tree}");
                Console.WriteLine($"Remaining number of demand pairs: {finalDemandPairs.Count}");
#if VERBOSEDEBUG
                Console.WriteLine($"Edge-disjoint demand pairs left:  {MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowGraph<Graph, Edge<Node>, Node>(tree, System.Linq.Enumerable.Select(finalDemandPairs, dp => (dp.Node1, dp.Node2)), new PerformanceMeasurements(""))}");
#endif
                Console.WriteLine($"Time required (ticks):            {stopwatch.ElapsedTicks}");
                Console.WriteLine($"Entire partial solution:          {partialSolution.Print()}");
                Console.WriteLine($"Remaining edges:                  {tree.Edges(new Counter()).Print()}");
                Console.WriteLine($"Remaining dps:                    {finalDemandPairs.Print()}");
                Console.WriteLine();
            }

            return experimentOutput;
        }

        /// <summary>
        /// Returns an instance of the correct subclass of <see cref="Algorithm"/> given <paramref name="algorithmType"/>.
        /// </summary>
        /// <param name="algorithmType">The <see cref="AlgorithmType"/> the return type should have.</param>
        /// <param name="instance">The instance we are going to solve.</param>
        /// <returns>An instance of a subclass of <see cref="Algorithm"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="algorithmType"/> is not supported as algorithm.</exception>
        private static Algorithm CreateAlgorithmInstance(AlgorithmType algorithmType, MulticutInstance instance)
        {
#if !EXPERIMENT
            Utils.NullCheck(instance, nameof(instance), "Trying to create a kernelisation algorithm instance, but the multicut instance to solve is null!");
#endif
            return algorithmType switch
            {
                AlgorithmType.GuoNiedermeierKernelisation => new GuoNiedermeierKernelisation(instance),
                AlgorithmType.BousquetKernelisation => new BousquetKernelisation(instance),
                AlgorithmType.ChenKernelisation => new ChenKernelisation(instance),
                _ => throw new NotSupportedException($"The algorithm type {algorithmType} is not supported!")
            };
        }
    }
}
