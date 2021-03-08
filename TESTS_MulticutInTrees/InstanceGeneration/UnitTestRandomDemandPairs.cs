// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.InstanceGeneration
{
    [TestClass]
    public class UnitTestRandomDemandPairs
    {
        [TestMethod]
        public void TestGenerateRandomDemandPairs()
        {
            Random random = new Random(1357151375);

            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(100, random);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => RandomDemandPairs.GenerateRandomDemandPairs(-26, tree, random));
            Assert.ThrowsException<ArgumentNullException>(() => RandomDemandPairs.GenerateRandomDemandPairs(5, null, random));

            List<DemandPair> demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(0, tree, random);
            Assert.IsNotNull(demandPairs);
            Assert.AreEqual(0, demandPairs.Count);

            demandPairs = RandomDemandPairs.GenerateRandomDemandPairs(40, tree, random);
            Assert.IsNotNull(demandPairs);
            Assert.AreEqual(40, demandPairs.Count);

            foreach (DemandPair demandPair in demandPairs)
            {
                Assert.AreNotEqual(demandPair.Node1, demandPair.Node2);
            }
        }
    }
}
