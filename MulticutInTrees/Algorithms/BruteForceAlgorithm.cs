// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Brute force algorithm that checks all subsets of a given size to find a solution.
    /// <br/>
    /// <b>WARNING: THIS IS AN ACTUAL O^*(2^k) ALGORITHM, DO NOT USE ON LARGE INSTANCES!!!</b>
    /// </summary>
    public class BruteForceAlgorithm
    {
        /// <summary>
        /// The <see cref="MulticutInstance"/> this <see cref="Algorithm"/> runs on.
        /// </summary>
        private MulticutInstance Instance { get; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not impact performance of this <see cref="Algorithm"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for a <see cref="BruteForceAlgorithm"/>.
        /// <br/>
        /// <b>WARNING: THIS IS AN ACTUAL O^*(2^k) ALGORITHM, DO NOT USE ON LARGE INSTANCES!!!</b>
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> to solve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public BruteForceAlgorithm(MulticutInstance instance)
        {
#if !EXPERIMENT
            Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of the brute force algorithm, but the multicut instance to solve is null!");
#endif
            Instance = instance;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Tries to find a solution with size at most K by checking all subsets of size K.
        /// </summary>
        /// <returns><see langword="true"/> if the instance could be solved, <see langword="false"/> otherwise.</returns>
        public bool Run()
        {
            // Find all subsets of size K.
            IEnumerable<IEnumerable<Edge<Node>>> subsets = Instance.Tree.Edges(MockCounter).AllSubsetsOfSize(Instance.K);

            // For each subset, check if it would separate all demand pairs.
            foreach (IEnumerable<Edge<Node>> subset in subsets)
            {
                bool correctsubset = true;
                foreach (DemandPair dp in Instance.DemandPairs.GetCountedEnumerable(MockCounter))
                {
                    bool separated = false;
                    foreach (Edge<Node> edge in subset)
                    {
                        if (dp.EdgeIsPartOfPath(edge, MockCounter))
                        {
                            separated = true;
                            break;
                        }
                    }
                    if (!separated)
                    {
                        correctsubset = false;
                        break;
                    }
                }
                if (correctsubset)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
