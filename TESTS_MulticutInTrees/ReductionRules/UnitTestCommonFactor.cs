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
    public class UnitTestCommonFactor
    {
        private static readonly Counter MockCounter = new();
        private static readonly PerformanceMeasurements MockMeasurements = new(nameof(UnitTestCommonFactor));

        private static CommonFactor GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(CommonFactor))
                {
                    return (CommonFactor)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(CommonFactor)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new();
            CountedCollection<DemandPair> dps = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new(instance);
            List<Edge<Node>> partialSolution = new();
            int maxSolutionSize = 4;
            CommonFactor commonFactor = new(tree, dps, algorithm, partialSolution, maxSolutionSize);
            Assert.IsNotNull(commonFactor);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new();
            CountedCollection<DemandPair> dps = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 10, 10);
            BousquetKernelisation algorithm = new(instance);
            List<Edge<Node>> partialSolution = new();

            Assert.ThrowsException<ArgumentNullException>(() => new CommonFactor(null, dps, algorithm, partialSolution, 3));
            Assert.ThrowsException<ArgumentNullException>(() => new CommonFactor(tree, null, algorithm, partialSolution, 3));
            Assert.ThrowsException<ArgumentNullException>(() => new CommonFactor(tree, dps, null, partialSolution, 3));
            Assert.ThrowsException<ArgumentNullException>(() => new CommonFactor(tree, dps, algorithm, null, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new CommonFactor(tree, dps, algorithm, partialSolution, -3));
        }

        [TestMethod]
        public void TestRunFirstIterationTrue()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge25, edge26 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node3, tree);
            DemandPair dp2 = new(2, node0, node4, tree);
            DemandPair dp3 = new(3, node0, node5, tree);
            DemandPair dp4 = new(4, node0, node6, tree);
            DemandPair dp5 = new(5, node1, node2, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 2, 2);
            BousquetKernelisation algorithm = new(instance);

            CommonFactor commonFactor = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(commonFactor.RunFirstIteration());
        }

        [TestMethod]
        public void TestRunFirstIterationFalse1()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node2 = new(2);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);
            Node node6 = new(6);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge25, edge26 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node3, tree);
            DemandPair dp2 = new(2, node0, node4, tree);
            DemandPair dp3 = new(3, node0, node5, tree);
            DemandPair dp4 = new(4, node0, node6, tree);
            DemandPair dp5 = new(5, node1, node2, tree);
            DemandPair dp6 = new(6, node1, node4, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5, dp6 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 5, 5);
            BousquetKernelisation algorithm = new(instance);

            CommonFactor commonFactor = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(commonFactor.RunFirstIteration());
        }

        [TestMethod]
        public void TestRunFirstIterationFalse2()
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
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            Edge<Node> edge07 = new(node0, node7);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge25, edge26, edge07 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node3, tree);
            DemandPair dp2 = new(2, node1, node4, tree);
            DemandPair dp3 = new(3, node7, node5, tree);
            DemandPair dp4 = new(4, node7, node6, tree);
            DemandPair dp5 = new(5, node3, node2, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 2, 2);
            BousquetKernelisation algorithm = new(instance);

            CommonFactor commonFactor = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(commonFactor.RunFirstIteration());
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
            Node node6 = new(6);
            Node node7 = new(7);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            Edge<Node> edge07 = new(node0, node7);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge14, edge25, edge26, edge07 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node3, tree);
            DemandPair dp2 = new(2, node1, node4, tree);
            DemandPair dp3 = new(3, node7, node5, tree);
            DemandPair dp4 = new(4, node7, node6, tree);
            DemandPair dp5 = new(5, node3, node2, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 2, 2);
            BousquetKernelisation algorithm = new(instance);

            CommonFactor commonFactor = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(commonFactor.RunFirstIteration());

            algorithm.RemoveDemandPair(dp1, MockMeasurements);

            Assert.IsFalse(commonFactor.RunLaterIteration());
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
            Node node6 = new(6);
            Node node7 = new(7);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            Edge<Node> edge07 = new(node0, node7);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge25, edge26, edge07 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node3, tree);
            DemandPair dp2 = new(2, node1, node4, tree);
            DemandPair dp3 = new(3, node7, node5, tree);
            DemandPair dp4 = new(4, node7, node6, tree);
            DemandPair dp5 = new(5, node3, node2, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 2, 2);
            BousquetKernelisation algorithm = new(instance);

            CommonFactor commonFactor = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(commonFactor.RunFirstIteration());

            algorithm.ContractEdge(edge07, MockMeasurements);

            Assert.IsTrue(commonFactor.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged()
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
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge02 = new(node0, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge14 = new(node1, node4);
            Edge<Node> edge25 = new(node2, node5);
            Edge<Node> edge26 = new(node2, node6);
            Edge<Node> edge07 = new(node0, node7);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge02, edge13, edge14, edge25, edge26, edge07 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node3, tree);
            DemandPair dp2 = new(2, node1, node4, tree);
            DemandPair dp3 = new(3, node7, node5, tree);
            DemandPair dp4 = new(4, node7, node6, tree);
            DemandPair dp5 = new(5, node3, node2, tree);
            CountedCollection<DemandPair> dps = new(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 2, 2);
            BousquetKernelisation algorithm = new(instance);

            CommonFactor commonFactor = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(commonFactor.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp3, node7, node0, MockMeasurements);

            Assert.IsTrue(commonFactor.RunLaterIteration());
        }
    }
}
