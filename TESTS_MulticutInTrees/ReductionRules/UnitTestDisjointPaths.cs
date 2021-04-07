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

namespace TESTS_MulticutInTrees.ReductionRules
{
    [TestClass]
    public class UnitTestDisjointPaths
    {
        private static readonly Counter MockCounter = new Counter();
        private static readonly PerformanceMeasurements MockMeasurements = new PerformanceMeasurements(nameof(UnitTestDisjointPaths));

        [TestMethod]
        public void TestConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();

            DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, partialSolution, maxSize);
            Assert.IsNotNull(disjointPaths);
        }

        [TestMethod]
        public void TestNullParamter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 10;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 10);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();

            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new DisjointPaths(null, demandPairs, algorithm, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new DisjointPaths(tree, null, algorithm, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, null, partialSolution, maxSize); });
            Assert.ThrowsException<ArgumentNullException>(() => { DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, null, maxSize); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, partialSolution, -1); });

            DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, partialSolution, maxSize);

            Assert.ThrowsException<ArgumentNullException>(() => disjointPaths.AfterDemandPathChanged(null));
            Assert.ThrowsException<ArgumentNullException>(() => disjointPaths.AfterDemandPathRemove(null));
            Assert.ThrowsException<ArgumentNullException>(() => disjointPaths.AfterEdgeContraction(null));
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
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node4, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node4, node5, MockCounter);
            tree.AddChild(node4, node6, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs1 = new CountedList<DemandPair>();
            CountedList<DemandPair> demandPairs2 = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node0, node5);
            DemandPair dp3 = new DemandPair(node4, node6);

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
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();

            DisjointPaths disjointPaths1 = new DisjointPaths(tree, demandPairs1, algorithm1, partialSolution, maxSize);
            DisjointPaths disjointPaths2 = new DisjointPaths(tree, demandPairs2, algorithm2, partialSolution, maxSize);

            Assert.IsTrue(disjointPaths1.RunFirstIteration());
            Assert.IsFalse(disjointPaths2.RunFirstIteration());
        }

        [TestMethod]
        public void TestAfterDemandPairChanged()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            TreeNode node6 = new TreeNode(6);
            tree.AddRoot(node0, MockCounter);
            tree.AddChild(node0, node1, MockCounter);
            tree.AddChild(node0, node4, MockCounter);
            tree.AddChild(node1, node2, MockCounter);
            tree.AddChild(node1, node3, MockCounter);
            tree.AddChild(node4, node5, MockCounter);
            tree.AddChild(node4, node6, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node0, node2);
            DemandPair dp2 = new DemandPair(node0, node5);
            DemandPair dp3 = new DemandPair(node0, node6);

            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);
            demandPairs.Add(dp3, MockCounter);

            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();

            DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, partialSolution, maxSize);

            Assert.IsFalse(disjointPaths.RunFirstIteration());

            algorithm.ChangeEndpointOfDemandPair(dp3, node0, node4, MockMeasurements);

            PropertyInfo list = typeof(Algorithm).GetProperty("LastChangedEdgesPerDemandPair", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> info = (CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>)list.GetGetMethod(true).Invoke(algorithm, new object[] { });

            Assert.IsTrue(disjointPaths.AfterDemandPathChanged(info));
        }

        [TestMethod]
        public void TestAfterDemandPairRemoved()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            int maxSize = 2;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 2);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();
            DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, partialSolution, maxSize);

            Assert.IsFalse(disjointPaths.AfterDemandPathRemove(new CountedList<DemandPair>()));
        }

        [TestMethod]
        public void TestAfterEdgeContraction()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            TreeNode node0 = new TreeNode(0);
            TreeNode node1 = new TreeNode(1);
            TreeNode node2 = new TreeNode(2);
            TreeNode node3 = new TreeNode(3);
            TreeNode node4 = new TreeNode(4);
            TreeNode node5 = new TreeNode(5);
            tree.AddRoot(node2, MockCounter);
            tree.AddChild(node2, node1, MockCounter);
            tree.AddChild(node2, node0, MockCounter);
            tree.AddChild(node2, node3, MockCounter);
            tree.AddChild(node3, node4, MockCounter);
            tree.AddChild(node3, node5, MockCounter);
            tree.UpdateNodeTypes();

            CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
            DemandPair dp1 = new DemandPair(node0, node4);
            DemandPair dp2 = new DemandPair(node1, node5);

            demandPairs.Add(dp1, MockCounter);
            demandPairs.Add(dp2, MockCounter);

            int maxSize = 1;
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, demandPairs, maxSize, 1);
            GuoNiedermeierKernelisation algorithm = new GuoNiedermeierKernelisation(instance);
            List<(TreeNode, TreeNode)> partialSolution = new List<(TreeNode, TreeNode)>();

            DisjointPaths disjointPaths = new DisjointPaths(tree, demandPairs, algorithm, partialSolution, maxSize);

            Assert.IsFalse(disjointPaths.RunFirstIteration());

            algorithm.ContractEdge((node2, node3), MockMeasurements);

            PropertyInfo list = typeof(Algorithm).GetProperty("LastContractedEdges", BindingFlags.NonPublic | BindingFlags.Instance);
            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> info = (CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>)list.GetGetMethod(true).Invoke(algorithm, new object[] { });
            
            Assert.IsTrue(disjointPaths.AfterEdgeContraction(info));
        }
    }
}
