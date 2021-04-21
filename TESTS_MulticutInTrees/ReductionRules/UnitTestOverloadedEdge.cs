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
    public class UnitTestOverloadedEdge
    {
        private static readonly Counter MockCounter = new();
        private static readonly PerformanceMeasurements MockMeasurements = new(nameof(UnitTestOverloadedEdge));

        private static OverloadedEdge GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(OverloadedEdge))
                {
                    return (OverloadedEdge)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(OverloadedEdge)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            int maxSize = 10;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new(instance);
            List<Edge<Node>> partialSolution = new();
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge = new();

            OverloadedEdge overloadedEdge = new(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            Assert.IsNotNull(overloadedEdge);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            int maxSize = 10;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new(instance);
            List<Edge<Node>> partialSolution = new();
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge = new();

            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new(null, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new(tree, null, algorithm, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new(tree, demandPairs, null, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new(tree, demandPairs, algorithm, null, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new(tree, demandPairs, algorithm, partialSolution, maxSize, null); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { OverloadedEdge oe = new(tree, demandPairs, algorithm, partialSolution, -56, demandPairsPerEdge); });
        }

        [TestMethod]
        public void TestFirstIteration()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node0, node2, tree);
            DemandPair dp2 = new(2, node0, node3, tree);
            DemandPair dp3 = new(3, node0, node4, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(overloadedEdge.RunFirstIteration());
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
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge13, edge14 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node0, node2, tree);
            DemandPair dp2 = new(2, node0, node3, tree);
            DemandPair dp3 = new(3, node0, node4, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedEdge.RunLaterIteration());
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
            Node node5 = new(5);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge45 = new(node4, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge13, edge14, edge45 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node0, node2, tree);
            DemandPair dp2 = new(2, node0, node3, tree);
            DemandPair dp3 = new(3, node0, node5, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new(instance);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedEdge.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp3, node5, node4, MockMeasurements);

            Assert.IsTrue(overloadedEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPairRemoved()
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
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge05 = new(node0, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge13, edge14, edge05 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node5, node2, tree);
            DemandPair dp2 = new(2, node5, node3, tree);
            DemandPair dp3 = new(3, node5, node1, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            algorithm.RemoveDemandPair(dp2, MockMeasurements);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedEdge.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterEdgeContracted()
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
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge05 = new(node0, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge13, edge14, edge05 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node5, node2, tree);
            DemandPair dp2 = new(2, node5, node3, tree);
            DemandPair dp3 = new(3, node5, node1, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            OverloadedEdge overloadedEdge = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedEdge.RunFirstIteration());

            algorithm.ContractEdge(edge01, MockMeasurements);

            Assert.IsTrue(overloadedEdge.RunLaterIteration());
        }
    }
}
