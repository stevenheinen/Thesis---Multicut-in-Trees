// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace TESTS_MulticutInTrees.Graphs
{
    [TestClass]
    public class UnitTestGraph
    {
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Graph<Node> graph = new Graph<Node>();

            Assert.IsNotNull(graph);
            Assert.IsNotNull(graph.Edges(counter));
            Assert.IsNotNull(graph.Nodes(counter));
        }

        [TestMethod]
        public void TestConstructorFromOtherGraph()
        {
            Graph<Node> otherGraph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            otherGraph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, counter);
            otherGraph.AddEdge(node0, node1, counter);
            otherGraph.AddEdge(node0, node2, counter);
            otherGraph.AddEdge(node0, node3, counter);
            otherGraph.AddEdge(node1, node2, counter);
            otherGraph.AddEdge(node1, node5, counter);
            otherGraph.AddEdge(node2, node5, counter);
            otherGraph.AddEdge(node3, node4, counter);
            otherGraph.AddEdge(node4, node5, counter);
            otherGraph.AddEdge(node4, node6, counter);
            otherGraph.AddEdge(node5, node7, counter);
            otherGraph.AddEdge(node5, node9, counter);
            otherGraph.AddEdge(node6, node8, counter);
            otherGraph.AddEdge(node7, node8, counter);

            Graph<Node> graph = new Graph<Node>(otherGraph, counter);

            Assert.IsNotNull(graph);
            Assert.AreEqual(otherGraph.NumberOfNodes(counter), graph.NumberOfNodes(counter));
            Assert.AreEqual(otherGraph.NumberOfEdges(counter), graph.NumberOfEdges(counter));
            Assert.IsTrue(graph.HasNode(node3, counter));
            Assert.IsTrue(graph.HasEdge((node8, node6), counter));
        }

        [TestMethod]
        public void TestHasNode()
        {
            Graph<Node> graph = new Graph<Node>();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node6 = new Node(6);
            Node node9 = new Node(9);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node3, node4, counter);

            Assert.IsTrue(graph.HasNode(node2, counter));
            Assert.IsTrue(graph.HasNode(node4, counter));
            Assert.IsFalse(graph.HasNode(node6, counter));
            Assert.IsFalse(graph.HasNode(node9, counter));
        }

        [TestMethod]
        public void TestHasEdge()
        {
            Graph<Node> graph = new Graph<Node>();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node6 = new Node(6);
            Node node9 = new Node(9);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node3, node4, counter);

            Assert.IsTrue(graph.HasEdge(node0, node2, counter));
            Assert.IsTrue(graph.HasEdge(node4, node3, counter));
            Assert.IsFalse(graph.HasEdge(node0, node4, counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(node1, node9, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(node6, node0, counter);
            });
        }

        [TestMethod]
        public void TestHasEdgeTuple()
        {
            Graph<Node> graph = new Graph<Node>();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node6 = new Node(6);
            Node node9 = new Node(9);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node3, node4, counter);

            Assert.IsTrue(graph.HasEdge((node0, node2), counter));
            Assert.IsTrue(graph.HasEdge((node4, node3), counter));
            Assert.IsFalse(graph.HasEdge((node0, node4), counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge((node1, node9), counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge((node6, node0), counter);
            });
        }

        [TestMethod]
        public void TestToString()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node5, counter);
            graph.AddEdge(node4, node6, counter);
            graph.AddEdge(node5, node7, counter);
            graph.AddEdge(node5, node9, counter);
            graph.AddEdge(node6, node8, counter);
            graph.AddEdge(node7, node8, counter);

            Assert.AreEqual("Graph with 10 nodes and 13 edges.", graph.ToString());
        }

        [TestMethod]
        public void TestAddNode()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);

            Assert.IsFalse(graph.HasNode(node0, counter));
            Assert.IsFalse(graph.HasNode(node1, counter));

            graph.AddNode(node0, counter);

            Assert.IsTrue(graph.HasNode(node0, counter));
            Assert.IsFalse(graph.HasNode(node1, counter));

            graph.AddNode(node1, counter);

            Assert.IsTrue(graph.HasNode(node0, counter));
            Assert.IsTrue(graph.HasNode(node1, counter));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddNode(node1, counter);
            });
        }

        [TestMethod]
        public void TestAddNodes()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            Assert.IsFalse(graph.HasNode(node0, counter));
            Assert.IsFalse(graph.HasNode(node1, counter));
            Assert.IsFalse(graph.HasNode(node2, counter));

            graph.AddNodes(new List<Node>() { node0, node1 }, counter);

            Assert.IsTrue(graph.HasNode(node0, counter));
            Assert.IsTrue(graph.HasNode(node1, counter));
            Assert.IsFalse(graph.HasNode(node2, counter));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddNodes(new List<Node>() { node2, node1 }, counter);
            });
        }

        [TestMethod]
        public void TestAddEdge()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, counter);

            Assert.IsFalse(graph.HasEdge(node0, node1, counter));
            
            graph.AddEdge(node0, node1, counter);

            Assert.IsTrue(graph.HasEdge(node1, node0, counter));

            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node5, counter);

            Assert.IsTrue(node2.HasNeighbour(node1, counter));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddEdge(node2, node0, counter);
            });

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddEdge(node1, node5, counter);
            });

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.RemoveEdge(node2, node0, counter);
                graph.AddEdge(node2, node0, counter, true);
                graph.AddEdge(node2, node0, counter, true);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.AddEdge(node6, node3, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.AddEdge(node2, node8, counter);
            });
        }

        [TestMethod]
        public void TestAddEdges()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, counter);

            Assert.IsFalse(graph.HasEdge(node0, node1, counter));

            graph.AddEdges(new List<(Node, Node)>() { (node0, node1) }, counter);

            Assert.IsTrue(graph.HasEdge(node1, node0, counter));

            graph.AddEdges(new List<(Node, Node)>() { (node0, node2), (node0, node3), (node1, node2), (node1, node5), (node2, node5), (node3, node4), (node4, node5) }, counter);

            Assert.IsTrue(node2.HasNeighbour(node1, counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.AddEdges(new List<(Node, Node)>() { (node6, node3) }, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.AddEdges(new List<(Node, Node)>() { (node2, node8) }, counter);
            });
        }

        [TestMethod]
        public void TestRemoveNode()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node5, counter);
            graph.AddEdge(node4, node6, counter);
            graph.AddEdge(node5, node7, counter);
            graph.AddEdge(node5, node9, counter);
            graph.AddEdge(node6, node8, counter);
            graph.AddEdge(node7, node8, counter);

            graph.RemoveNode(node4, counter);
            Assert.AreEqual(0, node4.Degree(counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(node3, node4, counter);
            });
            
            Assert.AreEqual(9, graph.NumberOfNodes(counter));
            Assert.IsFalse(node5.HasNeighbour(node4, counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNode(node4, counter);
            });
        }

        [TestMethod]
        public void TestRemoveNodes()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node5, counter);
            graph.AddEdge(node4, node6, counter);
            graph.AddEdge(node5, node7, counter);
            graph.AddEdge(node5, node9, counter);
            graph.AddEdge(node6, node8, counter);
            graph.AddEdge(node7, node8, counter);

            graph.RemoveNodes(new List<Node>() { node0, node4, node8 }, counter);
            Assert.AreEqual(0, node4.Degree(counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(node3, node4, counter);
            });

            Assert.AreEqual(7, graph.NumberOfNodes(counter));
            Assert.IsFalse(node5.HasNeighbour(node4, counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNodes(new List<Node>() { node2, node8 }, counter);
            });
        }

        [TestMethod]
        public void TestRemoveAllEdgesOfNode()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node5, counter);
            graph.AddEdge(node4, node6, counter);
            graph.AddEdge(node5, node7, counter);
            graph.AddEdge(node5, node9, counter);
            graph.AddEdge(node6, node8, counter);
            graph.AddEdge(node7, node8, counter);

            graph.RemoveAllEdgesOfNode(node4, counter);
            Assert.AreEqual(0, node4.Degree(counter));
            Assert.IsFalse(graph.HasEdge(node3, node4, counter));
            Assert.AreEqual(10, graph.NumberOfNodes(counter));
            Assert.IsFalse(node5.HasNeighbour(node4, counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNode(node2, counter);
                graph.RemoveAllEdgesOfNode(node2, counter);
            });
        }

        [TestMethod]
        public void TestRemoveEdge()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node5, counter);
            graph.AddEdge(node4, node6, counter);
            graph.AddEdge(node5, node7, counter);
            graph.AddEdge(node5, node9, counter);
            graph.AddEdge(node6, node8, counter);
            graph.AddEdge(node7, node8, counter);

            Assert.IsTrue(graph.HasEdge(node5, node7, counter));
            graph.RemoveEdge(node5, node7, counter);
            Assert.IsFalse(graph.HasEdge(node5, node7, counter));
            Assert.IsFalse(node7.HasNeighbour(node5, counter));

            graph.AddEdge(node5, node7, counter, true);
            graph.RemoveEdge(node5, node7, counter, true);
            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node5, node7, counter, true);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node7, node5, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node5, node7, counter);
            });

            graph.RemoveNode(node3, counter);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node3, node4, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node0, node3, counter);
            });
        }

        [TestMethod]
        public void TestRemoveEdges()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node5, counter);
            graph.AddEdge(node4, node6, counter);
            graph.AddEdge(node5, node7, counter);
            graph.AddEdge(node5, node9, counter);
            graph.AddEdge(node6, node8, counter);
            graph.AddEdge(node7, node8, counter);

            Assert.IsTrue(graph.HasEdge(node5, node7, counter));
            graph.RemoveEdges(new List<(Node, Node)>() { (node5, node7), (node0, node1) }, counter);
            Assert.IsFalse(graph.HasEdge(node5, node7, counter));
            Assert.IsFalse(node7.HasNeighbour(node5, counter));
            Assert.IsFalse(node0.HasNeighbour(node1, counter));
            Assert.IsTrue(node0.HasNeighbour(node2, counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<(Node, Node)>() { (node7, node5) }, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<(Node, Node)>() { (node5, node7) }, counter);
            });

            graph.RemoveNode(node3, counter);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<(Node, Node)>() { (node3, node4) }, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<(Node, Node)>() { (node0, node3) }, counter);
            });
        }

        [TestMethod]
        public void TestArgumentNull()
        {
            Graph<Node> g = new Graph<Node>();
            Node n = new Node(0);
            List<(Node, Node)> elist = new List<(Node, Node)>();
            List<Node> nlist = new List<Node>();

            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdge(n, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdge(null, n, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdge(n, n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdges(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdges(elist, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNode(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNodes(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNodes(nlist, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasEdge(n, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasEdge(null, n, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasEdge(n, n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasNode(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveAllEdgesOfNode(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveAllEdgesOfNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdge(null, n, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdge(n, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdge(n, n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdges(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdges(elist, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNode(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNodes(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNodes(nlist, null));
        }

        [TestMethod]
        public void TestAcyclic()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, counter);
            graph.AddEdges(new List<(Node, Node)>()
            {
                (node0, node1),
                (node1, node2),
                (node2, node3),
                (node3, node0)
            }, counter);

            Assert.IsFalse(graph.IsAcyclic(counter));

            graph.RemoveEdge(node0, node3, counter);

            Assert.IsTrue(graph.IsAcyclic(counter));

            graph.RemoveNode(node1, counter);
            graph.RemoveNode(node2, counter);
            graph.RemoveNode(node3, counter);

            Assert.IsTrue(graph.IsAcyclic(counter));
        }

        [TestMethod]
        public void TestConnected()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, counter);
            graph.AddEdges(new List<(Node, Node)>()
            {
                (node0, node1),
                (node1, node2),
                (node2, node3),
            }, counter);

            Assert.IsTrue(graph.IsConnected(counter));

            graph.RemoveNode(node1, counter);

            Assert.IsFalse(graph.IsConnected(counter));
        }
    }
}
