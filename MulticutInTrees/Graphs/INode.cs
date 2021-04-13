// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Interface for a node.
    /// </summary>
    /// <typeparam name="TNode">The type of node that implements <see cref="INode{N}"/>.</typeparam>
    public interface INode<TNode> where TNode : INode<TNode>
    {
        /// <summary>
        /// The unique identifier of this <see cref="INode{N}"/>.
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// The <see cref="NodeType"/> of this <see cref="INode{N}"/>.
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// The <see cref="CountedEnumerable{T}"/> of neighbours this <see cref="INode{N}"/> is connected to. Cannot be edited directly.
        /// <para>
        /// When using this <see cref="INode{N}"/> in combination with an <see cref="IGraph{N}"/>, refer to <seealso cref="IGraph{N}.AddEdge(N, N, Counter, bool)"/>, <seealso cref="IGraph{N}.AddEdges(IEnumerable{ValueTuple{N, N}}, Counter, bool)"/>, <seealso cref="IGraph{N}.RemoveEdge(N, N, Counter, bool)"/>, <seealso cref="IGraph{N}.RemoveEdges(IList{ValueTuple{N, N}}, Counter, bool)"/> and <seealso cref="IGraph{N}.RemoveAllEdgesOfNode(N, Counter, bool)"/>.
        /// <br/>
        /// When using this <see cref="INode{N}"/> without an <see cref="IGraph{N}"/>, refer to <seealso cref="AddNeighbour(TNode, Counter, bool)"/>, <seealso cref="AddNeighbours(IEnumerable{TNode}, Counter, bool)"/>, <seealso cref="RemoveNeighbour(TNode, Counter, bool)"/>, <seealso cref="RemoveNeighbours(IEnumerable{TNode}, Counter, bool)"/> and <seealso cref="RemoveAllNeighbours(Counter, bool)"/>.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all <typeparamref name="TNode"/>s that are neighbours of this <see cref="INode{N}"/>.</returns>
        public CountedEnumerable<TNode> Neighbours(Counter counter);

        /// <summary>
        /// The degree of this <see cref="INode{N}"/> in the graph.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of neighbours this <see cref="INode{N}"/> has.</returns>
        int Degree(Counter counter);

        /// <summary>
        /// Add an <typeparamref name="TNode"/> as neighbour to this <see cref="INode{N}"/>.
        /// </summary>
        /// <para>
        /// <b>Note:</b> When using this <see cref="INode{N}"/> in combination with an <see cref="IGraph{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// <param name="neighbour">The <typeparamref name="TNode"/> to be added as neighbour.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed"><b>Optional.</b> Whether the connection is directed. If <see langword="true"/>, only a connection between this <see cref="INode{N}"/> and <paramref name="neighbour"/> is made. If <see langword="false"/>, the connection from <paramref name="neighbour"/> to this <see cref="INode{N}"/> is also made.</param>
        void AddNeighbour(TNode neighbour, Counter counter, bool directed = false);

        /// <summary>
        /// Add multiple <typeparamref name="TNode"/>s as neighbours to this <see cref="INode{N}"/>.
        /// </summary>
        /// <para>
        /// <b>Note:</b> When using this <see cref="INode{N}"/> in combination with an <see cref="IGraph{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> of <typeparamref name="TNode"/>s to be added as neighbours to this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed"><b>Optional.</b> Whether the connection is directed. If <see langword="true"/>, only the connections between this <see cref="INode{N}"/> and the <typeparamref name="TNode"/>s in <paramref name="neighbours"/> are made. If <see langword="false"/>, the connections from the <typeparamref name="TNode"/>s in <paramref name="neighbours"/> to this <see cref="INode{N}"/> are also made.</param>
        void AddNeighbours(IEnumerable<TNode> neighbours, Counter counter, bool directed = false);

        /// <summary>
        /// Remove all neighbours from this <see cref="INode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="INode{N}"/> in combination with an <see cref="IGraph{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed"><b>Optional.</b> If <see langword="true"/>, only outgoing connections are removed. If <see langword="false"/>, incoming connections are removed as well.</param>
        void RemoveAllNeighbours(Counter counter, bool directed = false);

        /// <summary>
        /// Remove <paramref name="neighbour"/> from the neighbours of this <see cref="INode{N}"/>.
        /// </summary>
        /// <para>
        /// <b>Note:</b> When using this <see cref="INode{N}"/> in combination with an <see cref="IGraph{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// <param name="neighbour">The <typeparamref name="TNode"/> to be removed from the neighbours of this <see cref="INode{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed"><b>Optional.</b> Whether the connection is directed. If <see langword="true"/>, only the connection between this <see cref="INode{N}"/> and <paramref name="neighbour"/> will be removed. If <see langword="false"/>, the connection from <paramref name="neighbour"/> to this <see cref="INode{N}"/> will be removed as well.</param>
        void RemoveNeighbour(TNode neighbour, Counter counter, bool directed = false);

        /// <summary>
        /// Remove multiple <typeparamref name="TNode"/>s from the neighbours of this <see cref="INode{N}"/>.
        /// </summary>
        /// <para>
        /// <b>Note:</b> When using this <see cref="INode{N}"/> in combination with an <see cref="IGraph{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> of <typeparamref name="TNode"/>s to be removed from the neighbours of this <see cref="INode{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <param name="directed"><b>Optional.</b> Whether the connections are directed. If <see langword="true"/>, only the connections between this <see cref="INode{N}"/> and the <typeparamref name="TNode"/>s in <paramref name="neighbours"/> will be removed. If <see langword="false"/>, the connections from the <typeparamref name="TNode"/>s in <paramref name="neighbours"/> to this <see cref="INode{N}"/> will be removed as well.</param>
        void RemoveNeighbours(IEnumerable<TNode> neighbours, Counter counter, bool directed = false);

        /// <summary>
        /// Checks whether <paramref name="node"/> is a neighbour of this <see cref="INode{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> for which we want to know if it is a neighbour of this <see cref="INode{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is a neighbour of this <see cref="INode{N}"/>, <see langword="false"/> otherwise.</returns>
        bool HasNeighbour(TNode node, Counter counter);
    }
}