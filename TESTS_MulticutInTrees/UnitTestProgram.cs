// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees;

namespace TESTS_MulticutInTrees
{
    [TestClass]
    public class UnitTestProgram
    {
        [TestMethod]
        public void TestMain()
        {
            Program.Main(new string[0]);
        }
    }
}