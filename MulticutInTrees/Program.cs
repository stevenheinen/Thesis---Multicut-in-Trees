// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.Experiments;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Utilities;
using System.Linq;

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

            // For debug purposes only. Should not be included in the final version of the program.
            if (args.Length == 0)
            {
                string[] split;

                split = "--seed=0 --repetitions=3 --experiments=5 --algorithm=ImprovedGuoNiedermeierKernelisation --treeType=Prufer --dpType=Random --nrNodes=50 --nrDPs=40 --maxSolutionSize=0 -v".Split();
                
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