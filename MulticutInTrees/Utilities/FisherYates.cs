// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Class that shuffles the elements of an <see cref="IList{T}"/>.
    /// </summary>
    public static class FisherYates
    {
        /// <summary>
        /// Shuffles the indices of the elements in <paramref name="list"/> using the Fisher-Yates method.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="list"/>.</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> to be shuffled. Cannot be readonly.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="list"/> is readonly.</exception>
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
#if !EXPERIMENT
            Utils.NullCheck(list, nameof(list), "Trying to shuffle an IList, but the IList is null!");
            Utils.NullCheck(random, nameof(random), "Trying to shuffle an IList, but the random is null!");
#endif
            if (list.IsReadOnly && !(list is T[]))
            {
                throw new NotSupportedException($"Cannot shuffle this {list.GetType()} because it is readonly!");
            }

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
