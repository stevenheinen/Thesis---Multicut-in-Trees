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
    public class UnitTestIdleEdge
    {
        private static readonly PerformanceMeasurements MockPerformanceMeasurements = new PerformanceMeasurements(nameof(UnitTestIdleEdge));
        private static readonly Counter counter = new Counter();

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
            return algorithm.ReductionRules.IndexOf(algorithm.ReductionRules.First(rr => rr.GetType() == typeof(IdleEdge)));
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsNotNull(idleEdge);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) }
            }, counter);

            Random random = new Random(674648);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeNodeTupleList = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>();
            CountedList<DemandPair> removedDemandPairs = new CountedList<DemandPair>();
            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>();

            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(null, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, null, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), null); });

            Assert.ThrowsException<ArgumentNullException>(() => idleEdge.RunLaterIteration(null, removedDemandPairs, changedEdgesPerDemandPairList));
            Assert.ThrowsException<ArgumentNullException>(() => idleEdge.RunLaterIteration(contractedEdgeNodeTupleList, null, changedEdgesPerDemandPairList));
            Assert.ThrowsException<ArgumentNullException>(() => idleEdge.RunLaterIteration(contractedEdgeNodeTupleList, removedDemandPairs, null));

            MethodInfo method = typeof(IdleEdge).GetMethod("CanEdgeBeContracted", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => { method.Invoke(idleEdge, new object[] { new ValueTuple<TreeNode, TreeNode>(null, node1) }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => { method.Invoke(idleEdge, new object[] { new ValueTuple<TreeNode, TreeNode>(node0, null) }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, algorithm, demandPairsPerEdge);
            algorithm.ContractEdge((node1, node3), MockPerformanceMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(idleEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterDemandPathRemove1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node0, node1);
            DemandPair demandPair3 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, algorithm, demandPairsPerEdge);

            algorithm.RemoveDemandPair(demandPair3, MockPerformanceMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsTrue(idleEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterDemandPathRemove2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node0, node1);
            DemandPair demandPair3 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, algorithm, demandPairsPerEdge);

            algorithm.RemoveDemandPair(demandPair2, MockPerformanceMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(idleEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node3, node0);
            DemandPair demandPair2 = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, algorithm, demandPairsPerEdge);

            algorithm.ChangeEndpointOfDemandPair(demandPair1, node3, node1, MockPerformanceMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsTrue(idleEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node3, node0);
            DemandPair demandPair2 = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, algorithm, demandPairsPerEdge);

            algorithm.ChangeEndpointOfDemandPair(demandPair1, node0, node1, MockPerformanceMeasurements);

            (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>, CountedList<DemandPair>, CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>) info = GetLaterIterationInformation(algorithm);

            Assert.IsFalse(idleEdge.RunLaterIteration(info.Item1, info.Item2, info.Item3));
        }

        [TestMethod]
        public void TestFirstIterationFalse()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.UpdateNodeTypes();
            DemandPair demandPair = new DemandPair(node0, node1);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, algorithm, demandPairsPerEdge);

            Assert.IsFalse(idleEdge.RunFirstIteration());
        }
    }
}
