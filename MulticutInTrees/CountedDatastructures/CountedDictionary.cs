// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Implementation of an <see cref="Dictionary{TKey, TValue}"/> that counts the number of operations that has been performed on it.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in this <see cref="CountedDictionary{TKey, TValue}"/>.</typeparam>
    /// <typeparam name="TValue">The type of the values in this <see cref="CountedDictionary{TKey, TValue}"/>.</typeparam>
    public class CountedDictionary<TKey, TValue> where TKey : notnull
    {
        /// <summary>
        /// The internal <see cref="Dictionary{TKey, TValue}"/> that contains the collection of elements in this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        private Dictionary<TKey, TValue> Dictionary { get; }

        /// <summary>
        /// Gets or sets the <typeparamref name="TValue"/> associated with <paramref name="key"/> in this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the <typeparamref name="TValue"/> we want to get or set.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns>The <typeparamref name="TValue"/> associated with <paramref name="key"/>. If this value does not exist in the set operation, a new element with <paramref name="key"/> is added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public TValue this[TKey key, Counter counter]
        {
            get
            {
#if !EXPERIMENT
                Utils.NullCheck(counter, nameof(counter), "Trying to get the value corresponding to a key from a CountedDictionary, but the counter is null!");
#endif
                counter++;
                return Dictionary[key];
            }
            set
            {
#if !EXPERIMENT
                Utils.NullCheck(counter, nameof(counter), "Trying to set the value corresponding to a key in a CountedDictionary, but the counter is null!");
#endif
                counter++;
                Dictionary[key] = value;
            }
        }

        /// <summary>
        /// Constructor for a <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        public CountedDictionary()
        {
            Dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Constructor for a <see cref="CountedDictionary{TKey, TValue}"/> that contains the same values as <paramref name="dictionary"/>, and which values will be updated along with those in <paramref name="dictionary"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="CountedDictionary{TKey, TValue}"/> with the elements this <see cref="CountedDictionary{TKey, TValue}"/> should also contain.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dictionary"/> or <paramref name="dictionary"/> is <see langword="null"/>.</exception>
        public CountedDictionary(Dictionary<TKey, TValue> dictionary, Counter counter) : this()
        {
#if !EXPERIMENT
            Utils.NullCheck(dictionary, nameof(dictionary), "Trying to create an instance of a CountedDictionary with elements, but the Dictionary with elements is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to create an instance of a CountedDictionary with elements, but the counter is null!");
#endif
            foreach (KeyValuePair<TKey, TValue> kvPair in dictionary)
            {
                Add(kvPair.Key, kvPair.Value, counter);
            }
        }

        /// <summary>
        /// Gets all the keys in this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns>An <see cref="CountedEnumerable{T}"/> with all the keys in this <see cref="CountedDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<TKey> GetKeys(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the keys in a CountedDictionary, but the counter is null!");
#endif
            counter++;
            return new CountedEnumerable<TKey>(Dictionary.Keys, counter);
        }

        /// <summary>
        /// Gets all the values in this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns>An <see cref="CountedEnumerable{T}"/> with all the values in this <see cref="CountedDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<TValue> GetValues(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the values in a CountedDictionary, but the counter is null!");
#endif
            counter++;
            return new CountedEnumerable<TValue>(Dictionary.Values, counter);
        }

        /// <summary>
        /// Returns the number of elements in this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of elements in this <see cref="CountedDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int Count(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the number of elements in a CountedDictionary, but the counter is null!");
#endif
            counter++;
            return Dictionary.Count;
        }

        /// <summary>
        /// Add an element to this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the element to be added.</param>
        /// <param name="value">The <typeparamref name="TValue"/> of the element to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void Add(TKey key, TValue value, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to add an element to a CountedDictionary, but the counter is null!");
#endif
            counter++;
            Dictionary.Add(key, value);
        }

        /// <summary>
        /// Checks whether this <see cref="CountedDictionary{TKey, TValue}"/> contains a key <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> for which we want to know if it is present in this <see cref="CountedDictionary{TKey, TValue}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns><see langword="true"/> if this <see cref="CountedCollection{T}"/> contains an element with <paramref name="key"/> as key, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool ContainsKey(TKey key, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to see if a CountedDictionary contains a certain key, but the counter is null!");
#endif
            counter++;
            return Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Removes an item with <paramref name="key"/> as key from this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the value to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns><see langword="true"/> if the element with <paramref name="key"/> was removed successfully, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool Remove(TKey key, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to remove an element from a CountedDictionary, but the counter is null!");
#endif
            counter++;
            return Dictionary.Remove(key);
        }

        /// <summary>
        /// Try to get the value of a given <typeparamref name="TKey"/> from this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the value asked.</param>
        /// <param name="value">The <typeparamref name="TValue"/> corresponding to <paramref name="key"/>, if it exists.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns><see langword="true"/> if the element with <paramref name="key"/> is present, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the value of a given key from a CountedDictionary, but the counter is null!");
#endif
            counter++;
            return Dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes all elements from this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void Clear(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to clear a CountedDictionary, but the counter is null!");
#endif
            counter += Dictionary.Count;
            Dictionary.Clear();
        }

        /// <summary>
        /// Returns the <see cref="CountedEnumerable{T}"/> that can be used to enumerate over this <see cref="CountedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> that can be used to enumerate over this <see cref="CountedDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<KeyValuePair<TKey, TValue>> GetCountedEnumerable(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the enumerator of a CountedDictionary, but the counter is null!");
#endif
            return new CountedEnumerable<KeyValuePair<TKey, TValue>>(Dictionary, counter);
        }
    }
}
