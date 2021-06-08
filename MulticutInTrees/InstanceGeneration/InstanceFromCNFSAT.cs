// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MulticutInTrees.CommandLineArguments;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// Class that creates a <see cref="MulticutInstance"/> from a file with a CNF-SAT instance.
    /// <br/>
    /// Based on the method in Theorem 11 in <see href="https://doi.org/10.1016/S0196-6774(03)00073-7"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class InstanceFromCNFSAT
    {
        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not affect performance.
        /// </summary>
        private readonly static Counter MockCounter = new();

        /// <summary>
        /// Create an instance for MulticutInTrees from a file with a CNF-SAT instance.
        /// </summary>
        /// <param name="options">The <see cref="CommandLineOptions"/> to use.</param>
        /// <returns>A tuple with a <see cref="Graph"/> (the Multicut tree), a <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s in the instance, and the optimal K value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public static (Graph tree, CountedCollection<DemandPair> demandPairs, int optimalK) GenerateInstance(CommandLineOptions options)
        {
#if !Experiment
            Utils.NullCheck(options, nameof(options), "Trying to create an instance from a Vertex Cover instance, but the commandline options are null!");
#endif
            (int numberOfVariables, List<List<int>> clauses) = ReadCNFSATInstance(options.InstanceFilePath, out Random random);
            uint nodeCounter = 0;
            uint dpCounter = 0;
            AddVariablesToGraph(numberOfVariables, out Graph graph, out CountedCollection<DemandPair> demandPairs, out Dictionary<int, Node> variableToNodeInTree, out Node root, random, ref nodeCounter, ref dpCounter);
            AddClausesToGraph(clauses, graph, root, demandPairs, variableToNodeInTree, random, ref nodeCounter, ref dpCounter);
            graph.UpdateNodeTypes();

            int optimalK = numberOfVariables - clauses.Count;
            foreach (List<int> clause in clauses)
            {
                optimalK += clause.Count;
            }

            return (graph, demandPairs, optimalK);
        }

        /// <summary>
        /// Read the CNF-SAT instance from its file.
        /// </summary>
        /// <param name="fileName">The path to the file with the CNF-SAT instance.</param>
        /// <param name="random">The <see cref="Random"/> used for shuffling. Is seeded with all digits in the file name, or 0 if there are none.</param>
        /// <returns>A tuple with the number of variables and a <see cref="List{T}"/> of clauses.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file with name <paramref name="fileName"/> cannot be found.</exception>
        /// <exception cref="BadFileFormatException">Thrown when a line in the file is not of correct format: e.g. a word cannot be converted to an integer.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when we do not have access to the file.</exception>
        private static (int numberOfVariables, List<List<int>> clauses) ReadCNFSATInstance(string fileName, out Random random)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"Trying to read a Vertex Cover instance from a file, but the file {fileName} does not exist!");
            }

            string digitsInFileName = "0";
            foreach (char c in fileName.Split('\\')[^1])
            {
                if (char.IsDigit(c))
                {
                    digitsInFileName += c;
                }
            }
            random = new Random(int.Parse(digitsInFileName));

            try
            {
                using StreamReader sr = new(fileName);
                string line = sr.ReadLine().Trim();
                while (line.StartsWith('c'))
                {
                    line = sr.ReadLine().Trim();
                }

                if (!line.StartsWith('p'))
                {
                    throw new BadFileFormatException($"The line {line} is not of a correct format. It should start with a \'p\'!");
                }
                string[] split = line.Split();
                int index = 2;
                while (split[index] == "")
                {
                    index++;
                }

                if (!int.TryParse(split[index], out int numberOfVariables))
                {
                    throw new BadFileFormatException($"The line {line} is not of a correct format. Its third word should be a number that represents the number of variables, but it cannot be converted to an integer.!");
                }

                do
                {
                    index++;
                } while (split[index] == "");

                if (!int.TryParse(split[index], out int numberOfClauses))
                {
                    throw new BadFileFormatException($"The line {line} is not of a correct format. Its fourth word should be a number that represents the number of clauses, but it cannot be converted to an integer.!");
                }

                List<List<int>> clauses = new();
                for (int i = 0; i < numberOfClauses; i++)
                {
                    line = sr.ReadLine().Trim();
                    split = line.Split();
                    List<int> clause = new();
                    for (int j = 0; j < split.Length - 1; j++)
                    {
                        if (split[j] == "")
                        {
                            continue;
                        }
                        if (!int.TryParse(split[j], out int var))
                        {
                            throw new BadFileFormatException($"The line {line} is not of a correct format. Its {j}-th word should be a number that represents a variable in a clause, but it cannot be converted to an integer.!");
                        }
                        clause.Add(var);
                    }

                    clauses.Add(clause);
                }

                return (numberOfVariables, clauses);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Trying to read a CNF-SAT instance from a file, but we cannot open the file {fileName}: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a <see cref="Graph"/> with a root, and the structure with variables. Also creates the <see cref="DemandPair"/>s between the variables themselves.
        /// </summary>
        /// <param name="numberOfVariables">The number of variables in the instance.</param>
        /// <param name="graph">The <see cref="Graph"/> for the Multicut instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s for the Multicut instance.</param>
        /// <param name="variableToNodeInTree"><see cref="Dictionary{TKey, TValue}"/> to go from a variable (or its negation) to the correct <see cref="Node"/> in <paramref name="graph"/>.</param>
        /// <param name="root">The root of <paramref name="graph"/>. The backbone of the clause structure should be attached to this <see cref="Node"/>.</param>
        /// <param name="random">The <see cref="Random"/> used for shuffling the backbone.</param>
        /// <param name="nodeCounter">Reference to the counter with <see cref="Node"/> numbers, so no two <see cref="Node"/>s with the same <see cref="AbstractNode{TNode}.ID"/> will exist in the <see cref="Graph"/>.</param>
        /// <param name="dpCounter">Reference to the counter with <see cref="DemandPair"/> numbers, so no two <see cref="DemandPair"/>s with the same <see cref="DemandPair.ID"/> will exist in the instance.</param>
        private static void AddVariablesToGraph(int numberOfVariables, out Graph graph, out CountedCollection<DemandPair> demandPairs, out Dictionary<int, Node> variableToNodeInTree, out Node root, Random random, ref uint nodeCounter, ref uint dpCounter)
        {
            graph = new();
            demandPairs = new();

            root = new(nodeCounter++);
            graph.AddNode(root, MockCounter);
            List<Node> backBoneNodesForVariables = CreateBackBoneStructure(numberOfVariables - 1, graph, root, ref nodeCounter);
            backBoneNodesForVariables.Shuffle(random);

            variableToNodeInTree = new();
            for (int i = 1; i <= numberOfVariables; i++)
            {
                Node parentNode = new(nodeCounter++);
                Node trueNode = new(nodeCounter++);
                Node falseNode = new(nodeCounter++);
                variableToNodeInTree[i] = trueNode;
                variableToNodeInTree[-i] = falseNode;
                graph.AddNode(parentNode, MockCounter);
                graph.AddNode(trueNode, MockCounter);
                graph.AddNode(falseNode, MockCounter);
                graph.AddEdge(new Edge<Node>(parentNode, trueNode), MockCounter);
                graph.AddEdge(new Edge<Node>(parentNode, falseNode), MockCounter);
                graph.AddEdge(new Edge<Node>(parentNode, backBoneNodesForVariables[i - 1]), MockCounter);
                DemandPair dp = new(dpCounter++, trueNode, falseNode, graph);
                demandPairs.Add(dp, MockCounter);
            }
        }

        /// <summary>
        /// Adds the structures for the clauses in the CNF-SAT instance to <paramref name="graph"/> and their <see cref="DemandPair"/>s to <paramref name="demandPairs"/>.
        /// </summary>
        /// <param name="clauses">The clauses in the CNF-SAT instance.</param>
        /// <param name="graph">The <see cref="Graph"/> for the Multicut instance.</param>
        /// <param name="root">The <see cref="Node"/> in <paramref name="graph"/> to which the clause structure should be attached.</param>
        /// <param name="demandPairs"><see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s in the Multicut instance.</param>
        /// <param name="variableToNodeInTree"><see cref="Dictionary{TKey, TValue}"/> from variable to its <see cref="Node"/> in <paramref name="graph"/>.</param>
        /// <param name="random"><see cref="Random"/> used for shuffling the backbone.</param>
        /// <param name="nodeCounter">Reference to the counter with <see cref="Node"/> numbers, so no two <see cref="Node"/>s with the same <see cref="AbstractNode{TNode}.ID"/> will exist in the <see cref="Graph"/>.</param>
        /// <param name="dpCounter">Reference to the counter with <see cref="DemandPair"/> numbers, so no two <see cref="DemandPair"/>s with the same <see cref="DemandPair.ID"/> will exist in the instance.</param>
        private static void AddClausesToGraph(List<List<int>> clauses, Graph graph, Node root, CountedCollection<DemandPair> demandPairs, Dictionary<int, Node> variableToNodeInTree, Random random, ref uint nodeCounter, ref uint dpCounter)
        {
            List<Node> backBoneNodesForClauses = CreateBackBoneStructure(clauses.Count - 1, graph, root, ref nodeCounter);
            backBoneNodesForClauses.Shuffle(random);
            for (int i = 0; i < clauses.Count; i++)
            {
                CreateClauseStructure(clauses[i], graph, backBoneNodesForClauses[i], demandPairs, variableToNodeInTree, random, ref nodeCounter, ref dpCounter);
            }
        }

        /// <summary>
        /// Create the structure for a single clause.
        /// </summary>
        /// <param name="clause">The clause to create the structure for.</param>
        /// <param name="graph">The <see cref="Graph"/> for the Multicut instance.</param>
        /// <param name="nodeToAttachTo">The <see cref="Node"/> to attach the clause to.</param>
        /// <param name="demandPairs"><see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s in the Multicut instance.</param>
        /// <param name="variableToNodeInTree"><see cref="Dictionary{TKey, TValue}"/> from variable to its <see cref="Node"/> in <paramref name="graph"/>.</param>
        /// <param name="random"><see cref="Random"/> used for shuffling the backbone.</param>
        /// <param name="nodeCounter">Reference to the counter with <see cref="Node"/> numbers, so no two <see cref="Node"/>s with the same <see cref="AbstractNode{TNode}.ID"/> will exist in the <see cref="Graph"/>.</param>
        /// <param name="dpCounter">Reference to the counter with <see cref="DemandPair"/> numbers, so no two <see cref="DemandPair"/>s with the same <see cref="DemandPair.ID"/> will exist in the instance.</param>
        private static void CreateClauseStructure(List<int> clause, Graph graph, Node nodeToAttachTo, CountedCollection<DemandPair> demandPairs, Dictionary<int, Node> variableToNodeInTree, Random random, ref uint nodeCounter, ref uint dpCounter)
        {
            clause.Shuffle(random);
            List<Node> backBone = CreateBackBoneStructure(clause.Count - 1, graph, nodeToAttachTo, ref nodeCounter);
            List<Node> lastTwoNodes = new();
            for (int i = 0; i < clause.Count; i++)
            {
                Node node = new(nodeCounter++);
                graph.AddNode(node, MockCounter);
                graph.AddEdge(new Edge<Node>(node, backBone[i]), MockCounter);
                demandPairs.Add(new DemandPair(dpCounter++, node, variableToNodeInTree[clause[i]], graph), MockCounter);

                if (i < clause.Count - 2)
                {
                    demandPairs.Add(new DemandPair(dpCounter++, node, backBone[i + 1], graph), MockCounter);
                }
                else
                {
                    lastTwoNodes.Add(node);
                }
            }

            demandPairs.Add(new DemandPair(dpCounter++, lastTwoNodes[0], lastTwoNodes[1], graph), MockCounter);
        }

        /// <summary>
        /// Create a path in the graph to attach either nodes, variable structures or clause structures to.
        /// </summary>
        /// <param name="length">The length of the path.</param>
        /// <param name="graph">The <see cref="Graph"/> for the Multicut instance.</param>
        /// <param name="nodeToAttachTo">The <see cref="Node"/> to attach the path to.</param>
        /// <param name="nodeCounter">Reference to the counter with <see cref="Node"/> numbers, so no two <see cref="Node"/>s with the same <see cref="AbstractNode{TNode}.ID"/> will exist in the <see cref="Graph"/>.</param>
        /// <returns>A <see cref="List{T}"/> with the created backbone. The <see cref="Node"/>s are shuffled, so iterating through them means walking over the backbone in a random order.</returns>
        private static List<Node> CreateBackBoneStructure(int length, Graph graph, Node nodeToAttachTo, ref uint nodeCounter)
        {
            Node last = nodeToAttachTo;
            List<Node> backBoneNodesForVariables = new();
            for (int i = 0; i < length; i++)
            {
                Node node = new(nodeCounter++);
                graph.AddNode(node, MockCounter);
                graph.AddEdge(new Edge<Node>(last, node), MockCounter);
                backBoneNodesForVariables.Add(node);
                last = node;
            }

            // The last node should have 2 variables connected to it, so we add it again.
            backBoneNodesForVariables.Add(last);
            return backBoneNodesForVariables;
        }
    }
}
