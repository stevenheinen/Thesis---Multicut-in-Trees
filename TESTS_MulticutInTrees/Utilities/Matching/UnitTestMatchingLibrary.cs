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
using MulticutInTrees.Utilities.Matching;

namespace TESTS_MulticutInTrees.Utilities.Matching
{
    [TestClass]
    public class UnitTestMatchingLibrary
    {
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestNullParameter()
        {
            Graph g = new();
            Assert.ThrowsException<ArgumentNullException>(() => MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(g, null));
            Assert.ThrowsException<ArgumentNullException>(() => MatchingLibrary.HasMatchingOfSize<Graph, Edge<Node>, Node>(null, 10, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => MatchingLibrary.HasMatchingOfSize<Graph, Edge<Node>, Node>(g, 10, null));
        }

        [TestMethod]
        public void TestEmptyGraphs()
        {
            Graph g = new();

            Assert.AreEqual(0, MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(g, MockCounter).Count());

            g.AddNode(new Node(0), MockCounter);

            Assert.AreEqual(0, MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(g, MockCounter).Count());
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

            List<Edge<Node>> matching = MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(graph, MockCounter).ToList();

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

            List<Edge<Node>> matching = MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(graph, MockCounter).ToList();

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

            List<Edge<Node>> matching = MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(graph, MockCounter).ToList();

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

            List<Edge<Node>> matching = MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(graph, MockCounter).ToList();
            Assert.AreEqual(250, matching.Count);

            HashSet<Edge<Node>> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(e => Utils.OrderEdgeSmallToLarge((e.Endpoint1, e.Endpoint2))))
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

            List<Edge<Node>> matching = MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(graph, MockCounter).ToList();
            Assert.AreEqual(250, matching.Count);

            HashSet<Edge<Node>> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(e => Utils.OrderEdgeSmallToLarge((e.Endpoint1, e.Endpoint2))))
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

            List<Edge<Node>> matching = MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(graph, MockCounter).ToList();
            Assert.AreEqual(250, matching.Count);

            HashSet<Edge<Node>> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();

            Dictionary<(Node, Node), Edge<Node>> nodeTupleToEdge = (Dictionary<(Node, Node), Edge<Node>>)typeof(Graph).GetProperty("NodeTupleToEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(graph, Array.Empty<object>());
            foreach ((Node, Node) edge in matching.Select(e => Utils.OrderEdgeSmallToLarge((e.Endpoint1, e.Endpoint2))))
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

            List<Edge<Node>> matching = MatchingLibrary.FindMatching<Graph, Edge<Node>, Node>(graph, MockCounter).ToList();
            Assert.AreEqual(1, matching.Count);

            HashSet<Edge<Node>> hashedMatching = new(matching);
            Assert.AreEqual(matching.Count, hashedMatching.Count);

            HashSet<Node> matchedNodes = new();
            foreach (Edge<Node> edge in matching)
            {
                matchedNodes.Add(edge.Endpoint1);
                matchedNodes.Add(edge.Endpoint2);
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

            Assert.IsTrue(MatchingLibrary.HasMatchingOfSize<Graph, Edge<Node>, Node>(graph, 150, MockCounter));
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

            Assert.IsFalse(MatchingLibrary.HasMatchingOfSize<Graph, Edge<Node>, Node>(graph, 300, MockCounter));
        }
    }
 }