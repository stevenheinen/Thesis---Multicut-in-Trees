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
    public class UnitTestDominatedPath
    {
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);
            Assert.IsNotNull(dominatedPath);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(null, dps, gnfpt, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, null, gnfpt, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, null, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, null); });
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node0, node2, counter);
            tree.AddChild(node1, node3, counter);
            tree.AddChild(node1, node4, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node2, node1);
            DemandPair dp2 = new DemandPair(node2, node0);
            DemandPair dp3 = new DemandPair(node3, node4);
            DemandPair dp4 = new DemandPair(node1, node3);
            DemandPair dp5 = new DemandPair(node3, node1);

            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, counter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                { (node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>(){ dp1 }, counter) },
                { (node0, node2), new CountedCollection<DemandPair>(new List<DemandPair>(){ dp1, dp2 }, counter) },
                { (node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>(){ dp3, dp4, dp5 }, counter) },
                { (node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(){ dp3 }, counter) },
            }, counter);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);
            Assert.IsTrue(dominatedPath.RunFirstIteration());
            Assert.AreEqual(2, dps.Count(counter));
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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 }, counter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node3, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                { (node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, counter) },
                { (node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, counter) },
                { (node2, node3), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, counter) },
                { (node2, node4), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, counter) },
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeInformation = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>(new List<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>()
            {
                ((node3, node5), node3, new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, counter))
            }, counter);

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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 }, counter);
            tree.AddChild(node3, node6, counter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node3, node4);
            DemandPair dp3 = new DemandPair(node4, node6);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                { (node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, counter) },
                { (node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, counter) },
                { (node2, node3), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2, dp3 }, counter) },
                { (node2, node4), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2, dp3 }, counter) },
                { (node3, node6), new CountedCollection<DemandPair>(new List<DemandPair>() { dp3 }, counter) },
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> contractedEdgeInformation = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>(new List<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>()
            {
                ((node3, node5), node3, new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, counter) )
            }, counter);

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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 }, counter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node3, node4);
            DemandPair dp3 = new DemandPair(node1, node3);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                { (node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, counter) },
                { (node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, counter) },
                { (node2, node3), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, counter) },
                { (node2, node4), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, counter) },
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);

            CountedList<DemandPair> removedDemandpairs = new CountedList<DemandPair>(new List<DemandPair>() { dp3 }, counter);

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

            tree.AddRoot(node0, counter);
            tree.AddChild(node0, node1, counter);
            tree.AddChild(node1, node2, counter);
            tree.AddChildren(node2, new List<TreeNode>() { node3, node4 }, counter);
            tree.AddChild(node3, node5, counter);
            tree.AddChild(node3, node6, counter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node5, node4);
            DemandPair dp3 = new DemandPair(node4, node3);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                { (node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, counter) },
                { (node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, counter) },
                { (node2, node3), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2, dp3 }, counter) },
                { (node2, node4), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2, dp3 }, counter) },
                { (node3, node5), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, counter) },
            }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);

            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> information = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(new List<(CountedList<(TreeNode, TreeNode)>, DemandPair)>()
            {
                (new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)>(){ (node3, node6) }, counter), dp3)
            }, counter);

            Assert.IsTrue(dominatedPath.AfterDemandPathChanged(information));
        }
    }
}
