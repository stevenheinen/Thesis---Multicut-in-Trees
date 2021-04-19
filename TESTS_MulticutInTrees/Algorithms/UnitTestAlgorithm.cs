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
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestAlgorithm));
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new Graph();
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 2, 2);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            Assert.IsNotNull(gnfpt);
            Assert.IsNotNull(gnfpt.ReductionRules);
            Assert.AreNotEqual(0, gnfpt.ReductionRules.Count);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new Graph();
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            Node node = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            tree.AddNode(node1, MockCounter);
            tree.AddNode(node2, MockCounter);
            Edge<Node> edge = new Edge<Node>(node1, node2);
            tree.AddEdge(edge, MockCounter);
            DemandPair dp = new DemandPair(0, node1, node2, tree);

            Assert.ThrowsException<ArgumentNullException>(() => { GuoNiedermeierKernelisation g = new GuoNiedermeierKernelisation(null); });
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
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);
            
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node2, tree);
            DemandPair dp3 = new DemandPair(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            PropertyInfo dictProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> dict = (CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>)dictProperty.GetGetMethod(true).Invoke(gnfpt, new object[0]);
            Assert.AreEqual(2, dict[edge25, MockCounter].Count(MockCounter));
            Assert.AreEqual(1, dict[edge02, MockCounter].Count(MockCounter));
            Assert.AreEqual(2, dict[edge01, MockCounter].Count(MockCounter));
        }

        [TestMethod]
        public void TestRemoveDemandPairFromEdge()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node2, tree);
            DemandPair dp3 = new DemandPair(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);

            PropertyInfo dictProperty = typeof(Algorithm).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> dict = (CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>)dictProperty.GetGetMethod(true).Invoke(gnfpt, new object[0]);

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
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node2, tree);
            DemandPair dp3 = new DemandPair(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.CutEdge(edge25, MockMeasurements);

            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(1, ((CountedCollection<DemandPair>)demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, new object[] { })).Count(MockCounter));
        }

        [TestMethod]
        public void TestCutEdges()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node2, tree);
            DemandPair dp3 = new DemandPair(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.CutEdges(new CountedList<Edge<Node>>(new List<Edge<Node>>() { edge01, edge25 }, MockCounter), MockMeasurements);

            PropertyInfo demandPairsProperty = typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(0, ((CountedCollection<DemandPair>)demandPairsProperty.GetGetMethod(true).Invoke(gnfpt, new object[] { })).Count(MockCounter));
        }

        [TestMethod]
        public void TestContractEdge()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node2, tree);
            DemandPair dp3 = new DemandPair(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.ContractEdge(edge02, MockMeasurements);

            Assert.AreEqual(5, tree.NumberOfNodes(MockCounter));
        }

        [TestMethod]
        public void TestContractEdges()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node2, tree);
            DemandPair dp3 = new DemandPair(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.ContractEdges(new CountedList<Edge<Node>>(new List<Edge<Node>>() { edge02, edge14 }, MockCounter), MockMeasurements);

            Assert.AreEqual(4, tree.NumberOfNodes(MockCounter));
        }

        [TestMethod]
        public void TestChangeEndpointOfDemandPair()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node2, tree);
            DemandPair dp3 = new DemandPair(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.ChangeEndpointOfDemandPair(dp3, node5, node2, MockMeasurements);
            Assert.AreEqual(3, dp3.LengthOfPath(MockCounter));

            gnfpt.ChangeEndpointOfDemandPair(dp1, node4, node0, MockMeasurements);
            Assert.AreEqual(1, dp1.LengthOfPath(MockCounter));
        }

        [TestMethod]
        public void TestChangeEndpointOfDemandPairs()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);

            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge03 = new Edge<Node>(node0, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);

            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge03, edge14, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node2, tree);
            DemandPair dp3 = new DemandPair(3, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 100, 100);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            gnfpt.ChangeEndpointOfDemandPairs(new CountedList<(DemandPair, Node, Node)>(new List<(DemandPair, Node, Node)>() { (dp3, node5, node2), (dp1, node4, node0) }, MockCounter), MockMeasurements);
            Assert.AreEqual(3, dp3.LengthOfPath(MockCounter));
            Assert.AreEqual(1, dp1.LengthOfPath(MockCounter));
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents1()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);
            Node node23 = new Node(23);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22, node23 }, MockCounter);

            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge223 = new Edge<Node>(node2, node23);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge223, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new Dictionary<Node, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });
            
            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge21, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(-1, resultDict[node2, MockCounter]);
            Assert.AreEqual(-1, resultDict[node23, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents2()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22 }, MockCounter);

            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new Dictionary<Node, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge54, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(-1, resultDict[node14, MockCounter]);
            Assert.AreEqual(-1, resultDict[node4, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents3()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21 }, MockCounter);

            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new Dictionary<Node, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, -1 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, -1 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge59, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(resultDict[node3, MockCounter], resultDict[node7, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents4()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);
            Node node23 = new Node(23);
            Node node24 = new Node(24);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22, node23, node24 }, MockCounter);

            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);
            Edge<Node> edge523 = new Edge<Node>(node5, node23);
            Edge<Node> edge2334 = new Edge<Node>(node23, node24);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022, edge523, edge2334 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new Dictionary<Node, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, -1 }, { node24, -1 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge523, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreNotEqual(resultDict[node4, MockCounter], resultDict[node6, MockCounter]);
            Assert.AreNotEqual(resultDict[node9, MockCounter], resultDict[node6, MockCounter]);
            Assert.AreNotEqual(resultDict[node4, MockCounter], resultDict[node9, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents5()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);
            Node node23 = new Node(23);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22, node23 }, MockCounter);

            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge223 = new Edge<Node>(node2, node23);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge223, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new Dictionary<Node, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge10, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(-1, resultDict[node2, MockCounter]);
            Assert.AreEqual(-1, resultDict[node23, MockCounter]);
        }

        [TestMethod]
        public void TestUpdateCaterpillarCompontents6()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);
            Node node23 = new Node(23);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22, node23 }, MockCounter);

            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge223 = new Edge<Node>(node2, node23);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);

            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge223, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);

            tree.UpdateNodeTypes();
            Dictionary<Node, int> caterpillars = new Dictionary<Node, int>()
            {
                { node0, -1 }, { node1, -1 }, { node2, 0 }, { node3, 0 }, { node4, 0 }, { node5, -1 }, { node6, 1 }, { node7, 1 }, { node8, -1 }, { node9, 2 }, { node10, -1 }, { node11, -1 }, { node12, 0 }, { node13, 0 }, { node14, 0 }, { node15, -1 }, { node16, 1 }, { node17, 1 }, { node18, 1 }, { node19, -1 }, { node20, -1 }, { node21, 2 }, { node22, -1 }, { node23, 0 },
            };

            MulticutInstance i = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, new CountedCollection<DemandPair>(), 200, 200);
            GuoNiedermeierKernelisation a = new GuoNiedermeierKernelisation(i);
            PerformanceMeasurements m = new PerformanceMeasurements("bn");

            PropertyInfo dict = typeof(GuoNiedermeierKernelisation).GetProperty("CaterpillarComponentPerNode", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedDictionary<Node, int> caterpillarComponentPerNode = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            caterpillarComponentPerNode.Clear(MockCounter);
            foreach (KeyValuePair<Node, int> kv in caterpillars)
            {
                caterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
            }

            a.ContractEdge(edge10, m);

            CountedDictionary<Node, int> resultDict = (CountedDictionary<Node, int>)dict.GetGetMethod(true).Invoke(a, new object[] { });

            Assert.AreEqual(0, resultDict[node2, MockCounter]);
            Assert.AreEqual(0, resultDict[node23, MockCounter]);
        }
    }
}
