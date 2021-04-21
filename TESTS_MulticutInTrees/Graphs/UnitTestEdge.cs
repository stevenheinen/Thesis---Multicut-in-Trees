// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;

namespace TESTS_MulticutInTrees.Graphs
{
    [TestClass]
    public class UnitTestEdge
    {
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestConstructor()
        {
            Node n1 = new(1);
            Node n2 = new(2);
            Edge<Node> edge = new(n1, n2);

            Assert.IsNotNull(edge);

            Assert.ThrowsException<ArgumentException>(() => new Edge<Node>(n1, n1));
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Node n1 = new(1);
            Node n2 = new(2);
            Node n3 = new(3);
            Edge<Node> edge = new(n1, n2);

            Assert.ThrowsException<ArgumentNullException>(() => new Edge<Node>(null, n2));
            Assert.ThrowsException<ArgumentNullException>(() => new Edge<Node>(n1, null));
            Assert.ThrowsException<ArgumentNullException>(() => edge.ChangeEndpoint(null, n3, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => edge.ChangeEndpoint(n1, null, MockCounter));
            Assert.ThrowsException<ArgumentNullException>(() => edge.ChangeEndpoint(n1, n3, null));
            Assert.ThrowsException<ArgumentNullException>(() => edge.HasEndpoint(null));
        }

        [TestMethod]
        public void TestHasEndpoint()
        {
            Node n1 = new(1);
            Node n2 = new(2);
            Node n3 = new(3);
            Edge<Node> edge = new(n1, n2);

            Assert.IsTrue(edge.HasEndpoint(n1));
            Assert.IsTrue(edge.HasEndpoint(n2));
            Assert.IsFalse(edge.HasEndpoint(n3));
        }

        [TestMethod]
        public void TestChangeEndpoint1()
        {
            Node n1 = new(1);
            Node n2 = new(2);
            Node n3 = new(3);
            Edge<Node> edge = new(n1, n2);
            n1.AddNeighbour(n2, MockCounter);

            edge.ChangeEndpoint(n1, n3, MockCounter);

            Assert.IsFalse(n1.HasNeighbour(n2, MockCounter));
            Assert.IsFalse(n2.HasNeighbour(n1, MockCounter));
            Assert.IsTrue(n2.HasNeighbour(n3, MockCounter));
            Assert.IsTrue(n3.HasNeighbour(n2, MockCounter));

            edge.ChangeEndpoint(n2, n2, MockCounter);

            Assert.ThrowsException<InvalidOperationException>(() => edge.ChangeEndpoint(n1, n3, MockCounter));
            Assert.ThrowsException<InvalidOperationException>(() => edge.ChangeEndpoint(n2, n3, MockCounter));
            Assert.ThrowsException<InvalidOperationException>(() => edge.ChangeEndpoint(n3, n2, MockCounter));

            edge.ChangeEndpoint(n2, n1, MockCounter);

            Assert.IsFalse(n2.HasNeighbour(n1, MockCounter));
            Assert.IsFalse(n2.HasNeighbour(n3, MockCounter));
            Assert.IsFalse(n1.HasNeighbour(n2, MockCounter));
            Assert.IsFalse(n3.HasNeighbour(n2, MockCounter));
            Assert.IsTrue(n1.HasNeighbour(n3, MockCounter));
            Assert.IsTrue(n3.HasNeighbour(n1, MockCounter));
        }
    }
}
