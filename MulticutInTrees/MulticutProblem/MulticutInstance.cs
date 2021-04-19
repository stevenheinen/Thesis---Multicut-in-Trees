// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;

namespace MulticutInTrees.MulticutProblem
{
    /// <summary>
    /// Class that represents an instance for the Multicut problem.
    /// </summary>
    public class MulticutInstance
    {
        /// <summary>
        /// The original number of nodes in the instance.
        /// </summary>
        public int NumberOfNodes { get; }

        /// <summary>
        /// The original number of <see cref="DemandPair"/>s in the instance.
        /// </summary>
        public int NumberOfDemandPairs { get; }

        /// <summary>
        /// The <see cref="InputTreeType"/> used to generate the tree in the instance.
        /// </summary>
        public InputTreeType TreeType { get; }

        /// <summary>
        /// The <see cref="InputDemandPairsType"/> used to generate the <see cref="DemandPair"/>s in the instance.
        /// </summary>
        public InputDemandPairsType DPType { get; }

        /// <summary>
        /// The seed used for the random number generator in the instance.
        /// </summary>
        public int RandomSeed { get; }

        /// <summary>
        /// The input <see cref="Tree"/>.
        /// </summary>
        public Graph Tree { get; }

        /// <summary>
        /// The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.
        /// </summary>
        public CountedCollection<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The maximum size the cutset is allowed to be.
        /// </summary>
        public int K { get; }

        /// <summary>
        /// The minimum possible solution size for this instance.
        /// </summary>
        public int OptimalK { get; }

        /// <summary>
        /// Constructor for a <see cref="MulticutInstance"/> with an already computed tree and list of demand pairs.
        /// </summary>
        /// <param name="treeType">The <see cref="InputTreeType"/> used to generate the tree in the instance.</param>
        /// <param name="dpType">The <see cref="InputDemandPairsType"/> used to generate the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="randomSeed">The seed used for the random number generator in the instance.</param>
        /// <param name="tree">The tree of in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="k">The size the cutset is allowed to be.</param>
        /// <param name="optimalK">The minimum possible size the cutset can be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="demandPairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="k"/> is smaller than zero.</exception>
        internal MulticutInstance(InputTreeType treeType, InputDemandPairsType dpType, int randomSeed, Graph tree, CountedCollection<DemandPair> demandPairs, int k, int optimalK)
        {
            Counter mockCounter = new Counter();
#if !EXPERIMENT
            Utilities.Utils.NullCheck(tree, nameof(tree), "Trying to create a multicut instance, but the tree is null!");
            Utilities.Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create a multicut instance, but the list of demand pairs is null!");
            if (k < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(k), "Trying to create a multicut instance, but the maximum amount of edges that can be removed is smaller than zero!");
            }
            if (!tree.IsTree(mockCounter))
            {
                throw new ArgumentException("Trying to create a multicut instance, but the provided graph is not a tree!");
            }
#endif
            NumberOfNodes = tree.NumberOfNodes(mockCounter);
            NumberOfDemandPairs = demandPairs.Count(mockCounter);
            TreeType = treeType;
            DPType = dpType;
            RandomSeed = randomSeed;
            Tree = tree;
            DemandPairs = demandPairs;
            K = k;
            OptimalK = optimalK;
        }

        /// <summary>
        /// Constructor for a <see cref="MulticutInstance"/>. Checks using <paramref name="options"/> whether the instance already exists. If so, grabs it from the files. If not, it creates it and writes it to the files.
        /// </summary>
        /// <param name="randomSeed">The seed used for the random number generator in the instance.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> given to the program.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public MulticutInstance(int randomSeed, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(options, nameof(options), "Trying to create a multicut instance, but the commandline options are null!");
#endif
            Counter mockCounter = new Counter();

            // Try to read the instance from the files
            (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) = InstanceReaderWriter.ReadInstance(randomSeed, options);

            if (tree is null)
            {
                // If the instance does not exist in the files, create it.
                (tree, demandPairs, optimalK) = CreateInstance(randomSeed, options);
            }
            else if (options.Verbose)
            {
                int usedK = options.MaxSolutionSize > 0 ? options.MaxSolutionSize : optimalK;
                Console.WriteLine($"Found the instance in the instance files! Minimum possible solution size: {optimalK}. This experiment is using {usedK}.");
            }

            NumberOfNodes = tree.NumberOfNodes(mockCounter);
            NumberOfDemandPairs = demandPairs.Count(mockCounter);
            TreeType = options.InputTreeType;
            DPType = options.InputDemandPairsType;
            RandomSeed = randomSeed;
            Tree = tree;
            DemandPairs = demandPairs;
            K = options.MaxSolutionSize > 0 ? options.MaxSolutionSize : optimalK;
            OptimalK = optimalK;
        }

        /// <summary>
        /// The instance corresponding to <paramref name="options"/> does not exist yet, so we will create it here.
        /// </summary>
        /// <param name="randomSeed">The seed used for the random number generator in the instance.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> given to the program.</param>
        /// <returns>A tuple with a <see cref="Graph"/>, a <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s and the optimal K parameter value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) CreateInstance(int randomSeed, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(options, nameof(options), "Trying to create a multicut instance, but the commandline options are null!");
#endif
            if (options.Verbose)
            {
                Console.WriteLine("Instance not yet found in the instance files, creating it!");
            }

            Random treeRandom = new Random(randomSeed);
            Random demandPairRandom = new Random(randomSeed);
            Graph tree = CreateInputTree(options.InputTreeType, treeRandom, options.NumberOfNodes, options.InstanceFilePath);
            Dictionary<(int, int), double> distanceDistribution = ParseLengthDistributionDictionary(options.DistanceDistribution);
            CountedCollection<DemandPair> demandPairs = CreateInputDemandPairs(demandPairRandom, tree, options.InputDemandPairsType, options.NumberOfDemandPairs, distanceDistribution, options.DemandPairFilePath);

            GurobiMIPAlgorithm algorithm = new GurobiMIPAlgorithm(tree, demandPairs.GetLinkedList());
            int optimalK = algorithm.Run(options.Verbose);

            InstanceReaderWriter.WriteInstance(randomSeed, options, tree, demandPairs.GetLinkedList(), optimalK);

            if (options.Verbose)
            {
                Console.WriteLine($"Instance created! Minimum possible solution size: {optimalK}. This experiment is using {options.MaxSolutionSize}.");
            }

            return (tree, demandPairs, optimalK);
        }

        /// <summary>
        /// Create a <see cref="Graph"/> using the method given in <paramref name="inputTreeType"/>.
        /// </summary>
        /// <param name="inputTreeType">The <see cref="InputTreeType"/> that says which method to use to create the tree.</param>
        /// <param name="random">The <see cref="Random"/> to be used for random number generation. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <param name="numberOfNodes">The number of nodes in the resulting <see cref="AbstractGraph{TEdge, TNode}"/>. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <param name="filePath">The path to the file with the CNF-SAT / Vertex Cover instance to generate the <see cref="AbstractGraph{TEdge, TNode}"/> from. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <returns>A <see cref="Graph"/> that is generated according to <paramref name="inputTreeType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="inputTreeType"/> is its default value: <see cref="InputTreeType.None"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="inputTreeType"/> is not supported as tree generation type.</exception>
        private Graph CreateInputTree(InputTreeType inputTreeType, Random random, int numberOfNodes, string filePath)
        {
#if !EXPERIMENT
            if (inputTreeType == InputTreeType.None)
            {
                throw new ArgumentException("Trying to create a tree for an experiment, but the treetype is null!", nameof(inputTreeType));
            }
#endif
            return inputTreeType switch
            {
                InputTreeType.Caterpillar => CaterpillarGenerator.CreateCaterpillar(numberOfNodes, random),
                InputTreeType.CNFSAT => throw new NotImplementedException("CNF-SAT instances are not yet supported!"),
                InputTreeType.Prufer => TreeFromPruferSequence.GenerateTree(numberOfNodes, random),
                InputTreeType.VertexCover => throw new NotImplementedException("Vertex Cover instances are not yet supported!"),
                InputTreeType.Fixed => FixedTreeReader.ReadTree(filePath),
                _ => throw new NotSupportedException($"The input tree type {inputTreeType} is not supported!")
            };
        }

        /// <summary>
        /// Create a <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s using the method given in <see cref="InputTreeType"/>.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> to be used for random number generation.</param>
        /// <param name="inputTree">The <see cref="Graph"/> in which to generate the <see cref="DemandPair"/>s.</param>
        /// <param name="inputDemandPairsType">The <see cref="InputDemandPairsType"/> that says which method to use to create the <see cref="DemandPair"/>s.</param>
        /// <param name="numberOfDemandPairs">The required number of <see cref="DemandPair"/>s.</param>
        /// <param name="distanceProbability">A <see cref="Dictionary{TKey, TValue}"/> from a lower and upperbound on a distance to the probability of choosing that distance for the length of a <see cref="DemandPair"/>. Not required for all <see cref="InputTreeType"/>s.</param>
        /// <param name="filePath">The path to the file with the endpoints of the the <see cref="DemandPair"/>s. Only required for the <see cref="InputDemandPairsType.Fixed"/> type.</param>
        /// <returns>A <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s that were generated according to the method given by <paramref name="inputDemandPairsType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="inputDemandPairsType"/> is its default value: <see cref="InputDemandPairsType.None"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="inputDemandPairsType"/> is not supported as demand pair generation type.</exception>
        private CountedCollection<DemandPair> CreateInputDemandPairs(Random random, Graph inputTree, InputDemandPairsType inputDemandPairsType, int numberOfDemandPairs, Dictionary<(int, int), double> distanceProbability, string filePath)
        {
#if !EXPERIMENT
            if (inputDemandPairsType == InputDemandPairsType.None)
            {
                throw new ArgumentException("Trying to create the demand pairs for an experiment, but the type (how to generate) demand pairs is null!", nameof(inputDemandPairsType));
            }
#endif
            return inputDemandPairsType switch
            {
                InputDemandPairsType.Fixed => FixedDemandPairsReader.ReadDemandPairs(inputTree, filePath),
                InputDemandPairsType.LengthDistribution => throw new NotImplementedException("Demand pairs with a preferred length distribution are not yet supported!"),
                InputDemandPairsType.Random => new CountedCollection<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(numberOfDemandPairs, inputTree, random), new Counter()),
                _ => throw new NotSupportedException($"The input demand pair type {inputDemandPairsType} is not supported!")
            };
        }

        /// <summary>
        /// Parses the demand pair length distribution to a dictionary.
        /// </summary>
        /// <param name="commandLineArgument">The dictionary in string format from the command line.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with the requested length distribution for demand pairs.</returns>
        /// <exception cref="ArgumentException">Thrown when the dictionary in the command line is not in the correct format.</exception>
        private Dictionary<(int, int), double> ParseLengthDistributionDictionary(string commandLineArgument)
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
                throw new ArgumentException("Command line argument for dictionary of length distribution is not valid!", nameof(commandLineArgument));
            }

            for (int i = 0; i < keyValuePairs.Length; i += 3)
            {
                bool parseMin = int.TryParse(keyValuePairs[i], out int min);
                bool parseMax = int.TryParse(keyValuePairs[i + 1], out int max);
                bool parseChance = double.TryParse(keyValuePairs[i + 2], out double chance);
                if (!parseMin || !parseMax || !parseChance)
                {
                    throw new ArgumentException("Command line argument for dictionary of length distribution is not valid!", nameof(commandLineArgument));
                }
                result[(min, max)] = chance;
            }

            return result;
        }
    }
}
