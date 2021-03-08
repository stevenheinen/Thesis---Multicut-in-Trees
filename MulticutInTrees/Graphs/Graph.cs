// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// A graph that implements <see cref="IGraph{N}"/> and consists of a class that implements <see cref="INode{T}"/>.
    /// </summary>
    public class Graph<N> : IGraph<N> where N : INode<N>
    {
        /// <summary>
        /// The internal <see cref="CountedCollection{T}"/> of edges in this <see cref="Graph{N}"/>.
        /// </summary>
        private CountedCollection<(N, N)> InternalEdges { get; }

        /// <summary>
        /// The internal <see cref="CountedCollection{T}"/> of nodes in this <see cref="Graph{N}"/>.
        /// </summary>
        private CountedCollection<N> InternalNodes { get; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not affect the performance of an <see cref="Algorithms.Algorithm"/> or <see cref="ReductionRules.ReductionRule"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for a <see cref="Graph{N}"/>.
        /// </summary>
        public Graph()
        {
            MockCounter = new Counter();
            InternalNodes = new CountedCollection<N>();
            InternalEdges = new CountedCollection<(N, N)>();
        }

        /// <summary>
        /// Constructor for a <see cref="Graph{N}"/> from any type that implements <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="interfaceGraph">The <see cref="IGraph{N}"/> to create this new <see cref="Graph{N}"/> from.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="interfaceGraph"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        internal Graph(IGraph<N> interfaceGraph, Counter counter) : this()
        {
#if !EXPERIMENT
            Utils.NullCheck(interfaceGraph, nameof(interfaceGraph), "Trying to create a Graph from another instance that implements IGraph, but the other instance is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to create a Graph from another instance that implements IGraph, but the counter is null!");
#endif
            InternalNodes = new CountedCollection<N>(interfaceGraph.Nodes(counter), counter);
            InternalEdges = new CountedCollection<(N, N)>(interfaceGraph.Edges(counter), counter);
        }

        /// <summary>
        /// The publically visible collection of edges in this <see cref="Graph{N}"/>. Edges cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddEdge(N, N, Counter, bool)"/>, <seealso cref="AddEdges(IEnumerable{ValueTuple{N, N}}, Counter, bool)"/>, <seealso cref="RemoveEdge(N, N, Counter, bool)"/>, <seealso cref="RemoveEdges(IList{ValueTuple{N, N}}, Counter, bool)"/> and <seealso cref="RemoveAllEdgesOfNode(N, Counter, bool)"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all edges in this <see cref="Graph{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<(N, N)> Edges(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the edges in a graph, but the counter is null!");
#endif
            _ = counter++;
            return new CountedEnumerable<(N, N)>(InternalEdges.GetLinkedList(), counter);
        }

        /// <summary>
        /// The publically visible collection of nodes in this <see cref="Graph{N}"/>. Nodes cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddNode(N, Counter)"/>, <seealso cref="AddNodes(IEnumerable{N}, Counter)"/>, <seealso cref="RemoveNode(N, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{N}, Counter)"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all nodes in this <see cref="Graph{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<N> Nodes(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the nodes in a graph, but the counter is null!");
#endif
            _ = counter++;
            return new CountedEnumerable<N>(InternalNodes.GetLinkedList(), counter);
        }

        /// <summary>
        /// The number of nodes in this <see cref="Graph{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that represents the number of nodes in this <see cref="Graph{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int NumberOfNodes(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the number of nodes in a graph, but the counter is null!");
#endif
            return InternalNodes.Count(counter);
        }

        /// <summary>
        /// The number of edges in this <see cref="Graph{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that represents the number of edges in this <see cref="Graph{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int NumberOfEdges(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the number of edges in a graph, but the counter is null!");
#endif
            return InternalEdges.Count(counter);
        }

        /// <summary>
        /// Finds whether <paramref name="node"/> is part of this <see cref="Graph{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is part of this <see cref="Graph{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="Graph{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool HasNode(N node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to see if a node is in {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to see if a node is in {this}, but the counter is null!");
#endif
            return InternalNodes.Contains(node, counter);
        }

        /// <summary>
        /// Finds whether the edge between parameters <paramref name="origin"/> and <paramref name="directed"/> is part of this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <returns><see langword="true"/> if the edge between <paramref name="origin"/> and <paramref name="destination"/> exists in this <see cref="Graph{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="origin"/>, <paramref name="destination"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either <paramref name="origin"/> or <paramref name="destination"/> is not part of this <see cref="Graph{N}"/>.</exception>
        public bool HasEdge(N origin, N destination, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utils.NullCheck(origin, nameof(origin), $"Trying to find out whether an edge exists in {this}, but the origin of the edge is null!");
            Utils.NullCheck(destination, nameof(destination), $"Trying to find out whether an edge exists in {this}, but the destination of the edge is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to find out whether an edge exists in {this}, but the counter is null!");
            if (!HasNode(origin, MockCounter))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the origin of the edge is not part of {this}!");
            }
            if (!HasNode(destination, MockCounter))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the destination of the edge is not part of {this}!");
            }
#endif
            if (directed)
            {
                return InternalEdges.Contains((origin, destination), counter);
            }

            // One of these uses the MockCounter to ensure this contains only counts as a single operation.
            return InternalEdges.Contains((origin, destination), counter) || InternalEdges.Contains((destination, origin), MockCounter);
        }

        /// <summary>
        /// Finds whether the edge <paramref name="edge"/> exists in this <see cref="Graph{N}"/>.
        /// </summary>
        /// <param name="edge">The tuple of two <typeparamref name="N"/>s for which we want to know if it is part of this <see cref="Graph{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed">Optional. If <see langword="true"/>, only the edge from the first to the second endpoint of <paramref name="edge"/> is checked. If <see langword="false"/>, also the inverse edge is checked.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> exists in this <see cref="Graph{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasEdge((N, N) edge, Counter counter, bool directed = false)
        {
            return HasEdge(edge.Item1, edge.Item2, counter, directed);
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="Graph{N}"/>.
        /// Looks like "Graph with n nodes and m edges", where "n" and "m" are numbers.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Graph{N}"/>.</returns>
        public override string ToString()
        {
            return $"Graph with {NumberOfNodes(MockCounter)} nodes and {NumberOfEdges(MockCounter)} edges.";
        }

        /// <summary>
        /// Add a new <typeparamref name="N"/> <paramref name="node"/> to this <see cref="Graph{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="AlreadyInGraphException">Thrown when <paramref name="node"/> is already part of this <see cref="Graph{N}"/>.</exception>
        public void AddNode(N node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to add a node to {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add a node to {this}, but the counter is null!");
            if (HasNode(node, MockCounter))
            {
                throw new AlreadyInGraphException($"Trying to add {node} to {this}, but {node} is already part of {this}!");
            }
#endif
            InternalNodes.Add(node, counter);
        }

        /// <summary>
        /// Add multiple new <typeparamref name="N"/>s to this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> is <see langword="null"/>.</exception>
        public void AddNodes(IEnumerable<N> nodes, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(nodes, nameof(nodes), $"Trying to add an IEnumerable of nodes to {this}, but the IEnumerable is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add an IEnumerable of nodes to {this}, but the counter is null!");
#endif
            foreach (N node in nodes)
            {
                AddNode(node, counter);
            }
        }

        /// <summary>
        /// Add an edge between <paramref name="origin"/> and <paramref name="destination"/> to this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="origin"/>, <paramref name="destination"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either <paramref name="origin"/> or <paramref name="destination"/> is not part of this <see cref="Graph{N}"/>.</exception>
        /// <exception cref="AlreadyInGraphException">Thrown when the edge to be added is already part of this <see cref="Graph{N}"/>.</exception>
        public void AddEdge(N origin, N destination, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utils.NullCheck(origin, nameof(origin), $"Trying to add an edge to {this}, but the origin of the edge is null!");
            Utils.NullCheck(destination, nameof(destination), $"Trying to add an edge to {this}, but the destination of the edge is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add an edge to {this}, but the counter is null!");
            if (!HasNode(origin, MockCounter))
            {
                throw new NotInGraphException($"Trying to add an edge between {origin} and {destination} to {this}, but {origin} is not part of {this}!");
            }
            if (!HasNode(destination, MockCounter))
            {
                throw new NotInGraphException($"Trying to add an edge between {origin} and {destination} to {this}, but {destination} is not part of {this}!");
            }
#endif
            (N, N) edge1 = (origin, destination);
#if !EXPERIMENT
            if (HasEdge(edge1, MockCounter, true) || (!directed && HasEdge((destination, origin), MockCounter)))
            {
                throw new AlreadyInGraphException($"Trying to add an edge between {origin} and {destination} to {this}, but this edge is already part of {this}!");
            }
#endif
            origin.AddNeighbour(destination, counter, directed);
            InternalEdges.Add(edge1, counter);
        }

        /// <summary>
        /// Add multiple edges to this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void AddEdges(IEnumerable<(N, N)> edges, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), $"Trying to add an IEnumerable of edges to {this}, but the IEnumerable is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add an IEnumerable of edges to {this}, but the counter is null!");
#endif
            foreach ((N origin, N destination) in edges)
            {
                AddEdge(origin, destination, counter, directed);
            }
        }

        /// <summary>
        /// Remove an <typeparamref name="N"/> from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="Graph{N}"/>.</exception>
        public void RemoveNode(N node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to remove a node from {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove a node from {this}, but the counter is null!");
            if (!HasNode(node, MockCounter))
            {
                throw new NotInGraphException($"Trying to remove {node} from {this}, but {node} is not part of {this}!");
            }
#endif
            RemoveAllEdgesOfNode(node, counter);
            InternalNodes.Remove(node, counter);
        }

        /// <summary>
        /// Remove multiple <typeparamref name="N"/>s from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveNodes(IEnumerable<N> nodes, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(nodes, nameof(nodes), $"Trying to remove multiple nodes from {this}, but the IEnumerable with nodes is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove multiple nodes from {this}, but the counter is null!");
#endif
            foreach (N node in nodes)
            {
                RemoveNode(node, counter);
            }
        }

        /// <summary>
        /// Removes all edges connected to a given <typeparamref name="N"/> from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="Graph{N}"/>.</exception>
        public void RemoveAllEdgesOfNode(N node, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to remove all edges of a node in {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove all edges of a node in {this}, but the counter is null!");
            if (!HasNode(node, MockCounter))
            {
                throw new NotInGraphException($"Trying to remove all edges of {node} from {this}, but {node} is not part of {this}!");
            }
#endif
            RemoveEdges(node.Neighbours(counter).Select(neighbour => (node, neighbour)).ToList(), counter, directed);
        }

        /// <summary>
        /// Remove multiple edges from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveEdges(IList<(N, N)> edges, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), $"Trying to remove multiple edges from {this}, but the IEnumerable with edges is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove multiple edges from {this}, but the counter is null!");
#endif
            foreach ((N origin, N destination) in edges)
            {
                RemoveEdge(origin, destination, counter, directed);
            }
        }

        /// <summary>
        /// Remove an edge from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="origin"/>, <paramref name="destination"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="origin"/>, <paramref name="destination"/> or the edge between them is not part of this <see cref="Graph{N}"/>.</exception>
        public void RemoveEdge(N origin, N destination, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utils.NullCheck(origin, nameof(origin), $"Trying to remove the edge from {this}, but the origin of the edge is null!");
            Utils.NullCheck(destination, nameof(destination), $"Trying to remove the edge from {this}, but the destination of the edge is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove the edge from {this}, but the counter is null!");
            if (!HasNode(origin, MockCounter))
            {
                throw new NotInGraphException($"Trying to remove the edge between {origin} and {destination} from {this}, but {origin} is not part of {this}!");
            }
            if (!HasNode(destination, MockCounter))
            {
                throw new NotInGraphException($"Trying to remove the edge between {origin} and {destination} from {this}, but {destination} is not part of {this}!");
            }
#endif
            (N, N) edge1 = (origin, destination);
            (N, N) edge2 = (destination, origin);
#if !EXPERIMENT
            if ((directed && !HasEdge(edge1, MockCounter, true)) || ((!HasEdge(edge1, MockCounter, directed)) && (!HasEdge(edge2, MockCounter, directed))))
            {
                throw new NotInGraphException($"Trying to remove the edge between {origin} and {destination} from {this}, but this edge is not part of {this}!");
            }
#endif
            origin.RemoveNeighbour(destination, counter, directed);
            if (HasEdge(edge1, MockCounter, true))
            {
                InternalEdges.Remove(edge1, counter);
                return;
            }

            if (!directed)
            {
                if (HasEdge(edge2, MockCounter, true))
                {
                    InternalEdges.Remove(edge2, counter);
                }
            }
        }

        /// <summary>
        /// Checks whether this <see cref="Graph{N}"/> is acyclic.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used during this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="Graph{N}"/> is acyclic, <see langword="false"/> if it is cyclic.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool IsAcyclic(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to find out whether a graph is acyclic, but the counter is null!");
#endif
            return DFS.IsAcyclicGraph<Graph<N>, N>(this, counter);
        }

        /// <summary>
        /// Checks whether this <see cref="Graph{N}"/> is connected.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used during this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="Graph{N}"/> is connected, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool IsConnected(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to find out whether a graph is connected, but the counter is null!");
#endif
            return DFS.FindAllConnectedComponents(Nodes(counter), counter).Count == 1;
        }
    }
}
