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
    public class UnitTestOverloadedCaterpillar
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestOverloadedCaterpillar));

        private OverloadedCaterpillar GetReductionRuleInAlgorithm(Algorithm algorithm)
        {
            MethodInfo runPropertySet = typeof(ReductionRule).GetProperty("HasRun", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
            foreach (ReductionRule rr in algorithm.ReductionRules)
            {
                runPropertySet.Invoke(rr, new object[] { true });

                if (rr.GetType() == typeof(OverloadedCaterpillar))
                {
                    return (OverloadedCaterpillar)rr;
                }
            }

            throw new Exception($"Could not find a Reduction Rule of type {typeof(OverloadedCaterpillar)} in this algorithm. It has Reduction Rules: {algorithm.ReductionRules.Print()}.");
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
            CountedDictionary<Node, int> caterpillarComponentPerNode = new CountedDictionary<Node, int>();

            OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize);

            Assert.IsNotNull(overloadedCaterpillar);
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
            CountedDictionary<Node, int> caterpillarComponentPerNode = new CountedDictionary<Node, int>();

            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(null, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, null, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, null, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, null, caterpillarComponentPerNode, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, null, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, null, maxSize); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { OverloadedCaterpillar overloadedCaterpillar = new OverloadedCaterpillar(tree, demandPairs, algorithm, demandPairsPerNode, caterpillarComponentPerNode, partialSolution, -1); });
        }

        [TestMethod]
        public void TestRunFirstIteration1()
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
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22 }, MockCounter);
            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);
            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node7, node2, tree);
            DemandPair dp2 = new DemandPair(2, node7, node12, tree);
            DemandPair dp3 = new DemandPair(3, node7, node13, tree);
            DemandPair dp4 = new DemandPair(4, node7, node3, tree);
            DemandPair dp5 = new DemandPair(5, node7, node4, tree);
            DemandPair dp6 = new DemandPair(6, node7, node9, tree);
            DemandPair dp7 = new DemandPair(7, node7, node21, tree);
            DemandPair dp8 = new DemandPair(8, node7, node18, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);
            demandPairs.Add(dp6, MockCounter);
            demandPairs.Add(dp7, MockCounter);
            demandPairs.Add(dp8, MockCounter);

            int maxSize = 4;

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 4);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedCaterpillar overloadedCaterpillar = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsTrue(overloadedCaterpillar.RunFirstIteration());
        }

        [TestMethod]
        public void TestRunFirstIteration2()
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
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22 }, MockCounter);
            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);
            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node7, node2, tree);
            DemandPair dp2 = new DemandPair(2, node7, node12, tree);
            DemandPair dp3 = new DemandPair(3, node7, node19, tree);
            DemandPair dp4 = new DemandPair(4, node7, node3, tree);
            DemandPair dp5 = new DemandPair(5, node7, node4, tree);
            DemandPair dp6 = new DemandPair(6, node7, node9, tree);
            DemandPair dp7 = new DemandPair(7, node7, node21, tree);
            DemandPair dp8 = new DemandPair(8, node7, node18, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);
            demandPairs.Add(dp6, MockCounter);
            demandPairs.Add(dp7, MockCounter);
            demandPairs.Add(dp8, MockCounter);

            int maxSize = 4;

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 4);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedCaterpillar overloadedCaterpillar = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedCaterpillar.RunFirstIteration());
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
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22 }, MockCounter);
            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);
            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node7, node1, tree);
            DemandPair dp2 = new DemandPair(2, node7, node12, tree);
            DemandPair dp3 = new DemandPair(3, node7, node13, tree);
            DemandPair dp4 = new DemandPair(4, node7, node3, tree);
            DemandPair dp5 = new DemandPair(5, node7, node4, tree);
            DemandPair dp6 = new DemandPair(6, node7, node9, tree);
            DemandPair dp7 = new DemandPair(7, node7, node21, tree);
            DemandPair dp8 = new DemandPair(8, node7, node18, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);
            demandPairs.Add(dp6, MockCounter);
            demandPairs.Add(dp7, MockCounter);
            demandPairs.Add(dp8, MockCounter);

            int maxSize = 4;

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 4);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedCaterpillar overloadedCaterpillar = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedCaterpillar.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp1, node1, node2, MockMeasurements);

            Assert.IsTrue(overloadedCaterpillar.RunLaterIteration());
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
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            Node node22 = new Node(22);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21, node22 }, MockCounter);
            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            Edge<Node> edge1022 = new Edge<Node>(node10, node22);
            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921, edge1022 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node7, node1, tree);
            DemandPair dp2 = new DemandPair(2, node7, node12, tree);
            DemandPair dp3 = new DemandPair(3, node7, node13, tree);
            DemandPair dp4 = new DemandPair(4, node7, node3, tree);
            DemandPair dp5 = new DemandPair(5, node7, node4, tree);
            DemandPair dp6 = new DemandPair(6, node7, node9, tree);
            DemandPair dp7 = new DemandPair(7, node7, node21, tree);
            DemandPair dp8 = new DemandPair(8, node7, node18, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);
            demandPairs.Add(dp6, MockCounter);
            demandPairs.Add(dp7, MockCounter);
            demandPairs.Add(dp8, MockCounter);

            int maxSize = 4;

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 4);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedCaterpillar overloadedCaterpillar = GetReductionRuleInAlgorithm(algorithm);

            algorithm.RemoveDemandPair(dp3, MockMeasurements);

            Assert.IsFalse(overloadedCaterpillar.RunLaterIteration());
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
            Node node6 = new Node(6);
            Node node7 = new Node(7);
            Node node8 = new Node(8);
            Node node9 = new Node(9);
            Node node10 = new Node(10);
            Node node11 = new Node(11);
            Node node12 = new Node(12);
            Node node13 = new Node(13);
            Node node14 = new Node(14);
            Node node15 = new Node(15);
            Node node16 = new Node(16);
            Node node17 = new Node(17);
            Node node18 = new Node(18);
            Node node19 = new Node(19);
            Node node20 = new Node(20);
            Node node21 = new Node(21);
            tree.AddNodes(new List<Node>() { node0, node1, node2, node3, node4, node5, node6, node7, node8, node9, node10, node11, node12, node13, node14, node15, node16, node17, node18, node19, node20, node21 }, MockCounter);
            Edge<Node> edge54 = new Edge<Node>(node5, node4);
            Edge<Node> edge43 = new Edge<Node>(node4, node3);
            Edge<Node> edge414 = new Edge<Node>(node4, node14);
            Edge<Node> edge32 = new Edge<Node>(node3, node2);
            Edge<Node> edge312 = new Edge<Node>(node3, node12);
            Edge<Node> edge313 = new Edge<Node>(node3, node13);
            Edge<Node> edge21 = new Edge<Node>(node2, node1);
            Edge<Node> edge10 = new Edge<Node>(node1, node0);
            Edge<Node> edge111 = new Edge<Node>(node1, node11);
            Edge<Node> edge56 = new Edge<Node>(node5, node6);
            Edge<Node> edge59 = new Edge<Node>(node5, node9);
            Edge<Node> edge515 = new Edge<Node>(node5, node15);
            Edge<Node> edge67 = new Edge<Node>(node6, node7);
            Edge<Node> edge78 = new Edge<Node>(node7, node8);
            Edge<Node> edge716 = new Edge<Node>(node7, node16);
            Edge<Node> edge717 = new Edge<Node>(node7, node17);
            Edge<Node> edge718 = new Edge<Node>(node7, node18);
            Edge<Node> edge819 = new Edge<Node>(node8, node19);
            Edge<Node> edge820 = new Edge<Node>(node8, node20);
            Edge<Node> edge910 = new Edge<Node>(node9, node10);
            Edge<Node> edge921 = new Edge<Node>(node9, node21);
            tree.AddEdges(new List<Edge<Node>>() { edge54, edge43, edge414, edge32, edge312, edge313, edge21, edge10, edge111, edge56, edge59, edge515, edge67, edge78, edge716, edge717, edge718, edge819, edge820, edge910, edge921 }, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(1, node20, node4, tree);
            DemandPair dp2 = new DemandPair(2, node20, node12, tree);
            DemandPair dp3 = new DemandPair(3, node20, node6, tree);
            DemandPair dp4 = new DemandPair(4, node20, node17, tree);
            DemandPair dp5 = new DemandPair(5, node20, node0, tree);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);
            demandPairs.Add(dp4, MockCounter);
            demandPairs.Add(dp5, MockCounter);

            int maxSize = 3;

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 3);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);

            OverloadedCaterpillar overloadedCaterpillar = GetReductionRuleInAlgorithm(algorithm);

            Assert.IsFalse(overloadedCaterpillar.RunFirstIteration());

            algorithm.ContractEdge(edge59, MockMeasurements);

            Assert.IsTrue(overloadedCaterpillar.RunLaterIteration());
        }
    }
}
