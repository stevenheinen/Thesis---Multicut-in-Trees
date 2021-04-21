// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;

namespace TESTS_MulticutInTrees.CountedDatastructures
{
    [TestClass]
    public class UnitTestCountedEnumerator
    {
        [TestMethod]
        public void TestNullParameter()
        {
            List<int> list = new();
            IEnumerator<int> enumerator = list.GetEnumerator();
            Counter counter = new();

            Assert.ThrowsException<ArgumentNullException>(() => new CountedEnumerator<int>(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => new CountedEnumerator<int>(enumerator, null));
        }
    }
}
