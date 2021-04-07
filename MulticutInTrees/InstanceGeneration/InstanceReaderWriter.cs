// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.CountedDatastructures;
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
        private readonly static Counter MockCounter = new Counter();

        /// <summary>
        /// Write an instance to a file.
        /// </summary>
        /// <param name="randomSeed">The seed used for random number generation.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> for this experiment.</param>
        /// <param name="tree">The <see cref="Tree{N}"/> to be written.</param>
        /// <param name="demandPairs">The <see cref="DemandPair"/>s to be written.</param>
        /// <param name="optimalK">The optimal K value for the current instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/>, <paramref name="tree"/> or <paramref name="demandPairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the file to write to cannot be opened.</exception>
        public static void WriteInstance(int randomSeed, CommandLineOptions options, Tree<TreeNode> tree, List<DemandPair> demandPairs, int optimalK)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to write an instance to a file, but the command line options are null!");
            Utils.NullCheck(tree, nameof(tree), "Trying to write an instance to a file, but the tree in the instance is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to write an instance to a file, but the list of demand pairs is null!");
#endif
            int numberOfNodes = tree.NumberOfNodes(MockCounter);
            int numberOfDPs = demandPairs.Count;
            string fileName = CreateFilePath(randomSeed, options);

            try
            {
                using StreamWriter sw = new StreamWriter(fileName);

                // Write the header with number of nodes, number of dps and optimal k
                sw.WriteLine("// [Number of nodes] [Number of demand pairs] [Optimal K]");
                sw.WriteLine($"{numberOfNodes} {numberOfDPs} {optimalK}");

                // Write the root of the tree
                sw.WriteLine("// ID of the root of the tree");
                sw.WriteLine($"{tree.GetRoot(MockCounter).ID}");

                // Write all edges in the tree
                sw.WriteLine("// Edges, denoted by: [ID of endpoint 1] [ID of endpoint 2]");
                foreach ((TreeNode, TreeNode) edge in tree.Edges(MockCounter))
                {
                    sw.WriteLine($"{edge.Item1.ID} {edge.Item2.ID}");
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
        /// <param name="randomSeed">The seed used for random number generation.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> for this experiment.</param>
        /// <returns>If the file with this instance exists: a tuple with the <see cref="Tree{N}"/>, a <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s and the optimal K value. Otherwise, a tuple with <see langword="null"/>, <see langword="null"/> and -1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public static (Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, int optimalK) ReadInstance(int randomSeed, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to read an instance from a file, but the command line options are null!");
#endif
            string fileName = CreateFilePath(randomSeed, options);

            if (!File.Exists(fileName))
            {
                // The file does not exist, return an empty instance so it can be created and written.
                return (null, null, -1);
            }

            int optimalK;
            int numberOfNodes;
            int numberOfDPs;

            int root;
            List<(int, int)> edges = new List<(int, int)>();

            List<(int, int)> dps = new List<(int, int)>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                // Skip the first line with a comment
                sr.ReadLine();
                string[] header = sr.ReadLine().Split();
                numberOfNodes = int.Parse(header[0]);
                numberOfDPs = int.Parse(header[1]);
                optimalK = int.Parse(header[2]);

                sr.ReadLine();
                root = int.Parse(sr.ReadLine());

                // Reading edges, skip comment line
                sr.ReadLine();
                for (int i = 0; i < numberOfNodes - 1; i++)
                {
                    string[] edge = sr.ReadLine().Split();
                    edges.Add((int.Parse(edge[0]), int.Parse(edge[1])));
                }

                // Reading demand pairs, skip comment line
                sr.ReadLine();
                for (int i = 0; i < numberOfDPs; i++)
                {
                    string[] dp = sr.ReadLine().Split();
                    dps.Add((int.Parse(dp[0]), int.Parse(dp[1])));
                }
            }

            Tree<TreeNode> tree = Utils.CreateTreeWithEdges(numberOfNodes, edges);
            CountedList<DemandPair> demandPairs = Utils.CreateDemandPairs(tree, dps);

            return (tree, demandPairs, optimalK);           
        }

        /// <summary>
        /// Create the path of the file with the correct identifiers.
        /// </summary>
        /// <param name="randomSeed">The seed used for random number generation.</param>
        /// <param name="options">The <see cref="CommandLineOptions"/> for this experiment.</param>
        /// <returns>The path of the file for the instance with the given parameters.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        private static string CreateFilePath(int randomSeed, CommandLineOptions options)
        {
#if !EXPERIMENT
            Utils.NullCheck(options, nameof(options), "Trying to create the filename for an instance given command line options, but the command line options are null!");
#endif
            StringBuilder fileNameBuilder = new StringBuilder();
            fileNameBuilder.Append($"{options.InstanceDirectory}\\MulticutInstance_seed={randomSeed}_treeType={options.InputTreeType}_dpType={options.InputDemandPairsType}");

            if (options.InputTreeType == InputTreeType.Prufer || options.InputTreeType == InputTreeType.Caterpillar)
            {
                fileNameBuilder.Append($"_nrNodes={options.NumberOfNodes}");
            }
            else
            {
                fileNameBuilder.Append($"_treeFile=({options.InstanceFilePath})".Replace(".", "[dot]"));
            }

            if (options.InputDemandPairsType == InputDemandPairsType.Random)
            {
                fileNameBuilder.Append($"_nrDPs={options.NumberOfDemandPairs}");
            }
            else if (options.InputDemandPairsType == InputDemandPairsType.Fixed)
            {
                fileNameBuilder.Append($"_dpFile=({options.DemandPairFilePath})".Replace(".", "[dot]"));
            }
            else if (options.InputDemandPairsType == InputDemandPairsType.LengthDistribution)
            {
                fileNameBuilder.Append($"_dpLengthDist=({options.DistanceDistribution})");
            }

            fileNameBuilder.Append(".txt");
            return fileNameBuilder.ToString();
        }
    }
}
