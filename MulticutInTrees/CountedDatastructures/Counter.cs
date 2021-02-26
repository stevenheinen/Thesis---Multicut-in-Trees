// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Class for counting the number of operations. It contains a single variable that counts, but we can pass this class as reference, instead of the variable as value.
    /// </summary>
    public class Counter
    {
        /// <summary>
        /// The value of this <see cref="Counter"/>.
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Constructor for a <see cref="Counter"/>.
        /// </summary>
        public Counter()
        {
            Value = 0;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of the value of this <see cref="Counter"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of this <see cref="Counter"/>.</returns>
        public override string ToString()
        { 
            return Value.ToString();
        }

        /// <summary>
        /// Increments the value of this <see cref="Counter"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be incremented.</param>
        /// <returns>The incremented <see cref="Counter"/>.</returns>
        public static Counter operator ++(Counter counter)
        {
            counter.Value++;
            return counter;
        }

        /// <summary>
        /// Decrements the value of this <see cref="Counter"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be decremented.</param>
        /// <returns>The decremented <see cref="Counter"/>.</returns>
        public static Counter operator --(Counter counter)
        {
            counter.Value--;
            return counter;
        }

        /// <summary>
        /// Adds <paramref name="number"/> to the value of this <see cref="Counter"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be increased.</param>
        /// <param name="number">The number that should be added to <paramref name="counter"/>.</param>
        /// <returns>The <see cref="Counter"/> with the increased value.</returns>
        public static Counter operator +(Counter counter, long number)
        {
            counter.Value += number;
            return counter;
        }

        /// <summary>
        /// Subtracts <paramref name="number"/> from the value of this <see cref="Counter"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be decreased.</param>
        /// <param name="number">The number that should be subtracted from <paramref name="counter"/>.</param>
        /// <returns>The <see cref="Counter"/> with the decreased value.</returns>
        public static Counter operator -(Counter counter, long number)
        {
            counter.Value -= number;
            return counter;
        }
    }
}
