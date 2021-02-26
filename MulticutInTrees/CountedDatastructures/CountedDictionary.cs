// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Implementation of an <see cref="IDictionary{TKey, TValue}"/> that counts the number of operations that has been performed on it. See <seealso cref="OperationsCounter"/> for this number.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in this <see cref="CountedDictionary{TKey, TValue}"/>.</typeparam>
    /// <typeparam name="TValue">The type of the values in this <see cref="CountedDictionary{TKey, TValue}"/>.</typeparam>
    public class CountedDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
    {
        /// <summary>
        /// The internal <see cref="Dictionary{TKey, TValue}"/> that contains the collection of elements in this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        private protected Dictionary<TKey, TValue> Dictionary { get; set; }

        /// <summary>
        /// The number of operations that has been performed on this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        public Counter OperationsCounter { get; private set; }

        /// <inheritdoc/>
        public TValue this[TKey key]
        { 
            get
            {
                OperationsCounter++;
                return Dictionary[key];
            }
            set
            {
                OperationsCounter++;
                Dictionary[key] = value;
            } 
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys
        {
            get
            {
                OperationsCounter += Dictionary.Count;
                return Dictionary.Keys;
            }
        }

        /// <inheritdoc/>
        public ICollection<TValue> Values
        {
            get
            {
                OperationsCounter += Dictionary.Count;
                return Dictionary.Values;
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                OperationsCounter++;
                return Dictionary.Count;
            }
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).IsReadOnly;

        /// <summary>
        /// Constructor for a <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        public CountedDictionary()
        {
            OperationsCounter = new Counter();
            Dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Constructor for a <see cref="CountedDictionary{TKey, TValue}"/> that contains the same values as <paramref name="dictionary"/>, and which values will be updated along with those in <paramref name="dictionary"/>, but with its own <see cref="OperationsCounter"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="CountedDictionary{TKey, TValue}"/> with the elements this <see cref="CountedDictionary{TKey, TValue}"/> should also contain.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dictionary"/> is <see langword="null"/>.</exception>
        public CountedDictionary(CountedDictionary<TKey, TValue> dictionary) : this()
        {
            Utils.NullCheck(dictionary, nameof(dictionary), $"Trying to create an instance of a CountedDictionary with a reference to another CountedDictionary, but the reference is null!");

            Dictionary = dictionary.Dictionary;
        }
        
        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            OperationsCounter++;
            Dictionary.Add(key, value);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            OperationsCounter++;
            return Dictionary.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            OperationsCounter++;
            return Dictionary.Remove(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            OperationsCounter++;
            return Dictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            OperationsCounter += Dictionary.Count;
            Dictionary.Clear();
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Contains(item);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Remove(item);
        }

        // todo: bij elke reference de juiste counter meegeven
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that can be used to iterate over this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> that counts how many operations this <see cref="IEnumerable{T}"/> executed.</param>
        /// <returns>An <see cref="CountedDictionary{TKey, TValue}"/> that can be used to iterate over this <see cref="CountedCollection{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public IEnumerable<KeyValuePair<TKey, TValue>> GetCountedEnumerable(Counter counter)
        {
            Utils.NullCheck(counter, nameof(counter), "Grabbing the custom Counter Enumerable for a CountedDictionary, but the counter is null!");

            foreach (KeyValuePair<TKey, TValue> item in Dictionary)
            {
                counter++;
                yield return item;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotSupportedException();
            //return new CountedEnumerator<KeyValuePair<TKey, TValue>>(((IEnumerable<KeyValuePair<TKey, TValue>>)Dictionary).GetEnumerator(), OperationsCounter);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
            //return ((IEnumerable)Dictionary).GetEnumerator();
        }
    }
}
