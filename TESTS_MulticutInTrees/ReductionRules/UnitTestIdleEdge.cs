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
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair } }
            };

            Random random = new Random(5644687);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
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
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair } }
            };

            Random random = new Random(674648);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);

            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(null, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, null, new GuoNiedermeierFPT(instance), random, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, null, random, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, null); });

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
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair } }
            };

            Random random = new Random(6453422);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);

            Assert.IsFalse(idleEdge.AfterEdgeContraction(new CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)>() { ((node1, node3), node1, demandPairs) }));
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
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair1 } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair1 } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair1 } }
            };

            Random random = new Random(5468763);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);

            Assert.IsFalse(idleEdge.AfterDemandPathRemove(new CountedList<DemandPair>() { demandPair2 }));
            Assert.IsTrue(idleEdge.AfterDemandPathRemove(new CountedList<DemandPair>() { demandPair3 }));
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
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair1 } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair2 } },
                {(node1, node2), new CountedList<DemandPair>(){ demandPair2 } }
            };

            Random random = new Random(35468468);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);

            Assert.IsTrue(idleEdge.AfterDemandPathChanged(new CountedList<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)> { (node1, node4) }, demandPair2) }));
            Assert.IsFalse(idleEdge.AfterDemandPathChanged(new CountedList<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)> { (node1, node2) }, demandPair1) }));
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
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node1), new CountedList<DemandPair>(){ demandPair } },
            };

            Random random = new Random(74);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 1, random);
            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsFalse(idleEdge.RunFirstIteration());
        }
    }
}
