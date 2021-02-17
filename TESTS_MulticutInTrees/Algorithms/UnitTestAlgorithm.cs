// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Algorithms
{
    [TestClass]
    public class UnitTestAlgorithm
    {
        [TestMethod]
        public void TestRun1()
        {
            Random random = new Random(574);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node0, node2);
            tree.AddChild(node1, node3);

            tree.UpdateNodeTypes();

            DemandPair dp = new DemandPair(node0, node3);
            MulticutInstance instance = new MulticutInstance(tree, new List<DemandPair>() { dp }, 3, random);
            GuoNiedermeierFPT g = new GuoNiedermeierFPT(instance);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, long) solution = g.Run();
            Assert.AreEqual(0, solution.Item1.NumberOfEdges);
            Assert.AreEqual(1, solution.Item2.Count);
            Assert.AreEqual(0, solution.Item3.Count);
        }

        
        [TestMethod]
        public void TestRun2()
        {
            Random random = new Random(83);

            Tree<TreeNode> tree = new Tree<TreeNode>();

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

            tree.AddRoot(node1);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4 });
            tree.AddChildren(node2, new List<TreeNode>() { node5, node6 });
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9 });
            tree.AddChild(node4, node10);
            tree.AddChildren(node5, new List<TreeNode>() { node11, node12 });
            tree.AddChildren(node6, new List<TreeNode>() { node13, node14 });
            tree.AddChild(node7, node15);
            tree.AddChildren(node10, new List<TreeNode>() { node16, node17 });

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node1, node13);
            DemandPair demandPair2 = new DemandPair(node4, node5);
            DemandPair demandPair3 = new DemandPair(node7, node15);
            DemandPair demandPair4 = new DemandPair(node8, node10);
            DemandPair demandPair5 = new DemandPair(node11, node17);

            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 };

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 3, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, long) result = gnfpt.Run();

            Assert.AreEqual(1, result.Item1.NumberOfNodes);
            Assert.AreEqual(3, result.Item2.Count);
            Assert.AreEqual(0, result.Item3.Count);

            foreach (DemandPair dp in result.Item3)
            {
                if (!result.Item1.HasNode(dp.Node1) || !result.Item1.HasNode(dp.Node2))
                {
                    Assert.Fail($"There is a demand pair with an endpoint that does not exist: {dp.Node1}, {dp.Node2}, {result.Item1.Nodes.Print()}.");
                }
            }
        }

        [TestMethod]
        public void TestRandomLargerInstance1()
        {
            Random random = new Random(273);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(1000, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(500, tree, random);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 500, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, long) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
        }

        [TestMethod]
        public void TestRandomLargerInstance2()
        {
            Random random = new Random(789456);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(3000, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(2000, tree, random);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 2000, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, long) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
        }

        [TestMethod]
        public void TestRandomLargerInstance3()
        {
            Random random = new Random(8765);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(500, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(400, tree, random);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 400, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, long) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
        }
    }
}
