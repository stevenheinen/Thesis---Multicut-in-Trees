// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.MulticutProblem
{
    [TestClass]
    public class UnitTestDemandPair
    {
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);

            tree.AddNodes(new List<Node>() { node0, node1 }, MockCounter);
            tree.AddEdge(new Edge<Node>(node0, node1), MockCounter);

            DemandPair dp = new DemandPair(0, node0, node1, tree);
            Assert.IsNotNull(dp);

            Assert.AreEqual(node0, dp.Node1);
            Assert.AreEqual(node1, dp.Node2);
            Assert.AreEqual(1, dp.LengthOfPath(MockCounter));
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);

            tree.AddNodes(new List<Node>() { node0, node1 }, MockCounter);
            tree.AddEdge(edge01, MockCounter);

            Assert.ThrowsException<ArgumentNullException>(() => { DemandPair _dp = new DemandPair(1, node0, null, tree); });
            Assert.ThrowsException<ArgumentNullException>(() => { DemandPair _dp = new DemandPair(2, null, node0, tree); });
            Assert.ThrowsException<ArgumentNullException>(() => { DemandPair _dp = new DemandPair(3, node0, node1, null); });

            DemandPair dp = new DemandPair(0, node0, node1, tree);

            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted(null, node0, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted(edge01, null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.OnEdgeContracted(edge01, node2, null));
            Assert.ThrowsException<ArgumentNullException>(() => dp.ChangeEndpoint(node0, null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.ChangeEndpoint(null, node0, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => dp.ChangeEndpoint(node1, node0, null));

            Assert.ThrowsException<ArgumentNullException>(() => { dp.UpdateEndpointsAfterEdgeContraction(null, node1); });
            Assert.ThrowsException<ArgumentNullException>(() => { dp.UpdateEndpointsAfterEdgeContraction(edge01, null); });

            MethodInfo updateEdgesOnPathAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateEdgesOnPathAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { null, MockCounter }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { edge01, null }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestOnEndpointChanged()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23 }, MockCounter);

            DemandPair dp = new DemandPair(0, node0, node3, tree);

            Assert.ThrowsException<NotOnDemandPathException>(() => dp.ChangeEndpoint(node2, node1, MockCounter));

            dp.ChangeEndpoint(node3, node2, MockCounter);
            Assert.AreEqual(2, dp.LengthOfPath(MockCounter));

            dp.ChangeEndpoint(node0, node1, MockCounter);
            Assert.AreEqual(1, dp.LengthOfPath(MockCounter));

            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.ChangeEndpoint(node1, node2, MockCounter));
            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.ChangeEndpoint(node2, node1, MockCounter));
        }

        [TestMethod]
        public void TestOnEdgeContracted()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23 }, MockCounter);

            DemandPair dp = new DemandPair(0, node1, node2, tree);

            Assert.ThrowsException<ZeroLengthDemandPathException>(() => dp.OnEdgeContracted(edge12, node1, MockCounter));

            dp = new DemandPair(1, node0, node3, tree);
            dp.OnEdgeContracted(edge12, node1, MockCounter);
            Assert.AreEqual(2, dp.LengthOfPath(MockCounter));

            dp.OnEdgeContracted(edge01, node1, MockCounter);
            Assert.AreEqual(node1, dp.Node1);

            dp = new DemandPair(2, node0, node3, tree);
            dp.OnEdgeContracted(edge23, node2, MockCounter);
            Assert.AreEqual(node2, dp.Node2);

            dp = new DemandPair(3, node0, node2, tree);
            Assert.ThrowsException<NotOnDemandPathException>(() => dp.OnEdgeContracted(edge23, node2, MockCounter));

            dp = new DemandPair(4, node0, node1, tree);
            MethodInfo updateEdgesOnPathAfterEdgeContraction = typeof(DemandPair).GetMethod("UpdateEdgesOnPathAfterEdgeContraction", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => { updateEdgesOnPathAfterEdgeContraction.Invoke(dp, new object[] { edge23, MockCounter }); });
            Assert.IsInstanceOfType(t.InnerException, typeof(NotOnDemandPathException));
        }
    }
}
