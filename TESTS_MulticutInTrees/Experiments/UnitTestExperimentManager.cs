// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.Experiments;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.Experiments
{
    [TestClass]
    public class UnitTestExperimentManager
    {
        [TestMethod]
        public void TestNullParameter()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ExperimentManager.RunExperiment(null));

            CommandLineOptions options = new CommandLineOptions();
            Func<int, CommandLineOptions, ExperimentOutput> func = (i, c) => new ExperimentOutput(1, 1, InputTreeType.Fixed, InputDemandPairsType.Fixed, AlgorithmType.GuoNiedermeierKernelisation, 1, 1, 1, true, 1, 1, new PerformanceMeasurements("test"), new ReadOnlyCollection<PerformanceMeasurements>(new List<PerformanceMeasurements>()));

            TargetInvocationException t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("RunMultipleExperiments", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { null, func });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            
            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("RunMultipleExperiments", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { options, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("FormatParseOutput", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { null, 1, options });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));

            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("FormatParseOutput", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { "test", 1, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            
            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("RunGurobiMIPAlgorithm", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { 1, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            
            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("RunBranchingAlgorithm", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { 1, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            
            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("RunBruteForceAlgorithm", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { 1, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            
            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("RunKernelisationAlgorithm", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { 1, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
            
            t = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                MethodInfo method = typeof(ExperimentManager).GetMethod("CreateAlgorithmInstance", BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, new object[] { AlgorithmType.GuoNiedermeierKernelisation, null });
            });
            Assert.IsInstanceOfType(t.InnerException, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void TestGurobiMIPExperiment()
        {
            CommandLineOptions options = new CommandLineOptions() { AlgorithmType = AlgorithmType.GurobiMIPSolver, InputTreeType = InputTreeType.Prufer, NumberOfNodes = 10, InputDemandPairsType = InputDemandPairsType.Random, NumberOfDemandPairs = 6, InstanceDirectory = Directory.GetCurrentDirectory(), OutputDirectory = Directory.GetCurrentDirectory(), RandomSeed = 3, Repetitions = 3, Experiments = 2, Verbose = true };

            ExperimentManager.RunExperiment(options);

            options = new CommandLineOptions() { AlgorithmType = AlgorithmType.GurobiMIPSolver, InputTreeType = InputTreeType.Prufer, NumberOfNodes = 10, InputDemandPairsType = InputDemandPairsType.Random, NumberOfDemandPairs = 6, InstanceDirectory = Directory.GetCurrentDirectory(), OutputDirectory = Directory.GetCurrentDirectory(), RandomSeed = 3, Repetitions = 2, Experiments = 1 };

            ExperimentManager.RunExperiment(options);
        }

        [TestMethod]
        public void TestBruteForceExperiment()
        {
            CommandLineOptions options = new CommandLineOptions() { AlgorithmType = AlgorithmType.BruteForce, InputTreeType = InputTreeType.Prufer, NumberOfNodes = 10, InputDemandPairsType = InputDemandPairsType.Random, NumberOfDemandPairs = 6, InstanceDirectory = Directory.GetCurrentDirectory(), OutputDirectory = Directory.GetCurrentDirectory(), RandomSeed = 3, MaxSolutionSize = 4, Repetitions = 3, Experiments = 2, Verbose = true };

            ExperimentManager.RunExperiment(options);
            
            options = new CommandLineOptions() { AlgorithmType = AlgorithmType.BruteForce, InputTreeType = InputTreeType.Prufer, NumberOfNodes = 10, InputDemandPairsType = InputDemandPairsType.Random, NumberOfDemandPairs = 6, InstanceDirectory = Directory.GetCurrentDirectory(), OutputDirectory = Directory.GetCurrentDirectory(), RandomSeed = 3, MaxSolutionSize = 1, Repetitions = 3, Experiments = 2, Verbose = true };

            ExperimentManager.RunExperiment(options);
        }

        [TestMethod]
        public void TestBranchingAlgorithmExperiment()
        {
            CommandLineOptions options = new CommandLineOptions() { AlgorithmType = AlgorithmType.GuoNiederMeierBranching, InputTreeType = InputTreeType.Prufer, NumberOfNodes = 10, InputDemandPairsType = InputDemandPairsType.Random, NumberOfDemandPairs = 6, InstanceDirectory = Directory.GetCurrentDirectory(), OutputDirectory = Directory.GetCurrentDirectory(), RandomSeed = 3, MaxSolutionSize = 4, Repetitions = 3, Experiments = 2, Verbose = true };

            ExperimentManager.RunExperiment(options);
        }

        [TestMethod]
        public void TestGuoNiedermeierKernelisationExperiment()
        {
            CommandLineOptions options = new CommandLineOptions() { AlgorithmType = AlgorithmType.GuoNiedermeierKernelisation, InputTreeType = InputTreeType.Prufer, NumberOfNodes = 10, InputDemandPairsType = InputDemandPairsType.Random, NumberOfDemandPairs = 6, InstanceDirectory = Directory.GetCurrentDirectory(), OutputDirectory = Directory.GetCurrentDirectory(), RandomSeed = 3, MaxSolutionSize = 4, Repetitions = 3, Experiments = 2, Verbose = true };

            ExperimentManager.RunExperiment(options);
        }
    }
}
