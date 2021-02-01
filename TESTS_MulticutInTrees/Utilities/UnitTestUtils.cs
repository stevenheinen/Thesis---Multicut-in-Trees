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
            Assert.ThrowsException<ArgumentNullException>(() => Utils.OrderEdgeSmallToLarge((null, n)));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.OrderEdgeSmallToLarge((n, null)));
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
    }
}
