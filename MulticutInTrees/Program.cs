// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;
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
            // todo: use counted list/dict everywhere where possible throughout the entire program.

            Console.WriteLine("Hello World!");

            Random random = new Random(0);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(500, random);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(300, tree, random), new Counter());
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 21, random);
            
            Random random2 = new Random(0);
            Tree<TreeNode> tree2 = TreeFromPruferSequence.GenerateTree(500, random2);
            CountedList<DemandPair> demandPairs2 = new CountedList<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(300, tree2, random2), new Counter());
            MulticutInstance instance2 = new MulticutInstance(tree2, demandPairs2, 21, random2);

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
            Console.WriteLine($"Remaining edges: {fptTree.Edges.Print()}");
            Console.WriteLine($"Remaining dps:   {fptDemandPairs.Print()}");
            Console.WriteLine($"Time:            {fptWatch.Elapsed}");
            Console.WriteLine();

            /*
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);
            TreeNode node11 = new TreeNode(11);
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            TreeNode node15 = new TreeNode(15);
            TreeNode node16 = new TreeNode(16);
            TreeNode node17 = new TreeNode(17);
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);

            List<TreeNode> nodes = new List<TreeNode>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22 };

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node5 });
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3 });
            tree.AddChild(node3, node4);
            tree.AddChildren(node5, new List<TreeNode>() { node6, node10, node13, node22 });
            tree.AddChildren(node6, new List<TreeNode>() { node7, node8 });
            tree.AddChild(node8, node9);
            tree.AddChildren(node10, new List<TreeNode>() { node11, node12 });
            tree.AddChildren(node13, new List<TreeNode>() { node14, node15 });
            tree.AddChildren(node15, new List<TreeNode>() { node16, node17 });
            tree.AddChildren(node17, new List<TreeNode>() { node18, node20, node21 });
            tree.AddChild(node18, node19);

            tree.UpdateNodeTypes();

            foreach (TreeNode node in nodes)
            {
                Console.WriteLine($"{node.ID}:\t{node.Type}");
            }
            */

            /*
            int nrNodes = 500;
            double chance = 0.01;

            Graph<Node> graph = new Graph<Node>();

            for (uint i = 0; i < nrNodes; i++)
            {
                graph.AddNode(new Node(i));
            }

            for (int i = 0; i < nrNodes - 1; i++)
            {
                for (int j = i + 1; j < nrNodes; j++)
                {
                    if (Program.Random.NextDouble() < chance)
                    {
                        graph.AddEdge(graph.Nodes[i], graph.Nodes[j]);
                    }
                }
            }

            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes);

            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    graph.AddEdge(components[i][0], components[j][0]);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);

            Console.WriteLine(matching.Print());

            HashSet<(Node, Node)> hashedMatching = new HashSet<(Node, Node)>(matching);
            if (matching.Count != hashedMatching.Count)
            {
                throw new Exception();
            }

            HashSet<Node> matchedNodes = new HashSet<Node>();
            foreach ((Node, Node) edge in matching)
            {
                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            if (graph.Nodes.Count != matchedNodes.Count)
            {
                throw new Exception();
            }
            */

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