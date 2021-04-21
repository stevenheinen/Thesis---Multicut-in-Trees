// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class for reading a <see cref="AbstractGraph{TEdge, TNode}"/> from a file.
    /// </summary>
    public static class FixedTreeReader
    {
        /// <summary>
        /// Reads a <see cref="AbstractGraph{TEdge, TNode}"/> that is saved in the file on <paramref name="filePath"/>.
        /// <br/>
        /// The file should contain a line with the number of nodes in the tree, and then a line for each edge, with the IDs of the endpoints separated by a space, and nothing else.
        /// </summary>
        /// <param name="filePath">The path of the file where the <see cref="AbstractGraph{TEdge, TNode}"/> is stored.</param>
        /// <returns>A <see cref="Graph"/> as read from <paramref name="filePath"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is <see langword="null"/>.</exception>
        /// <exception cref="FileNotFoundException">Thrown when <paramref name="filePath"/> is not a valid file.</exception>
        /// <exception cref="BadFileFormatException">Thrown when a line in the file cannot be parsed to a number.</exception>
        public static Graph ReadTree(string filePath)
        {
#if !EXPERIMENT
            Utils.NullCheck(filePath, nameof(filePath), "Trying to read a tree from a file, but the path to the file is null!");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Trying to read a tree from the file {filePath}, but the file does not exist!");
            }
#endif
            int numberOfNodes;
            List<(int, int)> edges = new();

            using (StreamReader sr = new(filePath))
            {
                string line = sr.ReadLine();
                if (!int.TryParse(line, out numberOfNodes))
                {
                    throw new BadFileFormatException($"Reading a file with a fixed tree. The first line ({line}) could not be parsed to an integer! Expected a single number that represents the number of nodes.");
                }

                for (int i = 0; i < numberOfNodes - 1; i++)
                {
                    line = sr.ReadLine();
                    string[] edge = line.Split();
                    if (edge.Length != 2 || !int.TryParse(edge[0], out int endpoint1) || !int.TryParse(edge[1], out int endpoint2))
                    {
                        throw new BadFileFormatException($"Reading a file with a fixed tree. Could not parse line ({line}) to an edge. Expected a line with two numbers and a space between them.");
                    }

                    edges.Add((endpoint1, endpoint2));
                }
            }

            return Utils.CreateGraphWithEdges(numberOfNodes, edges);
        }
    }
}
