// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace TESTS_MulticutInTrees.Graphs
{
    [TestClass]
    public class UnitTestGraph
    {
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Graph graph = new Graph();

            Assert.IsNotNull(graph);
            Assert.IsNotNull(graph.Edges(MockCounter));
            Assert.IsNotNull(graph.Nodes(MockCounter));
        }

        [TestMethod]
        public void TestHasNode()
        {
            Graph graph = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node6 = new Node(6);
            Node node9 = new Node(9);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge12, edge34 }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(graph.HasNode(node2, MockCounter));
            Assert.IsTrue(graph.HasNode(node4, MockCounter));
            Assert.IsFalse(graph.HasNode(node6, MockCounter));
            Assert.IsFalse(graph.HasNode(node9, MockCounter));
        }

        [TestMethod]
        public void TestHasEdge()
        {
            Graph graph = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node6 = new Node(6);
            Node node9 = new Node(9);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge12, edge34 }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(graph.HasEdge(edge02, MockCounter));
            Assert.IsTrue(graph.HasEdge(edge34, MockCounter));

            Edge<Node> edge04 = new Edge<Node>(node0, node4);
            Assert.IsFalse(graph.HasEdge(edge04, MockCounter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                Edge<Node> edge19 = new Edge<Node>(node1, node9);
                graph.HasEdge(edge19, MockCounter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                Edge<Node> edge60 = new Edge<Node>(node6, node0);
                graph.HasEdge(edge60, MockCounter);
            });
        }

        [TestMethod]
        public void TestToString()
        {
            Graph graph = new Graph();

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            Edge<Node> edge57 = new Edge<Node>(node5, node7);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge12, edge15, edge25, edge34, edge45, edge46, edge57, edge59, edge68, edge78 }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.AreEqual("Graph with 10 nodes and 13 edges", graph.ToString());
        }

        [TestMethod]
        public void TestAddNode()
        {
            Graph graph = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);

            Assert.IsFalse(graph.HasNode(node0, MockCounter));
            Assert.IsFalse(graph.HasNode(node1, MockCounter));

            graph.AddNode(node0, MockCounter);

            Assert.IsTrue(graph.HasNode(node0, MockCounter));
            Assert.IsFalse(graph.HasNode(node1, MockCounter));

            graph.AddNode(node1, MockCounter);

            Assert.IsTrue(graph.HasNode(node0, MockCounter));
            Assert.IsTrue(graph.HasNode(node1, MockCounter));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddNode(node1, MockCounter);
            });
        }

        [TestMethod]
        public void TestAddNodes()
        {
            Graph graph = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            Assert.IsFalse(graph.HasNode(node0, MockCounter));
            Assert.IsFalse(graph.HasNode(node1, MockCounter));
            Assert.IsFalse(graph.HasNode(node2, MockCounter));

            graph.AddNodes(new List<Node>() { node0, node1 }, MockCounter);

            Assert.IsTrue(graph.HasNode(node0, MockCounter));
            Assert.IsTrue(graph.HasNode(node1, MockCounter));
            Assert.IsFalse(graph.HasNode(node2, MockCounter));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddNodes(new List<Node>() { node2, node1 }, MockCounter);
            });
        }

        [TestMethod]
        public void TestAddEdge()
        {
            Graph graph = new Graph();

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Assert.IsFalse(graph.HasEdge(edge01, MockCounter));

            graph.AddEdge(edge01, MockCounter);

            Assert.IsTrue(graph.HasEdge(edge01, MockCounter));

            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            graph.AddEdges(new List<Edge<Node>>() { edge02, edge03, edge12, edge15, edge25, edge34, edge45 }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(node2.HasNeighbour(node1, MockCounter));

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddEdge(edge02, MockCounter);
            });

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.AddEdge(edge15, MockCounter);
            });

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                graph.RemoveEdge(edge02, MockCounter);
                graph.AddEdge(edge02, MockCounter);
                graph.AddEdge(edge02, MockCounter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                Edge<Node> edge36 = new Edge<Node>(node3, node6);
                graph.AddEdge(edge36, MockCounter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                Edge<Node> edge82 = new Edge<Node>(node8, node2);
                graph.AddEdge(edge82, MockCounter);
            });
        }

        [TestMethod]
        public void TestAddEdges()
        {
            Graph graph = new Graph();

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Assert.IsFalse(graph.HasEdge(edge01, MockCounter));

            graph.AddEdges(new List<Edge<Node>>() { edge01 }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(graph.HasEdge(edge01, MockCounter));

            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);

            graph.AddEdges(new List<Edge<Node>>() { edge02, edge03, edge12, edge15, edge25, edge34, edge45 }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(node2.HasNeighbour(node1, MockCounter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                Edge<Node> edge63 = new Edge<Node>(node6, node3);
                graph.AddEdges(new List<Edge<Node>>() { edge63 }, MockCounter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                Edge<Node> edge28 = new Edge<Node>(node2, node8);
                graph.AddEdges(new List<Edge<Node>>() { edge28 }, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveNode()
        {
            Graph graph = new Graph();

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            Edge<Node> edge57 = new Edge<Node>(node5, node7);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge12, edge15, edge25, edge34, edge45, edge46, edge57, edge59, edge68, edge78 }, MockCounter);
            graph.UpdateNodeTypes();

            graph.RemoveNode(node4, MockCounter);
            Assert.AreEqual(0, node4.Degree(MockCounter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(edge34, MockCounter);
            });

            Assert.AreEqual(9, graph.NumberOfNodes(MockCounter));
            Assert.IsFalse(node5.HasNeighbour(node4, MockCounter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNode(node4, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveNodes()
        {
            Graph graph = new Graph();

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            Edge<Node> edge57 = new Edge<Node>(node5, node7);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge12, edge15, edge25, edge34, edge45, edge46, edge57, edge59, edge68, edge78 }, MockCounter);
            graph.UpdateNodeTypes();

            graph.RemoveNodes(new List<Node>() { node0, node4, node8 }, MockCounter);
            Assert.AreEqual(0, node4.Degree(MockCounter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.HasEdge(edge34, MockCounter);
            });

            Assert.AreEqual(7, graph.NumberOfNodes(MockCounter));
            Assert.IsFalse(node5.HasNeighbour(node4, MockCounter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNodes(new List<Node>() { node2, node8 }, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveAllEdgesOfNode()
        {
            Graph graph = new Graph();

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            Edge<Node> edge57 = new Edge<Node>(node5, node7);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge12, edge15, edge25, edge34, edge45, edge46, edge57, edge59, edge68, edge78 }, MockCounter);
            graph.UpdateNodeTypes();

            graph.RemoveAllEdgesOfNode(node4, MockCounter);
            Assert.AreEqual(0, node4.Degree(MockCounter));
            Assert.IsFalse(graph.HasEdge(edge34, MockCounter));
            Assert.AreEqual(10, graph.NumberOfNodes(MockCounter));
            Assert.IsFalse(node5.HasNeighbour(node4, MockCounter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveNode(node2, MockCounter);
                graph.RemoveAllEdgesOfNode(node2, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveEdge()
        {
            Graph graph = new Graph();

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            Edge<Node> edge57 = new Edge<Node>(node5, node7);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge12, edge15, edge25, edge34, edge45, edge46, edge57, edge59, edge68, edge78 }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(graph.HasEdge(edge57, MockCounter));
            graph.RemoveEdge(edge57, MockCounter);
            Assert.IsFalse(graph.HasEdge(edge57, MockCounter));
            Assert.IsFalse(node7.HasNeighbour(node5, MockCounter));

            graph.AddEdge(edge57, MockCounter);
            graph.RemoveEdge(edge57, MockCounter);
            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(edge57, MockCounter);
            });

            graph.RemoveNode(node3, MockCounter);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(edge34, MockCounter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdge(edge03, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveEdges()
        {
            Graph graph = new Graph();

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            Edge<Node> edge57 = new Edge<Node>(node5, node7);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge12, edge15, edge25, edge34, edge45, edge46, edge57, edge59, edge68, edge78 }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(graph.HasEdge(edge57, MockCounter));
            graph.RemoveEdges(new List<Edge<Node>>() { edge57, edge01 }, MockCounter);
            Assert.IsFalse(graph.HasEdge(edge57, MockCounter));
            Assert.IsFalse(node7.HasNeighbour(node5, MockCounter));
            Assert.IsFalse(node0.HasNeighbour(node1, MockCounter));
            Assert.IsTrue(node0.HasNeighbour(node2, MockCounter));

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<Edge<Node>>() { edge57 }, MockCounter);
            });

            graph.RemoveNode(node3, MockCounter);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<Edge<Node>>() { edge34 }, MockCounter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                graph.RemoveEdges(new List<Edge<Node>>() { edge03 }, MockCounter);
            });
        }

        [TestMethod]
        public void TestArgumentNull()
        {
            Graph g = new Graph();
            Node n = new Node(0);
            List<Edge<Node>> elist = new List<Edge<Node>>();
            List<Node> nlist = new List<Node>();
            Node n1 = new Node(1);
            Node n2 = new Node(2);
            Edge<Node> edge = new Edge<Node>(n1, n2);

            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdge(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdge(edge, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdges(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddEdges(elist, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNode(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNodes(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.AddNodes(nlist, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasEdge(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasEdge(edge, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasNode(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.HasNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveAllEdgesOfNode(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveAllEdgesOfNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdge(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdge(edge, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdges(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveEdges(elist, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNode(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNodes(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => g.RemoveNodes(nlist, null));
        }

        [TestMethod]
        public void TestAcyclic()
        {
            Graph graph = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge30 = new Edge<Node>(node3, node0);

            graph.AddEdges(new List<Edge<Node>>()
            {
                edge01,
                edge12,
                edge23,
                edge30
            }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsFalse(graph.IsAcyclic(MockCounter));

            graph.RemoveEdge(edge30, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(graph.IsAcyclic(MockCounter));

            graph.RemoveNode(node1, MockCounter);
            graph.RemoveNode(node2, MockCounter);
            graph.RemoveNode(node3, MockCounter);

            Assert.IsTrue(graph.IsAcyclic(MockCounter));
        }

        [TestMethod]
        public void TestConnected()
        {
            Graph graph = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);

            graph.AddEdges(new List<Edge<Node>>()
            {
                edge01,
                edge12,
                edge23
            }, MockCounter);
            graph.UpdateNodeTypes();

            Assert.IsTrue(graph.IsConnected(MockCounter));

            graph.RemoveNode(node1, MockCounter);

            Assert.IsFalse(graph.IsConnected(MockCounter));
        }
    }
}
