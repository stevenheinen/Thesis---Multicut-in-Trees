// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Utilities;

namespace MulticutInTrees
{
    /// <summary>
    /// A tree that implements <see cref="ITree{N}"/> and consists of a class that implements <see cref="ITreeNode{T}"/>.
    /// </summary>
    public class Tree<N> : ITree<N> where N : ITreeNode<N>
    {
        /// <summary>
        /// The internal <see cref="List{T}"/> of nodes in this <see cref="Tree{T}"/>.
        /// </summary>
        private List<N> InternalNodesInTree { get; set; }

        /// <summary>
        /// The internal <see cref="HashSet{T}"/> of nodes in this <see cref="Tree{T}"/>. Using a <see cref="HashSet{T}"/> makes lookups amortised faster.
        /// </summary>
        private HashSet<N> UniqueInternalNodesInTree { get; set; }

        /// <summary>
        /// The number of nodes in this <see cref="TreeNode"/>.
        /// </summary>
        public int NumberOfNodes
        {
            get
            {
                // Update the nodes, because connections might have been broken or made.
                UpdateNodesInTree();
                return InternalNodesInTree.Count;
            }
        }

        /// <summary>
        /// The root of this <see cref="Tree{N}"/>.
        /// </summary>
        public N Root { get; private set; }

        /// <summary>
        /// The depth of this <see cref="Tree{N}"/>. Equal to the depth of the subtree of the <see cref="Root"/> of this <see cref="Tree{N}"/>.
        /// </summary>
        public int Depth => Root.DepthOfSubtree;

        /// <summary>
        /// The publically visible collection of nodes in this <see cref="Tree{N}"/>. Cannot be added. Changes must be made to <typeparamref name="N"/>s themselves.
        /// </summary>
        public ReadOnlyCollection<N> NodesInTree
        {
            get
            {
                // Update the nodes, because connections might have been broken or made.
                UpdateNodesInTree();
                return InternalNodesInTree.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor for a <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="node">One of the nodes in this <see cref="Tree{N}"/>. Does not necessarily have to be the root.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
        public Tree(N node)
        {
            if (node is null)
            {
                throw new ArgumentNullException("node", "Trying to make a Tree with a root, but the root is null!");
            }

            // Root the tree at the parameter, then find all nodes in the tree and find the actual root of the tree.
            Root = node;
            UpdateNodesInTree();
            Root = FindRoot(NodesInTree);
        }

        /// <summary>
        /// Constructor for a <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="nodes">All nodes in this tree.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> is null.</exception>
        public Tree(IEnumerable<N> nodes)
        {
            if (nodes is null)
            {
                throw new ArgumentNullException("nodes", "Trying to make a Tree with an IEnumerable of nodes, but the IEnumerable is null!");
            }

            Root = FindRoot(nodes);
            InternalNodesInTree = new List<N>(nodes);
            UniqueInternalNodesInTree = new HashSet<N>(nodes);
        }

        /// <summary>
        /// Constructor for a <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="root">The root of this <see cref="Tree{N}"/>.</param>
        /// <param name="nodes">All nodes in this tree.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/> or <paramref name="nodes"/> is null.</exception>
        /// <exception cref="NoRootException">Thrown when <paramref name="root"/> is not the root of the tree consisting of nodes in <paramref name="nodes"/>.</exception>
        public Tree(IEnumerable<N> nodes, N root)
        {
            if (nodes is null)
            {
                throw new ArgumentNullException("nodes", "Trying to make a Tree with an IEnumerable of nodes, but the IEnumerable is null!");
            }
            if (root is null)
            {
                throw new ArgumentNullException("root", "Trying to make a Tree with a root, but the root is null!");
            }
            if (!root.Equals(FindRoot(nodes)))
            {
                throw new NoRootException("Trying to make a Tree with a root and a set of nodes, but the parameter root is the first occurrence of a root in the set of nodes!");
            }

            Root = root;
            InternalNodesInTree = new List<N>(nodes);
            UniqueInternalNodesInTree = new HashSet<N>(nodes);
        }

        /// <summary>
        /// Finds whether a given <typeparamref name="N"/> is part of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is part of this <see cref="Tree{N}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="Tree{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
        public bool HasNode(N node)
        {
            if (node is null)
            {
                throw new ArgumentNullException("node", $"Trying to see if a node is in {this}, but the node is null!");
            }

            // Update the nodes, because connections might have been broken or made.
            UpdateNodesInTree();
            return UniqueInternalNodesInTree.Contains(node);
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="Tree{N}"/>.
        /// Looks like "Tree with depth d and n nodes: [Node 0, Node 1, ..., Node n]", where "d" and "n" are numbers, and "Node i" is an example of the <see cref="string"/> representation of the i-th node.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Tree{N}"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Tree with depth {Depth} and {NumberOfNodes} nodes: [");
            foreach (N node in InternalNodesInTree)
            {
                sb.Append($"{node}, ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Update the nodes in this <see cref="Tree{N}"/>.
        /// </summary>
        private void UpdateNodesInTree()
        {
            // Find the connected component starting from the root of this tree and save the found nodes. Then update the root.
            InternalNodesInTree = new List<N>(DFS.FindConnectedComponent(Root));
            UniqueInternalNodesInTree = new HashSet<N>(InternalNodesInTree);
            Root = FindRoot(InternalNodesInTree);
        }

        /// <summary>
        /// Find the first occurance of a root of a given <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s.
        /// <para>
        /// <b>Note:</b> Only the first occurrence of a root is found. In case there are mutliple roots in <paramref name="nodes"/>, this is not detected.
        /// </para>
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> in which to search the root.</param>
        /// <returns>The first occurance of an <typeparamref name="N"/> that is a root.</returns>
        /// <exception cref="NoRootException">Thrown when there is no root in <paramref name="nodes"/>.</exception>
        private N FindRoot(IEnumerable<N> nodes)
        {
            foreach (N node in nodes)
            {
                if (node.Parent is null)
                {
                    return node;
                }
            }
            throw new NoRootException("Trying to find a root in a set of nodes, but there is none!");
        }
    }
}
