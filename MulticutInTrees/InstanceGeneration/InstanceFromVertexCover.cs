// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that creates a <see cref="MulticutInstance"/> from a file with a Vertex Cover instance.
    /// <br/>
    /// Based on the method in Proposition 3.1 in <see href="https://link.springer.com/article/10.1007/BF02523685"/>.
    /// </summary>
    public static class InstanceFromVertexCover
    {
        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not affect performance.
        /// </summary>
        private readonly static Counter MockCounter = new();

        /// <summary>
        /// Create an instance for MulticutInTrees from a file with a Vertex Cover instance.
        /// </summary>
        /// <param name="options">The <see cref="CommandLineOptions"/> to use.</param>
        /// <returns>A tuple with a <see cref="Graph"/> (the Multicut tree), a <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s in the instance, and the optimal K value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public static (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) GenerateInstance(CommandLineOptions options)
        {
#if !Experiment
            Utilities.Utils.NullCheck(options, nameof(options), "Trying to create an instance from a Vertex Cover instance, but the commandline options are null!");
#endif
            Graph vertexCoverGraph = ReadVertexCoverGraph(options.InstanceFilePath, out int optimalK);
            Graph multicutTree = CreateMulticutTree(vertexCoverGraph.Nodes(MockCounter), vertexCoverGraph.NumberOfNodes(MockCounter), out Node[] nodesInTree);
            CountedCollection<DemandPair> demandPairs = CreateDemandPairs(multicutTree, nodesInTree, vertexCoverGraph.Edges(MockCounter));
            return (multicutTree, demandPairs, optimalK);
        }

        /// <summary>
        /// Read a Vertex Cover instance from a file.
        /// </summary>
        /// <param name="fileName">The path to the file with the Vertex Cover instance.</param>
        /// <param name="optimalK">The optimal K value for this instance. Will be read from the file.</param>
        /// <returns>The Vertex Cover <see cref="Graph"/>.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file with name <paramref name="fileName"/> cannot be found.</exception>
        /// <exception cref="BadFileFormatException">Thrown when a line in the file is not of correct format: e.g. does not start with the correct character, or a word cannot be converted to an integer.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when we do not have access to the file.</exception>
        private static Graph ReadVertexCoverGraph(string fileName, out int optimalK)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"Trying to read a Vertex Cover instance from a file, but the file {fileName} does not exist!");
            }

            try
            {
                using StreamReader sr = new(fileName);
                string line = sr.ReadLine().Trim().Trim();
                while (line.StartsWith('c'))
                {
                    line = sr.ReadLine().Trim();
                }

                if (!line.StartsWith('p'))
                {
                    throw new BadFileFormatException($"The line {line} is not of a correct format. It should start with a \'p\'!");
                }
                string[] split = line.Split();

                if (!int.TryParse(split[2], out int numberOfNodes))
                {
                    throw new BadFileFormatException($"The line {line} is not of a correct format. Its third word should be a number that represents the number of nodes, but it cannot be converted to an integer.!");
                }
                if (!int.TryParse(split[3], out int numberOfEdges))
                {
                    throw new BadFileFormatException($"The line {line} is not of a correct format. Its fourth word should be a number that represents the number of edges, but it cannot be converted to an integer.!");
                }
                if (!int.TryParse(split[4], out optimalK))
                {
                    throw new BadFileFormatException($"The line {line} is not of a correct format. Its fifth word should be a number that represents the minimal Vertex Cover size, but it cannot be converted to an integer.!");
                }

                Graph graph = new();
                Node[] nodes = new Node[numberOfNodes + 1];
                for (uint i = 1; i <= numberOfNodes; i++)
                {
                    Node node = new(i);
                    nodes[i] = node;
                    graph.AddNode(node, MockCounter);
                }

                for (uint i = 0; i < numberOfEdges; i++)
                {
                    line = sr.ReadLine().Trim();
                    if (!line.StartsWith('e'))
                    {
                        throw new BadFileFormatException($"The line {line} is not of a correct format. It should start with a \'e\'!");
                    }

                    split = line.Split();
                    if (!int.TryParse(split[1], out int endpoint1))
                    {
                        throw new BadFileFormatException($"The line {line} is not of a correct format. Its second word should be a number that represents the endpoint of an edge, but it cannot be converted to an integer.!");
                    }
                    if (!int.TryParse(split[2], out int endpoint2))
                    {
                        throw new BadFileFormatException($"The line {line} is not of a correct format. Its third word should be a number that represents the endpoint of an edge, but it cannot be converted to an integer.!");
                    }

                    Edge<Node> edge = new(nodes[endpoint1], nodes[endpoint2]);
                    graph.AddEdge(edge, MockCounter);
                }

                return graph;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Trying to read a Vertex Cover instance from a file, but we cannot open the file {fileName}: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create the <see cref="Graph"/> (star-graph) for the Multicut instance.
        /// </summary>
        /// <param name="nodesInVertexCoverGraph"><see cref="Node"/>s in the Vertex Cover instance.</param>
        /// <param name="numberOfNodes">The number of nodes in the Vertex Cover instance.</param>
        /// <param name="nodesInTree">Array with <see cref="Node"/>s in the Multicut tree, indexed on their <see cref="AbstractNode{TNode}.ID"/>.</param>
        /// <returns>A <see cref="Graph"/> that is the Multicut tree/star-graph.</returns>
        private static Graph CreateMulticutTree(IEnumerable<Node> nodesInVertexCoverGraph, int numberOfNodes, out Node[] nodesInTree)
        {
            Graph graph = new();
            nodesInTree = new Node[numberOfNodes + 1];
            Node center = new(0);
            nodesInTree[0] = center;
            
            foreach (Node node in nodesInVertexCoverGraph)
            {
                Node treeNode = new(node.ID);
                nodesInTree[node.ID] = treeNode;
            }

            graph.AddNodes(nodesInTree, MockCounter);
            graph.AddEdges(nodesInTree.Skip(1).Select(n => new Edge<Node>(n, center)), MockCounter);
            graph.UpdateNodeTypes();
            return graph;
        }

        /// <summary>
        /// Create the <see cref="DemandPair"/>s for the multicut instance.
        /// </summary>
        /// <param name="multicutTree">The <see cref="Graph"/> with the Multicut tree.</param>
        /// <param name="nodesInTree">Array with <see cref="Node"/>s in the <paramref name="multicutTree"/>, indexed by their <see cref="AbstractNode{TNode}.ID"/>.</param>
        /// <param name="edgesInVertexCoverGraph">The <see cref="Edge{TNode}"/>s in the Vertex Cover instance, to be converted into <see cref="DemandPair"/>s.</param>
        /// <returns>A <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s in the Multicut instance.</returns>
        private static CountedCollection<DemandPair> CreateDemandPairs(Graph multicutTree, Node[] nodesInTree, IEnumerable<Edge<Node>> edgesInVertexCoverGraph)
        {
            CountedCollection<DemandPair> demandPairs = new();
            uint index = 0;
            foreach (Edge<Node> edge in edgesInVertexCoverGraph)
            {
                DemandPair dp = new(index++, nodesInTree[edge.Endpoint1.ID], nodesInTree[edge.Endpoint2.ID], multicutTree);
                demandPairs.Add(dp, MockCounter);
            }

            return demandPairs;
        }
    }
}
