// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Implementation of Dinic's algorithm for maximum flow in a graph.
    /// </summary>
    public static class DinicMaxFlow
    {
        /// <summary>
        /// Compute the maximum flow in <paramref name="inputGraph"/> for multiple sources and sinks and unit capacities for each edge.
        /// </summary>
        /// <typeparam name="G">The type of graph used.</typeparam>
        /// <typeparam name="N">The type of nodes in the graph.</typeparam>
        /// <param name="inputGraph">The <typeparamref name="G"/> in which we want to compute the flow.</param>
        /// <param name="sources">The <see cref="IEnumerable{T}"/> with source <typeparamref name="N"/>s. If there can depart multiple units of flow from an <typeparamref name="N"/>, add it that many times to <paramref name="sources"/>.</param>
        /// <param name="sinks">The <see cref="IEnumerable{T}"/> with sink <typeparamref name="N"/>s. If there can arrive multiple units of flow to an <typeparamref name="N"/>, add it that many times to <paramref name="sinks"/>.</param>
        /// <returns>The maximum flow in <paramref name="inputGraph"/> between all <typeparamref name="N"/>s in <paramref name="sources"/> and <paramref name="sinks"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputGraph"/>, <paramref name="sinks"/> or <paramref name="sources"/> is <see langword="null"/>.</exception>
        public static int MaxFlowMultipleSourcesSinksUnitCapacities<G, N>(G inputGraph, IEnumerable<N> sources, IEnumerable<N> sinks) where G : IGraph<N> where N : INode<N>
        {
            if (inputGraph is null)
            {
                throw new ArgumentNullException(nameof(inputGraph), "Trying to compute multiple source, multiple sink flow with unit capacities, but the input graph is null!");
            }
            if (sources is null)
            {
                throw new ArgumentNullException(nameof(sources), "Trying to compute multiple source, multiple sink flow with unit capacities, but the IEnumerable with sources is null!");
            }
            if (sinks is null)
            {
                throw new ArgumentNullException(nameof(sinks), "Trying to compute multiple source, multiple sink flow with unit capacities, but the IEnumerable with sinks is null!");
            }
            
            // Create a capacity dictionary with capacity 1 for each edge.
            ReadOnlyCollection<(N, N)> edges = inputGraph.Edges;
            Dictionary<(uint, uint), int> capacities = new Dictionary<(uint, uint), int>();
            foreach ((N, N) edge in edges)
            {
                capacities.Add((edge.Item1.ID, edge.Item2.ID), 1);
                capacities.Add((edge.Item2.ID, edge.Item1.ID), 1);
            }

            // Compute the flow in the graph with the capacity dictionary.
            return MaxFlowMultipleSourcesSinks(inputGraph, sources, sinks, capacities);
        }

        /// <summary>
        /// Compute the maximum flow in <paramref name="inputGraph"/> for multiple sources and sinks and arbitrary capacities for each edge.
        /// </summary>
        /// <typeparam name="G">The type of graph used.</typeparam>
        /// <typeparam name="N">The type of nodes in the graph.</typeparam>
        /// <param name="inputGraph">The <typeparamref name="G"/> in which we want to compute the flow.</param>
        /// <param name="sources">The <see cref="IEnumerable{T}"/> with source <typeparamref name="N"/>s. If there can depart multiple units of flow from an <typeparamref name="N"/>, add it that many times to <paramref name="sources"/>.</param>
        /// <param name="sinks">The <see cref="IEnumerable{T}"/> with sink <typeparamref name="N"/>s. If there can arrive multiple units of flow to an <typeparamref name="N"/>, add it that many times to <paramref name="sinks"/>.</param>
        /// <param name="capacities">The <see cref="Dictionary{TKey, TValue}"/> with a tuple with <see cref="INode{N}.ID"/>s that define the edges, and an <see cref="int"/> that defines the capacity on this edge. Each capacity is directed.</param>
        /// <returns>The maximum flow in <paramref name="inputGraph"/> between all <typeparamref name="N"/>s in <paramref name="sources"/> and <paramref name="sinks"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputGraph"/>, <paramref name="sinks"/> or <paramref name="sources"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when there is an edge in <paramref name="inputGraph"/> that does not have a capacity in <paramref name="capacities"/>.</exception>
        public static int MaxFlowMultipleSourcesSinks<G, N>(G inputGraph, IEnumerable<N> sources, IEnumerable<N> sinks, Dictionary<(uint, uint), int> capacities) where G : IGraph<N> where N : INode<N>
        {
            if (inputGraph is null)
            {
                throw new ArgumentNullException(nameof(inputGraph), "Trying to compute multiple source, multiple sink flow with capacities, but the input graph is null!");
            }
            if (sources is null)
            {
                throw new ArgumentNullException(nameof(sources), "Trying to compute multiple source, multiple sink flow with capacities, but the IEnumerable with sources is null!");
            }
            if (sinks is null)
            {
                throw new ArgumentNullException(nameof(sinks), "Trying to compute multiple source, multiple sink flow with capacities, but the IEnumerable with sinks is null!");
            }
            if (capacities is null)
            {
                throw new ArgumentNullException(nameof(capacities), "Trying to compute multiple source, multiple sink flow with capacities, but the dictionary with capacities is null!");
            }

            if (sources.Count() == 1 && sinks.Count() == 1)
            {
                // It is actually single source, single sink flow, so compute that instead of adding an extra dummy source and sink to the graph.
                return MaxFlow(inputGraph, sources.ElementAt(0), sinks.ElementAt(0), capacities);
            }

            // Check if each edge has a capacity
            ReadOnlyCollection<(N, N)> edges = inputGraph.Edges;
            foreach ((N, N) edge in edges)
            {
                (uint, uint) e1 = (edge.Item1.ID, edge.Item2.ID);
                (uint, uint) e2 = (edge.Item2.ID, edge.Item1.ID);
                if (!capacities.ContainsKey(e1) || !capacities.ContainsKey(e2))
                {
                    throw new KeyNotFoundException($"Trying to compute multiple source, multiple sink flow with capacities, but there is no capacity for the edge between {edge.Item1} and {edge.Item2}!");
                }
            }

            Graph<Node> internalGraph = new Graph<Node>((IGraph<Node>)inputGraph);
            HashSet<N> startPoints = new HashSet<N>(sources);
            HashSet<N> endPoints = new HashSet<N>(sinks);             
            Dictionary<uint, int> startOccurrences = new Dictionary<uint, int>(startPoints.Select(i => new KeyValuePair<uint, int>(i.ID, 0)));
            Dictionary<uint, int> endOccurrences = new Dictionary<uint, int>(endPoints.Select(i => new KeyValuePair<uint, int>(i.ID, 0)));
            foreach (N s in sources)
            {
                startOccurrences[s.ID]++;
            }
            foreach (N t in sinks)
            {
                endOccurrences[t.ID]++;
            }

            // Create a dummy source node connected to all sources.
            Node source = new Node(uint.MaxValue);
            internalGraph.AddNode(source);
            foreach (Node node in (IEnumerable<Node>)startPoints)
            {
                internalGraph.AddEdge(source, node, false);
                capacities.Add((source.ID, node.ID), startOccurrences[node.ID]);
                capacities.Add((node.ID, source.ID), startOccurrences[node.ID]);
            }

            // Create a dummy sink node connected to all sinks.
            Node sink = new Node(uint.MaxValue - 1);
            internalGraph.AddNode(sink);
            foreach (Node node in (IEnumerable<Node>)endPoints)
            {
                internalGraph.AddEdge(sink, node, false);
                capacities.Add((sink.ID, node.ID), endOccurrences[node.ID]);
                capacities.Add((node.ID, sink.ID), endOccurrences[node.ID]);
            }

            // Compute the flow
            int flow = MaxFlow(internalGraph, source, sink, capacities);

            // Remove the dummy source and sink
            internalGraph.RemoveNode(source);
            internalGraph.RemoveNode(sink);

            return flow;
        }

        /// <summary>
        /// Compute the maximum flow in <paramref name="inputGraph"/> with a single source and sink and unit capacities for each edge.
        /// </summary>
        /// <typeparam name="G">The type of graph used.</typeparam>
        /// <typeparam name="N">The type of nodes in the graph.</typeparam>
        /// <param name="inputGraph">The <typeparamref name="G"/> in which we want to compute the flow.</param>
        /// <param name="source">The <typeparamref name="N"/> that is the source of the flow.</param>
        /// <param name="sink">The <typeparamref name="N"/> that is the sink of the flow.</param>
        /// <returns>The maximum flow in <paramref name="inputGraph"/> between <paramref name="source"/> and <paramref name="sink"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputGraph"/>, <paramref name="sink"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
        public static int MaxFlowUnitCapacities<G, N>(G inputGraph, N source, N sink) where G : IGraph<N> where N : INode<N>
        {
            if (inputGraph is null)
            {
                throw new ArgumentNullException(nameof(inputGraph), "Trying to compute single source, single sink flow with unit capacities, but the input graph is null!");
            }
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source), "Trying to compute single source, single sink flow with unit capacities, but the source node is null!");
            }
            if (sink is null)
            {
                throw new ArgumentNullException(nameof(sink), "Trying to compute single source, single sink flow with unit capacities, but the sink node is null!");
            }

            // Create a capacity dictionary with capacity 1 for each edge.
            ReadOnlyCollection<(N, N)> edges = inputGraph.Edges;
            Dictionary<(uint, uint), int> capacities = new Dictionary<(uint, uint), int>();
            foreach ((N, N) edge in edges)
            {
                capacities.Add((edge.Item1.ID, edge.Item2.ID), 1);
                capacities.Add((edge.Item2.ID, edge.Item1.ID), 1);
            }

            // Compute the flow in the graph with the capacity dictionary.
            return MaxFlow(inputGraph, source, sink, capacities);
        }

        /// <summary>
        /// Compute the maximum flow in <paramref name="inputGraph"/> with a single source and sink and arbitrary capacities for each edge.
        /// </summary>
        /// <typeparam name="G">The type of graph used.</typeparam>
        /// <typeparam name="N">The type of nodes in the graph.</typeparam>
        /// <param name="inputGraph">The <typeparamref name="G"/> in which we want to compute the flow.</param>
        /// <param name="source">The <typeparamref name="N"/> that is the source of the flow.</param>
        /// <param name="sink">The <typeparamref name="N"/> that is the sink of the flow.</param>
        /// <param name="capacities">The <see cref="Dictionary{TKey, TValue}"/> with a tuple with <see cref="INode{N}.ID"/>s that define the edges, and an <see cref="int"/> that defines the capacity on this edge. Each capacity is directed.</param>
        /// <returns>The maximum flow in <paramref name="inputGraph"/> between <paramref name="source"/> and <paramref name="sink"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputGraph"/>, <paramref name="sink"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when there is an edge in <paramref name="inputGraph"/> that does not have a capacity in <paramref name="capacities"/>.</exception>
        public static int MaxFlow<G, N>(G inputGraph, N source, N sink, Dictionary<(uint, uint), int> capacities) where G : IGraph<N> where N : INode<N>
        {
            if (inputGraph is null)
            {
                throw new ArgumentNullException(nameof(inputGraph), "Trying to compute single source, single sink flow with capacities, but the input graph is null!");
            }
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source), "Trying to compute single source, single sink flow with capacities, but the source node is null!");
            }
            if (sink is null)
            {
                throw new ArgumentNullException(nameof(sink), "Trying to compute single source, single sink flow with capacities, but the sink node is null!");
            }
            if (capacities is null)
            {
                throw new ArgumentNullException(nameof(sink), "Trying to compute single source, single sink flow with capacities, but the dictionary with capacities is null!");
            }

            if (source.Equals(sink))
            {
                // There can be no flow between a node and itself, so return 0.
                return 0;
            }

            // Check if each edge has a capacity
            ReadOnlyCollection<(N, N)> edges = inputGraph.Edges;
            foreach ((N, N) edge in edges)
            {
                (uint, uint) e1 = (edge.Item1.ID, edge.Item2.ID);
                (uint, uint) e2 = (edge.Item2.ID, edge.Item1.ID);
                if (!capacities.ContainsKey(e1) || !capacities.ContainsKey(e2))
                {
                    throw new KeyNotFoundException($"Trying to compute single source, single sink flow with capacities, but there is no capacity for the edge between {edge.Item1} and {edge.Item2}!");
                }
            }

            // Initialise the flow
            int maximumFlow = 0;
            Dictionary<(uint, uint), int> flow = new Dictionary<(uint, uint), int>();
            foreach ((N, N) edge in edges)
            {
                flow.Add((edge.Item1.ID, edge.Item2.ID), 0);
                flow.Add((edge.Item2.ID, edge.Item1.ID), 0);
            }

            // Compute the flow
            Dictionary<uint, int> levels;
            // While we can reach the sink
            while ((levels = FindLevels(inputGraph, source, flow, capacities))[sink.ID] != -1)
            {
                int extraFlow;
                // While there is extra flow possible
                while ((extraFlow = SendFlow(inputGraph, source, sink, int.MaxValue, flow, capacities, levels)) != 0)
                {
                    maximumFlow += extraFlow;
                }                
            }

            return maximumFlow;
        }

        /// <summary>
        /// Find the level of each <typeparamref name="N"/> in <typeparamref name="G"/>.
        /// </summary>
        /// <typeparam name="G">The type of graph used.</typeparam>
        /// <typeparam name="N">The type of nodes in the graph.</typeparam>
        /// <param name="inputGraph">The <typeparamref name="G"/> in which we are computing the flow.</param>
        /// <param name="source">The origin of the flow.</param>
        /// <param name="flow">The <see cref="Dictionary{TKey, TValue}"/> with the current flow per edge.</param>
        /// <param name="capacities">The <see cref="Dictionary{TKey, TValue}"/> with the capacity for each edge.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with the level for each <typeparamref name="N"/> from <paramref name="source"/>. The level is equal to -1 if no flow can be sent through this <typeparamref name="N"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputGraph"/>, <paramref name="source"/>, <paramref name="flow"/> or <paramref name="capacities"/> is <see langword="null"/>.</exception>
        private static Dictionary<uint, int> FindLevels<G, N>(G inputGraph, N source, Dictionary<(uint, uint), int> flow, Dictionary<(uint, uint), int> capacities) where G : IGraph<N> where N : INode<N>
        {
            if (inputGraph is null)
            {
                throw new ArgumentNullException(nameof(inputGraph), "Trying to send flow, but the input graph is null!");
            }
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source), "Trying to send flow, but the source node is null!");
            }
            if (flow is null)
            {
                throw new ArgumentNullException(nameof(flow), "Trying to send flow, but the dictionary with flow is null!");
            }
            if (capacities is null)
            {
                throw new ArgumentNullException(nameof(capacities), "Trying to send flow, but the dictionary with capacities is null!");
            }

            // Initialise each level to -1.
            Dictionary<uint, int> levels = new Dictionary<uint, int>();
            foreach (N node in inputGraph.Nodes)
            {
                levels.Add(node.ID, -1);
            }

            levels[source.ID] = 0;
            Queue<N> queue = new Queue<N>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                N node = queue.Dequeue();
                foreach (N child in node.Neighbours)
                {
                    // Update the level of this child if it does not have a level yet and we can send flow over the edge from the current node to this child.
                    if (levels[child.ID] < 0 && flow[(node.ID, child.ID)] < capacities[(node.ID, child.ID)])
                    {
                        levels[child.ID] = levels[node.ID] + 1;
                        queue.Enqueue(child);
                    }
                }
            }

            return levels;
        }

        /// <summary>
        /// Send as much flow as possible from <paramref name="node"/> to <paramref name="sink"/>.
        /// </summary>
        /// <typeparam name="G">The type of graph used.</typeparam>
        /// <typeparam name="N">The type of nodes in the graph.</typeparam>
        /// <param name="inputGraph">The <typeparamref name="G"/> in which we are computing the flow.</param>
        /// <param name="node">The current <typeparamref name="N"/>.</param>
        /// <param name="sink">The sink of the flow.</param>
        /// <param name="currentFlow">The current flow as found thus far.</param>
        /// <param name="flow">The <see cref="Dictionary{TKey, TValue}"/> with the current flow per edge.</param>
        /// <param name="capacities">The <see cref="Dictionary{TKey, TValue}"/> with the capacity for each edge.</param>
        /// <param name="levels">The <see cref="Dictionary{TKey, TValue}"/> with the level of each node.</param>
        /// <returns>The maximum extra flow we can send from <paramref name="node"/> to <paramref name="sink"/> considering current flow and edge capacities.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputGraph"/>, <paramref name="node"/>, <paramref name="sink"/>, <paramref name="flow"/>, <paramref name="capacities"/> or <paramref name="levels"/> is <see langword="null"/>.</exception>
        private static int SendFlow<G, N>(G inputGraph, N node, N sink, int currentFlow, Dictionary<(uint, uint), int> flow, Dictionary<(uint, uint), int> capacities, Dictionary<uint, int> levels) where G : IGraph<N> where N : INode<N>
        {
            if (inputGraph is null)
            {
                throw new ArgumentNullException(nameof(inputGraph), "Trying to send flow, but the input graph is null!");
            }
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node), "Trying to send flow, but the source node is null!");
            }
            if (sink is null)
            {
                throw new ArgumentNullException(nameof(sink), "Trying to send flow, but the sink is null!");
            }
            if (flow is null)
            {
                throw new ArgumentNullException(nameof(flow), "Trying to send flow, but the dictionary with flow is null!");
            }
            if (capacities is null)
            {
                throw new ArgumentNullException(nameof(capacities), "Trying to send flow, but the dictionary with capacities is null!");
            }
            if (levels is null)
            {
                throw new ArgumentNullException(nameof(levels), "Trying to send flow, but the dictionary with levels is null!");
            }

            // If we have reached the sink, we do not send any more flow, so return.
            if (node.Equals(sink))
            {
                return currentFlow;
            }

            foreach (N child in node.Neighbours)
            {
                if (levels[child.ID] == levels[node.ID] + 1 && flow[(node.ID, child.ID)] < capacities[(node.ID, child.ID)])
                {
                    int cFlow = Math.Min(currentFlow, capacities[(node.ID, child.ID)] - flow[(node.ID, child.ID)]);

                    // Compute the flow from this child to the sink recursively.
                    int tempFlow = SendFlow(inputGraph, child, sink, cFlow, flow, capacities, levels);

                    // If there is flow from this child to the sink, update it in the dictionary and return this flow.
                    if (tempFlow > 0)
                    {
                        flow[(node.ID, child.ID)] += tempFlow;
                        flow[(child.ID, node.ID)] -= tempFlow;
                        return tempFlow;
                    }
                }
            }

            // We were not able to send any more flow to the children.
            return 0;
        }
    }
}
