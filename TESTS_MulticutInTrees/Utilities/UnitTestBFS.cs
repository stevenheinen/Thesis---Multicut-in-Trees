// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestBFS
    {
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestNullParameter()
        {
            Node node = new Node(0);
            HashSet<Node> target = new HashSet<Node>();

            Assert.ThrowsException<ArgumentNullException>(() => BFS.FindShortestPath(null, target, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => BFS.FindShortestPath(node, null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => BFS.FindShortestPath(node, target, null));
        }

        [TestMethod]
        public void TestOtherExceptions()
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge26 = new Edge<Node>(node2, node6);
            Edge<Node> edge37 = new Edge<Node>(node3, node7);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge15, edge25, edge26, edge37, edge67 }, MockCounter);
            Assert.ThrowsException<InvalidOperationException>(() => BFS.FindShortestPath(node0, new HashSet<Node>(), MockCounter));
            Assert.ThrowsException<InvalidOperationException>(() => BFS.FindShortestPath(node0, new HashSet<Node>() { node0, node2, node8 }, MockCounter));
            Assert.ThrowsException<NotInGraphException>(() => BFS.FindShortestPath(node0, new HashSet<Node>() { node8 }, MockCounter));
        }

        [TestMethod]
        public void TestShortestPath()
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge26 = new Edge<Node>(node2, node6);
            Edge<Node> edge37 = new Edge<Node>(node3, node7);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge15, edge25, edge26, edge37, edge67, edge68 }, MockCounter);

            List<Node> path = BFS.FindShortestPath(node0, new HashSet<Node>() { node8 }, MockCounter);
            List<Node> expectedPath = new List<Node>() { node0, node2, node6, node8 };

            CollectionAssert.AreEqual(expectedPath, path);
        }
    }
}
