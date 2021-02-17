// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace TESTS_MulticutInTrees.Graphs
{
    [TestClass]
    public class UnitTestTreeNode
    {
        [TestMethod]
        public void TestConstructorID()
        {
            TreeNode node = new TreeNode(0);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestID()
        {
            TreeNode node = new TreeNode(3248);
            Assert.AreEqual(node.ID, (uint)3248);
        }

        [TestMethod]
        public void TestChildren()
        {
            TreeNode node0 = new TreeNode(0);
            Assert.IsNotNull(node0.Children);
        }

        [TestMethod]
        public void TestParent()
        {
            TreeNode parent = new TreeNode(0);
            TreeNode child = new TreeNode(1);
            TreeNode node = new TreeNode(2);
            parent.AddChild(node);
            node.AddChild(child);
            Assert.AreEqual(child.Parent, node);
            Assert.AreEqual(node.Parent, parent);
        }

        [TestMethod]
        public void TestIsRoot()
        {
            TreeNode root = new TreeNode(0);
            TreeNode notRoot = new TreeNode(1);
            root.AddChild(notRoot);
            Assert.IsTrue(root.IsRoot());
            Assert.IsFalse(notRoot.IsRoot());
        }

        [TestMethod]
        public void TestHasChild()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            node0.AddChild(node1);
            Assert.IsTrue(node0.HasChild(node1));
            Assert.IsFalse(node1.HasChild(node0));

            TreeNode node2 = new TreeNode(2);
            node2.AddChild(node0);
            Assert.IsTrue(node2.HasChild(node0));
            Assert.IsFalse(node0.HasChild(node2));

            Assert.IsFalse(node2.HasChild(node1));
            Assert.IsFalse(node1.HasChild(node2));
        }

        [TestMethod]
        public void TestHasNullChild()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = new TreeNode(0);
                node.HasChild(null);
            });
        }

        [TestMethod]
        public void TestToString()
        {
            TreeNode node = new TreeNode(5664);
            Assert.AreEqual($"{node}", "TreeNode 5664");
        }

        [TestMethod]
        public void TestAddChild()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);

            node0.AddChild(node1);
            node0.AddChild(node2);

            Assert.IsTrue(node0.HasChild(node1));
            Assert.IsTrue(node0.HasChild(node2));

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.AddChild(null);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                node0.AddChild(node2);
            });

            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                node0.AddChild(node0);
            });

            Assert.ThrowsException<AddParentAsChildException>(() =>
            {
                node1.AddChild(node0);
            });
        }

        [TestMethod]
        public void TestAddChildren()
        {
            TreeNode parent = new TreeNode(0);
            List<TreeNode> children = new List<TreeNode>()
            {
                new TreeNode(1),
                new TreeNode(2),
                new TreeNode(3),
                new TreeNode(4),
            };

            parent.AddChildren(children);
            foreach (TreeNode child in children)
            {
                Assert.IsTrue(parent.HasChild(child));
                Assert.IsTrue(child.Parent == parent);
            }

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.AddChildren(null);
            });
        }

        [TestMethod]
        public void TestRemoveChild()
        {
            TreeNode parent = new TreeNode(0);
            TreeNode child = new TreeNode(1);
            parent.AddChild(child);
            TreeNode nonChild = new TreeNode(2);

            parent.RemoveChild(child);

            Assert.IsTrue(child.Parent is null);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveChild(child);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveChild(nonChild);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveChild(parent);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveChild(null);
            });
        }

        [TestMethod]
        public void TestRemoveChildren()
        {
            List<TreeNode> children = new List<TreeNode>()
            {
                new TreeNode(1),
                new TreeNode(2),
                new TreeNode(3),
                new TreeNode(4),
            };
            TreeNode parent = new TreeNode(0);
            TreeNode child = new TreeNode(5);
            parent.AddChildren(children);
            parent.AddChild(child);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveChildren(null);
            });

            parent.RemoveChildren(children);

            foreach (TreeNode _child in children)
            {
                Assert.IsTrue(_child.Parent is null);
                Assert.IsFalse(parent.HasChild(_child));
            }

            Assert.IsTrue(parent.HasChild(child));
            Assert.IsTrue(child.Parent == parent);
        }

        [TestMethod]
        public void TestRemoveAllChildren()
        {
            List<TreeNode> children = new List<TreeNode>()
            {
                new TreeNode(1),
                new TreeNode(2),
            };
            TreeNode parent = new TreeNode(0);
            parent.AddChildren(children);

            parent.RemoveAllChildren();

            Assert.IsTrue(parent.Children.Count == 0);
            Assert.IsFalse(parent.HasChild(children[0]));
            Assert.IsFalse(parent.HasChild(children[1]));
            Assert.IsTrue(children[0].Parent is null);
            Assert.IsTrue(children[1].Parent is null);
        }

        [TestMethod]
        public void TestDegree()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            Assert.AreEqual(node0.Degree, 0);

            node0.AddChildren(new List<TreeNode>() { node1, node2, node3 });
            Assert.AreEqual(node0.Degree, 3);
            Assert.AreEqual(node1.Degree, 1);

            node0.RemoveChild(node1);
            Assert.AreEqual(node0.Degree, 2);
            Assert.AreEqual(node1.Degree, 0);

            node4.AddChild(node0);
            Assert.AreEqual(node0.Degree, 3);

            node0.AddChild(node5);
            node0.RemoveChild(node2);
            Assert.AreEqual(node0.Degree, 3);
        }

        [TestMethod]
        public void TestDepthFromRoot()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);

            node0.AddChildren(new List<TreeNode>() { node1, node2, node3 });
            node2.AddChild(node4);
            node4.AddChild(node5);
            node3.AddChild(node6);
            node3.RemoveChild(node6);

            Assert.AreEqual(node0.DepthFromRoot, 0);
            Assert.AreEqual(node1.DepthFromRoot, 1);
            Assert.AreEqual(node2.DepthFromRoot, 1);
            Assert.AreEqual(node3.DepthFromRoot, 1);
            Assert.AreEqual(node4.DepthFromRoot, 2);
            Assert.AreEqual(node5.DepthFromRoot, 3);
            Assert.AreEqual(node6.DepthFromRoot, 0);
        }

        [TestMethod]
        public void TestDepthOfSubtree()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);

            node0.AddChildren(new List<TreeNode>() { node1, node2 });
            node1.AddChild(node3);
            node2.AddChild(node4);
            node4.AddChild(node5);
            node3.AddChild(node6);
            node6.AddChild(node7);
            node3.RemoveChild(node6);

            Assert.AreEqual(node0.HeightOfSubtree, 3);
            Assert.AreEqual(node1.HeightOfSubtree, 1);
            Assert.AreEqual(node2.HeightOfSubtree, 2);
            Assert.AreEqual(node3.HeightOfSubtree, 0);
            Assert.AreEqual(node4.HeightOfSubtree, 1);
            Assert.AreEqual(node5.HeightOfSubtree, 0);
            Assert.AreEqual(node6.HeightOfSubtree, 1);
            Assert.AreEqual(node7.HeightOfSubtree, 0);
        }

        [TestMethod]
        public void TestAddNeighbour()
        {
            ITreeNode<TreeNode> node0 = new TreeNode(0);
            ITreeNode<TreeNode> node1 = new TreeNode(1);
            ITreeNode<TreeNode> node2 = new TreeNode(2);

            node0.AddNeighbour((TreeNode)node1);
            node0.AddNeighbour((TreeNode)node2);

            Assert.IsTrue(node0.HasChild((TreeNode)node1));
            Assert.IsTrue(node0.HasChild((TreeNode)node2));

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.AddNeighbour(null);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                node0.AddNeighbour((TreeNode)node2);
            });

            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                node0.AddNeighbour((TreeNode)node0);
            });

            Assert.ThrowsException<AddParentAsChildException>(() =>
            {
                node1.AddNeighbour((TreeNode)node0);
            });
        }

        [TestMethod]
        public void TestAddNeighbours()
        {
            ITreeNode<TreeNode> parent = new TreeNode(0);
            List<TreeNode> children = new List<TreeNode>()
            {
                new TreeNode(1),
                new TreeNode(2),
                new TreeNode(3),
                new TreeNode(4),
            };

            parent.AddNeighbours(children);
            foreach (TreeNode child in children)
            {
                Assert.IsTrue(parent.HasChild(child));
                Assert.IsTrue(child.Parent == parent);
            }

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.AddNeighbours(null);
            });
        }

        [TestMethod]
        public void TestRemoveAllNeighbours()
        {
            TreeNode uberParent = new TreeNode(3);
            List<TreeNode> children = new List<TreeNode>()
            {
                new TreeNode(1),
                new TreeNode(2),
            };
            ITreeNode<TreeNode> parent = new TreeNode(0);
            parent.AddChildren(children);
            uberParent.AddChild((TreeNode)parent);

            parent.RemoveAllNeighbours();

            Assert.IsTrue(parent.Children.Count == 0);
            Assert.IsFalse(parent.HasChild(children[0]));
            Assert.IsFalse(parent.HasChild(children[1]));
            Assert.IsTrue(children[0].Parent is null);
            Assert.IsTrue(children[1].Parent is null);
            Assert.IsFalse(uberParent.HasChild((TreeNode)parent));
            Assert.IsTrue(parent.Parent is null);
        }

        [TestMethod]
        public void TestRemoveNeighbour()
        {
            ITreeNode<TreeNode> parent = new TreeNode(0);
            TreeNode child = new TreeNode(1);
            parent.AddChild(child);
            TreeNode nonChild = new TreeNode(2);
            TreeNode uberParent = new TreeNode(3);
            uberParent.AddChild((TreeNode)parent);
            Assert.IsTrue(parent.Parent == uberParent);
            
            parent.RemoveNeighbour(child);

            Assert.IsTrue(child.Parent is null);

            parent.RemoveNeighbour(uberParent);
            Assert.IsTrue(parent.Parent is null);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(child);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(nonChild);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour((TreeNode)parent);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveNeighbour(null);
            });
        }

        [TestMethod]
        public void TestRemoveNeighbours()
        {
            List<TreeNode> children = new List<TreeNode>()
            {
                new TreeNode(1),
                new TreeNode(2),
                new TreeNode(3),
                new TreeNode(4),
            };
            ITreeNode<TreeNode> parent = new TreeNode(0);
            parent.AddChildren(children);
            TreeNode child = new TreeNode(5);
            parent.AddChild(child);
            TreeNode uberParent = new TreeNode(6);
            uberParent.AddChild((TreeNode)parent);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveNeighbours(null);
            });

            parent.RemoveNeighbours(new List<TreeNode>(children) { uberParent });

            foreach (TreeNode _child in children)
            {
                Assert.IsTrue(_child.Parent is null);
                Assert.IsFalse(parent.HasChild(_child));
            }

            Assert.IsFalse(uberParent.HasChild((TreeNode)parent));
            Assert.IsTrue(parent.Parent is null);

            Assert.IsTrue(parent.HasChild(child));
            Assert.IsTrue(child.Parent == parent);
        }

        [TestMethod]
        public void TestHasNeighbour()
        {
            TreeNode node0 = new TreeNode(0);
            ITreeNode<TreeNode> node1 = new TreeNode(1);
            node0.AddChild((TreeNode)node1);
            TreeNode node2 = new TreeNode(2);
            node1.AddChild(node2);
            Assert.IsTrue(node1.HasNeighbour(node2));
            Assert.IsTrue(node1.HasNeighbour(node0));

            Assert.IsFalse(((ITreeNode<TreeNode>)node0).HasNeighbour(node2));
            Assert.IsFalse(((ITreeNode<TreeNode>)node2).HasNeighbour(node0));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.HasNeighbour(null);
            });
            Assert.AreEqual("node", a.ParamName);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            TreeNode t = new TreeNode(0);

            Assert.ThrowsException<ArgumentNullException>(() => t.AddChild(null));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChildren(null));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasChild(null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveChild(null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveChildren(null));
        }
    }
}
