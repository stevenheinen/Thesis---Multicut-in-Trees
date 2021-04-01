// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.InstanceGeneration
{
    [TestClass]
    public class UnitTestInstanceReaderWriter
    {
        [TestMethod]
        public void TestNullParameter()
        {
            int seed = 243;
            CommandLineOptions options = new CommandLineOptions();
            Tree<TreeNode> tree = new Tree<TreeNode>();
            List<DemandPair> demandPairs = new List<DemandPair>();
            int k = 1;

            Assert.ThrowsException<ArgumentNullException>(() => InstanceReaderWriter.WriteInstance(seed, null, tree, demandPairs, k));
            Assert.ThrowsException<ArgumentNullException>(() => InstanceReaderWriter.WriteInstance(seed, options, null, demandPairs, k));
            Assert.ThrowsException<ArgumentNullException>(() => InstanceReaderWriter.WriteInstance(seed, options, tree, null, k));
            Assert.ThrowsException<ArgumentNullException>(() => InstanceReaderWriter.ReadInstance(seed, null));

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(InstanceReaderWriter).GetMethod("CreateFilePath", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { seed, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestCreateFilePath()
        {
            MethodInfo method = typeof(InstanceReaderWriter).GetMethod("CreateFilePath", BindingFlags.NonPublic | BindingFlags.Static);

            CommandLineOptions options1 = new CommandLineOptions() { InstanceDirectory = "instanceDir", InputTreeType = InputTreeType.Caterpillar, NumberOfNodes = 50, InputDemandPairsType = InputDemandPairsType.Random, NumberOfDemandPairs = 25 };
            string fileName1 = (string)method.Invoke(null, new object[] { 5330, options1 });
            string expected1 = "instanceDir\\MulticutInstance_seed=5330_treeType=Caterpillar_dpType=Random_nrNodes=50_nrDPs=25.txt";
            Assert.AreEqual(expected1, fileName1);

            CommandLineOptions options2 = new CommandLineOptions() { InstanceDirectory = "instanceDir", InputTreeType = InputTreeType.CNFSAT, InstanceFilePath = "D:\\Downloads\\cnf.sat" , InputDemandPairsType = InputDemandPairsType.FromTreeInstance };
            string fileName2 = (string)method.Invoke(null, new object[] { 5330, options2 });
            string expected2 = "instanceDir\\MulticutInstance_seed=5330_treeType=CNFSAT_dpType=FromTreeInstance_treeFile=(D:\\Downloads\\cnf[dot]sat).txt";
            Assert.AreEqual(expected2, fileName2);

            CommandLineOptions options3 = new CommandLineOptions() { InstanceDirectory = "instanceDir", InputTreeType = InputTreeType.Caterpillar, NumberOfNodes = 50, InputDemandPairsType = InputDemandPairsType.Fixed, DemandPairFilePath = "D:\\Documents\\dp.txt" };
            string fileName3 = (string)method.Invoke(null, new object[] { 5330, options3 });
            string expected3 = "instanceDir\\MulticutInstance_seed=5330_treeType=Caterpillar_dpType=Fixed_nrNodes=50_dpFile=(D:\\Documents\\dp[dot]txt).txt";
            Assert.AreEqual(expected3, fileName3);

            CommandLineOptions options4 = new CommandLineOptions() { InstanceDirectory = "instanceDir", InputTreeType = InputTreeType.Prüfer, NumberOfNodes = 50, InputDemandPairsType = InputDemandPairsType.LengthDistribution, DistanceDistribution = "(2, 4, 0.5), (5, 10, 0.2), (50, 100, 0.3)" };
            string fileName4 = (string)method.Invoke(null, new object[] { 5330, options4 });
            string expected4 = "instanceDir\\MulticutInstance_seed=5330_treeType=Prüfer_dpType=LengthDistribution_nrNodes=50_dpLengthDist=((2, 4, 0.5), (5, 10, 0.2), (50, 100, 0.3)).txt";
            Assert.AreEqual(expected4, fileName4);
        }
    }
}
