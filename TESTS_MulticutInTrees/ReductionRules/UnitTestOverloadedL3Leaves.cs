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

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestOverloadedL3Leaves
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestOverloadedCaterpillar));

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = new CountedDictionary<TreeNode, CountedCollection<DemandPair>>();

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            Assert.IsNotNull(overloadedL3Leaves);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = new CountedDictionary<TreeNode, CountedCollection<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(null, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, null, algorithm, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, null, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, null, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, null, maxSize); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, -1); });

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            Assert.ThrowsException<ArgumentNullException>(() => overloadedL3Leaves.AfterDemandPathChanged(null));
            Assert.ThrowsException<ArgumentNullException>(() => overloadedL3Leaves.AfterDemandPathRemove(null));
            Assert.ThrowsException<ArgumentNullException>(() => overloadedL3Leaves.AfterEdgeContraction(null));
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
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);
            TreeNode node11 = new TreeNode(11);
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node5, MockCounter);
            tree.AddChild(node0, node9, MockCounter);
            tree.AddChild(node0, node11, MockCounter);
            tree.AddChild(node0, node12, MockCounter);
            tree.AddChild(node0, node13, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node2, node4, MockCounter);
            tree.AddChild(node5, node6, MockCounter);
            tree.AddChild(node6, node7, MockCounter);
            tree.AddChild(node6, node8, MockCounter);
            tree.AddChild(node9, node10, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node2, node11);
            DemandPair dp2 = new DemandPair(node2, node12);
            DemandPair dp3 = new DemandPair(node13, node2);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            Assert.IsTrue(overloadedL3Leaves.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPairChanged()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> info = (CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>)typeof(Algorithm).GetProperty("LastChangedEdgesPerDemandPair", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsFalse(overloadedL3Leaves.AfterDemandPathChanged(info));
        }

        [TestMethod]
        public void TestAfterDemandPairRemoved()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            CountedList<DemandPair> info = (CountedList<DemandPair>)typeof(Algorithm).GetProperty("LastRemovedDemandPairs", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsFalse(overloadedL3Leaves.AfterDemandPathRemove(info));
        }

        [TestMethod]
        public void TestAfterI2EdgeContraction()
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
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node5, MockCounter);
            tree.AddChild(node0, node9, MockCounter);
            tree.AddChild(node0, node11, MockCounter);
            tree.AddChild(node0, node12, MockCounter);
            tree.AddChild(node0, node13, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node2, node4, MockCounter);
            tree.AddChild(node5, node6, MockCounter);
            tree.AddChild(node6, node7, MockCounter);
            tree.AddChild(node6, node8, MockCounter);
            tree.AddChild(node9, node10, MockCounter);
            tree.AddChild(node4, node14, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node3, node0);
            DemandPair dp2 = new DemandPair(node14, node0);
            DemandPair dp3 = new DemandPair(node13, node7);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();

            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.ContractEdge((node4, node14), MockMeasurements);
            
            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> info = (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>)typeof(Algorithm).GetProperty("LastContractedEdges", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsFalse(overloadedL3Leaves.AfterEdgeContraction(info));
        }

        [TestMethod]
        public void TestAfterI3EdgeContraction()
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
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node5, MockCounter);
            tree.AddChild(node0, node9, MockCounter);
            tree.AddChild(node0, node11, MockCounter);
            tree.AddChild(node0, node12, MockCounter);
            tree.AddChild(node0, node13, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node2, node4, MockCounter);
            tree.AddChild(node5, node6, MockCounter);
            tree.AddChild(node6, node7, MockCounter);
            tree.AddChild(node6, node8, MockCounter);
            tree.AddChild(node9, node10, MockCounter);
            tree.AddChild(node13, node14, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node5, node11);
            DemandPair dp2 = new DemandPair(node12, node5);
            DemandPair dp3 = new DemandPair(node14, node5);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();

            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.ContractEdge((node13, node14), MockMeasurements);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> info = (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>)typeof(Algorithm).GetProperty("LastContractedEdges", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsTrue(overloadedL3Leaves.AfterEdgeContraction(info));
        }
    }
}
