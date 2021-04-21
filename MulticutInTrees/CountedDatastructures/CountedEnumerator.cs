// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Custom <see cref="IEnumerator{T}"/> that counts the number of operations it performed.
    /// </summary>
    /// <typeparam name="T">The type of elements this <see cref="IEnumerator{T}"/> enumerates.</typeparam>
    public class CountedEnumerator<T> : IEnumerator<T> where T : notnull
    {
        /// <summary>
        /// The <see cref="Counter"/> that counts the number of operations this <see cref="CountedEnumerator{T}"/> used.
        /// </summary>
#pragma warning disable IDE0052 // Remove unread private members. The counter is just a reference, so it is actually read.
        private Counter OperationsCounter { get; set; }
#pragma warning restore IDE0052 // Remove unread private members

        /// <summary>
        /// The internal <see cref="IEnumerator{T}"/> this <see cref="CountedEnumerator{T}"/> uses.
        /// </summary>
        private IEnumerator<T> Enumerator { get; }

        /// <inheritdoc/>
        public T Current
        {
            get
            {
                OperationsCounter++;
                return Enumerator.Current;
            }
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        object IEnumerator.Current
        {
            get
            {
                OperationsCounter++;
                return Enumerator.Current;
            }
        }

        /// <summary>
        /// Constructor for a <see cref="CountedEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">The <see cref="IEnumerator{T}"/> this <see cref="CountedEnumerator{T}"/> should use internally.</param>
        /// <param name="counter">The <see cref="Counter"/> this <see cref="CountedEnumerator{T}"/> should use.</param>
        public CountedEnumerator(IEnumerator<T> enumerator, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(enumerator, nameof(enumerator), "Trying to create an instance of a CountedEnumerator, but the IEnumerator is should use is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to create an instance of a CountedEnumerator, but the counter is should use is null!");
#endif
            Enumerator = enumerator;
            OperationsCounter = counter;
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            return Enumerator.MoveNext();
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Enumerator.Reset();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Enumerator.Dispose();
        }
    }
}
