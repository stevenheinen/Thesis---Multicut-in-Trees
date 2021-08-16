// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that generates <see cref="DemandPair"/>s according to a length distribution.
    /// </summary>
    public static class LengthDistributionDemandPairs
    {
        /// <summary>
        /// Creates <see cref="DemandPair"/>s in <paramref name="tree"/> using the length distribution given in <paramref name="lengthDistribution"/>.
        /// </summary>
        /// <param name="numberOfDemandPairs">The number of <see cref="DemandPair"/>s to generate.</param>
        /// <param name="tree">The <see cref="Graph"/> in which the demand pairs exist.</param>
        /// <param name="random">The <see cref="Random"/> to be used for random number generation.</param>
        /// <param name="lengthDistribution">The <see cref="Dictionary{TKey, TValue}"/> with the desired length distribution of the <see cref="DemandPair"/>s.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="DemandPair"/>s generated using a length distribution.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="random"/> or <paramref name="lengthDistribution"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfDemandPairs"/> is negative.</exception>
        public static List<DemandPair> CreateDemandPairs(int numberOfDemandPairs, Graph tree, Random random, Dictionary<(int, int), double> lengthDistribution)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to generate demand pairs according to a length distribution in a tree, but the tree is null!");
            Utils.NullCheck(random, nameof(random), "Trying to generate demand pairs according to a length distribution in a tree, but the random is null!");
            Utils.NullCheck(lengthDistribution, nameof(lengthDistribution), "Trying to generate demand pairs according to a length distribution in a tree, but the dictionary containing the length distribution is null!");
            if (numberOfDemandPairs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfDemandPairs), "Trying to generate demand pairs according to a length distribution in a tree, but the required number of demand pairs is negative!");
            }
#endif
            Counter counter = new();
            List<DemandPair> demandPairs = new();

            /*
            // TODO: THIS IS VERY TEMPORARY
            Node n1 = tree.Nodes(counter).ElementAt(0);
            Node n2 = n1.Neighbours(counter).First();
            for (uint i = 0; i < numberOfDemandPairs; i++)
            {
                DemandPair dp = new(i, n1, n2, tree);
                demandPairs.Add(dp);
            }
            return demandPairs;
            // END OF VERY TEMPORARY IMPLEMENTATION
            */

            Dictionary<int, List<(Node, Node)>> lengthToNodePairs = ComputeLengthToNodePairs(tree, counter, out int maximumLength);
            Dictionary<double, (int, int)> cumulativeLengthDistribution = ComputeCumulativeLengthDistribution(lengthDistribution);
            for (uint i = 0; i < numberOfDemandPairs; i++)
            {
                double randomValue = random.NextDouble();
                (int min, int max) = GetValueFromCumulativeChanceDistributionDictionary(randomValue, cumulativeLengthDistribution);
                List<(Node, Node)> possibleDemandPairEndpoints = FindPossibleDemandPairEndpoints(min, max, lengthToNodePairs, maximumLength);
                (Node endpoint1, Node endpoint2) = possibleDemandPairEndpoints.PickRandom(random);
                DemandPair dp = new(i, endpoint1, endpoint2, tree);
                demandPairs.Add(dp);
            }

            return demandPairs;
        }

        /// <summary>
        /// Finds the pairs of possible <see cref="DemandPair"/> endpoints within a given <paramref name="min"/>/<paramref name="max"/> length range. If not enough <see cref="DemandPair"/>s are possible within the given range, expand the range by 1 in every direction until enough <see cref="DemandPair"/>s exist in the range.
        /// </summary>
        /// <param name="min">The minimum length the <see cref="DemandPair"/> should have.</param>
        /// <param name="max">The maximum length the <see cref="DemandPair"/> should have.</param>
        /// <param name="lengthToNodePairs"><see cref="Dictionary{TKey, TValue}"/> from length to all <see cref="Node"/> pairs with that length between them.</param>
        /// <param name="maximumLength">The diameter of the graph.</param>
        /// <param name="minimumPossibleEndpoints">The minimum number of possible endpoints that should be gathered. If there are not enough possibilities within <paramref name="min"/> and <paramref name="max"/>, the search range gets expanded by 1 in both directions.</param>
        /// <returns>A <see cref="List{T}"/> of all <see cref="Node"/> pairs that can serve as <see cref="DemandPair"/>, such that the <see cref="DemandPair"/> has a length between <paramref name="min"/> and <paramref name="max"/>.</returns>
        private static List<(Node, Node)> FindPossibleDemandPairEndpoints(int min, int max, Dictionary<int, List<(Node, Node)>> lengthToNodePairs, int maximumLength, int minimumPossibleEndpoints = 5)
        {
            List<(Node, Node)> possibleDemandPairEndpoints = new();
            for (int length = min; length <= max; length++)
            {
                if (!lengthToNodePairs.ContainsKey(length))
                {
                    continue;
                }
                possibleDemandPairEndpoints.AddRange(lengthToNodePairs[length]);
            }

            while (possibleDemandPairEndpoints.Count < minimumPossibleEndpoints)
            {
                min--;
                max++;
                if (min > 0 && lengthToNodePairs.ContainsKey(min))
                {
                    possibleDemandPairEndpoints.AddRange(lengthToNodePairs[min]);
                }
                if (max <= maximumLength && lengthToNodePairs.ContainsKey(max))
                {
                    possibleDemandPairEndpoints.AddRange(lengthToNodePairs[max]);
                }

                if (min < 1 && max > maximumLength)
                {
                    throw new Exception("Trying to create demand pairs using a length distribution, but there are no demand pairs possible with any length!");
                }
            }

            return possibleDemandPairEndpoints;
        }

        /// <summary>
        /// Grab the correct value from a <see cref="Dictionary{TKey, TValue}"/> with cumulative chance distribution.
        /// </summary>
        /// <typeparam name="T">The type of values in the <see cref="Dictionary{TKey, TValue}"/>.</typeparam>
        /// <param name="randomValue">The value produced by the <see cref="Random"/> to be used to determine the <typeparamref name="T"/> to be chosen.</param>
        /// <param name="cumulativeChanceDistribution"><see cref="Dictionary{TKey, TValue}"/> with cumulative chance distribution in its keys. The key for each value is the first number LARGER than the chance to choose its corresponding value.</param>
        /// <returns>The <typeparamref name="T"/> chosen according to <paramref name="randomValue"/>.</returns>
        private static T GetValueFromCumulativeChanceDistributionDictionary<T>(double randomValue, Dictionary<double, T> cumulativeChanceDistribution)
        {
            return cumulativeChanceDistribution[cumulativeChanceDistribution.Keys.First(v => v > randomValue)];
        }

        /// <summary>
        /// Computes a cumulative chance <see cref="Dictionary{TKey, TValue}"/> for the length values provided in <paramref name="lengthDistribution"/>. The key of the <see cref="Dictionary{TKey, TValue}"/> is the first value LARGER than the chance for its element.
        /// </summary>
        /// <param name="lengthDistribution"><see cref="Dictionary{TKey, TValue}"/> with min/max pairs for the <see cref="DemandPair"/> distance, and the chance for this pair to happen.</param>
        /// <returns>A cumulative chance <see cref="Dictionary{TKey, TValue}"/> for the length values provided in <paramref name="lengthDistribution"/>.</returns>
        private static Dictionary<double, (int, int)> ComputeCumulativeLengthDistribution(Dictionary<(int, int), double> lengthDistribution)
        {
            double valuesSum = lengthDistribution.Values.Sum();
            foreach (KeyValuePair<(int, int), double> pair in lengthDistribution)
            {
                lengthDistribution[pair.Key] /= valuesSum;
            }

            Dictionary<double, (int, int)> cumulativeLengthDistribution = new();
            double cumulativeValue = 0.0;
            foreach (KeyValuePair<(int, int), double> pair in lengthDistribution)
            {
                cumulativeValue += pair.Value;
                cumulativeLengthDistribution[cumulativeValue] = pair.Key;
            }

            return cumulativeLengthDistribution;
        }

        /// <summary>
        /// Computes a <see cref="Dictionary{TKey, TValue}"/> with per length value a <see cref="List{T}"/> of tuples of <see cref="Node"/>s with the key as distance between them.
        /// </summary>
        /// <param name="tree">The <see cref="Graph"/> in which to compute this <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <param name="maximumLength">The maximum length between any two <see cref="Node"/>s in <paramref name="tree"/>. In other words, the diameter of <paramref name="tree"/>.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with per length value a <see cref="List{T}"/> of tuples of <see cref="Node"/>s with the key as distance between them.</returns>
        private static Dictionary<int, List<(Node, Node)>> ComputeLengthToNodePairs(Graph tree, Counter counter, out int maximumLength)
        {
            ConcurrentDictionary<(Node, Node), int> allPairsShortestPaths = BFS.AllPairsShortestPathTree<Graph, Edge<Node>, Node>(tree, counter);
            Dictionary<int, List<(Node, Node)>> lengthToNodePairs = new();
            maximumLength = -1;
            foreach (KeyValuePair<(Node, Node), int> pair in allPairsShortestPaths)
            {
                maximumLength = Math.Max(maximumLength, pair.Value);
                if (!lengthToNodePairs.ContainsKey(pair.Value))
                {
                    lengthToNodePairs[pair.Value] = new List<(Node, Node)>();
                }
                lengthToNodePairs[pair.Value].Add(pair.Key);
            }
            return lengthToNodePairs;
        }
    }
}
