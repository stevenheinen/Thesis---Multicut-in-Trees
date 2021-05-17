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
        private static readonly PerformanceMeasurements MockMeasurements = new(nameof(UnitTestAlgorithm));
        private static readonly Counter MockCounter = new();

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, demandPairs, 2, 2);
            GuoNiedermeierKernelisation gnfpt = new(instance);

            Assert.IsNotNull(gnfpt);
            Assert.IsNotNull(gnfpt.ReductionRules);
            Assert.AreNotEqual(0, gnfpt.ReductionRules.Count);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, demandPairs, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);
            Node node = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            tree.AddNode(node1, MockCounter);
            tree.AddNode(node2, MockCounter);
            Edge<Node> edge = new(node1, node2);
            tree.AddEdge(edge, MockCounter);
            DemandPair dp = new(0, node1, node2, tree);

            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierKernelisation g = new(null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(null, node, node, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(dp, null, node, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(dp, node, null, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPair(dp, node, node, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPairs(null, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ChangeEndpointOfDemandPairs(new CountedList<(DemandPair, Node, Node)>(), null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge(null, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdge(edge, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdges(null, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.ContractEdges(new CountedList<Edge<Node>>(), null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge(null, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdge(edge, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdges(null, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.CutEdges(new CountedList<Edge<Node>>(), null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPair(null, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPair(dp, null); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPairs(null, MockMeasurements); });
            Assert.ThrowsException<ArgumentNullException>(() => { gnfpt.RemoveDemandPairs(new CountedList<DemandPair>(), null); });
        }

        [TestMethod]
        public void TestDemandPathsPerEdge()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);
            
            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node3, node4, tree);
            DemandPair dp2 = new(2, node5, node2, tree);
            DemandPair dp3 = new(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);

            PropertyInfo dictProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> dict = (CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>)dictProperty.GetGetMethod(true).Invoke(gnfpt, Array.Empty<object>());
            Assert.AreEqual(2, dict[edge25, MockCounter].Count(MockCounter));
            Assert.AreEqual(1, dict[edge02, MockCounter].Count(MockCounter));
            Assert.AreEqual(2, dict[edge01, MockCounter].Count(MockCounter));
        }

        [TestMethod]
        public void TestRemoveDemandPairFromEdge()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node3, node4, tree);
            DemandPair dp2 = new(2, node5, node2, tree);
            DemandPair dp3 = new(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);

            PropertyInfo dictProperty = typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> dict = (CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>)dictProperty.GetGetMethod(true).Invoke(gnfpt, Array.Empty<object>());

            MethodInfo method = typeof(Algorithm).GetMethod("RemoveDemandPairFromEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(gnfpt, new object[] { edge01, dp3, MockMeasurements });
            Assert.AreEqual(1, dict[edge01, MockCounter].Count(MockCounter));
            Assert.AreEqual(dp1, dict[edge01, MockCounter].First(MockCounter));

            method.Invoke(gnfpt, new object[] { edge03, dp1, MockMeasurements });
            Assert.IsFalse(dict.ContainsKey(edge03, MockCounter));
        }

        [TestMethod]
        public void TestCutEdge()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node3, node4, tree);
            DemandPair dp2 = new(2, node5, node2, tree);
            DemandPair dp3 = new(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);
            gnfpt.CutEdge(edge25, MockMeasurements);

            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(1, ((CountedCollection<DemandPair>)demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, Array.Empty<object>())).Count(MockCounter));
        }

        [TestMethod]
        public void TestCutEdges()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node3, node4, tree);
            DemandPair dp2 = new(2, node5, node2, tree);
            DemandPair dp3 = new(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);
            gnfpt.CutEdges(new CountedList<Edge<Node>>(new List<Edge<Node>>() { edge01, edge25 }, MockCounter), MockMeasurements);

            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(0, ((CountedCollection<DemandPair>)demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, Array.Empty<object>())).Count(MockCounter));
        }

        [TestMethod]
        public void TestContractEdge()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node3, node4, tree);
            DemandPair dp2 = new(2, node5, node2, tree);
            DemandPair dp3 = new(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);
            gnfpt.ContractEdge(edge02, MockMeasurements);

            Assert.AreEqual(5, tree.NumberOfNodes(MockCounter));
        }

        [TestMethod]
        public void TestContractEdges()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node3, node4, tree);
            DemandPair dp2 = new(2, node5, node2, tree);
            DemandPair dp3 = new(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);
            gnfpt.ContractEdges(new CountedList<Edge<Node>>(new List<Edge<Node>>() { edge02, edge14 }, MockCounter), MockMeasurements);

            Assert.AreEqual(4, tree.NumberOfNodes(MockCounter));
        }

        [TestMethod]
        public void TestChangeEndpointOfDemandPair()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node3, node4, tree);
            DemandPair dp2 = new(2, node5, node2, tree);
            DemandPair dp3 = new(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);
            gnfpt.ChangeEndpointOfDemandPair(dp3, node5, node2, MockMeasurements);
            Assert.AreEqual(3, dp3.LengthOfPath(MockCounter));

            gnfpt.ChangeEndpointOfDemandPair(dp1, node4, node0, MockMeasurements);
            Assert.AreEqual(1, dp1.LengthOfPath(MockCounter));
        }

        [TestMethod]
        public void TestChangeEndpointOfDemandPairs()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge03 = new(node0, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node3, node4, tree);
            DemandPair dp2 = new(2, node5, node2, tree);
            DemandPair dp3 = new(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new(instance);
            gnfpt.ChangeEndpointOfDemandPairs(new CountedList<(DemandPair, Node, Node)>(new List<(DemandPair, Node, Node)>() { (dp3, node5, node2), (dp1, node4, node0) }, MockCounter), MockMeasurements);
            Assert.AreEqual(3, dp3.LengthOfPath(MockCounter));
            Assert.AreEqual(1, dp1.LengthOfPath(MockCounter));
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents1()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            Node node16 = new(16);
            Node node17 = new(17);
            Node node18 = new(18);
            Node node19 = new(19);
            Node node20 = new(20);
            Node node21 = new(21);
            Node node22 = new(22);
            Node node23 = new(23);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22, node23 }, MockCounter);

            Edge<Node> edge54 = new(node5, node4);
            Edge<Node> edge43 = new(node4, node3);
            Edge<Node> edge414 = new(node4, node14);
            Edge<Node> edge32 = new(node3, node2);
            Edge<Node> edge312 = new(node3, node12);
            Edge<Node> edge313 = new(node3, node13);
            Edge<Node> edge21 = new(node2, node1);
            Edge<Node> edge10 = new(node1, node0);
            Edge<Node> edge111 = new(node1, node11);
            Edge<Node> edge223 = new(node2, node23);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge59 = new(node5, node9);
            Edge<Node> edge515 = new(node5, node15);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge716 = new(node7, node16);
            Edge<Node> edge717 = new(node7, node17);
            Edge<Node> edge718 = new(node7, node18);
            Edge<Node> edge819 = new(node8, node19);
            Edge<Node> edge820 = new(node8, node20);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge921 = new(node9, node21);
            Edge<Node> edge1022 = new(node10, node22);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge223, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new(i);
            PerformanceMeasurements m = new("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());
            
            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge21, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            Assert.AreEqual(-1, resultDict[node2, MockCounter]);
            Assert.AreEqual(-1, resultDict[node23, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents2()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            Node node16 = new(16);
            Node node17 = new(17);
            Node node18 = new(18);
            Node node19 = new(19);
            Node node20 = new(20);
            Node node21 = new(21);
            Node node22 = new(22);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22 }, MockCounter);

            Edge<Node> edge54 = new(node5, node4);
            Edge<Node> edge43 = new(node4, node3);
            Edge<Node> edge414 = new(node4, node14);
            Edge<Node> edge32 = new(node3, node2);
            Edge<Node> edge312 = new(node3, node12);
            Edge<Node> edge313 = new(node3, node13);
            Edge<Node> edge21 = new(node2, node1);
            Edge<Node> edge10 = new(node1, node0);
            Edge<Node> edge111 = new(node1, node11);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge59 = new(node5, node9);
            Edge<Node> edge515 = new(node5, node15);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge716 = new(node7, node16);
            Edge<Node> edge717 = new(node7, node17);
            Edge<Node> edge718 = new(node7, node18);
            Edge<Node> edge819 = new(node8, node19);
            Edge<Node> edge820 = new(node8, node20);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge921 = new(node9, node21);
            Edge<Node> edge1022 = new(node10, node22);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 },
            };

            MulticutInstance i = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new(i);
            PerformanceMeasurements m = new("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge54, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            Assert.AreEqual(-1, resultDict[node14, MockCounter]);
            Assert.AreEqual(-1, resultDict[node4, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents3()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            Node node16 = new(16);
            Node node17 = new(17);
            Node node18 = new(18);
            Node node19 = new(19);
            Node node20 = new(20);
            Node node21 = new(21);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21 }, MockCounter);

            Edge<Node> edge54 = new(node5, node4);
            Edge<Node> edge43 = new(node4, node3);
            Edge<Node> edge414 = new(node4, node14);
            Edge<Node> edge32 = new(node3, node2);
            Edge<Node> edge312 = new(node3, node12);
            Edge<Node> edge313 = new(node3, node13);
            Edge<Node> edge21 = new(node2, node1);
            Edge<Node> edge10 = new(node1, node0);
            Edge<Node> edge111 = new(node1, node11);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge515 = new(node5, node15);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge716 = new(node7, node16);
            Edge<Node> edge717 = new(node7, node17);
            Edge<Node> edge718 = new(node7, node18);
            Edge<Node> edge819 = new(node8, node19);
            Edge<Node> edge820 = new(node8, node20);
            Edge<Node> edge59 = new(node5, node9);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge921 = new(node9, node21);
            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, -1 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, -1 },
            };

            MulticutInstance i = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new(i);
            PerformanceMeasurements m = new("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge59, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            Assert.AreEqual(resultDict[node3, MockCounter], resultDict[node7, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents4()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            Node node16 = new(16);
            Node node17 = new(17);
            Node node18 = new(18);
            Node node19 = new(19);
            Node node20 = new(20);
            Node node21 = new(21);
            Node node22 = new(22);
            Node node23 = new(23);
            Node node24 = new(24);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22, node23, node24 }, MockCounter);

            Edge<Node> edge54 = new(node5, node4);
            Edge<Node> edge43 = new(node4, node3);
            Edge<Node> edge414 = new(node4, node14);
            Edge<Node> edge32 = new(node3, node2);
            Edge<Node> edge312 = new(node3, node12);
            Edge<Node> edge313 = new(node3, node13);
            Edge<Node> edge21 = new(node2, node1);
            Edge<Node> edge10 = new(node1, node0);
            Edge<Node> edge111 = new(node1, node11);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge59 = new(node5, node9);
            Edge<Node> edge515 = new(node5, node15);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge716 = new(node7, node16);
            Edge<Node> edge717 = new(node7, node17);
            Edge<Node> edge718 = new(node7, node18);
            Edge<Node> edge819 = new(node8, node19);
            Edge<Node> edge820 = new(node8, node20);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge921 = new(node9, node21);
            Edge<Node> edge1022 = new(node10, node22);
            Edge<Node> edge523 = new(node5, node23);
            Edge<Node> edge2334 = new(node23, node24);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022, edge523, edge2334 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, -1 }, { node24, -1 },
            };

            MulticutInstance i = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new(i);
            PerformanceMeasurements m = new("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge523, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            Assert.AreNotEqual(resultDict[node4, MockCounter], resultDict[node6, MockCounter]);
            Assert.AreNotEqual(resultDict[node9, MockCounter], resultDict[node6, MockCounter]);
            Assert.AreNotEqual(resultDict[node4, MockCounter], resultDict[node9, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents5()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            Node node16 = new(16);
            Node node17 = new(17);
            Node node18 = new(18);
            Node node19 = new(19);
            Node node20 = new(20);
            Node node21 = new(21);
            Node node22 = new(22);
            Node node23 = new(23);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22, node23 }, MockCounter);

            Edge<Node> edge54 = new(node5, node4);
            Edge<Node> edge43 = new(node4, node3);
            Edge<Node> edge414 = new(node4, node14);
            Edge<Node> edge32 = new(node3, node2);
            Edge<Node> edge312 = new(node3, node12);
            Edge<Node> edge313 = new(node3, node13);
            Edge<Node> edge21 = new(node2, node1);
            Edge<Node> edge10 = new(node1, node0);
            Edge<Node> edge223 = new(node2, node23);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge59 = new(node5, node9);
            Edge<Node> edge515 = new(node5, node15);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge716 = new(node7, node16);
            Edge<Node> edge717 = new(node7, node17);
            Edge<Node> edge718 = new(node7, node18);
            Edge<Node> edge819 = new(node8, node19);
            Edge<Node> edge820 = new(node8, node20);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge921 = new(node9, node21);
            Edge<Node> edge1022 = new(node10, node22);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge223, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new(i);
            PerformanceMeasurements m = new("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge10, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            Assert.AreEqual(-1, resultDict[node2, MockCounter]);
            Assert.AreEqual(-1, resultDict[node23, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents6()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            Node node15 = new(15);
            Node node16 = new(16);
            Node node17 = new(17);
            Node node18 = new(18);
            Node node19 = new(19);
            Node node20 = new(20);
            Node node21 = new(21);
            Node node22 = new(22);
            Node node23 = new(23);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22, node23 }, MockCounter);

            Edge<Node> edge54 = new(node5, node4);
            Edge<Node> edge43 = new(node4, node3);
            Edge<Node> edge414 = new(node4, node14);
            Edge<Node> edge32 = new(node3, node2);
            Edge<Node> edge312 = new(node3, node12);
            Edge<Node> edge313 = new(node3, node13);
            Edge<Node> edge21 = new(node2, node1);
            Edge<Node> edge223 = new(node2, node23);
            Edge<Node> edge10 = new(node1, node0);
            Edge<Node> edge111 = new(node1, node11);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge59 = new(node5, node9);
            Edge<Node> edge515 = new(node5, node15);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge716 = new(node7, node16);
            Edge<Node> edge717 = new(node7, node17);
            Edge<Node> edge718 = new(node7, node18);
            Edge<Node> edge819 = new(node8, node19);
            Edge<Node> edge820 = new(node8, node20);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge921 = new(node9, node21);
            Edge<Node> edge1022 = new(node10, node22);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge223, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new(i);
            PerformanceMeasurements m = new("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge10, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, Array.Empty<object>());

            Assert.AreEqual(0, resultDict[node2, MockCounter]);
            Assert.AreEqual(0, resultDict[node23, MockCounter]);
        }
    }
}
