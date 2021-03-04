// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.CountedDatastructures
{
    [TestClass]
    public class UnitTestCounter
    {
        [TestMethod]
        public void TestConstructor()
        {
            Counter counter = new Counter();
            Assert.IsNotNull(counter);
            Assert.AreEqual(0, counter.Value);
        }

        [TestMethod]
        public void TestIncrement()
        {
            Counter counter = new Counter();
            counter++;
            Assert.AreEqual(1, counter.Value);
            ++counter;
            Assert.AreEqual(2, counter.Value);
        }

        [TestMethod]
        public void TestDecrement()
        {
            Counter counter = new Counter();
            counter--;
            Assert.AreEqual(-1, counter.Value);
            --counter;
            Assert.AreEqual(-2, counter.Value);
        }

        [TestMethod]
        public void TestAddition()
        {
            Counter counter = new Counter();
            counter += 683717687;
            counter += 34568;
            Assert.AreEqual(683752255, counter.Value);
            counter = counter + 127536;
            Assert.AreEqual(683879791, counter.Value);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            Counter counter = new Counter();
            counter += 684651743681;
            counter -= 864684;
            counter -= 135236154;
            Assert.AreEqual(684515642843, counter.Value);
            counter = counter - 234786591;
            Assert.AreEqual(684280856252, counter.Value);
        }

        [TestMethod]
        public void TestToString()
        {
            Counter counter = new Counter();
            counter = counter + 2357811;
            Assert.AreEqual("2357811", counter.ToString());
        }
    }
}
