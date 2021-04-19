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
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestConstructorID()
        {
            RootedTreeNode node = new RootedTreeNode(0);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestID()
        {
            RootedTreeNode node = new RootedTreeNode(3248);
            Assert.AreEqual(node.ID, (uint)3248);
        }

        [TestMethod]
        public void TestChildren()
        {
            RootedTreeNode node0 = new RootedTreeNode(0);
            Assert.IsNotNull(node0.Children(MockCounter));
        }

        [TestMethod]
        public void TestParent()
        {
            RootedTreeNode parent = new RootedTreeNode(0);
            RootedTreeNode child = new RootedTreeNode(1);
            RootedTreeNode node = new RootedTreeNode(2);
            parent.AddNeighbour(node, MockCounter);
            node.AddNeighbour(child, MockCounter);
            Assert.AreEqual(child.GetParent(MockCounter), node);
            Assert.AreEqual(node.GetParent(MockCounter), parent);
        }

        [TestMethod]
        public void TestIsRoot()
        {
            RootedTreeNode root = new RootedTreeNode(0);
            RootedTreeNode notRoot = new RootedTreeNode(1);
            root.AddNeighbour(notRoot, MockCounter);
            Assert.IsTrue(root.IsRoot(MockCounter));
            Assert.IsFalse(notRoot.IsRoot(MockCounter));
        }

        [TestMethod]
        public void TestHasChild()
        {
            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            node0.AddNeighbour(node1, MockCounter);
            Assert.IsTrue(node0.HasChild(node1, MockCounter));
            Assert.IsFalse(node1.HasChild(node0, MockCounter));

            RootedTreeNode node2 = new RootedTreeNode(2);
            node2.AddNeighbour(node0, MockCounter);
            Assert.IsTrue(node2.HasChild(node0, MockCounter));
            Assert.IsFalse(node0.HasChild(node2, MockCounter));

            Assert.IsFalse(node2.HasChild(node1, MockCounter));
            Assert.IsFalse(node1.HasChild(node2, MockCounter));
        }

        [TestMethod]
        public void TestHasNullChild()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RootedTreeNode node = new RootedTreeNode(0);
                node.HasChild(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestToString()
        {
            RootedTreeNode node = new RootedTreeNode(5664);
            Assert.AreEqual($"{node}", "RootedTreeNode 5664");
        }

        [TestMethod]
        public void TestAddChild()
        {
            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);

            node0.AddNeighbour(node1, MockCounter);
            node0.AddNeighbour(node2, MockCounter);

            Assert.IsTrue(node0.HasChild(node1, MockCounter));
            Assert.IsTrue(node0.HasChild(node2, MockCounter));

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.AddNeighbour(null, MockCounter);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                node0.AddNeighbour(node2, MockCounter);
            });

            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                node0.AddNeighbour(node0, MockCounter);
            });

            Assert.ThrowsException<AddParentAsChildException>(() =>
            {
                node1.AddNeighbour(node0, MockCounter);
            });
        }

        [TestMethod]
        public void TestAddChildren()
        {
            RootedTreeNode parent = new RootedTreeNode(0);
            List<RootedTreeNode> children = new List<RootedTreeNode>()
            {
                new RootedTreeNode(1),
                new RootedTreeNode(2),
                new RootedTreeNode(3),
                new RootedTreeNode(4),
            };

            parent.AddNeighbours(children, MockCounter);
            foreach (RootedTreeNode child in children)
            {
                Assert.IsTrue(parent.HasChild(child, MockCounter));
                Assert.IsTrue(child.GetParent(MockCounter) == parent);
            }

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.AddNeighbours(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveChild()
        {
            RootedTreeNode parent = new RootedTreeNode(0);
            RootedTreeNode child = new RootedTreeNode(1);
            parent.AddNeighbour(child, MockCounter);
            RootedTreeNode nonChild = new RootedTreeNode(2);

            parent.RemoveNeighbour(child, MockCounter);

            Assert.IsTrue(child.GetParent(MockCounter) is null);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(child, MockCounter);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(nonChild, MockCounter);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(parent, MockCounter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveNeighbour(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveChildren()
        {
            List<RootedTreeNode> children = new List<RootedTreeNode>()
            {
                new RootedTreeNode(1),
                new RootedTreeNode(2),
                new RootedTreeNode(3),
                new RootedTreeNode(4),
            };
            RootedTreeNode parent = new RootedTreeNode(0);
            RootedTreeNode child = new RootedTreeNode(5);
            parent.AddNeighbours(children, MockCounter);
            parent.AddNeighbour(child, MockCounter);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveNeighbours(null, MockCounter);
            });

            parent.RemoveNeighbours(children, MockCounter);

            foreach (RootedTreeNode _child in children)
            {
                Assert.IsTrue(_child.GetParent(MockCounter) is null);
                Assert.IsFalse(parent.HasChild(_child, MockCounter));
            }

            Assert.IsTrue(parent.HasChild(child, MockCounter));
            Assert.IsTrue(child.GetParent(MockCounter) == parent);
        }

        [TestMethod]
        public void TestRemoveAllChildren()
        {
            List<RootedTreeNode> children = new List<RootedTreeNode>()
            {
                new RootedTreeNode(1),
                new RootedTreeNode(2),
            };
            RootedTreeNode parent = new RootedTreeNode(0);
            parent.AddNeighbours(children, MockCounter);

            parent.RemoveAllNeighbours(MockCounter);

            Assert.IsTrue(parent.NumberOfChildren(MockCounter) == 0);
            Assert.IsFalse(parent.HasChild(children[0], MockCounter));
            Assert.IsFalse(parent.HasChild(children[1], MockCounter));
            Assert.IsTrue(children[0].GetParent(MockCounter) is null);
            Assert.IsTrue(children[1].GetParent(MockCounter) is null);
        }

        [TestMethod]
        public void TestDegree()
        {
            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);
            RootedTreeNode node3 = new RootedTreeNode(3);
            RootedTreeNode node4 = new RootedTreeNode(4);
            RootedTreeNode node5 = new RootedTreeNode(5);

            Assert.AreEqual(node0.Degree(MockCounter), 0);

            node0.AddNeighbours(new List<RootedTreeNode>() { node1, node2, node3 }, MockCounter);
            Assert.AreEqual(node0.Degree(MockCounter), 3);
            Assert.AreEqual(node1.Degree(MockCounter), 1);

            node0.RemoveNeighbour(node1, MockCounter);
            Assert.AreEqual(node0.Degree(MockCounter), 2);
            Assert.AreEqual(node1.Degree(MockCounter), 0);

            node4.AddNeighbour(node0, MockCounter);
            Assert.AreEqual(node0.Degree(MockCounter), 3);

            node0.AddNeighbour(node5, MockCounter);
            node0.RemoveNeighbour(node2, MockCounter);
            Assert.AreEqual(node0.Degree(MockCounter), 3);
        }

        [TestMethod]
        public void TestDepthFromRoot()
        {
            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);
            RootedTreeNode node3 = new RootedTreeNode(3);
            RootedTreeNode node4 = new RootedTreeNode(4);
            RootedTreeNode node5 = new RootedTreeNode(5);
            RootedTreeNode node6 = new RootedTreeNode(6);

            node0.AddNeighbours(new List<RootedTreeNode>() { node1, node2, node3 }, MockCounter);
            node2.AddNeighbour(node4, MockCounter);
            node4.AddNeighbour(node5, MockCounter);
            node3.AddNeighbour(node6, MockCounter);
            node3.RemoveNeighbour(node6, MockCounter);

            Assert.AreEqual(node0.DepthFromRoot(MockCounter), 0);
            Assert.AreEqual(node1.DepthFromRoot(MockCounter), 1);
            Assert.AreEqual(node2.DepthFromRoot(MockCounter), 1);
            Assert.AreEqual(node3.DepthFromRoot(MockCounter), 1);
            Assert.AreEqual(node4.DepthFromRoot(MockCounter), 2);
            Assert.AreEqual(node5.DepthFromRoot(MockCounter), 3);
            Assert.AreEqual(node6.DepthFromRoot(MockCounter), 0);
        }

        [TestMethod]
        public void TestDepthOfSubtree()
        {
            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);
            RootedTreeNode node3 = new RootedTreeNode(3);
            RootedTreeNode node4 = new RootedTreeNode(4);
            RootedTreeNode node5 = new RootedTreeNode(5);
            RootedTreeNode node6 = new RootedTreeNode(6);
            RootedTreeNode node7 = new RootedTreeNode(7);

            node0.AddNeighbours(new List<RootedTreeNode>() { node1, node2 }, MockCounter);
            node1.AddNeighbour(node3, MockCounter);
            node2.AddNeighbour(node4, MockCounter);
            node4.AddNeighbour(node5, MockCounter);
            node3.AddNeighbour(node6, MockCounter);
            node6.AddNeighbour(node7, MockCounter);
            node3.RemoveNeighbour(node6, MockCounter);

            Assert.AreEqual(node0.HeightOfSubtree(MockCounter), 3);
            Assert.AreEqual(node1.HeightOfSubtree(MockCounter), 1);
            Assert.AreEqual(node2.HeightOfSubtree(MockCounter), 2);
            Assert.AreEqual(node3.HeightOfSubtree(MockCounter), 0);
            Assert.AreEqual(node4.HeightOfSubtree(MockCounter), 1);
            Assert.AreEqual(node5.HeightOfSubtree(MockCounter), 0);
            Assert.AreEqual(node6.HeightOfSubtree(MockCounter), 1);
            Assert.AreEqual(node7.HeightOfSubtree(MockCounter), 0);
        }

        [TestMethod]
        public void TestAddNeighbour()
        {
            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);

            node0.AddNeighbour(node1, MockCounter);
            node0.AddNeighbour(node2, MockCounter);

            Assert.IsTrue(node0.HasChild(node1, MockCounter));
            Assert.IsTrue(node0.HasChild(node2, MockCounter));

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.AddNeighbour(null, MockCounter);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                node0.AddNeighbour(node2, MockCounter);
            });

            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                node0.AddNeighbour(node0, MockCounter);
            });

            Assert.ThrowsException<AddParentAsChildException>(() =>
            {
                node1.AddNeighbour(node0, MockCounter);
            });
        }

        [TestMethod]
        public void TestAddNeighbours()
        {
            RootedTreeNode parent = new RootedTreeNode(0);
            List<RootedTreeNode> children = new List<RootedTreeNode>()
            {
                new RootedTreeNode(1),
                new RootedTreeNode(2),
                new RootedTreeNode(3),
                new RootedTreeNode(4),
            };

            parent.AddNeighbours(children, MockCounter);
            foreach (RootedTreeNode child in children)
            {
                Assert.IsTrue(parent.HasChild(child, MockCounter));
                Assert.IsTrue(child.GetParent(MockCounter) == parent);
            }

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.AddNeighbours(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveAllNeighbours()
        {
            RootedTreeNode uberParent = new RootedTreeNode(3);
            List<RootedTreeNode> children = new List<RootedTreeNode>()
            {
                new RootedTreeNode(1),
                new RootedTreeNode(2),
            };
            RootedTreeNode parent = new RootedTreeNode(0);
            parent.AddNeighbours(children, MockCounter);
            uberParent.AddNeighbour(parent, MockCounter);

            parent.RemoveAllNeighbours(MockCounter);

            Assert.IsTrue(parent.NumberOfChildren(MockCounter) == 0);
            Assert.IsFalse(parent.HasChild(children[0], MockCounter));
            Assert.IsFalse(parent.HasChild(children[1], MockCounter));
            Assert.IsTrue(children[0].GetParent(MockCounter) is null);
            Assert.IsTrue(children[1].GetParent(MockCounter) is null);
            Assert.IsFalse(uberParent.HasChild(parent, MockCounter));
            Assert.IsTrue(parent.GetParent(MockCounter) is null);
        }

        [TestMethod]
        public void TestRemoveNeighbour()
        {
            RootedTreeNode parent = new RootedTreeNode(0);
            RootedTreeNode child = new RootedTreeNode(1);
            parent.AddNeighbour(child, MockCounter);
            RootedTreeNode nonChild = new RootedTreeNode(2);
            RootedTreeNode uberParent = new RootedTreeNode(3);
            uberParent.AddNeighbour(parent, MockCounter);
            Assert.IsTrue(parent.GetParent(MockCounter) == uberParent);

            parent.RemoveNeighbour(child, MockCounter);

            Assert.IsTrue(child.GetParent(MockCounter) is null);

            parent.RemoveNeighbour(uberParent, MockCounter);
            Assert.IsTrue(parent.GetParent(MockCounter) is null);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(child, MockCounter);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(nonChild, MockCounter);
            });

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                parent.RemoveNeighbour(parent, MockCounter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveNeighbour(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveNeighbours()
        {
            List<RootedTreeNode> children = new List<RootedTreeNode>()
            {
                new RootedTreeNode(1),
                new RootedTreeNode(2),
                new RootedTreeNode(3),
                new RootedTreeNode(4),
            };
            RootedTreeNode parent = new RootedTreeNode(0);
            parent.AddNeighbours(children, MockCounter);
            RootedTreeNode child = new RootedTreeNode(5);
            parent.AddNeighbour(child, MockCounter);
            RootedTreeNode uberParent = new RootedTreeNode(6);
            uberParent.AddNeighbour((RootedTreeNode)parent, MockCounter);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                parent.RemoveNeighbours(null, MockCounter);
            });

            parent.RemoveNeighbours(new List<RootedTreeNode>(children) { uberParent }, MockCounter);

            foreach (RootedTreeNode _child in children)
            {
                Assert.IsTrue(_child.GetParent(MockCounter) is null);
                Assert.IsFalse(parent.HasChild(_child, MockCounter));
            }

            Assert.IsFalse(uberParent.HasChild((RootedTreeNode)parent, MockCounter));
            Assert.IsTrue(parent.GetParent(MockCounter) is null);

            Assert.IsTrue(parent.HasChild(child, MockCounter));
            Assert.IsTrue(child.GetParent(MockCounter) == parent);
        }

        [TestMethod]
        public void TestHasNeighbour()
        {
            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            node0.AddNeighbour((RootedTreeNode)node1, MockCounter);
            RootedTreeNode node2 = new RootedTreeNode(2);
            node1.AddNeighbour(node2, MockCounter);
            Assert.IsTrue(node1.HasNeighbour(node2, MockCounter));
            Assert.IsTrue(node1.HasNeighbour(node0, MockCounter));

            Assert.IsFalse(node0.HasNeighbour(node2, MockCounter));
            Assert.IsFalse(node2.HasNeighbour(node0, MockCounter));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                node1.HasNeighbour(null, MockCounter);
            });
            Assert.AreEqual("node", a.ParamName);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            RootedTreeNode t = new RootedTreeNode(0);
            RootedTreeNode n1 = new RootedTreeNode(1);
            List<RootedTreeNode> list = new List<RootedTreeNode>();

            Assert.ThrowsException<ArgumentNullException>(() => t.AddNeighbour(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddNeighbours(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasChild(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNeighbour(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNeighbours(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddNeighbour(n1, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddNeighbours(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasChild(n1, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNeighbour(n1, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNeighbours(list, null));
        }
    }
}
