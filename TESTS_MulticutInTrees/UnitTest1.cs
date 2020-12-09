// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees;

namespace TESTS_MulticutInTrees
{
    [TestClass]
    public class UnitTest1
    {
        private const string Expected = "Hello World!";

        [TestMethod]
        public void TestMethod1()
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);
            Program.Main();

            var result = sw.ToString().Trim();
            Assert.AreEqual(Expected, result);
        }
    }
}