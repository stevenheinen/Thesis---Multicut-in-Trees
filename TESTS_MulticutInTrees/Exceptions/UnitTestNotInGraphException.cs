// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;

namespace TESTS_MulticutInTrees.Exceptions
{
    [TestClass]
    public class UnitTestNotInGraphException
    {
        [TestMethod]
        public void TestConstructorNoArguments()
        {
            NotInGraphException notInGraphException = new NotInGraphException();
            Assert.IsNotNull(notInGraphException);
        }

        [TestMethod]
        public void TestConstructorMessage()
        {
            NotInGraphException notInGraphException = new NotInGraphException("Test message");
            Assert.IsNotNull(notInGraphException);
            Assert.AreEqual(notInGraphException.Message, "Test message");
        }

        [TestMethod]
        public void TestConstructorMessageInner()
        {
            Exception inner = new Exception();
            NotInGraphException notInGraphException = new NotInGraphException("Test", inner);
            Assert.IsNotNull(notInGraphException);
            Assert.AreEqual(notInGraphException.Message, "Test");
            Assert.AreEqual(notInGraphException.InnerException, inner);
        }
    }
}