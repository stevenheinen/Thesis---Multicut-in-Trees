// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace TESTS_MulticutInTrees.Utilities
{
    [TestClass]
    public class UnitTestFisherYates
    {
        [TestMethod]
        public void CheckReadOnly()
        {
            Random random = new(6);

            List<int> list = new() { 7987, 4, 844, 54, 658, 1, 68, 135, 1745, 5324, 654, 15, 143, 514, 2414, 25, 24, 2512402, 125, 45, 441, 245, 41, 51, 5, 0, 7, 14705704 };

            Assert.ThrowsException<NotSupportedException>(() =>
            {
                ReadOnlyCollection<int> roc = list.AsReadOnly();
                roc.Shuffle(random);
            });

            Assert.ThrowsException<NotSupportedException>(() =>
            {
                ImmutableList<int> immutable = ImmutableList.ToImmutableList(list);
                immutable.Shuffle(random);
            });

            Assert.ThrowsException<NotSupportedException>(() =>
            {
                int[] array = list.ToArray();
                ImmutableArray<int> immutableArray = ImmutableArray.ToImmutableArray(array);
                immutableArray.Shuffle(random);
            });

            Assert.ThrowsException<NotSupportedException>(() =>
            {
                FisherYates.Shuffle(list.AsReadOnly(), random);
            });

            Assert.ThrowsException<NotSupportedException>(() =>
            {
                list.AsReadOnly().Shuffle(random);
            });
        }

        [TestMethod]
        public void TestShuffle()
        {
            Random random = new(697814);

            List<int> originalList = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            List<int> list = new(originalList);
            list.Shuffle(random);

            foreach (int i in originalList)
            {
                if (!list.Contains(i))
                {
                    Assert.Fail($"Original list contains the element {i}, but the shuffled list does not!");
                }
            }

            foreach (int i in list)
            {
                if (!originalList.Contains(i))
                {
                    Assert.Fail($"Shuffled list contains the element {i}, but the original list does not!");
                }
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i] == list[j])
                    {
                        Assert.Fail($"The shuffled list contains the duplicate element {list[i]}!");
                    }
                }
            }
        }

        [TestMethod]
        public void TestOtherElementTypes()
        {
            Random random = new(6543);

            RootedTreeNode t0 = new(0);
            RootedTreeNode t1 = new(1);
            RootedTreeNode t2 = new(2);
            RootedTreeNode t3 = new(3);
            RootedTreeNode t4 = new(4);
            RootedTreeNode t5 = new(5);
            RootedTreeNode t6 = new(6);

            List<RootedTreeNode> nodes = new() { t0, t1, t2, t3, t4, t5, t6 };
            nodes.Shuffle(random);
            Assert.IsNotNull(nodes);
        }

        [TestMethod]
        public void TestOtherListTypes()
        {
            Random random = new(0);

            int[] array = new int[5] { 4, 9, 5, 3, 1 };
            array.Shuffle(random);
            Assert.IsNotNull(array);

            Collection<int> collection = new() { 4, 9, 5, 3, 1 };
            collection.Shuffle(random);
            Assert.IsNotNull(collection);
        }
    }
}
