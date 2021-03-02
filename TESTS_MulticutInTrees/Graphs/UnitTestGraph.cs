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
            Assert.IsNotNull(graph.Edges);
            Assert.IsNotNull(graph.Nodes);
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

            otherGraph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 });
            otherGraph.AddEdge(node0, node1);
            otherGraph.AddEdge(node0, node2);
            otherGraph.AddEdge(node0, node3);
            otherGraph.AddEdge(node1, node2);
            otherGraph.AddEdge(node1, node5);
            otherGraph.AddEdge(node2, node5);
            otherGraph.AddEdge(node3, node4);
            otherGraph.AddEdge(node4, node5);
            otherGraph.AddEdge(node4, node6);
            otherGraph.AddEdge(node5, node7);
            otherGraph.AddEdge(node5, node9);
            otherGraph.AddEdge(node6, node8);
            otherGraph.AddEdge(node7, node8);

            Graph<Node> graph = new Graph<Node>(otherGraph);

            Assert.IsNotNull(graph);
            Assert.AreEqual(otherGraph.NumberOfNodes, graph.NumberOfNodes);
            Assert.AreEqual(otherGraph.NumberOfEdges, graph.NumberOfEdges);
            Assert.IsTrue(graph.HasNode(node3));
            Assert.IsTrue(graph.HasEdge((node8, node6)));
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node3, node4);

            Assert.IsTrue(graph.HasNode(node2));
            Assert.IsTrue(graph.HasNode(node4));
            Assert.IsFalse(graph.HasNode(node6));
            Assert.IsFalse(graph.HasNode(node9));
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node3, node4);

            Assert.IsTrue(graph.HasEdge(node0, node2));
            Assert.IsTrue(graph.HasEdge(node4, node3));
            Assert.IsFalse(graph.HasEdge(node0, node4));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(node1, node9);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(node6, node0);
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node3, node4);

            Assert.IsTrue(graph.HasEdge((node0, node2)));
            Assert.IsTrue(graph.HasEdge((node4, node3)));
            Assert.IsFalse(graph.HasEdge((node0, node4)));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge((node1, node9));
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge((node6, node0));
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node4, node6);
            graph.AddEdge(node5, node7);
            graph.AddEdge(node5, node9);
            graph.AddEdge(node6, node8);
            graph.AddEdge(node7, node8);

            Assert.AreEqual("Graph with 10 nodes and 13 edges.", graph.ToString());
        }

        [TestMethod]
        public void TestAddNode()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);

            Assert.IsFalse(graph.HasNode(node0));
            Assert.IsFalse(graph.HasNode(node1));

            graph.AddNode(node0);

            Assert.IsTrue(graph.HasNode(node0));
            Assert.IsFalse(graph.HasNode(node1));

            graph.AddNode(node1);

            Assert.IsTrue(graph.HasNode(node0));
            Assert.IsTrue(graph.HasNode(node1));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddNode(node1);
            });
        }

        [TestMethod]
        public void TestAddNodes()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            Assert.IsFalse(graph.HasNode(node0));
            Assert.IsFalse(graph.HasNode(node1));
            Assert.IsFalse(graph.HasNode(node2));

            graph.AddNodes(new List<Node>() { node0, node1 });

            Assert.IsTrue(graph.HasNode(node0));
            Assert.IsTrue(graph.HasNode(node1));
            Assert.IsFalse(graph.HasNode(node2));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddNodes(new List<Node>() { node2, node1 });
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 });

            Assert.IsFalse(graph.HasEdge(node0, node1));
            
            graph.AddEdge(node0, node1);

            Assert.IsTrue(graph.HasEdge(node1, node0));

            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);

            Assert.IsTrue(node2.HasNeighbour(node1));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddEdge(node2, node0);
            });

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddEdge(node1, node5);
            });

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.RemoveEdge(node2, node0);
                graph.AddEdge(node2, node0, true);
                graph.AddEdge(node2, node0, true);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.AddEdge(node6, node3);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.AddEdge(node2, node8);
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 });

            Assert.IsFalse(graph.HasEdge(node0, node1));

            graph.AddEdges(new List<(Node, Node)>() { (node0, node1) });

            Assert.IsTrue(graph.HasEdge(node1, node0));

            graph.AddEdges(new List<(Node, Node)>() { (node0, node2), (node0, node3), (node1, node2), (node1, node5), (node2, node5), (node3, node4), (node4, node5) });

            Assert.IsTrue(node2.HasNeighbour(node1));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.AddEdges(new List<(Node, Node)>() { (node6, node3) });
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.AddEdges(new List<(Node, Node)>() { (node2, node8) });
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node4, node6);
            graph.AddEdge(node5, node7);
            graph.AddEdge(node5, node9);
            graph.AddEdge(node6, node8);
            graph.AddEdge(node7, node8);

            graph.RemoveNode(node4);
            Assert.AreEqual(0, node4.Degree(counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(node3, node4);
            });
            
            Assert.AreEqual(9, graph.NumberOfNodes);
            Assert.IsFalse(node5.HasNeighbour(node4));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNode(node4);
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node4, node6);
            graph.AddEdge(node5, node7);
            graph.AddEdge(node5, node9);
            graph.AddEdge(node6, node8);
            graph.AddEdge(node7, node8);

            graph.RemoveNodes(new List<Node>() { node0, node4, node8 });
            Assert.AreEqual(0, node4.Degree(counter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(node3, node4);
            });

            Assert.AreEqual(7, graph.NumberOfNodes);
            Assert.IsFalse(node5.HasNeighbour(node4));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNodes(new List<Node>() { node2, node8 });
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node4, node6);
            graph.AddEdge(node5, node7);
            graph.AddEdge(node5, node9);
            graph.AddEdge(node6, node8);
            graph.AddEdge(node7, node8);

            graph.RemoveAllEdgesOfNode(node4);
            Assert.AreEqual(0, node4.Degree(counter));
            Assert.IsFalse(graph.HasEdge(node3, node4));
            Assert.AreEqual(10, graph.NumberOfNodes);
            Assert.IsFalse(node5.HasNeighbour(node4));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNode(node2);
                graph.RemoveAllEdgesOfNode(node2);
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node4, node6);
            graph.AddEdge(node5, node7);
            graph.AddEdge(node5, node9);
            graph.AddEdge(node6, node8);
            graph.AddEdge(node7, node8);

            Assert.IsTrue(graph.HasEdge(node5, node7));
            graph.RemoveEdge(node5, node7);
            Assert.IsFalse(graph.HasEdge(node5, node7));
            Assert.IsFalse(node7.HasNeighbour(node5));

            graph.AddEdge(node5, node7, true);
            graph.RemoveEdge(node5, node7, true);
            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node5, node7, true);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node7, node5);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node5, node7);
            });

            graph.RemoveNode(node3);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node3, node4);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(node0, node3);
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node4, node6);
            graph.AddEdge(node5, node7);
            graph.AddEdge(node5, node9);
            graph.AddEdge(node6, node8);
            graph.AddEdge(node7, node8);

            Assert.IsTrue(graph.HasEdge(node5, node7));
            graph.RemoveEdges(new List<(Node, Node)>() { (node5, node7), (node0, node1) });
            Assert.IsFalse(graph.HasEdge(node5, node7));
            Assert.IsFalse(node7.HasNeighbour(node5));
            Assert.IsFalse(node0.HasNeighbour(node1));
            Assert.IsTrue(node0.HasNeighbour(node2));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<(Node, Node)>() { (node7, node5) });
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<(Node, Node)>() { (node5, node7) });
            });

            graph.RemoveNode(node3);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<(Node, Node)>() { (node3, node4) });
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<(Node, Node)>() { (node0, node3) });
            });
        }

        [TestMethod]
        public void TestArgumentNull()
        {
            Graph<Node> g = new Graph<Node>();
            Node n = new Node(0);

            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdge(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdge(null, n));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdges(null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNode(null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNodes(null));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasEdge(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasEdge(null, n));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasNode(null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveAllEdgesOfNode(null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdge(null, n));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdge(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdges(null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNode(null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNodes(null));
        }

        [TestMethod]
        public void TestAcyclic()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 });
            graph.AddEdges(new List<(Node, Node)>()
            {
                (node0, node1),
                (node1, node2),
                (node2, node3),
                (node3, node0)
            });

            Assert.IsFalse(graph.IsAcyclic(counter));

            graph.RemoveEdge(node0, node3);

            Assert.IsTrue(graph.IsAcyclic(counter));

            graph.RemoveNode(node1);
            graph.RemoveNode(node2);
            graph.RemoveNode(node3);

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
            
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 });
            graph.AddEdges(new List<(Node, Node)>()
            {
                (node0, node1),
                (node1, node2),
                (node2, node3),
            });

            Assert.IsTrue(graph.IsConnected(counter));

            graph.RemoveNode(node1);

            Assert.IsFalse(graph.IsConnected(counter));
        }
    }
}
