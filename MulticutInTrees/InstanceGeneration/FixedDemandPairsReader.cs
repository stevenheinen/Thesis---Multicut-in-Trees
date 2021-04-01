// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class for reading <see cref="DemandPair"/>s from a file.
    /// </summary>
    public static class FixedDemandPairsReader
    {
        /// <summary>
        /// Reads a list of <see cref="DemandPair"/>s that is saved in the file on <paramref name="filePath"/>.
        /// <br/>
        /// The file should contain a line with the number of <see cref="DemandPair"/>s, and then a line for each <see cref="DemandPair"/>, with the IDs of the endpoints separated by a space, and nothing else.
        /// </summary>
        /// <param name="tree">The <see cref="Tree{N}"/> in which to create the <see cref="DemandPair"/>s.</param>
        /// <param name="filePath">The path of the file where the list is stored.</param>
        /// <returns>A <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s as read from <paramref name="filePath"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="filePath"/> is <see langword="null"/>.</exception>
        /// <exception cref="FileNotFoundException">Thrown when <paramref name="filePath"/> is not a valid file.</exception>
        public static CountedList<DemandPair> ReadDemandPairs(Tree<TreeNode> tree, string filePath)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to read demand pairs from a file, but the tree in which these demand pairs exist is null!");
            Utils.NullCheck(filePath, nameof(filePath), "Trying to read demand pairs from a file, but the path to the file is null!");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Trying to read demand pairs from the file {filePath}, but the file does not exist!");
            }
#endif
            Counter mockCounter = new Counter();

            int numberOfDPs;
            List<(int, int)> dps = new List<(int, int)>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                numberOfDPs = int.Parse(sr.ReadLine());

                for (int i = 0; i < numberOfDPs; i++)
                {
                    string[] dp = sr.ReadLine().Split();
                    dps.Add((int.Parse(dp[0]), int.Parse(dp[1])));
                }
            }

            return Utils.CreateDemandPairs(tree, dps);
        }
    }
}
