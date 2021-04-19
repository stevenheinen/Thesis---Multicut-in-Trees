// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Experiments;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.Algorithms
{
    [TestClass]
    public class UnitTestGuoNiedermeierBranching
    {
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new Graph();
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 0);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            Assert.IsNotNull(gnb);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Graph tree = new Graph();
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 0);

            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(null); });
        }

        [TestMethod]
        public void TestCase1()
        {
            Graph tree = new Graph();
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

            tree.AddNodes(new List<Node>() { node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge26 = new Edge<Node>(node2, node6);
            Edge<Node> edge37 = new Edge<Node>(node3, node7);
            Edge<Node> edge38 = new Edge<Node>(node3, node8);
            Edge<Node> edge39 = new Edge<Node>(node3, node9);
            Edge<Node> edge410 = new Edge<Node>(node4, node10);

            tree.AddEdges(new List<Edge<Node>>() { edge12, edge13, edge14, edge25, edge26, edge37, edge38, edge39, edge410 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node1, node5, tree);
            DemandPair dp2 = new DemandPair(2, node7, node8, tree);
            DemandPair dp3 = new DemandPair(3, node4, node10, tree);
            DemandPair dp4 = new DemandPair(4, node6, node3, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4 }, MockCounter);

            int k = 3;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 3);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run(true);

            Assert.IsTrue(solution.Item2.Solvable);
            Assert.AreEqual(3, solution.Item1.Count);
        }

        [TestMethod]
        public void TestCase2()
        {
            Graph tree = new Graph();
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

            tree.AddNodes(new List<Node>() { node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge26 = new Edge<Node>(node2, node6);
            Edge<Node> edge37 = new Edge<Node>(node3, node7);
            Edge<Node> edge38 = new Edge<Node>(node3, node8);
            Edge<Node> edge39 = new Edge<Node>(node3, node9);
            Edge<Node> edge410 = new Edge<Node>(node4, node10);

            tree.AddEdges(new List<Edge<Node>>() { edge12, edge13, edge14, edge25, edge26, edge37, edge38, edge39, edge410 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node1, node6, tree);
            DemandPair dp2 = new DemandPair(2, node4, node5, tree);
            DemandPair dp3 = new DemandPair(3, node8, node10, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            int k = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 2);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run(true);

            Assert.IsTrue(solution.Item2.Solvable);
            Assert.AreEqual(2, solution.Item1.Count);
        }

        [TestMethod]
        public void TestCase3()
        {
            Graph tree = new Graph();
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
            Node node11 = new Node(11);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge04 = new Edge<Node>(node0, node4);
            Edge<Node> edge05 = new Edge<Node>(node0, node5);
            Edge<Node> edge06 = new Edge<Node>(node0, node6);
            Edge<Node> edge17 = new Edge<Node>(node1, node7);
            Edge<Node> edge18 = new Edge<Node>(node1, node8);
            Edge<Node> edge19 = new Edge<Node>(node1, node9);
            Edge<Node> edge110 = new Edge<Node>(node1, node10);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge04, edge05, edge06, edge17, edge18, edge19, edge110, edge111 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node2, node11, tree);
            DemandPair dp2 = new DemandPair(2, node4, node9, tree);
            DemandPair dp3 = new DemandPair(3, node2, node8, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            int k = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 1);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run(true);

            Assert.IsTrue(solution.Item2.Solvable);
            Assert.AreEqual(1, solution.Item1.Count);
        }

        [TestMethod]
        public void TestCase4()
        {
            Graph tree = new Graph();
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
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);

            tree.AddNodes(new List<Node>() { node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17 }, MockCounter);

            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge26 = new Edge<Node>(node2, node6);
            Edge<Node> edge37 = new Edge<Node>(node3, node7);
            Edge<Node> edge38 = new Edge<Node>(node3, node8);
            Edge<Node> edge39 = new Edge<Node>(node3, node9);
            Edge<Node> edge410 = new Edge<Node>(node4, node10);
            Edge<Node> edge511 = new Edge<Node>(node5, node11);
            Edge<Node> edge512 = new Edge<Node>(node5, node12);
            Edge<Node> edge613 = new Edge<Node>(node6, node13);
            Edge<Node> edge614 = new Edge<Node>(node6, node14);
            Edge<Node> edge715 = new Edge<Node>(node7, node15);
            Edge<Node> edge1016 = new Edge<Node>(node10, node16);
            Edge<Node> edge1017 = new Edge<Node>(node10, node17);

            tree.AddEdges(new List<Edge<Node>>() { edge12, edge13, edge14, edge25, edge26, edge37, edge38, edge39, edge410, edge511, edge512, edge613, edge614, edge715, edge1016, edge1017 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node1, node13, tree);
            DemandPair dp2 = new DemandPair(2, node4, node5, tree);
            DemandPair dp3 = new DemandPair(3, node7, node15, tree);
            DemandPair dp4 = new DemandPair(4, node8, node10, tree);
            DemandPair dp5 = new DemandPair(5, node11, node17, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            int k = 3;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 3);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run(true);

            Assert.IsTrue(solution.Item2.Solvable);
            Assert.AreEqual(3, solution.Item1.Count);
        }

        [TestMethod]
        public void TestCase5()
        {
            Graph tree = new Graph();
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);

            tree.AddNodes(new List<Node>() { node1, node2, node3, node4, node5, node6 }, MockCounter);

            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge16 = new Edge<Node>(node1, node6);

            tree.AddEdges(new List<Edge<Node>>() { edge12, edge13, edge14, edge15, edge16 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node1, node2, tree);
            DemandPair dp2 = new DemandPair(2, node3, node1, tree);
            DemandPair dp3 = new DemandPair(3, node1, node4, tree);
            DemandPair dp4 = new DemandPair(4, node1, node5, tree);
            DemandPair dp5 = new DemandPair(5, node1, node6, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            int k = 4;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 4);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run(true);

            Assert.IsFalse(solution.Item2.Solvable);
        }
    }
}
