// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;

namespace TESTS_MulticutInTrees.CountedDatastructures
{
    [TestClass]
    public class UnitTestCounter
    {
        [TestMethod]
        public void TestConstructor()
        {
            Counter counter = new();
            Assert.IsNotNull(counter);
            Assert.AreEqual(0, counter.Value);
        }

        [TestMethod]
        public void TestIncrement()
        {
            Counter counter = new();
            counter++;
            Assert.AreEqual(1, counter.Value);
            ++counter;
            Assert.AreEqual(2, counter.Value);
        }

        [TestMethod]
        public void TestDecrement()
        {
            Counter counter = new();
            counter--;
            Assert.AreEqual(-1, counter.Value);
            --counter;
            Assert.AreEqual(-2, counter.Value);
        }

        [TestMethod]
        public void TestAddition()
        {
            Counter counter = new();
            counter += 683717687;
            counter += 34568;
            Assert.AreEqual(683752255, counter.Value);
#pragma warning disable IDE0054 // Use compound assignment
            counter = counter + 127536;
#pragma warning restore IDE0054 // Use compound assignment
            Assert.AreEqual(683879791, counter.Value);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            Counter counter = new();
            counter += 684651743681;
            counter -= 864684;
            counter -= 135236154;
            Assert.AreEqual(684515642843, counter.Value);
#pragma warning disable IDE0054 // Use compound assignment
            counter = counter - 234786591;
#pragma warning restore IDE0054 // Use compound assignment
            Assert.AreEqual(684280856252, counter.Value);
        }

        [TestMethod]
        public void TestToString()
        {
            Counter counter = new();
#pragma warning disable IDE0054 // Use compound assignment
            counter = counter + 2357811;
#pragma warning restore IDE0054 // Use compound assignment
            Assert.AreEqual("2357811", counter.ToString());
        }
    }
}
