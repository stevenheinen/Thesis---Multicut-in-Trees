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
            CommandLineOptions options = new();
            Graph tree = new();
            List<DemandPair> demandPairs = new();
            int k = 1;

            Assert.ThrowsException<ArgumentNullException>(() => InstanceReaderWriter.WriteInstance(seed, seed, null, tree, demandPairs, k));
            Assert.ThrowsException<ArgumentNullException>(() => InstanceReaderWriter.WriteInstance(seed, seed, options, null, demandPairs, k));
            Assert.ThrowsException<ArgumentNullException>(() => InstanceReaderWriter.WriteInstance(seed, seed, options, tree, null, k));
            Assert.ThrowsException<ArgumentNullException>(() => InstanceReaderWriter.ReadInstance(seed, seed, null));

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(InstanceReaderWriter).GetMethod("CreateFilePath", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { seed, seed, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestCreateFilePath()
        {
            MethodInfo method = typeof(InstanceReaderWriter).GetMethod("CreateFilePath", BindingFlags.NonPublic | BindingFlags.Static);

            CommandLineOptions options1 = new() { InstanceDirectory = "instanceDir", InputTreeType = InputTreeType.Caterpillar, NumberOfNodes = 50, InputDemandPairsType = InputDemandPairsType.Random, NumberOfDemandPairs = 25 };
            string fileName1 = (string)method.Invoke(null, new object[] { 5330, 414, options1 });
            string expected1 = "instanceDir\\MulticutInstance_treeSeed=5330_dpSeed=414_treeType=Caterpillar_dpType=Random_nrNodes=50_nrDPs=25.txt";
            Assert.AreEqual(expected1, fileName1);

            CommandLineOptions options2 = new() { InstanceDirectory = "instanceDir", InputTreeType = InputTreeType.CNFSAT, InstanceFilePath = "D:\\Downloads\\cnf.sat", InputDemandPairsType = InputDemandPairsType.FromTreeInstance };
            string fileName2 = (string)method.Invoke(null, new object[] { 5330, 414, options2 });
            string expected2 = "instanceDir\\MulticutInstance_treeSeed=5330_dpSeed=414_treeType=CNFSAT_dpType=FromTreeInstance_treeFile=(cnf[dot]sat).txt";
            Assert.AreEqual(expected2, fileName2);

            CommandLineOptions options3 = new() { InstanceDirectory = "instanceDir", InputTreeType = InputTreeType.Caterpillar, NumberOfNodes = 50, InputDemandPairsType = InputDemandPairsType.Fixed, DemandPairFilePath = "D:\\Documents\\dp.txt" };
            string fileName3 = (string)method.Invoke(null, new object[] { 5330, 414, options3 });
            string expected3 = "instanceDir\\MulticutInstance_treeSeed=5330_dpSeed=414_treeType=Caterpillar_dpType=Fixed_nrNodes=50_dpFile=(dp[dot]txt).txt";
            Assert.AreEqual(expected3, fileName3);

            CommandLineOptions options4 = new() { InstanceDirectory = "instanceDir", InputTreeType = InputTreeType.Prufer, NumberOfNodes = 50, NumberOfDemandPairs = 10, InputDemandPairsType = InputDemandPairsType.LengthDistribution, DistanceDistribution = "(2, 4, 0.5), (5, 10, 0.2), (50, 100, 0.3)" };
            string fileName4 = (string)method.Invoke(null, new object[] { 5330, 414, options4 });
            string expected4 = "instanceDir\\MulticutInstance_treeSeed=5330_dpSeed=414_treeType=Prufer_dpType=LengthDistribution_nrNodes=50_nrDPs=10_dpLengthDist=((2, 4, 0.5), (5, 10, 0.2), (50, 100, 0.3)).txt";
            Assert.AreEqual(expected4, fileName4);
        }
    }
}
