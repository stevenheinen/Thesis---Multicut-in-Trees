// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using MulticutInTrees.Algorithms;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.ReductionRules;
using MulticutInTrees.Utilities;

namespace MulticutInTrees
{
    /// <summary>
    /// The entry class for the program.
    /// </summary>
    public class Program
    {
        // TODO: Do not make singleton. Should be part of an instance.
        // TODO: Create some way to give it a seed, do not start with a fixed one. Used now for debugging purposes.
        /// <summary>
        /// The global <see cref="System.Random"/> used throughout the entire program.
        /// </summary>
        public readonly static Random Random = new Random(1);

        // TODO: Better solution
        /// <summary>
        /// <see cref="bool"/> that represents whether debug information should be printed to the console during execution of the algorithm.
        /// </summary>
        public readonly static bool PRINT_DEBUG_INFORMATION = false;

        /// <summary>
        /// The entry method for the program.
        /// </summary>
        public static void Main()
        {
            Console.WriteLine("Hello World!");

            /*
            int numberOfNodes = 500;
            int numberOfDemandPairs = 300;
            int k = 500;

            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(numberOfNodes);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(numberOfDemandPairs, tree);

            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(tree, demandPairs, k);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>) result = gnfpt.Run();

            //Console.WriteLine($"Result achieved!\nTree:\n{result.Item1}\nCut edges:\n{result.Item2.Print()}\nNumber of remaining Demand Pairs:\n{result.Item3.Count}");
            */

            /*
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node0, node2);
            tree.AddChild(node1, node3);

            DemandPair dp = new DemandPair(node0, node2);

            GuoNiedermeierFPT g = new GuoNiedermeierFPT(tree, new List<DemandPair>() { dp });

            var x = g.Run();
            */

            /*
            Graph<Node> g = new Graph<Node>();
            Node n = new Node(0);
            Node m = new Node(1);
            Node k = new Node(2);
            Node l = new Node(3);
            g.AddNode(n);
            g.AddNode(m);
            g.AddNode(k);
            g.AddNode(l);

            g.AddEdge(n, m);
            g.AddEdge(n, k);
            g.AddEdge(l, n);

            g.RemoveNode(n); 
            g.AddNode(n);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode a = new TreeNode(0);
            TreeNode b = new TreeNode(1);
            TreeNode c = new TreeNode(2);
            TreeNode d = new TreeNode(3);

            tree.AddRoot(a);
            tree.AddChildren(a, new List<TreeNode>(){ b, c });
            tree.AddChild(c, d);
            //*/

            /*
            Graph<Node> g = new Graph<Node>();
            Node n = new Node(0);
            Node m = new Node(1);
            g.AddNode(n);
            g.AddNode(m);

            g.AddEdge(n, m);
            g.RemoveNode(n);
            g.AddNode(n);

            Graph<TreeNode> t = new Graph<TreeNode>();
            TreeNode u = new TreeNode(0);
            TreeNode v = new TreeNode(1);
            t.AddNode(u);
            t.AddNode(v);
            t.AddEdge(u, v);
            //*/

            /*
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);

            Graph<Node> g = new Graph<Node>();

            g.AddNode(node0);
            g.AddNode(node1);
            g.AddNode(node2);
            g.AddNode(node3);
            g.AddNode(node4);
            g.AddNode(node5);
            g.AddNode(node6);
            g.AddNode(node7);
            g.AddNode(node8);
            g.AddNode(node9);
            g.AddNode(node10);

            g.AddEdge(node0, node1);
            g.AddEdge(node0, node2);
            g.AddEdge(node0, node3);
            g.AddEdge(node1, node6);
            g.AddEdge(node2, node4);
            g.AddEdge(node2, node5);
            g.AddEdge(node4, node9);
            g.AddEdge(node5, node8);
            g.AddEdge(node8, node7);
            g.AddEdge(node9, node10);

            int flow = DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities(g, new List<Node>() { node1, node3, node5, node4 }, new List<Node>() { node0, node10, node7, node6 });
            Console.WriteLine($"Flow: {flow}");
            //*/
        }
    }
}