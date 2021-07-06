// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Generator class for <see cref="DemandPair"/>s through a known solution.
    /// </summary>
    public static class KnownSolutionDemandPairs
    {
        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not affect performance.
        /// </summary>
        private readonly static Counter MockCounter = new();

        /// <summary>
        /// Generates <see cref="DemandPair"/>s through a known solution of a given size.
        /// </summary>
        /// <param name="tree">The <see cref="Graph"/> in the instance.</param>
        /// <param name="numberOfDemandPairs">The number of <see cref="DemandPair"/>s to be generated.</param>
        /// <param name="solutionSize">The size of the known solution.</param>
        /// <param name="random">The <see cref="Random"/> to be used for random number generation.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="DemandPair"/>s that each go through exactly one <see cref="Edge{TNode}"/> in a known solution.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfDemandPairs"/> or <paramref name="solutionSize"/> is smaller than 1.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="numberOfDemandPairs"/> is smaller than <paramref name="solutionSize"/>.</exception>
        public static List<DemandPair> GenerateDemandPairsThroughKnownSolution(Graph tree, int numberOfDemandPairs, int solutionSize, Random random)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create demand pairs from a known solution, but the tree is null!");
            Utils.NullCheck(random, nameof(random), "Trying to create demand pairs from a known solution, but the random number generator is null!");
            if (numberOfDemandPairs < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfDemandPairs), $"Trying to create demand pairs from a known solution, but the number of demand pairs to generate ({numberOfDemandPairs}) is smaller than one!");
            }
            if (solutionSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(solutionSize), $"Trying to create demand pairs from a known solution, but the size of the solution ({solutionSize}) is smaller than one!");
            }
            if (numberOfDemandPairs < solutionSize)
            {
                throw new ArgumentException($"The number of demand pairs ({numberOfDemandPairs}) is smaller than the size of the solution ({solutionSize}). This means the solution will have a smaller size than requested.");
            }
#endif
            List<Edge<Node>> solution = GenerateKnownSolution(tree, solutionSize, random).ToList();
            Dictionary<Edge<Node>, (List<Node>, List<Node>)> possibleDemandPairEndpoints = ComputePossibleDemandPairEndpoints(solution);
            List<DemandPair> result = new();
            for (int i = 0; i < solutionSize; i++)
            {
                Edge<Node> edge = solution[i];
                Node endpoint1 = possibleDemandPairEndpoints[edge].Item1.PickRandom(random);
                Node endpoint2 = possibleDemandPairEndpoints[edge].Item2.PickRandom(random);
                result.Add(new DemandPair((uint)i, endpoint1, endpoint2, tree));
            }
            for (uint i = (uint)solutionSize; i < numberOfDemandPairs; i++)
            {
                Edge<Node> edge = solution.PickRandom(random);
                Node endpoint1 = possibleDemandPairEndpoints[edge].Item1.PickRandom(random);
                Node endpoint2 = possibleDemandPairEndpoints[edge].Item2.PickRandom(random);
                result.Add(new DemandPair(i, endpoint1, endpoint2, tree));
            }
            return result;
        }

        /// <summary>
        /// Picks random <see cref="Edge{TNode}"/>s in <paramref name="tree"/> as the known solution for the instance.
        /// </summary>
        /// <param name="tree">The <see cref="Graph"/> in the instance.</param>
        /// <param name="solutionSize">The size the solution should be.</param>
        /// <param name="random">The <see cref="Random"/> to be used to pick the solution.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with <see cref="Edge{TNode}"/>s that form the known solution.</returns>
        private static IEnumerable<Edge<Node>> GenerateKnownSolution(Graph tree, int solutionSize, Random random)
        {
            int[] indices = new int[tree.NumberOfEdges(MockCounter)];
            for (int i = 0; i < tree.NumberOfEdges(MockCounter); i++)
            {
                indices[i] = i;
            }
            indices.Shuffle(random);
            return indices.Take(solutionSize).Select(i => tree.Edges(MockCounter).ElementAt(i));
        }
        
        /// <summary>
        /// Picks possible endpoints for the <see cref="DemandPair"/>s that will go through each <see cref="Edge{TNode}"/> in <paramref name="edges"/>.
        /// </summary>
        /// <param name="edges">The <see cref="Edge{TNode}"/>s that form the known solution.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with per <see cref="Edge{TNode}"/> a tuple with the possible endpoints of the <see cref="DemandPair"/>s going through it.</returns>
        private static Dictionary<Edge<Node>, (List<Node>, List<Node>)> ComputePossibleDemandPairEndpoints(IEnumerable<Edge<Node>> edges)
        {
            Dictionary<Edge<Node>, (List<Node>, List<Node>)> result = new();
            HashSet<Node> allEdgeEndpoints = new();
            foreach (Edge<Node> edge in edges)
            {
                allEdgeEndpoints.Add(edge.Endpoint1);
                allEdgeEndpoints.Add(edge.Endpoint2);
            }
            foreach (Edge<Node> edge in edges)
            {
                HashSet<Node> invalidEndpoints = new();
                foreach (Edge<Node> otherEdge in edges)
                {
                    if (edge == otherEdge)
                    {
                        continue;
                    }
                    List<Node> path = DFS.FindPathBetween(edge.Endpoint1, otherEdge.Endpoint1, MockCounter, new HashSet<Node>() { edge.Endpoint2 });
                    if (path.Last() != otherEdge.Endpoint1)
                    {
                        continue;
                    }
                    if (path.Contains(otherEdge.Endpoint2))
                    {
                        invalidEndpoints.Add(otherEdge.Endpoint1);
                    }
                    else
                    {
                        invalidEndpoints.Add(otherEdge.Endpoint2);
                    }
                }
                invalidEndpoints.Add(edge.Endpoint2);
                List<Node> cc1 = DFS.FindConnectedComponent(edge.Endpoint1, MockCounter, invalidEndpoints);
                invalidEndpoints.Clear();
                foreach (Edge<Node> otherEdge in edges)
                {
                    if (edge == otherEdge)
                    {
                        continue;
                    }
                    List<Node> path = DFS.FindPathBetween(edge.Endpoint2, otherEdge.Endpoint1, MockCounter, new HashSet<Node>() { edge.Endpoint1 });
                    if (path.Last() != otherEdge.Endpoint1)
                    {
                        continue;
                    }
                    if (path.Contains(otherEdge.Endpoint2))
                    {
                        invalidEndpoints.Add(otherEdge.Endpoint1);
                    }
                    else
                    {
                        invalidEndpoints.Add(otherEdge.Endpoint2);
                    }
                }
                invalidEndpoints.Add(edge.Endpoint1);
                List<Node> cc2 = DFS.FindConnectedComponent(edge.Endpoint2, MockCounter, invalidEndpoints);
                result[edge] = (cc1, cc2);
            }
            return result;
        }
    }
}
