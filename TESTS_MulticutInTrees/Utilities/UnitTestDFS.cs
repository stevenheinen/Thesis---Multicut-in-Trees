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
    public class UnitTestDFS
    {
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestFindConnectedComponent()
        {
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);

            node5.AddNeighbour(node6, MockCounter);
            node0.AddNeighbour(node1, MockCounter);
            node1.AddNeighbour(node2, MockCounter);
            node1.AddNeighbour(node3, MockCounter);
            node2.AddNeighbour(node4, MockCounter);

            List<Node> connectedComponent = DFS.FindConnectedComponent(node1, MockCounter);

            Assert.IsTrue(connectedComponent.Contains(node0));
            Assert.IsTrue(connectedComponent.Contains(node1));
            Assert.IsTrue(connectedComponent.Contains(node2));
            Assert.IsTrue(connectedComponent.Contains(node3));
            Assert.IsTrue(connectedComponent.Contains(node4));
            Assert.IsFalse(connectedComponent.Contains(node5));
            Assert.IsFalse(connectedComponent.Contains(node6));
            Assert.AreEqual(connectedComponent.Count, 5);
        }

        [TestMethod]
        public void TestFindConnectedComponentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.FindConnectedComponent(default(RootedTreeNode), MockCounter);
            });
        }

        [TestMethod]
        public void FindConnectedComponentSkip()
        {
            RootedTreeNode node0 = new(0);
            RootedTreeNode node1 = new(1);
            RootedTreeNode node2 = new(2);
            RootedTreeNode node3 = new(3);
            RootedTreeNode node4 = new(4);
            RootedTreeNode node5 = new(5);
            RootedTreeNode node6 = new(6);

            node5.AddNeighbour(node6, MockCounter);
            node0.AddNeighbour(node1, MockCounter);
            node1.AddNeighbour(node2, MockCounter);
            node1.AddNeighbour(node3, MockCounter);
            node2.AddNeighbour(node4, MockCounter);

            List<RootedTreeNode> connectedComponent = DFS.FindConnectedComponent(node1, MockCounter, new HashSet<RootedTreeNode>() { node2 });

            Assert.IsTrue(connectedComponent.Contains(node0));
            Assert.IsTrue(connectedComponent.Contains(node1));
            Assert.IsFalse(connectedComponent.Contains(node2));
            Assert.IsTrue(connectedComponent.Contains(node3));
            Assert.IsFalse(connectedComponent.Contains(node4));
            Assert.IsFalse(connectedComponent.Contains(node5));
            Assert.IsFalse(connectedComponent.Contains(node6));
            Assert.AreEqual(connectedComponent.Count, 3);
        }

        [TestMethod]
        public void FindConnectedComponentSkipNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RootedTreeNode node0 = new(0);
                RootedTreeNode node1 = new(1);
                DFS.FindConnectedComponent(default, MockCounter, new HashSet<RootedTreeNode>() { node0, node1 });
            });
        }


        [TestMethod]
        public void TestFindAllConnectedComponents()
        {
            RootedTreeNode node0 = new(0);
            RootedTreeNode node1 = new(1);
            RootedTreeNode node2 = new(2);
            RootedTreeNode node3 = new(3);
            RootedTreeNode node4 = new(4);
            RootedTreeNode node5 = new(5);
            RootedTreeNode node6 = new(6);

            node5.AddNeighbour(node6, MockCounter);
            node0.AddNeighbour(node1, MockCounter);
            node1.AddNeighbour(node2, MockCounter);
            node1.AddNeighbour(node3, MockCounter);
            node2.AddNeighbour(node4, MockCounter);

            List<RootedTreeNode> nodes = new() { node0, node1, node2, node3, node4, node5, node6 };

            List<List<RootedTreeNode>> allConnectedComponents = DFS.FindAllConnectedComponents(nodes, MockCounter);
            Assert.AreEqual(allConnectedComponents.Count, 2);

            foreach (List<RootedTreeNode> component in allConnectedComponents)
            {
                if (component.Count == 5)
                {
                    Assert.IsTrue(component.Contains(node0));
                    Assert.IsTrue(component.Contains(node1));
                    Assert.IsTrue(component.Contains(node2));
                    Assert.IsTrue(component.Contains(node3));
                    Assert.IsTrue(component.Contains(node4));
                    Assert.IsFalse(component.Contains(node5));
                    Assert.IsFalse(component.Contains(node6));
                }
                else if (component.Count == 2)
                {
                    Assert.IsFalse(component.Contains(node0));
                    Assert.IsFalse(component.Contains(node1));
                    Assert.IsFalse(component.Contains(node2));
                    Assert.IsFalse(component.Contains(node3));
                    Assert.IsFalse(component.Contains(node4));
                    Assert.IsTrue(component.Contains(node5));
                    Assert.IsTrue(component.Contains(node6));
                }
                else
                {
                    Assert.Fail("Found incorrect connected components");
                }
            }
        }

        [TestMethod]
        public void TestFindAllConnectedComponentsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                List<RootedTreeNode> nodes = null;
                DFS.FindAllConnectedComponents(nodes, MockCounter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                List<RootedTreeNode> nodes = null;
                RootedTreeNode node0 = new(0);
                RootedTreeNode node1 = new(1);
                DFS.FindAllConnectedComponents(nodes, MockCounter, new HashSet<RootedTreeNode>() { node0, node1 });
            });
        }

        [TestMethod]
        public void TestFindConnectedNodes()
        {
            RootedTreeNode node0 = new(0);
            RootedTreeNode node1 = new(1);
            RootedTreeNode node2 = new(2);
            RootedTreeNode node3 = new(3);
            RootedTreeNode node4 = new(4);
            RootedTreeNode node5 = new(5);
            RootedTreeNode node6 = new(6);

            node5.AddNeighbour(node6, MockCounter);
            node0.AddNeighbour(node1, MockCounter);
            node1.AddNeighbour(node2, MockCounter);
            node1.AddNeighbour(node3, MockCounter);
            node2.AddNeighbour(node4, MockCounter);

            Assert.IsFalse(DFS.AreConnected(node0, node6, MockCounter));
            Assert.IsTrue(DFS.AreConnected(node4, node3, MockCounter));
            Assert.IsTrue(DFS.AreConnected(node2, node0, MockCounter));
        }

        [TestMethod]
        public void TestFindConnectedNodesNull()
        {
            RootedTreeNode node0 = new(0);
            RootedTreeNode node1 = new(1);
            RootedTreeNode node2 = new(2);
            HashSet<RootedTreeNode> skip = new() { node2 };

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(default, node1, MockCounter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(default, node1, MockCounter, skip);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(node0, default, MockCounter);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(node0, default, MockCounter, skip);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected<RootedTreeNode>(default, default, MockCounter);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(default, default, MockCounter, skip);
            });
        }

        [TestMethod]
        public void TestPathBetween()
        {
            Graph tree = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);

            List<Node> path = DFS.FindPathBetween(node2, node4, MockCounter);
            CollectionAssert.AreEqual(new List<Node>() { node2, node0, node1, node4 }, path);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Node n = new(0);
            Node n2 = new(1);
            List<Node> list = new();
            HashSet<(Node, Node)> hashSet = new();
            RootedTree tree = new();
            Graph graph = new();

            Assert.ThrowsException<ArgumentNullException>(() => DFS.AreConnected(null, n, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.AreConnected(n, null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.AreConnected(n, n2, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllConnectedComponents<Node>(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllConnectedComponents(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindConnectedComponent<Node>(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindConnectedComponent(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclic<Graph, Edge<Node>, Node>(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclic<Graph, Edge<Node>, Node>(graph, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclic<RootedTree, Edge<RootedTreeNode>, RootedTreeNode>(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclic<RootedTree, Edge<RootedTreeNode>, RootedTreeNode>(tree, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindPathBetween(n, null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindPathBetween(null, n, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindPathBetween(n, n2, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FreeNodes(null, hashSet, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FreeNodes(list, null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FreeNodes(list, hashSet, null));
        }

        [TestMethod]
        public void TestAcyclicGraph()
        {
            Graph graph = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge30 = new(node3, node0);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge30 }, MockCounter);

            Assert.IsFalse(DFS.IsAcyclic<Graph, Edge<Node>, Node>(graph, MockCounter));

            graph.RemoveEdge(edge30, MockCounter);

            Assert.IsTrue(DFS.IsAcyclic<Graph, Edge<Node>, Node>(graph, MockCounter));

            graph.RemoveNode(node1, MockCounter);
            graph.RemoveNode(node2, MockCounter);
            graph.RemoveNode(node3, MockCounter);

            Assert.IsTrue(DFS.IsAcyclic<Graph, Edge<Node>, Node>(graph, MockCounter));
        }

        [TestMethod]
        public void TestAcyclicTree()
        {
            RootedTree tree = new();

            RootedTreeNode node0 = new(0);
            RootedTreeNode node1 = new(1);
            RootedTreeNode node2 = new(2);

            Assert.IsTrue(DFS.IsAcyclic<RootedTree, Edge<RootedTreeNode>, RootedTreeNode>(tree, MockCounter));

            tree.AddNode(node0, MockCounter);

            Assert.IsTrue(DFS.IsAcyclic<RootedTree, Edge<RootedTreeNode>, RootedTreeNode>(tree, MockCounter));

            tree.AddNode(node1, MockCounter);
            tree.AddNode(node2, MockCounter);
            Edge<RootedTreeNode> edge01 = new(node0, node1);
            Edge<RootedTreeNode> edge02 = new(node0, node2);
            tree.AddEdge(edge01, MockCounter);
            tree.AddEdge(edge02, MockCounter);

            Assert.IsTrue(DFS.IsAcyclic<RootedTree, Edge<RootedTreeNode>, RootedTreeNode>(tree, MockCounter));
            
            node1.AddNeighbour(node2, MockCounter);

            Assert.IsFalse(DFS.IsAcyclic<RootedTree, Edge<RootedTreeNode>, RootedTreeNode>(tree, MockCounter));
        }

        [TestMethod]
        public void TestFreeNodes()
        {
            Graph graph = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge16 = new(node1, node6);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge24 = new(node2, node4);
            Edge<Node> edge35 = new(node3, node5);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge56 = new(node5, node6);

            graph.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge12, edge13, edge16, edge23, edge24, edge35, edge45, edge56 }, MockCounter);

            HashSet<(Node, Node)> matching = new()
            {
                (node0, node1),
                (node2, node3),
                (node5, node6)
            };

            List<Node> unmatchedNodes = new() { node4 };

            List<Node> freeNodes = DFS.FreeNodes(unmatchedNodes, matching, MockCounter);
            Console.WriteLine(freeNodes.Print());
            Assert.AreEqual(3, freeNodes.Count);
            Assert.IsTrue(freeNodes.Contains(node0));
            Assert.IsTrue(freeNodes.Contains(node3));
            Assert.IsTrue(freeNodes.Contains(node6));
            Assert.IsFalse(freeNodes.Contains(node1));
            Assert.IsFalse(freeNodes.Contains(node2));
            Assert.IsFalse(freeNodes.Contains(node4));
            Assert.IsFalse(freeNodes.Contains(node5));
        }

        [TestMethod]
        public void TestCaterpillarComponents()
        {
            Graph tree = new();
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
            Node node18 = new(18);
            Node node19 = new(19);
            Node node20 = new(20);
            Node node21 = new(21);
            Node node22 = new(22);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22 }, MockCounter);
            Edge<Node> edge54 = new(node5, node4);
            Edge<Node> edge43 = new(node4, node3);
            Edge<Node> edge414 = new(node4, node14);
            Edge<Node> edge32 = new(node3, node2);
            Edge<Node> edge312 = new(node3, node12);
            Edge<Node> edge313 = new(node3, node13);
            Edge<Node> edge21 = new(node2, node1);
            Edge<Node> edge10 = new(node1, node0);
            Edge<Node> edge111 = new(node1, node11);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge59 = new(node5, node9);
            Edge<Node> edge515 = new(node5, node15);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge716 = new(node7, node16);
            Edge<Node> edge717 = new(node7, node17);
            Edge<Node> edge718 = new(node7, node18);
            Edge<Node> edge819 = new(node8, node19);
            Edge<Node> edge820 = new(node8, node20);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge921 = new(node9, node21);
            Edge<Node> edge1022 = new(node10, node22);
            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);
            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = DFS.DetermineCaterpillarComponents(tree.Nodes(MockCounter), MockCounter);

            Assert.AreEqual(-1, caterpillars[node0]);
            Assert.AreEqual(-1, caterpillars[node1]);
            Assert.AreEqual(-1, caterpillars[node11]);
            Assert.AreEqual(-1, caterpillars[node15]);
            Assert.AreEqual(-1, caterpillars[node5]);
            Assert.AreEqual(-1, caterpillars[node10]);
            Assert.AreEqual(-1, caterpillars[node22]);
            Assert.AreEqual(-1, caterpillars[node8]);
            Assert.AreEqual(-1, caterpillars[node19]);
            Assert.AreEqual(-1, caterpillars[node20]);
            Assert.AreNotEqual(-1, caterpillars[node2]);
            Assert.AreEqual(caterpillars[node2], caterpillars[node3]);
            Assert.AreEqual(caterpillars[node2], caterpillars[node12]);
            Assert.AreEqual(caterpillars[node2], caterpillars[node13]);
            Assert.AreEqual(caterpillars[node2], caterpillars[node4]);
            Assert.AreEqual(caterpillars[node2], caterpillars[node14]);
            Assert.AreNotEqual(caterpillars[node2], caterpillars[node6]);
            Assert.AreNotEqual(-1, caterpillars[node6]);
            Assert.AreEqual(caterpillars[node6], caterpillars[node7]);
            Assert.AreEqual(caterpillars[node6], caterpillars[node16]);
            Assert.AreEqual(caterpillars[node6], caterpillars[node17]);
            Assert.AreEqual(caterpillars[node6], caterpillars[node18]);
            Assert.AreNotEqual(caterpillars[node6], caterpillars[node9]);
            Assert.AreNotEqual(caterpillars[node2], caterpillars[node9]);
            Assert.AreNotEqual(-1, caterpillars[node9]);
            Assert.AreEqual(caterpillars[node9], caterpillars[node21]);
        }
    }
}
