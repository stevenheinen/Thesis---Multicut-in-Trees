// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using MulticutInTrees.Algorithms;
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
        /// <exception cref="NotSupportedException">Thrown when there is no experiment that can be executed given these command line options.</exception>
        internal static void RunExperiment(CommandLineOptions options)
        {
            switch (options.AlgorithmType)
            {
                case AlgorithmType.GurobiMIPSolver:
                    FindMinimumSolutionSize(options);
                    break;
                case AlgorithmType.GuoNiederMeierBranching:
                    List<ExperimentOutput> result1 = RunBranchingAlgorithmExperiments(options);
                    result1.WriteOutput(options.OutputDirectory);
                    break;
                case AlgorithmType.GuoNiedermeierKernelisation:
                    List<ExperimentOutput> result2 = RunKernelisationAlgorithmExperiments(options);
                    result2.WriteOutput(options.OutputDirectory);
                    break;
                default:
                    throw new NotSupportedException($"Trying to run an experiment, but there is no experiment supported with the given command line options.");
            }
        }

        /// <summary>
        /// Runs a number of experiments with a kernelisation algorithm.
        /// </summary>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="ExperimentOutput"/>s that result from these experiments.</returns>
        private static List<ExperimentOutput> RunKernelisationAlgorithmExperiments(CommandLineOptions options)
        {
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput($"Running the {options.AlgorithmType} algorithm", options.RandomSeed, options));
            }

            List<ExperimentOutput> output = new List<ExperimentOutput>();

            for (int i = 0; i < options.Repetitions; i++)
            {
                ExperimentOutput algorithmOutput = RunKernelisationAlgorithm(options.RandomSeed + i, options);
                output.Add(algorithmOutput);
            }

            return output;
        }

        /// <summary>
        /// Runs a number of experiments with the branching algorithm.
        /// </summary>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="ExperimentOutput"/>s that result from these experiments.</returns>
        private static List<ExperimentOutput> RunBranchingAlgorithmExperiments(CommandLineOptions options)
        {
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput($"Running Guo and Niedermeiers branching algorithm, trying to find a solution with size at most {options.MaxSolutionSize}", options.RandomSeed, options));
            }

            List<ExperimentOutput> output = new List<ExperimentOutput>();

            for (int i = 0; i < options.Repetitions; i++)
            {
                ExperimentOutput algorithmOutput = RunBranchingAlgorithm(options.RandomSeed + i, options).experimentOutput;
                output.Add(algorithmOutput);
            }

            return output;
        }

        /// <summary>
        /// Formats the input the program got from the command line arguments into a nice message.
        /// </summary>
        /// <param name="experimentMessage">The message that is specific to an experiment.</param>
        /// <param name="randomSeed">The seed used for random number generation.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>A <see cref="string"/> with a formatted representation of the command line arguments.</returns>
        private static string FormatParseOutput(string experimentMessage, int randomSeed, CommandLineOptions options)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{experimentMessage} on the following instance: {options.Repetitions} repetitions, random seed {randomSeed}, ");
            if (options.MaxSolutionSize != -1)
            {
                sb.Append($"a maximum solution size of {options.MaxSolutionSize}, ");
            }
            if (options.InputTreeType == InputTreeType.Prüfer || options.InputTreeType == InputTreeType.Caterpillar)
            {
                sb.Append($"a {options.InputTreeType} tree with {options.NumberOfNodes} nodes, ");
                if (options.InputDemandPairsType == InputDemandPairsType.Random)
                {
                    sb.Append($"and {options.NumberOfDemandPairs} randomly generated demand pairs.");
                }
                else
                {
                    sb.Append($"and {options.NumberOfDemandPairs} demand pairs generated using {options.DistanceDistribution} as distribution.");
                }
            }
            else
            {
                sb.Append($"and a tree and demand pairs from the {options.InputTreeType} instance found here \"{options.InstanceFilePath}\".");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Uses the gurobi MIP solver to determine the smallest possible solution size on this instance.
        /// </summary>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>The smallest possible solution size on the instance defined by the parametes in this function.</returns>
        private static int FindMinimumSolutionSize(CommandLineOptions options)
        {
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput("Finding the minimum possible value for the maximum solution size using the Gurobi MIP solver", options.RandomSeed, options));
            }

            (Tree<TreeNode> tree, CountedList<DemandPair> demandPairs) = GetInstance(options.RandomSeed, options);
            GurobiMIPAlgorithm algorithm = new GurobiMIPAlgorithm(tree, demandPairs.GetInternalList());
            int minimumSize = algorithm.Run(options.Verbose);

            if (options.Verbose)
            {
                Console.WriteLine($"Smallest possible K found! It is equal to {minimumSize}!");
            }

            return minimumSize;
        }

        /// <summary>
        /// Runs an experiment with the branching algorithm.
        /// </summary>
        /// <param name="randomSeed">The seed to use for the random number generation.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>A tuple with the solution and the <see cref="ExperimentOutput"/> of this algorithm.</returns>
        private static (List<(TreeNode, TreeNode)> solution, ExperimentOutput experimentOutput) RunBranchingAlgorithm(int randomSeed, CommandLineOptions options)
        {
            if (options.Verbose)
            {
                Console.WriteLine(FormatParseOutput($"Running Guo and Niedermeiers branching algorithm, trying to find a solution with size at most {options.MaxSolutionSize}", randomSeed, options));
            }

            (Tree<TreeNode> tree, CountedList<DemandPair> demandPairs) = GetInstance(randomSeed, options);
            MulticutInstance instance = new MulticutInstance(options.InputTreeType, options.InputDemandPairsType, randomSeed, tree, demandPairs, options.MaxSolutionSize);
            GuoNiedermeierBranching gnBranching = new GuoNiedermeierBranching(instance);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            (List<(TreeNode, TreeNode)> solution, ExperimentOutput experimentOutput) = gnBranching.Run(false, CancellationToken.None);
            stopwatch.Stop();

            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine("Branching algorithm result:");
                Console.WriteLine($"Solved?          {experimentOutput.Solvable}");
                Console.WriteLine($"Solution size:   {solution.Count}");
                Console.WriteLine($"Time required:   {stopwatch.ElapsedTicks}");
                Console.WriteLine($"Entire solution: {solution.Print()}");
                Console.WriteLine();
            }

            return (solution, experimentOutput);
        }

        /// <summary>
        /// Runs an experiment with a kernelisation algorithm.
        /// </summary>
        /// <param name="randomSeed">The seed to use for the random number generation.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>The <see cref="ExperimentOutput"/> of this algorithm.</returns>
        private static ExperimentOutput RunKernelisationAlgorithm(int randomSeed, CommandLineOptions options)
        {
            (Tree<TreeNode> tree, CountedList<DemandPair> demandPairs) = GetInstance(randomSeed, options);
            MulticutInstance instance = new MulticutInstance(options.InputTreeType, options.InputDemandPairsType, randomSeed, tree, demandPairs, options.MaxSolutionSize);
            Algorithm algorithm = CreateAlgorithmInstance(options.AlgorithmType, instance);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            (Tree<TreeNode> _, List<(TreeNode, TreeNode)> partialSolution, List<DemandPair> finalDemandPairs, ExperimentOutput experimentOutput) = algorithm.Run();
            stopwatch.Stop();

            if (options.Verbose)
            {
                Console.WriteLine();
                Console.WriteLine($"FPT algorithm ({algorithm.GetType()}) result:");
                Console.WriteLine($"Solved?                           {experimentOutput.Solvable}");
                Console.WriteLine($"Partial solution size:            {partialSolution.Count}");
                Console.WriteLine($"Remaining tree:                   {tree}");
                Console.WriteLine($"Remaining number of demand pairs: {finalDemandPairs.Count}");
                Console.WriteLine($"Time:                             {stopwatch.Elapsed}");
                Console.WriteLine($"Entire partial solution:          {partialSolution.Print()}");
                Console.WriteLine($"Remaining edges:                  {tree.Edges(new Counter()).Print()}");
                Console.WriteLine($"Remaining dps:                    {finalDemandPairs.Print()}");
                Console.WriteLine();
            }

            return experimentOutput;
        }

        /// <summary>
        /// Tries to find whether this instance already exists in the files. If so, it grabs it from there. If not, it creates it, finds the optimal K and writes it to the files for future reference.
        /// </summary>
        /// <param name="randomSeed">The seed used for the random number generator.</param>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <returns>A tuple with a <see cref="Tree{N}"/> and a <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s. The required information for an instance to run.</returns>
        private static (Tree<TreeNode> tree, CountedList<DemandPair> demandPairs) GetInstance(int randomSeed, CommandLineOptions options)
        {
            (Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, int optimalK) = InstanceReaderWriter.ReadInstance(options.InstanceDirectory, randomSeed, options.InputTreeType, options.NumberOfNodes, options.InstanceFilePath, options.InputDemandPairsType, options.NumberOfDemandPairs, options.DemandPairFilePath, options.DistanceDistribution);
            if (tree is null)
            {
                if (options.Verbose)
                {
                    Console.WriteLine("Instance not yet found in the instance files, creating it!");
                }

                Random treeRandom = new Random(randomSeed);
                Random demandPairRandom = new Random(randomSeed);
                tree = CreateInputTree(options.InputTreeType, treeRandom, options.NumberOfNodes, options.InstanceFilePath);
                Dictionary<(int, int), double> distanceDistribution = ParseLengthDistributionDictionary(options.DistanceDistribution);
                demandPairs = CreateInputDemandPairs(demandPairRandom, tree, options.InputDemandPairsType, options.NumberOfDemandPairs, distanceDistribution, options.DemandPairFilePath);

                GurobiMIPAlgorithm algorithm = new GurobiMIPAlgorithm(tree, demandPairs.GetInternalList());
                optimalK = algorithm.Run(options.Verbose);
                
                InstanceReaderWriter.WriteInstance(options.InstanceDirectory, randomSeed, tree, options.InputTreeType, options.InstanceFilePath, demandPairs.GetInternalList(), options.InputDemandPairsType, options.DemandPairFilePath, options.DistanceDistribution, optimalK);

                if (options.Verbose)
                {
                    Console.WriteLine($"Instance created! Minimum possible solution size: {optimalK}. This experiment is using {options.MaxSolutionSize}.");
                }
            }
            else if (options.Verbose)
            {
                Console.WriteLine($"Found the instance in the instance files! Minimum possible solution size: {optimalK}. This experiment is using {options.MaxSolutionSize}.");
            }

            return (tree, demandPairs);
        }

        /// <summary>
        /// Create a <see cref="Tree{T}"/> using the method given in <paramref name="inputTreeType"/>.
        /// </summary>
        /// <param name="inputTreeType">The <see cref="InputTreeType"/> that says which method to use to create the tree.</param>
        /// <param name="random">The <see cref="Random"/> to be used for random number generation. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <param name="numberOfNodes">The number of nodes in the resulting <see cref="Tree{T}"/>. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <param name="filePath">The path to the file with the CNF-SAT / Vertex Cover instance to generate the <see cref="Tree{N}"/> from. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <returns>A <see cref="Tree{N}"/> that is generated according to <paramref name="inputTreeType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="inputTreeType"/> is not supported as tree generation type.</exception>
        private static Tree<TreeNode> CreateInputTree(InputTreeType inputTreeType, Random random, int numberOfNodes, string filePath)
        {
            return inputTreeType switch
            {
                InputTreeType.Caterpillar => CaterpillarGenerator.CreateCaterpillar(numberOfNodes, random),
                InputTreeType.CNFSAT => throw new NotImplementedException("CNF-SAT instances are not yet supported!"),
                InputTreeType.Prüfer => TreeFromPruferSequence.GenerateTree(numberOfNodes, random),
                InputTreeType.VertexCover => throw new NotImplementedException("Vertex Cover instances are not yet supported!"),
                _ => throw new NotSupportedException($"The input tree type {inputTreeType} is not supported!")
            };
        }

        /// <summary>
        /// Create a <see cref="CountedList{T}"/> with <see cref="DemandPair"/>s using the method given in <see cref="InputTreeType"/>.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to be used for random number generation.</param>
        /// <param name="inputTree">The <see cref="Tree{N}"/> in which to generate the <see cref="DemandPair"/>s.</param>
        /// <param name="inputDemandPairType">The <see cref="InputDemandPairsType"/> that says which method to use to create the <see cref="DemandPair"/>s.</param>
        /// <param name="numberOfDemandPairs">The required number of <see cref="DemandPair"/>s.</param>
        /// <param name="distanceProbability">A <see cref="Dictionary{TKey, TValue}"/> from a lower and upperbound on a distance to the probability of choosing that distance for the length of a <see cref="DemandPair"/>. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <param name="filePath">The path to the file with the endpoints of the the <see cref="DemandPair"/>s. Only required for the <see cref="InputDemandPairsType.Fixed"/> type.</param>
        /// <returns>A <see cref="CountedList{T}"/> with <see cref="DemandPair"/>s that were generated according to the method given by <paramref name="inputDemandPairType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="inputDemandPairType"/> is not supported as demand pair generation type.</exception>
        private static CountedList<DemandPair> CreateInputDemandPairs(Random random, Tree<TreeNode> inputTree, InputDemandPairsType inputDemandPairType, int numberOfDemandPairs, Dictionary<(int, int), double> distanceProbability, string filePath)
        {
            return inputDemandPairType switch
            {
                InputDemandPairsType.Fixed => throw new NotImplementedException("Demand pairs with fixed endpoints are not yet supported!"),
                InputDemandPairsType.LengthDistribution => throw new NotImplementedException("Demand pairs with a preferred length distribution are not yet supported!"),
                InputDemandPairsType.Random => new CountedList<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(numberOfDemandPairs, inputTree, random), new Counter()),
                _ => throw new NotSupportedException($"The input demand pair type {inputDemandPairType} is not supported!")
            };
        }

        /// <summary>
        /// Returns an instance of the correct subclass of <see cref="Algorithm"/> given <paramref name="algorithmType"/>.
        /// </summary>
        /// <param name="algorithmType">The <see cref="AlgorithmType"/> the return type should have.</param>
        /// <param name="instance">The instance we are going to solve.</param>
        /// <returns>An instance of a subclass of <see cref="Algorithm"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="algorithmType"/> is not supported as algorithm.</exception>
        private static Algorithm CreateAlgorithmInstance(AlgorithmType algorithmType, MulticutInstance instance)
        {
            return algorithmType switch
            {
                AlgorithmType.GuoNiedermeierKernelisation => new GuoNiedermeierKernelisation(instance),
                _ => throw new NotSupportedException($"The algorithm type {algorithmType} is not supported!")
            };
        }

        /// <summary>
        /// Parses the demand pair length distribution to a dictionary.
        /// </summary>
        /// <param name="commandLineArgument">The dictionary in string format from the command line.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with the requested length distribution for demand pairs.</returns>
        /// <exception cref="ApplicationException">Thrown when the dictionary in the command line is not in the correct format.</exception>
        private static Dictionary<(int, int), double> ParseLengthDistributionDictionary(string commandLineArgument)
        {
            Dictionary<(int, int), double> result = new Dictionary<(int, int), double>();

            if (commandLineArgument is null)
            {
                return result;
            }

            commandLineArgument = commandLineArgument.Replace("(", "");
            commandLineArgument = commandLineArgument.Replace(")", "");
            string[] keyValuePairs = commandLineArgument.Split(", ");

            if (keyValuePairs.Length % 3 != 0)
            {
                throw new ApplicationException("Command line argument for dictionary of length distribution is not valid!");
            }

            for (int i = 0; i < keyValuePairs.Length; i += 3)
            {
                bool parseMin = int.TryParse(keyValuePairs[i], out int min);
                bool parseMax = int.TryParse(keyValuePairs[i + 1], out int max);
                bool parseChance = double.TryParse(keyValuePairs[i + 2], out double chance);
                if (!parseMin || !parseMax || !parseChance)
                {
                    throw new ApplicationException("Command line argument for dictionary of length distribution is not valid!");
                }
                result[(min, max)] = chance;
            }

            return result;
        }
    }
}
