// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Implementation of <see cref="ITreeNode{N}"/> to be used for nodes in a tree.
    /// </summary>
    public class TreeNode : ITreeNode<TreeNode>
    {
        /// <summary>
        /// The internal <see cref="List{T}"/> of <see cref="TreeNode"/>s that are children of this <see cref="TreeNode"/>.
        /// </summary>
        private List<TreeNode> InternalChildren { get; }

        /// <summary>
        /// The internal <see cref="HashSet{T}"/> of <see cref="TreeNode"/>s that are children of this <see cref="TreeNode"/>. Using a <see cref="HashSet{T}"/> makes lookups amortised faster.
        /// </summary>
        private HashSet<TreeNode> InternalUniqueChildren { get; }

        /// <inheritdoc/>
        public ReadOnlyCollection<TreeNode> Children => InternalChildren.AsReadOnly();

        /// <summary>
        /// The number of neighbours (children + parent) this <see cref="TreeNode"/> has.
        /// </summary>
        public int Degree
        {
            get
            {
                if (Parent is null)
                {
                    return InternalChildren.Count;
                }
                return InternalChildren.Count + 1;
            }
        }

        /// <inheritdoc/>
        public int DepthFromRoot
        {
            get
            {
                if (Parent is null)
                {
                    return 0;
                }
                return Parent.DepthFromRoot + 1;
            }
        }

        /// <inheritdoc/>
        public int HeightOfSubtree
        {
            get
            {
                if (InternalChildren.Count == 0)
                {
                    return 0;
                }
                int maxDepth = 0;
                foreach (TreeNode child in InternalChildren)
                {
                    maxDepth = Math.Max(maxDepth, child.HeightOfSubtree);
                }
                return maxDepth + 1;
            }
        }

        /// <summary>
        /// The internal representation for the parent of this <see cref="TreeNode"/>.
        /// </summary>
        public TreeNode Parent { get; private set; }

        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of all neighbours of this <see cref="TreeNode"/>. Includes <see cref="Parent"/> and <see cref="Children"/>. Cannot be edited directly.
        /// <br/>
        /// When using this <see cref="TreeNode"/> in combination with an <see cref="ITree{N}"/>, refer to <seealso cref="ITree{TreeNode}.AddChild(TreeNode, TreeNode)"/>, <seealso cref="ITree{TreeNode}.AddChildren(TreeNode, IEnumerable{TreeNode})"/>, <seealso cref="ITree{TreeNode}.RemoveNode(TreeNode)"/> and <seealso cref="ITree{TreeNode}.RemoveNodes(IEnumerable{TreeNode})"/>.
        /// <br/>
        /// When using this <see cref="TreeNode"/> without and <see cref="ITree{N}"/>, refer to <seealso cref="AddChild(TreeNode)"/>, <seealso cref="AddChildren(IEnumerable{TreeNode})"/>, <seealso cref="RemoveChild(TreeNode)"/>, <seealso cref="RemoveChildren(IEnumerable{TreeNode})"/> and <seealso cref="RemoveAllChildren"/>.
        /// </summary>
        public ReadOnlyCollection<TreeNode> Neighbours
        {
            get
            {
                if (Parent is null)
                {
                    return InternalChildren.AsReadOnly();
                }

                List<TreeNode> neighbours = new List<TreeNode>(InternalChildren)
                {
                    Parent
                };
                return neighbours.AsReadOnly();
            }
        }

        /// <summary>
        /// The unique identifier of this <see cref="TreeNode"/>.
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// Constructor for a <see cref="TreeNode"/>.
        /// <para>
        /// <b>Note:</b> Use this constructor when using this <see cref="TreeNode"/> in combination with an <see cref="IGraph{N}"/>.
        /// </para>
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="TreeNode"/>.</param>
        public TreeNode(uint id)
        {
            ID = id;
            InternalChildren = new List<TreeNode>();
            InternalUniqueChildren = new HashSet<TreeNode>();
        }

        /// <summary>
        /// Constructor for a <see cref="TreeNode"/>.
        /// <para>
        /// <b>Note:</b> DO NOT use this constructor when using this <see cref="TreeNode"/> in combination with an <see cref="IGraph{N}"/>.
        /// </para>
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="TreeNode"/>.</param>
        /// <param name="parent">The <see cref="TreeNode"/> that is the parent of the new <see cref="TreeNode"/> instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> is <see langword="null"/>.</exception>
        internal TreeNode(uint id, TreeNode parent)
        {
            Utils.NullCheck(parent, nameof(parent), $"Trying to create a new instance of {GetType()} with a predefined parent, but parent is null!");

            ID = id;
            InternalChildren = new List<TreeNode>();
            InternalUniqueChildren = new HashSet<TreeNode>();
            parent.AddChild(this);
        }

        /// <summary>
        /// Constructor for a <see cref="TreeNode"/>.
        /// <para>
        /// <b>Note:</b> DO NOT use this constructor when using this <see cref="TreeNode"/> in combination with an <see cref="IGraph{N}"/>.
        /// </para>
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="TreeNode"/>.</param>
        /// <param name="parent">The <see cref="TreeNode"/> that is the parent of the new <see cref="TreeNode"/> instance.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <see cref="TreeNode"/>s containing the children of this <see cref="TreeNode"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> or <paramref name="children"/> is <see langword="null"/>.</exception>
        internal TreeNode(uint id, TreeNode parent, IEnumerable<TreeNode> children)
        {
            Utils.NullCheck(parent, nameof(parent), $"Trying to create a new instance of {GetType()} with a predefined parent, but parent is null!");
            Utils.NullCheck(children, nameof(children), $"Trying to create a new instance of {GetType()} with predefined children, but the IEnumberable of children is null!");

            ID = id;
            InternalChildren = new List<TreeNode>();
            InternalUniqueChildren = new HashSet<TreeNode>();
            AddChildren(children);
            parent.AddChild(this);
        }

        /// <summary>
        /// Constructor for a <see cref="TreeNode"/>.
        /// <para>
        /// <b>Note:</b> DO NOT use this constructor when using this <see cref="TreeNode"/> in combination with an <see cref="IGraph{N}"/>.
        /// </para>
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="TreeNode"/>.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <see cref="TreeNode"/>s containing the children of this <see cref="TreeNode"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="children"/> is <see langword="null"/>.</exception>
        internal TreeNode(uint id, IEnumerable<TreeNode> children)
        {
            Utils.NullCheck(children, nameof(children), $"Trying to create a new instance of {GetType()} with predefined children, but the IEnumberable of children is null!");

            ID = id;
            InternalChildren = new List<TreeNode>();
            InternalUniqueChildren = new HashSet<TreeNode>();
            AddChildren(children);
        }

        /// <summary>
        /// Add another <see cref="TreeNode"/> as child to this <see cref="TreeNode"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.AddChild(N, N)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> to be added as child.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> is <see langword="null"/>.</exception>
        /// <exception cref="AddNeighbourToSelfException">Thrown when parameter <paramref name="child"/> is the same <see cref="TreeNode"/> as the <see cref="TreeNode"/> this method is called from.</exception>
        /// <exception cref="AddParentAsChildException">Thrown when parameter <paramref name="child"/> is the parent of the <see cref="TreeNode"/> this method is called from.</exception>
        /// <exception cref="AlreadyANeighbourException">Thrown when parameter <paramref name="child"/> is already a child of the <see cref="TreeNode"/> this method is called from.</exception>
        public void AddChild(TreeNode child)
        {
            Utils.NullCheck(child, nameof(child), $"Trying to add a child to {this}, but child is null!");
            if (child == this)
            {
                throw new AddNeighbourToSelfException($"Trying to add {this} as child to itself!");
            }
            if (!(Parent is null) && child == Parent)
            {
                throw new AddParentAsChildException($"Trying to add the parent of {this} as child to {this}!");
            }
            if (InternalUniqueChildren.Contains(child))
            {
                throw new AlreadyANeighbourException($"Trying to add {child} as a child to {this}, but {child} is already a child of {this}!");
            }

            InternalChildren.Add(child);
            InternalUniqueChildren.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// Add multiple <see cref="TreeNode"/>s as children to this <see cref="TreeNode"/>. Uses <see cref="AddChild(TreeNode)"/> internally to add each child individually.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.AddChildren(N, IEnumerable{N})"/> instead.
        /// </para>
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> with children to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="IEnumerable{T}"/> parameter with children is <see langword="null"/>.</exception>
        public void AddChildren(IEnumerable<TreeNode> children)
        {
            Utils.NullCheck(children, nameof(children), $"Trying to add multiple children to {this}, but the IEnumerable of children is null!");

            foreach (TreeNode child in children)
            {
                AddChild(child);
            }
        }

        /// <summary>
        /// Remove one of the children of this <see cref="TreeNode"/> from its children.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.RemoveNode(N)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> to be removed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotANeighbourException">Thrown when <paramref name="child"/> is not a child of this <see cref="TreeNode"/>.</exception>
        public void RemoveChild(TreeNode child)
        {
            Utils.NullCheck(child, nameof(child), $"Trying to remove a child from {this}, but the child is null!");
            if (!InternalUniqueChildren.Contains(child))
            {
                throw new NotANeighbourException($"Trying to remove {child} from the children of {this}, but {child} is no neighbour of {this}!");
            }

            if (child.Parent == this)
            {
                child.Parent = null;
            }
            InternalChildren.Remove(child); 
            InternalUniqueChildren.Remove(child);
        }

        /// <summary>
        /// Remove multiple <see cref="TreeNode"/>s from the children of this <see cref="TreeNode"/>. Uses <see cref="RemoveChild(TreeNode)"/> internally to remove each child individually.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.RemoveNodes(IEnumerable{N})"/> instead.
        /// </para>
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> with children to be removed.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="IEnumerable{T}"/> parameter with children is <see langword="null"/>.</exception>
        public void RemoveChildren(IEnumerable<TreeNode> children)
        {
            Utils.NullCheck(children, nameof(children), $"Trying to remove multiple children from {this}, but the IEnumerable with children is null!");

            foreach (TreeNode child in children)
            {
                RemoveChild(child);
            }
        }

        /// <summary>
        /// Remove all children from this <see cref="TreeNode"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.RemoveNodes(IEnumerable{N})"/> instead.
        /// </para>
        /// </summary>
        public void RemoveAllChildren()
        {
            foreach (TreeNode child in InternalChildren)
            {
                if (child.Parent == this)
                {
                    child.Parent = null;
                }
            }
            InternalChildren.Clear();
            InternalUniqueChildren.Clear();
        }

        /// <summary>
        /// Checks whether <paramref name="node"/> is a child of this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> for which we want to check if it is a neighbour of this <see cref="TreeNode"/>.</param>
        /// <returns><see langword="true"/> is <paramref name="node"/> is a child of this <see cref="TreeNode"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        public bool HasChild(TreeNode node)
        {
            Utils.NullCheck(node, nameof(node), $"Trying to see if a node is a child of {this}, but node is null!");

            return InternalUniqueChildren.Contains(node);
        }

        /// <summary>
        /// Checks whether this <see cref="TreeNode"/> is the root of the <see cref="ITree{N}"/> it is in.
        /// <br/>
        /// It checks whether the parent of this <see cref="TreeNode"/> is <see langword="null"/>.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="TreeNode"/> is the root of its <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        public bool IsRoot()
        {
            return Parent is null;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="TreeNode"/>.
        /// <br/>
        /// Looks like: "TreeNode [ID]", where "[ID]" is the <see cref="ID"/> of this <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of this <see cref="TreeNode"/>.</returns>
        public override string ToString()
        {
            return $"TreeNode {ID}";
        }

        /// <summary>
        /// Calls <see cref="AddChild(TreeNode)"/>.
        /// </summary>
        /// <param name="neighbour"><inheritdoc cref="AddChild(TreeNode)"/></param>
        /// <param name="directed">Unused.</param>
        void INode<TreeNode>.AddNeighbour(TreeNode neighbour, bool directed)
        {
            AddChild(neighbour);
        }

        /// <summary>
        /// Calls <see cref="AddChildren(IEnumerable{TreeNode})"/>.
        /// </summary>
        /// <param name="neighbours"><inheritdoc cref="AddChildren(IEnumerable{TreeNode})"/></param>
        /// <param name="directed">Unused.</param>
        void INode<TreeNode>.AddNeighbours(IEnumerable<TreeNode> neighbours, bool directed)
        {
            AddChildren(neighbours);
        }

        /// <summary>
        /// Removes all children and parent from this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="directed">Unused.</param>
        void INode<TreeNode>.RemoveAllNeighbours(bool directed)
        {
            RemoveAllChildren();
            Parent.RemoveChild(this);
        }

        /// <summary>
        /// Remove a <see cref="TreeNode"/> from the neighbours of this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="neighbour">The neighbour to be removed.</param>
        /// <param name="directed">Unused.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbour"/> is <see langword="null"/>.</exception>
        void INode<TreeNode>.RemoveNeighbour(TreeNode neighbour, bool directed)
        {
            Utils.NullCheck(neighbour, nameof(neighbour), $"Trying to remove a neighbour from {this}, but the neighbour is null!");

            if (neighbour == Parent)
            {
                Parent.RemoveChild(this);
            }
            else
            {
                RemoveChild(neighbour);
            }
        }

        /// <summary>
        /// Remove multiple <see cref="TreeNode"/>s from the neighbours of this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="neighbours"></param>
        /// <param name="directed"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbours"/> is <see langword="null"/>.</exception>
        void INode<TreeNode>.RemoveNeighbours(IEnumerable<TreeNode> neighbours, bool directed)
        {
            Utils.NullCheck(neighbours, nameof(neighbours), $"Trying to remove multiple neighbours from {this}, but the IEnumerable with neighbours is null!");

            foreach (TreeNode neighbour in neighbours)
            {
                ((INode<TreeNode>)this).RemoveNeighbour(neighbour);
            }
        }

        /// <summary>
        /// Checks whether <paramref name="node"/> is either the <see cref="Parent"/> or a child of this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> for which we want to know if it is a neighbour of this <see cref="TreeNode"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is the parent or a child of this <see cref="TreeNode"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        bool INode<TreeNode>.HasNeighbour(TreeNode node)
        {
            Utils.NullCheck(node, nameof(node), $"Trying to find out whether a node is a neighbour of {this}, but the node is null!");

            return node == Parent || HasChild(node);
        }
    }
}
