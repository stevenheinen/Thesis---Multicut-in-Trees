// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair } },
                {(node0, node1), new List<DemandPair>(){ demandPair } },
                {(node1, node4), new List<DemandPair>(){ demandPair } }
            };

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair } },
                {(node0, node1), new List<DemandPair>(){ demandPair } },
                {(node1, node4), new List<DemandPair>(){ demandPair } }
            };

            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(null, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, null, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), null); });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);

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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1, demandPair2 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair1 } },
                {(node0, node1), new List<DemandPair>(){ demandPair1, demandPair2 } },
                {(node1, node4), new List<DemandPair>(){ demandPair1 } },
                {(node1, node3), new List<DemandPair>(){ demandPair2 } }
            };

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.RunFirstIteration());

            DemandPair demandPair3 = new DemandPair(node1, node4);
            DemandPair demandPair4 = new DemandPair(node3, node1);
            DemandPair demandPair5 = new DemandPair(node0, node1);
            DemandPair demandPair6 = new DemandPair(node0, node2);
            List<DemandPair> demandPairs2 = new List<DemandPair>() { demandPair3, demandPair4, demandPair5, demandPair6 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge2 = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair6 } },
                {(node0, node1), new List<DemandPair>(){ demandPair5 } },
                {(node1, node4), new List<DemandPair>(){ demandPair3 } },
                {(node1, node3), new List<DemandPair>(){ demandPair4 } }
            };

            dominatedEdge = new DominatedEdge(tree, demandPairs2, new GuoNiedermeierFPT(tree, demandPairs2, 100), demandPairPerEdge2);
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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1, demandPair2, demandPair4, demandPair5, demandPair6 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair1, demandPair4 } },
                {(node0, node1), new List<DemandPair>(){ demandPair1, demandPair2 } },
                {(node1, node4), new List<DemandPair>(){ demandPair1, demandPair6 } },
                {(node1, node3), new List<DemandPair>(){ demandPair2, demandPair5 } }
            };

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathRemove(new List<DemandPair>() { demandPair3 }));

            demandPairPerEdge[(node0, node2)].Remove(demandPair4);
            demandPairs.Remove(demandPair4);
            dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);

            Assert.IsTrue(dominatedEdge.AfterDemandPathRemove(new List<DemandPair>() { demandPair4 }));
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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1, demandPair3, demandPair4, demandPair5, demandPair6 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node2), new List<DemandPair>(){ demandPair1, demandPair3, demandPair4 } },
                {(node0, node1), new List<DemandPair>(){ demandPair1 } },
                {(node1, node4), new List<DemandPair>(){ demandPair1, demandPair6 } },
                {(node1, node3), new List<DemandPair>(){ demandPair5 } }
            };

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.AfterDemandPathChanged(new List<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node1, node0) }, demandPair3) }));
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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node1), new List<DemandPair>(){ demandPair1, demandPair3 } },
                {(node1, node2), new List<DemandPair>(){ demandPair3, demandPair4, demandPair5 } },
                {(node2, node3), new List<DemandPair>(){ demandPair2, demandPair4, demandPair5 } }
            };

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathChanged(new List<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node1, node0) }, demandPair5) }));
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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node1), new List<DemandPair>(){ demandPair1, demandPair3 } },
                {(node1, node2), new List<DemandPair>(){ demandPair1, demandPair2 } },
                {(node2, node3), new List<DemandPair>(){ demandPair1 } },
                {(node3, node4), new List<DemandPair>(){ demandPair4, demandPair5 } }
            };

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.AfterDemandPathChanged(new List<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node3, node2) }, demandPair4) }));
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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node1), new List<DemandPair>(){ demandPair1, demandPair3 } },
                {(node1, node2), new List<DemandPair>(){ demandPair1, demandPair2 } },
                {(node2, node3), new List<DemandPair>(){ demandPair1, demandPair6 } },
                {(node3, node4), new List<DemandPair>(){ demandPair4, demandPair5, demandPair6 } }
            };

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathChanged(new List<(List<(TreeNode, TreeNode)>, DemandPair)>() { (new List<(TreeNode, TreeNode)>() { (node3, node2) }, demandPair4) }));
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
            List<DemandPair> demandPairs = new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 };

            Dictionary<(TreeNode, TreeNode), List<DemandPair>> demandPairPerEdge = new Dictionary<(TreeNode, TreeNode), List<DemandPair>>()
            {
                {(node0, node1), new List<DemandPair>(){ demandPair1, demandPair3 } },
                {(node1, node2), new List<DemandPair>(){ demandPair3, demandPair4, demandPair5 } },
                {(node2, node3), new List<DemandPair>(){ demandPair2, demandPair4, demandPair5 } }
            };

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierFPT(tree, demandPairs, 100), demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterEdgeContraction(new List<((TreeNode, TreeNode), TreeNode, List<DemandPair>)>() { ((node3, node4), node3, new List<DemandPair>()) }));
        }

    }
}
