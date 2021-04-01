// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

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
        /// <param name="instanceDirectoryPath">The path to the directory where instances are placed.</param>
        /// <param name="randomSeed">The seed used for random number generation.</param>
        /// <param name="tree">The <see cref="Tree{N}"/> to be written.</param>
        /// <param name="inputTreeType">The <see cref="InputTreeType"/> used to generate the tree in the current instance.</param>
        /// <param name="treeFileName">The name of the file that contains the instance from which we created the current tree. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="demandPairs">The <see cref="DemandPair"/>s to be written.</param>
        /// <param name="inputDemandPairsType">The <see cref="InputDemandPairsType"/> used to generate the demand pairs in the current instance.</param>
        /// <param name="dpFileName">The name of the file that contains the fixed demand pair endpoints. Not necessary for all <see cref="InputDemandPairsType"/>s.</param>
        /// <param name="dpLengthDist">The length distribution for the demand pairs. Not necessary for all <see cref="InputDemandPairsType"/>s.</param>
        /// <param name="optimalK">The optimal K value for the current instance.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the file to write to cannot be opened.</exception>
        public static void WriteInstance(string instanceDirectoryPath, int randomSeed, Tree<TreeNode> tree, InputTreeType inputTreeType, string treeFileName, List<DemandPair> demandPairs, InputDemandPairsType inputDemandPairsType, string dpFileName, string dpLengthDist, int optimalK)
        {
            int numberOfNodes = tree.NumberOfNodes(MockCounter);
            int numberOfDPs = demandPairs.Count;
            string fileName = CreateFilePath(instanceDirectoryPath, randomSeed, inputTreeType, numberOfNodes, treeFileName, inputDemandPairsType, numberOfDPs, dpFileName, dpLengthDist);

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
        /// <param name="instanceDirectoryPath">The path to the directory where instances are placed.</param>
        /// <param name="randomSeed">The seed used for random number generation.</param>
        /// <param name="inputTreeType">The <see cref="InputTreeType"/> used to generate the tree in the current instance.</param>
        /// <param name="numberOfNodes">The number of nodes in the tree in the current instance. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="treeFileName">The name of the file that contains the instance from which we created the current tree. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="inputDemandPairsType">The <see cref="InputDemandPairsType"/> used to generate the demand pairs in the current instance.</param>
        /// <param name="numberOfDPs">The number of demand pairs in the instance. Not necessary for all <see cref="InputDemandPairsType"/>s.</param>
        /// <param name="dpFileName">The name of the file that contains the fixed demand pair endpoints. Not necessary for all <see cref="InputDemandPairsType"/>s.</param>
        /// <param name="dpLengthDist">The length distribution for the demand pairs. Not necessary for all <see cref="InputDemandPairsType"/>s.</param>
        /// <returns>If the file with this instance exists: a tuple with the <see cref="Tree{N}"/>, a <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s and the optimal K value. Otherwise, a tuple with <see langword="null"/>, <see langword="null"/> and -1.</returns>
        public static (Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, int optimalK) ReadInstance(string instanceDirectoryPath, int randomSeed, InputTreeType inputTreeType, int numberOfNodes, string treeFileName, InputDemandPairsType inputDemandPairsType, int numberOfDPs, string dpFileName, string dpLengthDist)
        {
            string fileName = CreateFilePath(instanceDirectoryPath, randomSeed, inputTreeType, numberOfNodes, treeFileName,  inputDemandPairsType, numberOfDPs, dpFileName, dpLengthDist);

            if (File.Exists(fileName))
            {
                Tree<TreeNode> tree = new Tree<TreeNode>();
                CountedList<DemandPair> demandPairs = new CountedList<DemandPair>();
                int optimalK;

                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Skip the first line with a comment
                    sr.ReadLine();
                    string[] header = sr.ReadLine().Split();
                    numberOfNodes = int.Parse(header[0]);
                    numberOfDPs = int.Parse(header[1]);
                    optimalK = int.Parse(header[2]);

                    List<TreeNode> nodes = new List<TreeNode>();
                    for (uint i = 0; i < numberOfNodes; i++)
                    {
                        nodes.Add(new TreeNode(i));
                    }

                    sr.ReadLine();
                    TreeNode root = nodes[int.Parse(sr.ReadLine())];
                    tree.AddRoot(root, MockCounter);

                    // Reading edges, skip comment line
                    sr.ReadLine();
                    List<(TreeNode, TreeNode)> edges = new List<(TreeNode, TreeNode)>();
                    for (int i = 0; i < numberOfNodes - 1; i++)
                    {
                        string[] edge = sr.ReadLine().Split();
                        TreeNode endpoint1 = nodes[int.Parse(edge[0])];
                        TreeNode endpoint2 = nodes[int.Parse(edge[1])];
                        edges.Add((endpoint1, endpoint2));
                    }

                    // Actually add the edges to the tree
                    Queue<TreeNode> queue = new Queue<TreeNode>();
                    queue.Enqueue(root);
                    while (queue.Count > 0)
                    {
                        TreeNode node = queue.Dequeue();
                        IEnumerable<TreeNode> children = edges.Where(n => n.Item1 == node || n.Item2 == node).Select(n => n.Item1 == node ? n.Item2 : n.Item1);
                        edges = edges.Where(n => n.Item1 != node && n.Item2 != node).ToList();
                        tree.AddChildren(node, children, MockCounter);
                        foreach (TreeNode child in children)
                        {
                            queue.Enqueue(child);
                        }
                    }

                    tree.UpdateNodeTypes();

                    // Reading demand pairs, skip comment line
                    sr.ReadLine();
                    for (int i = 0; i < numberOfDPs; i++)
                    {
                        string[] dp = sr.ReadLine().Split();
                        TreeNode endpoint1 = nodes[int.Parse(dp[0])];
                        TreeNode endpoint2 = nodes[int.Parse(dp[1])];
                        demandPairs.Add(new DemandPair(endpoint1, endpoint2), MockCounter);
                    }
                }

                return (tree, demandPairs, optimalK);
            }
           
            // The file does not exist, return an empty instance so it can be created and written.
            return (null, null, -1);
        }

        /// <summary>
        /// Create the path of the file with the correct identifiers.
        /// </summary>
        /// <param name="instanceDirectoryPath">The path to the directory where instances are placed.</param>
        /// <param name="randomSeed">The seed used for random number generation.</param>
        /// <param name="inputTreeType">The <see cref="InputTreeType"/> used to generate the tree in the current instance.</param>
        /// <param name="numberOfNodes">The number of nodes in the tree in the current instance. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="treeFileName">The name of the file that contains the instance from which we created the current tree. Not necessary for all <see cref="InputTreeType"/>s.</param>
        /// <param name="inputDemandPairsType">The <see cref="InputDemandPairsType"/> used to generate the demand pairs in the current instance.</param>
        /// <param name="numberOfDPs">The number of demand pairs in the instance. Not necessary for all <see cref="InputDemandPairsType"/>s.</param>
        /// <param name="dpFileName">The name of the file that contains the fixed demand pair endpoints. Not necessary for all <see cref="InputDemandPairsType"/>s.</param>
        /// <param name="dpLengthDist">The length distribution for the demand pairs. Not necessary for all <see cref="InputDemandPairsType"/>s.</param>
        /// <returns>The path of the file for the instance with the given parameters.</returns>
        private static string CreateFilePath(string instanceDirectoryPath, int randomSeed, InputTreeType inputTreeType, int numberOfNodes, string treeFileName, InputDemandPairsType inputDemandPairsType, int numberOfDPs, string dpFileName, string dpLengthDist)
        {
            StringBuilder fileNameBuilder = new StringBuilder();
            fileNameBuilder.Append($"{instanceDirectoryPath}\\multicutinstance_seed={randomSeed}_treetype={inputTreeType}_dptype={inputDemandPairsType}");

            if (inputTreeType == InputTreeType.Prüfer || inputTreeType == InputTreeType.Caterpillar)
            {
                fileNameBuilder.Append($"_nrNodes={numberOfNodes}");
            }
            else
            {
                fileNameBuilder.Append($"_treeFile=({treeFileName})");
            }

            if (inputDemandPairsType == InputDemandPairsType.Random)
            {
                fileNameBuilder.Append($"_nrDPs={numberOfDPs}");
            }
            else if (inputDemandPairsType == InputDemandPairsType.Fixed)
            {
                fileNameBuilder.Append($"_dpFile=({dpFileName})");
            }
            else
            {
                fileNameBuilder.Append($"_dpLengthDist=({dpLengthDist})");
            }

            fileNameBuilder.Append(".txt");
            return fileNameBuilder.ToString();
        }
    }
}
