// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;

namespace TESTS_MulticutInTrees.Exceptions
{
    [TestClass]
    public class UnitTestAlreadyInGraphException
    {
        [TestMethod]
        public void TestConstructorNoArguments()
        {
            AlreadyInGraphException alreadyInGraphException = new AlreadyInGraphException();
            Assert.IsNotNull(alreadyInGraphException);
        }

        [TestMethod]
        public void TestConstructorMessage()
        {
            AlreadyInGraphException alreadyInGraphException = new AlreadyInGraphException("Test message");
            Assert.IsNotNull(alreadyInGraphException);
            Assert.AreEqual(alreadyInGraphException.Message, "Test message");
        }

        [TestMethod]
        public void TestConstructorMessageInner()
        {
            Exception inner = new Exception();
            AlreadyInGraphException alreadyInGraphException = new AlreadyInGraphException("Test", inner);
            Assert.IsNotNull(alreadyInGraphException);
            Assert.AreEqual(alreadyInGraphException.Message, "Test");
            Assert.AreEqual(alreadyInGraphException.InnerException, inner);
        }
    }
}
