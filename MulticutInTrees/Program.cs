// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static void Main()
        {
            // todo: write more unit tests for the new stuff

            Console.WriteLine("Hello World!");

            Random random = new Random(0);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(500, random);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(300, tree, random), new Counter());
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 21);

            Random random2 = new Random(0);
            Tree<TreeNode> tree2 = TreeFromPruferSequence.GenerateTree(500, random2);
            CountedList<DemandPair> demandPairs2 = new CountedList<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(300, tree2, random2), new Counter());
            MulticutInstance instance2 = new MulticutInstance(tree2, demandPairs2, 21);

            GuoNiedermeierBranching gnBranching = new GuoNiedermeierBranching(instance);
            GuoNiedermeierFPT gnFPT = new GuoNiedermeierFPT(instance2);

            /*
            Stopwatch branchWatch = new Stopwatch();
            branchWatch.Start();
            (bool branchSolved, List<(TreeNode, TreeNode)> branchEdges) = gnBranching.Run();
            branchWatch.Stop();
            */

            Stopwatch fptWatch = new Stopwatch();
            fptWatch.Start();
            (bool fptSolved, Tree<TreeNode> fptTree, List<(TreeNode, TreeNode)> fptEdges, List<DemandPair> fptDemandPairs) = gnFPT.Run();
            fptWatch.Stop();

            /*
            Console.WriteLine();
            Console.WriteLine("Branching algorithm:");
            Console.WriteLine("========================================");
            Console.WriteLine($"Solved:          {branchSolved}");
            Console.WriteLine($"Edges:           {branchEdges.Print()}");
            Console.WriteLine($"Time:            {branchWatch.Elapsed}");
            Console.WriteLine();
            */

            Console.WriteLine();
            Console.WriteLine("FPT algorithm:");
            Console.WriteLine("========================================");
            Console.WriteLine($"Solved:          {fptSolved}");
            Console.WriteLine($"Edges:           {fptEdges.Print()}");
            Console.WriteLine($"Remaining tree:  {fptTree}");
            Console.WriteLine($"Remaining edges: {fptTree.Edges(new Counter()).Print()}");
            Console.WriteLine($"Remaining dps:   {fptDemandPairs.Print()}");
            Console.WriteLine($"Time:            {fptWatch.Elapsed}");
            Console.WriteLine();
        }
    }
}