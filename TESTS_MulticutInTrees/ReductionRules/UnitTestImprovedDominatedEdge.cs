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
    public class UnitTestImprovedDominatedEdge
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestImprovedDominatedEdge));

        private ImprovedDominatedEdge GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(ImprovedDominatedEdge))
                {
                    return (ImprovedDominatedEdge)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(ImprovedDominatedEdge)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
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

            DemandPair demandPair = new DemandPair(0, node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, MockCounter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) }
            }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            ImprovedDominatedEdge dominatedEdge = new ImprovedDominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
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

            DemandPair demandPair = new DemandPair(0, node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, MockCounter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) }
            }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Assert.ThrowsException<ArgumentNullException>(() => { ImprovedDominatedEdge de = new ImprovedDominatedEdge(null, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { ImprovedDominatedEdge de = new ImprovedDominatedEdge(tree, null, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { ImprovedDominatedEdge de = new ImprovedDominatedEdge(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { ImprovedDominatedEdge de = new ImprovedDominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), null); });
        }

        [TestMethod]
        public void TestFirstIteration1()
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

            DemandPair demandPair1 = new DemandPair(1, node2, node4);
            DemandPair demandPair2 = new DemandPair(2, node3, node0);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);
            Assert.IsTrue(dominatedEdge.RunFirstIteration());
        }

        [TestMethod]
        public void TestFirstIteration2()
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

            DemandPair demandPair1 = new DemandPair(1, node1, node4);
            DemandPair demandPair2 = new DemandPair(2, node3, node1);
            DemandPair demandPair3 = new DemandPair(3, node0, node1);
            DemandPair demandPair4 = new DemandPair(4, node0, node2);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

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

            DemandPair demandPair1 = new DemandPair(1, node2, node4);
            DemandPair demandPair2 = new DemandPair(2, node3, node0);
            DemandPair demandPair3 = new DemandPair(3, node2, node1);
            DemandPair demandPair4 = new DemandPair(4, node2, node0);
            DemandPair demandPair5 = new DemandPair(5, node1, node3);
            DemandPair demandPair6 = new DemandPair(5, node1, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(demandPair3, MockMeasurements);

            Assert.IsFalse(dominatedEdge.RunLaterIteration());
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

            DemandPair demandPair1 = new DemandPair(1, node2, node4);
            DemandPair demandPair2 = new DemandPair(2, node3, node0);
            DemandPair demandPair4 = new DemandPair(3, node2, node0);
            DemandPair demandPair5 = new DemandPair(4, node1, node3);
            DemandPair demandPair6 = new DemandPair(5, node1, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(demandPair4, MockMeasurements);

            Assert.IsTrue(dominatedEdge.RunLaterIteration());
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

            DemandPair demandPair1 = new DemandPair(1, node2, node4);
            DemandPair demandPair2 = new DemandPair(2, node1, node4);
            DemandPair demandPair3 = new DemandPair(3, node2, node1);
            DemandPair demandPair4 = new DemandPair(4, node2, node0);
            DemandPair demandPair5 = new DemandPair(5, node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair3, node1, node0, MockMeasurements);

            Assert.IsTrue(dominatedEdge.RunLaterIteration());
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

            DemandPair demandPair1 = new DemandPair(1, node1, node0);
            DemandPair demandPair2 = new DemandPair(2, node3, node2);
            DemandPair demandPair3 = new DemandPair(3, node2, node0);
            DemandPair demandPair4 = new DemandPair(4, node1, node3);
            DemandPair demandPair5 = new DemandPair(5, node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair3, node0, node1, MockMeasurements);

            Assert.IsFalse(dominatedEdge.RunLaterIteration());
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

            DemandPair demandPair1 = new DemandPair(1, node0, node3);
            DemandPair demandPair2 = new DemandPair(2, node1, node2);
            DemandPair demandPair3 = new DemandPair(3, node0, node1);
            DemandPair demandPair4 = new DemandPair(4, node2, node4);
            DemandPair demandPair5 = new DemandPair(5, node3, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair4, node2, node3, MockMeasurements);

            Assert.IsTrue(dominatedEdge.RunLaterIteration());
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

            DemandPair demandPair1 = new DemandPair(1, node0, node3);
            DemandPair demandPair2 = new DemandPair(2, node1, node2);
            DemandPair demandPair3 = new DemandPair(3, node0, node1);
            DemandPair demandPair4 = new DemandPair(4, node2, node4);
            DemandPair demandPair5 = new DemandPair(5, node3, node4);
            DemandPair demandPair6 = new DemandPair(6, node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair4, node2, node3, MockMeasurements);

            Assert.IsFalse(dominatedEdge.RunLaterIteration());
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

            DemandPair demandPair1 = new DemandPair(1, node1, node0);
            DemandPair demandPair2 = new DemandPair(2, node4, node2);
            DemandPair demandPair3 = new DemandPair(3, node2, node0);
            DemandPair demandPair4 = new DemandPair(4, node1, node4);
            DemandPair demandPair5 = new DemandPair(5, node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new ImprovedGuoNiedermeierKernelisation(instance);

            algorithm.ContractEdge((node3, node4), MockMeasurements);

            ImprovedDominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(dominatedEdge.RunLaterIteration());
        }
    }
}
