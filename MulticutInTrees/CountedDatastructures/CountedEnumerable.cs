// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections;
using System.Collections.Generic;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Class for an <see cref="IEnumerable{T}"/> that counts the number of operations performed on it.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <see cref="CountedEnumerable{T}"/>.</typeparam>
    public class CountedEnumerable<T> : IEnumerable<T> where T : notnull
    {
        /// <summary>
        /// The <see cref="Counter"/> used for performance measurement.
        /// </summary>
        private Counter Counter { get; }

        /// <summary>
        /// The internal <see cref="IEnumerable{T}"/> we enumerate over.
        /// </summary>
        private IEnumerable<T> Enumerable { get; }

        /// <summary>
        /// Constructor for a <see cref="CountedEnumerable{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> we want to enumerate over.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable(IEnumerable<T> enumerable, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(enumerable, nameof(enumerable), "Trying to create a CountedEnumerable, but the enumerable it should count is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to create a CountedEnumerable, but the counter is null!");
#endif
            Counter = counter;
            Enumerable = enumerable;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return new CountedEnumerator<T>(Enumerable.GetEnumerator(), Counter);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CountedEnumerator<T>(Enumerable.GetEnumerator(), Counter);
        }
    }
}
