// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.MulticutProblem
{
    [TestClass]
    public class UnitTestMulticutInstance
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            List<DemandPair> dps = new List<DemandPair>();
            Random random = new Random(4854);

            MulticutInstance instance = new MulticutInstance(tree, dps, 2, random);

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            List<DemandPair> dps = new List<DemandPair>();
            Random random = new Random(4854);

            Assert.ThrowsException<ArgumentNullException>(() => { MulticutInstance instance = new MulticutInstance(null, dps, 2, random); });
            Assert.ThrowsException<ArgumentNullException>(() => { MulticutInstance instance = new MulticutInstance(tree, null, 2, random); });
            Assert.ThrowsException<ArgumentNullException>(() => { MulticutInstance instance = new MulticutInstance(tree, dps, 2, null); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { MulticutInstance instance = new MulticutInstance(tree, dps, 0, random); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { MulticutInstance instance = new MulticutInstance(tree, dps, -1, random); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { MulticutInstance instance = new MulticutInstance(tree, dps, -8459472, random); });
        }
    }
}
