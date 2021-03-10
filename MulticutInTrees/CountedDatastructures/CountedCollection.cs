// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.Exceptions;

namespace MulticutInTrees.CountedDatastructures
{
    /// <summary>
    /// Custom data structure that uses a <see cref="LinkedList{T}"/> and a <see cref="Dictionary{TKey, TValue}"/> for fast operations, and counts the number of operations it performs.
    /// </summary>
    /// <typeparam name="T">The type of elements in this <see cref="CountedCollection{T}"/>.</typeparam>
    public class CountedCollection<T> where T : notnull
    {
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
            Dictionary = new Dictionary<T, LinkedListNode<T>>();
            LinkedList = new LinkedList<T>();
        }

        /// <summary>
        /// Constructor for a <see cref="CountedCollection{T}"/> with all the elements in <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable"/> with elements that should be added to this <see cref="CountedCollection{T}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used to count the operations needed to add the elements of <paramref name="collection"/> to this new <see cref="CountedCollection{T}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is <see langword="null"/>.</exception>
        public CountedCollection(IEnumerable<T> collection, Counter counter) : this()
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(collection, nameof(collection), "Trying to create a new CountedCollection containing elements of an IEnumerable, but the IEnumerable is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to create a CountedCollection containing elements of an IEnumerable, but the Counter is null!");
#endif
            foreach (T item in collection)
            {
                Add(item, counter);
            }
        }

        /// <summary>
        /// Returns the number of elements in this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <returns>An <see cref="int"/> that represents the number of elements in this <see cref="CountedCollection{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int Count(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the number of elements in a CountedCollection, but the Counter is null!");
#endif
            counter++;
            return LinkedList.Count;
        }

        /// <summary>
        /// Counts the number of elements in this <see cref="CountedCollection{T}"/> that return <see langword="true"/> on <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The <see cref="Func{T, TResult}"/> that functions as the counting condition.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <returns>An <see cref="int"/> that represents the number of elements in this <see cref="CountedCollection{T}"/> that fit <paramref name="predicate"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int Count(Func<T, bool> predicate, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(predicate, nameof(predicate), "Trying to get the number of elements that fit a function in a CountedCollection, but the function is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the number of elements that fit a function in a CountedCollection, but the Counter is null!");
#endif
            int res = LinkedList.Count(predicate);
            counter += res;
            return res;
        }

        /// <summary>
        /// Returns the first element in this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <returns>The <typeparamref name="T"/> that is the first element in this <see cref="CountedCollection{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public T First(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the first element of a CountedCollection, but the Counter is null!");
#endif
            counter++;
            return LinkedList.First.Value;
        }

        /// <summary>
        /// Returns the first element in this <see cref="CountedCollection{T}"/> that fits <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The condition that determines which element we return.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <returns>The <typeparamref name="T"/> that is the first element in this <see cref="CountedCollection{T}"/> that passes <paramref name="predicate"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public T First(Func<T, bool> predicate, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(predicate, nameof(predicate), "Trying to get the first element of a CountedCollection that fits a condition, but the condition-function is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the first element of a CountedCollection that fits a condition, but the Counter is null!");
#endif
            return LinkedList.First(elem =>
            {
                counter++;
                return predicate(elem);
            });
        }

        /// <summary>
        /// Returns the last element in this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <returns>The <typeparamref name="T"/> that is the last element in this <see cref="CountedCollection{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public T Last(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the last element of a CountedCollection, but the Counter is null!");
#endif
            counter++;
            return LinkedList.Last.Value;
        }

        /// <summary>
        /// Adds an item to this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="AlreadyPresentException">Thrown when <paramref name="item"/> is already present in this <see cref="CountedCollection{T}"/>.</exception>
        public void Add(T item, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(item, nameof(item), "Trying to add an item to a CountedCollection, but the item is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to add an element to a CountedCollection, but the Counter is null!");
            if (Dictionary.ContainsKey(item))
            {
                throw new AlreadyPresentException($"Trying to add {item} to a CountedCollection, but {item} is already present in this CountedCollection!");
            }
#endif
            counter++;
            LinkedListNode<T> node = new LinkedListNode<T>(item);
            Dictionary[item] = node;
            LinkedList.AddLast(node);
        }

        /// <summary>
        /// Removes all elements in this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void Clear(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to clear a CountedCollection, but the Counter is null!");
#endif            
            counter += Dictionary.Count;
            Dictionary.Clear();
            LinkedList.Clear();
        }

        /// <summary>
        /// Checks whether this <see cref="CountedCollection{T}"/> contains <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> for which we want to know if it is part of this <see cref="CountedCollection{T}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="CountedCollection{T}"/> contains <paramref name="item"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool Contains(T item, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(item, nameof(item), "Trying to check whether a CountedCollection contains an element, but the item is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to check whether a CountedCollection contains an element, but the Counter is null!");
#endif            
            counter++;
            return Dictionary.ContainsKey(item);
        }

        /// <summary>
        /// Returns an <see cref="CountedEnumerable{T}"/> that can be used to iterate over this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> that counts how many operations this <see cref="IEnumerable{T}"/> executed.</param>
        /// <returns>An <see cref="CountedEnumerable{T}"/> that can be used to iterate over this <see cref="CountedCollection{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<T> GetCountedEnumerable(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Grabbing the custom Counter Enumerable for a CountedCollection, but the counter is null!");
#endif
            return new CountedEnumerable<T>(LinkedList, counter);
        }

        /// <summary>
        /// Removes <paramref name="item"/> from this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <returns><see langword="true"/> if we successfully removed <paramref name="item"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool Remove(T item, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(item, nameof(item), "Trying to remove an item from a CountedCollection, but the item is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to remove an element from a CountedCollection, but the Counter is null!");
#endif
            counter++;
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
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveFromStartWhile(Func<T, bool> predicate, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(predicate, nameof(predicate), "Trying to remove elements from the start of a CountedCollection, but the predicate is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to remove elements from the start of a CountedCollection, but the Counter is null!");
#endif
            while (predicate(LinkedList.First.Value))
            {
                counter++;
                Remove(LinkedList.First.Value, counter);
            }
        }

        /// <summary>
        /// Removes elements from the end of this <see cref="CountedCollection{T}"/> while <paramref name="predicate"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="predicate"><see cref="Func{T, TResult}"/> that is used to determine which elements to remove.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveFromEndWhile(Func<T, bool> predicate, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(predicate, nameof(predicate), "Trying to remove elements from the end of a CountedCollection, but the predicate is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to remove elements from end start of a CountedCollection, but the Counter is null!");
#endif
            while (predicate(LinkedList.Last.Value))
            {
                counter++;
                Remove(LinkedList.Last.Value, counter);
            }
        }

        /// <summary>
        /// Change <paramref name="oldElement"/> to <paramref name="newElement"/>.
        /// </summary>
        /// <param name="oldElement">The <typeparamref name="T"/> that will be replaced.</param>
        /// <param name="newElement">The <typeparamref name="T"/> that <paramref name="oldElement"/> should be replaced with.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="oldElement"/>, <paramref name="newElement"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="oldElement"/> is not part of this <see cref="CountedCollection{T}"/>.</exception>
        public void ChangeElement(T oldElement, T newElement, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(oldElement, nameof(oldElement), "Trying to replace an element by another in a CountedCollection, but the old element is null!");
            Utilities.Utils.NullCheck(newElement, nameof(newElement), "Trying to replace an element by another in a CountedCollection, but the new element is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to replace an element by another in a CountedCollection, but the counter is null!");
            if (!Dictionary.ContainsKey(oldElement))
            {
                throw new InvalidOperationException($"Trying to change {oldElement} to {newElement} in {this}, but {oldElement} is not part of this collection!");
            }
#endif
            counter++;
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
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="function"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void ChangeOccurrence(Func<T, T> function, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(function, nameof(function), "Trying to change all elements in a CountedCollection according to a function, but the function is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to change all elements in a CountedCollection according to a function, but the counter is null!");
#endif
            foreach (T node in LinkedList)
            {
                counter++;
                T newNode = function(node);
                ChangeElement(node, newNode, counter);
            }
        }

        /// <summary>
        /// Returns the elements before and after <paramref name="element"/> in this <see cref="CountedCollection{T}"/>.
        /// </summary>
        /// <param name="element">The element for which we want to know the elements before and after it.</param>
        /// <param name="counter">The <see cref="Counter"/> that should be used for this operation.</param>
        /// <returns>A tuple with two <typeparamref name="T"/>s: the <typeparamref name="T"/> before <paramref name="element"/> and the <typeparamref name="T"/> after <paramref name="element"/>. If either of these does not exist, <see langword="default"/> is returned in that tuple spot instead.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="element"/> is not part of this <see cref="CountedCollection{T}"/>.</exception>
        public (T, T) ElementBeforeAndAfter(T element, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(element, nameof(element), "Trying to find the element before and after an element in a CountedCollection, but the element is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to find the element before and after an element in a CountedCollection, but the counter is null!");
            if (!Dictionary.ContainsKey(element))
            {
                throw new InvalidOperationException($"Trying to find the element before and after {element} in {this}, but {element} is not part of this CountedCollection!");
            }
#endif
            counter += 2;
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
    }
}
