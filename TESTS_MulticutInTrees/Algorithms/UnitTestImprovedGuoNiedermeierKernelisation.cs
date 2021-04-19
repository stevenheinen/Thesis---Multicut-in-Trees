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
    public class UnitTestImprovedGuoNiedermeierKernelisation
    {
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestRun1()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp = new DemandPair(0, node0, node3, tree);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedCollection<DemandPair>(new List<DemandPair>() { dp }, MockCounter), 3, 1);
            ImprovedGuoNiedermeierKernelisation g = new ImprovedGuoNiedermeierKernelisation(instance);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) solution = g.Run();
            Assert.IsTrue(solution.Item4.Solvable);
            Assert.AreEqual(0, solution.Item1.NumberOfEdges(MockCounter));
            Assert.AreEqual(1, solution.Item2.Count);
            Assert.AreEqual(0, solution.Item3.Count);
        }


        [TestMethod]
        public void TestRun2()
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

            DemandPair demandPair1 = new DemandPair(1, node1, node13, tree);
            DemandPair demandPair2 = new DemandPair(2, node4, node5, tree);
            DemandPair demandPair3 = new DemandPair(3, node7, node15, tree);
            DemandPair demandPair4 = new DemandPair(4, node8, node10, tree);
            DemandPair demandPair5 = new DemandPair(5, node11, node17, tree);

            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 3, 3);
            ImprovedGuoNiedermeierKernelisation gnfpt = new ImprovedGuoNiedermeierKernelisation(instance);

            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsTrue(result.Item4.Solvable);
            Assert.AreEqual(1, result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(3, result.Item2.Count);
            Assert.AreEqual(0, result.Item3.Count);

            foreach (DemandPair dp in result.Item3)
            {
                if (!result.Item1.HasNode(dp.Node1, MockCounter) || !result.Item1.HasNode(dp.Node2, MockCounter))
                {
                    Assert.Fail($"There is a demand pair with an endpoint that does not exist: {dp.Node1}, {dp.Node2}, {result.Item1.Nodes(MockCounter).Print()}.");
                }
            }
        }

        [TestMethod]
        public void TestRandomLargerInstance1()
        {
            int randomSeed = 273;
            int nrNodes = 1000;
            int nrDPs = 500;
            int optimalK = 28;
            Graph tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(randomSeed))), MockCounter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, tree, demandPairs, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfpt = new ImprovedGuoNiedermeierKernelisation(instance);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);

            /* If this test fails, check the expected numbers using this code.
            Graph treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairsNaive = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(randomSeed))), MockCounter);
            MulticutInstance instanceNaive = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfptNaive = new ImprovedGuoNiedermeierKernelisation(instanceNaive);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.Run();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(MockCounter), result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            //*/

            Assert.AreEqual(11, result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(23, result.Item2.Count);
            Assert.AreEqual(10, result.Item3.Count);
        }

        [TestMethod]
        public void TestRandomLargerInstance2()
        {
            int randomSeed = 789456;
            int nrNodes = 3000;
            int nrDPs = 2000;
            int optimalK = 54;
            Graph tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(randomSeed))), MockCounter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, tree, demandPairs, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfpt = new ImprovedGuoNiedermeierKernelisation(instance);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);

            /* If this test fails, check the expected numbers using this code.
            Graph treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairsNaive = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(randomSeed))), MockCounter);
            MulticutInstance instanceNaive = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfptNaive = new ImprovedGuoNiedermeierKernelisation(instanceNaive);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.Run();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(MockCounter), result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            //*/

            Assert.AreEqual(90, result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(22, result.Item2.Count);
            Assert.AreEqual(105, result.Item3.Count);
        }

        [TestMethod]
        public void TestRandomLargerInstance3()
        {
            int randomSeed = 8765;
            int nrNodes = 500;
            int nrDPs = 400;
            int optimalK = 24;
            Graph tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(randomSeed))), MockCounter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, tree, demandPairs, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfpt = new ImprovedGuoNiedermeierKernelisation(instance);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);

            /* If this test fails, check the expected numbers using this code.
            Graph treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairsNaive = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(randomSeed))), MockCounter);
            MulticutInstance instanceNaive = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfptNaive = new ImprovedGuoNiedermeierKernelisation(instanceNaive);
            (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.Run();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(MockCounter), result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            //*/

            Assert.AreEqual(6, result.Item1.NumberOfNodes(MockCounter));
            Assert.AreEqual(22, result.Item2.Count);
            Assert.AreEqual(5, result.Item3.Count);
        }
    }
}
