// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Custom data structure that uses a <see cref="LinkedList{T}"/> and a <see cref="Dictionary{TKey, TValue}"/> for fast operations, and counts the number of operations it performs.
    /// </summary>
    /// <typeparam name="T">The type of elements in this <see cref="CountedCollection{T}"/>.</typeparam>
    public class CountedCollection<T> : IEnumerable<T>, ICollection<T>
    {
        /// <summary>
        /// The <see cref="Counter"/> that counts the number of operations this <see cref="CountedCollection{T}"/> performs.
        /// </summary>
        public Counter OperationsCounter { get; private set; }

        /// <summary>
        /// The internal <see cref="Dictionary{TKey, TValue}"/> from <typeparamref name="T"/> to <see cref="LinkedListNode{T}"/>.
        /// </summary>
        private Dictionary<T, LinkedListNode<T>> Dictionary { get; }

        /// <summary>
        /// The internal <see cref="LinkedList{T}"/>.
        /// </summary>
        private LinkedList<T> LinkedList { get; }

        /// <summary>
        /// Constructor for a <see cref="CountedCollection{T}"/>.
        /// </summary>
        public CountedCollection()
        {
            OperationsCounter = new Counter();
            Dictionary = new Dictionary<T, LinkedListNode<T>>();
            LinkedList = new LinkedList<T>();
        }

        /// <summary>
        /// Constructor for a <see cref="CountedCollection{T}"/> with all the elements in <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is <see langword="null"/>.</exception>
        public CountedCollection(IEnumerable<T> collection) : this()
        {
            Utils.NullCheck(collection, nameof(collection), $"Trying to create a new CountedCollection containing elements of an IEnumerable, but the IEnumerable is null!");

            foreach (T item in collection)
            {
                Add(item);
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                OperationsCounter++;
                return LinkedList.Count;
            }
        }

        /// <inheritdoc cref="LinkedList{T}.First"/>
        public T First
        {
            get
            {
                OperationsCounter++;
                return LinkedList.First.Value;
            }
        }

        /// <inheritdoc cref="LinkedList{T}.Last"/>
        public T Last
        {
            get
            {
                OperationsCounter++;
                return LinkedList.Last.Value;
            }
        }

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Add(T item)
        {
            OperationsCounter++;
            LinkedListNode<T> node = new LinkedListNode<T>(item);
            Dictionary[item] = node;
            LinkedList.AddLast(node);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            OperationsCounter += Dictionary.Count;
            Dictionary.Clear();
            LinkedList.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            OperationsCounter++;
            return Dictionary.ContainsKey(item);
        }

        // todo: bij elke reference de juiste counter meegeven
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that can be used to iterate over this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> that counts how many operations this <see cref="IEnumerable{T}"/> executed.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that can be used to iterate over this <see cref="CountedCollection{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public IEnumerable<T> GetCountedEnumerable(Counter counter)
        {
            Utils.NullCheck(counter, nameof(counter), "Grabbing the custom Counter Enumerable for a CountedCollection, but the counter is null!");

            foreach (T item in LinkedList)
            {
                counter++;
                yield return item;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotSupportedException("Counted Collections should use GetCountedEnumerable to count the number of operations.");
            //return new CountedEnumerator<T>(LinkedList.GetEnumerator(), OperationsCounter);
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            OperationsCounter++;
            if (!Dictionary.ContainsKey(item))
            {
                return false;
            }

            LinkedList.Remove(Dictionary[item]);
            Dictionary.Remove(item);
            return true;
        }

        /// <summary>
        /// Removes elements from the start of this <see cref="CountedCollection{T}"/> while <paramref name="predicate"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="predicate"><see cref="Func{T, TResult}"/> that is used to determine which elements to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public void RemoveFromStartWhile(Func<T, bool> predicate)
        {
            Utils.NullCheck(predicate, nameof(predicate), $"Trying to remove elements from the start of a CountedCollection, but the predicate is null!");

            while (predicate(LinkedList.First.Value))
            {
                OperationsCounter++;
                Remove(LinkedList.First.Value);
            }
        }

        /// <summary>
        /// Removes elements from the end of this <see cref="CountedCollection{T}"/> while <paramref name="predicate"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="predicate"><see cref="Func{T, TResult}"/> that is used to determine which elements to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public void RemoveFromEndWhile(Func<T, bool> predicate)
        {
            Utils.NullCheck(predicate, nameof(predicate), $"Trying to remove elements from the start of a CountedCollection, but the predicate is null!");

            while (predicate(LinkedList.Last.Value))
            {
                OperationsCounter++;
                Remove(LinkedList.Last.Value);
            }
        }

        /// <summary>
        /// Change <paramref name="oldElement"/> to <paramref name="newElement"/>.
        /// </summary>
        /// <param name="oldElement">The <typeparamref name="T"/> that will be replaced.</param>
        /// <param name="newElement">The <typeparamref name="T"/> that <paramref name="oldElement"/> should be replaced with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="oldElement"/> or <paramref name="newElement"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="oldElement"/> is not part of this <see cref="CountedCollection{T}"/>.</exception>
        public void ChangeElement(T oldElement, T newElement)
        {
            Utils.NullCheck(oldElement, nameof(oldElement), "Trying to replace an element by another in a CountedCollection, but the old element is null!");
            Utils.NullCheck(newElement, nameof(newElement), "Trying to replace an element by another in a CountedCollection, but the new element is null!");
            if (!Dictionary.ContainsKey(oldElement))
            {
                throw new InvalidOperationException($"Trying to change {oldElement} to {newElement} in {this}, but {oldElement} is not part of this collection!");
            }

            OperationsCounter++;
            if (oldElement.Equals(newElement))
            {
                return;
            }
            Dictionary[newElement] = Dictionary[oldElement];
            Dictionary[newElement].Value = newElement;
            Dictionary.Remove(oldElement);
        }

        /// <summary>
        /// Change each element in this <see cref="CountedCollection{T}"/> according to <paramref name="function"/>.
        /// </summary>
        /// <param name="function">The <see cref="Func{T, TResult}"/> that determines for each element what it should be replaced with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="function"/> is <see langword="null"/>.</exception>
        public void ChangeOccurrence(Func<T, T> function)
        {
            Utils.NullCheck(function, nameof(function), "Trying to change all elements in a CountedCollection according to a function, but the function is null!");

            foreach (T node in LinkedList)
            {
                OperationsCounter++;
                T newNode = function(node);
                ChangeElement(node, newNode);
            }
        }

        /// <summary>
        /// Returns the elements before and after <paramref name="element"/> in this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="element">The element for which we want to know the elements before and after it.</param>
        /// <returns>A tuple with two <typeparamref name="T"/>s: the <typeparamref name="T"/> before <paramref name="element"/> and the <typeparamref name="T"/> after <paramref name="element"/>. If either of these does not exist, <see langword="default"/> is returned in that tuple spot instead.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="element"/> is not part of this <see cref="CountedCollection{T}"/>.</exception>
        public (T, T) ElementBeforeAndAfter(T element)
        {
            Utils.NullCheck(element, nameof(element), "Trying to find the element before and after an element in a CountedCollection, but the element is null!");
            if (!Dictionary.ContainsKey(element))
            {
                throw new InvalidOperationException($"Trying to find the element before and after {element} in {this}, but {element} is not part of this CountedCollection!");
            }

            LinkedListNode<T> node = Dictionary[element];
            T before = default;
            T after = default;
            if (!(node.Previous is null))
            {
                before = node.Previous.Value;
            }
            if (!(node.Next is null))
            {
                after = node.Next.Value;
            }

            return (before, after);
        }

        /// <summary>
        /// Returns the internal <see cref="LinkedList{T}"/> this <see cref="CountedCollection{T}"/> uses.
        /// </summary>
        /// <returns><see cref="LinkedList"/>.</returns>
        public LinkedList<T> GetLinkedList()
        {
            return LinkedList;
        }

        /// <inheritdoc/>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException("Counted Collections should use GetCountedEnumerable to count the number of operations.");
        }
    }
}
