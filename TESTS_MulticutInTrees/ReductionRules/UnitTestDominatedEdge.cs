// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestDominatedEdge
    {
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(null, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, null, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), null); });

            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);

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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node3, node0);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1 }, counter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair2 }, counter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1 }, counter) },
                {(node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair2 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.RunFirstIteration());

            DemandPair demandPair3 = new DemandPair(node1, node4);
            DemandPair demandPair4 = new DemandPair(node3, node1);
            DemandPair demandPair5 = new DemandPair(node0, node1);
            DemandPair demandPair6 = new DemandPair(node0, node2);
            CountedList<DemandPair> demandPairs2 = new CountedList<DemandPair>(new List<DemandPair>() { demandPair3, demandPair4, demandPair5, demandPair6 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge2 = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair6 }, counter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair5 }, counter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair3 }, counter) },
                {(node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair4 }, counter) }
            }, counter);

            dominatedEdge = new DominatedEdge(tree, demandPairs2, new GuoNiedermeierKernelisation(instance), demandPairPerEdge2);
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair2 = new DemandPair(node3, node0);
            DemandPair demandPair3 = new DemandPair(node2, node1);
            DemandPair demandPair4 = new DemandPair(node2, node0);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            DemandPair demandPair6 = new DemandPair(node1, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair4, demandPair5, demandPair6 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair4 }, counter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair2 }, counter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair6 }, counter) },
                {(node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair2, demandPair5 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathRemove(new CountedList<DemandPair>(new List<DemandPair>() { demandPair3 }, counter)));

            demandPairPerEdge[(node0, node2), counter].Remove(demandPair4, counter);
            demandPairs.Remove(demandPair4, counter);
            dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);

            Assert.IsTrue(dominatedEdge.AfterDemandPathRemove(new CountedList<DemandPair>(new List<DemandPair>() { demandPair4 }, counter)));
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

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2 }, counter);
            tree.AddChildren(node1, new List<TreeNode>() { node3, node4 }, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node2, node4);
            DemandPair demandPair3 = new DemandPair(node2, node0);
            DemandPair demandPair4 = new DemandPair(node2, node0);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            DemandPair demandPair6 = new DemandPair(node1, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair3, demandPair4, demandPair5, demandPair6 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair3, demandPair4 }, counter) },
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1 }, counter) },
                {(node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair6 }, counter) },
                {(node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair5 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.AfterDemandPathChanged(new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>() { (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)>() { (node1, node0) }, counter), demandPair3) }, counter)));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();

            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChild(node2, node3, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node1, node0);
            DemandPair demandPair2 = new DemandPair(node3, node2);
            DemandPair demandPair3 = new DemandPair(node2, node0);
            DemandPair demandPair4 = new DemandPair(node1, node3);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair3 }, counter) },
                {(node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair3, demandPair4, demandPair5 }, counter) },
                {(node2, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair2, demandPair4, demandPair5 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathChanged(new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>() { (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)>() { (node1, node0) }, counter), demandPair3) }, counter)));
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChild(node2, node3, counter);
            tree.AddChild(node3, node4, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node0, node3);
            DemandPair demandPair2 = new DemandPair(node1, node2);
            DemandPair demandPair3 = new DemandPair(node0, node1);
            DemandPair demandPair4 = new DemandPair(node2, node4);
            DemandPair demandPair5 = new DemandPair(node3, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair3 }, counter) },
                {(node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair2 }, counter) },
                {(node2, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1 }, counter) },
                {(node3, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair4, demandPair5 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsTrue(dominatedEdge.AfterDemandPathChanged(new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>() { (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)>() { (node3, node2) }, counter), demandPair4) }, counter)));
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChild(node2, node3, counter);
            tree.AddChild(node3, node4, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node0, node3);
            DemandPair demandPair2 = new DemandPair(node1, node2);
            DemandPair demandPair3 = new DemandPair(node0, node1);
            DemandPair demandPair4 = new DemandPair(node2, node4);
            DemandPair demandPair5 = new DemandPair(node3, node4);
            DemandPair demandPair6 = new DemandPair(node2, node4);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair3 }, counter) },
                {(node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair2 }, counter) },
                {(node2, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair6 }, counter) },
                {(node3, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair4, demandPair5, demandPair6 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterDemandPathChanged(new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>() { (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)>() { (node3, node2) }, counter), demandPair4) }, counter)));
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChild(node2, node3, counter);

            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(node1, node0);
            DemandPair demandPair2 = new DemandPair(node3, node2);
            DemandPair demandPair3 = new DemandPair(node2, node0);
            DemandPair demandPair4 = new DemandPair(node1, node3);
            DemandPair demandPair5 = new DemandPair(node1, node3);
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                {(node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair1, demandPair3 }, counter) },
                {(node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair3, demandPair4, demandPair5 }, counter) },
                {(node2, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair2, demandPair4, demandPair5 }, counter) }
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsFalse(dominatedEdge.AfterEdgeContraction(new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>(new List<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>() { ((node3, node4), node3, new CountedCollection<DemandPair>()) }, counter)));
        }
    }
}
