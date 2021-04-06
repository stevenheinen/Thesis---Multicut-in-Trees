// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.MulticutProblem
{
    [TestClass]
    public class UnitTestMulticutInstance
    {
        private static readonly Counter MockCounter = new Counter();

        [TestMethod]
        public void TestInternalConstructor()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();

            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, 2);

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestPublicConstructor()
        {
            CommandLineOptions existingOptions = new CommandLineOptions() { RandomSeed = 0, AlgorithmType = AlgorithmType.GuoNiedermeierKernelisation, InputTreeType = InputTreeType.Fixed, InputDemandPairsType = InputDemandPairsType.Fixed, InstanceFilePath = "no.txt", OutputDirectory = Directory.GetCurrentDirectory(), InstanceDirectory = Directory.GetCurrentDirectory(), Verbose = true };

            MethodInfo fileNameMethod = typeof(InstanceReaderWriter).GetMethod("CreateFilePath", BindingFlags.NonPublic | BindingFlags.Static);
            string existingFile = (string)fileNameMethod.Invoke(null, new object[] { 0, existingOptions });

            using (StreamWriter sw = new StreamWriter(existingFile))
            {
                // Write the header with number of nodes, number of dps and optimal k
                sw.WriteLine("// [Number of nodes] [Number of demand pairs] [Optimal K]");
                sw.WriteLine("7 3 2");

                // Write the root of the tree
                sw.WriteLine("// ID of the root of the tree");
                sw.WriteLine("0");

                // Write all edges in the tree
                sw.WriteLine("// Edges, denoted by: [ID of endpoint 1] [ID of endpoint 2]");
                sw.WriteLine("0 1");
                sw.WriteLine("0 2");
                sw.WriteLine("1 3");
                sw.WriteLine("1 4");
                sw.WriteLine("2 5");
                sw.WriteLine("0 6");

                // Write the endpoints of all dps in the tree
                sw.WriteLine("// Demand pairs, denoted by: [ID of endpoint 1] [ID of endpoint 2]");
                sw.WriteLine("3 6");
                sw.WriteLine("4 1");
                sw.WriteLine("4 5");
            }

            MulticutInstance existingInstance = new MulticutInstance(0, existingOptions);
            Assert.IsNotNull(existingInstance);
            Assert.AreEqual(7, existingInstance.Tree.NumberOfNodes(MockCounter));
            Assert.AreEqual(3, existingInstance.DemandPairs.Count(MockCounter));

            using (StreamWriter sw = new StreamWriter("tree.txt"))
            {
                sw.WriteLine("7");
                sw.WriteLine("0 1");
                sw.WriteLine("0 2");
                sw.WriteLine("1 3");
                sw.WriteLine("1 4");
                sw.WriteLine("2 5");
                sw.WriteLine("0 6");
            }

            using (StreamWriter sw = new StreamWriter("dp.txt"))
            {
                sw.WriteLine("3");
                sw.WriteLine("3 6");
                sw.WriteLine("4 1");
                sw.WriteLine("4 5");
            }

            CommandLineOptions notExistingOptions = new CommandLineOptions() { RandomSeed = 0, AlgorithmType = AlgorithmType.GuoNiedermeierKernelisation, InputTreeType = InputTreeType.Fixed, InputDemandPairsType = InputDemandPairsType.Fixed, InstanceFilePath = "tree.txt", DemandPairFilePath = "dp.txt", OutputDirectory = Directory.GetCurrentDirectory(), InstanceDirectory = Directory.GetCurrentDirectory(), Verbose = true };

            string notExistingFile = (string)fileNameMethod.Invoke(null, new object[] { 0, notExistingOptions });
            File.Delete(notExistingFile);

            MulticutInstance notExistingInstance = new MulticutInstance(0, notExistingOptions);
            Assert.IsNotNull(notExistingInstance);
            Assert.AreEqual(7, notExistingInstance.Tree.NumberOfNodes(MockCounter));
            Assert.AreEqual(3, notExistingInstance.DemandPairs.Count(MockCounter));
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 1);

            Random random = new Random(12395);
            Dictionary<(int, int), double> distProb = new Dictionary<(int, int), double>();

            Assert.ThrowsException<ArgumentNullException>(() => new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, null, dps, 2));
            Assert.ThrowsException<ArgumentNullException>(() => new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, null, 2));
            Assert.ThrowsException<ArgumentNullException>(() => new MulticutInstance(0, null));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, -1, tree, dps, -8459472));

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(MulticutInstance).GetMethod("CreateInputTree", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(instance, new object[] { null, random, 5, "" });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentException));

            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(MulticutInstance).GetMethod("CreateInputDemandPairs", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(instance, new object[] { random, tree, null, 5, distProb, "" });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void TestParseLengthDistibutionDictionary()
        {
            Tree<TreeNode> tree = new Tree<TreeNode>();
            CountedList<DemandPair> dps = new CountedList<DemandPair>();
            MulticutInstance instance = new MulticutInstance(InputTreeType.Fixed, InputDemandPairsType.Fixed, 0, tree, dps, 1);
            MethodInfo method = typeof(MulticutInstance).GetMethod("ParseLengthDistributionDictionary", BindingFlags.NonPublic | BindingFlags.Instance);

            string test1 = "shouldNotWork";
            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(instance, new object[] { test1 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentException));

            string test2 = "(0, 1, 0.5), (2, three, 0.274)";
            t = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(instance, new object[] { test2 }));
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentException));

            string test3 = "(14, 51, 0.25), (2, 3, 0.6), (5, 10, 0.1), (51, 99, 0.05)";
            Dictionary<(int, int), double> dict1 = (Dictionary<(int, int), double>)method.Invoke(instance, new object[] { test3 });
            Assert.AreEqual(4, dict1.Count);
            Assert.AreEqual(0.6, dict1[(2, 3)]);

            string test4 = null;
            Dictionary<(int, int), double> dict2 = (Dictionary<(int, int), double>)method.Invoke(instance, new object[] { test4 });
            Assert.AreEqual(0, dict2.Count);
        }
    }
}
