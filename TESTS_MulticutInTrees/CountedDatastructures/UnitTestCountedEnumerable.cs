// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;

namespace TESTS_MulticutInTrees.CountedDatastructures
{
    [TestClass]
    public class UnitTestCountedEnumerable
    {
        [TestMethod]
        public void TestNullParameter()
        {
            List<int> enumerable = new List<int>();
            Counter counter = new Counter();
            Assert.ThrowsException<ArgumentNullException>(() => new CountedEnumerable<int>(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => new CountedEnumerable<int>(enumerable, null));
        }

        [TestMethod]
        public void TestGetEnumerator()
        {
            List<int> enumerable = new List<int>();
            Counter counter = new Counter();
            CountedEnumerable<int> countedEnumerable = new CountedEnumerable<int>(enumerable, counter);
            Assert.IsNotNull(countedEnumerable.GetEnumerator());
        }
    }
}
