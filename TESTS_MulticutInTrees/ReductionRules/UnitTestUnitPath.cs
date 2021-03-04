// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
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
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt);

            Assert.IsNotNull(unitPath);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt);

            Assert.IsNotNull(unitPath);

            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(null, demandPairs, gnfpt); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(tree, null, gnfpt); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new UnitPath(tree, demandPairs, null); });

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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node3, counter);
            tree.AddChild(node3, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node5);
            DemandPair dp2 = new DemandPair(node1, node3);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, counter);

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)> list = new CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)>();
            CountedList<DemandPair> dpList = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, counter);
            list.Add(((node2, node1), node1, dpList), counter);
            Assert.IsTrue(unitPath.AfterEdgeContraction(list));

            list = new CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)>(new List<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)> { ((node4, node5), node5, new CountedList<DemandPair>(new List<DemandPair>(){ dp1 }, counter)) }, counter);
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node3, counter);
            tree.AddChild(node3, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node5);
            DemandPair dp2 = new DemandPair(node1, node3);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, counter);

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt);

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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChild(node2, node3, counter);
            tree.AddChild(node3, node4, counter);
            tree.AddChild(node3, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node4);
            DemandPair dp2 = new DemandPair(node1, node2);

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, counter);

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            UnitPath unitPath = new UnitPath(tree, demandPairs, gnfpt);

            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> list = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>() { (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)>() { (node4, node5) }, counter), dp1) }, counter);
            Assert.IsFalse(unitPath.AfterDemandPathChanged(list));

            list = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>() { (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)> { (node3, node2) }, counter), dp2) }, counter);
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChild(node2, node3, counter);
            tree.AddChild(node3, node4, counter);
            tree.AddChild(node3, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node4);
            DemandPair dp2 = new DemandPair(node1, node2);

            CountedList<DemandPair> demandPairs1 = new CountedList<DemandPair>(new List<DemandPair>() { dp1 }, counter);
            CountedList<DemandPair> demandPairs2 = new CountedList<DemandPair>(new List<DemandPair>() { dp2 }, counter);

            MulticutInstance instance1 = new MulticutInstance(tree, demandPairs1, 1);
            MulticutInstance instance2 = new MulticutInstance(tree, demandPairs2, 1);
            GuoNiedermeierFPT gnfpt1 = new GuoNiedermeierFPT(instance1);
            GuoNiedermeierFPT gnfpt2 = new GuoNiedermeierFPT(instance2);

            UnitPath unitPath1 = new UnitPath(tree, demandPairs1, gnfpt1);
            UnitPath unitPath2 = new UnitPath(tree, demandPairs2, gnfpt2);

            Assert.IsFalse(unitPath1.RunFirstIteration());
            Assert.IsTrue(unitPath2.RunFirstIteration());
        }
    }
}
