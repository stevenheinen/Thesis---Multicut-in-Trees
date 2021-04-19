// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
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
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestMaximumMultiCommodityFlowInTrees));

        [TestMethod]
        public void TestCase1()
        {
            RootedTree tree = new RootedTree();
            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);
            RootedTreeNode node3 = new RootedTreeNode(3);
            RootedTreeNode node4 = new RootedTreeNode(4);
            RootedTreeNode node5 = new RootedTreeNode(5);
            RootedTreeNode node6 = new RootedTreeNode(6);
            RootedTreeNode node7 = new RootedTreeNode(7);
            RootedTreeNode node8 = new RootedTreeNode(8);
            RootedTreeNode node9 = new RootedTreeNode(9);
            RootedTreeNode node10 = new RootedTreeNode(10);
            RootedTreeNode node11 = new RootedTreeNode(11);
            tree.AddNodes(new List<RootedTreeNode>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11 }, MockCounter);
            Edge<RootedTreeNode> edge01 = new Edge<RootedTreeNode>(node0, node1);
            Edge<RootedTreeNode> edge02 = new Edge<RootedTreeNode>(node0, node2);
            Edge<RootedTreeNode> edge03 = new Edge<RootedTreeNode>(node0, node3);
            Edge<RootedTreeNode> edge14 = new Edge<RootedTreeNode>(node1, node4);
            Edge<RootedTreeNode> edge15 = new Edge<RootedTreeNode>(node1, node5);
            Edge<RootedTreeNode> edge16 = new Edge<RootedTreeNode>(node1, node6);
            Edge<RootedTreeNode> edge27 = new Edge<RootedTreeNode>(node2, node7);
            Edge<RootedTreeNode> edge28 = new Edge<RootedTreeNode>(node2, node8);
            Edge<RootedTreeNode> edge39 = new Edge<RootedTreeNode>(node3, node9);
            Edge<RootedTreeNode> edge310 = new Edge<RootedTreeNode>(node3, node10);
            Edge<RootedTreeNode> edge311 = new Edge<RootedTreeNode>(node3, node11);
            tree.AddEdges(new List<Edge<RootedTreeNode>>() { edge01, edge02, edge03, edge14, edge15, edge16, edge27, edge28, edge39, edge310, edge311 }, MockCounter);
            List<(RootedTreeNode, RootedTreeNode)> commodities = new List<(RootedTreeNode, RootedTreeNode)>() { (node5, node4), (node5, node6), (node6, node0), (node0, node7), (node9, node8), (node10, node11), (node3, node11) };

            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowTree(tree, commodities, MockMeasurements);
            Assert.AreEqual(4, flow);
        }

        [TestMethod]
        public void TestCase2()
        {
            Random random = new Random(80);
            Graph tree = TreeFromPruferSequence.GenerateTree(50, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(10, tree, random);
            IEnumerable<(Node, Node)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowGraph<Graph, Edge<Node>, Node>(tree, commodities, MockMeasurements);
            Assert.AreEqual(3, flow);
        }

        [TestMethod]
        public void TestCase3()
        {
            Random random = new Random(481087);
            Graph tree = TreeFromPruferSequence.GenerateTree(100, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(90, tree, random);
            IEnumerable<(Node, Node)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowGraph<Graph, Edge<Node>, Node>(tree, commodities, MockMeasurements);
            Assert.AreEqual(10, flow);
        }

        [TestMethod]
        public void TestCase4()
        {
            Random random = new Random(45054);
            Graph tree = TreeFromPruferSequence.GenerateTree(500, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(250, tree, random);
            IEnumerable<(Node, Node)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowGraph<Graph, Edge<Node>, Node>(tree, commodities, MockMeasurements);
            Assert.AreEqual(16, flow);
        }

        [TestMethod]
        public void TestCase5()
        {
            Random random = new Random(4878378);
            Graph tree = TreeFromPruferSequence.GenerateTree(2000, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(1000, tree, random);
            IEnumerable<(Node, Node)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowGraph<Graph, Edge<Node>, Node>(tree, commodities, MockMeasurements);
            Assert.AreEqual(36, flow);
        }

        [TestMethod]
        public void TestCase6()
        {
            Random random = new Random(9345745);
            Graph tree = TreeFromPruferSequence.GenerateTree(5000, random);
            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(4000, tree, random);
            IEnumerable<(Node, Node)> commodities = demandPairs.Select(n => (n.Node1, n.Node2));
            int flow = MaximumMultiCommodityFlowInTrees.ComputeMaximumMultiCommodityFlowGraph<Graph, Edge<Node>, Node>(tree, commodities, MockMeasurements);
            Assert.AreEqual(76, flow);
        }
    }
}
