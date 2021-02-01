// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Static class containing utility functions used throughout the entire program.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Method that checks whether <paramref name="parameter"/> is <see langword="null"/>. Throws an <see cref="ArgumentNullException"/> if it is.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="parameter"/>.</typeparam>
        /// <param name="parameter">The <typeparamref name="T"/> for which we want to check if it is <see langword="null"/>.</param>
        /// <param name="paramName">The name of <paramref name="parameter"/> in the method this is called from.</param>
        /// <param name="message">Optional. The custom message that should be given to the <see cref="ArgumentNullException"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is <see langword="null"/>.</exception>
        internal static void NullCheck<T>(T parameter, string paramName, string message = null)
        {
            if (paramName is null)
            {
                throw new ArgumentNullException("paramName", $"Trying to perform a null check on the parameters of a method, but the name of the parameter given to the null check method is null!");
            }

            if (parameter is null)
            {
                if (message is null)
                {
                    throw new ArgumentNullException(paramName);
                }

                throw new ArgumentNullException(paramName, message);
            }
        }

        /// <summary>
        /// Orders a tuple of <typeparamref name="N"/>s representing an edge in such a way that the <typeparamref name="N"/> with the smallest <see cref="INode{N}.ID"/> is the first element of the resulting tuple.
        /// </summary>
        /// <typeparam name="N">The type of nodes the edge is between. Implementation of <see cref="INode{N}"/>.</typeparam>
        /// <param name="edge">The tuple of <typeparamref name="N"/>s representing the edge we want to order.</param>
        /// <returns>A tuple with the same <typeparamref name="N"/>s as <paramref name="edge"/>, such that the <typeparamref name="N"/> with the smallest <see cref="INode{N}.ID"/> is the first element in the result.</returns>
        /// <exception cref="ArgumentNullException">Thrown whein either endpoint of <paramref name="edge"/> is <see langword="null"/>.</exception>
        public static (N, N) OrderEdgeSmallToLarge<N>((N, N) edge) where N : INode<N>
        {
            NullCheck(edge.Item1, nameof(edge.Item1), $"Trying to order an edge from smallest to largest, but the first endpoint of the edge is null!");
            NullCheck(edge.Item2, nameof(edge.Item2), $"Trying to order an edge from smallest to largest, but the second endpoint of the edge is null!");

            if (edge.Item2.ID < edge.Item1.ID)
            {
                return (edge.Item2, edge.Item1);
            }
            return edge;
        }

        /// <summary>
        /// Checks whether this <see cref="IEnumerable{T}"/> is a subset of another <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="IEnumerable{T}"/>s.</typeparam>
        /// <param name="subset">The current <see cref="IEnumerable{T}"/>, the potential subset.</param>
        /// <param name="largerSet">The other <see cref="IEnumerable{T}"/>, the potential superset.</param>
        /// <returns><see langword="true"/> if <paramref name="subset"/> is a subset of <paramref name="largerSet"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="subset"/> or <paramref name="largerSet"/> is <see langword="null"/>.</exception>
        public static bool IsSubsetOf<T>(this IEnumerable<T> subset, IEnumerable<T> largerSet)
        {
            NullCheck(subset, nameof(subset), $"Trying to see if an IEnumerable is a subset of another IEnumerable, but the first IEnumerable is null!");
            NullCheck(largerSet, nameof(largerSet), $"Trying to see if an IEnumerable is a subset of another IEnumerable, but the second IEnumerable is null!");

            if (subset.Count() > largerSet.Count())
            {
                return false;
            }

            return subset.All(n => largerSet.Contains(n));
        }

        /// <summary>
        /// Creates a <see cref="string"/> from an <see cref="IEnumerable{T}"/> like Python does.
        /// <br/>
        /// Looks like: "IEnumerable with {n} elements: [elem1, elem2, ..., elemn]" where n is a number, and elemi are elements of <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="list"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> we want to print.</param>
        /// <returns>A Python-like <see cref="string"/> representation of <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is <see langword="null"/>.</exception>
        public static string Print<T>(this IEnumerable<T> list)
        {
            NullCheck(list, nameof(list), $"Trying to print an IEnumerable, but the IEnumerable is null!");

            StringBuilder sb = new StringBuilder();
            sb.Append($"IEnumerable with {list.Count()} elements: [");
            foreach (T elem in list)
            {
                sb.Append($"{elem}, ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
