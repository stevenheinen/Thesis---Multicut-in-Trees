// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;

namespace TESTS_MulticutInTrees.Exceptions
{
    [TestClass]
    public class UnitTestMultipleRootsException
    {
        [TestMethod]
        public void TestConstructorNoArguments()
        {
            MultipleRootsException multipleRootsException = new MultipleRootsException();
            Assert.IsNotNull(multipleRootsException);
        }

        [TestMethod]
        public void TestConstructorMessage()
        {
            MultipleRootsException multipleRootsException = new MultipleRootsException("Test message");
            Assert.IsNotNull(multipleRootsException);
            Assert.AreEqual(multipleRootsException.Message, "Test message");
        }

        [TestMethod]
        public void TestConstructorMessageInner()
        {
            Exception inner = new Exception();
            MultipleRootsException multipleRootsException = new MultipleRootsException("Test", inner);
            Assert.IsNotNull(multipleRootsException);
            Assert.AreEqual(multipleRootsException.Message, "Test");
            Assert.AreEqual(multipleRootsException.InnerException, inner);
        }
    }
}
