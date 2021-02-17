// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.Graphs;
using MulticutInTrees.Exceptions;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Algorithms
{
    [TestClass]
    public class UnitTestGuoNiedermeierBranching
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            List<DemandPair> demandPairs = new List<DemandPair>();

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, 1);
            Assert.IsNotNull(gnb);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            List<DemandPair> demandPairs = new List<DemandPair>();

            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(null, demandPairs, 1); });
            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, null, 1); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, -98504); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, 0); });

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, 1);
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

            tree.AddRoot(node1);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4 });
            tree.AddChildren(node2, new List<TreeNode>() { node5, node6 });
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9 });
            tree.AddChild(node4, node10);

            DemandPair dp1 = new DemandPair(node1, node5);
            DemandPair dp2 = new DemandPair(node7, node8);
            DemandPair dp3 = new DemandPair(node4, node10);
            DemandPair dp4 = new DemandPair(node6, node3);
            List<DemandPair> demandPairs = new List<DemandPair>() { dp1, dp2, dp3, dp4 };

            int k = 3;

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, k);
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

            tree.AddRoot(node1);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4 });
            tree.AddChildren(node2, new List<TreeNode>() { node5, node6 });
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9 });
            tree.AddChild(node4, node10);

            DemandPair dp1 = new DemandPair(node1, node6);
            DemandPair dp2 = new DemandPair(node4, node5);
            DemandPair dp3 = new DemandPair(node8, node10);
            List<DemandPair> demandPairs = new List<DemandPair>() { dp1, dp2, dp3 };

            int k = 2;

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, k);
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

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3, node4, node5, node6 });
            tree.AddChildren(node1, new List<TreeNode>() { node7, node8, node9, node10, node11 });

            DemandPair dp1 = new DemandPair(node2, node11);
            DemandPair dp2 = new DemandPair(node4, node9);
            DemandPair dp3 = new DemandPair(node2, node8);
            List<DemandPair> demandPairs = new List<DemandPair>() { dp1, dp2, dp3 };

            int k = 1;

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, k);
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

            tree.AddRoot(node1);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4 });
            tree.AddChildren(node2, new List<TreeNode>() { node5, node6 });
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9 });
            tree.AddChild(node4, node10);
            tree.AddChildren(node5, new List<TreeNode>() { node11, node12 });
            tree.AddChildren(node6, new List<TreeNode>() { node13, node14 });
            tree.AddChild(node7, node15);
            tree.AddChildren(node10, new List<TreeNode>() { node16, node17 });

            DemandPair dp1 = new DemandPair(node1, node13);
            DemandPair dp2 = new DemandPair(node4, node5);
            DemandPair dp3 = new DemandPair(node7, node15);
            DemandPair dp4 = new DemandPair(node8, node10);
            DemandPair dp5 = new DemandPair(node11, node17);
            List<DemandPair> demandPairs = new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 };

            int k = 3;

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, k);
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

            tree.AddRoot(node1);
            tree.AddChildren(node1, new List<TreeNode>() { node2, node3, node4, node5, node6 });

            DemandPair dp1 = new DemandPair(node1, node2);
            DemandPair dp2 = new DemandPair(node3, node1);
            DemandPair dp3 = new DemandPair(node1, node4);
            DemandPair dp4 = new DemandPair(node1, node5);
            DemandPair dp5 = new DemandPair(node1, node6);
            List<DemandPair> demandPairs = new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 };

            int k = 4;

            GuoNiedermeierBranching gnb = new GuoNiedermeierBranching(tree, demandPairs, k);
            (bool, List<(TreeNode, TreeNode)>) solution = gnb.Run();

            Assert.IsFalse(solution.Item1);
        }
    }
}
