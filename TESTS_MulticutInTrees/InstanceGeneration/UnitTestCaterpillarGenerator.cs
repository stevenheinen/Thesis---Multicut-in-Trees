// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;

namespace TESTS_MulticutInTrees.InstanceGeneration
{
    [TestClass]
    public class UnitTestCaterpillarGenerator
    {
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestNullParameter()
        {
            Random random = new Random(654);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => CaterpillarGenerator.CreateCaterpillar(-1, random));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => CaterpillarGenerator.CreateCaterpillar(3, random));
            Assert.ThrowsException<ArgumentNullException>(() => CaterpillarGenerator.CreateCaterpillar(105, null));
        }

        [TestMethod]
        public void TestNodeTypes()
        {
            Random random = new Random(13758613);
            int numberOfNodes = 3841;

            Graph caterpillar = CaterpillarGenerator.CreateCaterpillar(numberOfNodes, random);

            Assert.AreEqual(numberOfNodes, caterpillar.NumberOfNodes(MockCounter));

            int i1Nodes = 0;

            foreach (Node node in caterpillar.Nodes(MockCounter))
            {
                if (node.Type == NodeType.I1)
                {
                    i1Nodes++;
                }
                if (node.Type == NodeType.I3 || node.Type == NodeType.L3)
                {
                    Assert.Fail("Caterpillar is not a caterpillar!");
                }
            }

            Assert.AreEqual(2, i1Nodes);
        }
    }
}
