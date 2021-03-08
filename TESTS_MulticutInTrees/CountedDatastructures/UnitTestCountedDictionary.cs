// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CountedDatastructures;

namespace TESTS_MulticutInTrees.CountedDatastructures
{
    [TestClass]
    public class UnitTestCountedDictionary
    {
        private static readonly Counter counter = new Counter();

        [TestMethod]
        public void TestConstructor()
        {
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>();
            Assert.IsNotNull(countedDictionary);
            Assert.AreEqual(0, countedDictionary.Count(counter));

            Dictionary<int, int> dictionary = new Dictionary<int, int>() { { 984, 894 }, { 897, 98479 }, { 749, 748 }, { 74, 97489 }, { 78, 789 }, { 748, 7 }, { 43, 2 }, { 3517420, 0 } };
            countedDictionary = new CountedDictionary<int, int>(dictionary, counter);
            Assert.IsNotNull(countedDictionary);
            Assert.AreEqual(dictionary.Count, countedDictionary.Count(counter));
        }

        [TestMethod]
        public void TestNullArgument()
        {
            int number = 0;
            List<int> list = new List<int>();
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>();

            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.GetCountedEnumerable(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.Add(number, number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.Clear(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.ContainsKey(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.Count(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.Remove(number, null));
            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.GetKeys(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.GetValues(null));
            Assert.ThrowsException<ArgumentNullException>(() => countedDictionary.TryGetValue(number, out int a, null));
            Assert.ThrowsException<ArgumentNullException>(() => { int a = countedDictionary[number, null]; });
            Assert.ThrowsException<ArgumentNullException>(() => { countedDictionary[number, null] = number; });
        }

        [TestMethod]
        public void TestGetKeys()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>() { { 984, 894 }, { 897, 98479 }, { 749, 748 }, { 74, 97489 }, { 78, 789 }, { 748, 7 }, { 43, 2 }, { 3517420, 0 } };
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>(dictionary, counter);
            List<int> expectedKeys = new List<int>() { 984, 897, 749, 74, 78, 748, 43, 3517420 };
            CollectionAssert.AreEqual(expectedKeys, new List<int>(countedDictionary.GetKeys(counter)));
        }

        [TestMethod]
        public void TestGetValues()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>() { { 984, 894 }, { 897, 98479 }, { 749, 748 }, { 74, 97489 }, { 78, 789 }, { 748, 7 }, { 43, 2 }, { 3517420, 0 } };
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>(dictionary, counter);
            List<int> expectedValues = new List<int>() { 894, 98479, 748, 97489, 789, 7, 2, 0 };
            CollectionAssert.AreEqual(expectedValues, new List<int>(countedDictionary.GetValues(counter)));
        }

        [TestMethod]
        public void TestIndex()
        {
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>();

            Assert.ThrowsException<KeyNotFoundException>(() => { int a = countedDictionary[8, counter]; });

            countedDictionary.Add(68574, 325, counter);
            Assert.AreEqual(325, countedDictionary[68574, counter]);
            countedDictionary[9854, counter] = 759679;
            Assert.AreEqual(759679, countedDictionary[9854, counter]);
        }

        [TestMethod]
        public void TestContainsKey()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>() { { 984, 894 }, { 897, 98479 }, { 749, 748 }, { 74, 97489 }, { 78, 789 }, { 748, 7 }, { 43, 2 }, { 3517420, 0 } };
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>(dictionary, counter);
            Assert.IsTrue(countedDictionary.ContainsKey(74, counter));
            Assert.IsFalse(countedDictionary.ContainsKey(165516, counter));
        }

        [TestMethod]
        public void TestRemove()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>() { { 984, 894 }, { 897, 98479 }, { 749, 748 }, { 74, 97489 }, { 78, 789 }, { 748, 7 }, { 43, 2 }, { 3517420, 0 } };
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>(dictionary, counter);
            Assert.IsTrue(countedDictionary.Remove(74, counter));
            Assert.AreEqual(dictionary.Count - 1, countedDictionary.Count(counter));
            Assert.IsFalse(countedDictionary.Remove(165516, counter));
            Assert.AreEqual(dictionary.Count - 1, countedDictionary.Count(counter));
        }

        [TestMethod]
        public void TestTryGetValue()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>() { { 984, 894 }, { 897, 98479 }, { 749, 748 }, { 74, 97489 }, { 78, 789 }, { 748, 7 }, { 43, 2 }, { 3517420, 0 } };
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>(dictionary, counter);
            Assert.IsTrue(countedDictionary.TryGetValue(74, out int res, counter));
            Assert.AreEqual(97489, res);
            Assert.IsFalse(countedDictionary.TryGetValue(165516, out int res2, counter));
            Assert.AreEqual(0, res2);
        }


        [TestMethod]
        public void TestClear()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>() { { 984, 894 }, { 897, 98479 }, { 749, 748 }, { 74, 97489 }, { 78, 789 }, { 748, 7 }, { 43, 2 }, { 3517420, 0 } };
            CountedDictionary<int, int> countedDictionary = new CountedDictionary<int, int>(dictionary, counter);
            countedDictionary.Clear(counter);
            Assert.AreEqual(0, countedDictionary.Count(counter));
        }
    }
}
