// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
                throw new ArgumentNullException(nameof(paramName), "Trying to perform a null check on the parameters of a method, but the name of the parameter given to the null check method is null!");
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
#if !EXPERIMENT
            NullCheck(edge.Item1, nameof(edge.Item1), "Trying to order an edge from smallest to largest, but the first endpoint of the edge is null!");
            NullCheck(edge.Item2, nameof(edge.Item2), "Trying to order an edge from smallest to largest, but the second endpoint of the edge is null!");
#endif
            if (edge.Item2.ID < edge.Item1.ID)
            {
                return (edge.Item2, edge.Item1);
            }
            return edge;
        }

        /// <summary>
        /// Checks whether this <see cref="IList{T}"/> is a subset of another <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="IList{T}"/>s.</typeparam>
        /// <param name="subset">The current <see cref="IList{T}"/>, the potential subset.</param>
        /// <param name="largerSet">The other <see cref="IList{T}"/>, the potential superset.</param>
        /// <returns><see langword="true"/> if <paramref name="subset"/> is a subset of <paramref name="largerSet"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="subset"/> or<paramref name="largerSet"/> is <see langword="null"/>.</exception>
        public static bool IsSubsetOf<T>(this IEnumerable<T> subset, IEnumerable<T> largerSet)
        {
#if !EXPERIMENT
            NullCheck(subset, nameof(subset), "Trying to see if an IEnumerable is a subset of another IEnumerable, but the first IEnumerable is null!");
            NullCheck(largerSet, nameof(largerSet), "Trying to see if an IEnumerable is a subset of another IEnumerable, but the second IEnumerable is null!");
#endif
            if (subset.Count() > largerSet.Count())
            {
                return false;
            }

            HashSet<T> larger = new HashSet<T>(largerSet);
            return subset.All(n => larger.Contains(n));
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
#if !EXPERIMENT
            NullCheck(list, nameof(list), "Trying to print an IEnumerable, but the IEnumerable is null!");
#endif
            StringBuilder sb = new StringBuilder();
            sb.Append($"{list.GetType()} with {list.Count()} elements: [");
            foreach (T elem in list)
            {
                sb.Append($"{elem}, ");
            }
            sb.Remove(sb.Length - 2, 2);
            if (!list.Any())
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append('.');
            }
            else
            {
                sb.Append("]");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks whether a path is a simple path (i.e. does not contain cycles).
        /// </summary>
        /// <typeparam name="N">The type of nodes on the path. Implements <see cref="INode{N}"/>.</typeparam>
        /// <param name="path">An <see cref="IEnumerable{T}"/> of tuples of <typeparamref name="N"/>s that represent the edges on the path.</param>
        /// <returns><see langword="true"/> if <paramref name="path"/> is a simple path, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
        public static bool IsSimplePath<N>(IEnumerable<(N, N)> path) where N : INode<N>
        {
#if !EXPERIMENT
            NullCheck(path, nameof(path), "Trying to see whether a path is a simple path, but the path is null!");
#endif
            for (int i = 0; i < path.Count() - 1; i++)
            {
                for (int j = i + 1; j < path.Count(); j++)
                {
                    if (path.ElementAt(i).Item1.Equals(path.ElementAt(j).Item2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Transforms an <see cref="IEnumerable{T}"/> with <typeparamref name="N"/>s representing a path to a <see cref="List{T}"/> with tuples of two <typeparamref name="N"/>s representing the edges on the path.
        /// </summary>
        /// <typeparam name="N">The type of nodes on the path. Implements <see cref="INode{N}"/>.</typeparam>
        /// <param name="path">An <see cref="IEnumerable{T}"/> with <typeparamref name="N"/>s we want to tranform to a <see cref="List{T}"/> with the edges on the path.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> with tuples of <typeparamref name="N"/>s representing the edges on <paramref name="path"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
        public static List<(N, N)> NodePathToEdgePath<N>(IEnumerable<N> path) where N : INode<N>
        {
#if !EXPERIMENT
            NullCheck(path, nameof(path), "Trying to transform a node path to an edge path, but the path is null!");
#endif
            List<(N, N)> result = new List<(N, N)>();
            for (int i = 0; i < path.Count() - 1; i++)
            {
                result.Add((path.ElementAt(i), path.ElementAt(i + 1)));
            }

            return result;
        }

        /// <summary>
        /// Picks a random element from an <see cref="IEnumerable{T}"/> that fits a given condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> we want to pick a conditioned random element from.</param>
        /// <param name="condition">The <see cref="Func{T, TResult}"/> that a <typeparamref name="T"/> in <paramref name="list"/> must return <see langword="true"/> on to be considered to be picked.</param>
        /// <param name="random">The <see cref="Random"/> used to pick an arbitrary element.</param>
        /// <returns>A uniform randomly picked <typeparamref name="T"/> that returns <see langword="true"/> on <paramref name="condition"/> from <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/>, <paramref name="condition"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        public static T PickRandomWhere<T>(this IEnumerable<T> list, Func<T, bool> condition, Random random)
        {
#if !EXPERIMENT
            NullCheck(list, nameof(list), "Trying to pick a random element form an IEnumerable that fits a condition, but the IEnumerable is null!");
            NullCheck(condition, nameof(condition), "Trying to pick a random element form an IEnumerable that fits a condition, but the function with the condition is null!");
            NullCheck(random, nameof(random), "Trying to pick a random element form an IEnumerable that fits a condition, but the random number generator is null!");
#endif
            return list.Where(n => condition(n)).PickRandom(random);
        }

        /// <summary>
        /// Picks a random element from an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> we want to pick a random element from.</param>
        /// <param name="random">The <see cref="Random"/> used to pick an arbitrary element.</param>
        /// <returns>A uniform randomly picked <typeparamref name="T"/> from <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        public static T PickRandom<T>(this IEnumerable<T> list, Random random)
        {
#if !EXPERIMENT
            NullCheck(list, nameof(list), "Trying to pick a random element from an IEnumerable, but the IEnumerable is null!");
            NullCheck(random, nameof(random), "Trying to pick a random element from an IEnumerable, but the random number generator is null!");
#endif
            return list.ElementAt(random.Next(list.Count()));
        }

        /// <summary>
        /// Finds the largest <see cref="int"/> for which <paramref name="function"/> returns <see langword="true"/>.
        /// </summary>
        /// <param name="minValue">The smallest value to check.</param>
        /// <param name="maxValue">The largest value to check.</param>
        /// <param name="function"><see cref="Func{T, TResult}"/> from <see cref="int"/> to <see cref="bool"/>. Given the numbers in the range between <paramref name="minValue"/> and <paramref name="maxValue"/>, should return any number of <see langword="true"/> results, followed by any number of <see langword="false"/> results.</param>
        /// <returns>The largest <see cref="int"/> for which <paramref name="function"/> returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="function"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="minValue"/> is larger than <paramref name="maxValue"/>.</exception>
        public static int BinarySearchGetLastTrue(int minValue, int maxValue, Func<int, bool> function)
        {
#if !EXPERIMENT
            NullCheck(function, nameof(function), "Trying to binary search for the last value that results in true, but the function is null!");
            if (minValue > maxValue)
            {
                throw new ArgumentException("Trying to binary search for the last value that results in true, but the start of the range of values to check is larger than the end of the range of values to check!");
            }
#endif
            int answer = -1;
            int current = (minValue + maxValue + 1) / 2;
            while (minValue <= maxValue)
            {
                if (function(current))
                {
                    answer = current;
                    minValue = current + 1;
                }
                else
                {
                    maxValue = current - 1;
                }
                current = (minValue + maxValue + 1) / 2;
            }
            return answer;
        }

        /// <summary>
        /// Finds the smallest <see cref="int"/> for which <paramref name="function"/> returns <see langword="true"/>.
        /// </summary>
        /// <param name="minValue">The smallest value to check.</param>
        /// <param name="maxValue">The largest value to check.</param>
        /// <param name="function"><see cref="Func{T, TResult}"/> from <see cref="int"/> to <see cref="bool"/>. Given the numbers in the range between <paramref name="minValue"/> and <paramref name="maxValue"/>, should return any number of <see langword="false"/> results, followed by any number of <see langword="true"/> results.</param>
        /// <returns>The smallest <see cref="int"/> for which <paramref name="function"/> returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="function"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="minValue"/> is larger than <paramref name="maxValue"/>.</exception>
        public static int BinarySearchGetFirstTrue(int minValue, int maxValue, Func<int, bool> function)
        {
#if !EXPERIMENT
            NullCheck(function, nameof(function), "Trying to binary search for the first value that results in true, but the function is null!");
            if (minValue > maxValue)
            {
                throw new ArgumentException("Trying to binary search for the first value that results in true, but the start of the range of values to check is larger than the end of the range of values to check!");
            }
#endif
            int answer = -1;
            int current = (minValue + maxValue + 1) / 2;
            while (minValue <= maxValue)
            {
                if (!function(current))
                {
                    minValue = current + 1;
                }
                else
                {
                    answer = current; 
                    maxValue = current - 1;
                }
                current = (minValue + maxValue + 1) / 2;
            }
            return answer;
        }

        /// <summary>
        /// Find all subsets of size <paramref name="subsetSize"/> in <paramref name="list"/>.
        /// <br/>
        /// Source: <see href="https://stackoverflow.com/questions/23974569/how-to-generate-all-subsets-of-a-given-size"/>
        /// </summary>
        /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> in which to find subsets.</param>
        /// <param name="subsetSize">The required size of the subsets.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with subsets of <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is <see langword="null"/>.</exception>
        public static IEnumerable<IEnumerable<T>> AllSubsetsOfSize<T>(this IEnumerable<T> list, int subsetSize)
        {
#if !EXPERIMENT
            NullCheck(list, nameof(list), "Trying to find all subsets of a certain size in an IEnumerable, but the IEnumerable is null!");
#endif
            // Generate list of sequences containing only 1 element e.g. {1}, {2}, ...
            IEnumerable<T[]> oneElemSequences = list.Select(x => new[] { x });

            // Generate List of T sequences
            List<List<T>> result = new List<List<T>>
            {
                // Add initial empty set
                new List<T>()
            };

            // Generate powerset, but skip sequences that are too long
            foreach (T[] oneElemSequence in oneElemSequences)
            {
                int length = result.Count;

                for (int i = 0; i < length; i++)
                {
                    if (result[i].Count >= subsetSize)
                    {
                        continue;
                    }

                    result.Add(result[i].Concat(oneElemSequence).ToList());
                }
            }

            return result.Where(x => x.Count == subsetSize);
        }
    }
}
