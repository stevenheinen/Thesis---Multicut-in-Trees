// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestDinicMaxFlow
    {
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestArgumentNull()
        {
            Graph g = new();
            Node n = new(0);
            Dictionary<(uint, uint), int> c = new();
            Dictionary<uint, int> l = new();

            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities<Graph, Edge<Node>, Node>(g, new List<Node>() { n }, null));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities<Graph, Edge<Node>, Node>(g, null, new List<Node>() { n }));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities<Graph, Edge<Node>, Node>(null, new List<Node>() { n }, new List<Node>() { n }));

            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(g, new List<Node>() { n }, null, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(g, null, new List<Node>() { n }, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(null, new List<Node>() { n }, new List<Node>() { n }, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(g, new List<Node>() { n }, new List<Node>() { n }, null));

            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowUnitCapacities<Graph, Edge<Node>, Node>(g, n, null));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowUnitCapacities<Graph, Edge<Node>, Node>(g, null, n));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowUnitCapacities<Graph, Edge<Node>, Node>(null, n, n));

            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(g, n, n, null));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(g, n, null, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(g, null, n, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(null, n, n, c));

            MethodInfo methodFindLevels = typeof(DinicMaxFlow).GetMethod("FindLevels", BindingFlags.NonPublic | BindingFlags.Static);
            methodFindLevels = methodFindLevels.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });

            TargetInvocationException t1 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodFindLevels.Invoke(null, new object[4] { g, n, c, null });
            });
            Assert.IsInstanceOfType(t1.InnerException, typeof(ArgumentNullException));

            TargetInvocationException t2 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodFindLevels.Invoke(null, new object[4] { g, n, null, c });
            });
            Assert.IsInstanceOfType(t2.InnerException, typeof(ArgumentNullException));

            TargetInvocationException t3 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodFindLevels.Invoke(null, new object[4] { g, null, c, c });
            });
            Assert.IsInstanceOfType(t3.InnerException, typeof(ArgumentNullException));

            TargetInvocationException t4 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodFindLevels.Invoke(null, new object[4] { null, n, c, c });
            });
            Assert.IsInstanceOfType(t4.InnerException, typeof(ArgumentNullException));


            MethodInfo methodSendFlow = typeof(DinicMaxFlow).GetMethod("SendFlow", BindingFlags.NonPublic | BindingFlags.Static);
            methodSendFlow = methodSendFlow.MakeGenericMethod(new Type[] { typeof(Graph), typeof(Edge<Node>), typeof(Node) });

            TargetInvocationException t5 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodSendFlow.Invoke(null, new object[7] { g, n, n, 0, c, c, null });
            });
            Assert.IsInstanceOfType(t5.InnerException, typeof(ArgumentNullException));

            TargetInvocationException t6 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodSendFlow.Invoke(null, new object[7] { g, n, n, 0, c, null, l });
            });
            Assert.IsInstanceOfType(t6.InnerException, typeof(ArgumentNullException));

            TargetInvocationException t7 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodSendFlow.Invoke(null, new object[7] { g, n, n, 0, null, c, l });
            });
            Assert.IsInstanceOfType(t7.InnerException, typeof(ArgumentNullException));

            TargetInvocationException t8 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodSendFlow.Invoke(null, new object[7] { g, n, null, 0, c, c, l });
            });
            Assert.IsInstanceOfType(t8.InnerException, typeof(ArgumentNullException));

            TargetInvocationException t9 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodSendFlow.Invoke(null, new object[7] { g, null, n, 0, c, c, l });
            });
            Assert.IsInstanceOfType(t9.InnerException, typeof(ArgumentNullException));

            TargetInvocationException t10 = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                methodSendFlow.Invoke(null, new object[7] { null, n, n, 0, c, c, l });
            });
            Assert.IsInstanceOfType(t10.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestFlowSingleSourceSingleSinkCapacities()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node1, true), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node2, true), MockCounter);
            graph.AddEdge(new Edge<Node>(node2, node1, true), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node3, true), MockCounter);
            graph.AddEdge(new Edge<Node>(node2, node4, true), MockCounter);
            graph.AddEdge(new Edge<Node>(node4, node3, true), MockCounter);
            graph.AddEdge(new Edge<Node>(node4, node5, true), MockCounter);
            graph.AddEdge(new Edge<Node>(node3, node5, true), MockCounter);

            Dictionary<(uint, uint), int> capacities = new()
            {
                { (0, 1), 11 },
                { (0, 2), 12 },
                { (2, 1), 1 },
                { (1, 3), 12 },
                { (2, 4), 11 },
                { (4, 3), 7 },
                { (4, 5), 4 },
                { (3, 5), 19 },
                { (1, 0), 11 },
                { (2, 0), 12 },
                { (1, 2), 1 },
                { (3, 1), 12 },
                { (4, 2), 11 },
                { (3, 4), 7 },
                { (5, 4), 4 },
                { (5, 3), 19 }
            };

            int flow = DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(graph, node0, node5, capacities);
            Assert.AreEqual(23, flow);
        }

        [TestMethod]
        public void TestFlowSingleSourceSingleSinkUnitCapacities()
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            graph.AddEdge(new Edge<Node>(node0, node1), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node3), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node6), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node5), MockCounter);
            graph.AddEdge(new Edge<Node>(node2, node5), MockCounter);
            graph.AddEdge(new Edge<Node>(node3, node4), MockCounter);
            graph.AddEdge(new Edge<Node>(node4, node8), MockCounter);
            graph.AddEdge(new Edge<Node>(node5, node6), MockCounter);
            graph.AddEdge(new Edge<Node>(node6, node7), MockCounter);
            graph.AddEdge(new Edge<Node>(node6, node8), MockCounter);
            graph.AddEdge(new Edge<Node>(node6, node9), MockCounter);
            graph.AddEdge(new Edge<Node>(node9, node10), MockCounter);
            graph.AddEdge(new Edge<Node>(node8, node10), MockCounter);
            graph.AddEdge(new Edge<Node>(node7, node10), MockCounter);

            int flow = DinicMaxFlow.MaxFlowUnitCapacities<Graph, Edge<Node>, Node>(graph, node0, node10);
            Assert.AreEqual(3, flow);
        }

        [TestMethod]
        public void TestFlowSingleSourceSingleSinkUnitCapacities2()
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            graph.AddEdge(new Edge<Node>(node0, node1), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node3), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node6), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node5), MockCounter);
            graph.AddEdge(new Edge<Node>(node2, node5), MockCounter);
            graph.AddEdge(new Edge<Node>(node3, node4), MockCounter);
            graph.AddEdge(new Edge<Node>(node4, node8), MockCounter);
            graph.AddEdge(new Edge<Node>(node5, node6), MockCounter);
            graph.AddEdge(new Edge<Node>(node6, node9), MockCounter);
            graph.AddEdge(new Edge<Node>(node9, node10), MockCounter);
            graph.AddEdge(new Edge<Node>(node8, node10), MockCounter);
            graph.AddEdge(new Edge<Node>(node7, node10), MockCounter);

            int flow = DinicMaxFlow.MaxFlowUnitCapacities<Graph, Edge<Node>, Node>(graph, node0, node10);
            Assert.AreEqual(2, flow);
        }

        [TestMethod]
        public void TestFlowMultipleSourceMultipleSinkUnitCapacities()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node3), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node1), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node2), MockCounter);

            List<Node> sources = new() { node0, node1 };
            List<Node> sinks = new() { node2, node3 };

            int flow = DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities<Graph, Edge<Node>, Node>(graph, sources, sinks);
            Assert.AreEqual(2, flow);
        }

        [TestMethod]
        public void TestFlowMultipleSourceMultipleSink()
        {
            Random random = new(34698);
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node3), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node3), MockCounter);

            Dictionary<(uint, uint), int> capacities = new()
            {
                { (0, 2), 1 },
                { (2, 0), 1 },
                { (0, 3), 5 },
                { (3, 0), 5 },
                { (1, 2), 4 },
                { (2, 1), 4 },
                { (1, 3), 4 },
                { (3, 1), 4 }
            };

            List<Node> sources = new() { node0, node0, node0, node0, node1, node1, node1 };
            sources.Shuffle(random);
            List<Node> sinks = new() { node2, node2, node2, node2, node2, node3 };
            sinks.Shuffle(random);

            int flow = DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(graph, sources, sinks, capacities);
            Assert.AreEqual(6, flow);
        }

        [TestMethod]
        public void TestNotEnoughCapacities()
        {
            Random random = new(6541);
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node3), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node3), MockCounter);

            List<Node> sources = new() { node0, node0, node0, node0, node1, node1, node1 };
            sources.Shuffle(random);
            List<Node> sinks = new() { node2, node2, node2, node2, node2, node3 };
            sinks.Shuffle(random);

            Dictionary<(uint, uint), int> capacities1 = new()
            {
                { (0, 2), 1 },
                { (2, 0), 1 },
                { (0, 3), 5 },
                { (3, 0), 5 },
                { (1, 2), 4 },
                { (2, 1), 4 },
                { (1, 3), 4 },
                { (3, 1), 4 }
            };

            Dictionary<(uint, uint), int> capacities2 = new()
            {
                { (0, 2), 1 },
                { (0, 3), 5 },
                { (3, 0), 5 },
                { (1, 2), 4 },
                { (2, 1), 4 },
                { (1, 3), 4 },
                { (3, 1), 4 }
            };

            Dictionary<(uint, uint), int> capacities3 = new()
            {
                { (0, 2), 1 },
                { (2, 0), 1 },
                { (0, 3), 5 },
                { (3, 0), 5 },
                { (1, 3), 4 },
                { (3, 1), 4 }
            };

            Dictionary<(uint, uint), int> capacities4 = new()
            {
                { (0, 2), 1 },
                { (2, 0), 1 },
                { (0, 3), 5 },
                { (3, 0), 5 },
                { (1, 2), 4 },
                { (2, 1), 4 },
                { (3, 1), 4 }
            };

            DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(graph, sources, sinks, capacities1);
            DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(graph, node0, node3, capacities1);

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(graph, sources, sinks, capacities2);
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(graph, node0, node3, capacities2);
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(graph, sources, sinks, capacities3);
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(graph, node0, node3, capacities3);
            });
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph, Edge<Node>, Node>(graph, sources, sinks, capacities4);
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlow<Graph, Edge<Node>, Node>(graph, node0, node3, capacities4);
            });
        }

        [TestMethod]
        public void TestSameSourceSink()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node3), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node3), MockCounter);

            int flow = DinicMaxFlow.MaxFlowUnitCapacities<Graph, Edge<Node>, Node>(graph, node1, node1);
            Assert.AreEqual(0, flow);
        }

        [TestMethod]
        public void TestMultipleSourceSinkButActuallyOne()
        {
            Graph graph = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node0, node3), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node2), MockCounter);
            graph.AddEdge(new Edge<Node>(node1, node3), MockCounter);

            List<Node> sources = new() { node0 };
            List<Node> sinks = new() { node2 };

            int flow = DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities<Graph, Edge<Node>, Node>(graph, sources, sinks);
            Assert.AreEqual(2, flow);
        }
    }
}
