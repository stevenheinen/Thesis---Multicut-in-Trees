// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Interface for a tree.
    /// </summary>
    /// <typeparam name="TNode">The type of <see cref="ITreeNode{N}"/> the tree consists of.</typeparam>
    public interface ITree<TNode> where TNode : ITreeNode<TNode>
    {
        /// <summary>
        /// The <see cref="CountedEnumerable{T}"/> of all edges in the graph. Cannot be edited directly.
        /// <para>
        /// See also: <seealso cref="AddChild(TNode, TNode, Counter)"/>, <seealso cref="AddChildren(TNode, IEnumerable{TNode}, Counter)"/>, <seealso cref="AddRoot(TNode, Counter)"/>, <seealso cref="RemoveNode(TNode, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{TNode}, Counter)"/>.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all edges in this <see cref="ITree{N}"/>.</returns>
        public CountedEnumerable<(TNode, TNode)> Edges(Counter counter);

        /// <summary>
        /// The <see cref="CountedEnumerable{T}"/> of all nodes in the graph. Cannot be edited directly.
        /// <para>
        /// See also: <seealso cref="AddChild(TNode, TNode, Counter)"/>, <seealso cref="AddChildren(TNode, IEnumerable{TNode}, Counter)"/>, <seealso cref="AddRoot(TNode, Counter)"/>, <seealso cref="RemoveNode(TNode, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{TNode}, Counter)"/>.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all nodes in this <see cref="ITree{N}"/>.</returns>
        public CountedEnumerable<TNode> Nodes(Counter counter);

        /// <summary>
        /// Get the root of this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>The <typeparamref name="TNode"/> that is the root of this <see cref="ITree{N}"/>.</returns>
        TNode GetRoot(Counter counter);

        /// <summary>
        /// Returns height of this <see cref="ITree{N}"/>. This is the distance from the root to the leaf with furthest depth.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that represents the height of this <see cref="ITree{N}"/>.</returns>
        int Height(Counter counter);

        /// <summary>
        /// Checks whether <paramref name="node"/> is part of this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> for which we want to know if it is part of this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        bool HasNode(TNode node, Counter counter);

        /// <summary>
        /// Checks whether the edge between <paramref name="parent"/> and <paramref name="child"/> is part of this <see cref="ITree{N}"/>. In other words, check whether <paramref name="parent"/> is the parent of <paramref name="child"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="TNode"/> that should be the parent of the connection.</param>
        /// <param name="child">The <typeparamref name="TNode"/> that should be the child of the connection.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if the edge between <paramref name="parent"/> and <paramref name="child"/> exists in this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        bool HasEdge(TNode parent, TNode child, Counter counter);

        /// <summary>
        /// Checks whether the edge <paramref name="edge"/> is part of this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="edge">The <see cref="ValueTuple{T1, T2}"/> of <typeparamref name="TNode"/>s for which we want to know if it is part of this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> exists in this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        bool HasEdge((TNode, TNode) edge, Counter counter);

        /// <summary>
        /// Add a new root to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="newRoot">The <typeparamref name="TNode"/> that will be the new root of this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        void AddRoot(TNode newRoot, Counter counter);

        /// <summary>
        /// Add a new <typeparamref name="TNode"/> to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="TNode"/> that will be the parent of the new <typeparamref name="TNode"/>. Should already be part of the <see cref="ITree{N}"/>.</param>
        /// <param name="child">The new <typeparamref name="TNode"/> that will be added as a child of <paramref name="parent"/> in this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        void AddChild(TNode parent, TNode child, Counter counter);

        /// <summary>
        /// Add multiple new <typeparamref name="TNode"/>s  to this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="TNode"/> that will be the parent of the new <typeparamref name="TNode"/>s. Should already be part of the <see cref="ITree{N}"/>.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of new <typeparamref name="TNode"/>s that will be added as children of <paramref name="parent"/> in this <see cref="ITree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        void AddChildren(TNode parent, IEnumerable<TNode> children, Counter counter);

        /// <summary>
        /// Removes an <typeparamref name="TNode"/> from this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        void RemoveNode(TNode node, Counter counter);

        /// <summary>
        /// Removes multiple <typeparamref name="TNode"/>s from this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> of <typeparamref name="TNode"/>s to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        void RemoveNodes(IEnumerable<TNode> nodes, Counter counter);

        /// <summary>
        /// Returns the number of <typeparamref name="TNode"/>s in this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of <typeparamref name="TNode"/>s in this <see cref="ITree{N}"/>.</returns>
        int NumberOfNodes(Counter counter);

        /// <summary>
        /// Returns the number of edges in this <see cref="ITree{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of edges in this <see cref="ITree{N}"/>.</returns>
        int NumberOfEdges(Counter counter);
    }
}
