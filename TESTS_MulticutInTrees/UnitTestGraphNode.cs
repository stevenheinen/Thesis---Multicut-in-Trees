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
            GraphNode node0 = new GraphNode(0, null);
            Assert.IsNotNull(node0);
            GraphNode node1 = new GraphNode(1);
            try
            {
                node0.AddNeighbour(node1);
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
            GraphNode node = new GraphNode(0);
            try
            {
                bool b = node == null;
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Right hand side of == operator for GraphNode is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
        }

        [TestMethod]
        public void TestCompareToNullLeft()
        {
            GraphNode node = new GraphNode(0);
            try
            {
                bool b = null == node;
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Left hand side of == operator for GraphNode is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
        }

        [TestMethod]
        public void TestCompareToNotNull()
        {
            GraphNode node = new GraphNode(0);
            try
            {
                bool b = node != null;
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Right hand side of != operator for GraphNode is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
        }

        [TestMethod]
        public void TestCompareToNotNullLeft()
        {
            GraphNode node = new GraphNode(0);
            try
            {
                bool b = null != node;
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Left hand side of != operator for GraphNode is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
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
            GraphNode node0 = new GraphNode(0);

            try
            {
                bool b = node0.Equals(null);
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Trying to compare {node0} to null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
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
            GraphNode node = new GraphNode(0);
            object obj = null;
            try
            {
                bool b = node.Equals(obj);
                Assert.Fail("Expected Exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Trying to compare {node} to null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
        }

        [TestMethod]
        public void TestGenericEqualsDifferentTypes()
        {
            GraphNode node = new GraphNode(0);

            int i = 0;
            string s = "0";
            object obj = new object();
            char c = '0';

            try
            {
                bool b = node.Equals(i);
                Assert.Fail("Expected Exception!");
            }
            catch (IncompatibleTypesException e)
            {
                Assert.AreEqual(e.Message, $"Type of {i} (type: {typeof(int)}) cannot be compared to {node} (type: {typeof(GraphNode)})!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(IncompatibleTypesException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }

            try
            {
                bool b = node.Equals(s);
                Assert.Fail("Expected Exception!");
            }
            catch (IncompatibleTypesException e)
            {
                Assert.AreEqual(e.Message, $"Type of {s} (type: {typeof(string)}) cannot be compared to {node} (type: {typeof(GraphNode)})!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(IncompatibleTypesException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }

            try
            {
                bool b = node.Equals(obj);
                Assert.Fail("Expected Exception!");
            }
            catch (IncompatibleTypesException e)
            {
                Assert.AreEqual(e.Message, $"Type of {obj} (type: {typeof(object)}) cannot be compared to {node} (type: {typeof(GraphNode)})!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(IncompatibleTypesException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }

            try
            {
                bool b = node.Equals(c);
                Assert.Fail("Expected Exception!");
            }
            catch (IncompatibleTypesException e)
            {
                Assert.AreEqual(e.Message, $"Type of {c} (type: {typeof(char)}) cannot be compared to {node} (type: {typeof(GraphNode)})!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(IncompatibleTypesException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
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
            GraphNode node0 = new GraphNode(0);

            try
            {
                node0.AddNeighbour(node0);
                Assert.Fail("Expected exception!");
            }
            catch (AddNeighbourToSelfException e)
            {
                Assert.AreEqual(e.Message, $"Trying to add {node0} as a child to itself!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected an {typeof(AddNeighbourToSelfException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
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
            GraphNode node0 = new GraphNode(0);

            try
            {
                node0.AddNeighbour(null);
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Trying to add a child to {node0}, but child is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
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
            GraphNode node0 = new GraphNode(0);

            try
            {
                node0.AddNeighbours(null);
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Trying to add a list of children to {node0}, but the list is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
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
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);

            try
            {
                node0.RemoveNeighbour(node1);
                Assert.Fail("Expected exception!");
            }
            catch (NotANeighbourException e)
            {
                Assert.AreEqual(e.Message, $"Trying to remove {node1} from {node0}'s neighbours, but {node1} is no neighbour of {node0}!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NotANeighbourException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
        }

        [TestMethod]
        public void TestRemoveNullChild()
        {
            GraphNode node0 = new GraphNode(0);

            try
            {
                node0.RemoveNeighbour(null);
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Trying to remove a child from {node0}, but child is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
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
            GraphNode node0 = new GraphNode(0);
            GraphNode node1 = new GraphNode(1);
            GraphNode node2 = new GraphNode(2);

            node0.AddNeighbours(new List<GraphNode>() { node1, node2 });

            try
            {
                node0.RemoveNeighbours(null);
                Assert.Fail("Expected exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Trying to remove a list of children from {node0}, but the list is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
        }

        [TestMethod]
        public void TestToString()
        {
            GraphNode node = new GraphNode(24362);
            Assert.AreEqual("Node 24362", node.ToString());
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
            GraphNode node = new GraphNode(0);
            try
            {
                node.HasNeighbour(null);
                Assert.Fail("Expected Exception!");
            }
            catch (NullReferenceException e)
            {
                Assert.AreEqual(e.Message, $"Trying to see if {node} has a neighbour, but the neighbour is null!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NullReferenceException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }
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
            List<GraphNode> nodes = new List<GraphNode>();
            for (uint i = 0; i < 5; i++)
            {
                nodes.Add(new GraphNode(i));
            }

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

            try
            {
                nodes[0].RemoveNeighbour(nodes[1]);
                Assert.Fail("An exception should have been thrown!");
            }
            catch (NotANeighbourException e)
            {
                Assert.AreEqual(e.Message, $"Trying to remove {nodes[1]} from {nodes[0]}'s neighbours, but {nodes[1]} is no neighbour of {nodes[0]}!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected a {typeof(NotANeighbourException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }

            try
            {
                nodes[0].AddNeighbour(nodes[2]);
                Assert.Fail("An exception should have been thrown!");
            }
            catch (AlreadyANeighbourException e)
            {
                Assert.AreEqual(e.Message, $"Trying to add {nodes[2]} as a neighbour to {nodes[0]}, but {nodes[2]} is already a neighbour of {nodes[0]}!");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected an {typeof(AlreadyANeighbourException)}, but got an {typeof(Exception)} with message: {e.Message}");
            }

            nodes[0].AddNeighbours(new List<GraphNode>() { nodes[3], nodes[4] });
            nodes[0].RemoveAllNeighbours();
            Assert.AreEqual(nodes[0].Degree, 0);
        }
    }
}
