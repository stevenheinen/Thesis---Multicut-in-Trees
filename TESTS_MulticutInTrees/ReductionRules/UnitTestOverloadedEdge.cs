// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestOverloadedEdge
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestOverloadedEdge));

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            OverloadedEdge overloadedEdge = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            Assert.IsNotNull(overloadedEdge);
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(null, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, null, algorithm, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, demandPairs, null, partialSolution, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, demandPairs, algorithm, null, maxSize, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, -56, demandPairsPerEdge); });
            Assert.ThrowsException<ArgumentNullException>(() => { OverloadedEdge oe = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, null); });

            OverloadedEdge overloadedEdge = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            Assert.ThrowsException<ArgumentNullException>(() => overloadedEdge.AfterDemandPathChanged(null));
            Assert.ThrowsException<ArgumentNullException>(() => overloadedEdge.AfterEdgeContraction(null));
            Assert.ThrowsException<ArgumentNullException>(() => overloadedEdge.AfterDemandPathRemove(null));
        }

        [TestMethod]
        public void TestFirstIteration()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node0, node3);
            DemandPair dp3 = new DemandPair(node0, node4);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                { (node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter) },
                { (node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, MockCounter) },
                { (node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, MockCounter) },
                { (node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>() { dp3 }, MockCounter) },
            }, MockCounter);

            OverloadedEdge overloadedEdge = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            Assert.IsTrue(overloadedEdge.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPathChanged1()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node0, node3);
            DemandPair dp3 = new DemandPair(node0, node1);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>(new Dictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>()
            {
                { (node0, node1), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1, dp2, dp3 }, MockCounter) },
                { (node1, node2), new CountedCollection<DemandPair>(new List<DemandPair>() { dp1 }, MockCounter) },
                { (node1, node3), new CountedCollection<DemandPair>(new List<DemandPair>() { dp2 }, MockCounter) },
                { (node1, node4), new CountedCollection<DemandPair>(new List<DemandPair>(), MockCounter) },
            }, MockCounter);

            OverloadedEdge overloadedEdge = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> info = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>();
            CountedList<(TreeNode, TreeNode)> affectedEdges = new CountedList<(TreeNode, TreeNode)>();
            affectedEdges.Add((node1, node4), MockCounter);
            info.Add((affectedEdges, dp3), MockCounter);

            Assert.IsFalse(overloadedEdge.AfterDemandPathChanged(info));
        }

        [TestMethod]
        public void TestAfterDemandPathChanged2()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.AddChild(node4, node5, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node0, node3);
            DemandPair dp3 = new DemandPair(node0, node5);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            
            CountedDictionary <(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedEdge overloadedEdge = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            Assert.IsFalse(overloadedEdge.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp3, node5, node4, MockMeasurements);

            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> info = (CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>)typeof(Algorithm).GetProperty("LastChangedEdgesPerDemandPair", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsTrue(overloadedEdge.AfterDemandPathChanged(info));
        }

        [TestMethod]
        public void TestAfterDemandPairRemoved()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();

            OverloadedEdge overloadedEdge = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            Assert.IsFalse(overloadedEdge.AfterDemandPathRemove(new CountedList<DemandPair>()));
        }

        [TestMethod]
        public void TestAfterEdgeContracted()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node5, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node1, node4, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node5, node2);
            DemandPair dp2 = new DemandPair(node5, node3);
            DemandPair dp3 = new DemandPair(node5, node1);
            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(tree, demandPairs, maxSize);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge = (CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>)typeof(GuoNiedermeierKernelisation).GetProperty("DemandPairsPerEdge", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            OverloadedEdge overloadedEdge = new OverloadedEdge(tree, demandPairs, algorithm, partialSolution, maxSize, demandPairsPerEdge);

            Assert.IsFalse(overloadedEdge.RunFirstIteration());

            algorithm.ContractEdge((node0, node1), MockMeasurements);

            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> info = (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>)typeof(Algorithm).GetProperty("LastContractedEdges", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsTrue(overloadedEdge.AfterEdgeContraction(info));
        }
    }
}
