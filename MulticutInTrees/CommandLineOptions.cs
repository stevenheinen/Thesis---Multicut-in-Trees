// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using MulticutInTrees.Algorithms;
using MulticutInTrees.InstanceGeneration;

namespace MulticutInTrees
{
    /// <summary>
    /// Class containing all command line options for this project.
    /// </summary>
    internal class CommandLineOptions
    {
        /// <summary>
        /// The seed to use for the random number generator.
        /// </summary>
        [Option("seed", HelpText = "The seed to use in this experiment for the random number generator.")]
        public int RandomSeed { get; set; }

        /// <summary>
        /// The type of algorithm to run.
        /// </summary>
        [Option("algorithm", Required = true, HelpText = "The algorithm to use in this experiment.")]
        public AlgorithmType AlgorithmType { get; set; }

        /// <summary>
        /// How to generate the tree for this instance.
        /// </summary>
        [Option("tree", Required = true, HelpText = "The way to generate the input tree in this experiment.")]
        public InputTreeType InputTreeType { get; set; }

        /// <summary>
        /// How to generate the demand pairs for this instance.
        /// </summary>
        [Option("dps", Required = true, HelpText = "The way to generate the input demand pairs in this experiment.")]
        public InputDemandPairType InputDemandPairType { get; set; }

        /// <summary>
        /// The number of nodes in this instance.
        /// </summary>
        [Option("nodes", HelpText = "The number of nodes in this instance. Only necessary for Prüfer or Caterpillar input tree types.")]
        public int NumberOfNodes { get; set; }

        /// <summary>
        /// The path to the file with the CNF-SAT or VC instance to generate the input tree from.
        /// </summary>
        [Option("file", HelpText = "The path to the file with either the CNF-SAT or Vertex Cover instance. Only necessary for CNFSAT or VertexCover input tree types.")]
        public string InstanceFilePath { get; set; }
                
        /// <summary>
        /// The number of Demand Pairs in the instance.
        /// </summary>
        [Option("demandpairs", HelpText = "The number of demand pairs to generate. Only necessary for Prüfer or Caterpillar input trees.")]
        public int NumberOfDemandPairs { get; set; }

        /// <summary>
        /// The preferred length distribution of the demand pairs.
        /// </summary>
        [Option("distanceDistribution", HelpText = "The distance distribution for the demand pairs. Only necessary for Prüfer or Caterpillar input trees and LengthDistribution input demand pair types. The argument should have any number of \"values\", separated by commas, with a value looking like: (min, max, percentage). For example: \"(0, 5, 0.30), (2, 8, 0.25), (9, 350, 0.45)\".")]
        public string DistanceDistribution { get; set; }

        /// <summary>
        /// The maximum size the solution is allowed to be.
        /// </summary>
        [Option("maxSolutionSize", HelpText = "The maximum size the solution is allowed to be.")]
        public int MaxSolutionSize { get; set; }

        /// <summary>
        /// Whether the program should run in debug mode.
        /// </summary>
        [Option('v', "verbose", HelpText = "Whether the program should run in verbose mode.")]
        public bool Verbose { get; set; }
    }
}
