// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestEdmondsMatching
    {
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestArgumentNull()
        {
            Graph<Node> graph = new Graph<Node>();
            List<(Node, Node)> list = new List<(Node, Node)>();
            List<Node> list2 = new List<Node>();
            HashSet<Node> hashset = new HashSet<Node>();
            HashSet<(Node, Node)> hashset2 = new HashSet<(Node, Node)>();
            Dictionary<Node, Node> dictionary = new Dictionary<Node, Node>();
            Dictionary<(Node, Node), Node> dictionary2 = new Dictionary<(Node, Node), Node>();
            Node node = new Node(0);
            Node nullnode = null;
            (Node, Node) edge = (node, node);

            Assert.ThrowsException<ArgumentNullException>(() => EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => EdmondsMatching.HasMatchingOfAtLeast<Graph<Node>, Node>(null, 2));

            MethodInfo method = typeof(EdmondsMatching).GetMethod("RecursiveFindMaximumMatching", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("RecursiveHasMatchingOfAtLeast", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, 2, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, 6, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("AugmentMatchingAlongPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { list, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("FindGreedyMaximalMatching", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("FindAugmentingPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("FindPathPPrime", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list, dictionary, hashset, hashset, dictionary2 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { hashset, null, dictionary, hashset, hashset, dictionary2 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { hashset, list, null, hashset, hashset, dictionary2 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { hashset, list, dictionary, null, hashset, dictionary2 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { hashset, list, dictionary, hashset, null, dictionary2 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { hashset, list, dictionary, hashset, hashset, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("BuildDigraphD", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, hashset2, hashset, dictionary, dictionary2 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { list, null, hashset, dictionary, dictionary2 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            method.Invoke(null, new object[] { list, hashset2, null, dictionary, dictionary2 });
            method.Invoke(null, new object[] { list, hashset2, hashset, null, dictionary2 });
            method.Invoke(null, new object[] { list, hashset2, hashset, dictionary, null });

            method = typeof(EdmondsMatching).GetMethod("CreateArcForD", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { (nullnode, node), edge, hashset, list, dictionary2, dictionary, dictionary }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { (node, nullnode), edge, hashset, list, dictionary2, dictionary, dictionary }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { edge, (nullnode, node), hashset, list, dictionary2, dictionary, dictionary }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { edge, (node, nullnode), hashset, list, dictionary2, dictionary, dictionary }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { edge, edge, null, list, dictionary2, dictionary, dictionary }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { edge, edge, hashset, null, dictionary2, dictionary, dictionary }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { edge, edge, hashset, list, null, dictionary, dictionary }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { edge, edge, hashset, list, dictionary2, null, dictionary }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { edge, edge, hashset, list, dictionary2, dictionary, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("FindAndContractBlossom", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, list, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("ExpandPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list, list2, node, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, null, list2, node, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, list, null, node, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, list, list2, null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, list, list2, node, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomOnEnd", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list2, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { list, null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { list, list2, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomInMiddle", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list, list2, node, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, null, list2, node, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, list, null, node, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, list, list2, null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, list, list2, node, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("IsAugmentingPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { list, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestCase1()
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

            List<Node> nodes = new List<Node>()
            {
                node0,
                node1,
                node2,
                node3,
                node4,
                node5,
                node6,
                node7,
                node8
            };

            List<(Node, Node)> edges = new List<(Node, Node)>()
            {
                (node0, node1),
                (node0, node3),
                (node1, node2),
                (node1, node3),
                (node1, node4),
                (node2, node5),
                (node3, node4),
                (node3, node6),
                (node3, node7),
                (node4, node5),
                (node4, node7),
                (node4, node8),
                (node6, node7),
                (node7, node8)
            };

            graph.AddNodes(nodes);
            graph.AddEdges(edges);

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);

            Assert.AreEqual(4, matching.Count);
        }

        [TestMethod]
        public void TestCase2()
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
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);

            List<Node> nodes = new List<Node>()
            {
                node0,
                node1,
                node2,
                node3,
                node4,
                node5,
                node6,
                node7,
                node8,
                node9,
                node10,
                node11,
                node12,
                node13,
                node14,
                node15
            };

            List<(Node, Node)> edges = new List<(Node, Node)>()
            {
                (node0, node1),
                (node0, node13),
                (node1, node4),
                (node1, node15),
                (node2, node13),
                (node2, node3),
                (node2, node6),
                (node3, node4),
                (node3, node5),
                (node4, node5),
                (node6, node7),
                (node7, node8),
                (node7, node14),
                (node8, node11),
                (node8, node12),
                (node9, node14),
                (node9, node10),
                (node10, node12),
                (node11, node12)
            };

            graph.AddNodes(nodes);
            graph.AddEdges(edges);

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);

            Assert.AreEqual(8, matching.Count);
        }

        [TestMethod]
        public void TestCase3()
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
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);

            List<Node> nodes = new List<Node>()
            {
                node0,
                node1,
                node2,
                node3,
                node4,
                node5,
                node6,
                node7,
                node8,
                node9,
                node10,
                node11,
                node12,
                node13,
                node14,
                node15,
                node16,
                node17
            };

            List<(Node, Node)> edges = new List<(Node, Node)>()
            {
                (node0, node1),
                (node0, node7),
                (node1, node4),
                (node2, node3),
                (node2, node5),
                (node2, node6),
                (node3, node6),
                (node4, node7),
                (node4, node8),
                (node5, node6),
                (node5, node12),
                (node5, node13),
                (node6, node13),
                (node7, node8),
                (node7, node10),
                (node8, node9),
                (node8, node10),
                (node9, node12),
                (node9, node13),
                (node10, node11),
                (node11, node14),
                (node11, node15),
                (node12, node13),
                (node12, node16),
                (node12, node17),
                (node13, node16),
                (node13, node17),
                (node14, node15),
                (node15, node16),
                (node16, node17)
            };

            graph.AddNodes(nodes);
            graph.AddEdges(edges);

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);

            Assert.AreEqual(9, matching.Count);
        }

        [TestMethod]
        public void TestCase4()
        {
            Random random = new Random(685450);
            Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(500, 0.3, random);

            int startEdges = graph.NumberOfEdges;
            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    graph.AddEdge(components[i][0], components[j][0]);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new HashSet<(Node, Node)>(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new HashSet<Node>();

            foreach ((Node, Node) edge in matching)
            {
                Assert.IsTrue(graph.HasEdge(edge));

                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            Assert.AreEqual(startEdges, graph.NumberOfEdges);
            Assert.AreEqual(graph.Nodes.Count, matchedNodes.Count);
        }

        [TestMethod]
        public void TestCase5()
        {
            Random random = new Random(9874);
            Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(500, 0.8, random);

            int startEdges = graph.NumberOfEdges;

            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    graph.AddEdge(components[i][0], components[j][0]);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new HashSet<(Node, Node)>(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new HashSet<Node>();
            foreach ((Node, Node) edge in matching)
            {
                Assert.IsTrue(graph.HasEdge(edge));

                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            Assert.AreEqual(startEdges, graph.NumberOfEdges);
            Assert.AreEqual(graph.Nodes.Count, matchedNodes.Count);
        }

        [TestMethod]
        public void TestCase6()
        {
            Random random = new Random(3);
            Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(501, 0.05, random);

            int startEdges = graph.NumberOfEdges;

            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    graph.AddEdge(components[i][0], components[j][0]);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new HashSet<(Node, Node)>(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new HashSet<Node>();
            foreach ((Node, Node) edge in matching)
            {
                Assert.IsTrue(graph.HasEdge(edge));

                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            Assert.AreEqual(startEdges, graph.NumberOfEdges);
            Assert.AreEqual(graph.Nodes.Count - 1, matchedNodes.Count);
        }

        [TestMethod]
        public void TestStarGraph()
        {
            int nrNodes = 100;
            Graph<Node> graph = new Graph<Node>();

            for (uint i = 0; i < nrNodes; i++)
            {
                graph.AddNode(new Node(i));
            }

            for (int i = 1; i < nrNodes; i++)
            {
                graph.AddEdge(graph.Nodes[0], graph.Nodes[i]);
            }

            int startEdges = graph.NumberOfEdges;

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);
            Assert.AreEqual(1, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new HashSet<(Node, Node)>(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new HashSet<Node>();
            foreach ((Node, Node) edge in matching)
            {
                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            Assert.AreEqual(startEdges, graph.NumberOfEdges);
            Assert.AreEqual(2, matchedNodes.Count);
        }

        [TestMethod]
        public void TestMatchingOfSizeTrue()
        {
            Random random = new Random(243);
            Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(500, 0.2, random);
            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    graph.AddEdge(components[i][0], components[j][0]);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            Assert.IsTrue(EdmondsMatching.HasMatchingOfAtLeast<Graph<Node>, Node>(graph, 150));
        }

        [TestMethod]
        public void TestMatchingOfSizeFalse()
        {
            Random random = new Random(9846);
            Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(500, 0.1, random);

            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    graph.AddEdge(components[i][0], components[j][0]);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes, counter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            Assert.IsFalse(EdmondsMatching.HasMatchingOfAtLeast<Graph<Node>, Node>(graph, 300));
        }

        [TestMethod]
        public void TestBlossomOnPath1()
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
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node2, node3);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node4, node9);
            graph.AddEdge(node5, node6);
            graph.AddEdge(node6, node7);
            graph.AddEdge(node7, node8);
            graph.AddEdge(node8, node12);
            graph.AddEdge(node8, node13);
            graph.AddEdge(node9, node10);
            graph.AddEdge(node10, node11);
            graph.AddEdge(node11, node12);
            graph.AddEdge(node13, node14);
            graph.AddEdge(node15, node14);

            List<(Node, Node)> contractedPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node13), (node13, node14), (node14, node15) };
            List<Node> blossom = new List<Node>() { node4, node5, node6, node7, node8, node9, node10, node11, node12 };
            List<(Node, Node)> matching = new List<(Node, Node)>() { (node1, node2), (node3, node4), (node5, node6), (node7, node8), (node9, node10), (node11, node12), (node13, node14) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomInMiddle", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { graph, contractedPath, blossom, node4, matching });

            List<(Node, Node)> expectedPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7), (node7, node8), (node8, node13), (node13, node14), (node14, node15) };
            Assert.AreEqual(11, augmentingPath.Count);
            CollectionAssert.AreEqual(expectedPath, augmentingPath);
        }

        [TestMethod]
        public void TestBlossomOnPath2()
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
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node2, node3);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node4, node8);
            graph.AddEdge(node5, node6);
            graph.AddEdge(node6, node7);
            graph.AddEdge(node7, node12);
            graph.AddEdge(node7, node13);
            graph.AddEdge(node8, node9);
            graph.AddEdge(node9, node10);
            graph.AddEdge(node10, node11);
            graph.AddEdge(node11, node12);
            graph.AddEdge(node13, node14);
            graph.AddEdge(node14, node15);

            List<(Node, Node)> contractedPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node13), (node13, node14), (node14, node15) };
            List<Node> blossom = new List<Node>() { node4, node5, node6, node7, node8, node9, node10, node11, node12 };
            List<(Node, Node)> matching = new List<(Node, Node)>() { (node1, node2), (node3, node4), (node5, node6), (node7, node12), (node8, node9), (node10, node11), (node13, node14) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomInMiddle", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { graph, contractedPath, blossom, node4, matching });

            List<(Node, Node)> expectedPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node8), (node8, node9), (node9, node10), (node10, node11), (node11, node12), (node12, node7), (node7, node13), (node13, node14), (node14, node15) };
            Assert.AreEqual(13, augmentingPath.Count);
            CollectionAssert.AreEqual(expectedPath, augmentingPath);
        }

        [TestMethod]
        public void TestBlossomAtEndOfPath1()
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
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node2, node3);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node5, node6);
            graph.AddEdge(node5, node13);
            graph.AddEdge(node6, node7);
            graph.AddEdge(node7, node8);
            graph.AddEdge(node8, node9);
            graph.AddEdge(node9, node10);
            graph.AddEdge(node10, node11);
            graph.AddEdge(node11, node12);
            graph.AddEdge(node12, node13);

            List<(Node, Node)> contractedPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5) };
            List<Node> blossom = new List<Node>() { node5, node6, node7, node8, node9, node10, node11, node12, node13 };
            List<(Node, Node)> matching = new List<(Node, Node)>() { (node1, node2), (node3, node4), (node5, node6), (node7, node8), (node9, node10), (node11, node12) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomOnEnd", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { contractedPath, blossom, matching });

            List<(Node, Node)> expectedPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7), (node7, node8), (node8, node9), (node9, node10), (node10, node11), (node11, node12), (node12, node13) };
            Assert.AreEqual(13, augmentingPath.Count);
            CollectionAssert.AreEqual(expectedPath, augmentingPath);
        }

        [TestMethod]
        public void TestBlossomAtEndOfPath2()
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
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13 });
            graph.AddEdge(node0, node1);
            graph.AddEdge(node1, node2);
            graph.AddEdge(node2, node3);
            graph.AddEdge(node3, node4);
            graph.AddEdge(node4, node5);
            graph.AddEdge(node5, node6);
            graph.AddEdge(node5, node13);
            graph.AddEdge(node6, node7);
            graph.AddEdge(node7, node8);
            graph.AddEdge(node8, node9);
            graph.AddEdge(node9, node10);
            graph.AddEdge(node10, node11);
            graph.AddEdge(node11, node12);
            graph.AddEdge(node12, node13);

            List<(Node, Node)> contractedPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5) };
            List<Node> blossom = new List<Node>() { node5, node6, node7, node8, node9, node10, node11, node12, node13 };
            List<(Node, Node)> matching = new List<(Node, Node)>() { (node1, node2), (node3, node4), (node5, node6), (node8, node9), (node10, node11), (node12, node13) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomOnEnd", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { contractedPath, blossom, matching });

            List<(Node, Node)> expectedPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7) };
            Assert.AreEqual(7, augmentingPath.Count);
            CollectionAssert.AreEqual(expectedPath, augmentingPath);
        }

        [TestMethod]
        public void UnitTestMoreExceptions()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);

            List<(Node, Node)> notAugmentingPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6) };
            List<(Node, Node)> emptyMatching = new List<(Node, Node)>();

            MethodInfo method = typeof(EdmondsMatching).GetMethod("AugmentMatchingAlongPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { emptyMatching, notAugmentingPath}));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));

            List<(Node, Node)> augmentingPath = new List<(Node, Node)>() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7) };
            List<(Node, Node)> tooEmptyMatching = new List<(Node, Node)>() { (node1, node2), (node5, node6) };

            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { tooEmptyMatching, augmentingPath }));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));

            List<(Node, Node)> wrongMatching = new List<(Node, Node)>() { (node1, node2), (node3, node4), (node4, node5), (node5, node6) };

            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { wrongMatching, augmentingPath }));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));
        }
    }
}
