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
    public class UnitTestNode
    {
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestConstructorNoNeighbours()
        {
            Node node;
            node = new Node(0);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestID()
        {
            Node node = new(496);
            Assert.AreEqual(node.ID, (uint)496);
        }

        [TestMethod]
        public void TestNeighbours()
        {
            Node node = new(0);
            Assert.IsNotNull(node.Neighbours(MockCounter));
        }

        [TestMethod]
        public void TestAddChildAddItself()
        {
            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                Node node0 = new(0);
                node0.AddNeighbour(node0, MockCounter);
            });
        }

        [TestMethod]
        public void TestAddChildAddOtherNode()
        {
            Node node0 = new(0);
            Node node1 = new(1);

            Assert.IsFalse(node0.HasNeighbour(node1, MockCounter));
            node0.AddNeighbour(node1, MockCounter);
            Assert.IsTrue(node0.HasNeighbour(node1, MockCounter));
        }

        [TestMethod]
        public void TestAddChildAddNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new(0);
                node0.AddNeighbour(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestAddChildren()
        {
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);

            node0.AddNeighbours(new List<Node>() { node1, node2 }, MockCounter);
            Assert.IsTrue(node0.HasNeighbour(node1, MockCounter));
            Assert.IsTrue(node0.HasNeighbour(node2, MockCounter));
        }

        [TestMethod]
        public void TestAddChildrenNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new(0);
                node0.AddNeighbours(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveAllChildren()
        {
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);

            node0.AddNeighbours(new List<Node>() { node1, node2 }, MockCounter);
            node0.RemoveAllNeighbours(MockCounter);
            Assert.IsFalse(node0.HasNeighbour(node1, MockCounter));
            Assert.IsFalse(node0.HasNeighbour(node2, MockCounter));
            Assert.IsFalse(node2.HasNeighbour(node0, MockCounter));
        }

        [TestMethod]
        public void TestRemoveChild()
        {
            Node node0 = new(0);
            Node node1 = new(1);

            node0.AddNeighbour(node1, MockCounter);
            node0.RemoveNeighbour(node1, MockCounter);
            Assert.IsFalse(node0.HasNeighbour(node1, MockCounter));
            Assert.IsFalse(node1.HasNeighbour(node0, MockCounter));
        }

        [TestMethod]
        public void TestRemoveNonExistingChild()
        {
            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                Node node0 = new(0);
                Node node1 = new(1);
                node0.RemoveNeighbour(node1, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveNullChild()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new(0);
                node0.RemoveNeighbour(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestRemoveChildren()
        {
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);

            node0.AddNeighbours(new List<Node>() { node1, node3, node2 }, MockCounter);
            node0.RemoveNeighbours(new List<Node>() { node2, node1 }, MockCounter);
            Assert.IsFalse(node0.HasNeighbour(node1, MockCounter));
            Assert.IsFalse(node0.HasNeighbour(node2, MockCounter));
            Assert.IsTrue(node0.HasNeighbour(node3, MockCounter));
        }

        [TestMethod]
        public void TestRemoveNullChildren()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new(0);
                Node node1 = new(1);
                Node node2 = new(2);
                node0.AddNeighbours(new List<Node>() { node1, node2 }, MockCounter);
                node0.RemoveNeighbours(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestToString()
        {
            Node node = new(24362);
            Assert.AreEqual("Node 24362", node.ToString());
        }

        [TestMethod]
        public void TestHasNeighbour()
        {
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            node2.AddNeighbour(node1, MockCounter);
            node1.AddNeighbour(node0, MockCounter);
            Assert.IsTrue(node2.HasNeighbour(node1, MockCounter));
            Assert.IsFalse(node2.HasNeighbour(node0, MockCounter));
            Assert.IsTrue(node1.HasNeighbour(node0, MockCounter));
            Assert.IsTrue(node1.HasNeighbour(node2, MockCounter));
            Assert.IsTrue(node0.HasNeighbour(node1, MockCounter));
            Assert.IsFalse(node0.HasNeighbour(node2, MockCounter));
        }

        [TestMethod]
        public void TestHasNullNeighbour()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node = new(0);
                node.HasNeighbour(null, MockCounter);
            });
        }

        [TestMethod]
        public void TestNeighbourList()
        {
            Node node0 = new(0);
            Node node1 = new(1);

            Assert.IsNotNull(node0.Neighbours(MockCounter));
            node0.AddNeighbour(node1, MockCounter);
            Assert.IsNotNull(node0.Neighbours(MockCounter));
            Assert.IsTrue(new List<Node>(node0.Neighbours(MockCounter)).Contains(node1));
        }

        [TestMethod]
        public void TestDegree()
        {
            List<Node> nodes = new()
            {
                new Node(0),
                new Node(1),
                new Node(2),
                new Node(3),
                new Node(4)
            };

            Assert.AreEqual(nodes[0].Degree(MockCounter), 0);

            nodes[0].AddNeighbour(nodes[1], MockCounter);
            Assert.AreEqual(nodes[0].Degree(MockCounter), 1);

            nodes[0].AddNeighbour(nodes[2], MockCounter);
            Assert.AreEqual(nodes[0].Degree(MockCounter), 2);

            nodes[0].AddNeighbours(new List<Node>() { nodes[3], nodes[4] }, MockCounter);
            Assert.AreEqual(nodes[0].Degree(MockCounter), 4);

            nodes[0].RemoveNeighbour(nodes[1], MockCounter);
            Assert.AreEqual(nodes[0].Degree(MockCounter), 3);

            nodes[0].RemoveNeighbours(new List<Node>() { nodes[3], nodes[4] }, MockCounter);
            Assert.AreEqual(nodes[0].Degree(MockCounter), 1);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                nodes[0].RemoveNeighbour(nodes[1], MockCounter);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                nodes[0].AddNeighbour(nodes[2], MockCounter);
            });

            nodes[0].AddNeighbours(new List<Node>() { nodes[3], nodes[4] }, MockCounter);
            nodes[0].RemoveAllNeighbours(MockCounter);
            Assert.AreEqual(nodes[0].Degree(MockCounter), 0);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Node n = new(0);
            List<Node> list = new();

            Assert.ThrowsException<ArgumentNullException>(() => n.AddNeighbour(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => n.AddNeighbour(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.AddNeighbours(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => n.AddNeighbours(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.HasNeighbour(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => n.HasNeighbour(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveNeighbour(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveNeighbour(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveNeighbours(null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveNeighbours(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveAllNeighbours(null));
        }
    }
}
