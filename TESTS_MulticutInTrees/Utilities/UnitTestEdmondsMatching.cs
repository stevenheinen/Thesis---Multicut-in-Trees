// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestEdmondsMatching
    {
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestArgumentNull()
        {
            Graph graph = new();
            List<(Node, Node)> list = new();
            List<Node> list2 = new();
            HashSet<Node> hashset = new();
            HashSet<(Node, Node)> hashset2 = new();
            Dictionary<Node, Node> dictionary = new();
            Dictionary<(Node, Node), Node> dictionary2 = new();
            Node node = new(0);
            Node nullnode = null;
            (Node, Node) edge = (node, node);

            Assert.ThrowsException<ArgumentNullException>(() => EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => EdmondsMatching.HasMatchingOfAtLeast<Graph, Edge<Node>, Node>(null, 2));

            MethodInfo method = typeof(EdmondsMatching).GetMethod("RecursiveFindMaximumMatching", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("RecursiveHasMatchingOfAtLeast", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
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
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("FindAugmentingPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
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
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { null, list, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, null, list }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { graph, list, null }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            method = typeof(EdmondsMatching).GetMethod("ExpandPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
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
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
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
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);

            List<Node> nodes = new()
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

            List<Edge<Node>> edges = new()
            {
                new Edge<Node>(node0, node1),
                new Edge<Node>(node0, node3),
                new Edge<Node>(node1, node2),
                new Edge<Node>(node1, node3),
                new Edge<Node>(node1, node4),
                new Edge<Node>(node2, node5),
                new Edge<Node>(node3, node4),
                new Edge<Node>(node3, node6),
                new Edge<Node>(node3, node7),
                new Edge<Node>(node4, node5),
                new Edge<Node>(node4, node7),
                new Edge<Node>(node4, node8),
                new Edge<Node>(node6, node7),
                new Edge<Node>(node7, node8)
            };

            graph.AddNodes(nodes, MockCounter);
            graph.AddEdges(edges, MockCounter);

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph);

            Assert.AreEqual(4, matching.Count);
        }

        [TestMethod]
        public void TestCase2()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);

            List<Node> nodes = new()
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

            List<Edge<Node>> edges = new()
            {
                new Edge<Node>(node0, node1),
                new Edge<Node>(node0, node13),
                new Edge<Node>(node1, node4),
                new Edge<Node>(node1, node15),
                new Edge<Node>(node2, node13),
                new Edge<Node>(node2, node3),
                new Edge<Node>(node2, node6),
                new Edge<Node>(node3, node4),
                new Edge<Node>(node3, node5),
                new Edge<Node>(node4, node5),
                new Edge<Node>(node6, node7),
                new Edge<Node>(node7, node8),
                new Edge<Node>(node7, node14),
                new Edge<Node>(node8, node11),
                new Edge<Node>(node8, node12),
                new Edge<Node>(node9, node14),
                new Edge<Node>(node9, node10),
                new Edge<Node>(node10, node12),
                new Edge<Node>(node11, node12)
            };

            graph.AddNodes(nodes, MockCounter);
            graph.AddEdges(edges, MockCounter);

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph);

            Assert.AreEqual(8, matching.Count);
        }

        [TestMethod]
        public void TestCase3()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            Node node16 = new(16);
            Node node17 = new(17);

            List<Node> nodes = new()
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

            List<Edge<Node>> edges = new()
            {
                new Edge<Node>(node0, node1),
                new Edge<Node>(node0, node7),
                new Edge<Node>(node1, node4),
                new Edge<Node>(node2, node3),
                new Edge<Node>(node2, node5),
                new Edge<Node>(node2, node6),
                new Edge<Node>(node3, node6),
                new Edge<Node>(node4, node7),
                new Edge<Node>(node4, node8),
                new Edge<Node>(node5, node6),
                new Edge<Node>(node5, node12),
                new Edge<Node>(node5, node13),
                new Edge<Node>(node6, node13),
                new Edge<Node>(node7, node8),
                new Edge<Node>(node7, node10),
                new Edge<Node>(node8, node9),
                new Edge<Node>(node8, node10),
                new Edge<Node>(node9, node12),
                new Edge<Node>(node9, node13),
                new Edge<Node>(node10, node11),
                new Edge<Node>(node11, node14),
                new Edge<Node>(node11, node15),
                new Edge<Node>(node12, node13),
                new Edge<Node>(node12, node16),
                new Edge<Node>(node12, node17),
                new Edge<Node>(node13, node16),
                new Edge<Node>(node13, node17),
                new Edge<Node>(node14, node15),
                new Edge<Node>(node15, node16),
                new Edge<Node>(node16, node17)
            };

            graph.AddNodes(nodes, MockCounter);
            graph.AddEdges(edges, MockCounter);

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph);

            Assert.AreEqual(9, matching.Count);
        }

        [TestMethod]
        public void TestCase4()
        {
            Random random = new(685450);
            Graph graph = ErdosRenyiGraph.CreateErdosRenyiGraph(500, 0.3, random);

            int startEdges = graph.NumberOfEdges(MockCounter);
            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);
            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    Edge<Node> edge = new(components[i][0], components[j][0]);
                    graph.AddEdge(edge, MockCounter);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(Utils.OrderEdgeSmallToLarge))
            {
                Assert.IsTrue(graph.HasEdge(nodeTupleToEdge[edge], MockCounter));

                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            Assert.AreEqual(startEdges, graph.NumberOfEdges(MockCounter));
            Assert.AreEqual(graph.NumberOfNodes(MockCounter), matchedNodes.Count);
        }

        [TestMethod]
        public void TestCase5()
        {
            Random random = new(9874);
            Graph graph = ErdosRenyiGraph.CreateErdosRenyiGraph(500, 0.8, random);

            int startEdges = graph.NumberOfEdges(MockCounter);

            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);
            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    Edge<Node> edge = new(components[i][0], components[j][0]);
                    graph.AddEdge(edge, MockCounter);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(Utils.OrderEdgeSmallToLarge))
            {
                Assert.IsTrue(graph.HasEdge(nodeTupleToEdge[edge], MockCounter));

                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            Assert.AreEqual(startEdges, graph.NumberOfEdges(MockCounter));
            Assert.AreEqual(graph.NumberOfNodes(MockCounter), matchedNodes.Count);
        }

        [TestMethod]
        public void TestCase6()
        {
            Random random = new(3);
            Graph graph = ErdosRenyiGraph.CreateErdosRenyiGraph(501, 0.05, random);

            int startEdges = graph.NumberOfEdges(MockCounter);

            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);
            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    Edge<Node> edge = new(components[i][0], components[j][0]);
                    graph.AddEdge(edge, MockCounter);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(Utils.OrderEdgeSmallToLarge))
            {
                Assert.IsTrue(graph.HasEdge(nodeTupleToEdge[edge], MockCounter));

                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            Assert.AreEqual(startEdges, graph.NumberOfEdges(MockCounter));
            Assert.AreEqual(graph.NumberOfNodes(MockCounter) - 1, matchedNodes.Count);
        }

        [TestMethod]
        public void TestStarGraph()
        {
            int nrNodes = 100;
            Graph graph = new();

            for (uint i = 0; i < nrNodes; i++)
            {
                graph.AddNode(new Node(i), MockCounter);
            }

            bool first = true;
            foreach (Node node in graph.Nodes(MockCounter))
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                graph.AddEdge(new Edge<Node>(graph.Nodes(MockCounter).First(), node), MockCounter);
            }

            int startEdges = graph.NumberOfEdges(MockCounter);

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph);
            Assert.AreEqual(1, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();
            foreach ((Node, Node) edge in matching)
            {
                matchedNodes.Add(edge.Item1);
                matchedNodes.Add(edge.Item2);
            }

            Assert.AreEqual(startEdges, graph.NumberOfEdges(MockCounter));
            Assert.AreEqual(2, matchedNodes.Count);
        }

        [TestMethod]
        public void TestMatchingOfSizeTrue()
        {
            Random random = new(243);
            Graph graph = ErdosRenyiGraph.CreateErdosRenyiGraph(500, 0.2, random);
            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);

            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    graph.AddEdge(new Edge<Node>(components[i][0], components[j][0]), MockCounter);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            Assert.IsTrue(EdmondsMatching.HasMatchingOfAtLeast<Graph, Edge<Node>, Node>(graph, 150));
        }

        [TestMethod]
        public void TestMatchingOfSizeFalse()
        {
            Random random = new(9846);
            Graph graph = ErdosRenyiGraph.CreateErdosRenyiGraph(500, 0.1, random);

            List<List<Node>> components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);

            for (int i = 0; i < components.Count - 1; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    graph.AddEdge(new Edge<Node>(components[i][0], components[j][0]), MockCounter);
                }
            }

            components = DFS.FindAllConnectedComponents(graph.Nodes(MockCounter), MockCounter);

            if (components.Count != 1)
            {
                throw new Exception();
            }

            Assert.IsFalse(EdmondsMatching.HasMatchingOfAtLeast<Graph, Edge<Node>, Node>(graph, 300));
        }

        [TestMethod]
        public void TestBlossomOnPath1()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge49 = new(node4, node9);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge812 = new(node8, node12);
            Edge<Node> edge813 = new(node8, node13);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge1011 = new(node10, node11);
            Edge<Node> edge1112 = new(node11, node12);
            Edge<Node> edge1314 = new(node13, node14);
            Edge<Node> edge1514 = new(node15, node14);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge45, edge49, edge56, edge67, edge78, edge812, edge813, edge910, edge1011, edge1112, edge1314, edge1514 }, MockCounter);

            List<(Node, Node)> contractedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node13), (node13, node14), (node14, node15) };
            List<Node> blossom = new() { node4, node5, node6, node7, node8, node9, node10, node11, node12 };
            List<(Node, Node)> matching = new() { (node1, node2), (node3, node4), (node5, node6), (node7, node8), (node9, node10), (node11, node12), (node13, node14) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomInMiddle", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { graph, contractedPath, blossom, node4, matching });

            List<(Node, Node)> expectedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7), (node7, node8), (node8, node13), (node13, node14), (node14, node15) };
            Assert.AreEqual(11, augmentingPath.Count);
            CollectionAssert.AreEqual(expectedPath, augmentingPath);
        }

        [TestMethod]
        public void TestBlossomOnPath2()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge48 = new(node4, node8);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge712 = new(node7, node12);
            Edge<Node> edge713 = new(node7, node13);
            Edge<Node> edge89 = new(node8, node9);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge1011 = new(node10, node11);
            Edge<Node> edge1112 = new(node11, node12);
            Edge<Node> edge1314 = new(node13, node14);
            Edge<Node> edge1415 = new(node14, node15);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge45, edge48, edge56, edge67, edge712, edge713, edge89, edge910, edge1011, edge1112, edge1314, edge1415 }, MockCounter);

            List<(Node, Node)> contractedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node13), (node13, node14), (node14, node15) };
            List<Node> blossom = new() { node4, node5, node6, node7, node8, node9, node10, node11, node12 };
            List<(Node, Node)> matching = new() { (node1, node2), (node3, node4), (node5, node6), (node7, node12), (node8, node9), (node10, node11), (node13, node14) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomInMiddle", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { graph, contractedPath, blossom, node4, matching });

            List<(Node, Node)> expectedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node8), (node8, node9), (node9, node10), (node10, node11), (node11, node12), (node12, node7), (node7, node13), (node13, node14), (node14, node15) };
            Assert.AreEqual(13, augmentingPath.Count);
            CollectionAssert.AreEqual(expectedPath, augmentingPath);
        }

        [TestMethod]
        public void TestBlossomAtEndOfPath1()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge513 = new(node5, node13);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge89 = new(node8, node9);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge1011 = new(node10, node11);
            Edge<Node> edge1112 = new(node11, node12);
            Edge<Node> edge1213 = new(node12, node13);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge45, edge56, edge513, edge67, edge78, edge89, edge910, edge1011, edge1112, edge1213 }, MockCounter);

            List<(Node, Node)> contractedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5) };
            List<Node> blossom = new() { node5, node6, node7, node8, node9, node10, node11, node12, node13 };
            List<(Node, Node)> matching = new() { (node1, node2), (node3, node4), (node5, node6), (node7, node8), (node9, node10), (node11, node12) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomOnEnd", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { contractedPath, blossom, matching });

            List<(Node, Node)> expectedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7), (node7, node8), (node8, node9), (node9, node10), (node10, node11), (node11, node12), (node12, node13) };
            Assert.AreEqual(13, augmentingPath.Count);
            CollectionAssert.AreEqual(expectedPath, augmentingPath);
        }

        [TestMethod]
        public void TestBlossomAtEndOfPath2()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge513 = new(node5, node13);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge89 = new(node8, node9);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge1011 = new(node10, node11);
            Edge<Node> edge1112 = new(node11, node12);
            Edge<Node> edge1213 = new(node12, node13);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge45, edge56, edge513, edge67, edge78, edge89, edge910, edge1011, edge1112, edge1213 }, MockCounter);

            List<(Node, Node)> contractedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5) };
            List<Node> blossom = new() { node5, node6, node7, node8, node9, node10, node11, node12, node13 };
            List<(Node, Node)> matching = new() { (node1, node2), (node3, node4), (node5, node6), (node8, node9), (node10, node11), (node12, node13) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomOnEnd", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { contractedPath, blossom, matching });

            List<(Node, Node)> expectedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7) };
            Assert.AreEqual(7, augmentingPath.Count);
            CollectionAssert.AreEqual(expectedPath, augmentingPath);
        }

        [TestMethod]
        public void UnitTestMoreExceptions()
        {
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);

            List<(Node, Node)> notAugmentingPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6) };
            List<(Node, Node)> emptyMatching = new();

            MethodInfo method = typeof(EdmondsMatching).GetMethod("AugmentMatchingAlongPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { emptyMatching, notAugmentingPath }));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));

            List<(Node, Node)> augmentingPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7) };
            List<(Node, Node)> tooEmptyMatching = new() { (node1, node2), (node5, node6) };

            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { tooEmptyMatching, augmentingPath }));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));

            List<(Node, Node)> wrongMatching = new() { (node1, node2), (node3, node4), (node4, node5), (node5, node6) };

            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { wrongMatching, augmentingPath }));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));
        }
    }
}
