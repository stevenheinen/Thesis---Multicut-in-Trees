// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;

namespace TESTS_MulticutInTrees.Exceptions
{
    [TestClass]
    public class UnitTestAlreadyPresentException
    {
        [TestMethod]
        public void TestConstructorNoArguments()
        {
            AlreadyPresentException alreadyPresentException = new AlreadyPresentException();
            Assert.IsNotNull(alreadyPresentException);
        }

        [TestMethod]
        public void TestConstructorMessage()
        {
            AlreadyPresentException alreadyPresentException = new AlreadyPresentException("Test message");
            Assert.IsNotNull(alreadyPresentException);
            Assert.AreEqual(alreadyPresentException.Message, "Test message");
        }

        [TestMethod]
        public void TestConstructorMessageInner()
        {
            Exception inner = new Exception();
            AlreadyPresentException alreadyPresentException = new AlreadyPresentException("Test", inner);
            Assert.IsNotNull(alreadyPresentException);
            Assert.AreEqual(alreadyPresentException.Message, "Test");
            Assert.AreEqual(alreadyPresentException.InnerException, inner);
        }
    }
}
