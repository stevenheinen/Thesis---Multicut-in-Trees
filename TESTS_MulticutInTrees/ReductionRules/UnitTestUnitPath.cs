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
    public class UnitTestUnitPath
    {
        private static readonly Counter MockCounter = new();
        private static readonly PerformanceMeasurements MockMeasurements = new(nameof(UnitTestUnitPath));

        private static UnitPath GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(UnitPath))
                {
                    return (UnitPath)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(UnitPath)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 0);
            GuoNiedermeierKernelisation gnfpt = new(instance);

            UnitPath unitPath = new(tree, demandPairs, gnfpt);

            Assert.IsNotNull(unitPath);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 0);
            GuoNiedermeierKernelisation gnfpt = new(instance);

            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new(null, demandPairs, gnfpt); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new(tree, null, gnfpt); });
            Assert.ThrowsException<ArgumentNullException>(() => { UnitPath up = new(tree, demandPairs, null); });
        }

        [TestMethod]
        public void TestAfterEdgeContraction1()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node2 = new(2);
            Node node1 = new(1);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge35 = new(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge35 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node4, tree);
            DemandPair dp2 = new(2, node1, node3, tree);

            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ContractEdge(edge12, MockMeasurements);

            Assert.IsTrue(unitPath.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterEdgeContraction2()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node2 = new(2);
            Node node1 = new(1);
            Node node3 = new(3);
            Node node4 = new(4);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge35 = new(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge35 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node4, tree);
            DemandPair dp2 = new(2, node1, node3, tree);

            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ContractEdge(edge34, MockMeasurements);

            Assert.IsFalse(unitPath.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathRemove()
        {
            Graph tree = new();
            Node node0 = new(0);
            Node node1 = new(1);
            Node node3 = new(3);
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node3, node5 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge35 = new(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge13, edge35 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node5, tree);
            DemandPair dp2 = new(2, node1, node3, tree);

            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(dp1, MockMeasurements);

            Assert.IsFalse(unitPath.RunLaterIteration());
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
            Node node5 = new(5);

            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge35 = new(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge35 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node4, tree);
            DemandPair dp2 = new(2, node1, node3, tree);

            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(dp1, node4, node3, MockMeasurements);

            Assert.IsFalse(unitPath.RunLaterIteration());
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
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge35 = new(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge35 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(1, node0, node4, tree);
            DemandPair dp2 = new(2, node1, node3, tree);

            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(dp2, node3, node2, MockMeasurements);

            Assert.IsTrue(unitPath.RunLaterIteration());
        }

        [TestMethod]
        public void TestRunFirstIteration1()
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
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge35 = new(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge35 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp1 = new(0, node0, node4, tree);

            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp1 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(unitPath.RunFirstIteration());
        }

        [TestMethod]
        public void TestRunFirstIteration2()
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
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge35 = new(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge34, edge35 }, MockCounter);

            tree.UpdateNodeTypes();

            DemandPair dp2 = new(0, node1, node2, tree);

            CountedCollection<DemandPair> demandPairs = new(new List<DemandPair>() { dp2 }, MockCounter);

            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, 1, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            UnitPath unitPath = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(unitPath.RunFirstIteration());
        }
    }
}
