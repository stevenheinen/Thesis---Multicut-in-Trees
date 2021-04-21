// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Experiments;
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
#if VERBOSEDEBUG
            if (OperatingSystem.IsWindows())
            {
                Console.BufferHeight = short.MaxValue - 1;
            }
#endif
            Console.WriteLine("Hello World!");

            int numberOfNodes = 50;// 5000;
            int numberOfDPs = 40;// 4000;
            int randomSeed = 0;
            int optimalK = 7;// 79;
            Graph tree = TreeFromPruferSequence.GenerateTree(numberOfNodes, new Random(randomSeed));
            CountedCollection<DemandPair> demandPairs = new(RandomDemandPairs.GenerateRandomDemandPairs(numberOfDPs, tree, new Random(randomSeed)), new Counter());
            MulticutInstance instance = new(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, tree, demandPairs, optimalK, optimalK);
            Algorithm algorithm = new BousquetKernelisation(instance);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            (Graph resTree, List<Edge<Node>> partialSolution, List<DemandPair> finalDemandPairs, ExperimentOutput experimentOutput) = algorithm.Run();
            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"FPT algorithm ({algorithm.GetType()}) result:");
            Console.WriteLine($"Solved?                           {experimentOutput.Solvable}");
            Console.WriteLine($"Partial solution size:            {partialSolution.Count}");
            Console.WriteLine($"Remaining tree:                   {tree}");
            Console.WriteLine($"Remaining number of demand pairs: {finalDemandPairs.Count}");
            Console.WriteLine($"Edge-disjoint demand pairs left:  {MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowGraph<Graph, Edge<Node>, Node>(tree, finalDemandPairs.Select(dp => (dp.Node1, dp.Node2)), new PerformanceMeasurements(""))}");
            Console.WriteLine($"Time required (ticks):            {stopwatch.ElapsedTicks}");
            Console.WriteLine($"Entire partial solution:          {partialSolution.Print()}");
            Console.WriteLine($"Remaining edges:                  {tree.Edges(new Counter()).Print()}");
            Console.WriteLine($"Remaining dps:                    {finalDemandPairs.Print()}");
            Console.WriteLine();

            // For debug purposes only. Should not be included in the final version of the program.
            if (args.Length == 0)
            {
                //string[] split = "--seed=0 --repetitions=1 --experiments=5 --algorithm=BousquetKernelisation --treeType=Prufer --dpType=Random --nrNodes=5000 --nrDPs=4000 --maxSolutionSize=0 -v".Split();
                string[] split = "--seed=0 --repetitions=1 --experiments=5 --algorithm=GuoNiedermeierKernelisation --treeType=Prufer --dpType=Random --nrNodes=5000 --nrDPs=4000 --maxSolutionSize=0 -v".Split();

                args = new string[split.Length + 2];
                split.CopyTo(args, 0);
                args[^1] = "--outputDir=D:\\Documents\\Universiteit\\Thesis\\ExperimentResults";
                args[^2] = "--instanceDir=D:\\Documents\\Universiteit\\Thesis\\Instances";
            }

            // Parse the command line arguments and run the experiments.
            CommandLineParser.ParseAndExecute(args);
        }
    }
}