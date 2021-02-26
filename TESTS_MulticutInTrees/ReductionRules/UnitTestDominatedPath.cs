// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestDominatedPath
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            Random random = new Random(564468);
            MulticutInstance instance = new MulticutInstance(tree, dps, 10, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>();

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, random, demandPairsPerEdge);
            Assert.IsNotNull(dominatedPath);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            Random random = new Random(546454);
            MulticutInstance instance = new MulticutInstance(tree, dps, 10, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(null, dps, gnfpt, random, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, null, gnfpt, random, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, null, random, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, null, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, random, null); });
        }

        [TestMethod]
        public void TestRunFirstIteration()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node0, node2);
            tree.AddChild(node1, node3);
            tree.AddChild(node1, node4);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node2, node1);
            DemandPair dp2 = new DemandPair(node2, node0);
            DemandPair dp3 = new DemandPair(node3, node4);
            DemandPair dp4 = new DemandPair(node1, node3);
            DemandPair dp5 = new DemandPair(node3, node1);

            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3, dp4, dp5 };
            Random random = new Random(6578489);
            MulticutInstance instance = new MulticutInstance(tree, dps, 10, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                { (node0, node1), new CountedList<DemandPair>(){ dp1 } },
                { (node0, node2), new CountedList<DemandPair>(){ dp1, dp2 } },
                { (node1, node3), new CountedList<DemandPair>(){ dp3, dp4, dp5 } },
                { (node1, node4), new CountedList<DemandPair>(){ dp3 } },
            };

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, random, demandPairsPerEdge);
            Assert.IsTrue(dominatedPath.RunFirstIteration());
            Assert.AreEqual(2, dps.Count);
        }


        [TestMethod]
        public void TestAfterEdgeContractionFalse()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 });
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node3, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                { (node0, node1), new CountedList<DemandPair>() { dp1 } },
                { (node1, node2), new CountedList<DemandPair>() { dp1 } },
                { (node2, node3), new CountedList<DemandPair>() { dp2 } },
                { (node2, node4), new CountedList<DemandPair>() { dp2 } },
            };

            Random random = new Random(96868648);
            MulticutInstance instance = new MulticutInstance(tree, dps, 2, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, random, demandPairsPerEdge);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)> contractedEdgeInformation = new CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)>()
            {
                ((node3, node5), node3, new CountedList<DemandPair>() { dp2 } )
            };

            Assert.IsFalse(dominatedPath.AfterEdgeContraction(contractedEdgeInformation));
        }

        [TestMethod]
        public void TestAfterEdgeContractionTrue()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 });
            tree.AddChild(node3, node6);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node3, node4);
            DemandPair dp3 = new DemandPair(node4, node6);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                { (node0, node1), new CountedList<DemandPair>() { dp1 } },
                { (node1, node2), new CountedList<DemandPair>() { dp1 } },
                { (node2, node3), new CountedList<DemandPair>() { dp2, dp3 } },
                { (node2, node4), new CountedList<DemandPair>() { dp2, dp3 } },
                { (node3, node6), new CountedList<DemandPair>() { dp3 } },
            };

            Random random = new Random(648564);
            MulticutInstance instance = new MulticutInstance(tree, dps, 2, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, random, demandPairsPerEdge);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)> contractedEdgeInformation = new CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)>()
            {
                ((node3, node5), node3, new CountedList<DemandPair>() { dp2 } )
            };

            Assert.IsTrue(dominatedPath.AfterEdgeContraction(contractedEdgeInformation));
        }

        [TestMethod]
        public void TestAfterDemandPathRemove()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 });
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node3, node4);
            DemandPair dp3 = new DemandPair(node1, node3);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                { (node0, node1), new CountedList<DemandPair>() { dp1 } },
                { (node1, node2), new CountedList<DemandPair>() { dp1 } },
                { (node2, node3), new CountedList<DemandPair>() { dp2 } },
                { (node2, node4), new CountedList<DemandPair>() { dp2 } },
            };

            Random random = new Random(9869898);
            MulticutInstance instance = new MulticutInstance(tree, dps, 2, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, random, demandPairsPerEdge);

            CountedList<DemandPair> removedDemandpairs = new CountedList<DemandPair>() { dp3 };

            Assert.IsFalse(dominatedPath.AfterDemandPathRemove(removedDemandpairs));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 });
            tree.AddChild(node3, node5);
            tree.AddChild(node3, node6);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node5, node4);
            DemandPair dp3 = new DemandPair(node4, node3);
            CountedList<DemandPair> dps = new CountedList<DemandPair>() { dp1, dp2, dp3 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                { (node0, node1), new CountedList<DemandPair>() { dp1 } },
                { (node1, node2), new CountedList<DemandPair>() { dp1 } },
                { (node2, node3), new CountedList<DemandPair>() { dp2, dp3 } },
                { (node2, node4), new CountedList<DemandPair>() { dp2, dp3 } },
                { (node3, node5), new CountedList<DemandPair>() { dp2 } },
            };

            Random random = new Random(989871350);
            MulticutInstance instance = new MulticutInstance(tree, dps, 2, random);
            GuoNiedermeierFPT gnfpt = new GuoNiedermeierFPT(instance);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, random, demandPairsPerEdge);

            CountedList<(List<(TreeNode, TreeNode)>, DemandPair)> information = new CountedList<(List<(TreeNode, TreeNode)>, DemandPair)>()
            {
                (new List<(TreeNode, TreeNode)>(){ (node3, node6) }, dp3)
            };
            
            Assert.IsTrue(dominatedPath.AfterDemandPathChanged(information));
        }
    }
}
