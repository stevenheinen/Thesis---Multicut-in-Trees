// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.Experiments;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;

namespace TESTS_MulticutInTrees.Experiments
{
    [TestClass]
    public class UnitTestExperimentOutputWriter
    {
        [TestMethod]
        public void TestNullParameter()
        {
            ExperimentOutput output = new(1, 1, InputTreeType.Fixed, InputDemandPairsType.Fixed, AlgorithmType.GuoNiedermeierKernelisation, 1, 1, 1, true, 1, 1, new PerformanceMeasurements(nameof(UnitTestExperimentOutputWriter)), new ReadOnlyCollection<PerformanceMeasurements>(new List<PerformanceMeasurements>()));

            Assert.ThrowsException<ArgumentNullException>(() => ExperimentOutputWriter.WriteOutput(null, ""));
            Assert.ThrowsException<ArgumentNullException>(() => ExperimentOutputWriter.WriteOutput(new List<ExperimentOutput>() { output }, null));
            Assert.ThrowsException<ArgumentException>(() => ExperimentOutputWriter.WriteOutput(new List<ExperimentOutput>(), ""));
        }
    }
}
