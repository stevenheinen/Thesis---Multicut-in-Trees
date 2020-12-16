// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MulticutInTrees
{
    /// <summary>
    /// Interface for a node.
    /// </summary>
    /// <typeparam name="N">The type of node that implements <see cref="ITreeNode{N}"/>.</typeparam>
    public interface ITreeNode<N> : IEquatable<ITreeNode<N>> where N : ITreeNode<N>
    {

        /// <summary>
        /// The unique identifier of this <see cref="ITreeNode{N}"/>.
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of children this <see cref="ITreeNode{N}"/> is connected to. Cannot be edited by itself. This should be done using existing methods.
        /// See also <seealso cref="AddChild(N)"/>, <seealso cref="AddChildren(IEnumerable{N})"/>, <seealso cref="RemoveChild(N)"/>, <seealso cref="RemoveChildren(IEnumerable{N})"/> and <seealso cref="RemoveAllChildren"/>.
        /// </summary>
        public ReadOnlyCollection<N> Children { get; }

        /// <summary>
        /// The degree of this <see cref="ITreeNode{N}"/> in the graph.
        /// </summary>
        public int Degree { get; }

        /// <summary>
        /// The depth of this <see cref="ITreeNode{N}"/> in the tree measured from the root.
        /// </summary>
        public int DepthFromRoot { get; }

        /// <summary>
        /// The depth of the subtree rooted at this <see cref="ITreeNode{N}"/>.
        /// </summary>
        public int DepthOfSubtree { get; }

        /// <summary>
        /// The parent of this <see cref="ITreeNode{N}"/> in the tree.
        /// </summary>
        public N Parent { get; }

        /// <summary>
        /// Add a new child to this <see cref="ITreeNode{N}"/>.
        /// </summary>
        /// <param name="child">The new child to be added.s</param>
        public void AddChild(N child);

        /// <summary>
        /// Add multiple new children to this <see cref="ITreeNode{N}"/>.
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of children to be added.</param>
        public void AddChildren(IEnumerable<N> children);

        /// <summary>
        /// Remove a child from this <see cref="ITreeNode{N}"/>.
        /// </summary>
        /// <param name="child">The child to be removed.</param>
        public void RemoveChild(N child);

        /// <summary>
        /// Remove multiple children from this <see cref="ITreeNode{N}"/>.
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of children to be removed.</param>
        public void RemoveChildren(IEnumerable<N> children);

        /// <summary>
        /// Removes all children from this <see cref="ITreeNode{N}"/>.
        /// </summary>
        public void RemoveAllChildren();

        /// <summary>
        /// Returns this <see cref="ITreeNode{N}"/> as a <see cref="string"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> of this <see cref="ITreeNode{N}"/>.</returns>
        public string ToString();

        /// <summary>
        /// Checks whether the parameter <paramref name="node"/> is a child of this node.
        /// </summary>
        /// <param name="node">The node that is potentially a child of this <see cref="ITreeNode{N}"/>.</param>
        /// <returns><see langword="true"/> if the parameter is a child of this node, <see langword="false"/> otherwise.</returns>
        public bool HasChild(N node);

        /// <summary>
        /// Checks whether this <see cref="ITreeNode{N}"/> is the root of the tree it is in.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="ITreeNode{N}"/> is the root of its tree, <see langword="false"/> otherwise.</returns>
        public bool IsRoot();
    }
}