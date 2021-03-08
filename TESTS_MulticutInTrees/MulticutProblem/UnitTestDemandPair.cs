// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestDemandPair
    {
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);

            DemandPair dp = new DemandPair(node0, node1);
            Assert.IsNotNull(dp);

            Assert.AreEqual(node0, dp.Node1);
            Assert.AreEqual(node1, dp.Node2);
            Assert.AreEqual(1, dp.LengthOfPath(counter));
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);

            Assert.ThrowsException<ArgumentNullException>(() => { DemandPair _dp = new DemandPair(node0, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { DemandPair _dp = new DemandPair(null, node0); });

            DemandPair dp = new DemandPair(node0, node1);

            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted((null, node0), node0, counter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted((node0, null), node0, counter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted((node1, node0), null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted((node0, node1), node2, null));
            Assert.ThrowsException<ArgumentNullException>(() => dp.ChangeEndpoint(node0, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.ChangeEndpoint(null, node0, counter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.ChangeEndpoint(node1, node0, null));

            MethodInfo updateEndpointsAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateEndpointsAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => { updateEndpointsAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(null, node0), node1 }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEndpointsAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node0, null), node1 }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEndpointsAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node1, node0), null }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            MethodInfo updateEdgesOnPathAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateEdgesOnPathAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(null, node0), node1, counter }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node0, null), node1, counter }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node1, node0), null, counter }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node1, node0), node1, null }); });
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChild(node2, node3, counter);

            DemandPair dp = new DemandPair(node0, node3);

            Assert.ThrowsException<NotOnDemandPathException>(() => dp.ChangeEndpoint(node2, node1, counter));

            dp.ChangeEndpoint(node3, node2, counter);
            Assert.AreEqual(2, dp.LengthOfPath(counter));

            dp.ChangeEndpoint(node0, node1, counter);
            Assert.AreEqual(1, dp.LengthOfPath(counter));

            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.ChangeEndpoint(node1, node2, counter));
            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.ChangeEndpoint(node2, node1, counter));
        }

        [TestMethod]
        public void TestOnEdgeContracted()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChild(node2, node3, counter);

            DemandPair dp = new DemandPair(node1, node2);

            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.OnEdgeContracted((node2, node1), node1, counter));
            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.OnEdgeContracted((node1, node2), node1, counter));

            dp = new DemandPair(node0, node3);
            dp.OnEdgeContracted((node2, node1), node1, counter);
            Assert.AreEqual(2, dp.LengthOfPath(counter));

            dp.OnEdgeContracted((node0, node1), node1, counter);
            Assert.AreEqual(node1, dp.Node1);

            dp = new DemandPair(node0, node3);
            dp.OnEdgeContracted((node3, node2), node2, counter);
            Assert.AreEqual(node2, dp.Node2);

            dp = new DemandPair(node0, node2);
            Assert.ThrowsException<NotOnDemandPathException>(() => dp.OnEdgeContracted((node2, node3), node2, counter));

            dp = new DemandPair(node0, node2);
            Assert.ThrowsException<NotOnDemandPathException>(() => dp.OnEdgeContracted((node3, node2), node3, counter));

            dp = new DemandPair(node0, node1);
            MethodInfo updateEdgesOnPathAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateEdgesOnPathAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { new ValueTuple<TreeNode, TreeNode>(node3, node2), node3, counter }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(NotOnDemandPathException));
        }
    }
}
