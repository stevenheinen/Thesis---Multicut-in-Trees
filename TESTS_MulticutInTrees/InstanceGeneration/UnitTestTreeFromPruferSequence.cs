// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.InstanceGeneration
{
    [TestClass]
    public class UnitTestTreeFromPruferSequence
    {
        [TestMethod]
        public void TestTree()
        {
            Random random = new Random(87867438);

            Tree<TreeNode> tree = TreeFromPruferSequence.GenerateTree(525, random);
            Assert.IsNotNull(tree);
            Assert.AreEqual(525, tree.NumberOfNodes);
            Assert.IsTrue(tree.IsValid());

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => TreeFromPruferSequence.GenerateTree(2, random));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => TreeFromPruferSequence.GenerateTree(0, random));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => TreeFromPruferSequence.GenerateTree(-5165, random));
        }
    }
}
