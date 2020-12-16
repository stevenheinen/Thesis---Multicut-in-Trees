// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MulticutInTrees.Exceptions;

namespace MulticutInTrees
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
        /// <value>The value of this identifier is given as paramter in the constructor.</value>
        public uint ID { get; }

        /// <inheritdoc/>
        public ReadOnlyCollection<TreeNode> Children => InternalChildren.AsReadOnly();

        /// <summary>
        /// The number of neighbours (children + parent) this <see cref="TreeNode"/> has.
        /// </summary>
        public int Degree
        {
            get
            {
                int degree = InternalChildren.Count;
                if (!(Parent is null))
                {
                    degree++;
                }
                return degree;
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
        public int DepthOfSubtree
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
                    maxDepth = Math.Max(maxDepth, child.DepthOfSubtree);
                }
                return maxDepth + 1;
            }
        }

        /// <summary>
        /// The internal representation for the parent of this <see cref="TreeNode"/>.
        /// </summary>
        public TreeNode Parent { get; private set; }

        /// <summary>
        /// Constructor for a <see cref="TreeNode"/>.
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
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="TreeNode"/>.</param>
        /// <param name="parent">The <see cref="TreeNode"/> that is the parent of the new <see cref="TreeNode"/> instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> is <see langword="null"/>.</exception>
        public TreeNode(uint id, TreeNode parent)
        {
            if (parent is null)
            {
                throw new ArgumentNullException("parent", $"Trying to create a new instance of {GetType()} with a predefined parent, but parent is null!");
            }

            ID = id;
            InternalChildren = new List<TreeNode>();
            InternalUniqueChildren = new HashSet<TreeNode>();
            parent.AddChild(this);
        }

        /// <summary>
        /// Constructor for a <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="TreeNode"/>.</param>
        /// <param name="parent">The <see cref="TreeNode"/> that is the parent of the new <see cref="TreeNode"/> instance.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <see cref="TreeNode"/>s containing the children of this <see cref="TreeNode"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> or <paramref name="children"/> is <see langword="null"/>.</exception>
        public TreeNode(uint id, TreeNode parent, IEnumerable<TreeNode> children)
        {
            if (parent is null)
            {
                throw new ArgumentNullException("parent", $"Trying to create a new instance of {GetType()} with a predefined parent, but parent is null!");
            }
            if (children is null)
            {
                throw new ArgumentNullException("children", $"Trying to create a new instance of {GetType()} with predefined children, but the IEnumberable of children is null!");
            }

            ID = id;
            InternalChildren = new List<TreeNode>();
            InternalUniqueChildren = new HashSet<TreeNode>();
            AddChildren(children);
            parent.AddChild(this);
        }

        /// <summary>
        /// Constructor for a <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="TreeNode"/>.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <see cref="TreeNode"/>s containing the children of this <see cref="TreeNode"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="children"/> is <see langword="null"/>.</exception>
        public TreeNode(uint id, IEnumerable<TreeNode> children)
        {
            if (children is null)
            {
                throw new ArgumentNullException("children", $"Trying to create a new instance of {GetType()} with predefined children, but the IEnumberable of children is null!");
            }

            ID = id;
            InternalChildren = new List<TreeNode>();
            InternalUniqueChildren = new HashSet<TreeNode>();
            AddChildren(children);
        }

        /// <summary>
        /// Add another <see cref="TreeNode"/> as child to this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> to be added as child.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> is <see langword="null"/>.</exception>
        /// <exception cref="AddNeighbourToSelfException">Thrown when parameter <paramref name="child"/> is the same <see cref="TreeNode"/> as the <see cref="TreeNode"/> this method is called from.</exception>
        /// <exception cref="AddParentAsChildException">Thrown when parameter <paramref name="child"/> is the parent of the <see cref="TreeNode"/> this method is called from.</exception>
        /// <exception cref="AlreadyANeighbourException">Thrown when parameter <paramref name="child"/> is already a child of the <see cref="TreeNode"/> this method is called from.</exception>
        public void AddChild(TreeNode child)
        {
            if (child is null)
            {
                throw new ArgumentNullException("child", $"Trying to add a child to {this}, but child is null!");
            }
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
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> with children to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="IEnumerable{T}"/> parameter with children is <see langword="null"/>.</exception>
        public void AddChildren(IEnumerable<TreeNode> children)
        {
            if (children is null)
            {
                throw new ArgumentNullException("children", $"Trying to add multiple children to {this}, but the IEnumerable of children is null!");
            }

            foreach (TreeNode child in children)
            {
                AddChild(child);
            }
        }

        /// <summary>
        /// Remove one of the children of this <see cref="TreeNode"/> from its children.
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> to be removed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotANeighbourException">Thrown when <paramref name="child"/> is not a child of this <see cref="TreeNode"/>.</exception>
        public void RemoveChild(TreeNode child)
        {
            if (child is null)
            {
                throw new ArgumentNullException("child", $"Trying to remove a child from {this}, but the child is null!");
            }
            if (!InternalUniqueChildren.Contains(child))
            {
                throw new NotANeighbourException($"Trying to remove {child} from the children of {this}, but {child} is no neighbour of {this}!");
            }

            child.Parent = null;
            InternalChildren.Remove(child); 
            InternalUniqueChildren.Remove(child);
        }

        /// <summary>
        /// Remove multiple <see cref="TreeNode"/>s from the children of this <see cref="TreeNode"/>. Uses <see cref="RemoveChild(TreeNode)"/> internally to remove each child individually.
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> with children to be removed.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="IEnumerable{T}"/> parameter with children is <see langword="null"/>.</exception>
        public void RemoveChildren(IEnumerable<TreeNode> children)
        {
            if (children is null)
            {
                throw new ArgumentNullException("children", $"Trying to remove multiple children from {this}, but the IEnumerable with children is null!");
            }

            foreach (TreeNode child in children)
            {
                RemoveChild(child);
            }
        }

        /// <summary>
        /// Remove all children from this <see cref="TreeNode"/>.
        /// </summary>
        public void RemoveAllChildren()
        {
            foreach (TreeNode child in InternalChildren)
            {
                child.Parent = null;
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
            if (node is null)
            {
                throw new ArgumentNullException("node", $"Trying to see if a node is a child of {this}, but node is null!");
            }

            return InternalUniqueChildren.Contains(node);
        }

        /// <summary>
        /// Indicates whether the current <see cref="TreeNode"/> is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare to the current <see cref="TreeNode"/>.</param>
        /// <returns><see langword="true"/> if the current <see cref="TreeNode"/> is equal to <paramref name="obj"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <see langword="null"/>.</exception>
        /// <exception cref="IncompatibleTypesException">Thrown when the type of <paramref name="obj"/> cannot be compared to a <see cref="TreeNode"/>.</exception>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException("obj", $"Trying to compare {this} to null!");
            }
            if (obj.GetType() != typeof(TreeNode))
            {
                throw new IncompatibleTypesException($"Type of {obj} (type: {obj.GetType()}) cannot be compared to {this} (type: {typeof(TreeNode)})!");
            }

            return Equals((TreeNode)obj);
        }

        /// <summary>
        /// Indicates whether the current <see cref="TreeNode"/> is equal to the <see cref="TreeNode"/> <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="TreeNode"/> to compare to the current <see cref="TreeNode"/>.</param>
        /// <returns><see langword="true"/> if the current <see cref="TreeNode"/> is equal to <paramref name="other"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is <see langword="null"/>.</exception>
        public bool Equals(ITreeNode<TreeNode> other)
        {
            if (other is null)
            {
                throw new ArgumentNullException("other", $"Trying to compare {this} to null!");
            }

            return ID.Equals(other.ID);
        }

        /// <summary>
        /// Checks if the current <see cref="TreeNode"/> is the root of its tree.
        /// </summary>
        /// <returns><see langword="true"/> is this <see cref="TreeNode"/> is the root of its tree, <see langword="false"/> otherwise.</returns>
        public bool IsRoot()
        {
            return Parent is null;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representing the current object.
        /// </summary>
        /// <returns>The <see cref="string"/> "TreeNode [ID]", where [ID] is <see cref="ID"/>.</returns>
        public override string ToString()
        {
            return $"TreeNode {ID}";
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /// <summary>
        /// Checks two <see cref="TreeNode"/>s for equality.
        /// </summary>
        /// <param name="lhs">The first <see cref="TreeNode"/>.</param>
        /// <param name="rhs">The second <see cref="TreeNode"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lhs"/> or <paramref name="rhs"/> is <see langword="null"/>.</exception>
        public static bool operator ==(TreeNode lhs, TreeNode rhs)
        {
            if (lhs is null)
            {
                throw new ArgumentNullException("lhs", "Left hand side of == operator for TreeNode is null!");
            }
            if (rhs is null)
            {
                throw new ArgumentNullException("rhs", "Right hand side of == operator for TreeNode is null!");
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Checks two <see cref="TreeNode"/>s for inequality.
        /// </summary>
        /// <param name="lhs">The first <see cref="TreeNode"/>.</param>
        /// <param name="rhs">The second <see cref="TreeNode"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lhs"/> or <paramref name="rhs"/> is <see langword="null"/>.</exception>
        public static bool operator !=(TreeNode lhs, TreeNode rhs)
        {
            if (lhs is null)
            {
                throw new ArgumentNullException("lhs", "Left hand side of != operator for TreeNode is null!");
            }
            if (rhs is null)
            {
                throw new ArgumentNullException("rhs", "Right hand side of != operator for TreeNode is null!");
            }

            return !lhs.Equals(rhs);
        }
    }
}
