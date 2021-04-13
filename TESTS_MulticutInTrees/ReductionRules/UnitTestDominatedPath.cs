// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestDominatedPath
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestDominatedPath));

        private (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) GetLaterIterationInformation(Algorithm algorithm)
        {
            PropertyInfo lastContractedEdgesProperty = typeof(Algorithm).GetProperty("LastContractedEdges", BindingFlags.NonPublic | BindingFlags.Instance);
            List<CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>> lastContractedEdges = (List<CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>>)lastContractedEdgesProperty.GetGetMethod(true).Invoke(algorithm, new object[] { });

            PropertyInfo lastRemovedDemandPairsProperty = typeof(Algorithm).GetProperty("LastRemovedDemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            List<CountedList<DemandPair>> lastRemovedDemandPairs = (List<CountedList<DemandPair>>)lastRemovedDemandPairsProperty.GetGetMethod(true).Invoke(algorithm, new object[] { });

            PropertyInfo lastChangedEdgesPerDemandPairProperty = typeof(Algorithm).GetProperty("LastChangedEdgesPerDemandPair", BindingFlags.NonPublic | BindingFlags.Instance);
            List<CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>> lastChangedEdgesPerDemandPair = (List<CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>>)lastChangedEdgesPerDemandPairProperty.GetGetMethod(true).Invoke(algorithm, new object[] { });

            int index = GetIndexOfReductionRule(algorithm);

            return (lastContractedEdges[index], lastRemovedDemandPairs[index], lastChangedEdgesPerDemandPair[index]);
        }

        private int GetIndexOfReductionRule(Algorithm algorithm)
        {
            return algorithm.ReductionRules.IndexOf(algorithm.ReductionRules.First(rr => rr.GetType() == typeof(DominatedPath)));
        }

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10, 10);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);
            Assert.IsNotNull(dominatedPath);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10, 10);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>();
            CountedList<DemandPair> removedDemandPairs = new CountedList<DemandPair>();
            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>();

            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(null, dps, gnfpt, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, null, gnfpt, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, null, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, null); });

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);

            Assert.ThrowsException<ArgumentNullException>(() => dominatedPath.RunLaterIteration(null, removedDemandPairs, changedEdgesPerDemandPairList));
            Assert.ThrowsException<ArgumentNullException>(() => dominatedPath.RunLaterIteration(contractedEdgeNodeTupleList, null, changedEdgesPerDemandPairList));
            Assert.ThrowsException<ArgumentNullException>(() => dominatedPath.RunLaterIteration(contractedEdgeNodeTupleList, removedDemandPairs, null));
        }

        [TestMethod]
        public void TestRunFirstIteration()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node2, node1);
            DemandPair dp2 = new DemandPair(node2, node0);
            DemandPair dp3 = new DemandPair(node3, node4);
            DemandPair dp4 = new DemandPair(node1, node3);
            DemandPair dp5 = new DemandPair(node3, node1);

            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedPath dominatedPath = new DominatedPath(tree, dps, algorithm, demandPairsPerEdge);
            Assert.IsTrue(dominatedPath.RunFirstIteration());
            Assert.AreEqual(2, dps.Count(MockCounter));
        }

        [TestMethod]
        public void TestAfterEdgeContractionFalse()
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
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 }, MockCounter);
            tree.AddChild(node3, node5, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedPath dominatedPath = new DominatedPath(tree, dps, algorithm, demandPairsPerEdge);

            algorithm.ContractEdge((node3, node5), MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(dominatedPath.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterEdgeContractionTrue()
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
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 }, MockCounter);
            tree.AddChild(node3, node5, MockCounter);
            tree.AddChild(node3, node6, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node5, node4);
            DemandPair dp3 = new DemandPair(node4, node6);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedPath dominatedPath = new DominatedPath(tree, dps, algorithm, demandPairsPerEdge);

            algorithm.ContractEdge((node3, node5), MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsTrue(dominatedPath.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterDemandPathRemove()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node3, node4);
            DemandPair dp3 = new DemandPair(node1, node3);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedPath dominatedPath = new DominatedPath(tree, dps, algorithm, demandPairsPerEdge);

            algorithm.RemoveDemandPair(dp3, MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(dominatedPath.RunLaterIteration(info.Item1, info.Item2, info.Item3));
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

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 }, MockCounter);
            tree.AddChild(node3, node5, MockCounter);
            tree.AddChild(node3, node6, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node5, node4);
            DemandPair dp3 = new DemandPair(node4, node6);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedPath dominatedPath = new DominatedPath(tree, dps, algorithm, demandPairsPerEdge);

            algorithm.ChangeEndpointOfDemandPair(dp3, node6, node3, MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsTrue(dominatedPath.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }
    }
}
