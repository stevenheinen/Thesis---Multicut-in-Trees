// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestUtils
    {
        [TestMethod]
        public void TestNullParameter()
        {
            Node n = new Node(0);
            List<int> list = new List<int>();
            Random random = new Random(0);
            Assert.ThrowsException<ArgumentNullException>(() => Utils.OrderEdgeSmallToLarge((null, n)));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.OrderEdgeSmallToLarge((n, null)));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.IsSubsetOf(null, list));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.IsSubsetOf(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.Print<int>(null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.Print<Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.NodePathToEdgePath<Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandomWhere<int>(null, n => true, random));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandomWhere(list, null, random));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandomWhere(list, n => true, null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandom<int>(null, random));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandom(list, null));
        }

        [TestMethod]
        public void TestOrderEdgeSmallToLarge()
        {
            Node n0 = new Node(0);
            Node n1 = new Node(1);

            Assert.AreEqual((n0, n1), Utils.OrderEdgeSmallToLarge((n0, n1)));
            Assert.AreEqual((n0, n1), Utils.OrderEdgeSmallToLarge((n1, n0)));
            Assert.AreEqual((n0, n0), Utils.OrderEdgeSmallToLarge((n0, n0)));
        }

        [TestMethod]
        public void TestNullCheck()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() => Utils.NullCheck<Node>(null, "testName", null));
            Assert.IsTrue(a.Message == "Value cannot be null. (Parameter 'testName')");

            Assert.ThrowsException<ArgumentNullException>(() => Utils.NullCheck<Node>(null, null));
        }

        [TestMethod]
        public void TestPrint()
        {
            List<int> list = new List<int>();
            Assert.AreEqual("System.Collections.Generic.List`1[System.Int32] with 0 elements.", list.Print());

            list = new List<int>() { 4, 984, 8, 465, 8, 47, 643, 85, 6, 43, 54, 384, 3, 46, 74, -146785, 4, -4, -4, 4, 56, 1, 4, -49 };
            Assert.AreEqual("System.Collections.Generic.List`1[System.Int32] with 24 elements: [4, 984, 8, 465, 8, 47, 643, 85, 6, 43, 54, 384, 3, 46, 74, -146785, 4, -4, -4, 4, 56, 1, 4, -49]", list.Print());
        }

        [TestMethod]
        public void TestPickRandom()
        {
            Random random = new Random(638276819);
            List<int> list = new List<int>() { 6854, 6584, 64, 684, 35, 2173, 814, 98, 14, 631, 18, 7, 6871, 8, 7, 81, 78, 17, 86, 167, 817, 3, 98, 78, 171, 306714107, 43, 714, 07, 7, 737, 54, 0, 54, 07, 1, 04, 453, 08, 3 };
            for (int i = 0; i < 1000; i++)
            {
                int element = list.PickRandom(random);
                Assert.IsTrue(list.Contains(element));
            }
        }

        [TestMethod]
        public void TestPickRandomWhere()
        {
            Random random = new Random(746818141);
            List<int> list = new List<int>() { 6854, 6584, 64, 684, 35, 2173, 814, 98, 14, 631, 18, 7, 6871, 8, 7, 81, 78, 17, 86, 167, 817, 3, 98, 78, 171, 306714107, 43, 714, 07, 7, 737, 54, 0, 54, 07, 1, 04, 453, 08, 3 };
            for (int i = 0; i < 1000; i++)
            {
                int element = list.PickRandomWhere(n => n % 2 == 0, random);
                Assert.IsTrue(element % 2 == 0);
                Assert.IsTrue(list.Contains(element));
            }
        }
    }
}
