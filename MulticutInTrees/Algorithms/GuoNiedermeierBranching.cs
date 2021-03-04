// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Implementation of the branching algorithm by Guo and Niedermeier to solve Multicut in Trees.
    /// <br/>
    /// Source: <see href="https://doi.org/10.1002/net.20081"/>
    /// </summary>
    public class GuoNiedermeierBranching
    {
        /// <summary>
        /// The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the input.
        /// </summary>
        private CountedList<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The size the cutset is allowed to be.
        /// </summary>
        private int K { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing the <see cref="TreeNode"/> that is the least common ancestor per <see cref="DemandPair"/>.
        /// </summary>
        private CountedDictionary<DemandPair, TreeNode> LeastCommonAncestors { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// <see cref="PerformanceMeasurements"/> this <see cref="Algorithm"/> used.
        /// </summary>
        private PerformanceMeasurements Measurements { get; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not impact performance of this <see cref="Algorithm"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Instructor for <see cref="GuoNiedermeierBranching"/>.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public GuoNiedermeierBranching(MulticutInstance instance)
        {
#if !EXPERIMENT
            Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of the GuoNiedermeier branching algorithm, but the instance we want to solve is null!");
#endif
            DemandPairs = instance.DemandPairs;
            K = instance.K;

            Measurements = new PerformanceMeasurements("Guo and Niedermeier branching");
            MockCounter = new Counter();
            LeastCommonAncestors = new CountedDictionary<DemandPair, TreeNode>();
            DemandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>();
        }

        /// <summary>
        /// Try to solve the instance.
        /// </summary>
        /// <returns>A tuple of an <see cref="bool"/> whether the instance is solvable and a <see cref="List{T}"/> of tuples of two <see cref="TreeNode"/>s representing the edges in the solution.</returns>
        public (bool, List<(TreeNode, TreeNode)>) Run()
        {
            FillDemandPathsPerEdge();

            Measurements.TimeSpentModifyingInstance.Start();

            FindLeastCommonAncestors();

            CountedList<DemandPair> sortedDemandPairs = new CountedList<DemandPair>(LeastCommonAncestors.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).OrderByDescending(e => e.Value.DepthFromRoot(MockCounter)).Select(e => e.Key), Measurements.DemandPairsOperationsCounter);
            HashSet<DemandPair> solvedDemandPairs = new HashSet<DemandPair>();
            List<(TreeNode, TreeNode)> solution = new List<(TreeNode, TreeNode)>();
            (bool solved, int size) = RecSolve(sortedDemandPairs, 0, solvedDemandPairs, solution);

            Measurements.TimeSpentModifyingInstance.Stop();

            PrintCounters();
            
            return (solved, solution);
        }

        /// <summary>
        /// Print the <see cref="PerformanceMeasurements"/> this algorithm used to the console.
        /// </summary>
        private void PrintCounters()
        {
            Console.WriteLine(Measurements);
        }

        /// <summary>
        /// Fills <see cref="DemandPairsPerEdge"/>.
        /// </summary>
        private void FillDemandPathsPerEdge()
        {
            // For each demand pair in the instance...
            foreach (DemandPair demandPair in DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                // For each edge on this demand pair...
                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath(Measurements.DemandPairsOperationsCounter))
                {
                    // Add this edge to the DemandPairsPerEdge dictionary.
                    (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);
                    if (!DemandPairsPerEdge.ContainsKey(usedEdge, Measurements.DemandPairsPerEdgeKeysCounter))
                    {
                        DemandPairsPerEdge[usedEdge, Measurements.DemandPairsPerEdgeKeysCounter] = new CountedList<DemandPair>();
                    }
                    DemandPairsPerEdge[usedEdge, Measurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, Measurements.DemandPairsPerEdgeValuesCounter);
                }
            }
        }

        /// <summary>
        /// Recursive function used when solving the instance.
        /// </summary>
        /// <param name="sortedDemandPairs"><see cref="List{T}"/> with <see cref="DemandPair"/>s that are sorted on the non-ascending depth of their least common ancestor.</param>
        /// <param name="currentIndex">The index of the current <see cref="DemandPair"/> in <paramref name="solvedDemandPairs"/> we need to look at.</param>
        /// <param name="solvedDemandPairs"><see cref="HashSet{T}"/> with <see cref="DemandPair"/>s that are already separated.</param>
        /// <param name="solution"><see cref="List{T}"/> with the solution thus far. Is added to in this method.</param>
        /// <returns><see langword="true"/> if a solution can be found with these parameters, <see langword="false"/> otherwise.</returns>
        private (bool, int) RecSolve(CountedList<DemandPair> sortedDemandPairs, int currentIndex, HashSet<DemandPair> solvedDemandPairs, List<(TreeNode, TreeNode)> solution)
        {
            int nrPairs = sortedDemandPairs.Count(Measurements.DemandPairsOperationsCounter);
            for (int i = currentIndex; i < nrPairs; i++)
            {
                if (solution.Count > K)
                {
                    return (false, K + 1);
                }

                DemandPair demandPair = sortedDemandPairs[i, Measurements.DemandPairsOperationsCounter];

                if (solvedDemandPairs.Contains(demandPair))
                {
                    continue;
                }

                TreeNode leastCommonAncestor = LeastCommonAncestors[demandPair, Measurements.DemandPairsOperationsCounter];
                (TreeNode, TreeNode) edgeInSolution;

                if (leastCommonAncestor == demandPair.Node1)
                {
                    edgeInSolution = Utils.OrderEdgeSmallToLarge(demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First());
                }
                else if (leastCommonAncestor == demandPair.Node2)
                {
                    edgeInSolution = Utils.OrderEdgeSmallToLarge(demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Last());
                }
                else
                {
                    (TreeNode, TreeNode) before = demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter).First(edge => edge.Item2 == leastCommonAncestor);
                    ((TreeNode, TreeNode) _, (TreeNode, TreeNode) after) = demandPair.EdgeBeforeAndAfter(before, Measurements.TreeOperationsCounter);

                    (TreeNode, TreeNode) edge1 = Utils.OrderEdgeSmallToLarge(before);
                    List<(TreeNode, TreeNode)> tempSolution1 = new List<(TreeNode, TreeNode)>(solution) { edge1 };
                    HashSet<DemandPair> tempSolvedDemandPairs1 = new HashSet<DemandPair>(solvedDemandPairs);
                    foreach (DemandPair solvedDemandPair in DemandPairsPerEdge[edge1, Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                    {
                        tempSolvedDemandPairs1.Add(solvedDemandPair);
                    }

                    (TreeNode, TreeNode) edge2 = Utils.OrderEdgeSmallToLarge(after);
                    List<(TreeNode, TreeNode)> tempSolution2 = new List<(TreeNode, TreeNode)>(solution) { edge2 };
                    HashSet<DemandPair> tempSolvedDemandPairs2 = new HashSet<DemandPair>(solvedDemandPairs);
                    foreach (DemandPair solvedDemandPair in DemandPairsPerEdge[edge2, Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                    {
                        tempSolvedDemandPairs2.Add(solvedDemandPair);
                    }

                    (bool solved1, int size1) = RecSolve(sortedDemandPairs, i + 1, tempSolvedDemandPairs1, tempSolution1);
                    (bool solved2, int size2) = RecSolve(sortedDemandPairs, i + 1, tempSolvedDemandPairs2, tempSolution2);

                    if (!solved1 && !solved2) 
                    {
                        return (false, K + 1);
                    }
                    if (solved1 && size1 <= size2)
                    {
                        edgeInSolution = edge1;
                    }
                    else if (solved2)
                    {
                        edgeInSolution = edge2;
                    }
                    else
                    {
                        throw new Exception($"Apparently there is an unhandled case... branch 1 solved? {solved1}, branch 1 size? {size1}, branch 2 solved? {solved2}, branch 2 size {size2}");
                    }
                }

                solution.Add(edgeInSolution);
                foreach (DemandPair solvedDemandPair in DemandPairsPerEdge[edgeInSolution, Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                {
                    solvedDemandPairs.Add(solvedDemandPair);
                }
            }

            if (solution.Count > K)
            {
                return (false, K + 1);
            }
            return (true, solution.Count);
        }

        /// <summary>
        /// Find all the least common ancestors for each demandpair in <see cref="DemandPairs"/> and save them in <see cref="LeastCommonAncestors"/>.
        /// </summary>
        private void FindLeastCommonAncestors()
        {
            foreach (DemandPair demandPair in DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                // Find the ancestors for each endpoint of the demand pair.
                CountedList<TreeNode> ancestors1 = new CountedList<TreeNode>(demandPair.Node1.FindAllAncestors(), Measurements.TreeOperationsCounter);
                CountedList<TreeNode> ancestors2 = new CountedList<TreeNode>(demandPair.Node2.FindAllAncestors(), Measurements.TreeOperationsCounter);
                int length1 = ancestors1.Count(Measurements.TreeOperationsCounter);
                int length2 = ancestors2.Count(Measurements.TreeOperationsCounter);
                int minSize = Math.Min(length1, length2) + 1;

                // Start from the root, while the path at index i (looked from back to front) is still the same, go to the next node.
                int i = 1;
                while (i < minSize && ancestors1[length1 - i, Measurements.TreeOperationsCounter] == ancestors2[length2 - i, Measurements.TreeOperationsCounter])
                {
                    i++;
                }

                // At index i (from the back), we have two nodes that are different, so the least common ancestor is the one before these.
                LeastCommonAncestors[demandPair, Measurements.DemandPairsOperationsCounter] = ancestors1[length1 - (i - 1), Measurements.TreeOperationsCounter];
            }
        }
    }
}
