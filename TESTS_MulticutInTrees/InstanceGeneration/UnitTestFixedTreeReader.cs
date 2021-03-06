// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.InstanceGeneration;

namespace TESTS_MulticutInTrees.InstanceGeneration
{
    [TestClass]
    public class UnitTestFixedTreeReader
    {
        [TestMethod]
        public void TestNullParameter()
        {
            string filepath = "test.txt";
            Assert.ThrowsException<ArgumentNullException>(() => FixedTreeReader.ReadTree(null));
            File.Delete(filepath);
            Assert.ThrowsException<FileNotFoundException>(() => FixedTreeReader.ReadTree(filepath));
        }
    }
}
