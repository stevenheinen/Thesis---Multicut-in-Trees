// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that can generate random <see cref="DemandPair"/>s.
    /// </summary>
    public static class RandomDemandPairs
    {
        /// <summary>
        /// Generate a <see cref="List{T}"/> of <paramref name="numberOfDemandPairs"/> random <see cref="DemandPair"/>s in <paramref name="tree"/>.
        /// </summary>
        /// <param name="numberOfDemandPairs">The required number of <see cref="DemandPair"/>s.</param>
        /// <param name="tree">The <see cref="Graph"/> in which to generate the <see cref="DemandPair"/>s.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <returns>A <see cref="List{T}"/> of <paramref name="numberOfDemandPairs"/> random <see cref="DemandPair"/>s in <paramref name="tree"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfDemandPairs"/> is negative.</exception>
        public static List<DemandPair> GenerateRandomDemandPairs(int numberOfDemandPairs, Graph tree, Random random)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(tree, nameof(tree), "Trying to generate random demand pairs in a tree, but the tree is null!");
            Utilities.Utils.NullCheck(random, nameof(random), "Trying to generate random demand pairs in a tree, but the random is null!");
            if (numberOfDemandPairs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfDemandPairs), "Trying to generate random demand pairs in a tree, but the required number of demand pairs is negative!");
            }
#endif
            Counter counter = new Counter();

            List<DemandPair> demandPairs = new List<DemandPair>();
            for (uint i = 0; i < numberOfDemandPairs; i++)
            {
                // Pick a random index for the first endpoint
                int index1 = random.Next(tree.NumberOfNodes(counter));

                // Pick a random index for the second enpoint. Call the index for the first endpoint i.
                // This second index can be in the range 0, ..., i-1, i+1, ..., n.
                // The random number picked is in de range 0, ..., n-1.
                // To compensate for that, increment the random number with 1 if it is equal to or larger than i.
                // Now index1 and index2 are both picked uniform randomly, and are unique.
                // Plus, there is no unnecessary random looping until a correct number is found.
                int index2 = random.Next(tree.NumberOfNodes(counter) - 1);
                if (index2 >= index1)
                {
                    index2++;
                }

                Node endpoint1 = tree.Nodes(counter).ElementAt(index1);
                Node endpoint2 = tree.Nodes(counter).ElementAt(index2);
                demandPairs.Add(new DemandPair(i, endpoint1, endpoint2, tree));
            }
            return demandPairs;
        }
    }
}
