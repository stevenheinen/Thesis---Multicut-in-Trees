// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using MulticutInTrees;

namespace TESTS_MulticutInTrees
{
    [TestClass]
    public class UnitTestNode
    {
        [TestMethod]
        public void TestConstructorNoNeighbours()
        {
            Node node;
            node = new Node(0);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TestConstructorWithNeighbours()
        {
            Node node0 = new Node(0);
            Node node1;
            node1 = new Node(1, new List<Node>() { node0 });
            Assert.IsNotNull(node1);
            Assert.IsTrue(node1.HasNeighbour(node0));
            Assert.AreEqual(1, node1.Degree);
        }

        [TestMethod]
        public void TestConstructorWithNullNeighbours()
        {
            Node node0 = new Node(0, null);
            Assert.IsNotNull(node0);
            Node node1 = new Node(1);
            try
            {
                node0.AddChild(node1);
                Assert.IsTrue(node0.HasNeighbour(node1));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void TestCompareToNull()
        {
            Node node = new Node(0);
            try
            {
                bool b = node == null;
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Right hand side of == operator for Node is null!");
            }
        }

        [TestMethod]
        public void TestCompareToNullLeft()
        {
            Node node = new Node(0);
            try
            {
                bool b = null == node;
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Left hand side of == operator for Node is null!");
            }
        }

        [TestMethod]
        public void TestCompareToNotNull()
        {
            Node node = new Node(0);
            try
            {
                bool b = node != null;
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Right hand side of != operator for Node is null!");
            }
        }

        [TestMethod]
        public void TestCompareToNotNullLeft()
        {
            Node node = new Node(0);
            try
            {
                bool b = null != node;
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Left hand side of != operator for Node is null!");
            }
        }

        [TestMethod]
        public void TestCompareToItselfTrue()
        {
            Node node0 = new Node(0);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(node0 == node0);
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void TestCompareToItselfFalse()
        {
            Node node0 = new Node(0);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsFalse(node0 != node0);
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void TestEqualToOther()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Assert.IsFalse(node0.Equals(node1));
        }

        [TestMethod]
        public void TestEqualToItself()
        {
            Node node0 = new Node(0);
            Assert.IsTrue(node0.Equals(node0));
        }

        [TestMethod]
        public void TestEqualToNull()
        {
            Node node0 = new Node(0);

            try
            {
                bool b = node0.Equals(null);
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to compare {node0} to null!");
            }
        }

        [TestMethod]
        public void TestEqualsOperatorToOther()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Assert.IsFalse(node0 == node1);
        }

        [TestMethod]
        public void TestNotEqualsOperatorToOtherTrue()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Assert.IsTrue(node0 != node1);
        }

        [TestMethod]
        public void TestAddChildAddItself()
        {
            Node node0 = new Node(0);

            try
            {
                node0.AddChild(node0);
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to add {node0} as a child to itself!");
            }
        }

        [TestMethod]
        public void TestAddChildAddOtherNode()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);

            Assert.IsFalse(node0.HasNeighbour(node1));
            node0.AddChild(node1);
            Assert.IsTrue(node0.HasNeighbour(node1));
        }

        [TestMethod]
        public void TestAddChildAddNull()
        {
            Node node0 = new Node(0);

            try
            {
                node0.AddChild(null);
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to add a child to {node0}, but child is null!");
            }
        }

        [TestMethod]
        public void TestAddChildren()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            node0.AddChildren(new List<Node>() { node1, node2 });
            Assert.IsTrue(node0.HasNeighbour(node1));
            Assert.IsTrue(node0.HasNeighbour(node2));
        }

        [TestMethod]
        public void TestAddChildrenNull()
        {
            Node node0 = new Node(0);

            try
            {
                node0.AddChildren(null);
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to add a list of children to {node0}, but the list is null!");
            }
        }

        [TestMethod]
        public void TestRemoveAllChildren()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            node0.AddChildren(new List<Node>() { node1, node2 });
            node0.RemoveAllChildren();
            Assert.IsFalse(node0.HasNeighbour(node1));
            Assert.IsFalse(node0.HasNeighbour(node2));
        }

        [TestMethod]
        public void TestRemoveChild()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);

            node0.AddChild(node1);
            node0.RemoveChild(node1);
            Assert.IsFalse(node0.HasNeighbour(node1));
        }

        [TestMethod]
        public void TestRemoveNonExistingChild()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);

            try
            {
                node0.RemoveChild(node1);
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to remove {node1} from {node0}'s neighbours, but {node1} is no neighbour of {node0}!");
            }
        }

        [TestMethod]
        public void TestRemoveNullChild()
        {
            Node node0 = new Node(0);

            try
            {
                node0.RemoveChild(null);
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to remove a child from {node0}, but child is null!");
            }
        }

        [TestMethod]
        public void TestRemoveChildren()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);

            node0.AddChildren(new List<Node>() { node1, node3, node2 });
            node0.RemoveChildren(new List<Node>() { node2, node1 });
            Assert.IsFalse(node0.HasNeighbour(node1));
            Assert.IsFalse(node0.HasNeighbour(node2));
            Assert.IsTrue(node0.HasNeighbour(node3));
        }

        [TestMethod]
        public void TestRemoveNullChildren()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);

            node0.AddChildren(new List<Node>() { node1, node2 });

            try
            {
                node0.RemoveChildren(null);
                Assert.Fail("Expected exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to remove a list of children from {node0}, but the list is null!");
            }
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
            Node node2 = new Node(2, new List<Node>() { node1 });
            node1.AddChild(node0);
            Assert.IsTrue(node2.HasNeighbour(node1));
            Assert.IsFalse(node2.HasNeighbour(node0));
            Assert.IsTrue(node1.HasNeighbour(node0));
            Assert.IsFalse(node1.HasNeighbour(node2));
            Assert.IsFalse(node0.HasNeighbour(node1));
            Assert.IsFalse(node0.HasNeighbour(node2));
        }

        [TestMethod]
        public void TestNeighbourList()
        {
            Node node0 = new Node(0);
            Node node1 = new Node(1);

            Assert.IsNotNull(node0.Neighbours);
            node0.AddChild(node1);
            Assert.IsNotNull(node0.Neighbours);
            Assert.IsTrue(node0.Neighbours.Contains(node1));
        }

        [TestMethod]
        public void TestDegree()
        {
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < 5; i++)
            {
                nodes.Add(new Node(i));
            }

            Assert.AreEqual(nodes[0].Degree, 0);

            nodes[0].AddChild(nodes[1]);
            Assert.AreEqual(nodes[0].Degree, 1);

            nodes[0].AddChild(nodes[2]);
            Assert.AreEqual(nodes[0].Degree, 2);

            nodes[0].AddChildren(new List<Node>() { nodes[3], nodes[4] });
            Assert.AreEqual(nodes[0].Degree, 4);

            nodes[0].RemoveChild(nodes[1]);
            Assert.AreEqual(nodes[0].Degree, 3);

            nodes[0].RemoveChildren(new List<Node>() { nodes[3], nodes[4] });
            Assert.AreEqual(nodes[0].Degree, 1);

            try
            {
                nodes[0].RemoveChild(nodes[1]);
                Assert.Fail("An exception should have been thrown!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to remove {nodes[1]} from {nodes[0]}'s neighbours, but {nodes[1]} is no neighbour of {nodes[0]}!");
            }

            try
            {
                nodes[0].AddChild(nodes[2]);
                Assert.Fail("An exception should have been thrown!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Trying to add {nodes[2]} as a neighbour to {nodes[0]}, but {nodes[2]} is already a neighbour of {nodes[0]}!");
            }

            nodes[0].AddChildren(new List<Node>() { nodes[3], nodes[4] });
            nodes[0].RemoveAllChildren();
            Assert.AreEqual(nodes[0].Degree, 0);
        }
    }
}
