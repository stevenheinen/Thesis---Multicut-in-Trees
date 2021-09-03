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
    public class UnitTestGeneralisedDominatingWingspan
    {
        private static readonly Counter MockCounter = new();
        private static readonly PerformanceMeasurements MockMeasurements = new(nameof(UnitTestCommonFactor));

        private static GeneralisedDominatingWingspan GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(GeneralisedDominatingWingspan))
                {
                    return (GeneralisedDominatingWingspan)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(GeneralisedDominatingWingspan)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new();
            CountedCollection<DemandPair> dps = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, 0, "", "", tree, dps, 10, 10);
            BousquetKernelisation algorithm = new(instance);
            CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode = new();
            CountedDictionary<Node, int> caterpillarComponentPerNode = new();
            List<Edge<Node>> partialSolution = new();
            int maxSolutionSize = 4;
            GeneralisedDominatingWingspan bidimensionalDominatingWingspan = new(tree, dps, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSolutionSize);
            Assert.IsNotNull(bidimensionalDominatingWingspan);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            int maxSize = 10;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new(instance);
            List<Edge<Node>> partialSolution = new();
            CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode = new();
            CountedDictionary<Node, int> caterpillarComponentPerNode = new();

            Assert.ThrowsException<ArgumentNullException>(() => new GeneralisedDominatingWingspan(null, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize));
            Assert.ThrowsException<ArgumentNullException>(() => new GeneralisedDominatingWingspan(tree, null, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize));
            Assert.ThrowsException<ArgumentNullException>(() => new GeneralisedDominatingWingspan(tree, demandPairs, null, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize));
            Assert.ThrowsException<ArgumentNullException>(() => new GeneralisedDominatingWingspan(tree, demandPairs, algorithm, null, caterpillarComponentPerNode, partialSolution, maxSize));
            Assert.ThrowsException<ArgumentNullException>(() => new GeneralisedDominatingWingspan(tree, demandPairs, algorithm, demandPairsPerNode, null, partialSolution, maxSize));
            Assert.ThrowsException<ArgumentNullException>(() => new GeneralisedDominatingWingspan(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, null, maxSize));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GeneralisedDominatingWingspan(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, -1));
        }

        [TestMethod]
        public void TestFirstIterationTrue()
        {
            Graph graph = new();
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge06 = new(node0, node6);
            Edge<Node> edge07 = new(node0, node7);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge410 = new(node4, node10);
            Edge<Node> edge108 = new(node10, node8);
            Edge<Node> edge109 = new(node10, node9);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge06, edge07, edge12, edge23, edge25, edge34, edge410, edge108, edge109 }, MockCounter);
            graph.UpdateNodeTypes();

            DemandPair dp1 = new(1, node5, node6, graph);
            DemandPair dp2 = new(2, node5, node9, graph);
            DemandPair dp3 = new(3, node6, node3, graph);
            DemandPair dp4 = new(4, node4, node0, graph);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3, dp4 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", graph, demandPairs, 1, 1);
            BousquetKernelisation algorithm = new(instance);

            GeneralisedDominatingWingspan bidimensionalDominatingWingspan = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(bidimensionalDominatingWingspan.RunFirstIteration());
        }

        [TestMethod]
        public void TestFirstIterationFalse()
        {
            Graph graph = new();
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge06 = new(node0, node6);
            Edge<Node> edge07 = new(node0, node7);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge48 = new(node4, node8);
            Edge<Node> edge49 = new(node4, node9);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge06, edge07, edge12, edge23, edge25, edge34, edge48, edge49 }, MockCounter);
            graph.UpdateNodeTypes();

            DemandPair dp1 = new(1, node5, node6, graph);
            DemandPair dp2 = new(2, node5, node9, graph);
            DemandPair dp3 = new(3, node6, node3, graph);
            DemandPair dp4 = new(4, node8, node0, graph);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3, dp4 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", graph, demandPairs, 2, 2);
            BousquetKernelisation algorithm = new(instance);

            GeneralisedDominatingWingspan bidimensionalDominatingWingspan = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(bidimensionalDominatingWingspan.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPairRemoved()
        {
            Graph graph = new();
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge06 = new(node0, node6);
            Edge<Node> edge07 = new(node0, node7);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge410 = new(node4, node10);
            Edge<Node> edge108 = new(node10, node8);
            Edge<Node> edge109 = new(node10, node9);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge06, edge07, edge12, edge23, edge25, edge34, edge410, edge108, edge109 }, MockCounter);
            graph.UpdateNodeTypes();

            DemandPair dp1 = new(1, node5, node6, graph);
            DemandPair dp2 = new(2, node5, node9, graph);
            DemandPair dp3 = new(3, node6, node3, graph);
            DemandPair dp4 = new(4, node4, node0, graph);
            DemandPair dp5 = new(5, node5, node3, graph);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", graph, demandPairs, 1, 1);
            BousquetKernelisation algorithm = new(instance);

            GeneralisedDominatingWingspan bidimensionalDominatingWingspan = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(bidimensionalDominatingWingspan.RunFirstIteration());

            algorithm.RemoveDemandPair(dp5, MockMeasurements);

            Assert.IsTrue(bidimensionalDominatingWingspan.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPairChanged()
        {
            Graph graph = new();
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge06 = new(node0, node6);
            Edge<Node> edge07 = new(node0, node7);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge410 = new(node4, node10);
            Edge<Node> edge108 = new(node10, node8);
            Edge<Node> edge109 = new(node10, node9);
            Edge<Node> edge811 = new(node8, node11);
            Edge<Node> edge912 = new(node9, node12);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge06, edge07, edge12, edge23, edge25, edge34, edge410, edge108, edge109, edge811, edge912 }, MockCounter);
            graph.UpdateNodeTypes();

            DemandPair dp1 = new(1, node5, node6, graph);
            DemandPair dp2 = new(2, node5, node9, graph);
            DemandPair dp3 = new(3, node6, node3, graph);
            DemandPair dp4 = new(4, node10, node0, graph);
            DemandPair dp5 = new(5, node2, node7, graph);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", graph, demandPairs, 2, 2);
            BousquetKernelisation algorithm = new(instance);

            GeneralisedDominatingWingspan bidimensionalDominatingWingspan = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(bidimensionalDominatingWingspan.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp4, node10, node4, MockMeasurements);

            Assert.IsTrue(bidimensionalDominatingWingspan.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterEdgeContraction()
        {
            Graph graph = new();
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
            graph.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge17 = new(node1, node7);
            Edge<Node> edge19 = new(node1, node9);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge46 = new(node4, node6);
            Edge<Node> edge78 = new(node7, node8);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge1011 = new(node10, node11);
            Edge<Node> edge1112 = new(node11, node12);
            Edge<Node> edge1113 = new(node11, node13);
            graph.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge17, edge19, edge23, edge34, edge45, edge46, edge78, edge910, edge1011, edge1112, edge1113 }, MockCounter);
            graph.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node5, graph);
            DemandPair dp2 = new(2, node0, node11, graph);
            DemandPair dp3 = new(3, node3, node1, graph);
            DemandPair dp4 = new(4, node2, node13, graph);
            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2, dp3, dp4 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", graph, demandPairs, 1, 1);
            BousquetKernelisation algorithm = new(instance);

            GeneralisedDominatingWingspan bidimensionalDominatingWingspan = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(bidimensionalDominatingWingspan.RunFirstIteration());

            algorithm.ContractEdge(edge17, MockMeasurements);

            Assert.IsTrue(bidimensionalDominatingWingspan.RunLaterIteration());
        }
    }
}
