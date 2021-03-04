// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.CountedDatastructures
{
    [TestClass]
    public class UnitTestCountedList
    {
        private readonly static Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            CountedList<int> countedList = new CountedList<int>();
            Assert.IsNotNull(countedList);
            Assert.AreEqual(0, countedList.Count(counter));

            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 748, 7, 43, 2, 3517420, 0, 71050742 };
            countedList = new CountedList<int>(list, counter);
            Assert.IsNotNull(countedList);
            Assert.AreEqual(list.Count, countedList.Count(counter));
        }

        [TestMethod]
        public void TestNullArgument()
        {
            int number = 0;
            List<int> list = new List<int>();
            CountedList<int> countedList = new CountedList<int>();

            Assert.ThrowsException<ArgumentNullException>(() => countedList.GetCountedEnumerable(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.Add(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.AddRange(null, counter));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.AddRange(list, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.Clear(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.Contains(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.Count(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.Insert(number, number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.Remove(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedList.RemoveAt(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => { int a = countedList[number, null]; });
            Assert.ThrowsException<ArgumentNullException>(() => { countedList[number, null] = number; });
        }

        [TestMethod]
        public void TestIndex()
        {
            CountedList<int> countedList = new CountedList<int>();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { int a = countedList[8, counter]; });
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { countedList[8, counter] = 8; });

            countedList.Add(68574, counter);
            Assert.AreEqual(68574, countedList[0, counter]);
            countedList[0, counter] = 759679;
            Assert.AreEqual(759679, countedList[0, counter]);
        }

        [TestMethod]
        public void TestRemoveAt()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 748, 7, 43, 2, 3517420, 0, 71050742 };
            CountedList<int> countedList = new CountedList<int>(list, counter);
            List<int> wantedList = new List<int>() { 984, 894, 897, 98479, 748, 74, 97489, 78, 789, 748, 7, 43, 2, 3517420, 0, 71050742 };
            countedList.RemoveAt(4, counter);
            CollectionAssert.AreEqual(wantedList, countedList.GetInternalList());
        }

        [TestMethod]
        public void TestInsert()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 748, 7, 43, 2, 3517420, 0, 71050742 };
            CountedList<int> countedList = new CountedList<int>(list, counter);
            List<int> wantedList = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 999999999, 748, 7, 43, 2, 3517420, 0, 71050742 };
            countedList.Insert(10, 999999999, counter);
            CollectionAssert.AreEqual(wantedList, countedList.GetInternalList());
        }

        [TestMethod]
        public void TestContains()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 748, 7, 43, 2, 3517420, 0, 71050742 };
            CountedList<int> countedList = new CountedList<int>(list, counter);
            Assert.IsTrue(countedList.Contains(97489, counter));
            Assert.IsFalse(countedList.Contains(165516, counter));
        }

        [TestMethod]
        public void TestRemove()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 748, 7, 43, 2, 3517420, 0, 71050742 };
            CountedList<int> countedList = new CountedList<int>(list, counter);
            Assert.IsTrue(countedList.Remove(97489, counter));
            Assert.AreEqual(list.Count - 1, countedList.Count(counter));
            Assert.IsFalse(countedList.Remove(165516, counter));
            Assert.AreEqual(list.Count - 1, countedList.Count(counter));
        }

        [TestMethod]
        public void TestClear()
        {
            List<int> list = new List<int>() { 984, 894, 897, 98479, 749, 748, 74, 97489, 78, 789, 748, 7, 43, 2, 3517420, 0, 71050742 };
            CountedList<int> countedList = new CountedList<int>(list, counter);
            countedList.Clear(counter);
            Assert.AreEqual(0, countedList.Count(counter));
        }
    }
}
