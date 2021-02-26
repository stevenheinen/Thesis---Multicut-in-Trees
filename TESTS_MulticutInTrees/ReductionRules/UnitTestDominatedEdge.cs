// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
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
    public class UnitTestDominatedEdge
    {
        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair } }
            };

            Random random = new Random(5744654);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsNotNull(dominatedEdge);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair } }
            };

            Random random = new Random(13895712);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(null, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, null, new GuoNiedermeierFPT(instance), random, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, null, random, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, null); });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);

            Assert.ThrowsException<ArgumentNullException>(() => dominatedEdge.AfterDemandPathChanged(null));
            Assert.ThrowsException<ArgumentNullException>(() => dominatedEdge.AfterDemandPathRemove(null));
            Assert.ThrowsException<ArgumentNullException>(() => dominatedEdge.AfterEdgeContraction(null));
        }

        [TestMethod]
        public void TestFirstIteration()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node3, node0);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1, demandPair2 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair1 } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair1, demandPair2 } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair1 } },
                {(node1, node3), new CountedList<DemandPair>(){ demandPair2 } }
            };

            Random random = new Random(48764);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.RunFirstIteration());

            DemandPair demandPair3 = new DemandPair(node1, node4);
            DemandPair demandPair4 = new DemandPair(node3, node1);
            DemandPair demandPair5 = new DemandPair(node0, node1);
            DemandPair demandPair6 = new DemandPair(node0, node2);
            CountedList<DemandPair> demandPairs2 = new CountedList<DemandPair>() { demandPair3, demandPair4, demandPair5, demandPair6 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge2 = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair6 } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair5 } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair3 } },
                {(node1, node3), new CountedList<DemandPair>(){ demandPair4 } }
            };

            dominatedEdge = new DominatedEdge(tree, demandPairs2, new GuoNiedermeierFPT(instance), random, demandPairPerEdge2);
            Assert.IsFalse(dominatedEdge.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathRemoved()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node3, node0);
            DemandPair demandPair3 = new DemandPair(node2, node1);
            DemandPair demandPair4 = new DemandPair(node2, node0);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            DemandPair demandPair6 = new DemandPair(node1, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1, demandPair2, demandPair4, demandPair5, demandPair6 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair1, demandPair4 } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair1, demandPair2 } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair1, demandPair6 } },
                {(node1, node3), new CountedList<DemandPair>(){ demandPair2, demandPair5 } }
            };

            Random random = new Random(74945);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathRemove(new CountedList<DemandPair>() { demandPair3 }));

            demandPairPerEdge[(node0, node2)].Remove(demandPair4);
            demandPairs.Remove(demandPair4);
            dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);

            Assert.IsTrue(dominatedEdge.AfterDemandPathRemove(new CountedList<DemandPair>() { demandPair4 }));
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

            tree.AddRoot(node0);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 });
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 });

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair3 = new DemandPair(node2, node0);
            DemandPair demandPair4 = new DemandPair(node2, node0);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            DemandPair demandPair6 = new DemandPair(node1, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1, demandPair3, demandPair4, demandPair5, demandPair6 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node2), new CountedList<DemandPair>(){ demandPair1, demandPair3, demandPair4 } },
                {(node0, node1), new CountedList<DemandPair>(){ demandPair1 } },
                {(node1, node4), new CountedList<DemandPair>(){ demandPair1, demandPair6 } },
                {(node1, node3), new CountedList<DemandPair>(){ demandPair5 } }
            };

            Random random = new Random(684332);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.AfterDemandPathChanged(new CountedList<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node1, node0) }, demandPair3) }));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0);
            tree.AddChild(node0, node1);
            tree.AddChild(node1, node2);
            tree.AddChild(node2, node3);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node1, node0);
            DemandPair demandPair2 = new DemandPair(node3, node2);
            DemandPair demandPair3 = new DemandPair(node2, node0);
            DemandPair demandPair4 = new DemandPair(node1, node3);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node1), new CountedList<DemandPair>(){ demandPair1, demandPair3 } },
                {(node1, node2), new CountedList<DemandPair>(){ demandPair3, demandPair4, demandPair5 } },
                {(node2, node3), new CountedList<DemandPair>(){ demandPair2, demandPair4, demandPair5 } }
            };

            Random random = new Random(65416845);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathChanged(new CountedList<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node1, node0) }, demandPair5) }));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged3()
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
            tree.AddChild(node2, node3);
            tree.AddChild(node3, node4);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node0, node3);
            DemandPair demandPair2 = new DemandPair(node1, node2);
            DemandPair demandPair3 = new DemandPair(node0, node1);
            DemandPair demandPair4 = new DemandPair(node2, node4);
            DemandPair demandPair5 = new DemandPair(node3, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node1), new CountedList<DemandPair>(){ demandPair1, demandPair3 } },
                {(node1, node2), new CountedList<DemandPair>(){ demandPair1, demandPair2 } },
                {(node2, node3), new CountedList<DemandPair>(){ demandPair1 } },
                {(node3, node4), new CountedList<DemandPair>(){ demandPair4, demandPair5 } }
            };

            Random random = new Random(654978);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.AfterDemandPathChanged(new CountedList<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node3, node2) }, demandPair4) }));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged4()
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
            tree.AddChild(node2, node3);
            tree.AddChild(node3, node4);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node0, node3);
            DemandPair demandPair2 = new DemandPair(node1, node2);
            DemandPair demandPair3 = new DemandPair(node0, node1);
            DemandPair demandPair4 = new DemandPair(node2, node4);
            DemandPair demandPair5 = new DemandPair(node3, node4);
            DemandPair demandPair6 = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node1), new CountedList<DemandPair>(){ demandPair1, demandPair3 } },
                {(node1, node2), new CountedList<DemandPair>(){ demandPair1, demandPair2 } },
                {(node2, node3), new CountedList<DemandPair>(){ demandPair1, demandPair6 } },
                {(node3, node4), new CountedList<DemandPair>(){ demandPair4, demandPair5, demandPair6 } }
            };

            Random random = new Random(68941);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathChanged(new CountedList<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node3, node2) }, demandPair4) }));
        }

        [TestMethod]
        public void TestAfterEdgeContraction()
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
            tree.AddChild(node2, node3);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node1, node0);
            DemandPair demandPair2 = new DemandPair(node3, node2);
            DemandPair demandPair3 = new DemandPair(node2, node0);
            DemandPair demandPair4 = new DemandPair(node1, node3);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 };

            CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>()
            {
                {(node0, node1), new CountedList<DemandPair>(){ demandPair1, demandPair3 } },
                {(node1, node2), new CountedList<DemandPair>(){ demandPair3, demandPair4, demandPair5 } },
                {(node2, node3), new CountedList<DemandPair>(){ demandPair2, demandPair4, demandPair5 } }
            };

            Random random = new Random(6984980);
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, 100, random);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(instance), random, demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterEdgeContraction(new CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)>() { ((node3, node4), node3, new CountedList<DemandPair>()) }));
        }
    }
}
