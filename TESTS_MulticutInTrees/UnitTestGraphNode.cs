// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees;
using MulticutInTrees.Exceptions;

namespace TESTS_MulticutInTrees
{
    [TestClass]
    public class UnitTestGraphNode
    {
        [TestMethod]
        public void TestConstructorNoNeighbours()
        {
            GraphNode node;
            node = new GraphNode(0);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestConstructorWithNeighbours()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1;
            node1 = new GraphNode(1, new List<GraphNode>() { node0 });
            Assert.IsNotNull(node1);
            Assert.IsTrue(node1.HasNeighbour(node0));
            Assert.AreEqual(1, node1.Degree);
        }

        [TestMethod]
        public void TestConstructorWithNullNeighbours()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node0 = new GraphNode(0, null);
            });
        }

        [TestMethod]
        public void TestID()
        {
            GraphNode node = new GraphNode(496);
            Assert.AreEqual(node.ID, (uint)496);
        }

        [TestMethod]
        public void TestNeighbours()
        {
            GraphNode node = new GraphNode(0);
            Assert.IsNotNull(node.Neighbours);
        }

        [TestMethod]
        public void TestCompareToNull()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node = new GraphNode(0);
                bool b = node == null;
            });
            Assert.AreEqual(a.ParamName, "rhs");
        }

        [TestMethod]
        public void TestCompareToNullLeft()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node = new GraphNode(0);
                bool b = null == node;
            });
            Assert.AreEqual(a.ParamName, "lhs");
        }

        [TestMethod]
        public void TestCompareToNotNull()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node = new GraphNode(0);
                bool b = node != null;
            });
            Assert.AreEqual(a.ParamName, "rhs");
        }

        [TestMethod]
        public void TestCompareToNotNullLeft()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node = new GraphNode(0);
                bool b = null != node;
            });
            Assert.AreEqual(a.ParamName, "lhs");
        }

        [TestMethod]
        public void TestCompareToItselfTrue()
        {
            GraphNode node0 = new GraphNode(0);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(node0 == node0);
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void TestCompareToItselfFalse()
        {
            GraphNode node0 = new GraphNode(0);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsFalse(node0 != node0);
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void TestEqualToOther()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);
            Assert.IsFalse(node0.Equals(node1));
        }

        [TestMethod]
        public void TestEqualToItself()
        {
            GraphNode node0 = new GraphNode(0);
            Assert.IsTrue(node0.Equals(node0));
        }

        [TestMethod]
        public void TestEqualToNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node = new GraphNode(0);
                bool b = node.Equals(null);
            });
        }

        [TestMethod]
        public void TestEqualsOperatorToOther()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);
            Assert.IsFalse(node0 == node1);
        }

        [TestMethod]
        public void TestNotEqualsOperatorToOtherTrue()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);
            Assert.IsTrue(node0 != node1);
        }

        [TestMethod]
        public void TestGenericEqualsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node = new GraphNode(0);
                object obj = null;
                bool b = node.Equals(obj);
            });
        }

        [TestMethod]
        public void TestGenericEqualsDifferentTypes()
        {
            GraphNode node = new GraphNode(0);

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
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);

            Assert.IsTrue(node0.Equals((object)node0));
            Assert.IsFalse(node0.Equals((object)node1));
        }

        [TestMethod]
        public void TestAddChildAddItself()
        {
            Assert.ThrowsException<AddNeighbourToSelfException>(() =>
            {
                GraphNode node0 = new GraphNode(0);
                node0.AddNeighbour(node0);
            });
        }

        [TestMethod]
        public void TestAddChildAddOtherNode()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);

            Assert.IsFalse(node0.HasNeighbour(node1));
            node0.AddNeighbour(node1);
            Assert.IsTrue(node0.HasNeighbour(node1));
        }

        [TestMethod]
        public void TestAddChildAddNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node0 = new GraphNode(0);
                node0.AddNeighbour(null);
            });
        }

        [TestMethod]
        public void TestAddChildren()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);
            GraphNode node2 = new GraphNode(2);

            node0.AddNeighbours(new List<GraphNode>() { node1, node2 });
            Assert.IsTrue(node0.HasNeighbour(node1));
            Assert.IsTrue(node0.HasNeighbour(node2));
        }

        [TestMethod]
        public void TestAddChildrenNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node0 = new GraphNode(0);
                node0.AddNeighbours(null);
            });
        }

        [TestMethod]
        public void TestRemoveAllChildren()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);
            GraphNode node2 = new GraphNode(2);

            node0.AddNeighbours(new List<GraphNode>() { node1, node2 });
            node0.RemoveAllNeighbours();
            Assert.IsFalse(node0.HasNeighbour(node1));
            Assert.IsFalse(node0.HasNeighbour(node2));
        }

        [TestMethod]
        public void TestRemoveChild()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);

            node0.AddNeighbour(node1);
            node0.RemoveNeighbour(node1);
            Assert.IsFalse(node0.HasNeighbour(node1));
        }

        [TestMethod]
        public void TestRemoveNonExistingChild()
        {
            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                GraphNode node0 = new GraphNode(0);
                GraphNode node1 = new GraphNode(1);
                node0.RemoveNeighbour(node1);
            });
        }

        [TestMethod]
        public void TestRemoveNullChild()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node0 = new GraphNode(0);
                node0.RemoveNeighbour(null);
            });
        }

        [TestMethod]
        public void TestRemoveChildren()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);
            GraphNode node2 = new GraphNode(2);
            GraphNode node3 = new GraphNode(3);

            node0.AddNeighbours(new List<GraphNode>() { node1, node3, node2 });
            node0.RemoveNeighbours(new List<GraphNode>() { node2, node1 });
            Assert.IsFalse(node0.HasNeighbour(node1));
            Assert.IsFalse(node0.HasNeighbour(node2));
            Assert.IsTrue(node0.HasNeighbour(node3));
        }

        [TestMethod]
        public void TestRemoveNullChildren()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node0 = new GraphNode(0);
                GraphNode node1 = new GraphNode(1);
                GraphNode node2 = new GraphNode(2);
                node0.AddNeighbours(new List<GraphNode>() { node1, node2 });
                node0.RemoveNeighbours(null);
            });
        }

        [TestMethod]
        public void TestToString()
        {
            GraphNode node = new GraphNode(24362);
            Assert.AreEqual("GraphNode 24362", node.ToString());
        }

        [TestMethod]
        public void TestHasNeighbour()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);
            GraphNode node2 = new GraphNode(2, new List<GraphNode>() { node1 });
            node1.AddNeighbour(node0);
            Assert.IsTrue(node2.HasNeighbour(node1));
            Assert.IsFalse(node2.HasNeighbour(node0));
            Assert.IsTrue(node1.HasNeighbour(node0));
            Assert.IsFalse(node1.HasNeighbour(node2));
            Assert.IsFalse(node0.HasNeighbour(node1));
            Assert.IsFalse(node0.HasNeighbour(node2));
        }

        [TestMethod]
        public void TestHasNullNeighbour()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GraphNode node = new GraphNode(0);
                node.HasNeighbour(null);
            });
        }

        [TestMethod]
        public void TestNeighbourList()
        {
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);

            Assert.IsNotNull(node0.Neighbours);
            node0.AddNeighbour(node1);
            Assert.IsNotNull(node0.Neighbours);
            Assert.IsTrue(node0.Neighbours.Contains(node1));
        }

        [TestMethod]
        public void TestDegree()
        {
            List<GraphNode> nodes = new List<GraphNode>() 
            { 
                new GraphNode(0), 
                new GraphNode(1), 
                new GraphNode(2), 
                new GraphNode(3), 
                new GraphNode(4) 
            };

            Assert.AreEqual(nodes[0].Degree, 0);

            nodes[0].AddNeighbour(nodes[1]);
            Assert.AreEqual(nodes[0].Degree, 1);

            nodes[0].AddNeighbour(nodes[2]);
            Assert.AreEqual(nodes[0].Degree, 2);

            nodes[0].AddNeighbours(new List<GraphNode>() { nodes[3], nodes[4] });
            Assert.AreEqual(nodes[0].Degree, 4);

            nodes[0].RemoveNeighbour(nodes[1]);
            Assert.AreEqual(nodes[0].Degree, 3);

            nodes[0].RemoveNeighbours(new List<GraphNode>() { nodes[3], nodes[4] });
            Assert.AreEqual(nodes[0].Degree, 1);

            Assert.ThrowsException<NotANeighbourException>(() =>
            {
                nodes[0].RemoveNeighbour(nodes[1]);
            });

            Assert.ThrowsException<AlreadyANeighbourException>(() =>
            {
                nodes[0].AddNeighbour(nodes[2]);
            });

            nodes[0].AddNeighbours(new List<GraphNode>() { nodes[3], nodes[4] });
            nodes[0].RemoveAllNeighbours();
            Assert.AreEqual(nodes[0].Degree, 0);
        }
    }
}
