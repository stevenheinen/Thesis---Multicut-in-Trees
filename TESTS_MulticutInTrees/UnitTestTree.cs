// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees;
using MulticutInTrees.Exceptions;
using System.Reflection;

namespace TESTS_MulticutInTrees
{
    [TestClass]
    public class UnitTestTree
    {
        [TestMethod]
        public void TestConstructorNode()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1, node0);
            TreeNode node2 = new TreeNode(2);

            Tree<TreeNode> tree = new Tree<TreeNode>(node1);

            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Root == node0);
            Assert.IsTrue(tree.HasNode(node0));
            Assert.IsTrue(tree.HasNode(node1));
            Assert.IsFalse(tree.HasNode(node2));
        }

        [TestMethod]
        public void TestConstructorNodeNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node = null;
                Tree<TreeNode> tree = new Tree<TreeNode>(node);
            });
        }

        [TestMethod]
        public void TestConstructorList()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1, node0);
            TreeNode node2 = new TreeNode(2);

            Tree<TreeNode> tree = new Tree<TreeNode>(new List<TreeNode>() { node0, node1 });

            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Root == node0);
            Assert.IsTrue(tree.HasNode(node0));
            Assert.IsTrue(tree.HasNode(node1));
            Assert.IsFalse(tree.HasNode(node2));
        }

        [TestMethod]
        public void TestConstructorListNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                List<TreeNode> nodes = null;
                Tree<TreeNode> tree = new Tree<TreeNode>(nodes);
            });
        }

        [TestMethod]
        public void TestConstructorListAndRoot()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1, node0);
            TreeNode node2 = new TreeNode(2);

            Tree<TreeNode> tree = new Tree<TreeNode>(new List<TreeNode>() { node0, node1 }, node0);

            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Root == node0);
            Assert.IsTrue(tree.HasNode(node0));
            Assert.IsTrue(tree.HasNode(node1));
            Assert.IsFalse(tree.HasNode(node2));
        }

        [TestMethod]
        public void TestConstructorListAndWrongRoot()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1, node0);
            TreeNode node2 = new TreeNode(2);

            Assert.ThrowsException<NoRootException>(() =>
            {
                Tree<TreeNode> tree = new Tree<TreeNode>(new List<TreeNode>() { node0, node1 }, node1);
            });

            Assert.ThrowsException<NoRootException>(() =>
            {
                Tree<TreeNode> tree = new Tree<TreeNode>(new List<TreeNode>() { node0, node1 }, node2);
            });
        }

        [TestMethod]
        public void TestConstructorListAndRootNull()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = null;

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Tree<TreeNode> tree = new Tree<TreeNode>(null, node0);
            });
            Assert.AreEqual(a.ParamName, "nodes");

            ArgumentNullException b = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Tree<TreeNode> tree = new Tree<TreeNode>(new List<TreeNode>() { node0 }, node1);
            });
            Assert.AreEqual(b.ParamName, "root");

            ArgumentNullException c = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Tree<TreeNode> tree = new Tree<TreeNode>(null, node1);
            });
            Assert.AreEqual(c.ParamName, "nodes");
        }

        [TestMethod]
        public void TestToString()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            node0.AddChild(node1);
            node1.AddChild(node2);
            node2.AddChild(node3);
            node3.AddChild(node4);

            Tree<TreeNode> tree = new Tree<TreeNode>(node0);

            Assert.AreEqual(tree.ToString(), $"Tree with depth 4 and 5 nodes: [TreeNode 0, TreeNode 1, TreeNode 2, TreeNode 3, TreeNode 4]");
        }

        [TestMethod]
        public void TestHasNode()
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

            Tree<TreeNode> tree1 = new Tree<TreeNode>(node3);

            Assert.IsTrue(tree1.HasNode(node0));
            Assert.IsTrue(tree1.HasNode(node1));
            Assert.IsTrue(tree1.HasNode(node2));
            Assert.IsTrue(tree1.HasNode(node3));
            Assert.IsTrue(tree1.HasNode(node4));
            Assert.IsFalse(tree1.HasNode(node5));
            Assert.IsFalse(tree1.HasNode(node6));

            Tree<TreeNode> tree2 = new Tree<TreeNode>(new List<TreeNode>() { node5, node6 });

            Assert.IsFalse(tree2.HasNode(node0));
            Assert.IsFalse(tree2.HasNode(node1));
            Assert.IsFalse(tree2.HasNode(node2));
            Assert.IsFalse(tree2.HasNode(node3));
            Assert.IsFalse(tree2.HasNode(node4));
            Assert.IsTrue(tree2.HasNode(node5));
            Assert.IsTrue(tree2.HasNode(node6));

            Tree<TreeNode> tree3 = new Tree<TreeNode>(new List<TreeNode>() { node5, node6 }, node5);

            Assert.IsFalse(tree3.HasNode(node0));
            Assert.IsFalse(tree3.HasNode(node1));
            Assert.IsFalse(tree3.HasNode(node2));
            Assert.IsFalse(tree3.HasNode(node3));
            Assert.IsFalse(tree3.HasNode(node4));
            Assert.IsTrue(tree3.HasNode(node5));
            Assert.IsTrue(tree3.HasNode(node6));
        }

        [TestMethod]
        public void TestHasNodeNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                TreeNode node0 = new TreeNode(0);
                TreeNode node1 = new TreeNode(1);
                TreeNode node2 = new TreeNode(2);
                TreeNode node3 = new TreeNode(3);
                TreeNode node4 = new TreeNode(4);
                TreeNode node5 = null;

                node0.AddChild(node1);
                node1.AddChild(node2);
                node1.AddChild(node3);
                node2.AddChild(node4);

                Tree<TreeNode> tree1 = new Tree<TreeNode>(node3);

                tree1.HasNode(node5);
            });
        }


        [TestMethod]
        public void TestDepth()
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
            node2.AddChild(node3);
            node3.AddChild(node4);

            Tree<TreeNode> tree = new Tree<TreeNode>(node0);

            Assert.AreEqual(tree.Depth, 4);
            Assert.AreEqual(tree.Depth, tree.Root.DepthOfSubtree);
        }

        [TestMethod]
        public void TestRoot()
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

            Tree<TreeNode> tree1 = new Tree<TreeNode>(node3);
            Assert.AreEqual(tree1.Root, node0);
            Assert.AreNotEqual(tree1.Root, node3);

            Tree<TreeNode> tree2 = new Tree<TreeNode>(new List<TreeNode>() { node5, node6 });
            Assert.AreEqual(tree2.Root, node5);

            Tree<TreeNode> tree3 = new Tree<TreeNode>(new List<TreeNode>() { node5, node6 }, node5);
            Assert.AreEqual(tree3.Root, node5);
        }

        [TestMethod]
        public void TestNumberOfNodes()
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

            Tree<TreeNode> tree1 = new Tree<TreeNode>(node3);
            Assert.AreEqual(tree1.NumberOfNodes, 5);

            Tree<TreeNode> tree2 = new Tree<TreeNode>(new List<TreeNode>() { node5, node6 });
            Assert.AreEqual(tree2.NumberOfNodes, 2);

            Tree<TreeNode> tree3 = new Tree<TreeNode>(new List<TreeNode>() { node5, node6 }, node5);
            Assert.AreEqual(tree3.NumberOfNodes, 2);
        }

        [TestMethod]
        public void TestUpdateNodesInTree()
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

            Tree<TreeNode> tree = new Tree<TreeNode>(node3);

            node1.AddChild(node2);
            node1.AddChild(node3);
            node2.AddChild(node4);

            System.Reflection.MethodInfo method = typeof(Tree<TreeNode>).GetMethod("UpdateNodesInTree", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(tree, new object[0]);

            Assert.AreEqual(tree.NodesInTree.Count, tree.NumberOfNodes);
            Assert.AreEqual(tree.Root, node0);
        }

        [TestMethod]
        public void TestFindRoot()
        {
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            node0.AddChild(node1);

            Tree<TreeNode> tree = new Tree<TreeNode>(node3);

            node1.AddChild(node2);
            node1.AddChild(node3);
            node2.AddChild(node4);

            MethodInfo method = typeof(Tree<TreeNode>).GetMethod("FindRoot", BindingFlags.NonPublic | BindingFlags.Instance);
            List<TreeNode> nodes = new List<TreeNode>() { node0, node1, node2, node3, node4 };
            TreeNode foundRoot = (TreeNode)method.Invoke(tree, new object[1] { nodes });

            Assert.AreEqual(foundRoot, node0);
        }

        [TestMethod]
        public void TestFindRootException()
        {
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                TreeNode node0 = new TreeNode(0);
                TreeNode node1 = new TreeNode(1);
                TreeNode node2 = new TreeNode(2);
                TreeNode node3 = new TreeNode(3);
                TreeNode node4 = new TreeNode(4);

                node0.AddChild(node1);

                Tree<TreeNode> tree = new Tree<TreeNode>(node3);

                node1.AddChild(node2);
                node1.AddChild(node3);
                node2.AddChild(node4);

                MethodInfo method = typeof(Tree<TreeNode>).GetMethod("FindRoot", BindingFlags.NonPublic | BindingFlags.Instance);
                List<TreeNode> nodes = new List<TreeNode>() { node1, node2, node3, node4 };
                TreeNode foundRoot = (TreeNode)method.Invoke(tree, new object[1] { nodes });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(NoRootException));
        }
    }
}
