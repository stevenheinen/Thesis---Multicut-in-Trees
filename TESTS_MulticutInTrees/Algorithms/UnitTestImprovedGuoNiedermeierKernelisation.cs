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
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestRun1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node0, node2, counter);
            tree.AddChild(node1, node3, counter);

            tree.UpdateNodeTypes();

            DemandPair dp = new DemandPair(node0, node3);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedList<DemandPair>(new List<DemandPair>() { dp }, counter), 3, 1);
            ImprovedGuoNiedermeierKernelisation g = new ImprovedGuoNiedermeierKernelisation(instance);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) solution = g.Run();
            Assert.IsTrue(solution.Item4.Solvable);
            Assert.AreEqual(0, solution.Item1.NumberOfEdges(counter));
            Assert.AreEqual(1, solution.Item2.Count);
            Assert.AreEqual(0, solution.Item3.Count);
        }


        [TestMethod]
        public void TestRun2()
        {
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

            tree.AddRoot(node1, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4 }, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node5, node6 }, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9 }, counter);
            tree.AddChild(node4, node10, counter);
            tree.AddChildren(node5, new List<TreeNode>() { node11, node12 }, counter);
            tree.AddChildren(node6, new List<TreeNode>() { node13, node14 }, counter);
            tree.AddChild(node7, node15, counter);
            tree.AddChildren(node10, new List<TreeNode>() { node16, node17 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node1, node13);
            DemandPair demandPair2 = new DemandPair(node4, node5);
            DemandPair demandPair3 = new DemandPair(node7, node15);
            DemandPair demandPair4 = new DemandPair(node8, node10);
            DemandPair demandPair5 = new DemandPair(node11, node17);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 3, 3);
            ImprovedGuoNiedermeierKernelisation gnfpt = new ImprovedGuoNiedermeierKernelisation(instance);

            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsTrue(result.Item4.Solvable);
            Assert.AreEqual(1, result.Item1.NumberOfNodes(counter));
            Assert.AreEqual(3, result.Item2.Count);
            Assert.AreEqual(0, result.Item3.Count);

            foreach (DemandPair dp in result.Item3)
            {
                if (!result.Item1.HasNode(dp.Node1, counter) || !result.Item1.HasNode(dp.Node2, counter))
                {
                    Assert.Fail($"There is a demand pair with an endpoint that does not exist: {dp.Node1}, {dp.Node2}, {result.Item1.Nodes(counter).Print()}.");
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
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(randomSeed))), counter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, tree, demandPairs, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfpt = new ImprovedGuoNiedermeierKernelisation(instance);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);

            /* If this test fails, check the expected numbers using this code.
            Tree<TreeNode> treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairsNaive = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(randomSeed))), counter);
            MulticutInstance instanceNaive = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfptNaive = new ImprovedGuoNiedermeierKernelisation(instanceNaive);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.Run();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(counter), result.Item1.NumberOfNodes(counter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            */

            Assert.AreEqual(11, result.Item1.NumberOfNodes(counter));
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
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(randomSeed))), counter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, tree, demandPairs, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfpt = new ImprovedGuoNiedermeierKernelisation(instance);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);

            /* If this test fails, check the expected numbers using this code.
            Tree<TreeNode> treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairsNaive = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(randomSeed))), counter);
            MulticutInstance instanceNaive = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfptNaive = new ImprovedGuoNiedermeierKernelisation(instanceNaive);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.Run();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(counter), result.Item1.NumberOfNodes(counter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            */

            Assert.AreEqual(90, result.Item1.NumberOfNodes(counter));
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
            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, tree, new Random(randomSeed))), counter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, tree, demandPairs, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfpt = new ImprovedGuoNiedermeierKernelisation(instance);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) result = gnfpt.Run();

            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.IsNotNull(result.Item4);

            /* If this test fails, check the expected numbers using this code.
            Tree<TreeNode> treeNaive = TreeFromPruferSequence.GenerateTree(nrNodes, new Random(randomSeed));
            CountedList<DemandPair> demandPairsNaive = new CountedList<DemandPair>(new List<DemandPair>(RandomDemandPairs.GenerateRandomDemandPairs(nrDPs, treeNaive, new Random(randomSeed))), counter);
            MulticutInstance instanceNaive = new MulticutInstance(InputTreeType.Prufer, InputDemandPairsType.Random, randomSeed, treeNaive, demandPairsNaive, optimalK, optimalK);
            ImprovedGuoNiedermeierKernelisation gnfptNaive = new ImprovedGuoNiedermeierKernelisation(instanceNaive);
            (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) resultNaive = gnfptNaive.Run();

            Assert.AreEqual(resultNaive.Item1.NumberOfNodes(counter), result.Item1.NumberOfNodes(counter));
            Assert.AreEqual(resultNaive.Item2.Count, result.Item2.Count);
            Assert.AreEqual(resultNaive.Item3.Count, result.Item3.Count);
            */

            Assert.AreEqual(8, result.Item1.NumberOfNodes(counter));
            Assert.AreEqual(21, result.Item2.Count);
            Assert.AreEqual(6, result.Item3.Count);
        }
    }
}
