// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Implementation of an <see cref="IList{T}"/> that counts the number of operations that has been performed on it. See <seealso cref="OperationsCounter"/> for this number.
    /// </summary>
    /// <typeparam name="T">The type of elements in this <see cref="CountedList{T}"/>.</typeparam>
    public class CountedList<T> : IList<T> where T : notnull
    {
        /// <summary>
        /// The internal <see cref="List{T}"/> that contains the collection of elements in this <see cref="CountedList{T}"/>.
        /// </summary>
        private protected List<T> List { get; set; }

        /// <summary>
        /// The number of operations that has been performed on this <see cref="CountedList{T}"/>.
        /// </summary>
        public Counter OperationsCounter { get; private set; }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                OperationsCounter++;
                return List.Count;
            }
        }

        /// <inheritdoc/>
        public bool IsReadOnly => ((IList<T>)List).IsReadOnly;

        /// <inheritdoc/>
        public T this[int index] 
        {
            get
            {
                OperationsCounter++;
                return List[index];
            }
            set
            {
                OperationsCounter++;
                List[index] = value;
            }
        }

        /// <summary>
        /// Constructor for a <see cref="CountedList{T}"/>.
        /// </summary>
        public CountedList()
        {
            OperationsCounter = new Counter();
            List = new List<T>();
        }

        /// <summary>
        /// Constructor for a <see cref="CountedList{T}"/> with a shared <see cref="List{T}"/> but its own <see cref="Counter"/>.
        /// </summary>
        /// <param name="list">The shared <see cref="List{T}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is <see langword="null"/>.</exception>
        public CountedList(CountedList<T> list)
        {
            Utils.NullCheck(list, nameof(list), $"Trying to create a CountedList with a shared List, but the shared list is null!");

            OperationsCounter = new Counter();
            List = list.List;
        }

        /// <summary>
        /// Constructor for a <see cref="CountedList{T}"/> with a shared <see cref="List{T}"/> but its own <see cref="Counter"/>.
        /// </summary>
        /// <param name="list">The shared <see cref="List{T}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is <see langword="null"/>.</exception>
        public CountedList(List<T> list)
        {
            Utils.NullCheck(list, nameof(list), $"Trying to create a CountedList with a shared List, but the shared list is null!");

            OperationsCounter = new Counter();
            List = list;
        }

        /// <summary>
        /// Constructor for a <see cref="CountedList{T}"/> with a specific <see cref="Counter"/>.
        /// </summary>
        /// <param name="counter">The specific <see cref="Counter"/> this <see cref="CountedList{T}"/> should use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedList(Counter counter)
        {
            Utils.NullCheck(counter, nameof(counter), $"Trying to create a CountedList with a shared counter, but the shared counter is null!");
            
            OperationsCounter = counter;
            List = new List<T>();
        }

        /// <summary>
        /// Constructor for a <see cref="CountedList{T}"/> with a shared <see cref="List{T}"/> and a specific <see cref="Counter"/>.
        /// </summary>
        /// <param name="list">The shared <see cref="List{T}"/>.</param>
        /// <param name="counter">The specific <see cref="Counter"/> this <see cref="CountedList{T}"/> should use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedList(CountedList<T> list, Counter counter)
        {
            Utils.NullCheck(list, nameof(list), $"Trying to create a CountedList with a shared list and a shared counter, but the shared list is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to create a CountedList with a shared list and a shared counter, but the shared counter is null!");
            
            OperationsCounter = counter;
            List = list.List;
        }

        /// <summary>
        /// Constructor for a <see cref="CountedList{T}"/> with a shared <see cref="List{T}"/> and a specific <see cref="Counter"/>.
        /// </summary>
        /// <param name="list">The shared <see cref="List{T}"/>.</param>
        /// <param name="counter">The specific <see cref="Counter"/> this <see cref="CountedList{T}"/> should use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedList(List<T> list, Counter counter)
        {
            Utils.NullCheck(list, nameof(list), $"Trying to create a CountedList with a shared list and a shared counter, but the shared list is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to create a CountedList with a shared list and a shared counter, but the shared counter is null!");
            
            OperationsCounter = counter;
            List = list;
        }

        /// <summary>
        /// Returns the internal <see cref="List{T}"/> of this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <returns><see cref="List"/>.</returns>
        public List<T> GetInternalList()
        {
            return List;
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            int index = List.IndexOf(item);
            OperationsCounter += index;
            if (index == -1)
            {
                OperationsCounter += List.Count + 1;
            }
            return index;
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            OperationsCounter += 1 + List.Count - index;
            List.Insert(index, item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            OperationsCounter += List.Count - index;
            List.RemoveAt(index);
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            OperationsCounter++;
            List.Add(item);
        }

        /// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
        public void AddRange(IEnumerable<T> collection)
        {
            OperationsCounter += collection.Count();
            List.AddRange(collection);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            OperationsCounter += List.Count;
            List.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            if (List.Contains(item))
            {
                OperationsCounter += List.IndexOf(item);
                return true;
            }

            OperationsCounter += List.Count;
            return false;
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            OperationsCounter += List.Count;
            return List.Remove(item);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)List).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotSupportedException();
            //return new CountedEnumerator<T>(((IEnumerable<T>)List).GetEnumerator(), OperationsCounter);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
            //return ((IEnumerable)List).GetEnumerator();
        }

        // todo: bij elke reference de juiste counter meegeven
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that can be used to iterate over this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> that counts how many operations this <see cref="IEnumerable{T}"/> executed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that can be used to iterate over this <see cref="CountedList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public IEnumerable<T> GetCountedEnumerable(Counter counter)
        {
            Utils.NullCheck(counter, nameof(counter), "Grabbing the custom Counter Enumerable for a CountedList, but the counter is null!");

            foreach (T item in List)
            {
                counter++;
                yield return item;
            }
        }
    }
}
