// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;

namespace TESTS_MulticutInTrees.Exceptions
{
    [TestClass]
    public class UnitTestNoRootException
    {
        [TestMethod]
        public void TestConstructorNoArguments()
        {
            NoRootException addNeighbourToSelfException = new NoRootException();
            Assert.IsNotNull(addNeighbourToSelfException);
        }

        [TestMethod]
        public void TestConstructorMessage()
        {
            NoRootException addNeighbourToSelfException = new NoRootException("Test message");
            Assert.IsNotNull(addNeighbourToSelfException);
            Assert.AreEqual(addNeighbourToSelfException.Message, "Test message");
        }

        [TestMethod]
        public void TestConstructorMessageInner()
        {
            Exception inner = new Exception();
            NoRootException addNeighbourToSelfException = new NoRootException("Test", inner);
            Assert.IsNotNull(addNeighbourToSelfException);
            Assert.AreEqual(addNeighbourToSelfException.Message, "Test");
            Assert.AreEqual(addNeighbourToSelfException.InnerException, inner);
        }
    }
}
