// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that handles writing instances to files and creating instances from such files.
    /// </summary>
    public static class InstanceReaderWriter
    {
        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not affect performance.
        /// </summary>
        private static readonly Counter MockCounter = new();

        /// <summary>
        /// Write an instance to a file.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> for this experiment.</param>
        /// <param name="tree">The <see cref="Graph"/> to be written.</param>
        /// <param name="demandPairs">The <see cref="DemandPair"/>s to be written.</param>
        /// <param name="optimalK">The optimal K value for the current instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/>, <paramref name="tree"/> or <paramref name="demandPairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the file to write to cannot be opened.</exception>
        public static void WriteInstance(int treeSeed, int dpSeed, CommandLineOptions options, Graph tree, IEnumerable<DemandPair> demandPairs, int optimalK)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to write an instance to a file, but the command line options are null!");
            Utils.NullCheck(tree, nameof(tree), "Trying to write an instance to a file, but the tree in the instance is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to write an instance to a file, but the list of demand pairs is null!");
#endif
            int numberOfNodes = tree.NumberOfNodes(MockCounter);
            int numberOfDPs = demandPairs.Count();
            string fileName = CreateFilePath(treeSeed, dpSeed, options);

            try
            {
                using StreamWriter sw = new(fileName);

                // Write the header with number of nodes, number of dps and optimal k
                sw.WriteLine("// [Number of nodes] [Number of demand pairs] [Optimal K]");
                sw.WriteLine($"{numberOfNodes} {numberOfDPs} {optimalK}");

                // Write all edges in the tree
                sw.WriteLine("// Edges, denoted by: [ID of endpoint 1] [ID of endpoint 2]");
                foreach (Edge<Node> edge in tree.Edges(MockCounter))
                {
                    sw.WriteLine($"{edge.Endpoint1.ID} {edge.Endpoint2.ID}");
                }

                // Write the endpoints of all dps in the tree
                sw.WriteLine("// Demand pairs, denoted by: [ID of endpoint 1] [ID of endpoint 2]");
                foreach (DemandPair demandPair in demandPairs)
                {
                    sw.WriteLine($"{demandPair.Node1.ID} {demandPair.Node2.ID}");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("An UnauthorizedAccessException was thrown! Message: " + e.Message);
                Console.WriteLine("Do you have enough permissions to write to this file? Is your antivirus software blocking access?");
                Console.WriteLine("File is not written, continuing...");
            }
        }

        /// <summary>
        /// Read an instance from a file.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> for this experiment.</param>
        /// <returns>If the file with this instance exists: a tuple with the <see cref="Graph"/>, a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s and the optimal K value. Otherwise, a tuple with <see langword="null"/>, <see langword="null"/> and -1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        /// <exception cref="BadFileFormatException">Thrown when one of the lines in the file we are reading is not in the correct format.</exception>
        public static (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) ReadInstance(int treeSeed, int dpSeed, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to read an instance from a file, but the command line options are null!");
#endif
            string fileName = CreateFilePath(treeSeed, dpSeed, options);

            if (!File.Exists(fileName))
            {
                // The file does not exist, return an empty instance so it can be created and written.
                return (null, null, -1);
            }

            int optimalK;
            int numberOfNodes;

            List<(int, int)> edges = new();
            List<(int, int)> dps = new();

            using (StreamReader sr = new(fileName))
            {
                string line = sr.ReadLine();

                // Skip the lines with a comment
                while (line.StartsWith("//"))
                {
                    line = sr.ReadLine();
                }

                string[] header = line.Split();

                if (header.Length != 3 || !int.TryParse(header[0], out numberOfNodes) || !int.TryParse(header[1], out int numberOfDPs) || !int.TryParse(header[2], out optimalK))
                {
                    throw new BadFileFormatException($"Trying to read an instance from a file. The second line ({line}) is not in the correct format. Expected line with three integers, separated by a space. The first one should represent the number of nodes, the second one the number of demand pairs, and the final one the optimal K value.");
                }

                edges = ReadEdges(ref line, sr, numberOfNodes);
                dps = ReadDemandPairs(ref line, sr, numberOfDPs);
            }

            Graph tree = Utils.CreateGraphWithEdges(numberOfNodes, edges);
            CountedCollection<DemandPair> demandPairs = Utils.CreateDemandPairs(tree, dps);

            return (tree, demandPairs, optimalK);
        }

        /// <summary>
        /// Read the edges from the instance file.
        /// </summary>
        /// <param name="line">A single line in the file.</param>
        /// <param name="sr">The <see cref="StreamReader"/> we use.</param>
        /// <param name="numberOfNodes">The number of nodes in the instance.</param>
        /// <returns>A <see cref="List{T}"/> with tuples of two <see cref="int"/>s that are the IDs of the endpoints of an edge.</returns>
        private static List<(int, int)> ReadEdges(ref string line, StreamReader sr, int numberOfNodes)
        {
            List<(int, int)> edges = new();

            // Reading edges, skip comment line
            line = sr.ReadLine();
            while (line.StartsWith("//"))
            {
                line = sr.ReadLine();
            }

            for (int i = 0; i < numberOfNodes - 1; i++)
            {
                string[] edge = line.Split();

                if (edge.Length != 2 || !int.TryParse(edge[0], out int endpoint1) || !int.TryParse(edge[1], out int endpoint2))
                {
                    throw new BadFileFormatException($"Trying to read an instance from a file. The line ({line}) is not in the correct format. Expected line with two integers, separated by a space, representing the IDs of the endpoints of an edge.");
                }

                edges.Add((endpoint1, endpoint2));
                line = sr.ReadLine();
                while (line.StartsWith("//"))
                {
                    line = sr.ReadLine();
                }
            }

            return edges;
        }

        /// <summary>
        /// Read the <see cref="DemandPair"/>s from the instance file.
        /// </summary>
        /// <param name="line">A single line in the file.</param>
        /// <param name="sr">The <see cref="StreamReader"/> we use.</param>
        /// <param name="numberOfDPs">The number of <see cref="DemandPair"/>s in the instance.</param>
        /// <returns>A <see cref="List{T}"/> with tuples of two <see cref="int"/>s that are the IDs of the endpoints of a <see cref="DemandPair"/>.</returns>
        private static List<(int, int)> ReadDemandPairs(ref string line, StreamReader sr, int numberOfDPs)
        {
            List<(int, int)> dps = new();

            // Reading demand pairs
            for (int i = 0; i < numberOfDPs; i++)
            {
                string[] dp = line.Split();

                if (dp.Length != 2 || !int.TryParse(dp[0], out int endpoint1) || !int.TryParse(dp[1], out int endpoint2))
                {
                    throw new BadFileFormatException($"Trying to read an instance from a file. The line ({line}) is not in the correct format. Expected line with two integers, separated by a space, representing the IDs of the endpoints of a demand pair.");
                }

                dps.Add((endpoint1, endpoint2));

                if (i != numberOfDPs - 1)
                {
                    line = sr.ReadLine();
                    while (line.StartsWith("//"))
                    {
                        line = sr.ReadLine();
                    }
                }
            }

            return dps;
        }

        /// <summary>
        /// Create the path of the file with the correct identifiers.
        /// </summary>
        /// <param name="treeSeed">The seed used for the random number generator for the <see cref="AbstractGraph{TEdge, TNode}"/> in the instance.</param>
        /// <param name="dpSeed">The seed used for the random number generator for the <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> for this experiment.</param>
        /// <returns>The path of the file for the instance with the given parameters.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private static string CreateFilePath(int treeSeed, int dpSeed, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to create the filename for an instance given command line options, but the command line options are null!");
#endif
            StringBuilder fileNameBuilder = new();
            fileNameBuilder.Append($"{options.InstanceDirectory}\\MulticutInstance_treeSeed={treeSeed}_dpSeed={dpSeed}_treeType={options.InputTreeType}_dpType={options.InputDemandPairsType}");

            if (options.InputTreeType == InputTreeType.Prufer || options.InputTreeType == InputTreeType.Caterpillar || options.InputTreeType == InputTreeType.Degree3Tree)
            {
                fileNameBuilder.Append($"_nrNodes={options.NumberOfNodes}");
            }
            else
            {
                fileNameBuilder.Append($"_treeFile=({options.InstanceFilePath})".Replace(".", "[dot]").Replace("\\", "[slash]").Replace(":", ""));
            }

            if (options.InputDemandPairsType == InputDemandPairsType.Random)
            {
                fileNameBuilder.Append($"_nrDPs={options.NumberOfDemandPairs}");
            }
            else if (options.InputDemandPairsType == InputDemandPairsType.Fixed)
            {
                fileNameBuilder.Append($"_dpFile=({options.DemandPairFilePath})".Replace(".", "[dot]").Replace("\\", "[slash]").Replace(":", ""));
            }
            else if (options.InputDemandPairsType == InputDemandPairsType.LengthDistribution)
            {
                fileNameBuilder.Append($"_dpLengthDist=({options.DistanceDistribution})");
            }
            else if (options.InputDemandPairsType == InputDemandPairsType.ThroughKnownSolution)
            {
                fileNameBuilder.Append($"_nrDPs={options.NumberOfDemandPairs}_solSize={options.MaxSolutionSize}");
            }

            fileNameBuilder.Append(".txt");
            return fileNameBuilder.ToString();
        }
    }
}
