// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
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
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestArgumentNull()
        {
            Graph<Node> g = new Graph<Node>();
            Node n = new Node(0);
            Dictionary<(uint, uint), int> c = new Dictionary<(uint, uint), int>();
            Dictionary<uint, int> l = new Dictionary<uint, int>();

            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities(g, new List<Node>() { n }, null));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities(g, null, new List<Node>() { n }));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities<Graph<Node>, Node>(null, new List<Node>() { n }, new List<Node>() { n }));

            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinks(g, new List<Node>() { n }, null, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinks(g, null, new List<Node>() { n }, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph<Node>, Node>(null, new List<Node>() { n }, new List<Node>() { n }, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowMultipleSourcesSinks<Graph<Node>, Node>(g, new List<Node>() { n }, new List<Node>() { n }, null));

            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowUnitCapacities(g, n, null));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowUnitCapacities(g, null, n));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlowUnitCapacities<Graph<Node>, Node>(null, n, n));

            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlow(g, n, n, null));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlow(g, n, null, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlow(g, null, n, c));
            Assert.ThrowsException<ArgumentNullException>(() => DinicMaxFlow.MaxFlow<Graph<Node>, Node>(null, n, n, c));

            MethodInfo methodFindLevels = typeof(DinicMaxFlow).GetMethod("FindLevels", BindingFlags.NonPublic | BindingFlags.Static);
            methodFindLevels = methodFindLevels.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });

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
            methodSendFlow = methodSendFlow.MakeGenericMethod(new Type[2] { typeof(Graph<Node>), typeof(Node) });

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
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, counter);
            graph.AddEdge(node0, node1, counter, true);
            graph.AddEdge(node0, node2, counter, true);
            graph.AddEdge(node2, node1, counter, true);
            graph.AddEdge(node1, node3, counter, true);
            graph.AddEdge(node2, node4, counter, true);
            graph.AddEdge(node4, node3, counter, true);
            graph.AddEdge(node4, node5, counter, true);
            graph.AddEdge(node3, node5, counter, true);

            Dictionary<(uint, uint), int> capacities = new Dictionary<(uint, uint), int>()
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

            int flow = DinicMaxFlow.MaxFlow(graph, node0, node5, capacities);
            Assert.AreEqual(23, flow);
        }

        [TestMethod]
        public void TestFlowSingleSourceSingleSinkUnitCapacities()
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, counter);

            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node6, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node8, counter);
            graph.AddEdge(node5, node6, counter);
            graph.AddEdge(node6, node7, counter);
            graph.AddEdge(node6, node8, counter);
            graph.AddEdge(node6, node9, counter);
            graph.AddEdge(node9, node10, counter);
            graph.AddEdge(node8, node10, counter);
            graph.AddEdge(node7, node10, counter);

            int flow = DinicMaxFlow.MaxFlowUnitCapacities(graph, node0, node10);
            Assert.AreEqual(3, flow);
        }

        [TestMethod]
        public void TestFlowSingleSourceSingleSinkUnitCapacities2()
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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, counter);

            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node6, counter);
            graph.AddEdge(node1, node5, counter);
            graph.AddEdge(node2, node5, counter);
            graph.AddEdge(node3, node4, counter);
            graph.AddEdge(node4, node8, counter);
            graph.AddEdge(node5, node6, counter);
            graph.AddEdge(node6, node9, counter);
            graph.AddEdge(node9, node10, counter);
            graph.AddEdge(node8, node10, counter);
            graph.AddEdge(node7, node10, counter);

            int flow = DinicMaxFlow.MaxFlowUnitCapacities(graph, node0, node10);
            Assert.AreEqual(2, flow);
        }

        [TestMethod]
        public void TestFlowMultipleSourceMultipleSinkUnitCapacities()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node0, node1, counter);
            graph.AddEdge(node1, node2, counter);

            List<Node> sources = new List<Node>() { node0, node1 };
            List<Node> sinks = new List<Node>() { node2, node3 };

            int flow = DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities(graph, sources, sinks);
            Assert.AreEqual(2, flow);
        }

        [TestMethod]
        public void TestFlowMultipleSourceMultipleSink()
        {
            Random random = new Random(34698);
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node3, counter);

            Dictionary<(uint, uint), int> capacities = new Dictionary<(uint, uint), int>()
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

            List<Node> sources = new List<Node>() { node0, node0, node0, node0, node1, node1, node1 };
            sources.Shuffle(random);
            List<Node> sinks = new List<Node>() { node2, node2, node2, node2, node2, node3 };
            sinks.Shuffle(random);

            int flow = DinicMaxFlow.MaxFlowMultipleSourcesSinks(graph, sources, sinks, capacities);
            Assert.AreEqual(6, flow);
        }

        [TestMethod]
        public void TestNotEnoughCapacities()
        {
            Random random = new Random(6541);
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node3, counter);

            List<Node> sources = new List<Node>() { node0, node0, node0, node0, node1, node1, node1 };
            sources.Shuffle(random);
            List<Node> sinks = new List<Node>() { node2, node2, node2, node2, node2, node3 };
            sinks.Shuffle(random);

            Dictionary<(uint, uint), int> capacities1 = new Dictionary<(uint, uint), int>()
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

            Dictionary<(uint, uint), int> capacities2 = new Dictionary<(uint, uint), int>()
            {
                { (0, 2), 1 },
                { (0, 3), 5 },
                { (3, 0), 5 },
                { (1, 2), 4 },
                { (2, 1), 4 },
                { (1, 3), 4 },
                { (3, 1), 4 }
            };

            Dictionary<(uint, uint), int> capacities3 = new Dictionary<(uint, uint), int>()
            {
                { (0, 2), 1 },
                { (2, 0), 1 },
                { (0, 3), 5 },
                { (3, 0), 5 },
                { (1, 3), 4 },
                { (3, 1), 4 }
            };

            Dictionary<(uint, uint), int> capacities4 = new Dictionary<(uint, uint), int>()
            {
                { (0, 2), 1 },
                { (2, 0), 1 },
                { (0, 3), 5 },
                { (3, 0), 5 },
                { (1, 2), 4 },
                { (2, 1), 4 },
                { (3, 1), 4 }
            };

            DinicMaxFlow.MaxFlowMultipleSourcesSinks(graph, sources, sinks, capacities1);
            DinicMaxFlow.MaxFlow(graph, node0, node3, capacities1);

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlowMultipleSourcesSinks(graph, sources, sinks, capacities2);
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlow(graph, node0, node3, capacities2);
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlowMultipleSourcesSinks(graph, sources, sinks, capacities3);
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlow(graph, node0, node3, capacities3);
            });
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlowMultipleSourcesSinks(graph, sources, sinks, capacities4);
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                DinicMaxFlow.MaxFlow(graph, node0, node3, capacities4);
            });
        }

        [TestMethod]
        public void TestSameSourceSink()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node3, counter);

            int flow = DinicMaxFlow.MaxFlowUnitCapacities(graph, node1, node1);
            Assert.AreEqual(0, flow);
        }

        [TestMethod]
        public void TestMultipleSourceSinkButActuallyOne()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, counter);
            graph.AddEdge(node0, node2, counter);
            graph.AddEdge(node0, node3, counter);
            graph.AddEdge(node1, node2, counter);
            graph.AddEdge(node1, node3, counter);

            List<Node> sources = new List<Node>() { node0 };
            List<Node> sinks = new List<Node>() { node2 };

            int flow = DinicMaxFlow.MaxFlowMultipleSourcesSinksUnitCapacities(graph, sources, sinks);
            Assert.AreEqual(2, flow);
        }
    }
}
