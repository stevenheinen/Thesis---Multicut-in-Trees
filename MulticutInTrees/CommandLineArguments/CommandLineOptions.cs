// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using CommandLine;
using MulticutInTrees.Algorithms;
using MulticutInTrees.InstanceGeneration;

namespace MulticutInTrees.CommandLineArguments
{
    /// <summary>
    /// Class containing all command line options for this project.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// The seed to use for the random number generator that generates the tree for the experiments. Note: when there are multiple repetitions, the seed used is this argument + the repetition (0-based). For example, with seed 0 and 5 repetitions, the experiments use seeds 0, 1, 2, 3 and 4 respectively.
        /// </summary>
        [Option("treeSeed", HelpText = "The seed to use for the random number generator that generates the tree for the experiments. Note: when there are multiple experiments, the seed used is this argument + the repetition (0-based). For example, with seed 0 and 5 repetitions, the experiments use seeds 0, 1, 2, 3 and 4 respectively.")]
        public int TreeSeed { get; set; }

        /// <summary>
        /// The seed to use for the random number generator that generates the demand pairs for the experiments. Note: when there are multiple repetitions, the seed used is this argument + the repetition (0-based). For example, with seed 0 and 5 repetitions, the experiments use seeds 0, 1, 2, 3 and 4 respectively.
        /// </summary>
        [Option("dpSeed", HelpText = "The seed to use for the random number generator that generates the demand pairs for the experiments. Note: when there are multiple experiments, the seed used is this argument + the repetition (0-based). For example, with seed 0 and 5 repetitions, the experiments use seeds 0, 1, 2, 3 and 4 respectively.")]
        public int DemandPairSeed { get; set; }

        /// <summary>
        /// The number of experiments to run with these settings. Each experiment starts with its own random number generator, that is seeded as the seed argument plus the number of the current experiment. Only works for generated input. When input is read from a file, this option will be used as a multiplier for the number of repetitions per experiment
        /// </summary>
        [Option("experiments", Default = 1, HelpText = "The number of experiments to run with these settings. Each experiment starts with its own random number generator, that is seeded as the seed argument plus the number of the current experiment. Only works for generated input. When input is read from a file, this option will be used as a multiplier for the number of repetitions per experiment.")]
        public int Experiments { get; set; }

        /// <summary>
        /// The number of repetitions to run per experiment.
        /// </summary>
        [Option("repetitions", Default = 1, HelpText = "The number of repetitions to run per experiment.")]
        public int Repetitions { get; set; }

        /// <summary>
        /// The type of algorithm to run.
        /// </summary>
        [Option("algorithm", Required = true, HelpText = "The algorithm to use in this experiment.")]
        public AlgorithmType AlgorithmType { get; set; }

        /// <summary>
        /// How to generate the tree for this instance.
        /// </summary>
        [Option("treeType", Required = true, HelpText = "The way to generate the input tree in this experiment.")]
        public InputTreeType InputTreeType { get; set; }

        /// <summary>
        /// How to generate the demand pairs for this instance.
        /// </summary>
        [Option("dpType", Required = true, HelpText = "The way to generate the input demand pairs in this experiment.")]
        public InputDemandPairsType InputDemandPairsType { get; set; }

        /// <summary>
        /// The number of nodes in this instance.
        /// </summary>
        [Option("nrNodes", HelpText = "The number of nodes in this instance. Only necessary for Prüfer or Caterpillar input tree types.")]
        public int NumberOfNodes { get; set; }

        /// <summary>
        /// The path to the file with either the CNF-SAT or Vertex Cover instance, or the fixed tree. Only necessary for CNFSAT, VertexCover or Fixed input tree types.
        /// </summary>
        [Option("instanceFilePath", Default = "", HelpText = "The path to the file with either the CNF-SAT or Vertex Cover instance, or the fixed tree. Only necessary for CNFSAT, VertexCover or Fixed input tree types.")]
        public string InstanceFilePath { get; set; }

        /// <summary>
        /// The path to the file with the endpoints of the demand pairs. Only necessary for fixed input demand pairs.
        /// </summary>
        [Option("dpFile", Default = "", HelpText = "The path to the file with the endpoints of the demand pairs. Only necessary for fixed input demand pairs.")]
        public string DemandPairFilePath { get; set; }

        /// <summary>
        /// The number of Demand Pairs in the instance.
        /// </summary>
        [Option("nrDPs", HelpText = "The number of demand pairs to generate. Only necessary for Prüfer or Caterpillar input trees.")]
        public int NumberOfDemandPairs { get; set; }

        /// <summary>
        /// The preferred length distribution of the demand pairs.
        /// </summary>
        [Option("distanceDistribution", HelpText = "The distance distribution for the demand pairs. Only necessary for Prüfer or Caterpillar input trees and LengthDistribution input demand pair types. The argument should have any number of \"values\", separated by commas, with a value looking like: (min, max, percentage). For example: \"(0, 5, 0.30), (2, 8, 0.25), (9, 350, 0.45)\".")]
        public string DistanceDistribution { get; set; }

        /// <summary>
        /// The maximum size the solution is allowed to be. If this value is 0 or smaller, the optimal possible value will be used.
        /// </summary>
        [Option("maxSolutionSize", HelpText = "The maximum size the solution is allowed to be. If this value is 0 or smaller, the optimal possible value will be used.")]
        public int MaxSolutionSize { get; set; }

        /// <summary>
        /// The directory where the output of the experiments should be saved.
        /// </summary>
        [Option("outputDir", Required = true, HelpText = "The directory where the output of the experiments should be saved.")]
        public string OutputDirectory { get; set; }

        /// <summary>
        /// The directory where the instances are stored or should be stored.
        /// </summary>
        [Option("instanceDir", Required = true, HelpText = "The directory where the instances are stored or should be stored.")]
        public string InstanceDirectory { get; set; }

        /// <summary>
        /// The maximum time that can be spent by the MIP solver to compute the optimal solution.
        /// </summary>
        [Option("mipTimeLimit", Default = 21600.0, HelpText = "The maximum time that can be spent by the MIP solver to compute the optimal solution.")]
        public double MIPTimeLimit { get; set; }

        /// <summary>
        /// Whether to overwrite existing instances if they already exist.
        /// </summary>
        [Option("overwrite", HelpText = "Whether to overwrite existing instances if they already exist.")]
        public bool Overwrite { get; set; }

        /// <summary>
        /// Whether the program should run in debug mode.
        /// </summary>
        [Option('v', "verbose", HelpText = "Whether the program should run in verbose mode.")]
        public bool Verbose { get; set; }
    }
}
