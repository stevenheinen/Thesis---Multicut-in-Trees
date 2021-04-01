// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.MulticutProblem
{
    [TestClass]
    public class UnitTestPerformanceMeasurements
    {
        [TestMethod]
        public void TestNullParameter()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PerformanceMeasurements(null));
        }

        [TestMethod]
        public void TestConstructor()
        {
            PerformanceMeasurements pm = new PerformanceMeasurements("test");
            Assert.IsNotNull(pm);
        }

        [TestMethod]
        public void TestToString()
        {
            string ownerName = "3718ty187tg1311241`2-0b5130t13";
            string expected = $"\n==============================================================\n==============================================================\n{ownerName} instance modifications\n==============================================================\nNumber of contracted edges:    0\nNumber of changed demandpairs: 0\nNumber of removed demandpairs: 0\n==============================================================\n{ownerName} operations\n==============================================================\nOperations on the input tree:              0\nOperations on demandpairs:                 0\nOperations on demandpairs per edge keys:   0\nOperations on demandpairs per edge values: 0\n==============================================================\n{ownerName} time\n==============================================================\nTime spent checking applicability (in ticks): 0\nTime spent modifying the instance (in ticks): 0\nTime spent checking applicability (TimeSpan): 00:00:00.0000000\nTime spent modifying the instance (TimeSpan): 00:00:00.0000000\n==============================================================\n\n";
            PerformanceMeasurements pm = new PerformanceMeasurements(ownerName);
            Assert.AreEqual(expected, pm.ToString());
        }
    }
}
