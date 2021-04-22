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
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 0);

            GuoNiedermeierBranching gnb = new(instance);
            Assert.IsNotNull(gnb);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 0);

            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierBranching gnb = new(null); });
        }

        [TestMethod]
        public void TestCase1()
        {
            Graph tree = new();
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);

            tree.AddNodes(new List<Node>() { node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            Edge<Node> edge37 = new(node3, node7);
            Edge<Node> edge38 = new(node3, node8);
            Edge<Node> edge39 = new(node3, node9);
            Edge<Node> edge410 = new(node4, node10);

            tree.AddEdges(new List<Edge<Node>>() { edge12, edge13, edge14, edge25, edge26, edge37, edge38, edge39, edge410 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node1, node5, tree);
            DemandPair dp2 = new(2, node7, node8, tree);
            DemandPair dp3 = new(3, node4, node10, tree);
            DemandPair dp4 = new(4, node6, node3, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3, dp4 }, MockCounter);

            int k = 3;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 3);

            GuoNiedermeierBranching gnb = new(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run();

            Assert.IsTrue(solution.Item2.Solvable);
            Assert.AreEqual(3, solution.Item1.Count);
        }

        [TestMethod]
        public void TestCase2()
        {
            Graph tree = new();
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);

            tree.AddNodes(new List<Node>() { node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            Edge<Node> edge37 = new(node3, node7);
            Edge<Node> edge38 = new(node3, node8);
            Edge<Node> edge39 = new(node3, node9);
            Edge<Node> edge410 = new(node4, node10);

            tree.AddEdges(new List<Edge<Node>>() { edge12, edge13, edge14, edge25, edge26, edge37, edge38, edge39, edge410 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node1, node6, tree);
            DemandPair dp2 = new(2, node4, node5, tree);
            DemandPair dp3 = new(3, node8, node10, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            int k = 2;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 2);

            GuoNiedermeierBranching gnb = new(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run();

            Assert.IsTrue(solution.Item2.Solvable);
            Assert.AreEqual(2, solution.Item1.Count);
        }

        [TestMethod]
        public void TestCase3()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge04 = new(node0, node4);
            Edge<Node> edge05 = new(node0, node5);
            Edge<Node> edge06 = new(node0, node6);
            Edge<Node> edge17 = new(node1, node7);
            Edge<Node> edge18 = new(node1, node8);
            Edge<Node> edge19 = new(node1, node9);
            Edge<Node> edge110 = new(node1, node10);
            Edge<Node> edge111 = new(node1, node11);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge04, edge05, edge06, edge17, edge18, edge19, edge110, edge111 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node2, node11, tree);
            DemandPair dp2 = new(2, node4, node9, tree);
            DemandPair dp3 = new(3, node2, node8, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            int k = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 1);

            GuoNiedermeierBranching gnb = new(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run();

            Assert.IsTrue(solution.Item2.Solvable);
            Assert.AreEqual(1, solution.Item1.Count);
        }

        [TestMethod]
        public void TestCase4()
        {
            Graph tree = new();
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            Node node16 = new(16);
            Node node17 = new(17);

            tree.AddNodes(new List<Node>() { node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17 }, MockCounter);

            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            Edge<Node> edge37 = new(node3, node7);
            Edge<Node> edge38 = new(node3, node8);
            Edge<Node> edge39 = new(node3, node9);
            Edge<Node> edge410 = new(node4, node10);
            Edge<Node> edge511 = new(node5, node11);
            Edge<Node> edge512 = new(node5, node12);
            Edge<Node> edge613 = new(node6, node13);
            Edge<Node> edge614 = new(node6, node14);
            Edge<Node> edge715 = new(node7, node15);
            Edge<Node> edge1016 = new(node10, node16);
            Edge<Node> edge1017 = new(node10, node17);

            tree.AddEdges(new List<Edge<Node>>() { edge12, edge13, edge14, edge25, edge26, edge37, edge38, edge39, edge410, edge511, edge512, edge613, edge614, edge715, edge1016, edge1017 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node1, node13, tree);
            DemandPair dp2 = new(2, node4, node5, tree);
            DemandPair dp3 = new(3, node7, node15, tree);
            DemandPair dp4 = new(4, node8, node10, tree);
            DemandPair dp5 = new(5, node11, node17, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            int k = 3;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 3);

            GuoNiedermeierBranching gnb = new(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run();

            Assert.IsTrue(solution.Item2.Solvable);
            Assert.AreEqual(3, solution.Item1.Count);
        }

        [TestMethod]
        public void TestCase5()
        {
            Graph tree = new();
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);

            tree.AddNodes(new List<Node>() { node1, node2, node3, node4, node5, node6 }, MockCounter);

            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge15 = new(node1, node5);
            Edge<Node> edge16 = new(node1, node6);

            tree.AddEdges(new List<Edge<Node>>() { edge12, edge13, edge14, edge15, edge16 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node1, node2, tree);
            DemandPair dp2 = new(2, node3, node1, tree);
            DemandPair dp3 = new(3, node1, node4, tree);
            DemandPair dp4 = new(4, node1, node5, tree);
            DemandPair dp5 = new(5, node1, node6, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            int k = 4;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, k, 4);

            GuoNiedermeierBranching gnb = new(instance);
            (List<Edge<Node>>, ExperimentOutput) solution = gnb.Run();

            Assert.IsFalse(solution.Item2.Solvable);
        }
    }
}
