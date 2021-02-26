// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestDemandPair
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);

            DemandPair dp = new DemandPair(node0, node1);
            Assert.IsNotNull(dp);

            Assert.AreEqual(node0, dp.Node1);
            Assert.AreEqual(node1, dp.Node2);
            Assert.AreEqual(1, dp.EdgesOnDemandPath.Count);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);

            Assert.ThrowsException<ArgumentNullException>(() => { DemandPair _dp = new DemandPair(node0, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { DemandPair _dp = new DemandPair(null, node0); });

            DemandPair dp = new DemandPair(node0, node1);

            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted((null, node0), node0));
            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted((node0, null), node0));
            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted((node1, node0), null));
            Assert.ThrowsException<ArgumentNullException>(() => dp.ChangeEndpoint(node0, null));
            Assert.ThrowsException<ArgumentNullException>(() => dp.ChangeEndpoint(null, node0));

            MethodInfo updateEndpointsAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateEndpointsAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => { updateEndpointsAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(null, node0), node1 }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEndpointsAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node0, null), node1 }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEndpointsAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node1, node0), null }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            //MethodInfo updateNodesOnPathAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateNodesOnPathAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            //t = Assert.ThrowsException<TargetInvocationException>(() => { updateNodesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(null, node0), node1 }); });
            //Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            //t = Assert.ThrowsException<TargetInvocationException>(() => { updateNodesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node0, null), node1 }); });
            //Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            //t = Assert.ThrowsException<TargetInvocationException>(() => { updateNodesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node1, node0), null }); });
            //Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            
            MethodInfo updateEdgesOnPathAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateEdgesOnPathAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(null, node0), node1 }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node0, null), node1 }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node1, node0), null }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestOnEndpointChanged()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChild(node2, node3);

            DemandPair dp = new DemandPair(node0, node3);

            Assert.ThrowsException<NotOnDemandPathException>(() => dp.ChangeEndpoint(node2, node1));

            dp.ChangeEndpoint(node3, node2);
            Assert.AreEqual(2, dp.EdgesOnDemandPath.Count);

            dp.ChangeEndpoint(node0, node1);
            Assert.AreEqual(1, dp.EdgesOnDemandPath.Count);

            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.ChangeEndpoint(node1, node2));
            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.ChangeEndpoint(node2, node1));
        }

        [TestMethod]
        public void TestOnEdgeContracted()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChild(node2, node3);

            DemandPair dp = new DemandPair(node1, node2);

            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.OnEdgeContracted((node2, node1), node1));
            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.OnEdgeContracted((node1, node2), node1));

            dp = new DemandPair(node0, node3);
            dp.OnEdgeContracted((node2, node1), node1);
            Assert.AreEqual(2, dp.EdgesOnDemandPath.Count);

            dp.OnEdgeContracted((node0, node1), node1);
            Assert.AreEqual(node1, dp.Node1);

            dp = new DemandPair(node0, node3);
            dp.OnEdgeContracted((node3, node2), node2);
            Assert.AreEqual(node2, dp.Node2);

            dp = new DemandPair(node0, node2);
            Assert.ThrowsException<NotOnDemandPathException>(() => dp.OnEdgeContracted((node2, node3), node2));

            dp = new DemandPair(node0, node2);
            Assert.ThrowsException<NotOnDemandPathException>(() => dp.OnEdgeContracted((node3, node2), node3));

            dp = new DemandPair(node0, node1);
            MethodInfo updateEdgesOnPathAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateEdgesOnPathAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node3, node2), node3 }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(NotOnDemandPathException));
        }
    }
}
