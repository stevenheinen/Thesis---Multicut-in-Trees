// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestMaximumMultiCommodityFlowInTrees
    {
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestCase1()
        {
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
            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5, node6 }, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node7, node8 }, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node9, node10, node11 }, counter);
            List<(TreeNode, TreeNode)> commodities = new List<(TreeNode, TreeNode)>() { (node5, node4), (node5, node6), (node6, node0), (node0, node7), (node9, node8), (node10, node11), (node3, node11) };

            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlow(tree, commodities, counter);
            Assert.AreEqual(4, flow);
        }

        [TestMethod]
        public void TestCase2()
        {
            Random random = new Random(80);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(50, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(10, tree, random);
            IEnumerable<(TreeNode, TreeNode)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlow(tree, commodities, counter);
            Assert.AreEqual(4, flow);
        }

        [TestMethod]
        public void TestCase3()
        {
            Random random = new Random(481087);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(100, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(90, tree, random);
            IEnumerable<(TreeNode, TreeNode)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlow(tree, commodities, counter);
            Assert.AreEqual(13, flow);
        }

        [TestMethod]
        public void TestCase4()
        {
            Random random = new Random(45054);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(500, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(250, tree, random);
            IEnumerable<(TreeNode, TreeNode)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlow(tree, commodities, counter);
            Assert.AreEqual(16, flow);
        }

        [TestMethod]
        public void TestCase5()
        {
            Random random = new Random(4878378);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(2000, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(1000, tree, random);
            IEnumerable<(TreeNode, TreeNode)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlow(tree, commodities, counter);
            Assert.AreEqual(33, flow);
        }

        [TestMethod]
        public void TestCase6()
        {
            Random random = new Random(9345745);
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(5000, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(4000, tree, random);
            IEnumerable<(TreeNode, TreeNode)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlow(tree, commodities, counter);
            Assert.AreEqual(73, flow);
        }
    }
}
