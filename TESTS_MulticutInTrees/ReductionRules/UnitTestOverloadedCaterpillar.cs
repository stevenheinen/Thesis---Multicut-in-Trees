﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestOverloadedCaterpillar
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestOverloadedCaterpillar));

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = new CountedDictionary<TreeNode, CountedCollection<DemandPair>>();
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = new CountedDictionary<TreeNode, int>();

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize);

            Assert.IsNotNull(overloadedCaterpillar);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = new CountedDictionary<TreeNode, CountedCollection<DemandPair>>();
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = new CountedDictionary<TreeNode, int>();

            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(null, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, null, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, null, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, null, caterpillarComponentPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, null, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, null, maxSize); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, -1); });

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize);

            Assert.ThrowsException<ArgumentNullException>(() => overloadedCaterpillar.AfterDemandPathChanged(null));
            Assert.ThrowsException<ArgumentNullException>(() => overloadedCaterpillar.AfterDemandPathRemove(null));
            Assert.ThrowsException<ArgumentNullException>(() => overloadedCaterpillar.AfterEdgeContraction(null));
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
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);
            tree.AddRoot(node5, MockCounter);
            tree.AddChild(node5, node4, MockCounter);
            tree.AddChild(node4, node3, MockCounter);
            tree.AddChild(node4, node14, MockCounter);
            tree.AddChild(node3, node2, MockCounter);
            tree.AddChild(node3, node12, MockCounter);
            tree.AddChild(node3, node13, MockCounter);
            tree.AddChild(node2, node1, MockCounter);
            tree.AddChild(node1, node0, MockCounter);
            tree.AddChild(node1, node11, MockCounter);
            tree.AddChild(node5, node6, MockCounter);
            tree.AddChild(node5, node9, MockCounter);
            tree.AddChild(node5, node15, MockCounter);
            tree.AddChild(node6, node7, MockCounter);
            tree.AddChild(node7, node8, MockCounter);
            tree.AddChild(node7, node16, MockCounter);
            tree.AddChild(node7, node17, MockCounter);
            tree.AddChild(node7, node18, MockCounter);
            tree.AddChild(node8, node19, MockCounter);
            tree.AddChild(node8, node20, MockCounter);
            tree.AddChild(node9, node10, MockCounter);
            tree.AddChild(node9, node21, MockCounter);
            tree.AddChild(node10, node22, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node7, node2);
            DemandPair dp2 = new DemandPair(node7, node12);
            DemandPair dp3 = new DemandPair(node7, node13);
            DemandPair dp4 = new DemandPair(node7, node3);
            DemandPair dp5 = new DemandPair(node7, node4);
            DemandPair dp6 = new DemandPair(node7, node9);
            DemandPair dp7 = new DemandPair(node7, node21);
            DemandPair dp8 = new DemandPair(node7, node18);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);
            demandPairs.Add(dp6, MockCounter);
            demandPairs.Add(dp7, MockCounter);
            demandPairs.Add(dp8, MockCounter);

            int maxSize = 4;

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });
            CountedDictionary<TreeNode, int> caterpillarComponentsPerNode = (CountedDictionary<TreeNode, int>)typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentsPerNode, partialSolution, maxSize);

            Assert.IsTrue(overloadedCaterpillar.RunFirstIteration());
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
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);
            tree.AddRoot(node5, MockCounter);
            tree.AddChild(node5, node4, MockCounter);
            tree.AddChild(node4, node3, MockCounter);
            tree.AddChild(node4, node14, MockCounter);
            tree.AddChild(node3, node2, MockCounter);
            tree.AddChild(node3, node12, MockCounter);
            tree.AddChild(node3, node13, MockCounter);
            tree.AddChild(node2, node1, MockCounter);
            tree.AddChild(node1, node0, MockCounter);
            tree.AddChild(node1, node11, MockCounter);
            tree.AddChild(node5, node6, MockCounter);
            tree.AddChild(node5, node9, MockCounter);
            tree.AddChild(node5, node15, MockCounter);
            tree.AddChild(node6, node7, MockCounter);
            tree.AddChild(node7, node8, MockCounter);
            tree.AddChild(node7, node16, MockCounter);
            tree.AddChild(node7, node17, MockCounter);
            tree.AddChild(node7, node18, MockCounter);
            tree.AddChild(node8, node19, MockCounter);
            tree.AddChild(node8, node20, MockCounter);
            tree.AddChild(node9, node10, MockCounter);
            tree.AddChild(node9, node21, MockCounter);
            tree.AddChild(node10, node22, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node7, node2);
            DemandPair dp2 = new DemandPair(node7, node12);
            DemandPair dp3 = new DemandPair(node7, node19);
            DemandPair dp4 = new DemandPair(node7, node3);
            DemandPair dp5 = new DemandPair(node7, node4);
            DemandPair dp6 = new DemandPair(node7, node9);
            DemandPair dp7 = new DemandPair(node7, node21);
            DemandPair dp8 = new DemandPair(node7, node18);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);
            demandPairs.Add(dp6, MockCounter);
            demandPairs.Add(dp7, MockCounter);
            demandPairs.Add(dp8, MockCounter);

            int maxSize = 4;

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });
            CountedDictionary<TreeNode, int> caterpillarComponentsPerNode = (CountedDictionary<TreeNode, int>)typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentsPerNode, partialSolution, maxSize);

            Assert.IsFalse(overloadedCaterpillar.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPairChanged()
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
            TreeNode node15 = new TreeNode(15);
            TreeNode node16 = new TreeNode(16);
            TreeNode node17 = new TreeNode(17);
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);
            tree.AddRoot(node5, MockCounter);
            tree.AddChild(node5, node4, MockCounter);
            tree.AddChild(node4, node3, MockCounter);
            tree.AddChild(node4, node14, MockCounter);
            tree.AddChild(node3, node2, MockCounter);
            tree.AddChild(node3, node12, MockCounter);
            tree.AddChild(node3, node13, MockCounter);
            tree.AddChild(node2, node1, MockCounter);
            tree.AddChild(node1, node0, MockCounter);
            tree.AddChild(node1, node11, MockCounter);
            tree.AddChild(node5, node6, MockCounter);
            tree.AddChild(node5, node9, MockCounter);
            tree.AddChild(node5, node15, MockCounter);
            tree.AddChild(node6, node7, MockCounter);
            tree.AddChild(node7, node8, MockCounter);
            tree.AddChild(node7, node16, MockCounter);
            tree.AddChild(node7, node17, MockCounter);
            tree.AddChild(node7, node18, MockCounter);
            tree.AddChild(node8, node19, MockCounter);
            tree.AddChild(node8, node20, MockCounter);
            tree.AddChild(node9, node10, MockCounter);
            tree.AddChild(node9, node21, MockCounter);
            tree.AddChild(node10, node22, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node7, node1);
            DemandPair dp2 = new DemandPair(node7, node12);
            DemandPair dp3 = new DemandPair(node7, node13);
            DemandPair dp4 = new DemandPair(node7, node3);
            DemandPair dp5 = new DemandPair(node7, node4);
            DemandPair dp6 = new DemandPair(node7, node9);
            DemandPair dp7 = new DemandPair(node7, node21);
            DemandPair dp8 = new DemandPair(node7, node18);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);
            demandPairs.Add(dp6, MockCounter);
            demandPairs.Add(dp7, MockCounter);
            demandPairs.Add(dp8, MockCounter);

            int maxSize = 4;

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });
            CountedDictionary<TreeNode, int> caterpillarComponentsPerNode = (CountedDictionary<TreeNode, int>)typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentsPerNode, partialSolution, maxSize);

            Assert.IsFalse(overloadedCaterpillar.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp1, node1, node2, MockMeasurements);

            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> info = (CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>)typeof(Algorithm).GetProperty("LastChangedEdgesPerDemandPair", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsTrue(overloadedCaterpillar.AfterDemandPathChanged(info));
        }

        [TestMethod]
        public void TestAfterDemandPairRemoved()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = new CountedDictionary<TreeNode, CountedCollection<DemandPair>>();
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = new CountedDictionary<TreeNode, int>();

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize);

            Assert.IsFalse(overloadedCaterpillar.AfterDemandPathRemove(new CountedList<DemandPair>()));
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
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            tree.AddRoot(node5, MockCounter);
            tree.AddChild(node5, node4, MockCounter);
            tree.AddChild(node4, node3, MockCounter);
            tree.AddChild(node4, node14, MockCounter);
            tree.AddChild(node3, node2, MockCounter);
            tree.AddChild(node3, node12, MockCounter);
            tree.AddChild(node3, node13, MockCounter);
            tree.AddChild(node2, node1, MockCounter);
            tree.AddChild(node1, node0, MockCounter);
            tree.AddChild(node1, node11, MockCounter);
            tree.AddChild(node5, node6, MockCounter);
            tree.AddChild(node5, node9, MockCounter);
            tree.AddChild(node5, node15, MockCounter);
            tree.AddChild(node6, node7, MockCounter);
            tree.AddChild(node7, node8, MockCounter);
            tree.AddChild(node7, node16, MockCounter);
            tree.AddChild(node7, node17, MockCounter);
            tree.AddChild(node7, node18, MockCounter);
            tree.AddChild(node8, node19, MockCounter);
            tree.AddChild(node8, node20, MockCounter);
            tree.AddChild(node9, node10, MockCounter);
            tree.AddChild(node9, node21, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node20, node4);
            DemandPair dp2 = new DemandPair(node20, node12);
            DemandPair dp3 = new DemandPair(node20, node6);
            DemandPair dp4 = new DemandPair(node20, node17);
            DemandPair dp5 = new DemandPair(node20, node0);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);

            int maxSize = 3;

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<TreeNode, CountedCollection<DemandPair>> demandPairsPerNode = (CountedDictionary<TreeNode, CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });
            CountedDictionary<TreeNode, int> caterpillarComponentsPerNode = (CountedDictionary<TreeNode, int>)typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentsPerNode, partialSolution, maxSize);

            Assert.IsFalse(overloadedCaterpillar.RunFirstIteration());

            algorithm.ContractEdge((node5, node9), MockMeasurements);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> info = (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>)typeof(Algorithm).GetProperty("LastContractedEdges", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsTrue(overloadedCaterpillar.AfterEdgeContraction(info));
        }
    }
}
