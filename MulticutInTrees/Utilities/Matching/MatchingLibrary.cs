// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities.Matching
{
    /// <summary>
    /// Uses a slightly modified implementation of the Blossom algorithm for graph matching.
    /// <br/>
    /// <b>NOTE:</b> Only works on undirected graphs!
    /// <br/>
    /// Source: <see href="https://github.com/dilsonpereira/Minimum-Cost-Perfect-Matching"/>
    /// </summary>
    internal static class MatchingLibrary
    {
        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not affect performance.
        /// </summary>
        private readonly static Counter MockCounter = new();

        /// <summary>
        /// The name of the file that is used as input for the matching library.
        /// </summary>
        private readonly static string FileName = $"{AppDomain.CurrentDomain.BaseDirectory}Utilities\\Matching\\MatchingGraph.txt";

        /// <summary>
        /// Uses a matching algorithm to find whether <paramref name="graph"/> has a matching with a size of at least <paramref name="requiredSize"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph used.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> to find the matching in.</param>
        /// <param name="requiredSize">The required size of the matching.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> used for graph operations.</param>
        /// <returns><see langword="true"/> if the maximum matching in <paramref name="graph"/> has size at least <paramref name="requiredSize"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graph"/> is <see langword="null"/>.</exception>
        public static bool HasMatchingOfSize<TGraph, TEdge, TNode>(TGraph graph, int requiredSize, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find a matching in a graph, but the graph is null!");
#endif
            if (graph.NumberOfEdges(graphCounter) < requiredSize)
            {
                return false;
            }

            if (graph.IsAcyclic(MockCounter))
            {
                return EdmondsMatching.HasMatchingOfAtLeast<TGraph, TEdge, TNode>(graph, requiredSize);
            }

            string output = RunMatchingLibrary<TGraph, TEdge, TNode>(graph, graphCounter, out Dictionary<(int, int), TEdge> _);
            int size = ParseMatchingSize(output);
            return size >= requiredSize;
        }

        /// <summary>
        /// Uses a matching algorithm to find the maximum matching in <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph used.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> to find the matching in.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> used for graph operations.</param>
        /// <returns>The maximum matching in <paramref name="graph"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="Exception">Thrown if the matching output cannot be parsed to the matching.</exception>
        public static IEnumerable<TEdge> FindMatching<TGraph, TEdge, TNode>(TGraph graph, Counter graphCounter) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(graph, nameof(graph), "Trying to find a matching in a graph, but the graph is null!");
#endif
            string output = RunMatchingLibrary<TGraph, TEdge, TNode>(graph, graphCounter, out Dictionary<(int, int), TEdge> libEdgeToOrigEdge);

            try
            {
                List<TEdge> result = ParseMatchingEdges<TEdge, TNode>(output, libEdgeToOrigEdge);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception($"Cannot parse the output of the matching library to the edges in the matching!", e);
            }
        }

        /// <summary>
        /// Run the NetworkX maximum cardinality matching algorithm on <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph used.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> to find the matching in.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> used for graph operations.</param>
        /// <param name="libEdgeToOrigEdge"><see cref="Dictionary{TKey, TValue}"/> with information from node ID tuple to the original <typeparamref name="TEdge"/> itself.</param>
        /// <returns>The output of the library.</returns>
        /// <exception cref="Exception">Thrown when the library threw an exception.</exception>
        private static string RunMatchingLibrary<TGraph, TEdge, TNode>(TGraph graph, Counter graphCounter, out Dictionary<(int, int), TEdge> libEdgeToOrigEdge) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
            CreateInputFile<TGraph, TEdge, TNode>(graph, graphCounter, out libEdgeToOrigEdge);

            StringBuilder errorBuilder = new();
            StringBuilder outputBuilder = new();

            Process p = new();
            p.EnableRaisingEvents = true;
            p.StartInfo = new ProcessStartInfo($"{AppDomain.CurrentDomain.BaseDirectory}Utilities\\Matching\\Min-Cost-Perfect-Matching.exe", $"-f \"{FileName}\" --max")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            p.OutputDataReceived += (sender, e) => outputBuilder.Append(e.Data + '\n');
            p.ErrorDataReceived += (sender, e) => errorBuilder.Append(e.Data + '\n');

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();

            string error = errorBuilder.ToString();
            string output = outputBuilder.ToString();

            if (error != "" && error != "\n")
            {
                throw new Exception($"The matching library threw an error! Error: {error}");
            }

            File.Delete(FileName);
            return output;
        }

        /// <summary>
        /// Parses the matching output to the edges.
        /// </summary>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="matching">The output of the matching library.</param>
        /// <param name="libEdgeToOrigEdge"><see cref="Dictionary{TKey, TValue}"/> with information from node ID tuple to the original <typeparamref name="TEdge"/> itself.</param>
        /// <returns>A <see cref="List{T}"/> with <typeparamref name="TEdge"/>s that represent the <typeparamref name="TEdge"/>s in the matching.</returns>
        /// <exception cref="Exception">Thrown when a line of the output cannot be parsed to an edge.</exception>
        private static List<TEdge> ParseMatchingEdges<TEdge, TNode>(string matching, Dictionary<(int, int), TEdge> libEdgeToOrigEdge) where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
            List<TEdge> result = new();
            string[] split = matching.Split('\n');
            split = split.Where(e => e != "").ToArray();
            for (int i = 1; i < split.Length; i++)
            {
                string[] line = split[i].Split();
                if (!int.TryParse(line[0], out int u) || !int.TryParse(line[1], out int v))
                {
                    throw new Exception($"The output of the matching library cannot be parsed to an edge! Line with error: {split[i]}\nEntire output: {matching}");
                }
                (int, int) tup = u <= v ? (u, v) : (v, u);
                result.Add(libEdgeToOrigEdge[tup]);
            }

            return result;
        }

        /// <summary>
        /// Parses the matching output to the size of the matching.
        /// </summary>
        /// <param name="matching">The output of the matching library.</param>
        /// <returns>The size of the matching</returns>
        /// <exception cref="Exception">Thrown when the first line of the output cannot be parsed to the matching size.</exception>
        private static int ParseMatchingSize(string matching)
        {
            if (!int.TryParse(matching.Split('\n')[0], out int size))
            {
                throw new Exception($"The first line of the output of the matching library cannot be parsed to an int! Output: {matching}");
            }
            return size;
        }

        /// <summary>
        /// Creates the file that will be used as input in the matching library.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph used.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> to find the matching in.</param>
        /// <param name="graphCounter">The <see cref="Counter"/> used for graph operations.</param>
        /// <param name="libEdgeToOrigEdge"><see cref="Dictionary{TKey, TValue}"/> with information from node ID tuple to the original <typeparamref name="TEdge"/> itself.</param>
        private static void CreateInputFile<TGraph, TEdge, TNode>(TGraph graph, Counter graphCounter, out Dictionary<(int, int), TEdge> libEdgeToOrigEdge) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
            libEdgeToOrigEdge = new();
            Dictionary<TNode, int> origNodeToLibNode = new();

            int n = graph.NumberOfNodes(graphCounter);
            int m = graph.NumberOfEdges(graphCounter);

            StringBuilder sb = new();
            sb.Append($"{n}\n");
            sb.Append($"{m}\n");

            int key = 0;
            foreach (TNode node in graph.Nodes(graphCounter))
            {
                origNodeToLibNode[node] = key;
                key++;
            }

            foreach (TEdge edge in graph.Edges(graphCounter))
            {
                int u = origNodeToLibNode[edge.Endpoint1];
                int v = origNodeToLibNode[edge.Endpoint2];
                (int, int) tup = u <= v ? (u, v) : (v, u);
                libEdgeToOrigEdge[tup] = edge;
                sb.Append($"{u} {v}\n");
            }

            using StreamWriter sw = new(FileName);
            sw.Write(sb.ToString());
        }
    }
}
