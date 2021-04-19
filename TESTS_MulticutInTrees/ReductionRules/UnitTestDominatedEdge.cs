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
    public class UnitTestDominatedEdge
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestDominatedEdge));

        private DominatedEdge GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(DominatedEdge))
                {
                    return (DominatedEdge)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(DominatedEdge)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(0, node2, node4, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair }, MockCounter);

            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>(new Dictionary<Edge<Node>, CountedCollection<DemandPair>>()
            {
                {edge02, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {edge01, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {edge14, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) }
            }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            DominatedEdge dominatedEdge = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsNotNull(dominatedEdge);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair = new DemandPair(0, node2, node4, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair }, MockCounter);

            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairPerEdge = new CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>(new Dictionary<Edge<Node>, CountedCollection<DemandPair>>()
            {
                {edge02, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {edge01, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {edge14, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) }
            }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(null, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, null, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedEdge de = new DominatedEdge(tree, demandPairs, new GuoNiedermeierKernelisation(instance), null); });
        }

        [TestMethod]
        public void TestFirstIteration1()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node2, node4, tree);
            DemandPair demandPair2 = new DemandPair(2, node3, node0, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);
            Assert.IsTrue(dominatedEdge.RunFirstIteration());
        }

        [TestMethod]
        public void TestFirstIteration2()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node1, node4, tree);
            DemandPair demandPair2 = new DemandPair(2, node3, node1, tree);
            DemandPair demandPair3 = new DemandPair(3, node0, node1, tree);
            DemandPair demandPair4 = new DemandPair(4, node0, node2, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(dominatedEdge.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathRemoved1()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node2, node4, tree);
            DemandPair demandPair2 = new DemandPair(2, node3, node0, tree);
            DemandPair demandPair3 = new DemandPair(3, node2, node1, tree);
            DemandPair demandPair4 = new DemandPair(4, node2, node0, tree);
            DemandPair demandPair5 = new DemandPair(5, node1, node3, tree);
            DemandPair demandPair6 = new DemandPair(6, node1, node4, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(demandPair3, MockMeasurements);

            Assert.IsFalse(dominatedEdge.RunLaterIteration());
        }

        public void TestAfterDemandPathRemoved2()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node2, node4, tree);
            DemandPair demandPair2 = new DemandPair(2, node3, node0, tree);
            DemandPair demandPair4 = new DemandPair(3, node2, node0, tree);
            DemandPair demandPair5 = new DemandPair(4, node1, node3, tree);
            DemandPair demandPair6 = new DemandPair(5, node1, node4, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(demandPair4, MockMeasurements);

            Assert.IsTrue(dominatedEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged1()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge02 = new Edge<Node>(node0, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge14 = new Edge<Node>(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node2, node4, tree);
            DemandPair demandPair2 = new DemandPair(2, node1, node4, tree);
            DemandPair demandPair3 = new DemandPair(3, node2, node1, tree);
            DemandPair demandPair4 = new DemandPair(4, node2, node0, tree);
            DemandPair demandPair5 = new DemandPair(5, node1, node3, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair3, node1, node0, MockMeasurements);

            Assert.IsTrue(dominatedEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged2()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node1, node0, tree);
            DemandPair demandPair2 = new DemandPair(2, node3, node2, tree);
            DemandPair demandPair3 = new DemandPair(3, node2, node0, tree);
            DemandPair demandPair4 = new DemandPair(4, node1, node3, tree);
            DemandPair demandPair5 = new DemandPair(5, node1, node3, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair3, node0, node1, MockMeasurements);

            Assert.IsFalse(dominatedEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged3()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node0, node3, tree);
            DemandPair demandPair2 = new DemandPair(2, node1, node2, tree);
            DemandPair demandPair3 = new DemandPair(3, node0, node1, tree);
            DemandPair demandPair4 = new DemandPair(4, node2, node4, tree);
            DemandPair demandPair5 = new DemandPair(5, node3, node4, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair4, node2, node3, MockMeasurements);

            Assert.IsTrue(dominatedEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged4()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node0, node3, tree);
            DemandPair demandPair2 = new DemandPair(2, node1, node2, tree);
            DemandPair demandPair3 = new DemandPair(3, node0, node1, tree);
            DemandPair demandPair4 = new DemandPair(4, node2, node4, tree);
            DemandPair demandPair5 = new DemandPair(5, node3, node4, tree);
            DemandPair demandPair6 = new DemandPair(6, node2, node4, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5, demandPair6 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair4, node2, node3, MockMeasurements);

            Assert.IsFalse(dominatedEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterEdgeContraction()
        {
            Graph tree = new Graph();

            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new DemandPair(1, node1, node0, tree);
            DemandPair demandPair2 = new DemandPair(2, node4, node2, tree);
            DemandPair demandPair3 = new DemandPair(3, node2, node0, tree);
            DemandPair demandPair4 = new DemandPair(4, node1, node4, tree);
            DemandPair demandPair5 = new DemandPair(5, node1, node3, tree);
            CountedCollection<DemandPair> demandPairs = new CountedCollection<DemandPair>(new List<DemandPair>() { demandPair1, demandPair2, demandPair3, demandPair4, demandPair5 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            algorithm.ContractEdge(edge34, MockMeasurements);

            DominatedEdge dominatedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(dominatedEdge.RunLaterIteration());
        }
    }
}
