// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestUtils
    {
        private readonly static Counter MockCounter = new Counter();

        [TestMethod]
        public void TestNullParameter()
        {
            Node n = new Node(0);
            List<int> list = new List<int>();
            Random random = new Random(0);
            Assert.ThrowsException<ArgumentNullException>(() => Utils.OrderEdgeSmallToLarge((null, n)));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.OrderEdgeSmallToLarge((n, null)));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.IsSubsetOf(null, list));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.IsSubsetOf(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.Print<int>(null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.Print<Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.NodePathToEdgePath<Node>(null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandomWhere<int>(null, n => true, random));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandomWhere(list, null, random));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandomWhere(list, n => true, null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandom<int>(null, random));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.PickRandom(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.BinarySearchGetLastTrue(0, 10, null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.BinarySearchGetFirstTrue(0, 10, null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.AllSubsetsOfSize<int>(null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.CreateTreeWithEdges(10, null));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.CreateDemandPairs(null, new List<(int, int)>()));
            Assert.ThrowsException<ArgumentNullException>(() => Utils.CreateDemandPairs(new Tree<TreeNode>(), null));
        }

        [TestMethod]
        public void TestOrderEdgeSmallToLarge()
        {
            Node n0 = new Node(0);
            Node n1 = new Node(1);

            Assert.AreEqual((n0, n1), Utils.OrderEdgeSmallToLarge((n0, n1)));
            Assert.AreEqual((n0, n1), Utils.OrderEdgeSmallToLarge((n1, n0)));
            Assert.AreEqual((n0, n0), Utils.OrderEdgeSmallToLarge((n0, n0)));
        }

        [TestMethod]
        public void TestNullCheck()
        {
            ArgumentNullException a = Assert.ThrowsException<ArgumentNullException>(() => Utils.NullCheck<Node>(null, "testName", null));
            Assert.IsTrue(a.Message == "Value cannot be null. (Parameter 'testName')");

            Assert.ThrowsException<ArgumentNullException>(() => Utils.NullCheck<Node>(null, null));

            int x = 3;
            Utils.NullCheck(x, nameof(x), "This test should pass");
        }

        [TestMethod]
        public void TestPrint()
        {
            List<int> list = new List<int>();
            Assert.AreEqual("System.Collections.Generic.List`1[System.Int32] with 0 elements.", list.Print());

            list = new List<int>() { 4, 984, 8, 465, 8, 47, 643, 85, 6, 43, 54, 384, 3, 46, 74, -146785, 4, -4, -4, 4, 56, 1, 4, -49 };
            Assert.AreEqual("System.Collections.Generic.List`1[System.Int32] with 24 elements: [4, 984, 8, 465, 8, 47, 643, 85, 6, 43, 54, 384, 3, 46, 74, -146785, 4, -4, -4, 4, 56, 1, 4, -49]", list.Print());
        }

        [TestMethod]
        public void TestPickRandom()
        {
            Random random = new Random(638276819);
            List<int> list = new List<int>() { 6854, 6584, 64, 684, 35, 2173, 814, 98, 14, 631, 18, 7, 6871, 8, 7, 81, 78, 17, 86, 167, 817, 3, 98, 78, 171, 306714107, 43, 714, 07, 7, 737, 54, 0, 54, 07, 1, 04, 453, 08, 3 };
            for (int i = 0; i < 1000; i++)
            {
                int element = list.PickRandom(random);
                Assert.IsTrue(list.Contains(element));
            }
        }

        [TestMethod]
        public void TestPickRandomWhere()
        {
            Random random = new Random(746818141);
            List<int> list = new List<int>() { 6854, 6584, 64, 684, 35, 2173, 814, 98, 14, 631, 18, 7, 6871, 8, 7, 81, 78, 17, 86, 167, 817, 3, 98, 78, 171, 306714107, 43, 714, 07, 7, 737, 54, 0, 54, 07, 1, 04, 453, 08, 3 };
            for (int i = 0; i < 1000; i++)
            {
                int element = list.PickRandomWhere(n => n % 2 == 0, random);
                Assert.IsTrue(element % 2 == 0);
                Assert.IsTrue(list.Contains(element));
            }
        }

        [TestMethod]
        public void TestBinarySearchGetFirstTrue()
        {
            Assert.ThrowsException<ArgumentException>(() => Utils.BinarySearchGetFirstTrue(10, 5, x => x > 7));
            Assert.AreEqual(-1, Utils.BinarySearchGetFirstTrue(0, 100, n => n > 500));
            Assert.AreEqual(0, Utils.BinarySearchGetFirstTrue(0, 100, n => n > -1));
            Assert.AreEqual(100, Utils.BinarySearchGetFirstTrue(0, 100, n => n >= 100));
            Assert.AreEqual(21, Utils.BinarySearchGetFirstTrue(0, 100, n => n >= 21));
            Assert.AreEqual(20, Utils.BinarySearchGetFirstTrue(0, 100, n => n >= 20));
        }

        [TestMethod]
        public void TestBinarySearchGetLastTrue()
        {
            Assert.ThrowsException<ArgumentException>(() => Utils.BinarySearchGetLastTrue(10, 5, x => x > 7));
            Assert.AreEqual(-1, Utils.BinarySearchGetLastTrue(0, 100, n => n > 500));
            Assert.AreEqual(0, Utils.BinarySearchGetLastTrue(0, 100, n => n <= 0));
            Assert.AreEqual(100, Utils.BinarySearchGetLastTrue(0, 100, n => n <= 100));
            Assert.AreEqual(21, Utils.BinarySearchGetLastTrue(0, 100, n => n <= 21));
            Assert.AreEqual(20, Utils.BinarySearchGetLastTrue(0, 100, n => n <= 20));
        }

        [TestMethod]
        public void TestAllSubsetsOfSize()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5 };
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.AllSubsetsOfSize(-1));

            int size = 0;
            IEnumerable<IEnumerable<int>> res = list.AllSubsetsOfSize(size);
            Assert.AreEqual(1, res.Count());
            foreach (IEnumerable<int> subset in res)
            {
                Assert.AreEqual(size, subset.Count());
            }

            size = 5;
            res = list.AllSubsetsOfSize(size);
            Assert.AreEqual(1, res.Count());
            foreach (IEnumerable<int> subset in res)
            {
                Assert.AreEqual(size, subset.Count());
            }

            size = 4;
            res = list.AllSubsetsOfSize(size);
            Assert.AreEqual(5, res.Count());
            foreach (IEnumerable<int> subset in res)
            {
                Assert.AreEqual(size, subset.Count());
            }

            size = 3;
            res = list.AllSubsetsOfSize(size);
            Assert.AreEqual(10, res.Count());
            foreach (IEnumerable<int> subset in res)
            {
                Assert.AreEqual(size, subset.Count());
            }

            size = 2;
            res = list.AllSubsetsOfSize(size);
            Assert.AreEqual(10, res.Count());
            foreach (IEnumerable<int> subset in res)
            {
                Assert.AreEqual(size, subset.Count());
            }

            size = 1;
            res = list.AllSubsetsOfSize(size);
            Assert.AreEqual(5, res.Count());
            foreach (IEnumerable<int> subset in res)
            {
                Assert.AreEqual(size, subset.Count());
            }
        }

        [TestMethod]
        public void TestCreateTreeWithEdges()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Utils.CreateTreeWithEdges(-1, new List<(int, int)>()));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateTreeWithEdges(5, new List<(int, int)>() { (1, 0), (4, 3), (2, 0) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateTreeWithEdges(5, new List<(int, int)>() { (1, 0), (4, 3), (2, 0), (1, 3), (2, 3) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateTreeWithEdges(5, new List<(int, int)>() { (1, 0), (4, -1), (2, 0), (1, 3) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateTreeWithEdges(5, new List<(int, int)>() { (1, 0), (4, 3), (-5, 0), (1, 3) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateTreeWithEdges(5, new List<(int, int)>() { (1, 5), (4, 3), (-5, 0), (1, 3) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateTreeWithEdges(5, new List<(int, int)>() { (5, 0), (4, 3), (-5, 0), (1, 3) }));

            Tree<TreeNode> test = Utils.CreateTreeWithEdges(5, new List<(int, int)>() { (1, 0), (4, 3), (2, 0), (1, 3) });
            Assert.IsNotNull(test);
            Assert.AreEqual(5, test.NumberOfNodes(MockCounter));
            Assert.AreEqual(4, test.NumberOfEdges(MockCounter));
            Assert.AreEqual(3, test.Height(MockCounter));
        }

        [TestMethod]
        public void TestCreateDemandPairs()
        {
            Tree<TreeNode> tree = Utils.CreateTreeWithEdges(5, new List<(int, int)>() { (1, 0), (4, 3), (2, 0), (1, 3) });
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateDemandPairs(tree, new List<(int, int)>() { (1, 0), (4, -1), (2, 0), (1, 3) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateDemandPairs(tree, new List<(int, int)>() { (1, 0), (4, 3), (-5, 0), (1, 3) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateDemandPairs(tree, new List<(int, int)>() { (1, 5), (4, 3), (-5, 0), (1, 3) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateDemandPairs(tree, new List<(int, int)>() { (5, 0), (4, 3), (-5, 0), (1, 3) }));

            tree.RemoveNode(tree.Nodes(MockCounter).First(n => n.ID == 3), MockCounter);

            Assert.ThrowsException<ArgumentException>(() => Utils.CreateDemandPairs(tree, new List<(int, int)>() { (1, 0), (2, 0), (2, 3) }));
            Assert.ThrowsException<ArgumentException>(() => Utils.CreateDemandPairs(tree, new List<(int, int)>() { (1, 0), (3, 0), (2, 1) }));

            CountedList<DemandPair> demandPairs = Utils.CreateDemandPairs(tree, new List<(int, int)>() { (1, 0), (2, 0), (2, 4) });
            Assert.IsNotNull(demandPairs);
            Assert.AreEqual(3, demandPairs.Count(MockCounter));
        }
    }
}
