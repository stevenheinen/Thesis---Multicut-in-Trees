// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestUniqueDirection
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestUniqueDirection));

        private UniqueDirection GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(UniqueDirection))
                {
                    return (UniqueDirection)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(UniqueDirection)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> dpsPerNode = new CountedDictionary<TreeNode, CountedCollection<DemandPair>>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> dpsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();
            UniqueDirection uniqueDirection = new UniqueDirection(tree, dps, algorithm, dpsPerNode, dpsPerEdge);
            Assert.IsNotNull(uniqueDirection);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> dpsPerNode = new CountedDictionary<TreeNode, CountedCollection<DemandPair>>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> dpsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(null, dps, algorithm, dpsPerNode, dpsPerEdge));
            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(tree, null, algorithm, dpsPerNode, dpsPerEdge));
            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(tree, dps, null, dpsPerNode, dpsPerEdge));
            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(tree, dps, algorithm, null, dpsPerEdge));
            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(tree, dps, algorithm, dpsPerNode, null));
        }

        [TestMethod]
        public void TestRunFirstIterationFalse()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.AddChild(node2, node5, MockCounter);
            tree.AddChild(node2, node6, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node3);
            DemandPair dp2 = new DemandPair(2, node0, node4);
            DemandPair dp3 = new DemandPair(3, node3, node4);
            DemandPair dp4 = new DemandPair(4, node0, node6);
            DemandPair dp5 = new DemandPair(5, node5, node6);
            DemandPair dp6 = new DemandPair(6, node0, node5);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5, dp6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(uniqueDirection.RunFirstIteration());
        }

        [TestMethod]
        public void TestRunFirstIterationTrue1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.AddChild(node2, node5, MockCounter);
            tree.AddChild(node2, node6, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node3);
            DemandPair dp2 = new DemandPair(2, node0, node4);
            DemandPair dp3 = new DemandPair(3, node3, node4);
            DemandPair dp4 = new DemandPair(4, node0, node6);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(uniqueDirection.RunFirstIteration());
            Assert.AreEqual(5, tree.NumberOfEdges(MockCounter));
        }

        [TestMethod]
        public void TestRunFirstIterationTrue2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.AddChild(node0, node5, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node3);
            DemandPair dp2 = new DemandPair(2, node0, node4);
            DemandPair dp3 = new DemandPair(3, node3, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(uniqueDirection.RunFirstIteration());
            Assert.AreEqual(3, tree.NumberOfEdges(MockCounter));
        }

        [TestMethod]
        public void TestRunFirstIterationTrue3()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.AddChild(node0, node5, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node3);
            DemandPair dp2 = new DemandPair(2, node0, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(uniqueDirection.RunFirstIteration());
            Assert.AreEqual(1, tree.NumberOfEdges(MockCounter));
        }

        [TestMethod]
        public void TestPathOfLength2BetweenLeaves()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node2, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(0, node1, node2);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(uniqueDirection.RunFirstIteration());
            Assert.AreEqual(1, tree.NumberOfEdges(MockCounter));
        }

        [TestMethod]
        public void TestAfterDemandPairRemoved()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node2, node4, MockCounter);
            tree.AddChild(node2, node5, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node1);
            DemandPair dp2 = new DemandPair(2, node1, node3);
            DemandPair dp3 = new DemandPair(3, node3, node4);
            DemandPair dp4 = new DemandPair(4, node4, node5);
            DemandPair dp5 = new DemandPair(5, node3, node5);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            algorithm.CutEdge((node0, node1), MockMeasurements);

            Assert.IsTrue(uniqueDirection.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterEdgeContracted()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node2, node4, MockCounter);
            tree.AddChild(node2, node5, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node1);
            DemandPair dp2 = new DemandPair(2, node1, node3);
            DemandPair dp3 = new DemandPair(3, node3, node4);
            DemandPair dp4 = new DemandPair(4, node4, node5);
            DemandPair dp5 = new DemandPair(5, node3, node5);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            algorithm.CutEdge((node0, node1), MockMeasurements);

            Assert.IsTrue(uniqueDirection.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged()
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
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.AddChild(node1, node5, MockCounter);
            tree.AddChild(node2, node6, MockCounter);
            tree.AddChild(node2, node7, MockCounter);
            tree.AddChild(node2, node8, MockCounter);
            tree.AddChild(node6, node9, MockCounter);
            tree.AddChild(node6, node10, MockCounter);
            tree.AddChild(node6, node11, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node3);
            DemandPair dp3 = new DemandPair(3, node4, node5);
            DemandPair dp4 = new DemandPair(4, node6, node7);
            DemandPair dp5 = new DemandPair(5, node6, node8);
            DemandPair dp6 = new DemandPair(6, node7, node8);
            DemandPair dp7 = new DemandPair(7, node0, node3);
            DemandPair dp8 = new DemandPair(8, node0, node5);
            DemandPair dp9 = new DemandPair(9, node0, node9);
            DemandPair dp10 = new DemandPair(10, node9, node11);
            DemandPair dp11 = new DemandPair(11, node9, node10);
            DemandPair dp12 = new DemandPair(12, node10, node11);
            DemandPair dp13 = new DemandPair(13, node2, node3);
            DemandPair dp14 = new DemandPair(14, node2, node11);
            DemandPair dp15 = new DemandPair(15, node6, node11);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9, dp10, dp11, dp12, dp13, dp14, dp15 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(uniqueDirection.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp9, node0, node2, MockMeasurements);

            Assert.IsTrue(uniqueDirection.RunLaterIteration());
        }
    }
}
