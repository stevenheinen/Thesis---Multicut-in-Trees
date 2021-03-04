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
    public class UnitTestIdleEdge
    {
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node0, node1), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node1, node4), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), demandPairPerEdge);
            Assert.IsNotNull(idleEdge);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node0, node1), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node1, node4), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) }
            }, counter);

            Random random = new Random(674648);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), demandPairPerEdge);

            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(null, demandPairs, new GuoNiedermeierFPT(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, null, new GuoNiedermeierFPT(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), null); });

            Assert.ThrowsException<ArgumentNullException>(() => idleEdge.AfterDemandPathChanged(null));
            Assert.ThrowsException<ArgumentNullException>(() => idleEdge.AfterDemandPathRemove(null));
            Assert.ThrowsException<ArgumentNullException>(() => idleEdge.AfterEdgeContraction(null));

            MethodInfo method = typeof(IdleEdge).GetMethod("CanEdgeBeContracted", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => { method.Invoke(idleEdge, new object[] { new ValueTuple<TreeNode, TreeNode>(null, node1) }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => { method.Invoke(idleEdge, new object[] { new ValueTuple<TreeNode, TreeNode>(node0, null) }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node0, node1), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node1, node4), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), demandPairPerEdge);

            Assert.IsFalse(idleEdge.AfterEdgeContraction(new CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)>(new List<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)>() { ((node1, node3), node1, demandPairs) }, counter)));
        }

        [TestMethod]
        public void TestAfterDemandPathRemove()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node0, node1);
            DemandPair demandPair3 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair1 }, counter) },
                {(node0, node1), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair1 }, counter) },
                {(node1, node4), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair1 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), demandPairPerEdge);

            Assert.IsFalse(idleEdge.AfterDemandPathRemove(new CountedList<DemandPair>(new List<DemandPair>() { demandPair2 }, counter)));
            Assert.IsTrue(idleEdge.AfterDemandPathRemove(new CountedList<DemandPair>(new List<DemandPair>() { demandPair3 }, counter)));
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node0);
            DemandPair demandPair2 = new DemandPair(node2, node1);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair1 }, counter) },
                {(node0, node1), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair2 }, counter) },
                {(node1, node2), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair2 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), demandPairPerEdge);

            Assert.IsTrue(idleEdge.AfterDemandPathChanged(new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>() { (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)> { (node1, node4) }, counter), demandPair2) }, counter)));
            Assert.IsFalse(idleEdge.AfterDemandPathChanged(new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>() { (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)> { (node1, node2) }, counter), demandPair1) }, counter)));
        }

        [TestMethod]
        public void TestFirstIterationFalse()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.UpdateNodeTypes();
            DemandPair demandPair = new DemandPair(node0, node1);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node1), new CountedList<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
            }, counter);

            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), demandPairPerEdge);
            Assert.IsFalse(idleEdge.RunFirstIteration());
        }
    }
}
