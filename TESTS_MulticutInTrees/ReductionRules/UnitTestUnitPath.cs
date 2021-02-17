// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestUnitPath
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            List<DemandPair> demandPairs = new List<DemandPair>();
            Random random = new Random(648468);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt, random);

            Assert.IsNotNull(unitPath);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            List<DemandPair> demandPairs = new List<DemandPair>();
            Random random = new Random(8465210);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt, random);

            Assert.IsNotNull(unitPath);

            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(null, demandPairs, gnfpt, random); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(tree, null, gnfpt, random); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(tree, demandPairs, null, random); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(tree, demandPairs, gnfpt, null); });

            Assert.ThrowsException<ArgumentNullException>(() => unitPath.AfterDemandPathChanged(null));
            Assert.ThrowsException<ArgumentNullException>(() => unitPath.AfterDemandPathRemove(null));
            Assert.ThrowsException<ArgumentNullException>(() => unitPath.AfterEdgeContraction(null));

            MethodInfo method = typeof(UnitPath).GetMethod("DemandPathHasLengthOne", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                method.Invoke(unitPath, new object[] { null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestAfterEdgeContraction()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node2 = new TreeNode(2);
            TreeNode node1 = new TreeNode(1);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node3);
            tree.AddChild(node3, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node5);
            DemandPair dp2 = new DemandPair(node1, node3);

            List<DemandPair> demandPairs = new List<DemandPair>() { dp1, dp2 };

            Random random = new Random(68468);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt, random);

            List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)> list = new List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)>() { ((node2, node1), node1, new List<DemandPair>() { dp1, dp2 }) };
            Assert.IsTrue(unitPath.AfterEdgeContraction(list));

            list = new List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)>() { ((node4, node5), node5, new List<DemandPair>() { dp1 }) };
            Assert.IsFalse(unitPath.AfterEdgeContraction(list));
        }

        [TestMethod]
        public void TestAfterDemandPathRemove()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node3 = new TreeNode(3);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node3);
            tree.AddChild(node3, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node5);
            DemandPair dp2 = new DemandPair(node1, node3);

            List<DemandPair> demandPairs = new List<DemandPair>() { dp1, dp2 };

            Random random = new Random(61521645);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt, random);

            Assert.IsFalse(unitPath.AfterDemandPathRemove(demandPairs));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChild(node2, node3);
            tree.AddChild(node3, node4);
            tree.AddChild(node3, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node4);
            DemandPair dp2 = new DemandPair(node1, node2);

            List<DemandPair> demandPairs = new List<DemandPair>() { dp1, dp2 };

            Random random = new Random(6546);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt, random);

            List<(List<(TreeNode, TreeNode)>, DemandPair)> list = new List<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node4, node5) }, dp1) };
            Assert.IsFalse(unitPath.AfterDemandPathChanged(list));

            list = new List<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node3, node2) }, dp2) };
            Assert.IsTrue(unitPath.AfterDemandPathChanged(list));
        }

        [TestMethod]
        public void TestRunFirstIteration()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChild(node2, node3);
            tree.AddChild(node3, node4);
            tree.AddChild(node3, node5);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node4);
            DemandPair dp2 = new DemandPair(node1, node2);

            List<DemandPair> demandPairs1 = new List<DemandPair>() { dp1 };
            List<DemandPair> demandPairs2 = new List<DemandPair>() { dp2 };

            Random random1 = new Random(684);
            Random random2 = new Random(684);
            MulticutInstance instance1 = new MulticutInstance(tree, demandPairs1, 1, random1);
            MulticutInstance instance2 = new MulticutInstance(tree, demandPairs2, 1, random2);
            GuoNiedermeierFPT gnfpt1 = new GuoNiedermeierFPT(instance1);
            GuoNiedermeierFPT gnfpt2 = new GuoNiedermeierFPT(instance2);

            UnitPath unitPath1 = new UnitPath(tree, demandPairs1, gnfpt1, random1);
            UnitPath unitPath2 = new UnitPath(tree, demandPairs2, gnfpt2, random2);

            Assert.IsFalse(unitPath1.RunFirstIteration());
            Assert.IsTrue(unitPath2.RunFirstIteration());
        }
    }
}
