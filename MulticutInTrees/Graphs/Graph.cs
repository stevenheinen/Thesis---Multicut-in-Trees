// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        /// The internal <see cref="List{T}"/> of edges in this <see cref="Graph{N}"/>.
        /// </summary>
        protected List<(N, N)> InternalEdges { get; set; }

        /// <summary>
        /// The internal <see cref="HashSet{T}"/> of edges in this <see cref="Graph{N}"/>.
        /// </summary>
        protected HashSet<(N, N)> UniqueInternalEdges { get; set; }

        /// <summary>
        /// The internal <see cref="HashSet{T}"/> of nodes in this <see cref="Graph{N}"/>
        /// </summary>
        protected HashSet<N> UniqueInternalNodes { get; set; }
       
        /// <summary>
        /// The internal <see cref="List{T}"/> of nodes in this <see cref="Graph{N}"/>.
        /// </summary>
        protected List<N> InternalNodes { get; set; }

        /// <summary>
        /// The number of nodes in this <see cref="Graph{N}"/>.
        /// </summary>
        public int NumberOfNodes => InternalNodes.Count;

        /// <summary>
        /// The number of edges in this <see cref="Graph{N}"/>.
        /// </summary>
        public int NumberOfEdges => InternalEdges.Count;

        /// <summary>
        /// The publically visible collection of edges in this <see cref="Graph{N}"/>. Edges cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddEdge(N, N, bool)"/>, <seealso cref="AddEdges(IEnumerable{ValueTuple{N, N}}, bool)"/>, <seealso cref="RemoveEdge(N, N, bool)"/>, <seealso cref="RemoveEdges(IList{ValueTuple{N, N}}, bool)"/> and <seealso cref="RemoveAllEdgesOfNode(N, bool)"/>.
        /// </summary>
        public ReadOnlyCollection<(N, N)> Edges => InternalEdges.AsReadOnly();

        /// <summary>
        /// The publically visible collection of nodes in this <see cref="Graph{N}"/>. Nodes cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddNode(N)"/>, <seealso cref="AddNodes(IEnumerable{N})"/>, <seealso cref="RemoveNode(N)"/> and <seealso cref="RemoveNodes(IEnumerable{N})"/>.
        /// </summary>
        public ReadOnlyCollection<N> Nodes => InternalNodes.AsReadOnly();

        /// <summary>
        /// Constructor for a <see cref="Graph{N}"/>.
        /// </summary>
        public Graph()
        {
            InternalNodes = new List<N>();
            UniqueInternalNodes = new HashSet<N>();
            InternalEdges = new List<(N, N)>();
            UniqueInternalEdges = new HashSet<(N, N)>();
        }

        /// <summary>
        /// Constructor for a <see cref="Graph{N}"/> from any type that implements <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="interfaceGraph">The <see cref="IGraph{N}"/> to create this new <see cref="Graph{N}"/> from.</param>
        public Graph(IGraph<N> interfaceGraph)
        {
            InternalNodes = new List<N>(interfaceGraph.Nodes);
            UniqueInternalNodes = new HashSet<N>(InternalNodes);
            InternalEdges = new List<(N, N)>(interfaceGraph.Edges);
            UniqueInternalEdges = new HashSet<(N, N)>(InternalEdges);
        }

        /// <summary>
        /// Finds whether <paramref name="node"/> is part of this <see cref="Graph{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is part of this <see cref="Graph{N}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="Graph{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        public bool HasNode(N node)
        {
            Utils.NullCheck(node, nameof(node), $"Trying to see if a node is in {this}, but the node is null!");

            return UniqueInternalNodes.Contains(node);
        }

        /// <summary>
        /// Finds whether the edge between parameters <paramref name="origin"/> and <paramref name="directed"/> is part of this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <returns><see langword="true"/> if the edge between <paramref name="origin"/> and <paramref name="destination"/> exists in this <see cref="Graph{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="origin"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either <paramref name="origin"/> or <paramref name="destination"/> is not part of this <see cref="Graph{N}"/>.</exception>
        public bool HasEdge(N origin, N destination, bool directed = false)
        {
            Utils.NullCheck(origin, nameof(origin), $"Trying to find out whether an edge exists in {this}, but the origin of the edge is null!");
            Utils.NullCheck(destination, nameof(destination), $"Trying to find out whether an edge exists in {this}, but the destination of the edge is null!");
            if (!HasNode(origin))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the origin of the edge is not part of {this}!");
            }
            if (!HasNode(destination))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the destination of the edge is not part of {this}!");
            }

            if (directed) 
            {
                return UniqueInternalEdges.Contains((origin, destination));
            }

            return UniqueInternalEdges.Contains((origin, destination)) || UniqueInternalEdges.Contains((destination, origin));
        }

        /// <summary>
        /// Finds whether the edge <paramref name="edge"/> exists in this <see cref="Graph{N}"/>.
        /// </summary>
        /// <param name="edge">The tuple of two <typeparamref name="N"/>s for which we want to know if it is part of this <see cref="Graph{N}"/>.</param>
        /// <param name="directed">Optional. If <see langword="true"/>, only the edge from the first to the second endpoint of <paramref name="edge"/> is checked. If <see langword="false"/>, also the inverse edge is checked.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> exists in this <see cref="Graph{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasEdge((N, N) edge, bool directed = false)
        {
            return HasEdge(edge.Item1, edge.Item2, directed);
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="Graph{N}"/>.
        /// Looks like "Graph with n nodes and m edges", where "n" and "m" are numbers.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Graph{N}"/>.</returns>
        public override string ToString()
        {
            return $"Graph with {NumberOfNodes} nodes and {NumberOfEdges} edges.";
        }

        // TODO: Should not be necessary.
        /// <summary>
        /// Update the nodes in this <see cref="Graph{N}"/>.
        /// </summary>
        protected void UpdateNodesInGraph()
        {
            if (InternalNodes.Count == 0)
            {
                return;
            }

            InternalNodes = new List<N>(DFS.FindConnectedComponent(InternalNodes[0]));
            UniqueInternalNodes = new HashSet<N>(InternalNodes);
        }

        // TODO: Should not be necessary.
        /// <summary>
        /// Update the edges in this <see cref="Graph{N}"/>.
        /// </summary>
        protected void UpdateEdgesInGraph()
        {
            InternalEdges = new List<(N, N)>(DFS.FindAllEdgesGraph<Graph<N>, N>(this));
            UniqueInternalEdges = new HashSet<(N, N)>(InternalEdges);
        }

        /// <summary>
        /// Add a new <typeparamref name="N"/> <paramref name="node"/> to this <see cref="Graph{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        /// <exception cref="AlreadyInGraphException">Thrown when <paramref name="node"/> is already part of this <see cref="Graph{N}"/>.</exception>
        public void AddNode(N node)
        {
            Utils.NullCheck(node, nameof(node), $"Trying to add a node to {this}, but the node is null!");
            if (HasNode(node))
            {
                throw new AlreadyInGraphException($"Trying to add {node} to {this}, but {node} is already part of {this}!");
            }

            UniqueInternalNodes.Add(node);
            InternalNodes.Add(node);
        }

        /// <summary>
        /// Add multiple new <typeparamref name="N"/>s to this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> is <see langword="null"/>.</exception>
        public void AddNodes(IEnumerable<N> nodes)
        {
            Utils.NullCheck(nodes, nameof(nodes), $"Trying to add an IEnumerable of nodes to {this}, but the IEnumerable is null!");

            foreach (N node in nodes)
            {
                AddNode(node);
            }
        }

        /// <summary>
        /// Add an edge between <paramref name="origin"/> and <paramref name="destination"/> to this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="origin"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either <paramref name="origin"/> or <paramref name="destination"/> is not part of this <see cref="Graph{N}"/>.</exception>
        /// <exception cref="AlreadyInGraphException">Thrown when the edge to be added is already part of this <see cref="Graph{N}"/>.</exception>
        public void AddEdge(N origin, N destination, bool directed = false)
        {
            Utils.NullCheck(origin, nameof(origin), $"Trying to add an edge to {this}, but the origin of the edge is null!");
            Utils.NullCheck(destination, nameof(destination), $"Trying to add an edge to {this}, but the destination of the edge is null!");
            if (!HasNode(origin))
            {
                throw new NotInGraphException($"Trying to add an edge between {origin} and {destination} to {this}, but {origin} is not part of {this}!");
            }
            if (!HasNode(destination))
            {
                throw new NotInGraphException($"Trying to add an edge between {origin} and {destination} to {this}, but {destination} is not part of {this}!");
            }

            (N, N) edge1 = (origin, destination);
            if (HasEdge(edge1, true) || (!directed && HasEdge((destination, origin), false)))
            {
                throw new AlreadyInGraphException($"Trying to add an edge between {origin} and {destination} to {this}, but this edge is already part of {this}!");
            }

            origin.AddNeighbour(destination, directed);
            InternalEdges.Add(edge1);
            UniqueInternalEdges.Add(edge1);
        }

        /// <summary>
        /// Add multiple edges to this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> is <see langword="null"/>.</exception>
        public void AddEdges(IEnumerable<(N, N)> edges, bool directed = false)
        {
            Utils.NullCheck(edges, nameof(edges), $"Trying to add an IEnumerable of edges to {this}, but the IEnumerable is null!");

            foreach ((N origin, N destination) in edges)
            {
                AddEdge(origin, destination, directed);
            }
        }

        /// <summary>
        /// Remove an <typeparamref name="N"/> from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="Graph{N}"/>.</exception>
        public void RemoveNode(N node)
        {
            Utils.NullCheck(node, nameof(node), $"Trying to remove a node from {this}, but the node is null!");
            if (!HasNode(node))
            {
                throw new NotInGraphException($"Trying to remove {node} from {this}, but {node} is not part of {this}!");
            }

            RemoveAllEdgesOfNode(node);
            UniqueInternalNodes.Remove(node);
            InternalNodes.Remove(node);
        }

        /// <summary>
        /// Remove multiple <typeparamref name="N"/>s from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> is <see langword="null"/>.</exception>
        public void RemoveNodes(IEnumerable<N> nodes)
        {
            Utils.NullCheck(nodes, nameof(nodes), $"Trying to remove multiple nodes from {this}, but the IEnumerable with nodes is null!");

            foreach (N node in nodes)
            {
                RemoveNode(node);
            }
        }

        /// <summary>
        /// Removes all edges connected to a given <typeparamref name="N"/> from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="Graph{N}"/>.</exception>
        public void RemoveAllEdgesOfNode(N node, bool directed = false)
        {
            Utils.NullCheck(node, nameof(node), $"Trying to remove all edges of a node in {this}, but the node is null!");
            if (!HasNode(node))
            {
                throw new NotInGraphException($"Trying to remove all edges of {node} from {this}, but {node} is not part of {this}!");
            }

            RemoveEdges(node.Neighbours.Select(neighbour => (node, neighbour)).ToList(), directed);
        }

        /// <summary>
        /// Remove multiple edges from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> is <see langword="null"/>.</exception>
        public void RemoveEdges(IList<(N, N)> edges, bool directed = false)
        {
            Utils.NullCheck(edges, nameof(edges), $"Trying to remove multiple edges from {this}, but the IEnumerable with edges is null!");

            foreach ((N origin, N destination) in edges)
            {
                RemoveEdge(origin, destination, directed);
            }
        }

        /// <summary>
        /// Remove an edge from this <see cref="Graph{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="origin"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="origin"/>, <paramref name="destination"/> or the edge between them is not part of this <see cref="Graph{N}"/>.</exception>
        public void RemoveEdge(N origin, N destination, bool directed = false)
        { 
            Utils.NullCheck(origin, nameof(origin), $"Trying to remove the edge from {this}, but the origin of the edge is null!");
            Utils.NullCheck(destination, nameof(destination), $"Trying to remove the edge from {this}, but the destination of the edge is null!");
            if (!HasNode(origin))
            {
                throw new NotInGraphException($"Trying to remove the edge between {origin} and {destination} from {this}, but {origin} is not part of {this}!");
            }
            if (!HasNode(destination))
            {
                throw new NotInGraphException($"Trying to remove the edge between {origin} and {destination} from {this}, but {destination} is not part of {this}!");
            }

            (N, N) edge1 = (origin, destination);
            (N, N) edge2 = (destination, origin);
            if ((directed && !HasEdge(edge1, true)) || (!HasEdge(edge1, directed)) && (!HasEdge(edge2, directed)))
            {
                throw new NotInGraphException($"Trying to remove the edge between {origin} and {destination} from {this}, but this edge is not part of {this}!");
            }

            origin.RemoveNeighbour(destination, directed);
            if (HasEdge(edge1, true))
            {
                InternalEdges.Remove(edge1);
                UniqueInternalEdges.Remove(edge1);
                return;
            }

            if (!directed)
            {
                if (HasEdge(edge2, true))
                {
                    InternalEdges.Remove(edge2);
                    UniqueInternalEdges.Remove(edge2);
                }
            }
        }

        /// <summary>
        /// Checks whether this <see cref="Graph{N}"/> is acyclic.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="Graph{N}"/> is acyclic, <see langword="false"/> if it is cyclic.</returns>
        public bool IsAcyclic()
        {
            return DFS.IsAcyclicGraph<Graph<N>, N>(this);
        }

        /// <summary>
        /// Checks whether this <see cref="Graph{N}"/> is connected.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="Graph{N}"/> is connected, <see langword="false"/> otherwise.</returns>
        public bool IsConnected()
        {
            return DFS.FindAllConnectedComponents(Nodes, null).Count == 1;
        }
    }
}
