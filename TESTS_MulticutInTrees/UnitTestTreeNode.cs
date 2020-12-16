// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees;
using MulticutInTrees.Exceptions;

namespace TESTS_MulticutInTrees
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
        public void TestConstructorParent()
        {
            TreeNode parent = new TreeNode(0);
            TreeNode node = new TreeNode(1, parent);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestConstructorNullParent()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode parent = null;
                TreeNode node = new TreeNode(0, parent);
            });
        }

        [TestMethod]
        public void TestConstructorChildren()
        {
            TreeNode child1 = new TreeNode(0);
            TreeNode child2 = new TreeNode(1);
            TreeNode node = new TreeNode(2, new List<TreeNode>() { child1, child2 });
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestConstructorNullChildren()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                List<TreeNode> children = null;
                TreeNode node = new TreeNode(0, children);
            });
        }

        [TestMethod]
        public void TestConstructorParentChildren()
        {
            TreeNode parent = new TreeNode(0);
            TreeNode child1 = new TreeNode(1);
            TreeNode child2 = new TreeNode(2);
            TreeNode node = new TreeNode(3, parent, new List<TreeNode>() { child1, child2 });
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestConstructorNullParentChildren()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode parent = null;
                TreeNode child1 = new TreeNode(1);
                TreeNode child2 = new TreeNode(2);
                TreeNode node = new TreeNode(0, parent, new List<TreeNode>() { child1, child2 });
            });
            Assert.AreEqual(a.ParamName, "parent");
        }

        [TestMethod]
        public void TestConstructorParentNullChildren()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                List<TreeNode> children = null;
                TreeNode parent = new TreeNode(0);
                TreeNode node = new TreeNode(1, parent, children);
            });
            Assert.AreEqual(a.ParamName, "children");
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

            TreeNode node1 = new TreeNode(1, node0);
            Assert.IsNotNull(node1.Children);

            TreeNode node2 = new TreeNode(2, new List<TreeNode>() { node0 });
            Assert.IsNotNull(node2.Children);

            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4, node1, new List<TreeNode>() { node3 });
            Assert.IsNotNull(node4.Children);
        }

        [TestMethod]
        public void TestParent()
        {
            TreeNode parent = new TreeNode(0);
            TreeNode child = new TreeNode(1);
            TreeNode node = new TreeNode(2, parent, new List<TreeNode>() { child });
            Assert.AreEqual(child.Parent, node);
            Assert.AreEqual(node.Parent, parent);
        }

        [TestMethod]
        public void TestCompareToNull()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = new TreeNode(0);
                bool b = node == null;
            });
            Assert.AreEqual(a.ParamName, "rhs");
        }

        [TestMethod]
        public void TestCompareToNullLeft()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = new TreeNode(0);
                bool b = null == node;
            });
            Assert.AreEqual(a.ParamName, "lhs");
        }

        [TestMethod]
        public void TestCompareInequalToNull()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = new TreeNode(0);
                bool b = node != null;
            });
            Assert.AreEqual(a.ParamName, "rhs");
        }

        [TestMethod]
        public void TestCompareInequalToNullLeft()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = new TreeNode(0);
                bool b = null != node;
            });
            Assert.AreEqual(a.ParamName, "lhs");
        }

        [TestMethod]
        public void TestEqualsObjectNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = new TreeNode(0);
                bool b = node.Equals((object)null);
            });
        }

        [TestMethod]
        public void TestEqualsTreeNodeNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = new TreeNode(0);
                bool b = node.Equals(null);
            });
        }

        [TestMethod]
        public void TestCompareToItselfTrue()
        {
            TreeNode node0 = new TreeNode(0);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(node0 == node0);
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void TestCompareToItselfFalse()
        {
            TreeNode node0 = new TreeNode(0);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsFalse(node0 != node0);
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void TestEqualToOther()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            Assert.IsFalse(node0.Equals(node1));
        }

        [TestMethod]
        public void TestEqualToItself()
        {
            TreeNode node0 = new TreeNode(0);
            Assert.IsTrue(node0.Equals(node0));
        }

        [TestMethod]
        public void TestEqualsOperatorToOther()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            Assert.IsFalse(node0 == node1);
        }

        [TestMethod]
        public void TestNotEqualsOperatorToOtherTrue()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            Assert.IsTrue(node0 != node1);
        }

        [TestMethod]
        public void TestGenericEqualsDifferentTypes()
        {
            TreeNode node = new TreeNode(0);

            Assert.ThrowsException<IncompatibleTypesException>(() =>
            {
                int i = 0;
                bool b = node.Equals(i);
            });

            Assert.ThrowsException<IncompatibleTypesException>(() =>
            {
                string s = "0";
                bool b = node.Equals(s);
            });

            Assert.ThrowsException<IncompatibleTypesException>(() =>
            {
                object obj = new object();
                bool b = node.Equals(obj);
            });

            Assert.ThrowsException<IncompatibleTypesException>(() =>
            {
                char c = '0';
                bool b = node.Equals(c);
            });
        }

        [TestMethod]
        public void TestGenericEqualsCorrect()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);

            Assert.IsTrue(node0.Equals((object)node0));
            Assert.IsFalse(node0.Equals((object)node1));
        }

        [TestMethod]
        public void TestIsRoot()
        {
            TreeNode root = new TreeNode(0);
            TreeNode notRoot = new TreeNode(1, root);
            Assert.IsTrue(root.IsRoot());
            Assert.IsFalse(notRoot.IsRoot());
        }

        [TestMethod]
        public void TestHasChild()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1, node0);
            Assert.IsTrue(node0.HasChild(node1));
            Assert.IsFalse(node1.HasChild(node0));

            TreeNode node2 = new TreeNode(2, new List<TreeNode>() { node0 });
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
            TreeNode child = new TreeNode(1, parent);
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
            TreeNode parent = new TreeNode(0, children);
            TreeNode child = new TreeNode(5, parent);

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
            TreeNode parent = new TreeNode(0, children);

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

            Assert.AreEqual(node0.DepthOfSubtree, 3);
            Assert.AreEqual(node1.DepthOfSubtree, 1);
            Assert.AreEqual(node2.DepthOfSubtree, 2);
            Assert.AreEqual(node3.DepthOfSubtree, 0);
            Assert.AreEqual(node4.DepthOfSubtree, 1);
            Assert.AreEqual(node5.DepthOfSubtree, 0);
            Assert.AreEqual(node6.DepthOfSubtree, 1);
            Assert.AreEqual(node7.DepthOfSubtree, 0);
        }

    }
}
