// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Interface for a general graph.
    /// </summary>
    /// <typeparam name="N">The type of nodes used in the graph.</typeparam>
    public interface IGraph<N> where N : INode<N>
    {
        /// <summary>
        /// The number of nodes in this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of nodes in this <see cref="IGraph{N}"/>.</returns>
        public int NumberOfNodes(Counter counter);

        /// <summary>
        /// The number of edges in this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of edges in this <see cref="IGraph{N}"/>.</returns>
        public int NumberOfEdges(Counter counter);

        /// <summary>
        /// The <see cref="CountedEnumerable{T}"/> of all edges in the graph. Cannot be edited directly.
        /// <para>
        /// See also: <seealso cref="AddEdge(N, N, Counter, bool)"/>, <seealso cref="AddEdges(IEnumerable{ValueTuple{N, N}}, Counter, bool)"/>, <seealso cref="RemoveEdge(N, N, Counter, bool)"/>, <seealso cref="RemoveEdges(IList{ValueTuple{N, N}}, Counter, bool)"/> and <seealso cref="RemoveAllEdgesOfNode(N, Counter, bool)"/>.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all edges in this <see cref="IGraph{N}"/>.</returns>
        public CountedEnumerable<(N, N)> Edges(Counter counter);

        /// <summary>
        /// The <see cref="CountedEnumerable{T}"/> of all nodes in the graph. Cannot be edited directly.
        /// <para>
        /// See also: <seealso cref="AddNode(N, Counter)"/>, <seealso cref="AddNodes(IEnumerable{N}, Counter)"/>, <seealso cref="RemoveNode(N, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{N}, Counter)"/>.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all nodes in this <see cref="IGraph{N}"/>.</returns>
        public CountedEnumerable<N> Nodes(Counter counter);

        /// <summary>
        /// Checks whether <paramref name="node"/> is part of this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is part of this <see cref="IGraph{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="IGraph{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasNode(N node, Counter counter);

        /// <summary>
        /// Checks whether an edge between <paramref name="origin"/> and <paramref name="destination"/> exists in this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="origin">The first endpont of the edge.</param>
        /// <param name="destination">The other endpoint of the edge.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed">Optional: if <see langword="true"/>, we only check for an edge from <paramref name="origin"/> to <paramref name="destination"/>. If <see langword="false"/>, we also check the edge from <paramref name="destination"/> to <paramref name="origin"/>.</param>
        /// <returns><see langword="true"/> if the edge between <paramref name="origin"/> and <paramref name="destination"/> exists in this <see cref="IGraph{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasEdge(N origin, N destination, Counter counter, bool directed = false);

        /// <summary>
        /// Checks whether the edge <paramref name="edge"/> exists in this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="edge">The tuple of two <typeparamref name="N"/>s that defines the edge.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed">Optional: if <see langword="true"/>, we only check for an edge from the first <typeparamref name="N"/> in <paramref name="edge"/> to the second <typeparamref name="N"/> in <paramref name="edge"/>. If <see langword="false"/>, we also check the edge from the second <typeparamref name="N"/> in <paramref name="edge"/> to the first <typeparamref name="N"/> in <paramref name="edge"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> exists in this <see cref="IGraph{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasEdge((N, N) edge, Counter counter, bool directed = false);

        /// <summary>
        /// Add an <typeparamref name="N"/> to this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        public void AddNode(N node, Counter counter);

        /// <summary>
        /// Add multiple <typeparamref name="N"/>s to this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        public void AddNodes(IEnumerable<N> nodes, Counter counter);

        /// <summary>
        /// Add an edge to this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="origin">The first endpoint of the edge.</param>
        /// <param name="destination">The other endpoint of the edge.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed">If <see langword="true"/>, only the edge from <paramref name="origin"/> to <paramref name="destination"/> is added. If <see langword="false"/>, the edge from <paramref name="destination"/> to <paramref name="origin"/> is added as well.</param>
        public void AddEdge(N origin, N destination, Counter counter, bool directed = false);

        /// <summary>
        /// Add multiple edges to this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="edges">An <see cref="IEnumerable{T}"/> with a tuple for each edge to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed">If <see langword="true"/>, only the edge from the first element of each tuple to the second element of the same tuple is added. If <see langword="false"/>, the edge from the second to the first element is added as well.</param>
        public void AddEdges(IEnumerable<(N, N)> edges, Counter counter, bool directed = false);

        /// <summary>
        /// Remove a <typeparamref name="N"/> from this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        public void RemoveNode(N node, Counter counter);

        /// <summary>
        /// Remove multiple <typeparamref name="N"/>s from this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        public void RemoveNodes(IEnumerable<N> nodes, Counter counter);

        /// <summary>
        /// Remove an edge from this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="origin">The first endpoint of the edge.</param>
        /// <param name="destination">The other endpoint of the edge.</param>
        /// <param name="directed">If <see langword="true"/>, only the edge from <paramref name="origin"/> to <paramref name="destination"/> is removed. If <see langword="false"/>, the edge from <paramref name="destination"/> to <paramref name="origin"/> is removed as well.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        public void RemoveEdge(N origin, N destination, Counter counter, bool directed = false);

        /// <summary>
        /// Remove multiple edges from this <see cref="IGraph{N}"/>.
        /// </summary>
        /// <param name="edges">An <see cref="IList{T}"/> with a tuple for each edge to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed">If <see langword="true"/>, only the edge from the first element of each tuple to the second element of the same tuple is removed. If <see langword="false"/>, the edge from the second to the first element is removed as well.</param>
        public void RemoveEdges(IList<(N, N)> edges, Counter counter, bool directed = false);

        /// <summary>
        /// Remove all edges that are connected to <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> from which we want to remove all its edges.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed">If <see langword="true"/>, only the outgoing edges from <paramref name="node"/> are removed. If <see langword="false"/>, the edges incoming to <paramref name="node"/> are removed as well.</param>
        public void RemoveAllEdgesOfNode(N node, Counter counter, bool directed = false);

        /// <summary>
        /// Checks whether this <see cref="IGraph{N}"/> is acyclic.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used during this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="IGraph{N}"/> is acyclic, <see langword="false"/> if it is cyclic.</returns>
        public bool IsAcyclic(Counter counter);

        /// <summary>
        /// Checks whether this <see cref="IGraph{N}"/> is connected.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used during this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="IGraph{N}"/> is connected, <see langword="false"/> otherwise.</returns>
        public bool IsConnected(Counter counter);
    }
}
