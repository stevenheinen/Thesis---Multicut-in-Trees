// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Implementation of an <see cref="IList{T}"/> that counts the number of operations that has been performed on it.
    /// </summary>
    /// <typeparam name="T">The type of elements in this <see cref="CountedList{T}"/>.</typeparam>
    public class CountedList<T> where T : notnull
    {
        /// <summary>
        /// The internal <see cref="List{T}"/> that contains the collection of elements in this <see cref="CountedList{T}"/>.
        /// </summary>
        private List<T> List { get; }

        /// <summary>
        /// Gets or sets the element at the specified index in this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public T this[int index, Counter counter]
        {
            get
            {
#if !EXPERIMENT
                Utils.NullCheck(counter, nameof(counter), "Trying to get an element at an index in a CountedList, but the counter is null!");
#endif
                counter++;
                return List[index];
            }
            set
            {
#if !EXPERIMENT
                Utils.NullCheck(counter, nameof(counter), "Trying to set an element at an index in a CountedList, but the counter is null!");
#endif
                counter++;
                List[index] = value;
            }
        }

        /// <summary>
        /// Constructor for a <see cref="CountedList{T}"/>.
        /// </summary>
        public CountedList()
        {
            List = new List<T>();
        }

        /// <summary>
        /// Constructor for a <see cref="CountedList{T}"/> with initial elements.
        /// </summary>
        /// <param name="list">The <see cref="IEnumerable{T}"/> with initial elements.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedList(IEnumerable<T> list, Counter counter) : this()
        {
#if !EXPERIMENT
            Utils.NullCheck(list, nameof(list), "Trying to create a CountedList with elements, but the IEnumerable of elements is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to create a CountedList with elements, but the counter is null!");
#endif
            AddRange(list, counter);
        }

        /// <summary>
        /// Returns the internal <see cref="List{T}"/> of this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <returns><see cref="List"/>.</returns>
        public List<T> GetInternalList()
        {
            return List;
        }

        /// <summary>
        /// Returns the number of elements in this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of elements in this <see cref="CountedList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int Count(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the number of elements in a CountedList, but the counter is null!");
#endif
            counter++;
            return List.Count;
        }


        /// <summary>
        /// Removes the element at the given index from this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="index">The index of the element to be removed.</param>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveAt(int index, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to remove an element at a given index from a CountedList, but the counter is null!");
#endif
            counter += List.Count - index;
            List.RemoveAt(index);
        }

        /// <summary>
        /// Adds an element to this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> to be added.</param>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void Add(T item, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to add an element to a CountedList, but the counter is null!");
#endif
            counter++;
            List.Add(item);
        }

        /// <summary>
        /// Insert an item at the specified index into this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="index">The index where the item should be inserted.</param>
        /// <param name="item">The item that should be inserted.</param>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void Insert(int index, T item, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to insert an element in a CountedList, but the counter is null!");
#endif
            counter++;
            List.Insert(index, item);
        }

        /// <summary>
        /// Adds a range of elements to this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> of elements to be added.</param>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void AddRange(IEnumerable<T> collection, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to add a range of elements to a CountedList, but the counter is null!");
#endif
            counter += collection.Count();
            List.AddRange(collection);
        }

        /// <summary>
        /// Deletes all elements from this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void Clear(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to clear a CountedList, but the counter is null!");
#endif
            counter += List.Count;
            List.Clear();
        }

        /// <summary>
        /// Checks whether this <see cref="CountedList{T}"/> contains a given element.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> for which we want to know if it is present in this <see cref="CountedList{T}"/>.</param>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is present in this <see cref="CountedList{T}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool Contains(T item, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to check whether an element is present in a CountedList, but the counter is null!");
#endif
            if (List.Contains(item))
            {
                counter += List.IndexOf(item);
                return true;
            }

            counter += List.Count;
            return false;
        }

        /// <summary>
        /// Removes a given element from this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> to be removed.</param>
        /// <param name="counter">The counter used for performance measurement.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> could be removed from this <see cref="CountedList{T}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool Remove(T item, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to remove an element from a CountedList, but the counter is null!");
#endif
            counter += List.Count;
            return List.Remove(item);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that can be used to iterate over this <see cref="CountedList{T}"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> that counts how many operations this <see cref="IEnumerable{T}"/> executed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that can be used to iterate over this <see cref="CountedList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public IEnumerable<T> GetCountedEnumerable(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Grabbing the custom Counter Enumerable for a CountedList, but the counter is null!");
#endif
            return new CountedEnumerable<T>(List, counter);
        }
    }
}
