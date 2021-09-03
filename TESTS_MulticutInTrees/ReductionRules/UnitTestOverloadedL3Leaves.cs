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
    public class UnitTestOverloadedL3Leaves
    {
        private static readonly Counter MockCounter = new();
        private static readonly PerformanceMeasurements MockMeasurements = new(nameof(UnitTestOverloadedCaterpillar));

        private static OverloadedL3Leaves GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(OverloadedL3Leaves))
                {
                    return (OverloadedL3Leaves)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(OverloadedL3Leaves)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
        }

        [TestMethod]
        public void TestConstructor()
        {
            Graph tree = new();
            CountedCollection<DemandPair> demandPairs = new();
            int maxSize = 10;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new(instance);
            List<Edge<Node>> partialSolution = new();
            CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode = new();

            OverloadedL3Leaves overloadedL3Leaves = new(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            Assert.IsNotNull(overloadedL3Leaves);
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

            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new(null, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new(tree, null, algorithm, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new(tree, demandPairs, null, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new(tree, demandPairs, algorithm, null, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new(tree, demandPairs, algorithm, demandPairsPerNode, null, maxSize); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { OverloadedL3Leaves overloadedL3Leaves = new(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, -1); });
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
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge05 = new(node0, node5);
            Edge<Node> edge09 = new(node0, node9);
            Edge<Node> edge011 = new(node0, node11);
            Edge<Node> edge012 = new(node0, node12);
            Edge<Node> edge013 = new(node0, node13);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge24 = new(node2, node4);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge68 = new(node6, node8);
            Edge<Node> edge910 = new(node9, node10);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node2, node11, tree);
            DemandPair dp2 = new(2, node2, node12, tree);
            DemandPair dp3 = new(3, node13, node2, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new(instance);

            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(overloadedL3Leaves.RunFirstIteration());
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
            Node node7 = new(7);
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge05 = new(node0, node5);
            Edge<Node> edge09 = new(node0, node9);
            Edge<Node> edge011 = new(node0, node11);
            Edge<Node> edge012 = new(node0, node12);
            Edge<Node> edge013 = new(node0, node13);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge24 = new(node2, node4);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge68 = new(node6, node8);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge414 = new(node4, node14);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910, edge414 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node3, node0, tree);
            DemandPair dp2 = new(2, node14, node0, tree);
            DemandPair dp3 = new(3, node13, node7, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp2, node14, node4, MockMeasurements);

            Assert.IsFalse(overloadedL3Leaves.RunLaterIteration());
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
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge05 = new(node0, node5);
            Edge<Node> edge09 = new(node0, node9);
            Edge<Node> edge011 = new(node0, node11);
            Edge<Node> edge012 = new(node0, node12);
            Edge<Node> edge013 = new(node0, node13);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge24 = new(node2, node4);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge68 = new(node6, node8);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge414 = new(node4, node14);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910, edge414 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node3, node0, tree);
            DemandPair dp2 = new(2, node14, node0, tree);
            DemandPair dp3 = new(3, node13, node7, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);

            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.RemoveDemandPair(dp3, MockMeasurements);

            Assert.IsFalse(overloadedL3Leaves.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterI2EdgeContraction()
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
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge05 = new(node0, node5);
            Edge<Node> edge09 = new(node0, node9);
            Edge<Node> edge011 = new(node0, node11);
            Edge<Node> edge012 = new(node0, node12);
            Edge<Node> edge013 = new(node0, node13);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge24 = new(node2, node4);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge68 = new(node6, node8);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge414 = new(node4, node14);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910, edge414 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node3, node0, tree);
            DemandPair dp2 = new(2, node14, node0, tree);
            DemandPair dp3 = new(3, node13, node7, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new(instance);
            GetReductionRuleInAlgorithm(algorithm);
            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.ContractEdge(edge414, MockMeasurements);

            Assert.IsFalse(overloadedL3Leaves.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterI3EdgeContraction()
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
            Node node8 = new(8);
            Node node9 = new(9);
            Node node10 = new(10);
            Node node11 = new(11);
            Node node12 = new(12);
            Node node13 = new(13);
            Node node14 = new(14);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14 }, MockCounter);
            Edge<Node> edge01 = new(node0, node1);
            Edge<Node> edge05 = new(node0, node5);
            Edge<Node> edge09 = new(node0, node9);
            Edge<Node> edge011 = new(node0, node11);
            Edge<Node> edge012 = new(node0, node12);
            Edge<Node> edge013 = new(node0, node13);
            Edge<Node> edge12 = new(node1, node2);
            Edge<Node> edge23 = new(node2, node3);
            Edge<Node> edge24 = new(node2, node4);
            Edge<Node> edge56 = new(node5, node6);
            Edge<Node> edge67 = new(node6, node7);
            Edge<Node> edge68 = new(node6, node8);
            Edge<Node> edge910 = new(node9, node10);
            Edge<Node> edge1314 = new(node13, node14);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910, edge1314 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedCollection<DemandPair> demandPairs = new();
            DemandPair dp1 = new(1, node5, node11, tree);
            DemandPair dp2 = new(2, node12, node5, tree);
            DemandPair dp3 = new(3, node14, node5, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, -1, "", "", tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new(instance);

            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.ContractEdge(edge1314, MockMeasurements);

            Assert.IsTrue(overloadedL3Leaves.RunLaterIteration());
        }
    }
}
