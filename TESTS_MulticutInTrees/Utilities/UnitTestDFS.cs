// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestDFS
    {
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

            node5.AddChild(node6);

            node0.AddChild(node1);
            node1.AddChild(node2);
            node1.AddChild(node3);
            node2.AddChild(node4);

            List<TreeNode> connectedComponent = DFS.FindConnectedComponent(node1);

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
                DFS.FindConnectedComponent(default(TreeNode));
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

            node5.AddChild(node6);

            node0.AddChild(node1);
            node1.AddChild(node2);
            node1.AddChild(node3);
            node2.AddChild(node4);

            List<TreeNode> connectedComponent = DFS.FindConnectedComponent(node1, new HashSet<TreeNode>() { node2 });

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
                DFS.FindConnectedComponent(default, new HashSet<TreeNode>() { node0, node1 });
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

            node5.AddChild(node6);

            node0.AddChild(node1);
            node1.AddChild(node2);
            node1.AddChild(node3);
            node2.AddChild(node4);

            List<TreeNode> nodes = new List<TreeNode>() { node0, node1, node2, node3, node4, node5, node6 };

            List<List<TreeNode>> allConnectedComponents = DFS.FindAllConnectedComponents(nodes);
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
                DFS.FindAllConnectedComponents(nodes);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                List<TreeNode> nodes = null;
                TreeNode node0 = new TreeNode(0);
                TreeNode node1 = new TreeNode(1);
                DFS.FindAllConnectedComponents(nodes, new HashSet<TreeNode>() { node0, node1 });
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

            node5.AddChild(node6);

            node0.AddChild(node1);
            node1.AddChild(node2);
            node1.AddChild(node3);
            node2.AddChild(node4);

            Assert.IsFalse(DFS.AreConnected(node0, node6));
            Assert.IsTrue(DFS.AreConnected(node4, node3));
            Assert.IsTrue(DFS.AreConnected(node2, node0));
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
                DFS.AreConnected(default, node1);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(default, node1, skip);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(node0, default);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(node0, default, skip);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected<TreeNode>(default, default);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.AreConnected(default, default, skip);
            });
        }

        [TestMethod]
        public void TestNullFindEdges()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.FindAllEdgesGraph<Graph<Node>, Node>(null);
            });
            Assert.AreEqual(a.ParamName, "graph");

            ArgumentNullException b = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                DFS.FindAllEdgesGraph<Graph<Node>, Node>(default);
            });
            Assert.AreEqual(b.ParamName, "graph");
        }

        [TestMethod]
        public void TestFindAllEdgesGraph()
        {
            Graph<Node> graph = new Graph<Node>();

            Assert.AreEqual(0, DFS.FindAllEdgesGraph<Graph<Node>, Node>(graph).Count);

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

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 });
            graph.AddEdges(edges);

            List<(Node, Node)> foundEdges = DFS.FindAllEdgesGraph<Graph<Node>, Node>(graph);

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

            Assert.AreEqual(0, DFS.FindAllEdgesTree<Tree<TreeNode>, TreeNode>(tree).Count);

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

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            List<(TreeNode, TreeNode)> foundEdges = DFS.FindAllEdgesTree<Tree<TreeNode>, TreeNode>(tree);

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
        public void TestNullArgument()
        {
            Node n = new Node(0);

            Assert.ThrowsException<ArgumentNullException>(() => DFS.AreConnected(null, n));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.AreConnected(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllConnectedComponents<Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllEdgesGraph<Graph<Node>, Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindAllEdgesTree<Tree<TreeNode>, TreeNode>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.FindConnectedComponent<Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclicGraph<Graph<Node>, Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(null));
        }
        
        [TestMethod]
        public void TestAcyclicGraph()
        {
            Graph<Node> graph = new Graph<Node>();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            graph.AddNodes(new List<Node>() { node0, node1, node2, node3 });
            graph.AddEdges(new List<(Node, Node)>()
            {
                (node0, node1),
                (node1, node2),
                (node2, node3),
                (node3, node0)
            });

            Assert.IsFalse(DFS.IsAcyclicGraph<Graph<Node>, Node>(graph));

            graph.RemoveEdge(node0, node3);

            Assert.IsTrue(DFS.IsAcyclicGraph<Graph<Node>, Node>(graph));

            graph.RemoveNode(node1);
            graph.RemoveNode(node2);
            graph.RemoveNode(node3);

            Assert.IsTrue(DFS.IsAcyclicGraph<Graph<Node>, Node>(graph));
        }

        [TestMethod]
        public void TestAcyclicTree()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);

            Assert.ThrowsException<NoRootException>(() => DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree));

            tree.AddRoot(node0);

            Assert.IsTrue(DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree));

            tree.AddChild(node0, node1);
            tree.AddChild(node0, node2);

            Assert.IsTrue(DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree));

            node1.AddChild(node2);

            Assert.IsFalse(DFS.IsAcyclicTree<Tree<TreeNode>, TreeNode>(tree));
        }
    }
}
