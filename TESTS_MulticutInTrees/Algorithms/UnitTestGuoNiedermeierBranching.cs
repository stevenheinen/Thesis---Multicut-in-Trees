// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.Algorithms
{
    [TestClass]
    public class UnitTestGuoNiedermeierBranching
    {
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            Assert.IsNotNull(gnb);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1);

            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(null); });
        }

        [TestMethod]
        public void TestCase1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
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

            tree.AddRoot(node1, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4 }, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node5, node6 }, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9 }, counter);
            tree.AddChild(node4, node10, counter);

            DemandPair dp1 = new DemandPair(node1, node5);
            DemandPair dp2 = new DemandPair(node7, node8);
            DemandPair dp3 = new DemandPair(node4, node10);
            DemandPair dp4 = new DemandPair(node6, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4 }, counter);

            int k = 3;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, k);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (bool, List<(TreeNode, TreeNode)>) solution = gnb.Run();

            Assert.IsTrue(solution.Item1);
            Assert.AreEqual(3, solution.Item2.Count);
        }

        [TestMethod]
        public void TestCase2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
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

            tree.AddRoot(node1, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4 }, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node5, node6 }, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9 }, counter);
            tree.AddChild(node4, node10, counter);

            DemandPair dp1 = new DemandPair(node1, node6);
            DemandPair dp2 = new DemandPair(node4, node5);
            DemandPair dp3 = new DemandPair(node8, node10);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            int k = 2;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, k);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (bool, List<(TreeNode, TreeNode)>) solution = gnb.Run();

            Assert.IsTrue(solution.Item1);
            Assert.AreEqual(2, solution.Item2.Count);
        }

        [TestMethod]
        public void TestCase3()
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3, node4, node5, node6 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node7, node8, node9, node10, node11 }, counter);

            DemandPair dp1 = new DemandPair(node2, node11);
            DemandPair dp2 = new DemandPair(node4, node9);
            DemandPair dp3 = new DemandPair(node2, node8);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            int k = 1;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, k);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (bool, List<(TreeNode, TreeNode)>) solution = gnb.Run();

            Assert.IsTrue(solution.Item1);
            Assert.AreEqual(1, solution.Item2.Count);
        }

        [TestMethod]
        public void TestCase4()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
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

            tree.AddRoot(node1, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4 }, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node5, node6 }, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9 }, counter);
            tree.AddChild(node4, node10, counter);
            tree.AddChildren(node5, new List<TreeNode>() { node11, node12 }, counter);
            tree.AddChildren(node6, new List<TreeNode>() { node13, node14 }, counter);
            tree.AddChild(node7, node15, counter);
            tree.AddChildren(node10, new List<TreeNode>() { node16, node17 }, counter);

            DemandPair dp1 = new DemandPair(node1, node13);
            DemandPair dp2 = new DemandPair(node4, node5);
            DemandPair dp3 = new DemandPair(node7, node15);
            DemandPair dp4 = new DemandPair(node8, node10);
            DemandPair dp5 = new DemandPair(node11, node17);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, counter);

            int k = 3;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, k);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (bool, List<(TreeNode, TreeNode)>) solution = gnb.Run();

            Assert.IsTrue(solution.Item1);
            Assert.AreEqual(3, solution.Item2.Count);
        }

        [TestMethod]
        public void TestCase5()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);

            tree.AddRoot(node1, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4, node5, node6 }, counter);

            DemandPair dp1 = new DemandPair(node1, node2);
            DemandPair dp2 = new DemandPair(node3, node1);
            DemandPair dp3 = new DemandPair(node1, node4);
            DemandPair dp4 = new DemandPair(node1, node5);
            DemandPair dp5 = new DemandPair(node1, node6);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, counter);

            int k = 4;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, k);

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(instance);
            (bool, List<(TreeNode, TreeNode)>) solution = gnb.Run();

            Assert.IsFalse(solution.Item1);
        }
    }
}
