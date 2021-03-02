// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Interface for a tree.
    /// </summary>
    /// <typeparam name="N">The type of <see cref="ITreeNode{N}"/> the tree consists of.</typeparam>
    public interface ITree<N> where N : ITreeNode<N>
    {
        /// <summary>
        /// The <see cref="IEnumerable{T}"/> of all edges in the graph. Cannot be edited directly.
        /// <para>
        /// See also: <seealso cref="AddChild(N, N, Counter)"/>, <seealso cref="AddChildren(N, IEnumerable{N}, Counter)"/>, <seealso cref="AddRoot(N, Counter)"/>, <seealso cref="RemoveNode(N, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{N}, Counter)"/>.
        /// </para>
        /// </summary>
        public IEnumerable<(N, N)> Edges { get; }

        /// <summary>
        /// The <see cref="IEnumerable{T}"/> of all nodes in the graph. Cannot be edited directly.
        /// <para>
        /// See also: <seealso cref="AddChild(N, N, Counter)"/>, <seealso cref="AddChildren(N, IEnumerable{N}, Counter)"/>, <seealso cref="AddRoot(N, Counter)"/>, <seealso cref="RemoveNode(N, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{N}, Counter)"/>.
        /// </para>
        /// </summary>
        public IEnumerable<N> Nodes { get; }

        /// <summary>
        /// Get the root of this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>The <typeparamref name="N"/> that is the root of this <see cref="ITree{N}"/>.</returns>
        public N GetRoot(Counter counter);

        /// <summary>
        /// Returns height of this <see cref="ITree{N}"/>. This is the distance from the root to the leaf with furthest depth.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that represents the height of this <see cref="ITree{N}"/>.</returns>
        public int Height(Counter counter);

        /// <summary>
        /// Checks whether <paramref name="node"/> is part of this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is part of this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasNode(N node, Counter counter);

        /// <summary>
        /// Checks whether the edge between <paramref name="parent"/> and <paramref name="child"/> is part of this <see cref="ITree{N}"/>. In other words, check whether <paramref name="parent"/> is the parent of <paramref name="child"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="N"/> that should be the parent of the connection.</param>
        /// <param name="child">The <typeparamref name="N"/> that should be the child of the connection.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if the edge between <paramref name="parent"/> and <paramref name="child"/> exists in this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasEdge(N parent, N child, Counter counter);

        /// <summary>
        /// Checks whether the edge <paramref name="edge"/> is part of this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="edge">The <see cref="ValueTuple{T1, T2}"/> of <typeparamref name="N"/>s for which we want to know if it is part of this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> exists in this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasEdge((N, N) edge, Counter counter);

        /// <summary>
        /// Add a new root to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="newRoot">The <typeparamref name="N"/> that will be the new root of this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void AddRoot(N newRoot, Counter counter);

        /// <summary>
        /// Add a new <typeparamref name="N"/> to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="N"/> that will be the parent of the new <typeparamref name="N"/>. Should already be part of the <see cref="ITree{N}"/>.</param>
        /// <param name="child">The new <typeparamref name="N"/> that will be added as a child of <paramref name="parent"/> in this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void AddChild(N parent, N child, Counter counter);

        /// <summary>
        /// Add multiple new <typeparamref name="N"/>s  to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="N"/> that will be the parent of the new <typeparamref name="N"/>s. Should already be part of the <see cref="ITree{N}"/>.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of new <typeparamref name="N"/>s that will be added as children of <paramref name="parent"/> in this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void AddChildren(N parent, IEnumerable<N> children, Counter counter);

        /// <summary>
        /// Removes an <typeparamref name="N"/> from this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void RemoveNode(N node, Counter counter);

        /// <summary>
        /// Removes multiple <typeparamref name="N"/>s from this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void RemoveNodes(IEnumerable<N> nodes, Counter counter);
    }
}
