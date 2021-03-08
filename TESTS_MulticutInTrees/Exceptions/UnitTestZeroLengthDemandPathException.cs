// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;

namespace TESTS_MulticutInTrees.Exceptions
{
    [TestClass]
    public class UnitTestZeroLengthDemandPathException
    {
        [TestMethod]
        public void TestConstructorNoArguments()
        {
            ZeroLengthDemandPathException notInGraphException = new ZeroLengthDemandPathException();
            Assert.IsNotNull(notInGraphException);
        }

        [TestMethod]
        public void TestConstructorMessage()
        {
            ZeroLengthDemandPathException notInGraphException = new ZeroLengthDemandPathException("Test message");
            Assert.IsNotNull(notInGraphException);
            Assert.AreEqual(notInGraphException.Message, "Test message");
        }

        [TestMethod]
        public void TestConstructorMessageInner()
        {
            Exception inner = new Exception();
            ZeroLengthDemandPathException notInGraphException = new ZeroLengthDemandPathException("Test", inner);
            Assert.IsNotNull(notInGraphException);
            Assert.AreEqual(notInGraphException.Message, "Test");
            Assert.AreEqual(notInGraphException.InnerException, inner);
        }
    }
}
