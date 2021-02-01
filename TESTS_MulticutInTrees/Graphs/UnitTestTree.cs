// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace TESTS_MulticutInTrees.Graphs
{
    [TestClass]
    public class UnitTestTree
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            Assert.IsNotNull(tree);

            Assert.IsNotNull(tree.Edges);
            Assert.IsNotNull(tree.Nodes);
        }

        [TestMethod]
        public void TestAddRoot()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node = new TreeNode(0);
            tree.AddRoot(node);
            Assert.AreEqual(node, tree.Root);

            TreeNode node1 = new TreeNode(1);
            tree.AddRoot(node1);
            Assert.AreEqual(node1, tree.Root);
            Assert.AreEqual(node1, node.Parent);

            TreeNode node2 = new TreeNode(2);
            tree.AddChild(node1, node2);
            TreeNode node3 = new TreeNode(3);
            tree.AddRoot(node3);

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddRoot(null);
            });
            Assert.AreEqual("newRoot", a.ParamName);
        }

        [TestMethod]
        public void TestAddChildrenToParent()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 });
            tree.AddChild(node2, node6);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9, node10 });

            MethodInfo method = typeof(Tree<TreeNode>).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(tree, new object[1] { node3 });

            Assert.AreEqual(7, node0.Children.Count);
            Assert.AreEqual(0, node3.Children.Count);
            Assert.AreEqual(node0, node9.Parent);
            Assert.IsTrue(node0.HasChild(node7));

            tree.RemoveNode(node10);
            tree.AddRoot(node10);
            method.Invoke(tree, new object[1] { node10 });

            Tree<TreeNode> tree2 = new Tree<TreeNode>();
            TreeNode node11 = new TreeNode(11);
            tree2.AddRoot(node11);
            method.Invoke(tree2, new object[1] { node11 });
        }

        [TestMethod]
        public void TestAddChildrenToParentNull()
        {
            TargetInvocationException a = Assert.ThrowsException<TargetInvocationException>(() => 
            {
                Tree<TreeNode> tree = new Tree<TreeNode>();
                MethodInfo method = typeof(Tree<TreeNode>).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(tree, new object[1] { null });
            });
            Assert.IsInstanceOfType(a.InnerException, typeof(ArgumentNullException));
            Assert.AreEqual("node", ((ArgumentNullException)a.InnerException).ParamName);
        }

        [TestMethod]
        public void TestHasNode()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);

            tree.AddRoot(node0);

            Assert.IsTrue(tree.HasNode(node0));

            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 });

            Assert.IsTrue(tree.HasNode(node5));
            Assert.IsFalse(tree.HasNode(node9));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.HasNode(null);
            });
            Assert.AreEqual("node", a.ParamName);
        }

        [TestMethod]
        public void TestHasEdge()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);

            tree.AddRoot(node1);
            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node2, node3 });
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 });

            Assert.IsTrue(tree.HasEdge(node0, node1));
            Assert.IsTrue(tree.HasEdge(node1, node5));
            Assert.IsFalse(tree.HasEdge(node0, node4));

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.HasEdge(node0, null);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.HasEdge(null, node10);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.HasEdge(node1, node9);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.HasEdge(node7, node5);
            });
        }

        [TestMethod]
        public void TestHasEdgeTuple()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);

            tree.AddRoot(node1);
            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node2, node3 });
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 });

            Assert.IsTrue(tree.HasEdge((node0, node1)));
            Assert.IsTrue(tree.HasEdge((node1, node5)));
            Assert.IsFalse(tree.HasEdge((node0, node4)));

            Assert.IsTrue(tree.HasEdge((node1, node0)));
            Assert.IsTrue(tree.HasEdge((node5, node1)));
            Assert.IsFalse(tree.HasEdge((node4, node0)));

            Assert.ThrowsException<ArgumentNullException>(() => tree.HasEdge((node0, null)));
            Assert.ThrowsException<ArgumentNullException>(() => tree.HasEdge((null, node10)));
            Assert.ThrowsException<NotInGraphException>(() => tree.HasEdge((node1, node9)));
            Assert.ThrowsException<NotInGraphException>(() => tree.HasEdge((node7, node5)));
            Assert.ThrowsException<NotInGraphException>(() => tree.HasEdge((node9, node1)));
            Assert.ThrowsException<NotInGraphException>(() => tree.HasEdge((node5, node7)));
        }


        [TestMethod]
        public void TestAddChild()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            Assert.AreEqual(node0, node1.Parent);
            Assert.IsTrue(tree.HasNode(node1));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddChild(null, node3);
            });
            Assert.AreEqual("parent", a.ParamName);

            ArgumentNullException b = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddChild(node0, null);
            });
            Assert.AreEqual("child", b.ParamName);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.AddChild(node8, node1);
            });

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                tree.AddChild(node0, node1);
            });
        }

        [TestMethod]
        public void TestAddChildren()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 });
            tree.AddChild(node2, node6);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9, node10 });

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddChildren(null, new List<TreeNode>() { node1, node4, node8 });
            });
            Assert.AreEqual("parent", a.ParamName);

            ArgumentNullException b = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddChildren(node5, null);
            });
            Assert.AreEqual("children", b.ParamName);

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                tree.AddChildren(node1, new List<TreeNode> { node5 });
            });
        }

        [TestMethod]
        public void TestRemoveNode()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 });
            tree.AddChild(node2, node6);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9, node10 });

            Assert.AreEqual(11, tree.NumberOfNodes);
            Assert.AreEqual(10, tree.NumberOfEdges);

            tree.RemoveNode(node6);
            Assert.AreEqual(10, tree.NumberOfNodes);
            Assert.AreEqual(9, tree.NumberOfEdges);

            tree.RemoveNode(node3);
            Assert.AreEqual(9, tree.NumberOfNodes);
            Assert.AreEqual(8, tree.NumberOfEdges);

            Assert.IsFalse(node0.HasChild(node3));
            Assert.AreEqual(6, node0.Children.Count);

            tree.AddRoot(node3);
            tree.RemoveNode(node3);

            Assert.ThrowsException<MultipleRootsException>(() =>
            {
                tree.RemoveNode(node0);
            });

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.RemoveNode(null);
            });
            Assert.AreEqual("node", a.ParamName);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.RemoveNode(node6);
            });
        }

        [TestMethod]
        public void TestRemoveNodes()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 });
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 });
            tree.AddChild(node2, node6);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9, node10 });

            tree.RemoveNodes(new List<TreeNode>() { node1, node3, node9 });
            Assert.AreEqual(8, tree.NumberOfNodes);
            Assert.AreEqual(7, tree.NumberOfEdges);
            Assert.IsTrue(node0.HasChild(node7));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.RemoveNodes(null);
            });
            Assert.AreEqual("nodes", a.ParamName);
        }

        [TestMethod]
        public void TestUpdateRoot()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            MethodInfo method = typeof(Tree<TreeNode>).GetMethod("FindRoot", BindingFlags.NonPublic | BindingFlags.Instance);

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                method.Invoke(tree, new object[0]);
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(NoRootException));

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            tree.AddRoot(node0);
            tree.AddChild(node0, node1);

            TreeNode foundRoot = (TreeNode)method.Invoke(tree, new object[0]);
            Assert.AreEqual(node0, foundRoot);

            PropertyInfo nodesProperty = typeof(Tree<TreeNode>).GetProperty("InternalNodes", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException u = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo m = nodesProperty.GetSetMethod(true);
                TreeNode node2 = new TreeNode(2);
                TreeNode node3 = new TreeNode(3);
                m.Invoke(tree, new object[1] { new List<TreeNode>() { node2, node3 } });
                method.Invoke(tree, new object[0]);
            });
            Assert.IsInstanceOfType(u.InnerException, typeof(MultipleRootsException));
        }

        [TestMethod]
        public void TestArgumentNull()
        {
            Tree<TreeNode> t = new Tree<TreeNode>();
            TreeNode n = new TreeNode(0);

            Assert.ThrowsException<ArgumentNullException>(() => t.AddChild(null, n));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChild(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChildren(null, new List<TreeNode>() { n }));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChildren(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddRoot(null));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasEdge(null, n));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasEdge(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasNode(null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNode(null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNodes(null));

            MethodInfo method = typeof(Tree<TreeNode>).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException e = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                method.Invoke(t, new object[1] { null });
            });
            Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestValidTree()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            Assert.IsFalse(tree.IsValid());

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node0, node2);

            Assert.IsTrue(tree.IsValid());

            PropertyInfo property = typeof(TreeNode).GetProperty("Parent");
            property.GetSetMethod(true).Invoke(node1, new object[] { null });

            Assert.IsFalse(tree.IsValid());

            property.GetSetMethod(true).Invoke(node1, new object[] { node0 });

            Assert.IsTrue(tree.IsValid());

            TreeNode node3 = new TreeNode(3);
            property.GetSetMethod(true).Invoke(node3, new object[] { node2 });

            PropertyInfo nodeproperty = typeof(Tree<TreeNode>).GetProperty("InternalNodes", BindingFlags.NonPublic | BindingFlags.Instance);
            nodeproperty.GetSetMethod(true).Invoke(tree, new object[] { new List<TreeNode>() { node0, node1, node2, node3 } });

            Assert.IsFalse(tree.IsValid());

            node0.AddChild(node3);
            property.GetSetMethod(true).Invoke(node3, new object[] { node2 });

            Assert.IsFalse(tree.IsValid());
        }
    }
}
