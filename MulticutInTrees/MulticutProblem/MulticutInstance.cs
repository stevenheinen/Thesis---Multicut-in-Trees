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
        /// The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.
        /// </summary>
        public int TreeSeed { get; }

        /// <summary>
        /// The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.
        /// </summary>
        public int DPSeed { get; }

        /// <summary>
        /// The name of the file from which the tree was generated.
        /// </summary>
        public string TreeFileName { get; }

        /// <summary>
        /// The name of the file from which the <see cref="DemandPair"/>s were generated.
        /// </summary>
        public string DPFileName { get; }

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
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="treeFileName">The name of the file from which the tree was generated.</param>
        /// <param name="dpFileName">The name of the file from which the <see cref="DemandPair"/>s were generated.</param>
        /// <param name="tree">The tree of in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="k">The size the cutset is allowed to be.</param>
        /// <param name="optimalK">The minimum possible size the cutset can be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="demandPairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="k"/> is smaller than zero.</exception>
        internal MulticutInstance(InputTreeType treeType, InputDemandPairsType dpType, int treeSeed, int dpSeed, string treeFileName, string dpFileName, Graph tree, CountedCollection<DemandPair> demandPairs, int k, int optimalK)
        {
            Counter mockCounter = new();
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
            TreeSeed = treeSeed;
            DPSeed = dpSeed;
            TreeFileName = treeFileName;
            DPFileName = dpFileName;
            Tree = tree;
            DemandPairs = demandPairs;
            K = k;
            OptimalK = optimalK;
        }

        /// <summary>
        /// Constructor for a <see cref="MulticutInstance"/>. Checks using <paramref name="options"/> whether the instance already exists. If so, grabs it from the files. If not, it creates it and writes it to the files.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> given to the program.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public MulticutInstance(int treeSeed, int dpSeed, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(options, nameof(options), "Trying to create a multicut instance, but the commandline options are null!");
#endif
            Counter mockCounter = new();

            // Try to read the instance from the files, or create a new one if we want to overwrite the existing instance.
            (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) = options.Overwrite ? CreateInstance(treeSeed, dpSeed, options) : InstanceReaderWriter.ReadInstance(treeSeed, dpSeed, options);

            if (tree is null)
            {
                // If the instance does not exist in the files, create it.
                (tree, demandPairs, optimalK) = CreateInstance(treeSeed, dpSeed, options);
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
            TreeSeed = treeSeed;
            DPSeed = dpSeed;
            TreeFileName = options.InstanceFilePath.Split("\\")[^1];
            DPFileName = options.DemandPairFilePath.Split("\\")[^1];
            Tree = tree;
            DemandPairs = demandPairs;
            K = options.MaxSolutionSize > 0 ? options.MaxSolutionSize : optimalK;
            OptimalK = optimalK;
        }

        /// <summary>
        /// The instance corresponding to <paramref name="options"/> does not exist yet, so we will create it here.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> given to the program.</param>
        /// <returns>A tuple with a <see cref="Graph"/>, a <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s and the optimal K parameter value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private static (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) CreateInstance(int treeSeed, int dpSeed, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(options, nameof(options), "Trying to create a multicut instance, but the commandline options are null!");
#endif
            if (options.Verbose)
            {
                Console.WriteLine("Instance not yet found in the instance files, creating it!");
            }

            (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) = CreateInputTreeAndDemandPairs(treeSeed, dpSeed, options);

            InstanceReaderWriter.WriteInstance(treeSeed, dpSeed, options, tree, demandPairs.GetLinkedList(), optimalK);

            if (options.Verbose)
            {
                Console.WriteLine($"Instance created! Minimum possible solution size: {optimalK}. This experiment is using {(options.MaxSolutionSize == 0 ? optimalK : options.MaxSolutionSize)}.");
            }

            return (tree, demandPairs, optimalK);
        }

        /// <summary>
        /// Create the input <see cref="Graph"/> and <see cref="DemandPair"/>s given the commandline options.
        /// </summary>
        /// <param name="treeSeed">The seed for the current random number generator for the <see cref="Graph"/>.</param>
        /// <param name="dpSeed">The seed for the current random number generator for the <see cref="DemandPair"/>s.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/>.</param>
        /// <returns>A tuple with a <see cref="Graph"/>, a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s, and the optimal K value for this instance.</returns>
        /// <exception cref="ApplicationException">Thrown when an invalid combination of <see cref="CommandLineOptions.InputTreeType"/> and <see cref="CommandLineOptions.InputDemandPairsType"/> is provided.</exception>
        /// <exception cref="Exception">Thrown when Gurobi finds a differently sized solution than expected when creating <see cref="DemandPair"/>s through a known solution.</exception>
        private static (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) CreateInputTreeAndDemandPairs(int treeSeed, int dpSeed, CommandLineOptions options)
        {
            switch (options.InputTreeType, options.InputDemandPairsType)
            {
                case (InputTreeType.Prufer, InputDemandPairsType.Random):
                    {
                        Graph tree = TreeFromPruferSequence.GenerateTree(options.NumberOfNodes, new Random(treeSeed));
                        CountedCollection<DemandPair> demandPairs = new(RandomDemandPairs.GenerateRandomDemandPairs(options.NumberOfDemandPairs, tree, new Random(dpSeed)), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Prufer, InputDemandPairsType.LengthDistribution):
                    {
                        Graph tree = TreeFromPruferSequence.GenerateTree(options.NumberOfNodes, new Random(treeSeed));
                        Dictionary<(int, int), double> distanceDistribution = ParseLengthDistributionDictionary(options.DistanceDistribution);
                        CountedCollection<DemandPair> demandPairs = new(LengthDistributionDemandPairs.CreateDemandPairs(options.NumberOfDemandPairs, tree, new Random(dpSeed), distanceDistribution), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Prufer, InputDemandPairsType.ThroughKnownSolution):
                    {
                        Graph tree = TreeFromPruferSequence.GenerateTree(options.NumberOfNodes, new Random(treeSeed));
                        CountedCollection<DemandPair> demandPairs = new(KnownSolutionDemandPairs.GenerateDemandPairsThroughKnownSolution(tree, options.NumberOfDemandPairs, options.MaxSolutionSize, new Random(dpSeed)), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Caterpillar, InputDemandPairsType.Random):
                    {
                        Graph tree = CaterpillarGenerator.CreateCaterpillar(options.NumberOfNodes, new Random(treeSeed));
                        CountedCollection<DemandPair> demandPairs = new(RandomDemandPairs.GenerateRandomDemandPairs(options.NumberOfDemandPairs, tree, new Random(dpSeed)), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Caterpillar, InputDemandPairsType.LengthDistribution):
                    {
                        Graph tree = CaterpillarGenerator.CreateCaterpillar(options.NumberOfNodes, new Random(treeSeed));
                        Dictionary<(int, int), double> distanceDistribution = ParseLengthDistributionDictionary(options.DistanceDistribution);
                        CountedCollection<DemandPair> demandPairs = new(LengthDistributionDemandPairs.CreateDemandPairs(options.NumberOfDemandPairs, tree, new Random(dpSeed), distanceDistribution), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Caterpillar, InputDemandPairsType.ThroughKnownSolution):
                    {
                        Graph tree = CaterpillarGenerator.CreateCaterpillar(options.NumberOfNodes, new Random(treeSeed));
                        CountedCollection<DemandPair> demandPairs = new(KnownSolutionDemandPairs.GenerateDemandPairsThroughKnownSolution(tree, options.NumberOfDemandPairs, options.MaxSolutionSize, new Random(dpSeed)), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Degree3Tree, InputDemandPairsType.Random):
                    {
                        Graph tree = Degree3TreeGenerator.CreateDegree3Tree(options.NumberOfNodes);
                        CountedCollection<DemandPair> demandPairs = new(RandomDemandPairs.GenerateRandomDemandPairs(options.NumberOfDemandPairs, tree, new Random(dpSeed)), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Degree3Tree, InputDemandPairsType.LengthDistribution):
                    {
                        Graph tree = Degree3TreeGenerator.CreateDegree3Tree(options.NumberOfNodes);
                        Dictionary<(int, int), double> distanceDistribution = ParseLengthDistributionDictionary(options.DistanceDistribution);
                        CountedCollection<DemandPair> demandPairs = new(LengthDistributionDemandPairs.CreateDemandPairs(options.NumberOfDemandPairs, tree, new Random(dpSeed), distanceDistribution), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Degree3Tree, InputDemandPairsType.ThroughKnownSolution):
                    {
                        Graph tree = Degree3TreeGenerator.CreateDegree3Tree(options.NumberOfNodes);
                        CountedCollection<DemandPair> demandPairs = new(KnownSolutionDemandPairs.GenerateDemandPairsThroughKnownSolution(tree, options.NumberOfDemandPairs, options.MaxSolutionSize, new Random(dpSeed)), new Counter());
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.VertexCover, InputDemandPairsType.FromTreeInstance):
                    {
                        (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) = InstanceFromVertexCover.GenerateInstance(options);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.CNFSAT, InputDemandPairsType.FromTreeInstance):
                    {
                        (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) = InstanceFromCNFSAT.GenerateInstance(options);
                        return (tree, demandPairs, optimalK);
                    }
                case (InputTreeType.Fixed, InputDemandPairsType.Fixed):
                    {
                        Graph tree = FixedTreeReader.ReadTree(options.InstanceFilePath);
                        CountedCollection<DemandPair> demandPairs = FixedDemandPairsReader.ReadDemandPairs(tree, options.DemandPairFilePath);
                        GurobiMIPAlgorithm algorithm = new(tree, demandPairs.GetLinkedList());
                        int optimalK = algorithm.Run(options.MIPTimeLimit, options.Verbose);
                        return (tree, demandPairs, optimalK);
                    }
                default:
                    throw new ApplicationException($"The provided combination of input tree ({options.InputTreeType}) and demand pair ({options.InputDemandPairsType}) types is not valid. Please choose from ({InputTreeType.Prufer}, {InputDemandPairsType.Random}), ({InputTreeType.Prufer}, {InputDemandPairsType.LengthDistribution}), ({InputTreeType.Caterpillar}, {InputDemandPairsType.Random}), ({InputTreeType.Caterpillar}, {InputDemandPairsType.LengthDistribution}), ({InputTreeType.VertexCover}, {InputDemandPairsType.FromTreeInstance}), ({InputTreeType.CNFSAT}, {InputDemandPairsType.FromTreeInstance}), or , ({InputTreeType.Fixed}, {InputDemandPairsType.Fixed})");
            }
        }

        /// <summary>
        /// Parses the demand pair length distribution to a dictionary.
        /// </summary>
        /// <param name="commandLineArgument">The dictionary in string format from the command line.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with the requested length distribution for demand pairs.</returns>
        /// <exception cref="ArgumentException">Thrown when the dictionary in the command line is not in the correct format.</exception>
        private static Dictionary<(int, int), double> ParseLengthDistributionDictionary(string commandLineArgument)
        {
            Dictionary<(int, int), double> result = new();

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
