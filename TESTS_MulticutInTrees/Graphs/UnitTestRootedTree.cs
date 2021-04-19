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
    public class UnitTestRootedTree
    {
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            RootedTree tree = new RootedTree();
            Assert.IsNotNull(tree);

            Assert.IsNotNull(tree.Edges(MockCounter));
            Assert.IsNotNull(tree.Nodes(MockCounter));
        }

        [TestMethod]
        public void TestAddNode()
        {
            RootedTree tree = new RootedTree();
            RootedTreeNode node0 = new RootedTreeNode(0);
            tree.AddNode(node0, MockCounter);
            Assert.AreEqual(node0, tree.GetRoot(MockCounter));

            RootedTreeNode node1 = new RootedTreeNode(1);
            tree.AddNode(node1, MockCounter);
            Edge<RootedTreeNode> edge01 = new Edge<RootedTreeNode>(node0, node1);
            tree.AddEdge(edge01, MockCounter);
            Assert.AreEqual(node0, node1.GetParent(MockCounter));

            RootedTreeNode node2 = new RootedTreeNode(2);
            tree.AddNode(node2, MockCounter);
            Edge<RootedTreeNode> edge02 = new Edge<RootedTreeNode>(node0, node2);
            tree.AddEdge(edge02, MockCounter);
            Assert.AreEqual(node0, node2.GetParent(MockCounter));
            Assert.AreEqual(2, node0.NumberOfChildren(MockCounter));

            RootedTreeNode node3 = new RootedTreeNode(3);
            tree.AddNode(node3, MockCounter);
            Edge<RootedTreeNode> edge13 = new Edge<RootedTreeNode>(node1, node3);
            tree.AddEdge(edge13, MockCounter);
            Assert.AreEqual(node1, node3.GetParent(MockCounter));
            Assert.AreEqual(node0, node1.GetParent(MockCounter));
            Assert.AreEqual(1, node1.NumberOfChildren(MockCounter));
            Assert.AreEqual(2, node1.Degree(MockCounter));
        }

        [TestMethod]
        public void TestAddChildrenToParent()
        {
            RootedTree tree = new RootedTree();

            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);
            RootedTreeNode node3 = new RootedTreeNode(3);
            RootedTreeNode node4 = new RootedTreeNode(4);
            RootedTreeNode node5 = new RootedTreeNode(5);
            RootedTreeNode node6 = new RootedTreeNode(6);
            RootedTreeNode node7 = new RootedTreeNode(7);
            RootedTreeNode node8 = new RootedTreeNode(8);
            RootedTreeNode node9 = new RootedTreeNode(9);
            RootedTreeNode node10 = new RootedTreeNode(10);

            tree.AddNodes(new List<RootedTreeNode>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            Edge<RootedTreeNode> edge01 = new Edge<RootedTreeNode>(node0, node1);
            Edge<RootedTreeNode> edge02 = new Edge<RootedTreeNode>(node0, node2);
            Edge<RootedTreeNode> edge03 = new Edge<RootedTreeNode>(node0, node3);
            Edge<RootedTreeNode> edge14 = new Edge<RootedTreeNode>(node1, node4);
            Edge<RootedTreeNode> edge15 = new Edge<RootedTreeNode>(node1, node5);
            Edge<RootedTreeNode> edge26 = new Edge<RootedTreeNode>(node2, node6);
            Edge<RootedTreeNode> edge37 = new Edge<RootedTreeNode>(node3, node7);
            Edge<RootedTreeNode> edge38 = new Edge<RootedTreeNode>(node3, node8);
            Edge<RootedTreeNode> edge39 = new Edge<RootedTreeNode>(node3, node9);
            Edge<RootedTreeNode> edge310 = new Edge<RootedTreeNode>(node3, node10);

            tree.AddEdges(new List<Edge<RootedTreeNode>>() { edge01, edge02, edge03, edge14, edge15, edge26, edge37, edge38, edge39, edge310 }, MockCounter);

            MethodInfo method = typeof(RootedTree).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(tree, new object[] { node3, MockCounter });

            Assert.AreEqual(7, node0.NumberOfChildren(MockCounter));
            Assert.AreEqual(0, node3.NumberOfChildren(MockCounter));
            Assert.AreEqual(node0, node9.GetParent(MockCounter));
            Assert.IsTrue(node0.HasChild(node7, MockCounter));

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                method.Invoke(tree, new object[] { node0, MockCounter });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(NotSupportedException));

            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                RootedTree tree2 = new RootedTree();
                RootedTreeNode node11 = new RootedTreeNode(11);
                tree2.AddNode(node11, MockCounter);
                method.Invoke(tree2, new object[] { node11, MockCounter });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(NotSupportedException));
        }

        [TestMethod]
        public void TestAddChildrenToParentNull()
        {
            TargetInvocationException a = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                RootedTree tree = new RootedTree();
                MethodInfo method = typeof(RootedTree).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(tree, new object[] { null, MockCounter });
            });
            Assert.IsInstanceOfType(a.InnerException, typeof(ArgumentNullException));
            Assert.AreEqual("node", ((ArgumentNullException)a.InnerException).ParamName);

            a = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                RootedTree tree = new RootedTree();
                RootedTreeNode node = new RootedTreeNode(0);
                MethodInfo method = typeof(RootedTree).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(tree, new object[] { node, null });
            });
            Assert.IsInstanceOfType(a.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestRemoveNode()
        {
            RootedTree tree = new RootedTree();

            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);
            RootedTreeNode node3 = new RootedTreeNode(3);
            RootedTreeNode node4 = new RootedTreeNode(4);
            RootedTreeNode node5 = new RootedTreeNode(5);
            RootedTreeNode node6 = new RootedTreeNode(6);
            RootedTreeNode node7 = new RootedTreeNode(7);
            RootedTreeNode node8 = new RootedTreeNode(8);
            RootedTreeNode node9 = new RootedTreeNode(9);
            RootedTreeNode node10 = new RootedTreeNode(10);

            tree.AddNodes(new List<RootedTreeNode>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            Edge<RootedTreeNode> edge01 = new Edge<RootedTreeNode>(node0, node1);
            Edge<RootedTreeNode> edge02 = new Edge<RootedTreeNode>(node0, node2);
            Edge<RootedTreeNode> edge03 = new Edge<RootedTreeNode>(node0, node3);
            Edge<RootedTreeNode> edge14 = new Edge<RootedTreeNode>(node1, node4);
            Edge<RootedTreeNode> edge15 = new Edge<RootedTreeNode>(node1, node5);
            Edge<RootedTreeNode> edge26 = new Edge<RootedTreeNode>(node2, node6);
            Edge<RootedTreeNode> edge37 = new Edge<RootedTreeNode>(node3, node7);
            Edge<RootedTreeNode> edge38 = new Edge<RootedTreeNode>(node3, node8);
            Edge<RootedTreeNode> edge39 = new Edge<RootedTreeNode>(node3, node9);
            Edge<RootedTreeNode> edge310 = new Edge<RootedTreeNode>(node3, node10);

            tree.AddEdges(new List<Edge<RootedTreeNode>>() { edge01, edge02, edge03, edge14, edge15, edge26, edge37, edge38, edge39, edge310 }, MockCounter);

            Assert.AreEqual(11, tree.NumberOfNodes(MockCounter));
            Assert.AreEqual(10, tree.NumberOfEdges(MockCounter));

            tree.RemoveNode(node6, MockCounter);
            Assert.AreEqual(10, tree.NumberOfNodes(MockCounter));
            Assert.AreEqual(9, tree.NumberOfEdges(MockCounter));

            tree.RemoveNode(node3, MockCounter);
            Assert.AreEqual(9, tree.NumberOfNodes(MockCounter));
            Assert.AreEqual(8, tree.NumberOfEdges(MockCounter));

            Assert.IsFalse(node0.HasChild(node3, MockCounter));
            Assert.AreEqual(6, node0.NumberOfChildren(MockCounter));

            tree.AddNode(node3, MockCounter);
            tree.AddEdge(new Edge<RootedTreeNode>(node3, node0), MockCounter);
            tree.RemoveNode(node3, MockCounter);

            Assert.ThrowsException<MultipleRootsException>(() =>
            {
                tree.RemoveNode(node0, MockCounter);
            });

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.RemoveNode(null, MockCounter);
            });
            Assert.AreEqual("node", a.ParamName);

            Assert.ThrowsException<NotInGraphException>(() =>
            {
                tree.RemoveNode(node6, MockCounter);
            });

            Assert.IsTrue(tree.IsValid());
        }

        [TestMethod]
        public void TestRemoveNodes()
        {
            RootedTree tree = new RootedTree();

            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);
            RootedTreeNode node3 = new RootedTreeNode(3);
            RootedTreeNode node4 = new RootedTreeNode(4);
            RootedTreeNode node5 = new RootedTreeNode(5);
            RootedTreeNode node6 = new RootedTreeNode(6);
            RootedTreeNode node7 = new RootedTreeNode(7);
            RootedTreeNode node8 = new RootedTreeNode(8);
            RootedTreeNode node9 = new RootedTreeNode(9);
            RootedTreeNode node10 = new RootedTreeNode(10);

            tree.AddNodes(new List<RootedTreeNode>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);

            Edge<RootedTreeNode> edge01 = new Edge<RootedTreeNode>(node0, node1);
            Edge<RootedTreeNode> edge02 = new Edge<RootedTreeNode>(node0, node2);
            Edge<RootedTreeNode> edge03 = new Edge<RootedTreeNode>(node0, node3);
            Edge<RootedTreeNode> edge14 = new Edge<RootedTreeNode>(node1, node4);
            Edge<RootedTreeNode> edge15 = new Edge<RootedTreeNode>(node1, node5);
            Edge<RootedTreeNode> edge26 = new Edge<RootedTreeNode>(node2, node6);
            Edge<RootedTreeNode> edge37 = new Edge<RootedTreeNode>(node3, node7);
            Edge<RootedTreeNode> edge38 = new Edge<RootedTreeNode>(node3, node8);
            Edge<RootedTreeNode> edge39 = new Edge<RootedTreeNode>(node3, node9);
            Edge<RootedTreeNode> edge310 = new Edge<RootedTreeNode>(node3, node10);

            tree.AddEdges(new List<Edge<RootedTreeNode>>() { edge01, edge02, edge03, edge14, edge15, edge26, edge37, edge38, edge39, edge310 }, MockCounter);

            tree.RemoveNodes(new List<RootedTreeNode>() { node1, node3, node9 }, MockCounter);
            Assert.AreEqual(8, tree.NumberOfNodes(MockCounter));
            Assert.AreEqual(7, tree.NumberOfEdges(MockCounter));
            Assert.IsTrue(node0.HasChild(node7, MockCounter));

            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                tree.RemoveNodes(null, MockCounter);
            });
            Assert.AreEqual("nodes", a.ParamName);
        }

        [TestMethod]
        public void TestUpdateRoot()
        {
            RootedTree tree = new RootedTree();
            MethodInfo method = typeof(RootedTree).GetMethod("FindRoot", BindingFlags.NonPublic | BindingFlags.Instance);

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                method.Invoke(tree, new object[] { MockCounter });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(NoRootException));

            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            tree.AddNode(node0, MockCounter);
            tree.AddNode(node1, MockCounter);
            tree.AddEdge(new Edge<RootedTreeNode>(node0, node1), MockCounter);

            RootedTreeNode foundRoot = (RootedTreeNode)method.Invoke(tree, new object[] { MockCounter });
            Assert.AreEqual(node0, foundRoot);

            PropertyInfo nodesProperty = typeof(RootedTree).GetProperty("InternalNodes", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException u = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                RootedTreeNode node2 = new RootedTreeNode(2);
                RootedTreeNode node3 = new RootedTreeNode(3);
                CountedCollection<RootedTreeNode> collection = (CountedCollection<RootedTreeNode>)nodesProperty.GetGetMethod(true).Invoke(tree, new object[] { });
                collection.Clear(MockCounter);
                collection.Add(node2, MockCounter);
                collection.Add(node3, MockCounter);
                method.Invoke(tree, new object[] { MockCounter });
            });
            Assert.IsInstanceOfType(u.InnerException, typeof(MultipleRootsException));
        }

        [TestMethod]
        public void TestArgumentNull()
        {
            RootedTree t = new RootedTree();
            RootedTreeNode n = new RootedTreeNode(0);
            RootedTreeNode n2 = new RootedTreeNode(1);

            MethodInfo method = typeof(RootedTree).GetMethod("AddChildrenToParent", BindingFlags.NonPublic | BindingFlags.Instance);
            TargetInvocationException e = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                method.Invoke(t, new object[] { null, MockCounter });
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
            RootedTree tree = new RootedTree();

            Assert.IsFalse(tree.IsValid());

            RootedTreeNode node0 = new RootedTreeNode(0);
            RootedTreeNode node1 = new RootedTreeNode(1);
            RootedTreeNode node2 = new RootedTreeNode(2);

            tree.AddNodes(new List<RootedTreeNode>() { node0, node1, node2 }, MockCounter);

            tree.AddEdge(new Edge<RootedTreeNode>(node0, node1), MockCounter);
            tree.AddEdge(new Edge<RootedTreeNode>(node0, node2), MockCounter);

            Assert.IsTrue(tree.IsValid());

            MethodInfo setParentMethod = typeof(RootedTreeNode).GetMethod("SetParent", BindingFlags.NonPublic | BindingFlags.Instance);
            setParentMethod.Invoke(node1, new object[] { null, MockCounter });

            Assert.IsFalse(tree.IsValid());

            setParentMethod.Invoke(node1, new object[] { node0, MockCounter });

            Assert.IsTrue(tree.IsValid());

            RootedTreeNode node3 = new RootedTreeNode(3);
            setParentMethod.Invoke(node3, new object[] { node2, MockCounter });

            PropertyInfo nodeproperty = typeof(RootedTree).GetProperty("InternalNodes", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedCollection<RootedTreeNode> collection = (CountedCollection<RootedTreeNode>)nodeproperty.GetGetMethod(true).Invoke(tree, new object[] { });
            collection.Clear(MockCounter);
            collection.Add(node0, MockCounter);
            collection.Add(node1, MockCounter);
            collection.Add(node2, MockCounter);
            collection.Add(node3, MockCounter);

            Assert.IsFalse(tree.IsValid());
            
            setParentMethod.Invoke(node3, new object[] { null, MockCounter });

            tree.AddEdge(new Edge<RootedTreeNode>(node0, node3), MockCounter);
            setParentMethod.Invoke(node3, new object[] { node2, MockCounter });

            Assert.IsFalse(tree.IsValid());
        }
    }
}
