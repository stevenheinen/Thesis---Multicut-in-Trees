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
    public class UnitTestDisjointPaths
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestDisjointPaths));

        private DisjointPaths GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(DisjointPaths))
                {
                    return (DisjointPaths)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(DisjointPaths)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new Graph();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<Edge<Node>> partialSolution = new List<Edge<Node>>();

            DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, partialSolution, maxSize);
            Assert.IsNotNull(disjointPaths);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Graph tree = new Graph();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<Edge<Node>> partialSolution = new List<Edge<Node>>();

            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new DisjointPaths(null, demandPairs, algorithm, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new DisjointPaths(tree, null, algorithm, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, null, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, null, maxSize); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, partialSolution, -1); });
        }

        [TestMethod]
        public void TestFirstIteration()
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
            Edge<Node> edge04 = new Edge<Node>(node0, node4);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge04, edge12, edge13, edge45, edge46 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs1 = new CountedList<DemandPair>();
            CountedList<DemandPair> demandPairs2 = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node0, node2, tree);
            DemandPair dp2 = new DemandPair(2, node0, node5, tree);
            DemandPair dp3 = new DemandPair(3, node4, node6, tree);

            demandPairs1.Add(dp1, MockCounter);
            demandPairs1.Add(dp2, MockCounter);
            demandPairs1.Add(dp3, MockCounter);
            demandPairs2.Add(dp1, MockCounter);
            demandPairs2.Add(dp2, MockCounter);

            int maxSize = 2;
            MulticutInstance instance1 = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs1, maxSize, 2);
            MulticutInstance instance2 = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs2, maxSize, 2);
            GuoNiedermeierKernelisation algorithm1 = new GuoNiedermeierKernelisation(instance1);
            GuoNiedermeierKernelisation algorithm2 = new GuoNiedermeierKernelisation(instance2);
            List<Edge<Node>> partialSolution = new List<Edge<Node>>();

            DisjointPaths disjointPaths1 = new DisjointPaths(tree, demandPairs1, algorithm1, partialSolution, maxSize);
            DisjointPaths disjointPaths2 = new DisjointPaths(tree, demandPairs2, algorithm2, partialSolution, maxSize);

            Assert.IsTrue(disjointPaths1.RunFirstIteration());
            Assert.IsFalse(disjointPaths2.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPairChanged()
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
            Edge<Node> edge04 = new Edge<Node>(node0, node4);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge04, edge12, edge13, edge45, edge46 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node0, node2, tree);
            DemandPair dp2 = new DemandPair(2, node0, node5, tree);
            DemandPair dp3 = new DemandPair(3, node0, node6, tree);

            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            DisjointPaths disjointPaths = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(disjointPaths.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp3, node0, node4, MockMeasurements);

            Assert.IsTrue(disjointPaths.RunLaterIteration());
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
            Node node6 = new Node(6);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge04 = new Edge<Node>(node0, node4);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge13 = new Edge<Node>(node1, node3);
            Edge<Node> edge45 = new Edge<Node>(node4, node5);
            Edge<Node> edge46 = new Edge<Node>(node4, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge04, edge12, edge13, edge45, edge46 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node0, node2, tree);
            DemandPair dp2 = new DemandPair(2, node0, node5, tree);
            DemandPair dp3 = new DemandPair(3, node0, node6, tree);

            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            DisjointPaths disjointPaths = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(dp3, MockMeasurements);

            Assert.IsFalse(disjointPaths.RunLaterIteration());
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
            Node node5 = new Node(5);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge20 = new Edge<Node>(node2, node0);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge34 = new Edge<Node>(node3, node4);
            Edge<Node> edge35 = new Edge<Node>(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge21, edge20, edge23, edge34, edge35 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node0, node4, tree);
            DemandPair dp2 = new DemandPair(2, node1, node5, tree);

            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            DisjointPaths disjointPaths = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(disjointPaths.RunFirstIteration());

            algorithm.ContractEdge(edge23, MockMeasurements);

            Assert.IsTrue(disjointPaths.RunLaterIteration());
        }
    }
}
