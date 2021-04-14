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
    public class UnitTestOverloadedEdge
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestOverloadedEdge));

        private OverloadedEdge GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(OverloadedEdge))
                {
                    return (OverloadedEdge)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(OverloadedEdge)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            OverloadedEdge overloadedEdge = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            Assert.IsNotNull(overloadedEdge);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(null, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, null, algorithm, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, demandPairs, null, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, demandPairs, algorithm, null, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, null); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, -56, demandPairsPerEdge); });
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
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node0, node2);
            DemandPair dp2 = new DemandPair(2, node0, node3);
            DemandPair dp3 = new DemandPair(3, node0, node4);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(overloadedEdge.RunFirstIteration());
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
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node0, node2);
            DemandPair dp2 = new DemandPair(2, node0, node3);
            DemandPair dp3 = new DemandPair(3, node0, node4);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedEdge.RunLaterIteration());
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
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.AddChild(node4, node5, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node0, node2);
            DemandPair dp2 = new DemandPair(2, node0, node3);
            DemandPair dp3 = new DemandPair(3, node0, node5);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedEdge.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp3, node5, node4, MockMeasurements);

            Assert.IsTrue(overloadedEdge.RunLaterIteration());
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
            tree.AddChild(node0, node5, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node5, node2);
            DemandPair dp2 = new DemandPair(2, node5, node3);
            DemandPair dp3 = new DemandPair(3, node5, node1);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            algorithm.RemoveDemandPair(dp2, MockMeasurements);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedEdge.RunLaterIteration());
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
            tree.AddChild(node0, node5, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node5, node2);
            DemandPair dp2 = new DemandPair(2, node5, node3);
            DemandPair dp3 = new DemandPair(3, node5, node1);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedEdge.RunFirstIteration());

            algorithm.ContractEdge((node0, node1), MockMeasurements);

            Assert.IsTrue(overloadedEdge.RunLaterIteration());
        }
    }
}
