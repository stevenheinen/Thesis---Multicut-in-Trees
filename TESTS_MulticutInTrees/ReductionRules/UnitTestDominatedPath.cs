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
    public class UnitTestDominatedPath
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestDominatedPath));

        private DominatedPath GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(DominatedPath))
                {
                    return (DominatedPath)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(DominatedPath)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new Graph();
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10, 10);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>();

            DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, demandPairsPerEdge);
            Assert.IsNotNull(dominatedPath);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new Graph();
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10, 10);
            GuoNiedermeierKernelisation gnfpt = new GuoNiedermeierKernelisation(instance);
            CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(null, dps, gnfpt, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, null, gnfpt, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, null, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { DominatedPath dominatedPath = new DominatedPath(tree, dps, gnfpt, null); });
        }

        [TestMethod]
        public void TestRunFirstIteration()
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

            DemandPair dp1 = new DemandPair(1, node2, node1, tree);
            DemandPair dp2 = new DemandPair(2, node2, node0, tree);
            DemandPair dp3 = new DemandPair(3, node3, node4, tree);
            DemandPair dp4 = new DemandPair(4, node1, node3, tree);
            DemandPair dp5 = new DemandPair(5, node3, node1, tree);

            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3, dp4, dp5 }, MockCounter);
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 10, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = GetReductionRuleInAlgorithm(algorithm);
            Assert.IsTrue(dominatedPath.RunFirstIteration());
            Assert.AreEqual(2, dps.Count(MockCounter));
        }

        [TestMethod]
        public void TestAfterEdgeContractionFalse()
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
            Edge<Node> edge35 = new Edge<Node>(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge24, edge35 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node2, tree);
            DemandPair dp2 = new DemandPair(2, node5, node4, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ContractEdge(edge35, MockMeasurements);

            Assert.IsFalse(dominatedPath.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterEdgeContractionTrue()
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
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge35 = new Edge<Node>(node3, node5);
            Edge<Node> edge36 = new Edge<Node>(node3, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge24, edge35, edge36 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node2, tree);
            DemandPair dp2 = new DemandPair(2, node5, node4, tree);
            DemandPair dp3 = new DemandPair(3, node4, node6, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ContractEdge(edge35, MockMeasurements);

            Assert.IsTrue(dominatedPath.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathRemove()
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
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge24 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node2, tree);
            DemandPair dp2 = new DemandPair(2, node3, node4, tree);
            DemandPair dp3 = new DemandPair(3, node1, node3, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(dp3, MockMeasurements);

            Assert.IsFalse(dominatedPath.RunLaterIteration());
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
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge35 = new Edge<Node>(node3, node5);
            Edge<Node> edge36 = new Edge<Node>(node3, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge12, edge23, edge24, edge35, edge36 }, MockCounter);
            tree.UpdateNodeTypes();

            DemandPair dp1 = new DemandPair(1, node0, node2, tree);
            DemandPair dp2 = new DemandPair(2, node5, node4, tree);
            DemandPair dp3 = new DemandPair(3, node4, node6, tree);
            CountedCollection<DemandPair> dps = new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter);

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            DominatedPath dominatedPath = GetReductionRuleInAlgorithm(algorithm);

            algorithm.ChangeEndpointOfDemandPair(dp3, node6, node3, MockMeasurements);

            Assert.IsTrue(dominatedPath.RunLaterIteration());
        }
    }
}
