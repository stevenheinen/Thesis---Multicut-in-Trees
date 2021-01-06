// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Interface for a tree.
    /// </summary>
    /// <typeparam name="N">The type of <see cref="ITreeNode{N}"/> the tree consists of.</typeparam>
    public interface ITree<N> where N : ITreeNode<N>
    {
        /// <summary>
        /// The number of nodes in this <see cref="ITree{N}"/>.
        /// </summary>
        public int NumberOfNodes { get; }
        
        /// <summary>
        /// The root of this <see cref="ITree{N}"/>.
        /// </summary>
        public N Root { get; }

        /// <summary>
        /// The height of this <see cref="ITree{N}"/>. This is the distance from the root to the leaf with furthest depth.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of all edges in the graph. Cannot be edited directly.
        /// <para>
        /// See also: <seealso cref="AddChild(N, N)"/>, <seealso cref="AddChildren(N, IEnumerable{N})"/>, <seealso cref="AddRoot(N)"/>, <seealso cref="RemoveNode(N)"/> and <seealso cref="RemoveNodes(IEnumerable{N})"/>.
        /// </para>
        /// </summary>
        public ReadOnlyCollection<(N, N)> Edges { get; }

        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of all nodes in the graph. Cannot be edited directly.
        /// <para>
        /// See also: <seealso cref="AddChild(N, N)"/>, <seealso cref="AddChildren(N, IEnumerable{N})"/>, <seealso cref="AddRoot(N)"/>, <seealso cref="RemoveNode(N)"/> and <seealso cref="RemoveNodes(IEnumerable{N})"/>.
        /// </para>
        /// </summary>
        public ReadOnlyCollection<N> Nodes { get; }

        /// <summary>
        /// Checks whether <paramref name="node"/> is part of this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is part of this <see cref="ITree{N}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasNode(N node);

        /// <summary>
        /// Checks whether the edge between <paramref name="parent"/> and <paramref name="child"/> is part of this <see cref="ITree{N}"/>. In other words, check whether <paramref name="parent"/> is the parent of <paramref name="child"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="N"/> that should be the parent of the connection.</param>
        /// <param name="child">The <typeparamref name="N"/> that should be the child of the connection.</param>
        /// <returns><see langword="true"/> if the edge between <paramref name="parent"/> and <paramref name="child"/> exists in this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasEdge(N parent, N child);

        /// <summary>
        /// Add a new root to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="newRoot">The <typeparamref name="N"/> that will be the new root of this <see cref="ITree{N}"/>.</param>
        public void AddRoot(N newRoot);

        /// <summary>
        /// Add a new <typeparamref name="N"/> to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="N"/> that will be the parent of the new <typeparamref name="N"/>. Should already be part of the <see cref="ITree{N}"/>.</param>
        /// <param name="child">The new <typeparamref name="N"/> that will be added as a child of <paramref name="parent"/> in this <see cref="ITree{N}"/>.</param>
        public void AddChild(N parent, N child);

        /// <summary>
        /// Add multiple new <typeparamref name="N"/>s  to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="N"/> that will be the parent of the new <typeparamref name="N"/>s. Should already be part of the <see cref="ITree{N}"/>.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of new <typeparamref name="N"/>s that will be added as children of <paramref name="parent"/> in this <see cref="ITree{N}"/>.</param>
        public void AddChildren(N parent, IEnumerable<N> children);

        /// <summary>
        /// Removes an <typeparamref name="N"/> from this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> to be removed.</param>
        public void RemoveNode(N node);

        /// <summary>
        /// Removes multiple <typeparamref name="N"/>s from this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be removed.</param>
        public void RemoveNodes(IEnumerable<N> nodes);
    }
}
