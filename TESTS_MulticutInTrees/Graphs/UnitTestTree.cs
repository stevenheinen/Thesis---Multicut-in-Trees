// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace TESTS_MulticutInTrees.Graphs
{
    [TestClass]
    public class UnitTestTree
    {
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            Assert.IsNotNull(tree);

            Assert.IsNotNull(tree.Edges(counter));
            Assert.IsNotNull(tree.Nodes(counter));
        }

        [TestMethod]
        public void TestAddRoot()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node = new TreeNode(0);
            tree.AddRoot(node, counter);
            Assert.AreEqual(node, tree.GetRoot(counter));

            TreeNode node1 = new TreeNode(1);
            tree.AddRoot(node1, counter);
            Assert.AreEqual(node1, tree.GetRoot(counter));
            Assert.AreEqual(node1, node.GetParent(counter));

            TreeNode node2 = new TreeNode(2);
            tree.AddChild(node1, node2, counter);
            TreeNode node3 = new TreeNode(3);
            tree.AddRoot(node3, counter);

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddRoot(null, counter);
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 }, counter);
            tree.AddChild(node2, node6, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9, node10 }, counter);

            MethodInfo method = typeof(Tree<TreeNode>).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(tree, new object[] { node3, counter });

            Assert.AreEqual(7, node0.NumberOfChildren(counter));
            Assert.AreEqual(0, node3.NumberOfChildren(counter));
            Assert.AreEqual(node0, node9.GetParent(counter));
            Assert.IsTrue(node0.HasChild(node7, counter));

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                tree.RemoveNode(node10, counter);
                tree.AddRoot(node10, counter);
                method.Invoke(tree, new object[] { node10, counter });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(NotSupportedException));

            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                Tree<TreeNode> tree2 = new Tree<TreeNode>();
                TreeNode node11 = new TreeNode(11);
                tree2.AddRoot(node11, counter);
                method.Invoke(tree2, new object[] { node11, counter });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(NotSupportedException));
        }

        [TestMethod]
        public void TestAddChildrenToParentNull()
        {
            TargetInvocationException a = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                Tree<TreeNode> tree = new Tree<TreeNode>();
                MethodInfo method = typeof(Tree<TreeNode>).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(tree, new object[] { null, counter });
            });
            Assert.IsInstanceOfType(a.InnerException, typeof(ArgumentNullException));
            Assert.AreEqual("node", ((ArgumentNullException)a.InnerException).ParamName);

            a = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                Tree<TreeNode> tree = new Tree<TreeNode>();
                TreeNode node = new TreeNode(0);
                MethodInfo method = typeof(Tree<TreeNode>).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(tree, new object[] { node, null });
            });
            Assert.IsInstanceOfType(a.InnerException, typeof(ArgumentNullException));
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

            tree.AddRoot(node0, counter);

            Assert.IsTrue(tree.HasNode(node0, counter));

            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 }, counter);

            Assert.IsTrue(tree.HasNode(node5, counter));
            Assert.IsFalse(tree.HasNode(node9, counter));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.HasNode(null, counter);
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

            tree.AddRoot(node1, counter);
            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node2, node3 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 }, counter);

            Assert.IsTrue(tree.HasEdge(node0, node1, counter));
            Assert.IsTrue(tree.HasEdge(node1, node5, counter));
            Assert.IsFalse(tree.HasEdge(node0, node4, counter));

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.HasEdge(node0, null, counter);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.HasEdge(null, node10, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.HasEdge(node1, node9, counter);
            });

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.HasEdge(node7, node5, counter);
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

            tree.AddRoot(node1, counter);
            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node2, node3 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 }, counter);

            Assert.IsTrue(tree.HasEdge((node0, node1), counter));
            Assert.IsTrue(tree.HasEdge((node1, node5), counter));
            Assert.IsFalse(tree.HasEdge((node0, node4), counter));

            Assert.IsTrue(tree.HasEdge((node1, node0), counter));
            Assert.IsTrue(tree.HasEdge((node5, node1), counter));
            Assert.IsFalse(tree.HasEdge((node4, node0), counter));

            Assert.ThrowsException<ArgumentNullException>(() => tree.HasEdge((node0, null), counter));
            Assert.ThrowsException<ArgumentNullException>(() => tree.HasEdge((null, node10), counter));
            Assert.ThrowsException<NotInGraphException>(() => tree.HasEdge((node1, node9), counter));
            Assert.ThrowsException<NotInGraphException>(() => tree.HasEdge((node7, node5), counter));
            Assert.ThrowsException<NotInGraphException>(() => tree.HasEdge((node9, node1), counter));
            Assert.ThrowsException<NotInGraphException>(() => tree.HasEdge((node5, node7), counter));
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            Assert.AreEqual(node0, node1.GetParent(counter));
            Assert.IsTrue(tree.HasNode(node1, counter));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddChild(null, node3, counter);
            });
            Assert.AreEqual("parent", a.ParamName);

            ArgumentNullException b = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddChild(node0, null, counter);
            });
            Assert.AreEqual("child", b.ParamName);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.AddChild(node8, node1, counter);
            });

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                tree.AddChild(node0, node1, counter);
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 }, counter);
            tree.AddChild(node2, node6, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9, node10 }, counter);

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddChildren(null, new List<TreeNode>() { node1, node4, node8 }, counter);
            });
            Assert.AreEqual("parent", a.ParamName);

            ArgumentNullException b = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.AddChildren(node5, null, counter);
            });
            Assert.AreEqual("children", b.ParamName);

            Assert.ThrowsException<AlreadyInGraphException>(() =>
            {
                tree.AddChildren(node1, new List<TreeNode> { node5 }, counter);
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 }, counter);
            tree.AddChild(node2, node6, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9, node10 }, counter);

            Assert.AreEqual(11, tree.NumberOfNodes(counter));
            Assert.AreEqual(10, tree.NumberOfEdges(counter));

            tree.RemoveNode(node6, counter);
            Assert.AreEqual(10, tree.NumberOfNodes(counter));
            Assert.AreEqual(9, tree.NumberOfEdges(counter));

            tree.RemoveNode(node3, counter);
            Assert.AreEqual(9, tree.NumberOfNodes(counter));
            Assert.AreEqual(8, tree.NumberOfEdges(counter));

            Assert.IsFalse(node0.HasChild(node3, counter));
            Assert.AreEqual(6, node0.NumberOfChildren(counter));

            tree.AddRoot(node3, counter);
            tree.RemoveNode(node3, counter);

            Assert.ThrowsException<MultipleRootsException>(() =>
            {
                tree.RemoveNode(node0, counter);
            });

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.RemoveNode(null, counter);
            });
            Assert.AreEqual("node", a.ParamName);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.RemoveNode(node6, counter);
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node4, node5 }, counter);
            tree.AddChild(node2, node6, counter);
            tree.AddChildren(node3, new List<TreeNode>() { node7, node8, node9, node10 }, counter);

            tree.RemoveNodes(new List<TreeNode>() { node1, node3, node9 }, counter);
            Assert.AreEqual(8, tree.NumberOfNodes(counter));
            Assert.AreEqual(7, tree.NumberOfEdges(counter));
            Assert.IsTrue(node0.HasChild(node7, counter));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.RemoveNodes(null, counter);
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
                method.Invoke(tree, new object[] { counter });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(NoRootException));

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);

            TreeNode foundRoot = (TreeNode)method.Invoke(tree, new object[] { counter });
            Assert.AreEqual(node0, foundRoot);

            PropertyInfo nodesProperty = typeof(Tree<TreeNode>).GetProperty("InternalNodes", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException u = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                TreeNode node2 = new TreeNode(2);
                TreeNode node3 = new TreeNode(3);
                CountedCollection<TreeNode> collection = (CountedCollection<TreeNode>)nodesProperty.GetGetMethod(true).Invoke(tree, new object[] { });
                collection.Clear(counter);
                collection.Add(node2, counter);
                collection.Add(node3, counter);
                method.Invoke(tree, new object[] { counter });
            });
            Assert.IsInstanceOfType(u.InnerException, typeof(MultipleRootsException));
        }

        [TestMethod]
        public void TestArgumentNull()
        {
            Tree<TreeNode> t = new Tree<TreeNode>();
            TreeNode n = new TreeNode(0);
            TreeNode n2 = new TreeNode(1);

            Assert.ThrowsException<ArgumentNullException>(() => t.AddChild(null, n, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChild(n, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChild(n, n2, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChildren(null, new List<TreeNode>() { n }, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChildren(n, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddChildren(n, new List<TreeNode>() { n }, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddRoot(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.AddRoot(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasEdge(null, n, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasEdge(n, null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasEdge(n, n2, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasNode(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.HasNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNode(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNode(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNodes(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => t.RemoveNodes(new List<TreeNode>() { n }, null));

            MethodInfo method = typeof(Tree<TreeNode>).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException e = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                method.Invoke(t, new object[] { null, counter });
            });
            Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentNullException));
            e = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                method.Invoke(t, new object[] { n, null });
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node0, node2, counter);

            Assert.IsTrue(tree.IsValid());

            MethodInfo setParentMethod = typeof(TreeNode).GetMethod("SetParent", BindingFlags.NonPublic | BindingFlags.Instance);
            setParentMethod.Invoke(node1, new object[] { null, counter });

            Assert.IsFalse(tree.IsValid());

            setParentMethod.Invoke(node1, new object[] { node0, counter });

            Assert.IsTrue(tree.IsValid());

            TreeNode node3 = new TreeNode(3);
            setParentMethod.Invoke(node3, new object[] { node2, counter });

            PropertyInfo nodeproperty = typeof(Tree<TreeNode>).GetProperty("InternalNodes", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedCollection<TreeNode> collection = (CountedCollection<TreeNode>)nodeproperty.GetGetMethod(true).Invoke(tree, new object[] { });
            collection.Clear(counter);
            collection.Add(node0, counter);
            collection.Add(node1, counter);
            collection.Add(node2, counter);
            collection.Add(node3, counter);

            Assert.IsFalse(tree.IsValid());

            node0.AddChild(node3, counter);
            setParentMethod.Invoke(node3, new object[] { node2, counter });

            Assert.IsFalse(tree.IsValid());
        }
    }
}
