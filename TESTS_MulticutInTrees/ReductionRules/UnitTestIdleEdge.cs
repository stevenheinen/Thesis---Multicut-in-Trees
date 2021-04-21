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
    public class UnitTestIdleEdge
    {
        private static readonly PerformanceMeasurements MockPerformanceMeasurements = new(nameof(UnitTestIdleEdge));
        private static readonly Counter MockCounter = new();

        private static IdleEdge GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(IdleEdge))
                {
                    return (IdleEdge)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(IdleEdge)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair = new(0, node2, node4, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair }, MockCounter);

            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairPerEdge = new(new Dictionary<Edge<Node>, CountedCollection<DemandPair>>()
            {
                {edge02, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {edge01, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {edge14, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) }
            }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            IdleEdge idleEdge = new(tree, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge);
            Assert.IsNotNull(idleEdge);
        }

        [TestMethod]
        public void TestNullArgument()
        {
            Graph tree = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair = new(0, node2, node4, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair }, MockCounter);

            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairPerEdge = new(new Dictionary<Edge<Node>, CountedCollection<DemandPair>>()
            {
                {edge02, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {edge01, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) },
                {edge14, new CountedCollection<DemandPair>(new List<DemandPair>(){ demandPair }, MockCounter) }
            }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);

            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new(null, demandPairs, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new(tree, null, new GuoNiedermeierKernelisation(instance), demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new(tree, demandPairs, null, demandPairPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { IdleEdge i = new(tree, demandPairs, new GuoNiedermeierKernelisation(instance), null); });
        }

        [TestMethod]
        public void TestAfterEdgeContraction()
        {
            Graph tree = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair = new(0, node2, node4, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            GuoNiedermeierKernelisation algorithm = new(instance);
            IdleEdge idleEdge = GetReductionRuleInAlgorithm(algorithm);
            algorithm.ContractEdge(edge13, MockPerformanceMeasurements);

            Assert.IsFalse(idleEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathRemove1()
        {
            Graph tree = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new(1, node2, node4, tree);
            DemandPair demandPair2 = new(2, node0, node1, tree);
            DemandPair demandPair3 = new(3, node1, node3, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair1, demandPair2, demandPair3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            IdleEdge idleEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(demandPair3, MockPerformanceMeasurements);

            Assert.IsTrue(idleEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathRemove2()
        {
            Graph tree = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new(1, node2, node4, tree);
            DemandPair demandPair2 = new(2, node0, node1, tree);
            DemandPair demandPair3 = new(3, node1, node3, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair1, demandPair2, demandPair3 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            IdleEdge idleEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(demandPair2, MockPerformanceMeasurements);

            Assert.IsFalse(idleEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged1()
        {
            Graph tree = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new(1, node3, node0, tree);
            DemandPair demandPair2 = new(2, node2, node4, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair1, demandPair2 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);
            IdleEdge idleEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair1, node3, node1, MockPerformanceMeasurements);

            Assert.IsTrue(idleEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged2()
        {
            Graph tree = new();

            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair demandPair1 = new(1, node3, node0, tree);
            DemandPair demandPair2 = new(2, node2, node4, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair1, demandPair2 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 100, 100);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);

            IdleEdge idleEdge = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(demandPair1, node0, node1, MockPerformanceMeasurements);

            Assert.IsFalse(idleEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestFirstIterationFalse()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            tree.AddNodes(new List<Node>() { node0, node1 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            tree.AddEdge(edge01, MockCounter);
            tree.UpdateNodeTypes();
            DemandPair demandPair = new(0, node0, node1, tree);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { demandPair }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, 1, 1);
            Algorithm algorithm = new GuoNiedermeierKernelisation(instance);
            IdleEdge idleEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(idleEdge.RunFirstIteration());
        }
    }
}
