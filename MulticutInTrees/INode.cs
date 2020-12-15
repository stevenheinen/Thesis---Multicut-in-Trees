// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MulticutInTrees
{
    /// <summary>
    /// Interface for a node.
    /// </summary>
    /// <typeparam name="N">The type of node that implements <see cref="INode{N}"/>.</typeparam>
    public interface INode<N> : IEquatable<INode<N>> where N : INode<N>
    {

        /// <summary>
        /// The unique identifier of this <see cref="INode{N}"/>.
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of neighbours this <see cref="INode{N}"/> is connected to. Cannot be edited by itself. This should be done using existing methods.
        /// See also <seealso cref="AddNeighbour(N)"/>, <seealso cref="AddNeighbours(IEnumerable{N})"/>, <seealso cref="RemoveNeighbour(N)"/>, <seealso cref="RemoveNeighbours(IEnumerable{N})"/> and <seealso cref="RemoveAllNeighbours"/>.
        /// </summary>
        public ReadOnlyCollection<N> Neighbours { get; }

        /// <summary>
        /// The degree of this <see cref="INode{N}"/> in the graph.
        /// </summary>
        public int Degree { get; }

        /// <summary>
        /// Add a new neighbour to this <see cref="INode{N}"/>.
        /// </summary>
        /// <param name="neighbour">The new neighbour to be added.s</param>
        public void AddNeighbour(N neighbour);

        /// <summary>
        /// Add multiple new neighbours to this <see cref="INode{N}"/>.
        /// </summary>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> of neighbours to be added.</param>
        public void AddNeighbours(IEnumerable<N> neighbours);

        /// <summary>
        /// Remove a neighbour from this <see cref="INode{N}"/>.
        /// </summary>
        /// <param name="neighbour">The neighbour to be removed.</param>
        public void RemoveNeighbour(N neighbour);

        /// <summary>
        /// Remove multiple neighbours from this <see cref="INode{N}"/>.
        /// </summary>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> of neighbours to be removed.</param>
        public void RemoveNeighbours(IEnumerable<N> neighbours);
        
        /// <summary>
        /// Removes all neighbours from this <see cref="INode{N}"/>.
        /// </summary>
        public void RemoveAllNeighbours();

        /// <summary>
        /// Returns this <see cref="INode{N}"/> as a <see cref="string"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> of this <see cref="INode{N}"/>.</returns>
        public string ToString();

        /// <summary>
        /// Checks whether the parameter <paramref name="node"/> is a neighbour of this node.
        /// </summary>
        /// <param name="node">The node that is potentially a neighbour of this <see cref="INode{N}"/>.</param>
        /// <returns><see langword="true"/> if the parameter is a neighbour of this node, <see langword="false"/> otherwise.</returns>
        public bool HasNeighbour(N node);
    }
}