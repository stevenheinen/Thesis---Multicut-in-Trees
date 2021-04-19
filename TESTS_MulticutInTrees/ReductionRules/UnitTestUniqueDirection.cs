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
using MulticutInTrees.ReductionRules;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestUniqueDirection
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestUniqueDirection));

        private UniqueDirection GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(UniqueDirection))
                {
                    return (UniqueDirection)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(UniqueDirection)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new Graph();
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);
            CountedDictionary<Node, CountedCollection<DemandPair>> dpsPerNode = new CountedDictionary<Node, CountedCollection<DemandPair>>();
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> dpsPerEdge = new CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>();
            UniqueDirection uniqueDirection = new UniqueDirection(tree, dps, algorithm, dpsPerNode, dpsPerEdge);
            Assert.IsNotNull(uniqueDirection);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new Graph();
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);
            CountedDictionary<Node, CountedCollection<DemandPair>> dpsPerNode = new CountedDictionary<Node, CountedCollection<DemandPair>>();
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> dpsPerEdge = new CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(null, dps, algorithm, dpsPerNode, dpsPerEdge));
            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(tree, null, algorithm, dpsPerNode, dpsPerEdge));
            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(tree, dps, null, dpsPerNode, dpsPerEdge));
            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(tree, dps, algorithm, null, dpsPerEdge));
            Assert.ThrowsException<ArgumentNullException>(() => new UniqueDirection(tree, dps, algorithm, dpsPerNode, null));
        }

        [TestMethod]
        public void TestRunFirstIterationFalse()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge26 = new Edge<Node>(node2, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge25, edge26 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node3, tree);
            DemandPair dp2 = new DemandPair(2, node0, node4, tree);
            DemandPair dp3 = new DemandPair(3, node3, node4, tree);
            DemandPair dp4 = new DemandPair(4, node0, node6, tree);
            DemandPair dp5 = new DemandPair(5, node5, node6, tree);
            DemandPair dp6 = new DemandPair(6, node0, node5, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5, dp6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(uniqueDirection.RunFirstIteration());
        }

        [TestMethod]
        public void TestRunFirstIterationTrue1()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            Edge<Node> edge26 = new Edge<Node>(node2, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge25, edge26 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node3, tree);
            DemandPair dp2 = new DemandPair(2, node0, node4, tree);
            DemandPair dp3 = new DemandPair(3, node3, node4, tree);
            DemandPair dp4 = new DemandPair(4, node0, node6, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(uniqueDirection.RunFirstIteration());
            Assert.AreEqual(5, tree.NumberOfEdges(MockCounter));
        }

        [TestMethod]
        public void TestRunFirstIterationTrue2()
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
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge05 = new Edge<Node>(node0, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge05 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node3, tree);
            DemandPair dp2 = new DemandPair(2, node0, node4, tree);
            DemandPair dp3 = new DemandPair(3, node3, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(uniqueDirection.RunFirstIteration());
            Assert.AreEqual(3, tree.NumberOfEdges(MockCounter));
        }

        [TestMethod]
        public void TestRunFirstIterationTrue3()
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
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge05 = new Edge<Node>(node0, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge05 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node3, tree);
            DemandPair dp2 = new DemandPair(2, node0, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(uniqueDirection.RunFirstIteration());
            Assert.AreEqual(1, tree.NumberOfEdges(MockCounter));
        }

        [TestMethod]
        public void TestPathOfLength2BetweenLeaves()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            tree.AddNodes(new List<Node>() { node0, node1, node2 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(0, node1, node2, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(uniqueDirection.RunFirstIteration());
            Assert.AreEqual(1, tree.NumberOfEdges(MockCounter));
        }

        [TestMethod]
        public void TestAfterDemandPairRemoved()
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
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge24, edge25 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node1, tree);
            DemandPair dp2 = new DemandPair(2, node1, node3, tree);
            DemandPair dp3 = new DemandPair(3, node3, node4, tree);
            DemandPair dp4 = new DemandPair(4, node4, node5, tree);
            DemandPair dp5 = new DemandPair(5, node3, node5, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(dp1, MockMeasurements);

            Assert.IsTrue(uniqueDirection.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterEdgeContracted()
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
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge25 = new Edge<Node>(node2, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge24, edge25 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node1, tree);
            DemandPair dp2 = new DemandPair(2, node1, node3, tree);
            DemandPair dp3 = new DemandPair(3, node3, node4, tree);
            DemandPair dp4 = new DemandPair(4, node4, node5, tree);
            DemandPair dp5 = new DemandPair(5, node3, node5, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ContractEdge(edge23, MockMeasurements);

            Assert.IsFalse(uniqueDirection.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged()
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
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            Edge<Node> edge15 = new Edge<Node>(node1, node5);
            Edge<Node> edge26 = new Edge<Node>(node2, node6);
            Edge<Node> edge27 = new Edge<Node>(node2, node7);
            Edge<Node> edge28 = new Edge<Node>(node2, node8);
            Edge<Node> edge69 = new Edge<Node>(node6, node9);
            Edge<Node> edge610 = new Edge<Node>(node6, node10);
            Edge<Node> edge611 = new Edge<Node>(node6, node11);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge15, edge26, edge27, edge28, edge69, edge610, edge611 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node3, node4, tree);
            DemandPair dp2 = new DemandPair(2, node5, node3, tree);
            DemandPair dp3 = new DemandPair(3, node4, node5, tree);
            DemandPair dp4 = new DemandPair(4, node6, node7, tree);
            DemandPair dp5 = new DemandPair(5, node6, node8, tree);
            DemandPair dp6 = new DemandPair(6, node7, node8, tree);
            DemandPair dp7 = new DemandPair(7, node0, node3, tree);
            DemandPair dp8 = new DemandPair(8, node0, node5, tree);
            DemandPair dp9 = new DemandPair(9, node0, node9, tree);
            DemandPair dp10 = new DemandPair(10, node9, node11, tree);
            DemandPair dp11 = new DemandPair(11, node9, node10, tree);
            DemandPair dp12 = new DemandPair(12, node10, node11, tree);
            DemandPair dp13 = new DemandPair(13, node2, node3, tree);
            DemandPair dp14 = new DemandPair(14, node2, node11, tree);
            DemandPair dp15 = new DemandPair(15, node6, node11, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9, dp10, dp11, dp12, dp13, dp14, dp15 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new BousquetKernelisation(instance);

            UniqueDirection uniqueDirection = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(uniqueDirection.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp9, node0, node2, MockMeasurements);

            Assert.IsTrue(uniqueDirection.RunLaterIteration());
        }
    }
}
