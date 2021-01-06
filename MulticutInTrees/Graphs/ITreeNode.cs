// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Interface for a node to be used in a tree.
    /// </summary>
    /// <typeparam name="N">The type of node that implements <see cref="ITreeNode{N}"/>.</typeparam>
    public interface ITreeNode<N> : INode<N> where N : INode<N>
    {
        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of children this <see cref="ITreeNode{N}"/> is connected to. Cannot be edited directly.
        /// <para>
        /// When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/>, refer to <seealso cref="ITree{N}.AddChild(N, N)"/>, <seealso cref="ITree{N}.AddChildren(N, IEnumerable{N})"/>, <seealso cref="ITree{N}.RemoveNode(N)"/> and <seealso cref="ITree{N}.RemoveNodes(IEnumerable{N})"/>.
        /// <br/>
        /// When using this <see cref="ITreeNode{N}"/> without and <see cref="ITree{N}"/>, refer to <seealso cref="AddChild(N)"/>, <seealso cref="AddChildren(IEnumerable{N})"/>, <seealso cref="RemoveChild(N)"/>, <seealso cref="RemoveChildren(IEnumerable{N})"/> and <seealso cref="RemoveAllChildren"/>.
        /// </para>
        /// </summary>
        public ReadOnlyCollection<N> Children { get; }

        /// <summary>
        /// The depth of this <see cref="ITreeNode{N}"/> in the tree measured from the root.
        /// </summary>
        public int DepthFromRoot { get; }

        /// <summary>
        /// The height of the subtree rooted at this <see cref="ITreeNode{N}"/>.
        /// </summary>
        public int HeightOfSubtree { get; }

        /// <summary>
        /// The parent of this <see cref="ITreeNode{N}"/> in the tree. Cannot be edited directly.
        /// <para>
        /// When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/>, refer to <seealso cref="ITree{N}.AddChild(N, N)"/>, <seealso cref="ITree{N}.AddChildren(N, IEnumerable{N})"/>, <seealso cref="ITree{N}.RemoveNode(N)"/> and <seealso cref="ITree{N}.RemoveNodes(IEnumerable{N})"/>.
        /// <br/>
        /// When using this <see cref="ITreeNode{N}"/> without and <see cref="ITree{N}"/>, refer to <seealso cref="AddChild(N)"/>, <seealso cref="AddChildren(IEnumerable{N})"/>, <seealso cref="RemoveChild(N)"/>, <seealso cref="RemoveChildren(IEnumerable{N})"/> and <seealso cref="RemoveAllChildren"/>.
        /// </para>
        /// </summary>
        public N Parent { get; }

        /// <summary>
        /// Checks whether this <see cref="ITreeNode{N}"/> is the root of the tree it is in.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="ITreeNode{N}"/> is the root of its tree, <see langword="false"/> otherwise.</returns>
        public bool IsRoot();

        /// <summary>
        /// Add an <typeparamref name="N"/> as child to this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="child">The <typeparamref name="N"/> to be added as child.</param>
        public void AddChild(N child);

        /// <summary>
        /// Add multiple <typeparamref name="N"/>s as children to this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be added as children.</param>
        public void AddChildren(IEnumerable<N> children);

        /// <summary>
        /// Remove an <typeparamref name="N"/> from the children of this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="child">The <typeparamref name="N"/> to be removed.</param>
        public void RemoveChild(N child);

        /// <summary>
        /// Remove multiple <typeparamref name="N"/>s from the children of this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be removed from the children of this <see cref="ITreeNode{N}"/>.</param>
        public void RemoveChildren(IEnumerable<N> children);

        /// <summary>
        /// Removes all children from this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        public void RemoveAllChildren();

        /// <summary>
        /// Checks whether <paramref name="node"/> is a child of this <see cref="ITreeNode{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is a child of this <see cref="ITreeNode{N}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is a child of this <see cref="ITreeNode{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasChild(N node);
    }
}