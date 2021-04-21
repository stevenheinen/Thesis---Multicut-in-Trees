// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;

namespace TESTS_MulticutInTrees.InstanceGeneration
{
    [TestClass]
    public class UnitTestErdosRenyiGraph
    {
        private static readonly Counter counter = new();

        [TestMethod]
        public void TestWrongParameters()
        {
            Random random = new(0);

            Assert.ThrowsException<ArgumentNullException>(() => ErdosRenyiGraph.CreateErdosRenyiGraph(10, 0.5, null));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ErdosRenyiGraph.CreateErdosRenyiGraph(-54, 0.5, random));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ErdosRenyiGraph.CreateErdosRenyiGraph(10, -0.5, random));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ErdosRenyiGraph.CreateErdosRenyiGraph(10, 1.0001, random));
        }

        [TestMethod]
        public void TestCase1()
        {
            Random random = new(654685432);
            int numberOfNodes = 1000;
            double chance = 0.5;
            Graph graph = ErdosRenyiGraph.CreateErdosRenyiGraph(numberOfNodes, chance, random);
            Assert.IsNotNull(graph);
            Assert.AreEqual(numberOfNodes, graph.NumberOfNodes(counter));
        }
    }
}
