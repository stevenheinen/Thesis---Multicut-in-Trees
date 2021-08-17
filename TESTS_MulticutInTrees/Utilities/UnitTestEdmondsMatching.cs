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
            Graph g = new();
            Assert.ThrowsException<ArgumentNullException>(() => EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => EdmondsMatching.HasMatchingOfSize<Graph, Edge<Node>, Node>(null, 2, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(g, null));
            Assert.ThrowsException<ArgumentNullException>(() => EdmondsMatching.HasMatchingOfSize<Graph, Edge<Node>, Node>(g, 2, null));
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
            List<Edge<Node>> edges = new() { edge01, edge12, edge23, edge34, edge45, edge49, edge56, edge67, edge78, edge812, edge813, edge910, edge1011, edge1112, edge1314, edge1514 };
            graph.AddEdges( edges, MockCounter);

            List<(Node, Node)> contractedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node13), (node13, node14), (node14, node15) };
            List<Node> blossom = new() { node4, node5, node6, node7, node8, node9, node10, node11, node12 };
            HashSet<(Node, Node)> matching = new() { (node1, node2), (node3, node4), (node5, node6), (node7, node8), (node9, node10), (node11, node12), (node13, node14) };

            List<(Node, Node)> unmatchedEdges = new(edges.Select(e => Utils.OrderEdgeSmallToLarge<Edge<Node>, Node>(e)).Where(e => !matching.Contains(e)));

            MethodInfo dGraphMethod = typeof(EdmondsMatching).GetMethod("BuildDigraphD", BindingFlags.NonPublic | BindingFlags.Static);
            dGraphMethod = dGraphMethod.MakeGenericMethod(new Type[] { typeof(Node) });
            object[] args = new object[] { unmatchedEdges, matching, new HashSet<Node>(), new Dictionary<Node, Node>(), new Dictionary<Node, Node>(), new Dictionary<(Node, Node), Node>(), MockCounter };
            Graph d = (Graph)dGraphMethod.Invoke(null, args);

            // If the blossom is at the start of the path, reverse the path.
            for (int i = 0; i < contractedPath.Count; i += 2)
            {
                if (contractedPath[i].Item1.Equals(node4))
                {
                    contractedPath = contractedPath.Select(n => (n.Item2, n.Item1)).ToList();
                    contractedPath.Reverse();
                    break;
                }
            }

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomInMiddle", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[] { typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { contractedPath, matching, node4, args[3], args[4], args[5], MockCounter });

            List<(Node, Node)> expectedPath = new() { (node15, node14), (node14, node13), (node13, node8), (node8, node7), (node7, node6), (node6, node5), (node5, node4), (node4, node3), (node3, node2), (node2, node1), (node1, node0) };
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
            List<Edge<Node>> edges = new() { edge01, edge12, edge23, edge34, edge45, edge48, edge56, edge67, edge712, edge713, edge89, edge910, edge1011, edge1112, edge1314, edge1415 };
            graph.AddEdges(edges, MockCounter);

            List<(Node, Node)> contractedPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node13), (node13, node14), (node14, node15) };
            List<Node> blossom = new() { node4, node5, node6, node7, node8, node9, node10, node11, node12 };
            HashSet<(Node, Node)> matching = new() { (node1, node2), (node3, node4), (node5, node6), (node7, node12), (node8, node9), (node10, node11), (node13, node14) };
            
            List<(Node, Node)> unmatchedEdges = new(edges.Select(e => Utils.OrderEdgeSmallToLarge<Edge<Node>, Node>(e)).Where(e => !matching.Contains(e)));

            MethodInfo dGraphMethod = typeof(EdmondsMatching).GetMethod("BuildDigraphD", BindingFlags.NonPublic | BindingFlags.Static);
            dGraphMethod = dGraphMethod.MakeGenericMethod(new Type[] { typeof(Node) });
            object[] args = new object[] { unmatchedEdges, matching, new HashSet<Node>(), new Dictionary<Node, Node>(), new Dictionary<Node, Node>(), new Dictionary<(Node, Node), Node>(), MockCounter };
            Graph d = (Graph)dGraphMethod.Invoke(null, args);

            // If the blossom is at the start of the path, reverse the path.
            for (int i = 0; i < contractedPath.Count; i += 2)
            {
                if (contractedPath[i].Item1.Equals(node4))
                {
                    contractedPath = contractedPath.Select(n => (n.Item2, n.Item1)).ToList();
                    contractedPath.Reverse();
                    break;
                }
            }

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomInMiddle", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[] { typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { contractedPath, matching, node4, args[3], args[4], args[5], MockCounter });

            List<(Node, Node)> expectedPath = new() { (node15, node14), (node14, node13), (node13, node7), (node7, node12), (node12, node11), (node11, node10), (node10, node9), (node9, node8), (node8, node4), (node4, node3), (node3, node2), (node2, node1), (node1, node0) };
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
            HashSet<Node> blossom = new() { node5, node6, node7, node8, node9, node10, node11, node12, node13 };
            HashSet<(Node, Node)> edgesInBlossom = new() { (node5, node6), (node6, node7), (node7, node8), (node8, node9), (node9, node10), (node10, node11), (node11, node12), (node12, node13), (node5, node13) };
            HashSet<(Node, Node)> matching = new() { (node1, node2), (node3, node4), (node5, node6), (node7, node8), (node9, node10), (node11, node12) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomOnEnd", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { contractedPath, blossom, edgesInBlossom, matching, MockCounter });

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
            HashSet<Node> blossom = new() { node5, node6, node7, node8, node9, node10, node11, node12, node13 };
            HashSet<(Node, Node)> edgesInBlossom = new() { (node5, node6), (node6, node7), (node7, node8), (node8, node9), (node9, node10), (node10, node11), (node11, node12), (node12, node13), (node5, node13) };
            HashSet<(Node, Node)> matching = new() { (node1, node2), (node3, node4), (node5, node6), (node8, node9), (node10, node11), (node12, node13) };

            MethodInfo method = typeof(EdmondsMatching).GetMethod("ExpandPathBlossomOnEnd", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });
            List<(Node, Node)> augmentingPath = (List<(Node, Node)>)method.Invoke(null, new object[] { contractedPath, blossom, edgesInBlossom, matching, MockCounter });

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
            HashSet<(Node, Node)> emptyMatching = new();

            MethodInfo method = typeof(EdmondsMatching).GetMethod("AugmentMatchingAlongPath", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(new Type[1] { typeof(Node) });

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { emptyMatching, notAugmentingPath }));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));

            List<(Node, Node)> augmentingPath = new() { (node0, node1), (node1, node2), (node2, node3), (node3, node4), (node4, node5), (node5, node6), (node6, node7) };
            HashSet<(Node, Node)> tooEmptyMatching = new() { (node1, node2), (node5, node6) };

            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { tooEmptyMatching, augmentingPath }));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));

            HashSet<(Node, Node)> wrongMatching = new() { (node1, node2), (node3, node4), (node4, node5), (node5, node6) };

            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, new object[] { wrongMatching, augmentingPath }));
            Assert.IsInstanceOfType(t.InnerException, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void TestEmptyGraphs()
        {
            Graph g = new();

            Assert.AreEqual(0, EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(g, MockCounter).Count);

            g.AddNode(new Node(0), MockCounter);

            Assert.AreEqual(0, EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(g, MockCounter).Count);
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

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph, MockCounter);

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

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph, MockCounter);

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

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph, MockCounter);

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

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph, MockCounter);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(e => (e.Item1, e.Item2)))
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

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph, MockCounter);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(e => (e.Item1, e.Item2)))
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

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph, MockCounter);
            Assert.AreEqual(250, matching.Count);

            HashSet<(Node, Node)> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(e => (e.Item1, e.Item2)))
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

            List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph, Edge<Node>, Node>(graph, MockCounter);
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

            Assert.IsTrue(EdmondsMatching.HasMatchingOfSize<Graph, Edge<Node>, Node>(graph, 150, MockCounter));
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

            Assert.IsFalse(EdmondsMatching.HasMatchingOfSize<Graph, Edge<Node>, Node>(graph, 300, MockCounter));
        }
    }
}
