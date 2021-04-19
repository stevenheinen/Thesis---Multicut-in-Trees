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
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestOverloadedCaterpillar));

        private OverloadedL3Leaves GetReductionRuleInAlgorithm(Algorithm algorithm)
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
            Graph tree = new Graph();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<Edge<Node>> partialSolution = new List<Edge<Node>>();
            CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode = new CountedDictionary<Node, CountedCollection<DemandPair>>();

            OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize);

            Assert.IsNotNull(overloadedL3Leaves);
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
            CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode = new CountedDictionary<Node, CountedCollection<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(null, demandPairs, algorithm, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, null, algorithm, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, null, demandPairsPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, null, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, null, maxSize); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { OverloadedL3Leaves overloadedL3Leaves = new OverloadedL3Leaves(tree, demandPairs, algorithm, demandPairsPerNode, partialSolution, -1); });
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
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge05 = new Edge<Node>(node0, node5);
            Edge<Node> edge09 = new Edge<Node>(node0, node9);
            Edge<Node> edge011 = new Edge<Node>(node0, node11);
            Edge<Node> edge012 = new Edge<Node>(node0, node12);
            Edge<Node> edge013 = new Edge<Node>(node0, node13);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node2, node11, tree);
            DemandPair dp2 = new DemandPair(2, node2, node12, tree);
            DemandPair dp3 = new DemandPair(3, node13, node2, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(overloadedL3Leaves.RunFirstIteration());
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
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge05 = new Edge<Node>(node0, node5);
            Edge<Node> edge09 = new Edge<Node>(node0, node9);
            Edge<Node> edge011 = new Edge<Node>(node0, node11);
            Edge<Node> edge012 = new Edge<Node>(node0, node12);
            Edge<Node> edge013 = new Edge<Node>(node0, node13);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910, edge414 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node3, node0, tree);
            DemandPair dp2 = new DemandPair(2, node14, node0, tree);
            DemandPair dp3 = new DemandPair(3, node13, node7, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp2, node14, node4, MockMeasurements);

            Assert.IsFalse(overloadedL3Leaves.RunLaterIteration());
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
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge05 = new Edge<Node>(node0, node5);
            Edge<Node> edge09 = new Edge<Node>(node0, node9);
            Edge<Node> edge011 = new Edge<Node>(node0, node11);
            Edge<Node> edge012 = new Edge<Node>(node0, node12);
            Edge<Node> edge013 = new Edge<Node>(node0, node13);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910, edge414 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node3, node0, tree);
            DemandPair dp2 = new DemandPair(2, node14, node0, tree);
            DemandPair dp3 = new DemandPair(3, node13, node7, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.RemoveDemandPair(dp3, MockMeasurements);

            Assert.IsFalse(overloadedL3Leaves.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterI2EdgeContraction()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge05 = new Edge<Node>(node0, node5);
            Edge<Node> edge09 = new Edge<Node>(node0, node9);
            Edge<Node> edge011 = new Edge<Node>(node0, node11);
            Edge<Node> edge012 = new Edge<Node>(node0, node12);
            Edge<Node> edge013 = new Edge<Node>(node0, node13);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910, edge414 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node3, node0, tree);
            DemandPair dp2 = new DemandPair(2, node14, node0, tree);
            DemandPair dp3 = new DemandPair(3, node13, node7, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            GetReductionRuleInAlgorithm(algorithm);
            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.ContractEdge(edge414, MockMeasurements);

            Assert.IsFalse(overloadedL3Leaves.RunLaterIteration());
        }

        [TestMethod]
        public void TestAfterI3EdgeContraction()
        {
            Graph tree = new Graph();
            Node node0 = new Node(0);
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            Node node4 = new Node(4);
            Node node5 = new Node(5);
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14 }, MockCounter);
            Edge<Node> edge01 = new Edge<Node>(node0, node1);
            Edge<Node> edge05 = new Edge<Node>(node0, node5);
            Edge<Node> edge09 = new Edge<Node>(node0, node9);
            Edge<Node> edge011 = new Edge<Node>(node0, node11);
            Edge<Node> edge012 = new Edge<Node>(node0, node12);
            Edge<Node> edge013 = new Edge<Node>(node0, node13);
            Edge<Node> edge12 = new Edge<Node>(node1, node2);
            Edge<Node> edge23 = new Edge<Node>(node2, node3);
            Edge<Node> edge24 = new Edge<Node>(node2, node4);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge68 = new Edge<Node>(node6, node8);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge1314 = new Edge<Node>(node13, node14);
            tree.AddEdges(new List<Edge<Node>>() { edge01, edge05, edge09, edge011, edge012, edge013, edge12, edge23, edge24, edge56, edge67, edge68, edge910, edge1314 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node5, node11, tree);
            DemandPair dp2 = new DemandPair(2, node12, node5, tree);
            DemandPair dp3 = new DemandPair(3, node14, node5, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedL3Leaves overloadedL3Leaves = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedL3Leaves.RunFirstIteration());

            algorithm.ContractEdge(edge1314, MockMeasurements);

            Assert.IsTrue(overloadedL3Leaves.RunLaterIteration());
        }
    }
}
