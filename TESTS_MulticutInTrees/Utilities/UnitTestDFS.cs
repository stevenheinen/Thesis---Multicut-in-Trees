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
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestFindConnectedComponent()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);

            node5.AddChild(node6, counter);

            node0.AddChild(node1, counter);
            node1.AddChild(node2, counter);
            node1.AddChild(node3, counter);
            node2.AddChild(node4, counter);

            List<TreeNode> connectedComponent = DFS.FindConnectedComponent(node1, counter);

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
                DFS.FindConnectedComponent(default(TreeNode), counter);
            });
        }

        [TestMethod]
        public void FindConnectedComponentSkip()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);

            node5.AddChild(node6, counter);

            node0.AddChild(node1, counter);
            node1.AddChild(node2, counter);
            node1.AddChild(node3, counter);
            node2.AddChild(node4, counter);

            List<TreeNode> connectedComponent = DFS.FindConnectedComponent(node1, counter, new HashSet<TreeNode>() { node2 });

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
                TreeNode node0 = new TreeNode(0);
                TreeNode node1 = new TreeNode(1);
                DFS.FindConnectedComponent(default, counter, new HashSet<TreeNode>() { node0, node1 });
            });
        }


        [TestMethod]
        public void TestFindAllConnectedComponents()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);

            node5.AddChild(node6, counter);

            node0.AddChild(node1, counter);
            node1.AddChild(node2, counter);
            node1.AddChild(node3, counter);
            node2.AddChild(node4, counter);

            List<TreeNode> nodes = new List<TreeNode>() { node0, node1, node2, node3, node4, node5, node6 };

            List<List<TreeNode>> allConnectedComponents = DFS.FindAllConnectedComponents(nodes, counter);
            Assert.AreEqual(allConnectedComponents.Count, 2);

            foreach (List<TreeNode> component in allConnectedComponents)
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
                List<TreeNode> nodes = null;
                DFS.FindAllConnectedComponents(nodes, counter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                List<TreeNode> nodes = null;
                TreeNode node0 = new TreeNode(0);
                TreeNode node1 = new TreeNode(1);
                DFS.FindAllConnectedComponents(nodes, counter, new HashSet<TreeNode>() { node0, node1 });
            });
        }

        [TestMethod]
        public void TestFindConnectedNodes()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);

            node5.AddChild(node6, counter);

            node0.AddChild(node1, counter);
            node1.AddChild(node2, counter);
            node1.AddChild(node3, counter);
            node2.AddChild(node4, counter);

            Assert.IsFalse(DFS.AreConnected(node0, node6, counter));
            Assert.IsTrue(DFS.AreConnected(node4, node3, counter));
            Assert.IsTrue(DFS.AreConnected(node2, node0, counter));
        }

        [TestMethod]
        public void TestFindConnectedNodesNull()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            HashSet<TreeNode> skip = new HashSet<TreeNode>() { node2 };

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(default, node1, counter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(default, node1, counter, skip);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(node0, default, counter);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(node0, default, counter, skip);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected<TreeNode>(default, default, counter);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(default, default, counter, skip);
            });
        }

        [TestMethod]
        public void TestNullFindEdges()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.FindAllEdgesGraph<Graph<Node>, Node>(null, counter);
            });
            Assert.AreEqual(a.ParamName, "graph");

            ArgumentNullException b = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.FindAllEdgesGraph<Graph<Node>, Node>(default, counter);
            });
            Assert.AreEqual(b.ParamName, "graph");
        }

        [TestMethod]
        public void TestFindAllEdgesGraph()
        {
            Graph<Node> graph = new Graph<Node>();

            Assert.AreEqual(0, DFS.FindAllEdgesGraph<Graph<Node>, Node>(graph, counter).Count);

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);

            List<(Node, Node)> edges = new List<(Node, Node)>()
            {
                (node0, node1),
                (node0, node2),
                (node1, node2),
                (node1, node3),
                (node2, node4),
                (node3, node4)
            };

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, counter);
            graph.AddEdges(edges, counter);

            List<(Node, Node)> foundEdges = DFS.FindAllEdgesGraph<Graph<Node>, Node>(graph, counter);

            foreach ((Node, Node) edge in foundEdges)
            {
                (Node, Node) edge2 = (edge.Item2, edge.Item1);
                Assert.IsTrue(edges.Contains(edge) || edges.Contains(edge2));
            }

            foreach ((Node, Node) edge in edges)
            {
                (Node, Node) edge2 = (edge.Item2, edge.Item1);
                Assert.IsTrue(foundEdges.Contains(edge) || foundEdges.Contains(edge2));
            }
        }

        [TestMethod]
        public void TestFindAllEdgesTree()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            Assert.AreEqual(0, DFS.FindAllEdgesTree<Tree<TreeNode>, TreeNode>(tree, counter).Count);

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            List<(TreeNode, TreeNode)> edges = new List<(TreeNode, TreeNode)>()
            {
                (node0, node1),
                (node0, node2),
                (node1, node3),
                (node1, node4)
            };

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            List<(TreeNode, TreeNode)> foundEdges = DFS.FindAllEdgesTree<Tree<TreeNode>, TreeNode>(tree, counter);

            foreach ((TreeNode, TreeNode) edge in foundEdges)
            {
                (TreeNode, TreeNode) edge2 = (edge.Item2, edge.Item1);
                Assert.IsTrue(edges.Contains(edge) || edges.Contains(edge2));
            }

            foreach ((TreeNode, TreeNode) edge in edges)
            {
                (TreeNode, TreeNode) edge2 = (edge.Item2, edge.Item1);
                Assert.IsTrue(foundEdges.Contains(edge) || foundEdges.Contains(edge2));
            }
        }

        [TestMethod]
        public void TestPathBetween()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            List<TreeNode> path = DFS.FindPathBetween(node2, node4, counter);
            CollectionAssert.AreEqual(new List<TreeNode>() { node2, node0, node1, node4 }, path);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Node n = new Node(0);
            Node n2 = new Node(1);
            List<Node> list = new List<Node>();
            HashSet<(Node, Node)> hashSet = new HashSet<(Node, Node)>();
            Tree<TreeNode> tree = new Tree<TreeNode>();
            Graph<Node> graph = new Graph<Node>();

            Assert.ThrowsException<ArgumentNullException>(() => DFS.AreConnected(null, n, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.AreConnected(n, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.AreConnected(n, n2, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllConnectedComponents<Node>(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllConnectedComponents(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllEdgesGraph<Graph<Node>, Node>(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllEdgesGraph<Graph<Node>, Node>(graph, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllEdgesTree<Tree<TreeNode>, TreeNode>(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllEdgesTree<Tree<TreeNode>, TreeNode>(tree, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindConnectedComponent<Node>(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindConnectedComponent(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclicGraph<Graph<Node>, Node>(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclicGraph<Graph<Node>, Node>(graph, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindPathBetween(n, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindPathBetween(null, n, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindPathBetween(n, n2, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FreeNodes(null, hashSet, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FreeNodes(list, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FreeNodes(list, hashSet, null));
        }

        [TestMethod]
        public void TestAcyclicGraph()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 }, counter);
            graph.AddEdges(new List<(Node, Node)>()
            {
                (node0, node1),
                (node1, node2),
                (node2, node3),
                (node3, node0)
            }, counter);

            Assert.IsFalse(DFS.IsAcyclicGraph<Graph<Node>, Node>(graph, counter));

            graph.RemoveEdge(node0, node3, counter);

            Assert.IsTrue(DFS.IsAcyclicGraph<Graph<Node>, Node>(graph, counter));

            graph.RemoveNode(node1, counter);
            graph.RemoveNode(node2, counter);
            graph.RemoveNode(node3, counter);

            Assert.IsTrue(DFS.IsAcyclicGraph<Graph<Node>, Node>(graph, counter));
        }

        [TestMethod]
        public void TestAcyclicTree()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);

            Assert.ThrowsException<NoRootException>(() => DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree, counter));

            tree.AddRoot(node0, counter);

            Assert.IsTrue(DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree, counter));

            tree.AddChild(node0, node1, counter);
            tree.AddChild(node0, node2, counter);

            Assert.IsTrue(DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree, counter));

            node1.AddChild(node2, counter);

            Assert.IsFalse(DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree, counter));
        }

        [TestMethod]
        public void TestFreeNodes()
        {
            Graph<Node> graph = new Graph<Node>();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, counter);

            graph.AddEdges(new List<(Node, Node)>()
            {
                (node0, node1),
                (node0, node2),
                (node1, node2),
                (node1, node3),
                (node1, node6),
                (node2, node3),
                (node2, node4),
                (node3, node5),
                (node4, node5),
                (node5, node6)
            }, counter);

            HashSet<(Node, Node)> matching = new HashSet<(Node, Node)>()
            {
                (node0, node1),
                (node2, node3),
                (node5, node6)
            };

            List<Node> unmatchedNodes = new List<Node>() { node4 };

            List<Node> freeNodes = DFS.FreeNodes(unmatchedNodes, matching, counter);
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
    }
}
