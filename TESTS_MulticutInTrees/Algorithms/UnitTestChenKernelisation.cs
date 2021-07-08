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
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Algorithms
{
    [TestClass]
    public class UnitTestChenKernelisation
    {
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestRun1()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp = new(0, node0, node3, tree);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, new CountedCollection<DemandPair>(new List<DemandPair>() { dp }, MockCounter), 3, 1);
            ChenKernelisation g = new(instance);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) solution = g.Run();

            Assert.IsTrue(solution.Item4.Solvable);
            Assert.AreEqual(0, solution.Item1.NumberOfEdges(MockCounter));
            Assert.AreEqual(1, solution.Item2.Count);
            Assert.AreEqual(0, solution.Item3.Count);
        }


        [TestMethod]
        public void TestRun2()
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

            DemandPair demandPair1 = new(1, node1, node13, tree);
            DemandPair demandPair2 = new(2, node4, node5, tree);
            DemandPair demandPair3 = new(3, node7, node15, tree);
            DemandPair demandPair4 = new(4, node8, node10, tree);
            DemandPair demandPair5 = new(5, node11, node17, tree);

            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, demandPairs, 3, 3);
            ChenKernelisation gnfpt = new(instance);

            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsTrue(result.Item4.Solvable);
            Assert.AreEqual(0, result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(3, result.Item2.Count);
            Assert.AreEqual(0, result.Item3.Count);
        }

        [TestMethod]
        public void TestRandomLargerInstance1()
        {
            int treeSeed = 137;
            int dpSeed = 763;
            int nrNodes = 1000;
            int nrDPs = 500;
            int optimalK = 28;
            Graph tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(treeSeed));
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(dpSeed))), MockCounter);
            MulticutInstance instance = new(InputTreeType.Prufer, InputDemandPairsType.Random, treeSeed, dpSeed, tree, demandPairs, optimalK, optimalK);
            ChenKernelisation gnfpt = new(instance);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);
            Assert.IsTrue(result.Item4.Solvable);

            /* If this test fails, check the expected numbers using this code.
            Graph treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(treeSeed));
            CountedCollection<DemandPair> demandPairsNaive = new(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(dpSeed))), MockCounter);
            MulticutInstance instanceNaive = new(InputTreeType.Prufer, InputDemandPairsType.Random, treeSeed, dpSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ChenKernelisation gnfptNaive = new(instanceNaive);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.RunNaively();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(MockCounter), result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            //*/

            Assert.AreEqual(27, result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(13, result.Item2.Count);
            Assert.AreEqual(34, result.Item3.Count);
        }

        [TestMethod]
        public void TestRandomLargerInstance2()
        {
            int treeSeed = 202;
            int dpSeed = 247;
            int nrNodes = 3000;
            int nrDPs = 2000;
            int optimalK = 54;
            Graph tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(treeSeed));
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(dpSeed))), MockCounter);
            MulticutInstance instance = new(InputTreeType.Prufer, InputDemandPairsType.Random, treeSeed, dpSeed, tree, demandPairs, optimalK, optimalK);
            ChenKernelisation gnfpt = new(instance);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);
            Assert.IsTrue(result.Item4.Solvable);

            /* If this test fails, check the expected numbers using this code.
            Graph treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(treeSeed));
            CountedCollection<DemandPair> demandPairsNaive = new(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(dpSeed))), MockCounter);
            MulticutInstance instanceNaive = new(InputTreeType.Prufer, InputDemandPairsType.Random, treeSeed, dpSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ChenKernelisation gnfptNaive = new(instanceNaive);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.RunNaively();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(MockCounter), result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            //*/

            Assert.AreEqual(71, result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(29, result.Item2.Count);
            Assert.AreEqual(85, result.Item3.Count);
        }

        [TestMethod]
        public void TestRandomLargerInstance3()
        {
            int treeSeed = 555;
            int dpSeed = 853;
            int nrNodes = 500;
            int nrDPs = 400;
            int optimalK = 24;
            Graph tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(treeSeed));
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(dpSeed))), MockCounter);
            MulticutInstance instance = new(InputTreeType.Prufer, InputDemandPairsType.Random, treeSeed, dpSeed, tree, demandPairs, optimalK, optimalK);
            ChenKernelisation gnfpt = new(instance);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);
            Assert.IsTrue(result.Item4.Solvable);

            /* If this test fails, check the expected numbers using this code.
            Graph treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(treeSeed));
            CountedCollection<DemandPair> demandPairsNaive = new(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(dpSeed))), MockCounter);
            MulticutInstance instanceNaive = new(InputTreeType.Prufer, InputDemandPairsType.Random, treeSeed, dpSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ChenKernelisation gnfptNaive = new(instanceNaive);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.RunNaively();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(MockCounter), result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            //*/

            Assert.AreEqual(39, result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(12, result.Item2.Count);
            Assert.AreEqual(45, result.Item3.Count);
        }
    }
}
