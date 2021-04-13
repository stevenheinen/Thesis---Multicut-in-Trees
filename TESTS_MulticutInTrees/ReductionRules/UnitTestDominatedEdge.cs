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
    public class UnitTestDominatedEdge
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestDominatedEdge));

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
            return algorithm.ReductionRules.IndexOf(algorithm.ReductionRules.First(rr => rr.GetType() == typeof(DominatedEdge)));
        }

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, MockCounter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, MockCounter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, MockCounter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) }
            }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsNotNull(dominatedEdge);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, MockCounter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, MockCounter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, MockCounter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) }
            }, MockCounter);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>();
            CountedList<DemandPair> removedDemandPairs = new CountedList<DemandPair>();
            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>();

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(null, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, null, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), null); });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);

            Assert.ThrowsException<ArgumentNullException>(() => dominatedEdge.RunLaterIteration(null, removedDemandPairs, changedEdgesPerDemandPairList));
            Assert.ThrowsException<ArgumentNullException>(() => dominatedEdge.RunLaterIteration(contractedEdgeNodeTupleList, null, changedEdgesPerDemandPairList));
            Assert.ThrowsException<ArgumentNullException>(() => dominatedEdge.RunLaterIteration(contractedEdgeNodeTupleList, removedDemandPairs, null));
        }

        [TestMethod]
        public void TestFirstIteration()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, MockCounter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, MockCounter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node3, node0);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2 }, MockCounter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1 }, MockCounter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair2 }, MockCounter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1 }, MockCounter) },
                {(node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair2 }, MockCounter) }
            }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.RunFirstIteration());

            DemandPair demandPair3 = new DemandPair(node1, node4);
            DemandPair demandPair4 = new DemandPair(node3, node1);
            DemandPair demandPair5 = new DemandPair(node0, node1);
            DemandPair demandPair6 = new DemandPair(node0, node2);
            CountedList<DemandPair> demandPairs2 = new CountedList<DemandPair>(new List<DemandPair>() { demandPair3, demandPair4, demandPair5, demandPair6 }, MockCounter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge2 = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair6 }, MockCounter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair5 }, MockCounter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair3 }, MockCounter) },
                {(node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair4 }, MockCounter) }
            }, MockCounter);

            dominatedEdge = new DominatedEdge(tree, demandPairs2, new GuoNiedermeierKernelisation(instance), demandPairPerEdge2);
            Assert.IsFalse(dominatedEdge.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathRemoved1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, MockCounter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, MockCounter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node3, node0);
            DemandPair demandPair3 = new DemandPair(node2, node1);
            DemandPair demandPair4 = new DemandPair(node2, node0);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            DemandPair demandPair6 = new DemandPair(node1, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, algorithm, demandPairPerEdge);

            algorithm.RemoveDemandPair(demandPair3, MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(dominatedEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        public void TestAfterDemandPathRemoved2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, MockCounter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, MockCounter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node3, node0);
            DemandPair demandPair4 = new DemandPair(node2, node0);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            DemandPair demandPair6 = new DemandPair(node1, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, algorithm, demandPairPerEdge);

            algorithm.RemoveDemandPair(demandPair4, MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsTrue(dominatedEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, MockCounter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, MockCounter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node1, node4);
            DemandPair demandPair3 = new DemandPair(node2, node1);
            DemandPair demandPair4 = new DemandPair(node2, node0);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, algorithm, demandPairPerEdge);

            algorithm.ChangeEndpointOfDemandPair(demandPair3, node1, node0, MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsTrue(dominatedEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node1, node0);
            DemandPair demandPair2 = new DemandPair(node3, node2);
            DemandPair demandPair3 = new DemandPair(node2, node0);
            DemandPair demandPair4 = new DemandPair(node1, node3);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, algorithm, demandPairPerEdge);

            algorithm.ChangeEndpointOfDemandPair(demandPair3, node0, node1, MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(dominatedEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged3()
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
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node3, node4, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node0, node3);
            DemandPair demandPair2 = new DemandPair(node1, node2);
            DemandPair demandPair3 = new DemandPair(node0, node1);
            DemandPair demandPair4 = new DemandPair(node2, node4);
            DemandPair demandPair5 = new DemandPair(node3, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, algorithm, demandPairPerEdge);

            algorithm.ChangeEndpointOfDemandPair(demandPair4, node2, node3, MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsTrue(dominatedEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged4()
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
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node3, node4, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node0, node3);
            DemandPair demandPair2 = new DemandPair(node1, node2);
            DemandPair demandPair3 = new DemandPair(node0, node1);
            DemandPair demandPair4 = new DemandPair(node2, node4);
            DemandPair demandPair5 = new DemandPair(node3, node4);
            DemandPair demandPair6 = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, algorithm, demandPairPerEdge);

            algorithm.ChangeEndpointOfDemandPair(demandPair4, node2, node3, MockMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(dominatedEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterEdgeContraction()
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
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node3, node4, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node1, node0);
            DemandPair demandPair2 = new DemandPair(node4, node2);
            DemandPair demandPair3 = new DemandPair(node2, node0);
            DemandPair demandPair4 = new DemandPair(node1, node4);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            algorithm.ContractEdge((node3, node4), MockMeasurements);

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, algorithm, demandPairPerEdge);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(dominatedEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }
    }
}
