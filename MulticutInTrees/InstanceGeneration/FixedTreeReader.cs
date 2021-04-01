// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class for reading a <see cref="Tree{N}"/> from a file.
    /// </summary>
    public static class FixedTreeReader
    {
        /// <summary>
        /// Reads a <see cref="Tree{N}"/> that is saved in the file on <paramref name="filePath"/>.
        /// <br/>
        /// The file should contain a line with the number of nodes in the tree, and then a line for each edge, with the IDs of the endpoints separated by a space, and nothing else.
        /// </summary>
        /// <param name="filePath">The path of the file where the <see cref="Tree{N}"/> is stored.</param>
        /// <returns>A <see cref="Tree{N}"/> as read from <paramref name="filePath"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is <see langword="null"/>.</exception>
        /// <exception cref="FileNotFoundException">Thrown when <paramref name="filePath"/> is not a valid file.</exception>
        public static Tree<TreeNode> ReadTree(string filePath)
        {
#if !EXPERIMENT
            Utils.NullCheck(filePath, nameof(filePath), "Trying to read a tree from a file, but the path to the file is null!");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Trying to read a tree from the file {filePath}, but the file does not exist!");
            }
#endif
            Counter mockCounter = new Counter();

            int numberOfNodes;
            List<(int, int)> edges = new List<(int, int)>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                numberOfNodes = int.Parse(sr.ReadLine());

                for (int i = 0; i < numberOfNodes - 1; i++)
                {
                    string[] edge = sr.ReadLine().Split();
                    edges.Add((int.Parse(edge[0]), int.Parse(edge[1])));
                }
            }

            return Utils.CreateTreeWithEdges(numberOfNodes, edges);
        }
    }
}
