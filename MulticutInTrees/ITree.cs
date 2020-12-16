// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MulticutInTrees
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
        /// The <see cref="ReadOnlyCollection{T}"/> of all nodes in the tree.
        /// </summary>
        public ReadOnlyCollection<N> NodesInTree { get; }

        /// <summary>
        /// The root of this <see cref="ITree{N}"/>.
        /// </summary>
        public N Root { get; }

        /// <summary>
        /// The depth of this <see cref="ITree{N}"/>. This is the distance from the root to the leaf with furthest depth.
        /// </summary>
        public int Depth { get; }
    }
}
