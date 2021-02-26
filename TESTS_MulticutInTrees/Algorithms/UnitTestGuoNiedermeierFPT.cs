// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Exceptions;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Algorithms
{
    [TestClass]
    public class UnitTestGuoNiedermeierFPT
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 2, new Random(7));
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            Assert.IsNotNull(gnfpt);
            Assert.IsNotNull(gnfpt.ReductionRules);

            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierFPT).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(demandPairsProperty.GetGetMethod(true));
            Assert.AreEqual(demandPairs, demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, new object[] { }));

            PropertyInfo inputProperty = typeof(GuoNiedermeierFPT).GetProperty("Tree", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(inputProperty.GetGetMethod(true));
            Assert.AreEqual(tree, inputProperty.GetGetMethod(true).Invoke(gnfpt, new object[] { }));

            PropertyInfo partialSolutionProperty = typeof(GuoNiedermeierFPT).GetProperty("PartialSolution", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(partialSolutionProperty.GetGetMethod(true));

            PropertyInfo lastIterationEdgeContractionProperty = typeof(GuoNiedermeierFPT).GetProperty("LastIterationEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(lastIterationEdgeContractionProperty.GetGetMethod(true));

            PropertyInfo lastIterationDemandPairRemovalProperty = typeof(GuoNiedermeierFPT).GetProperty("LastIterationDemandPairRemoval", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(lastIterationDemandPairRemovalProperty.GetGetMethod(true));

            PropertyInfo lastIterationDemandPairChangeProperty = typeof(GuoNiedermeierFPT).GetProperty("LastIterationDemandPairChange", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(lastIterationDemandPairChangeProperty.GetGetMethod(true));

            PropertyInfo kProperty = typeof(GuoNiedermeierFPT).GetProperty("K", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(kProperty.GetGetMethod(true));

            PropertyInfo lastContractedEdgesProperty = typeof(GuoNiedermeierFPT).GetProperty("LastContractedEdges", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(lastContractedEdgesProperty.GetGetMethod(true));

            PropertyInfo lastRemovedDemandPairsProperty = typeof(GuoNiedermeierFPT).GetProperty("LastRemovedDemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(lastRemovedDemandPairsProperty.GetGetMethod(true));

            PropertyInfo lastChangedEdgesPerDemandPairProperty = typeof(GuoNiedermeierFPT).GetProperty("LastChangedEdgesPerDemandPair", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(lastChangedEdgesPerDemandPairProperty.GetGetMethod(true));
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Random random = new Random(238512352);
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            TreeNode node = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            DemandPair dp = new DemandPair(node1, node2);

            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierFPT g = new GuoNiedermeierFPT(null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(null, node, node); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(dp, null, node); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(dp, node, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPairs(null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge((null, node)); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge((node, null)); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge((null, null)); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdges(null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge((null, node)); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge((node, null)); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge((null, null)); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdges(null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPair(null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPairs(null); });
        }

        [TestMethod]
        public void TestDemandPathsPerEdge()
        {
            Random random = new Random(587);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChild(node1, node4);
            tree.AddChild(node2, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node3, node4);
            DemandPair dp2 = new DemandPair(node5, node2);
            DemandPair dp3 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            MulticutInstance instance = new MulticutInstance(tree, dps, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            PropertyInfo dictProperty = typeof(GuoNiedermeierFPT).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> dict = (CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>)dictProperty.GetGetMethod(true).Invoke(gnfpt, new object[0]);
            Assert.AreEqual(2, dict[(node2, node5)].Count);
            Assert.AreEqual(1, dict[(node0, node2)].Count);
            Assert.AreEqual(2, dict[(node0, node1)].Count);
        }

        [TestMethod]
        public void TestRemoveDemandPairFromEdge()
        {
            Random random = new Random(153);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChild(node1, node4);
            tree.AddChild(node2, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node3, node4);
            DemandPair dp2 = new DemandPair(node5, node2);
            DemandPair dp3 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            MulticutInstance instance = new MulticutInstance(tree, dps, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            PropertyInfo dictProperty = typeof(GuoNiedermeierFPT).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> dict = (CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>)dictProperty.GetGetMethod(true).Invoke(gnfpt, new object[0]);

            MethodInfo method = typeof(GuoNiedermeierFPT).GetMethod("RemoveDemandPairFromEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(gnfpt, new object[] { (node1, node0), dp3 });
            Assert.AreEqual(1, dict[(node0, node1)].Count);
            Assert.AreEqual(dp1, dict[(node0, node1)][0]);

            method.Invoke(gnfpt, new object[] { (node0, node3), dp1 });
            Assert.IsFalse(dict.ContainsKey((node0, node3)));
        }

        [TestMethod]
        public void TestCutEdge()
        {
            Random random = new Random(64823);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChild(node1, node4);
            tree.AddChild(node2, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node3, node4);
            DemandPair dp2 = new DemandPair(node5, node2);
            DemandPair dp3 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            MulticutInstance instance = new MulticutInstance(tree, dps, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            gnfpt.CutEdge((node5, node2));

            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierFPT).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(1, ((CountedList<DemandPair>)demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, new object[] { })).Count);
        }

        [TestMethod]
        public void TestCutEdges()
        {
            Random random = new Random(1359);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChild(node1, node4);
            tree.AddChild(node2, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node3, node4);
            DemandPair dp2 = new DemandPair(node5, node2);
            DemandPair dp3 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            MulticutInstance instance = new MulticutInstance(tree, dps, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            gnfpt.CutEdges(new List<(TreeNode, TreeNode)>() { (node0, node1), (node5, node2) });
            
            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierFPT).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(0, ((CountedList<DemandPair>)demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, new object[] { })).Count);
        }

        [TestMethod]
        public void TestContractEdge()
        {
            Random random = new Random(563135645);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChild(node1, node4);
            tree.AddChild(node2, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node3, node4);
            DemandPair dp2 = new DemandPair(node5, node2);
            DemandPair dp3 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            MulticutInstance instance = new MulticutInstance(tree, dps, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            gnfpt.ContractEdge((node0, node2));

            Assert.AreEqual(5, tree.NumberOfNodes);
        }

        [TestMethod]
        public void TestContractEdges()
        {
            Random random = new Random(9684357);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChild(node1, node4);
            tree.AddChild(node2, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node3, node4);
            DemandPair dp2 = new DemandPair(node5, node2);
            DemandPair dp3 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            MulticutInstance instance = new MulticutInstance(tree, dps, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            gnfpt.ContractEdges(new List<(TreeNode, TreeNode)>() { (node0, node2), (node4, node1) });

            Assert.AreEqual(4, tree.NumberOfNodes);
        }

        [TestMethod]
        public void TestChangeEndpointOfDemandPair()
        {
            Random random = new Random(8323);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChild(node1, node4);
            tree.AddChild(node2, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node3, node4);
            DemandPair dp2 = new DemandPair(node5, node2);
            DemandPair dp3 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            MulticutInstance instance = new MulticutInstance(tree, dps, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            gnfpt.ChangeEndpointOfDemandPair(dp3, node5, node2);
            Assert.AreEqual(3, dp3.EdgesOnDemandPath.Count);

            gnfpt.ChangeEndpointOfDemandPair(dp1, node4, node0);
            Assert.AreEqual(1, dp1.EdgesOnDemandPath.Count);
        }

        [TestMethod]
        public void TestChangeEndpointOfDemandPairs()
        {
            Random random = new Random(139);

            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChild(node1, node4);
            tree.AddChild(node2, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node3, node4);
            DemandPair dp2 = new DemandPair(node5, node2);
            DemandPair dp3 = new DemandPair(node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            MulticutInstance instance = new MulticutInstance(tree, dps, 100, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            gnfpt.ChangeEndpointOfDemandPairs(new List<(DemandPair, TreeNode, TreeNode)>(){ (dp3, node5, node2), (dp1, node4, node0) });
            Assert.AreEqual(3, dp3.EdgesOnDemandPath.Count);
            Assert.AreEqual(1, dp1.EdgesOnDemandPath.Count);
        }
    }
}
