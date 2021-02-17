// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestBFS
    {
        [TestMethod]
        public void TestNullParameter()
        {
            Node node = new Node(0);
            HashSet<Node> target = new HashSet<Node>();

            Assert.ThrowsException<ArgumentNullException>(() => BFS.FindShortestPath(null, target));
            Assert.ThrowsException<ArgumentNullException>(() => BFS.FindShortestPath(node, null));
        }

        [TestMethod]
        public void TestOtherExceptions()
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8 });

            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node4);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node2, node6);
            graph.AddEdge(node3, node7);
            graph.AddEdge(node6, node7);

            Assert.ThrowsException<InvalidOperationException>(() => BFS.FindShortestPath(node0, new HashSet<Node>()));
            Assert.ThrowsException<InvalidOperationException>(() => BFS.FindShortestPath(node0, new HashSet<Node>() { node0, node2, node8 }));
            Assert.ThrowsException<NotInGraphException>(() => BFS.FindShortestPath(node0, new HashSet<Node>() { node8 }));
        }

        [TestMethod]
        public void TestShortestPath()
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8 });

            graph.AddEdge(node0, node1);
            graph.AddEdge(node0, node2);
            graph.AddEdge(node0, node3);
            graph.AddEdge(node1, node4);
            graph.AddEdge(node1, node5);
            graph.AddEdge(node2, node5);
            graph.AddEdge(node2, node6);
            graph.AddEdge(node3, node7);
            graph.AddEdge(node6, node7);
            graph.AddEdge(node6, node8);

            List<Node> path = BFS.FindShortestPath(node0, new HashSet<Node>() { node8 });
            List<Node> expectedPath = new List<Node>() { node0, node2, node6, node8 };

            CollectionAssert.AreEqual(expectedPath, path);
        }
    }
}
