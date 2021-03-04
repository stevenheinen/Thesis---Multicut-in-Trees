// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Interface for a node to be used in a tree.
    /// </summary>
    /// <typeparam name="N">The type of node that implements <see cref="ITreeNode{N}"/>.</typeparam>
    public interface ITreeNode<N> : INode<N> where N : INode<N>
    {
        /// <summary>
        /// The <see cref="CountedEnumerable{T}"/> of children this <see cref="ITreeNode{N}"/> is connected to. Cannot be edited directly.
        /// <para>
        /// When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/>, refer to <seealso cref="ITree{N}.AddChild(N, N, Counter)"/>, <seealso cref="ITree{N}.AddChildren(N, IEnumerable{N}, Counter)"/>, <seealso cref="ITree{N}.RemoveNode(N, Counter)"/> and <seealso cref="ITree{N}.RemoveNodes(IEnumerable{N}, Counter)"/>.
        /// <br/>
        /// When using this <see cref="ITreeNode{N}"/> without and <see cref="ITree{N}"/>, refer to <seealso cref="AddChild(N, Counter)"/>, <seealso cref="AddChildren(IEnumerable{N}, Counter)"/>, <seealso cref="RemoveChild(N, Counter)"/>, <seealso cref="RemoveChildren(IEnumerable{N}, Counter)"/> and <seealso cref="RemoveAllChildren"/>.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with <typeparamref name="N"/>s that are children of this <see cref="ITreeNode{N}"/>.</returns>
        public CountedEnumerable<N> Children(Counter counter);

        /// <summary>
        /// The depth of this <see cref="ITreeNode{N}"/> in the tree measured from the root.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that is equal to the depth of this <see cref="ITreeNode{N}"/> in the tree measured from the root.</returns>
        public int DepthFromRoot(Counter counter);

        /// <summary>
        /// The height of the subtree rooted at this <see cref="ITreeNode{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that is equal to the height of the subtree that is rooted at this <see cref="ITreeNode{N}"/>.</returns>
        public int HeightOfSubtree(Counter counter);

        /// <summary>
        /// Checks whether this <see cref="ITreeNode{N}"/> is the root of the tree it is in.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="ITreeNode{N}"/> is the root of its tree, <see langword="false"/> otherwise.</returns>
        public bool IsRoot(Counter counter);

        /// <summary>
        /// Add an <typeparamref name="N"/> as child to this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="child">The <typeparamref name="N"/> to be added as child.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void AddChild(N child, Counter counter);

        /// <summary>
        /// Add multiple <typeparamref name="N"/>s as children to this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be added as children.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void AddChildren(IEnumerable<N> children, Counter counter);

        /// <summary>
        /// Remove an <typeparamref name="N"/> from the children of this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="child">The <typeparamref name="N"/> to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void RemoveChild(N child, Counter counter);

        /// <summary>
        /// Remove multiple <typeparamref name="N"/>s from the children of this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be removed from the children of this <see cref="ITreeNode{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void RemoveChildren(IEnumerable<N> children, Counter counter);

        /// <summary>
        /// Removes all children from this <see cref="ITreeNode{N}"/>.
        /// <para>
        /// <b>Note:</b> When using this <see cref="ITreeNode{N}"/> in combination with an <see cref="ITree{N}"/> or any other graph, please use the graph's methods to add children. Changes made using these methods are not reflected in the graph.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        public void RemoveAllChildren(Counter counter);

        /// <summary>
        /// Checks whether <paramref name="node"/> is a child of this <see cref="ITreeNode{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is a child of this <see cref="ITreeNode{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is a child of this <see cref="ITreeNode{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool HasChild(N node, Counter counter);

        /// <summary>
        /// Returns the parent of this <see cref="ITreeNode{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>The <typeparamref name="N"/> that is the parent of this <see cref="ITreeNode{N}"/>.</returns>
        public N GetParent(Counter counter);

        /// <summary>
        /// Returns the number of children this <see cref="ITreeNode{N}"/> has.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of children this <see cref="ITreeNode{N}"/> has.</returns>
        public int NumberOfChildren(Counter counter);
    }
}