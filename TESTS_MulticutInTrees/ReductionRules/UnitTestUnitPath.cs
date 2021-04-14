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
    public class UnitTestUnitPath
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestUnitPath));

        private UnitPath GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(UnitPath))
                {
                    return (UnitPath)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(UnitPath)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 0);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt);

            Assert.IsNotNull(unitPath);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 0);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(null, demandPairs, gnfpt); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(tree, null, gnfpt); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(tree, demandPairs, null); });
        }

        [TestMethod]
        public void TestAfterEdgeContraction1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node2 = new TreeNode(2);
            TreeNode node1 = new TreeNode(1);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node3, node4, MockCounter);
            tree.AddChild(node3, node5, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node4);
            DemandPair dp2 = new DemandPair(2, node1, node3);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ContractEdge((node1, node2), MockMeasurements);

            Assert.IsTrue(unitPath.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterEdgeContraction2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node2 = new TreeNode(2);
            TreeNode node1 = new TreeNode(1);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node3, node4, MockCounter);
            tree.AddChild(node3, node5, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node4);
            DemandPair dp2 = new DemandPair(2, node1, node3);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ContractEdge((node3, node4), MockMeasurements);

            Assert.IsFalse(unitPath.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathRemove()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node3 = new TreeNode(3);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node3, node5, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node5);
            DemandPair dp2 = new DemandPair(2, node1, node3);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(dp1, MockMeasurements);

            Assert.IsFalse(unitPath.RunLaterIteration());
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
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node3, node4, MockCounter);
            tree.AddChild(node3, node5, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node4);
            DemandPair dp2 = new DemandPair(2, node1, node3);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(dp1, node4, node3, MockMeasurements);

            Assert.IsFalse(unitPath.RunLaterIteration());
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
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node3, node4, MockCounter);
            tree.AddChild(node3, node5, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node4);
            DemandPair dp2 = new DemandPair(2, node1, node3);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(dp2, node3, node2, MockMeasurements);

            Assert.IsTrue(unitPath.RunLaterIteration());
        }

        [TestMethod]
        public void TestRunFirstIteration1()
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
            tree.AddChild(node3, node4, MockCounter);
            tree.AddChild(node3, node5, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(0, node0, node4);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(unitPath.RunFirstIteration());
        }

        [TestMethod]
        public void TestRunFirstIteration2()
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
            tree.AddChild(node3, node4, MockCounter);
            tree.AddChild(node3, node5, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp2 = new DemandPair(0, node1, node2);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(unitPath.RunFirstIteration());
        }
    }
}
