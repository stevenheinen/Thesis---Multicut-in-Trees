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
        private static readonly Counter MockCounter = new();
        private static readonly PerformanceMeasurements MockMeasurements = new(nameof(UnitTestDisjointPaths));

        private static DisjointPaths GetReductionRuleInAlgorithm(Algorithm algorithm)
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
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            int maxSize = 10;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new(instance);
            List<Edge<Node>> partialSolution = new();

            DisjointPaths disjointPaths = new(tree, demandPairs, algorithm, partialSolution, maxSize);
            Assert.IsNotNull(disjointPaths);
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

            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new(null, demandPairs, algorithm, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new(tree, null, algorithm, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new(tree, demandPairs, null, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new(tree, demandPairs, algorithm, null, maxSize); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { DisjointPaths disjointPaths = new(tree, demandPairs, algorithm, partialSolution, -1); });
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
            Node node5 = new(5);
            Node node6 = new(6);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge04 = new(node0, node4);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge46 = new(node4, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge04, edge12, edge13, edge45, edge46 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs1 = new();
            CountedCollection<DemandPair> demandPairs2 = new();
            DemandPair dp1 = new(1, node0, node2, tree);
            DemandPair dp2 = new(2, node0, node5, tree);
            DemandPair dp3 = new(3, node4, node6, tree);

            demandPairs1.Add(dp1, MockCounter);
            demandPairs1.Add(dp2, MockCounter);
            demandPairs1.Add(dp3, MockCounter);
            demandPairs2.Add(dp1, MockCounter);
            demandPairs2.Add(dp2, MockCounter);

            int maxSize = 2;
            MulticutInstance instance1 = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs1, maxSize, 2);
            MulticutInstance instance2 = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs2, maxSize, 2);
            GuoNiedermeierKernelisation algorithm1 = new(instance1);
            GuoNiedermeierKernelisation algorithm2 = new(instance2);
            List<Edge<Node>> partialSolution = new();

            DisjointPaths disjointPaths1 = new(tree, demandPairs1, algorithm1, partialSolution, maxSize);
            DisjointPaths disjointPaths2 = new(tree, demandPairs2, algorithm2, partialSolution, maxSize);

            Assert.IsTrue(disjointPaths1.RunFirstIteration());
            Assert.IsFalse(disjointPaths2.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPairChanged()
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
            Edge<Node> edge04 = new(node0, node4);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge46 = new(node4, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge04, edge12, edge13, edge45, edge46 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node0, node2, tree);
            DemandPair dp2 = new(2, node0, node5, tree);
            DemandPair dp3 = new(3, node0, node6, tree);

            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new(instance);

            DisjointPaths disjointPaths = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(disjointPaths.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp3, node0, node4, MockMeasurements);

            Assert.IsTrue(disjointPaths.RunLaterIteration());
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
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge04 = new(node0, node4);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge13 = new(node1, node3);
            Edge<Node> edge45 = new(node4, node5);
            Edge<Node> edge46 = new(node4, node6);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge04, edge12, edge13, edge45, edge46 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node0, node2, tree);
            DemandPair dp2 = new(2, node0, node5, tree);
            DemandPair dp3 = new(3, node0, node6, tree);

            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new(instance);

            DisjointPaths disjointPaths = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(dp3, MockMeasurements);

            Assert.IsFalse(disjointPaths.RunLaterIteration());
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
            Node node5 = new(5);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5 }, MockCounter);
            Edge<Node> edge21 = new(node2, node1);
            Edge<Node> edge20 = new(node2, node0);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge34 = new(node3, node4);
            Edge<Node> edge35 = new(node3, node5);
            tree.AddEdges(new List<Edge<Node>>() { edge21, edge20, edge23, edge34, edge35 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node0, node4, tree);
            DemandPair dp2 = new(2, node1, node5, tree);

            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            DisjointPaths disjointPaths = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(disjointPaths.RunFirstIteration());

            algorithm.ContractEdge(edge23, MockMeasurements);

            Assert.IsTrue(disjointPaths.RunLaterIteration());
        }
    }
}
