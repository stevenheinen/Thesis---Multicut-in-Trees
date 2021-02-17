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
    public class UnitTestIdleEdge
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair } },
                {(node0, node1), new List<DemandPair>(){ demandPair } },
                {(node1, node4), new List<DemandPair>(){ demandPair } }
            };

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
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

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair } },
                {(node0, node1), new List<DemandPair>(){ demandPair } },
                {(node1, node4), new List<DemandPair>(){ demandPair } }
            };

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);

            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(null, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, null, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), null); });

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

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair } },
                {(node0, node1), new List<DemandPair>(){ demandPair } },
                {(node1, node4), new List<DemandPair>(){ demandPair } }
            };

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);

            Assert.IsFalse(idleEdge.AfterEdgeContraction(new List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)>() { ((node1, node3), node1, demandPairs) }));
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

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node0, node1);
            DemandPair demandPair3 = new DemandPair(node1, node3);
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair1 } },
                {(node0, node1), new List<DemandPair>(){ demandPair1 } },
                {(node1, node4), new List<DemandPair>(){ demandPair1 } }
            };

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);

            Assert.IsFalse(idleEdge.AfterDemandPathRemove(new List<DemandPair>() { demandPair2 }));
            Assert.IsTrue(idleEdge.AfterDemandPathRemove(new List<DemandPair>() { demandPair3 }));
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

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node0);
            DemandPair demandPair2 = new DemandPair(node2, node1);
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair1 } },
                {(node0, node1), new List<DemandPair>(){ demandPair2 } },
                {(node1, node2), new List<DemandPair>(){ demandPair2 } }
            };

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);

            Assert.IsTrue(idleEdge.AfterDemandPathChanged(new List<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)> { (node1, node4) }, demandPair2) }));
            Assert.IsFalse(idleEdge.AfterDemandPathChanged(new List<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)> { (node1, node2) }, demandPair1) }));
        }

        [TestMethod]
        public void TestFirstIterationFalse()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.UpdateNodeTypes();
            DemandPair demandPair = new DemandPair(node0, node1);
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node1), new List<DemandPair>(){ demandPair } },
            };

            IdleEdge idleEdge = new IdleEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 1), demandPairPerEdge);
            Assert.IsFalse(idleEdge.RunFirstIteration());
        }
    }
}
