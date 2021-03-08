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
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestConstructorNoNeighbours()
        {
            Node node;
            node = new Node(0);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestConstructorWithUndirectedNeighbours()
        {
            Node node0 = new Node(0);
            Node node1;
            node1 = new Node(1, new List<Node>() { node0 }, counter);
            Assert.IsNotNull(node1);
            Assert.IsTrue(node1.HasNeighbour(node0, counter));
            Assert.IsTrue(node0.HasNeighbour(node1, counter));
            Assert.AreEqual(1, node1.Degree(counter));
            Assert.AreEqual(1, node0.Degree(counter));
        }

        [TestMethod]
        public void TestConstructorWithDirectedNeighbours()
        {
            Node node0 = new Node(0);
            Node node1;
            node1 = new Node(1, new List<Node>() { node0 }, counter, true);
            Assert.IsNotNull(node1);
            Assert.IsTrue(node1.HasNeighbour(node0, counter));
            Assert.IsFalse(node0.HasNeighbour(node1, counter));
            Assert.AreEqual(1, node1.Degree(counter));
            Assert.AreEqual(0, node0.Degree(counter));
        }

        [TestMethod]
        public void TestConstructorWithNullNeighbours()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new Node(0, null, counter);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new Node(0, new List<Node>(), null);
            });
        }

        [TestMethod]
        public void TestID()
        {
            Node node = new Node(496);
            Assert.AreEqual(node.ID, (uint)496);
        }

        [TestMethod]
        public void TestNeighbours()
        {
            Node node = new Node(0);
            Assert.IsNotNull(node.Neighbours(counter));
        }

        [TestMethod]
        public void TestAddChildAddItself()
        {
            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                Node node0 = new Node(0);
                node0.AddNeighbour(node0, counter);
            });
        }

        [TestMethod]
        public void TestAddChildAddOtherNode()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);

            Assert.IsFalse(node0.HasNeighbour(node1, counter));
            node0.AddNeighbour(node1, counter);
            Assert.IsTrue(node0.HasNeighbour(node1, counter));
        }

        [TestMethod]
        public void TestAddChildAddNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new Node(0);
                node0.AddNeighbour(null, counter);
            });
        }

        [TestMethod]
        public void TestAddChildren()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            node0.AddNeighbours(new List<Node>() { node1, node2 }, counter);
            Assert.IsTrue(node0.HasNeighbour(node1, counter));
            Assert.IsTrue(node0.HasNeighbour(node2, counter));
        }

        [TestMethod]
        public void TestAddChildrenNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new Node(0);
                node0.AddNeighbours(null, counter);
            });
        }

        [TestMethod]
        public void TestRemoveAllChildren()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            node0.AddNeighbours(new List<Node>() { node1, node2 }, counter);
            node0.RemoveAllNeighbours(counter);
            Assert.IsFalse(node0.HasNeighbour(node1, counter));
            Assert.IsFalse(node0.HasNeighbour(node2, counter));
            Assert.IsFalse(node2.HasNeighbour(node0, counter));
        }

        [TestMethod]
        public void TestRemoveChild()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);

            node0.AddNeighbour(node1, counter);
            node0.RemoveNeighbour(node1, counter);
            Assert.IsFalse(node0.HasNeighbour(node1, counter));
            Assert.IsFalse(node1.HasNeighbour(node0, counter));
        }

        [TestMethod]
        public void TestRemoveNonExistingChild()
        {
            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                Node node0 = new Node(0);
                Node node1 = new Node(1);
                node0.RemoveNeighbour(node1, counter);
            });
        }

        [TestMethod]
        public void TestRemoveNullChild()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new Node(0);
                node0.RemoveNeighbour(null, counter);
            });
        }

        [TestMethod]
        public void TestRemoveChildren()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            node0.AddNeighbours(new List<Node>() { node1, node3, node2 }, counter);
            node0.RemoveNeighbours(new List<Node>() { node2, node1 }, counter);
            Assert.IsFalse(node0.HasNeighbour(node1, counter));
            Assert.IsFalse(node0.HasNeighbour(node2, counter));
            Assert.IsTrue(node0.HasNeighbour(node3, counter));
        }

        [TestMethod]
        public void TestRemoveNullChildren()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node0 = new Node(0);
                Node node1 = new Node(1);
                Node node2 = new Node(2);
                node0.AddNeighbours(new List<Node>() { node1, node2 }, counter);
                node0.RemoveNeighbours(null, counter);
            });
        }

        [TestMethod]
        public void TestToString()
        {
            Node node = new Node(24362);
            Assert.AreEqual("Node 24362", node.ToString());
        }

        [TestMethod]
        public void TestHasNeighbour()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2, new List<Node>() { node1 }, counter);
            node1.AddNeighbour(node0, counter);
            Assert.IsTrue(node2.HasNeighbour(node1, counter));
            Assert.IsFalse(node2.HasNeighbour(node0, counter));
            Assert.IsTrue(node1.HasNeighbour(node0, counter));
            Assert.IsTrue(node1.HasNeighbour(node2, counter));
            Assert.IsTrue(node0.HasNeighbour(node1, counter));
            Assert.IsFalse(node0.HasNeighbour(node2, counter));
        }

        [TestMethod]
        public void TestHasNullNeighbour()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Node node = new Node(0);
                node.HasNeighbour(null, counter);
            });
        }

        [TestMethod]
        public void TestNeighbourList()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);

            Assert.IsNotNull(node0.Neighbours(counter));
            node0.AddNeighbour(node1, counter);
            Assert.IsNotNull(node0.Neighbours(counter));
            Assert.IsTrue(new List<Node>(node0.Neighbours(counter)).Contains(node1));
        }

        [TestMethod]
        public void TestDegree()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(0),
                new Node(1),
                new Node(2),
                new Node(3),
                new Node(4)
            };

            Assert.AreEqual(nodes[0].Degree(counter), 0);

            nodes[0].AddNeighbour(nodes[1], counter);
            Assert.AreEqual(nodes[0].Degree(counter), 1);

            nodes[0].AddNeighbour(nodes[2], counter);
            Assert.AreEqual(nodes[0].Degree(counter), 2);

            nodes[0].AddNeighbours(new List<Node>() { nodes[3], nodes[4] }, counter);
            Assert.AreEqual(nodes[0].Degree(counter), 4);

            nodes[0].RemoveNeighbour(nodes[1], counter);
            Assert.AreEqual(nodes[0].Degree(counter), 3);

            nodes[0].RemoveNeighbours(new List<Node>() { nodes[3], nodes[4] }, counter);
            Assert.AreEqual(nodes[0].Degree(counter), 1);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                nodes[0].RemoveNeighbour(nodes[1], counter);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                nodes[0].AddNeighbour(nodes[2], counter);
            });

            nodes[0].AddNeighbours(new List<Node>() { nodes[3], nodes[4] }, counter);
            nodes[0].RemoveAllNeighbours(counter);
            Assert.AreEqual(nodes[0].Degree(counter), 0);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Node n = new Node(0);
            List<Node> list = new List<Node>();

            Assert.ThrowsException<ArgumentNullException>(() => n.AddNeighbour(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => n.AddNeighbour(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.AddNeighbours(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => n.AddNeighbours(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.HasNeighbour(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => n.HasNeighbour(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveNeighbour(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveNeighbour(n, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveNeighbours(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveNeighbours(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => n.RemoveAllNeighbours(null));
        }
    }
}
