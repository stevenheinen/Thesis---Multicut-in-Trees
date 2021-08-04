// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Diagnostics.CodeAnalysis;
using MulticutInTrees.CommandLineArguments;

namespace MulticutInTrees
{
    /// <summary>
    /// The entry class for the program.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /// <summary>
        /// The entry method for the program.
        /// </summary>
        /// <param name="args">Array with command line arguments.</param>
        public static void Main(string[] args)
        {
#if VERBOSEDEBUG
            if (OperatingSystem.IsWindows())
            {
                Console.BufferHeight = short.MaxValue - 1;
            }
#endif
            Console.WriteLine("Hello World!");

            // For debug purposes only. Should not be included in the final version of the program.
            if (args.Length == 0)
            {
                string[] split = "--treeSeed=6809501 --dpSeed=6721572 --repetitions=1 --experiments=1 --algorithm=GenerateInstances --treeType=Prufer --dpType=ThroughKnownSolution --nrNodes=128 --nrDPs=128 --maxSolutionSize=5 -v".Split();

                args = new string[split.Length + 2];
                split.CopyTo(args, 0);
                //args[^1] = "--outputDir=D:\\Documents\\Universiteit\\Thesis\\ExperimentResults";
                args[^1] = "--outputDir=D:\\Downloads";
                args[^2] = "--instanceDir=D:\\Documents\\Universiteit\\Thesis\\Instances";
                //args[^3] = "--instanceFilePath=D:\\Documents\\Universiteit\\Thesis\\3SAT-instances\\uf50-218\\uf50-0001.cnf";
                //args[^3] = "--instanceFilePath=D:\\Downloads\\3sat-test.cnf";
                //args[^3] = "--instanceFilePath=D:\\Documents\\Universiteit\\Thesis\\GNPVertexCoverInstances\\gnp_nrNodes=1024_nrEdges=1024_seed=9607534.mis";
            }

            // Parse the command line arguments and run the experiments.
            CommandLineParser.ParseAndExecute(args);
        }
    }
}