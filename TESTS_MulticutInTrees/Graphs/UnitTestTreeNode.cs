// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace TESTS_MulticutInTrees.Graphs
{
    [TestClass]
    public class UnitTestTreeNode
    {
        private static readonly Counter counter = new Counter();

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
            Assert.IsNotNull(node0.Children(counter));
        }

        [TestMethod]
        public void TestParent()
        {
            TreeNode parent = new TreeNode(0);
            TreeNode child = new TreeNode(1);
            TreeNode node = new TreeNode(2);
            parent.AddChild(node, counter);
            node.AddChild(child, counter);
            Assert.AreEqual(child.GetParent(counter), node);
            Assert.AreEqual(node.GetParent(counter), parent);
        }

        [TestMethod]
        public void TestIsRoot()
        {
            TreeNode root = new TreeNode(0);
            TreeNode notRoot = new TreeNode(1);
            root.AddChild(notRoot, counter);
            Assert.IsTrue(root.IsRoot(counter));
            Assert.IsFalse(notRoot.IsRoot(counter));
        }

        [TestMethod]
        public void TestHasChild()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            node0.AddChild(node1, counter);
            Assert.IsTrue(node0.HasChild(node1, counter));
            Assert.IsFalse(node1.HasChild(node0, counter));

            TreeNode node2 = new TreeNode(2);
            node2.AddChild(node0, counter);
            Assert.IsTrue(node2.HasChild(node0, counter));
            Assert.IsFalse(node0.HasChild(node2, counter));

            Assert.IsFalse(node2.HasChild(node1, counter));
            Assert.IsFalse(node1.HasChild(node2, counter));
        }

        [TestMethod]
        public void TestHasNullChild()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = new TreeNode(0);
                node.HasChild(null, counter);
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

            node0.AddChild(node1, counter);
            node0.AddChild(node2, counter);

            Assert.IsTrue(node0.HasChild(node1, counter));
            Assert.IsTrue(node0.HasChild(node2, counter));

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.AddChild(null, counter);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                node0.AddChild(node2, counter);
            });

            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                node0.AddChild(node0, counter);
            });

            Assert.ThrowsException<AddParentAsChildException>(() =>
            {
                node1.AddChild(node0, counter);
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

            parent.AddChildren(children, counter);
            foreach (TreeNode child in children)
            {
                Assert.IsTrue(parent.HasChild(child, counter));
                Assert.IsTrue(child.GetParent(counter) == parent);
            }

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.AddChildren(null, counter);
            });
        }

        [TestMethod]
        public void TestRemoveChild()
        {
            TreeNode parent = new TreeNode(0);
            TreeNode child = new TreeNode(1);
            parent.AddChild(child, counter);
            TreeNode nonChild = new TreeNode(2);

            parent.RemoveChild(child, counter);

            Assert.IsTrue(child.GetParent(counter) is null);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveChild(child, counter);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveChild(nonChild, counter);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveChild(parent, counter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveChild(null, counter);
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
            parent.AddChildren(children, counter);
            parent.AddChild(child, counter);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveChildren(null, counter);
            });

            parent.RemoveChildren(children, counter);

            foreach (TreeNode _child in children)
            {
                Assert.IsTrue(_child.GetParent(counter) is null);
                Assert.IsFalse(parent.HasChild(_child, counter));
            }

            Assert.IsTrue(parent.HasChild(child, counter));
            Assert.IsTrue(child.GetParent(counter) == parent);
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
            parent.AddChildren(children, counter);

            parent.RemoveAllChildren(counter);

            Assert.IsTrue(parent.NumberOfChildren(counter) == 0);
            Assert.IsFalse(parent.HasChild(children[0], counter));
            Assert.IsFalse(parent.HasChild(children[1], counter));
            Assert.IsTrue(children[0].GetParent(counter) is null);
            Assert.IsTrue(children[1].GetParent(counter) is null);
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

            Assert.AreEqual(node0.Degree(counter), 0);

            node0.AddChildren(new List<TreeNode>() { node1, node2, node3 }, counter);
            Assert.AreEqual(node0.Degree(counter), 3);
            Assert.AreEqual(node1.Degree(counter), 1);

            node0.RemoveChild(node1, counter);
            Assert.AreEqual(node0.Degree(counter), 2);
            Assert.AreEqual(node1.Degree(counter), 0);

            node4.AddChild(node0, counter);
            Assert.AreEqual(node0.Degree(counter), 3);

            node0.AddChild(node5, counter);
            node0.RemoveChild(node2, counter);
            Assert.AreEqual(node0.Degree(counter), 3);
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

            node0.AddChildren(new List<TreeNode>() { node1, node2, node3 }, counter);
            node2.AddChild(node4, counter);
            node4.AddChild(node5, counter);
            node3.AddChild(node6, counter);
            node3.RemoveChild(node6, counter);

            Assert.AreEqual(node0.DepthFromRoot(counter), 0);
            Assert.AreEqual(node1.DepthFromRoot(counter), 1);
            Assert.AreEqual(node2.DepthFromRoot(counter), 1);
            Assert.AreEqual(node3.DepthFromRoot(counter), 1);
            Assert.AreEqual(node4.DepthFromRoot(counter), 2);
            Assert.AreEqual(node5.DepthFromRoot(counter), 3);
            Assert.AreEqual(node6.DepthFromRoot(counter), 0);
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

            node0.AddChildren(new List<TreeNode>() { node1, node2 }, counter);
            node1.AddChild(node3, counter);
            node2.AddChild(node4, counter);
            node4.AddChild(node5, counter);
            node3.AddChild(node6, counter);
            node6.AddChild(node7, counter);
            node3.RemoveChild(node6, counter);

            Assert.AreEqual(node0.HeightOfSubtree(counter), 3);
            Assert.AreEqual(node1.HeightOfSubtree(counter), 1);
            Assert.AreEqual(node2.HeightOfSubtree(counter), 2);
            Assert.AreEqual(node3.HeightOfSubtree(counter), 0);
            Assert.AreEqual(node4.HeightOfSubtree(counter), 1);
            Assert.AreEqual(node5.HeightOfSubtree(counter), 0);
            Assert.AreEqual(node6.HeightOfSubtree(counter), 1);
            Assert.AreEqual(node7.HeightOfSubtree(counter), 0);
        }

        [TestMethod]
        public void TestAddNeighbour()
        {
            ITreeNode<TreeNode> node0 = new TreeNode(0);
            ITreeNode<TreeNode> node1 = new TreeNode(1);
            ITreeNode<TreeNode> node2 = new TreeNode(2);

            node0.AddNeighbour((TreeNode)node1, counter);
            node0.AddNeighbour((TreeNode)node2, counter);

            Assert.IsTrue(node0.HasChild((TreeNode)node1, counter));
            Assert.IsTrue(node0.HasChild((TreeNode)node2, counter));

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.AddNeighbour(null, counter);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                node0.AddNeighbour((TreeNode)node2, counter);
            });

            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                node0.AddNeighbour((TreeNode)node0, counter);
            });

            Assert.ThrowsException<AddParentAsChildException>(() =>
            {
                node1.AddNeighbour((TreeNode)node0, counter);
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

            parent.AddNeighbours(children, counter);
            foreach (TreeNode child in children)
            {
                Assert.IsTrue(parent.HasChild(child, counter));
                Assert.IsTrue(child.GetParent(counter) == parent);
            }

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.AddNeighbours(null, counter);
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
            parent.AddChildren(children, counter);
            uberParent.AddChild((TreeNode)parent, counter);

            parent.RemoveAllNeighbours(counter);

            Assert.IsTrue(parent.NumberOfChildren(counter) == 0);
            Assert.IsFalse(parent.HasChild(children[0], counter));
            Assert.IsFalse(parent.HasChild(children[1], counter));
            Assert.IsTrue(children[0].GetParent(counter) is null);
            Assert.IsTrue(children[1].GetParent(counter) is null);
            Assert.IsFalse(uberParent.HasChild((TreeNode)parent, counter));
            Assert.IsTrue(parent.GetParent(counter) is null);
        }

        [TestMethod]
        public void TestRemoveNeighbour()
        {
            ITreeNode<TreeNode> parent = new TreeNode(0);
            TreeNode child = new TreeNode(1);
            parent.AddChild(child, counter);
            TreeNode nonChild = new TreeNode(2);
            TreeNode uberParent = new TreeNode(3);
            uberParent.AddChild((TreeNode)parent, counter);
            Assert.IsTrue(parent.GetParent(counter) == uberParent);

            parent.RemoveNeighbour(child, counter);

            Assert.IsTrue(child.GetParent(counter) is null);

            parent.RemoveNeighbour(uberParent, counter);
            Assert.IsTrue(parent.GetParent(counter) is null);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(child, counter);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(nonChild, counter);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour((TreeNode)parent, counter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveNeighbour(null, counter);
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
            parent.AddChildren(children, counter);
            TreeNode child = new TreeNode(5);
            parent.AddChild(child, counter);
            TreeNode uberParent = new TreeNode(6);
            uberParent.AddChild((TreeNode)parent, counter);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveNeighbours(null, counter);
            });

            parent.RemoveNeighbours(new List<TreeNode>(children) { uberParent }, counter);

            foreach (TreeNode _child in children)
            {
                Assert.IsTrue(_child.GetParent(counter) is null);
                Assert.IsFalse(parent.HasChild(_child, counter));
            }

            Assert.IsFalse(uberParent.HasChild((TreeNode)parent, counter));
            Assert.IsTrue(parent.GetParent(counter) is null);

            Assert.IsTrue(parent.HasChild(child, counter));
            Assert.IsTrue(child.GetParent(counter) == parent);
        }

        [TestMethod]
        public void TestHasNeighbour()
        {
            TreeNode node0 = new TreeNode(0);
            ITreeNode<TreeNode> node1 = new TreeNode(1);
            node0.AddChild((TreeNode)node1, counter);
            TreeNode node2 = new TreeNode(2);
            node1.AddChild(node2, counter);
            Assert.IsTrue(node1.HasNeighbour(node2, counter));
            Assert.IsTrue(node1.HasNeighbour(node0, counter));

            Assert.IsFalse(((ITreeNode<TreeNode>)node0).HasNeighbour(node2, counter));
            Assert.IsFalse(((ITreeNode<TreeNode>)node2).HasNeighbour(node0, counter));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.HasNeighbour(null, counter);
            });
            Assert.AreEqual("node", a.ParamName);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            TreeNode t = new TreeNode(0);
            TreeNode n1 = new TreeNode(1);
            List<TreeNode> list = new List<TreeNode>();

            Assert.ThrowsException<ArgumentNullException>(() => t.AddChild(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChildren(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasChild(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveChild(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveChildren(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChild(n1, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChildren(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasChild(n1, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveChild(n1, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveChildren(list, null));
        }
    }
}
