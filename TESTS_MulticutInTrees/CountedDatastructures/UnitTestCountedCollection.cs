// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.CountedDatastructures
{
    [TestClass]
    public class UnitTestCountedCollection
    {
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            CountedCollection<int> countedCollection = new CountedCollection<int>();
            Assert.IsNotNull(countedCollection);
            Assert.AreEqual(0, countedCollection.Count(counter));

            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            countedCollection = new CountedCollection<int>(list, counter);
            Assert.IsNotNull(countedCollection);
            Assert.AreEqual(list.Count, countedCollection.Count(counter));
        }

        [TestMethod]
        public void TestNullArgument()
        {
            int number = 0;
            List<int> list = new List<int>();
            CountedCollection<int> countedCollection = new CountedCollection<int>();

            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.GetCountedEnumerable(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.Add(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.ChangeElement(number, number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.ChangeOccurrence(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.ChangeOccurrence(n => n + 1, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.Clear(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.Contains(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.Count(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.ElementBeforeAndAfter(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.Remove(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.First(n => n == 3, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.First(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.First(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.Last(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.RemoveFromEndWhile(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.RemoveFromEndWhile(n => n < 2, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.RemoveFromStartWhile(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => countedCollection.RemoveFromStartWhile(n => n < 2, null));
        }

        [TestMethod]
        public void TestCount()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            Assert.AreEqual(list.Count, countedCollection.Count(counter));
            Assert.AreEqual(list.Count(n => n % 2 == 0), countedCollection.Count(n => n % 2 == 0, counter));
        }

        [TestMethod]
        public void TestFirst()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            Assert.AreEqual(list[0], countedCollection.First(counter));
            Assert.AreEqual(list.First(n => n > 1000), countedCollection.First(n => n > 1000, counter));
        }

        [TestMethod]
        public void TestLast()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            Assert.AreEqual(list[^1], countedCollection.Last(counter));
        }

        [TestMethod]
        public void TestAdd()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            countedCollection.Add(654465645, counter);
            Assert.ThrowsException<AlreadyPresentException>(() => countedCollection.Add(654465645, counter));
        }

        [TestMethod]
        public void TestContains()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            Assert.IsTrue(countedCollection.Contains(97489, counter));
            Assert.IsFalse(countedCollection.Contains(165516, counter));
        }

        [TestMethod]
        public void TestRemove()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            Assert.IsTrue(countedCollection.Remove(97489, counter));
            Assert.AreEqual(list.Count - 1, countedCollection.Count(counter));
            Assert.IsFalse(countedCollection.Remove(165516, counter));
            Assert.AreEqual(list.Count - 1, countedCollection.Count(counter));
        }

        [TestMethod]
        public void TestClear()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            countedCollection.Clear(counter);
            Assert.AreEqual(0, countedCollection.Count(counter));
        }

        [TestMethod]
        public void TestRemoveFromStartWhile()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            countedCollection.RemoveFromStartWhile(n => n < 1000, counter);
            Assert.AreEqual(list.Count - 3, countedCollection.Count(counter));
        }

        [TestMethod]
        public void TestRemoveFromEndWhile()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            countedCollection.RemoveFromEndWhile(n => n < 10 || n > 1000, counter);
            Assert.AreEqual(list.Count - 4, countedCollection.Count(counter));
        }

        [TestMethod]
        public void TestElementBeforeAndAfter()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 8, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            (int before, int after) = countedCollection.ElementBeforeAndAfter(78, counter);
            Assert.AreEqual(97489, before);
            Assert.AreEqual(789, after);
            (before, after) = countedCollection.ElementBeforeAndAfter(984, counter);
            Assert.AreEqual(0, before);
            Assert.AreEqual(894, after);
            (before, after) = countedCollection.ElementBeforeAndAfter(71050742, counter);
            Assert.AreEqual(8, before);
            Assert.AreEqual(0, after);
            Assert.ThrowsException<InvalidOperationException>(() => { (before, after) = countedCollection.ElementBeforeAndAfter(-2, counter); });
        }

        [TestMethod]
        public void TestChangeOccurrence()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050743 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            countedCollection.ChangeOccurrence(n => n % 2 == 1 ? n + 1 : n, counter);
            Assert.AreEqual(0, countedCollection.Count(n => n % 2 == 1, counter));
            Assert.AreEqual(71050744, countedCollection.Last(counter));
        }

        [TestMethod]
        public void TestChangeElement()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 7, 43, 2, 3517420, 0, 71050742 };
            CountedCollection<int> countedCollection = new CountedCollection<int>(list, counter);
            Assert.ThrowsException<InvalidOperationException>(() => countedCollection.ChangeElement(-1, 1000, counter));
            countedCollection.ChangeElement(71050742, 1000, counter);
            Assert.AreEqual(1000, countedCollection.Last(counter));
        }
    }
}
