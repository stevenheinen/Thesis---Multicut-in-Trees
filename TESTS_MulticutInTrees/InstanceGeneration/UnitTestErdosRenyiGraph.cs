// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.InstanceGeneration
{
    [TestClass]
    public class UnitTestErdosRenyiGraph
    {
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestWrongParameters()
        {
            Random random = new Random(0);

            Assert.ThrowsException<ArgumentNullException>(() => { Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(10, 0.5, null); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(-54, 0.5, random); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(10, -0.5, random); });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(10, 1.0001, random); });
        }

        [TestMethod]
        public void TestCase1()
        {
            Random random = new Random(654685432);
            int numberOfNodes = 1000;
            double chance = 0.5;
            Graph<Node> graph = ErdosRenyiGraph.CreateErdosRenyiGraph(numberOfNodes, chance, random);
            Assert.IsNotNull(graph);
            Assert.AreEqual(numberOfNodes, graph.NumberOfNodes(counter));
        }
    }
}
