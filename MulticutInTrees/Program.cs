// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees
{
    /// <summary>
    /// The entry class for the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry method for the program.
        /// </summary>
        /// <param name="args">Array with command line arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            if (args.Length == 0)
            {
                args = "--seed=0 --algorithm=GuoNiedermeierKernelisation --maxSolutionSize=400 --tree=Prüfer --dps=Random --nodes=500 --demandpairs=300 -v=true".Split();
            }

            Parser parser = new Parser(p => 
            {
                p.AutoHelp = true;
                p.CaseInsensitiveEnumValues = true;
                p.CaseSensitive = false;
            });

            ParserResult<CommandLineOptions> result = parser.ParseArguments<CommandLineOptions>(args);
            result.WithParsed(RunExperiment).WithNotParsed(e => HandleParseError(result));
        }

        /// <summary>
        /// Run an experiment given the command line options.
        /// </summary>
        /// <param name="options">The options given to the program via the command line.</param>
        /// <exception cref="NotSupportedException">Thrown when there is no experiment that can be executed given these command line options.</exception>
        private static void RunExperiment(CommandLineOptions options)
        {
            Dictionary<(int, int), double> distanceDictionary = ParseLengthDistributionDictionary(options.DistanceDistribution);
            switch (options.AlgorithmType, options.MaxSolutionSize)
            {
                case (AlgorithmType.GuoNiederMeierBranching, 0):
                    FindMinimumSolutionSize(options.RandomSeed, options.InputTreeType, options.NumberOfNodes, options.InstanceFilePath, options.InputDemandPairType, options.NumberOfDemandPairs, distanceDictionary, options.Verbose);
                    break;
                case (AlgorithmType.GuoNiederMeierBranching, _):
                    RunBranchingAlgorithm(options.RandomSeed, options.InputTreeType, options.NumberOfNodes, options.InstanceFilePath, options.InputDemandPairType, options.NumberOfDemandPairs, distanceDictionary, options.MaxSolutionSize, options.Verbose, CancellationToken.None);
                    break;
                case (AlgorithmType.GuoNiedermeierKernelisation, _):
                    RunKernelisationAlgorithm(options.RandomSeed, options.AlgorithmType, options.InputTreeType, options.NumberOfNodes, options.InstanceFilePath, options.InputDemandPairType, options.NumberOfDemandPairs, distanceDictionary, options.MaxSolutionSize, options.Verbose);
                    break;
                default:
                    throw new NotSupportedException($"Trying to run an experiment, but there is no experiment supported with the given command line options.");
            }
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

        /// <summary>
        /// Writes the errors that occurred during the parsing of the command line arguments to the console and stops the execution of the program.
        /// </summary>
        /// <param name="parserResult">The result of the parsing of the command line arguments.</param>
        /// <exception cref="ApplicationException">Thrown to stop the program.</exception>
        private static void HandleParseError(ParserResult<CommandLineOptions> parserResult)
        {
            HelpText helpText = HelpText.AutoBuild(parserResult);
            helpText.AddEnumValuesToHelpText = true;
            helpText.AdditionalNewLineAfterOption = false;
            helpText.AddOptions(parserResult);
            Console.WriteLine(helpText);
            throw new ApplicationException("The command line arguments are not valid. Aborting.");
        }

        /// <summary>
        /// Formats the input the program got from the command line arguments into a nice message.
        /// </summary>
        /// <param name="experimentMessage">The message that is specific to an experiment.</param>
        /// <param name="randomSeed">The seed used for random number generation.</param>
        /// <param name="inputTreeType">The way to generate the input tree.</param>
        /// <param name="numberOfNodes">The number of nodes in the input tree. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="filePath">The path to the file that contains the CNF-SAT or Vertex Cover instance to generate the tree from. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="inputDemandPairType">The way to generate the demand pairs. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="numberOfDemandPairs">The required number of demand pairs. Not necessary for all <see cref="InputDemandPairType"/>s.</param>
        /// <param name="distanceProbability">The distribution on the required distances for demand pairs. Not necessary for all <see cref="InputDemandPairType"/>s.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to have.</param>
        /// <returns>A <see cref="string"/> with a formatted representation of the command line arguments.</returns>
        private static string FormatParseOutput(string experimentMessage, int randomSeed, InputTreeType inputTreeType, int numberOfNodes, string filePath, InputDemandPairType inputDemandPairType, int numberOfDemandPairs, Dictionary<(int, int), double> distanceProbability, int maxSolutionSize = -1)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{experimentMessage} on the following instance: random seed {randomSeed}, ");
            if (maxSolutionSize != -1)
            {
                sb.Append($"a maximum solution size of {maxSolutionSize}, ");
            }
            if (inputTreeType == InputTreeType.Prüfer || inputTreeType == InputTreeType.Caterpillar)
            {
                sb.Append($"a {inputTreeType} tree with {numberOfNodes} nodes, ");
                if (inputDemandPairType == InputDemandPairType.Random)
                {
                    sb.Append($"and {numberOfDemandPairs} randomly generated demand pairs.");
                }
                else
                {
                    sb.Append($"and {numberOfDemandPairs} demand pairs generated using {distanceProbability.Print()} as distribution.");
                }
            }
            else
            {
                sb.Append($"and a tree and demand pairs from the {inputTreeType} instance found here \"{filePath}\".");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Uses the branching algorithm and binary search to determine the smallest possible solution size on this instance.
        /// </summary>
        /// <param name="randomSeed">The seed to use for the random number generation.</param>
        /// <param name="inputTreeType">The way to generate the input tree.</param>
        /// <param name="numberOfNodes">The number of nodes in the input tree. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="filePath">The path to the file that contains the CNF-SAT or Vertex Cover instance to generate the tree from. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="inputDemandPairType">The way to generate the demand pairs. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="numberOfDemandPairs">The required number of demand pairs. Not necessary for all <see cref="InputDemandPairType"/>s.</param>
        /// <param name="distanceProbability">The distribution on the required distances for demand pairs. Not necessary for all <see cref="InputDemandPairType"/>s.</param>
        /// <param name="verbose">Whether additional information should be written to the console.</param>
        /// <returns>The smallest possible solution size on the instance defined by the parametes in this function.</returns>
        private static int FindMinimumSolutionSize(int randomSeed, InputTreeType inputTreeType, int numberOfNodes, string filePath, InputDemandPairType inputDemandPairType, int numberOfDemandPairs, Dictionary<(int, int), double> distanceProbability, bool verbose = false)
        {
            if (verbose) 
            {
                Console.WriteLine(FormatParseOutput("Finding the minimum possible value for the maximum solution size using Guo and Niedermeiers branching algorithm", randomSeed, inputTreeType, numberOfNodes, filePath, inputDemandPairType, numberOfDemandPairs, distanceProbability));
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task<int> binarySearch = Task.Run(() =>
            {
                return Utils.BinarySearchGetFirstTrue(0, Math.Min(numberOfDemandPairs, numberOfNodes - 1), n =>
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    { 
                        return false;
                    }

                    if (verbose)
                    {
                        Console.WriteLine($"Now checking {n}");
                    }
                    return RunBranchingAlgorithm(randomSeed, inputTreeType, numberOfNodes, filePath, inputDemandPairType, numberOfDemandPairs, distanceProbability, n, false, cancellationTokenSource.Token).solved;
                });
            });

            Task<int> branchExact = Task.Run(() =>
            {
                return RunBranchingAlgorithm(randomSeed, inputTreeType, numberOfNodes, filePath, inputDemandPairType, numberOfDemandPairs, distanceProbability, numberOfDemandPairs, true, cancellationTokenSource.Token).solution.Count;
            });

            Task<int>[] tasks = new Task<int>[] { binarySearch, branchExact };
            int firstDoneIndex = Task.WaitAny(tasks);
            cancellationTokenSource.Cancel();
            int smallestSolutionSize = tasks[firstDoneIndex].Result;

            if (verbose)
            {
                string name = "Exact Branching";
                if (firstDoneIndex == 0)
                {
                    name = "Binary Search";
                }

                Console.WriteLine($"Task {name} finished first. The smallest found solution size is {smallestSolutionSize}!");
            }

            return smallestSolutionSize;
        }

        /// <summary>
        /// Runs an experiment with the branching algorithm.
        /// </summary>
        /// <param name="randomSeed">The seed to use for the random number generation.</param>
        /// <param name="inputTreeType">The way to generate the input tree.</param>
        /// <param name="numberOfNodes">The number of nodes in the input tree. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="filePath">The path to the file that contains the CNF-SAT or Vertex Cover instance to generate the tree from. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="inputDemandPairType">The way to generate the demand pairs. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="numberOfDemandPairs">The required number of demand pairs. Not necessary for all <see cref="InputDemandPairType"/>s.</param>
        /// <param name="distanceProbability">The distribution on the required distances for demand pairs. Not necessary for all <see cref="InputDemandPairType"/>s.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to have.</param>
        /// <param name="findSmallest">Whether we should find a solution with the smallest possible size, or just a solution with size at most k.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that will be regularly checked and can stop the algorithm prematurely.</param>
        /// <param name="verbose">Whether additional information should be written to the console.</param>
        /// <returns><see langword="true"/> if the instance could be solved, <see langword="false"/> otherwise.</returns>
        private static (bool solved, List<(TreeNode, TreeNode)> solution) RunBranchingAlgorithm(int randomSeed, InputTreeType inputTreeType, int numberOfNodes, string filePath, InputDemandPairType inputDemandPairType, int numberOfDemandPairs, Dictionary<(int, int), double> distanceProbability, int maxSolutionSize, bool findSmallest, CancellationToken cancellationToken, bool verbose = false)
        {
            if (verbose)
            {
                string smallest;
                if (findSmallest)
                {
                    smallest = ", trying to find the smallest possible solution,";
                }
                else
                {
                    smallest = ", trying to find a solution with size at most k,";
                }
                Console.WriteLine(FormatParseOutput($"Running Guo and Niedermeiers branching algorithm{smallest}", randomSeed, inputTreeType, numberOfNodes, filePath, inputDemandPairType, numberOfDemandPairs, distanceProbability, maxSolutionSize));
            }

            Random treeRandom = new Random(randomSeed);
            Random demandPairRandom = new Random(randomSeed);
            Tree<TreeNode> tree = CreateInputTree(inputTreeType, treeRandom, numberOfNodes, filePath);
            CountedList<DemandPair> demandPairs = CreateInputDemandPairs(demandPairRandom, tree, inputDemandPairType, numberOfDemandPairs, distanceProbability);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSolutionSize);
            GuoNiedermeierBranching gnBranching = new GuoNiedermeierBranching(instance);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            (bool solved, List<(TreeNode, TreeNode)> solution) = gnBranching.Run(findSmallest, cancellationToken);
            stopwatch.Stop();

            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine("Branching algorithm result:");
                Console.WriteLine($"Solved?          {solved}");
                Console.WriteLine($"Solution size:   {solution.Count}");
                Console.WriteLine($"Time required:   {stopwatch.ElapsedTicks}");
                Console.WriteLine($"Entire solution: {solution.Print()}");
                Console.WriteLine();
            }

            return (solved, solution);
        }

        /// <summary>
        /// Runs an experiment with a kernelisation algorithm.
        /// </summary>
        /// <param name="randomSeed">The seed to use for the random number generation.</param>
        /// <param name="algorithmType">The type of algorithm to run.</param>
        /// <param name="inputTreeType">The way to generate the input tree.</param>
        /// <param name="numberOfNodes">The number of nodes in the input tree. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="filePath">The path to the file that contains the CNF-SAT or Vertex Cover instance to generate the tree from. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="inputDemandPairType">The way to generate the demand pairs. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="numberOfDemandPairs">The required number of demand pairs. Not necessary for all <see cref="InputDemandPairType"/>s.</param>
        /// <param name="distanceProbability">The distribution on the required distances for demand pairs. Not necessary for all <see cref="InputDemandPairType"/>s.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to have.</param>
        /// <param name="verbose">Whether additional information should be written to the console.</param>
        /// <returns><see langword="true"/> if the instance could be solved, <see langword="false"/> otherwise.</returns>
        private static bool RunKernelisationAlgorithm(int randomSeed, AlgorithmType algorithmType, InputTreeType inputTreeType, int numberOfNodes, string filePath, InputDemandPairType inputDemandPairType, int numberOfDemandPairs, Dictionary<(int, int), double> distanceProbability, int maxSolutionSize, bool verbose = false)
        {
            if (verbose)
            {
                Console.WriteLine(FormatParseOutput($"Running the {algorithmType} algorithm", randomSeed, inputTreeType, numberOfNodes, filePath, inputDemandPairType, numberOfDemandPairs, distanceProbability, maxSolutionSize));
            }

            Random treeRandom = new Random(randomSeed);
            Random demandPairRandom = new Random(randomSeed);
            Tree<TreeNode> tree = CreateInputTree(inputTreeType, treeRandom, numberOfNodes, filePath);
            CountedList<DemandPair> demandPairs = CreateInputDemandPairs(demandPairRandom, tree, inputDemandPairType, numberOfDemandPairs, distanceProbability);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSolutionSize);
            Algorithm algorithm = CreateAlgorithmInstance(algorithmType, instance);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            (bool solved, Tree<TreeNode> _, List<(TreeNode, TreeNode)> partialSolution, List<DemandPair> finalDemandPairs) = algorithm.Run();
            stopwatch.Stop();

            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine($"FPT algorithm ({algorithm.GetType()}) result:");
                Console.WriteLine($"Solved?                           {solved}");
                Console.WriteLine($"Partial solution size:            {partialSolution.Count}");
                Console.WriteLine($"Remaining tree:                   {tree}");
                Console.WriteLine($"Remaining number of demand pairs: {finalDemandPairs.Count}");
                Console.WriteLine($"Time:                             {stopwatch.Elapsed}");
                Console.WriteLine($"Entire partial solution:          {partialSolution.Print()}");
                Console.WriteLine($"Remaining edges:                  {tree.Edges(new Counter()).Print()}");
                Console.WriteLine($"Remaining dps:                    {finalDemandPairs.Print()}");
                Console.WriteLine();
            }

            return solved;
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
                InputTreeType.Caterpillar => throw new NotImplementedException("Caterpillars are not yet supported!"),
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
        /// <param name="inputDemandPairType">The <see cref="InputDemandPairType"/> that says which method to use to create the <see cref="DemandPair"/>s.</param>
        /// <param name="numberOfDemandPairs">The required number of <see cref="DemandPair"/>s.</param>
        /// <param name="distanceProbability">A <see cref="Dictionary{TKey, TValue}"/> from a lower and upperbound on a distance to the probability of choosing that distance for the length of a <see cref="DemandPair"/>. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <returns>A <see cref="CountedList{T}"/> with <see cref="DemandPair"/>s that were generated according to the method given by <paramref name="inputDemandPairType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="inputDemandPairType"/> is not supported as demand pair generation type.</exception>
        private static CountedList<DemandPair> CreateInputDemandPairs(Random random, Tree<TreeNode> inputTree, InputDemandPairType inputDemandPairType, int numberOfDemandPairs, Dictionary<(int, int), double> distanceProbability)
        {
            return inputDemandPairType switch
            {
                InputDemandPairType.LengthDistribution => throw new NotImplementedException("Demand pairs witgh a preferred length distribution are not yet supported!"),
                InputDemandPairType.Random => new CountedList<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(numberOfDemandPairs, inputTree, random), new Counter()),
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
    }
}