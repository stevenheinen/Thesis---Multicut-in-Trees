// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.Algorithms
{

    [TestClass]
    public class UnitTestAlgorithm
    {
        private static readonly PerformanceMeasurements measurements = new PerformanceMeasurements(nameof(UnitTestGuoNiedermeierKernelisation));
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 2, 2);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            Assert.IsNotNull(gnfpt);
            Assert.IsNotNull(gnfpt.ReductionRules);
            Assert.AreNotEqual(0, gnfpt.ReductionRules.Count);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            TreeNode node = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            DemandPair dp = new DemandPair(0, node1, node2);

            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierKernelisation g = new GuoNiedermeierKernelisation(null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(null, node, node, measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(dp, null, node, measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(dp, node, null, measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(dp, node, node, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPairs(null, measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPairs(new CountedList<(DemandPair, TreeNode, TreeNode)>(), null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge((null, node), measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge((node, null), measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge((null, null), measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge((node, node), null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdges(null, measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdges(new CountedList<(TreeNode, TreeNode)>(), null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge((null, node), measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge((node, null), measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge((null, null), measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge((node, node), null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdges(null, measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdges(new CountedList<(TreeNode, TreeNode)>(), null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPair(null, measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPair(dp, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPairs(null, measurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPairs(new CountedList<DemandPair>(), null); });
        }

        [TestMethod]
        public void TestDemandPathsPerEdge()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChild(node1, node4, counter);
            tree.AddChild(node2, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node2);
            DemandPair dp3 = new DemandPair(3, node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            PropertyInfo dictProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> dict = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)dictProperty.GetGetMethod(true).Invoke(gnfpt, new object[0]);
            Assert.AreEqual(2, dict[(node2, node5), counter].Count(counter));
            Assert.AreEqual(1, dict[(node0, node2), counter].Count(counter));
            Assert.AreEqual(2, dict[(node0, node1), counter].Count(counter));
        }

        [TestMethod]
        public void TestRemoveDemandPairFromEdge()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChild(node1, node4, counter);
            tree.AddChild(node2, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node2);
            DemandPair dp3 = new DemandPair(3, node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            PropertyInfo dictProperty = typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> dict = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)dictProperty.GetGetMethod(true).Invoke(gnfpt, new object[0]);

            MethodInfo method = typeof(Algorithm).GetMethod("RemoveDemandPairFromEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(gnfpt, new object[] { (node1, node0), dp3, measurements });
            Assert.AreEqual(1, dict[(node0, node1), counter].Count(counter));
            Assert.AreEqual(dp1, dict[(node0, node1), counter].First(counter));

            method.Invoke(gnfpt, new object[] { (node0, node3), dp1, measurements });
            Assert.IsFalse(dict.ContainsKey((node0, node3), counter));
        }

        [TestMethod]
        public void TestCutEdge()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChild(node1, node4, counter);
            tree.AddChild(node2, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node2);
            DemandPair dp3 = new DemandPair(3, node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.CutEdge((node5, node2), measurements);

            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(1, ((CountedList<DemandPair>)demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, new object[] { })).Count(counter));
        }

        [TestMethod]
        public void TestCutEdges()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChild(node1, node4, counter);
            tree.AddChild(node2, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node2);
            DemandPair dp3 = new DemandPair(3, node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.CutEdges(new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)>() { (node0, node1), (node5, node2) }, counter), measurements);

            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(0, ((CountedList<DemandPair>)demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, new object[] { })).Count(counter));
        }

        [TestMethod]
        public void TestContractEdge()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChild(node1, node4, counter);
            tree.AddChild(node2, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node2);
            DemandPair dp3 = new DemandPair(3, node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.ContractEdge((node0, node2), measurements);

            Assert.AreEqual(5, tree.NumberOfNodes(counter));
        }

        [TestMethod]
        public void TestContractEdges()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChild(node1, node4, counter);
            tree.AddChild(node2, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node2);
            DemandPair dp3 = new DemandPair(3, node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.ContractEdges(new CountedList<(TreeNode, TreeNode)>(new List<(TreeNode, TreeNode)>() { (node0, node2), (node4, node1) }, counter), measurements);

            Assert.AreEqual(4, tree.NumberOfNodes(counter));
        }

        [TestMethod]
        public void TestChangeEndpointOfDemandPair()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChild(node1, node4, counter);
            tree.AddChild(node2, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node2);
            DemandPair dp3 = new DemandPair(3, node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.ChangeEndpointOfDemandPair(dp3, node5, node2, measurements);
            Assert.AreEqual(3, dp3.LengthOfPath(counter));

            gnfpt.ChangeEndpointOfDemandPair(dp1, node4, node0, measurements);
            Assert.AreEqual(1, dp1.LengthOfPath(counter));
        }

        [TestMethod]
        public void TestChangeEndpointOfDemandPairs()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);

            tree.AddRoot(node0, counter);
            tree.AddChildren(node0, new List<TreeNode>() { node1, node2, node3 }, counter);
            tree.AddChild(node1, node4, counter);
            tree.AddChild(node2, node5, counter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4);
            DemandPair dp2 = new DemandPair(2, node5, node2);
            DemandPair dp3 = new DemandPair(3, node5, node4);
            CountedList<DemandPair> dps = new CountedList<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, counter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.ChangeEndpointOfDemandPairs(new CountedList<(DemandPair, TreeNode, TreeNode)>(new List<(DemandPair, TreeNode, TreeNode)>() { (dp3, node5, node2), (dp1, node4, node0) }, counter), measurements);
            Assert.AreEqual(3, dp3.LengthOfPath(counter));
            Assert.AreEqual(1, dp1.LengthOfPath(counter));
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);
            TreeNode node11 = new TreeNode(11);
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            TreeNode node15 = new TreeNode(15);
            TreeNode node16 = new TreeNode(16);
            TreeNode node17 = new TreeNode(17);
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);
            TreeNode node23 = new TreeNode(23);

            tree.AddRoot(node5, counter);
            tree.AddChild(node5, node4, counter);
            tree.AddChild(node4, node3, counter);
            tree.AddChild(node4, node14, counter);
            tree.AddChild(node3, node2, counter);
            tree.AddChild(node3, node12, counter);
            tree.AddChild(node3, node13, counter);
            tree.AddChild(node2, node1, counter);
            tree.AddChild(node1, node0, counter);
            tree.AddChild(node1, node11, counter);
            tree.AddChild(node2, node23, counter);
            tree.AddChild(node5, node6, counter);
            tree.AddChild(node5, node9, counter);
            tree.AddChild(node5, node15, counter);
            tree.AddChild(node6, node7, counter);
            tree.AddChild(node7, node8, counter);
            tree.AddChild(node7, node16, counter);
            tree.AddChild(node7, node17, counter);
            tree.AddChild(node7, node18, counter);
            tree.AddChild(node8, node19, counter);
            tree.AddChild(node8, node20, counter);
            tree.AddChild(node9, node10, counter);
            tree.AddChild(node9, node21, counter);
            tree.AddChild(node10, node22, counter);

            tree.UpdateNodeTypes();
            Dictionary<TreeNode, int> caterpillars = new Dictionary<TreeNode, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedList<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });
            
            caterpillarComponentPerNode.Clear(counter);
            foreach (KeyValuePair<TreeNode, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, counter);
            }

            a.ContractEdge((node2, node1), m);

            CountedDictionary<TreeNode, int> resultDict = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(-1, resultDict[node2, counter]);
            Assert.AreEqual(-1, resultDict[node23, counter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);
            TreeNode node11 = new TreeNode(11);
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            TreeNode node15 = new TreeNode(15);
            TreeNode node16 = new TreeNode(16);
            TreeNode node17 = new TreeNode(17);
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);

            tree.AddRoot(node5, counter);
            tree.AddChild(node5, node4, counter);
            tree.AddChild(node4, node3, counter);
            tree.AddChild(node4, node14, counter);
            tree.AddChild(node3, node2, counter);
            tree.AddChild(node3, node12, counter);
            tree.AddChild(node3, node13, counter);
            tree.AddChild(node2, node1, counter);
            tree.AddChild(node1, node0, counter);
            tree.AddChild(node1, node11, counter);
            tree.AddChild(node5, node6, counter);
            tree.AddChild(node5, node9, counter);
            tree.AddChild(node5, node15, counter);
            tree.AddChild(node6, node7, counter);
            tree.AddChild(node7, node8, counter);
            tree.AddChild(node7, node16, counter);
            tree.AddChild(node7, node17, counter);
            tree.AddChild(node7, node18, counter);
            tree.AddChild(node8, node19, counter);
            tree.AddChild(node8, node20, counter);
            tree.AddChild(node9, node10, counter);
            tree.AddChild(node9, node21, counter);
            tree.AddChild(node10, node22, counter);

            tree.UpdateNodeTypes();
            Dictionary<TreeNode, int> caterpillars = new Dictionary<TreeNode, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedList<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(counter);
            foreach (KeyValuePair<TreeNode, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, counter);
            }

            a.ContractEdge((node4, node5), m);

            CountedDictionary<TreeNode, int> resultDict = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(-1, resultDict[node14, counter]);
            Assert.AreEqual(-1, resultDict[node4, counter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents3()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);
            TreeNode node11 = new TreeNode(11);
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            TreeNode node15 = new TreeNode(15);
            TreeNode node16 = new TreeNode(16);
            TreeNode node17 = new TreeNode(17);
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);

            tree.AddRoot(node5, counter);
            tree.AddChild(node5, node4, counter);
            tree.AddChild(node4, node3, counter);
            tree.AddChild(node4, node14, counter);
            tree.AddChild(node3, node2, counter);
            tree.AddChild(node3, node12, counter);
            tree.AddChild(node3, node13, counter);
            tree.AddChild(node2, node1, counter);
            tree.AddChild(node1, node0, counter);
            tree.AddChild(node1, node11, counter);
            tree.AddChild(node5, node6, counter);
            tree.AddChild(node5, node15, counter);
            tree.AddChild(node6, node7, counter);
            tree.AddChild(node7, node8, counter);
            tree.AddChild(node7, node16, counter);
            tree.AddChild(node7, node17, counter);
            tree.AddChild(node7, node18, counter);
            tree.AddChild(node8, node19, counter);
            tree.AddChild(node8, node20, counter);
            tree.AddChild(node5, node9, counter);
            tree.AddChild(node9, node10, counter);
            tree.AddChild(node9, node21, counter);

            tree.UpdateNodeTypes();
            Dictionary<TreeNode, int> caterpillars = new Dictionary<TreeNode, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, -1 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, -1 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedList<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(counter);
            foreach (KeyValuePair<TreeNode, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, counter);
            }

            a.ContractEdge((node5, node9), m);

            CountedDictionary<TreeNode, int> resultDict = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(resultDict[node3, counter], resultDict[node7, counter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents4()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);
            TreeNode node11 = new TreeNode(11);
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            TreeNode node15 = new TreeNode(15);
            TreeNode node16 = new TreeNode(16);
            TreeNode node17 = new TreeNode(17);
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);
            TreeNode node23 = new TreeNode(23);
            TreeNode node24 = new TreeNode(24);

            tree.AddRoot(node5, counter);
            tree.AddChild(node5, node4, counter);
            tree.AddChild(node4, node3, counter);
            tree.AddChild(node4, node14, counter);
            tree.AddChild(node3, node2, counter);
            tree.AddChild(node3, node12, counter);
            tree.AddChild(node3, node13, counter);
            tree.AddChild(node2, node1, counter);
            tree.AddChild(node1, node0, counter);
            tree.AddChild(node1, node11, counter);
            tree.AddChild(node5, node6, counter);
            tree.AddChild(node5, node9, counter);
            tree.AddChild(node5, node15, counter);
            tree.AddChild(node6, node7, counter);
            tree.AddChild(node7, node8, counter);
            tree.AddChild(node7, node16, counter);
            tree.AddChild(node7, node17, counter);
            tree.AddChild(node7, node18, counter);
            tree.AddChild(node8, node19, counter);
            tree.AddChild(node8, node20, counter);
            tree.AddChild(node9, node10, counter);
            tree.AddChild(node9, node21, counter);
            tree.AddChild(node10, node22, counter);
            tree.AddChild(node5, node23, counter);
            tree.AddChild(node23, node24, counter);

            tree.UpdateNodeTypes();
            Dictionary<TreeNode, int> caterpillars = new Dictionary<TreeNode, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, -1 }, { node24, -1 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedList<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(counter);
            foreach (KeyValuePair<TreeNode, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, counter);
            }

            a.ContractEdge((node5, node23), m);

            CountedDictionary<TreeNode, int> resultDict = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreNotEqual(resultDict[node4, counter], resultDict[node6, counter]);
            Assert.AreNotEqual(resultDict[node9, counter], resultDict[node6, counter]);
            Assert.AreNotEqual(resultDict[node4, counter], resultDict[node9, counter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents5()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            TreeNode node15 = new TreeNode(15);
            TreeNode node16 = new TreeNode(16);
            TreeNode node17 = new TreeNode(17);
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);
            TreeNode node23 = new TreeNode(23);

            tree.AddRoot(node5, counter);
            tree.AddChild(node5, node4, counter);
            tree.AddChild(node4, node3, counter);
            tree.AddChild(node4, node14, counter);
            tree.AddChild(node3, node2, counter);
            tree.AddChild(node3, node12, counter);
            tree.AddChild(node3, node13, counter);
            tree.AddChild(node2, node1, counter);
            tree.AddChild(node1, node0, counter);
            tree.AddChild(node2, node23, counter);
            tree.AddChild(node5, node6, counter);
            tree.AddChild(node5, node9, counter);
            tree.AddChild(node5, node15, counter);
            tree.AddChild(node6, node7, counter);
            tree.AddChild(node7, node8, counter);
            tree.AddChild(node7, node16, counter);
            tree.AddChild(node7, node17, counter);
            tree.AddChild(node7, node18, counter);
            tree.AddChild(node8, node19, counter);
            tree.AddChild(node8, node20, counter);
            tree.AddChild(node9, node10, counter);
            tree.AddChild(node9, node21, counter);
            tree.AddChild(node10, node22, counter);

            tree.UpdateNodeTypes();
            Dictionary<TreeNode, int> caterpillars = new Dictionary<TreeNode, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedList<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(counter);
            foreach (KeyValuePair<TreeNode, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, counter);
            }

            a.ContractEdge((node0, node1), m);

            CountedDictionary<TreeNode, int> resultDict = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(-1, resultDict[node2, counter]);
            Assert.AreEqual(-1, resultDict[node23, counter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents6()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            TreeNode node7 = new TreeNode(7);
            TreeNode node8 = new TreeNode(8);
            TreeNode node9 = new TreeNode(9);
            TreeNode node10 = new TreeNode(10);
            TreeNode node11 = new TreeNode(11);
            TreeNode node12 = new TreeNode(12);
            TreeNode node13 = new TreeNode(13);
            TreeNode node14 = new TreeNode(14);
            TreeNode node15 = new TreeNode(15);
            TreeNode node16 = new TreeNode(16);
            TreeNode node17 = new TreeNode(17);
            TreeNode node18 = new TreeNode(18);
            TreeNode node19 = new TreeNode(19);
            TreeNode node20 = new TreeNode(20);
            TreeNode node21 = new TreeNode(21);
            TreeNode node22 = new TreeNode(22);
            TreeNode node23 = new TreeNode(23);

            tree.AddRoot(node5, counter);
            tree.AddChild(node5, node4, counter);
            tree.AddChild(node4, node3, counter);
            tree.AddChild(node4, node14, counter);
            tree.AddChild(node3, node2, counter);
            tree.AddChild(node3, node12, counter);
            tree.AddChild(node3, node13, counter);
            tree.AddChild(node2, node1, counter);
            tree.AddChild(node2, node23, counter);
            tree.AddChild(node1, node0, counter);
            tree.AddChild(node1, node11, counter);
            tree.AddChild(node5, node6, counter);
            tree.AddChild(node5, node9, counter);
            tree.AddChild(node5, node15, counter);
            tree.AddChild(node6, node7, counter);
            tree.AddChild(node7, node8, counter);
            tree.AddChild(node7, node16, counter);
            tree.AddChild(node7, node17, counter);
            tree.AddChild(node7, node18, counter);
            tree.AddChild(node8, node19, counter);
            tree.AddChild(node8, node20, counter);
            tree.AddChild(node9, node10, counter);
            tree.AddChild(node9, node21, counter);
            tree.AddChild(node10, node22, counter);

            tree.UpdateNodeTypes();
            Dictionary<TreeNode, int> caterpillars = new Dictionary<TreeNode, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedList<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<TreeNode, int> caterpillarComponentPerNode = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(counter);
            foreach (KeyValuePair<TreeNode, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, counter);
            }

            a.ContractEdge((node0, node1), m);

            CountedDictionary<TreeNode, int> resultDict = (CountedDictionary<TreeNode, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(0, resultDict[node2, counter]);
            Assert.AreEqual(0, resultDict[node23, counter]);
        }
    }
}
