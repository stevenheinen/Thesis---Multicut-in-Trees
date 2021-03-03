// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MulticutInTrees.CountedDatastructures;
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
        /// The <see cref="CountedCollection{T}"/> with children of this <see cref="TreeNode"/>.
        /// </summary>
        private CountedCollection<TreeNode> InternalChildren { get; }

        // todo: should have counter... Also, when enumerating over these a counter should be used
        /// <summary>
        /// The <see cref="IEnumerable{T}"/> with children of this <see cref="TreeNode"/>.
        /// </summary>
        public IEnumerable<TreeNode> Children => InternalChildren.GetLinkedList();
        
        /// <summary>
        /// The <see cref="NodeType"/> of this <see cref="TreeNode"/>.
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for things that should not impact the performance of an <see cref="Algorithms.Algorithm"/> or <see cref="ReductionRules.ReductionRule"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// The internal representation for the parent of this <see cref="TreeNode"/>.
        /// </summary>
        private TreeNode Parent { get; set;}

        // todo: when enumerating over these a counter should be used
        // todo: comment, nullcheck, move
        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of all neighbours of this <see cref="TreeNode"/>. Includes <see cref="Parent"/> and <see cref="Children"/>. Cannot be edited directly.
        /// <br/>
        /// When using this <see cref="TreeNode"/> in combination with an <see cref="ITree{N}"/>, refer to <seealso cref="ITree{TreeNode}.AddChild(TreeNode, TreeNode, Counter)"/>, <seealso cref="ITree{TreeNode}.AddChildren(TreeNode, IEnumerable{TreeNode}, Counter)"/>, <seealso cref="ITree{TreeNode}.RemoveNode(TreeNode, Counter)"/> and <seealso cref="ITree{TreeNode}.RemoveNodes(IEnumerable{TreeNode}, Counter)"/>.
        /// <br/>
        /// When using this <see cref="TreeNode"/> without and <see cref="ITree{N}"/>, refer to <seealso cref="AddChild(TreeNode, Counter)"/>, <seealso cref="AddChildren(IEnumerable{TreeNode}, Counter)"/>, <seealso cref="RemoveChild(TreeNode, Counter)"/>, <seealso cref="RemoveChildren(IEnumerable{TreeNode}, Counter)"/> and <seealso cref="RemoveAllChildren"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        public IEnumerable<TreeNode> Neighbours(Counter counter)
        {
            _ = counter++;

            if (Parent is null)
            {
                return InternalChildren.GetLinkedList();
            }

            List<TreeNode> neighbours = new List<TreeNode>(InternalChildren.GetLinkedList())
            {
                Parent
            };
            return neighbours.AsReadOnly();
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
            InternalChildren = new CountedCollection<TreeNode>();
            Type = NodeType.Other;
            MockCounter = new Counter();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public TreeNode GetParent(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the root of a TreeNode, but the counter is null!");
#endif            
            _ = counter++;
            return Parent;
        }

        /// <summary>
        /// Set the parent of this <see cref="TreeNode"/> to <paramref name="newParent"/>.
        /// </summary>
        /// <param name="newParent">The <see cref="TreeNode"/> that will become the new parent of this <see cref="TreeNode"/>.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        private void SetParent(TreeNode newParent, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to set the root of a TreeNode, but the counter is null!");
#endif
            _ = counter++;
            Parent = newParent;
        }

        /// <summary>
        /// The number of neighbours (children + parent) this <see cref="TreeNode"/> has.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int Degree(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the degree of a TreeNode, but the counter is null!");
#endif
            if (Parent is null)
            {
                return InternalChildren.Count(counter);
            }
            return InternalChildren.Count(counter) + 1;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int DepthFromRoot(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the depth of a TreeNode, but the counter is null!");
#endif
            if (Parent is null)
            {
                _ = counter++;
                return 0;
            }
            return Parent.DepthFromRoot(counter) + 1;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int HeightOfSubtree(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the height of the subtree of a TreeNode, but the counter is null!");
#endif
            _ = counter++;
            if (InternalChildren.Count(counter) == 0)
            {
                return 0;
            }
            int maxDepth = 0;
            foreach (TreeNode child in InternalChildren.GetCountedEnumerable(MockCounter))
            {
                maxDepth = Math.Max(maxDepth, child.HeightOfSubtree(MockCounter));
            }
            return maxDepth + 1;
        }

        /// <summary>
        /// Add another <see cref="TreeNode"/> as child to this <see cref="TreeNode"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.AddChild(N, N, Counter)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> to be added as child.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="AddNeighbourToSelfException">Thrown when parameter <paramref name="child"/> is the same <see cref="TreeNode"/> as the <see cref="TreeNode"/> this method is called from.</exception>
        /// <exception cref="AddParentAsChildException">Thrown when parameter <paramref name="child"/> is the parent of the <see cref="TreeNode"/> this method is called from.</exception>
        /// <exception cref="AlreadyANeighbourException">Thrown when parameter <paramref name="child"/> is already a child of the <see cref="TreeNode"/> this method is called from.</exception>
        public void AddChild(TreeNode child, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(child, nameof(child), $"Trying to add a child to {this}, but child is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add a child to {this}, but the counter is null!");
            if (child == this)
            {
                throw new AddNeighbourToSelfException($"Trying to add {this} as child to itself!");
            }
            if (!(Parent is null) && child == Parent)
            {
                throw new AddParentAsChildException($"Trying to add the parent of {this} as child to {this}!");
            }
            if (InternalChildren.Contains(child, MockCounter))
            {
                throw new AlreadyANeighbourException($"Trying to add {child} as a child to {this}, but {child} is already a child of {this}!");
            }
#endif
            InternalChildren.Add(child, counter);
            child.SetParent(this, counter);
        }

        /// <summary>
        /// Add multiple <see cref="TreeNode"/>s as children to this <see cref="TreeNode"/>. Uses <see cref="AddChild(TreeNode, Counter)"/> internally to add each child individually.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.AddChildren(N, IEnumerable{N}, Counter)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> with children to be added.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="children"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void AddChildren(IEnumerable<TreeNode> children, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(children, nameof(children), $"Trying to add multiple children to {this}, but the IEnumerable of children is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add multiple children to {this}, but the counter is null!");
#endif
            foreach (TreeNode child in children)
            {
                AddChild(child, counter);
            }
        }

        /// <summary>
        /// Remove one of the children of this <see cref="TreeNode"/> from its children.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.RemoveNode(N, Counter)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> to be removed.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotANeighbourException">Thrown when <paramref name="child"/> is not a child of this <see cref="TreeNode"/>.</exception>
        public void RemoveChild(TreeNode child, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(child, nameof(child), $"Trying to remove a child from {this}, but the child is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove a child from {this}, but the counter is null!");
            if (!InternalChildren.Contains(child, MockCounter))
            {
                throw new NotANeighbourException($"Trying to remove {child} from the children of {this}, but {child} is no neighbour of {this}!");
            }
#endif
            if (child.GetParent(counter) == this)
            {
                child.SetParent(null, counter);
            }
            InternalChildren.Remove(child, counter); 
        }

        /// <summary>
        /// Remove multiple <see cref="TreeNode"/>s from the children of this <see cref="TreeNode"/>. Uses <see cref="RemoveChild(TreeNode, Counter)"/> internally to remove each child individually.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.RemoveNodes(IEnumerable{N}, Counter)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="children">The <see cref="IEnumerable{T}"/> with children to be removed.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="children"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveChildren(IEnumerable<TreeNode> children, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(children, nameof(children), $"Trying to remove multiple children from {this}, but the IEnumerable with children is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove multiple children from {this}, but the counter is null!");
#endif
            foreach (TreeNode child in children)
            {
                RemoveChild(child, counter);
            }
        }

        /// <summary>
        /// Remove all children from this <see cref="TreeNode"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="TreeNode"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="ITree{N}.RemoveNodes(IEnumerable{N}, Counter)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveAllChildren(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove all children from {this}, but the counter is null!");
#endif
            foreach (TreeNode child in InternalChildren.GetCountedEnumerable(counter))
            {
                if (child.GetParent(counter) == this)
                {
                    child.SetParent(null, counter);
                }
            }
            InternalChildren.Clear(counter);
        }

        /// <summary>
        /// Checks whether <paramref name="node"/> is a child of this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> for which we want to check if it is a neighbour of this <see cref="TreeNode"/>.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> is <paramref name="node"/> is a child of this <see cref="TreeNode"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool HasChild(TreeNode node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to see if a node is a child of {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to see if a node is a child of {this}, but the counter is null!");
#endif
            return InternalChildren.Contains(node, counter);
        }

        /// <summary>
        /// Checks whether this <see cref="TreeNode"/> is the root of the <see cref="ITree{N}"/> it is in.
        /// <br/>
        /// It checks whether the parent of this <see cref="TreeNode"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="TreeNode"/> is the root of its <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool IsRoot(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), $"Trying to check if {this} is a root, but the counter is null!");
#endif
            _ = counter++;
            return Parent is null;
        }

        /// <summary>
        /// Finds all ancestors for this <see cref="TreeNode"/>. Includes this <see cref="TreeNode"/> itself.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> with <see cref="TreeNode"/>s that are ancestors of this <see cref="TreeNode"/>. Ordered from this <see cref="TreeNode"/> to the root.</returns>
        public List<TreeNode> FindAllAncestors()
        {
            List<TreeNode> ancestors = new List<TreeNode>() { this };
            TreeNode parent = this;
            while ((parent = parent.GetParent(MockCounter)) != null)
            {
                ancestors.Add(parent);
            }
            return ancestors;
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
        /// Calls <see cref="AddChild(TreeNode, Counter)"/>.
        /// </summary>
        /// <param name="neighbour"><inheritdoc cref="AddChild(TreeNode, Counter)"/></param>
        /// <param name="directed">Unused.</param>
        void INode<TreeNode>.AddNeighbour(TreeNode neighbour, bool directed)
        {
            AddChild(neighbour, MockCounter);
        }

        /// <summary>
        /// Calls <see cref="AddChildren(IEnumerable{TreeNode}, Counter)"/>.
        /// </summary>
        /// <param name="neighbours"><inheritdoc cref="AddChildren(IEnumerable{TreeNode}, Counter)"/></param>
        /// <param name="directed">Unused.</param>
        void INode<TreeNode>.AddNeighbours(IEnumerable<TreeNode> neighbours, bool directed)
        {
            AddChildren(neighbours, MockCounter);
        }

        /// <summary>
        /// Removes all children and parent from this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="directed">Unused.</param>
        void INode<TreeNode>.RemoveAllNeighbours(bool directed)
        {
            RemoveAllChildren(MockCounter);
            Parent.RemoveChild(this, MockCounter);
        }

        /// <summary>
        /// Remove a <see cref="TreeNode"/> from the neighbours of this <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="neighbour">The neighbour to be removed.</param>
        /// <param name="directed">Unused.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbour"/> is <see langword="null"/>.</exception>
        void INode<TreeNode>.RemoveNeighbour(TreeNode neighbour, bool directed)
        {
#if !EXPERIMENT
            Utils.NullCheck(neighbour, nameof(neighbour), $"Trying to remove a neighbour from {this}, but the neighbour is null!");
#endif
            if (neighbour == Parent)
            {
                Parent.RemoveChild(this, MockCounter);
            }
            else
            {
                RemoveChild(neighbour, MockCounter);
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
#if !EXPERIMENT
            Utils.NullCheck(neighbours, nameof(neighbours), $"Trying to remove multiple neighbours from {this}, but the IEnumerable with neighbours is null!");
#endif
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
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to find out whether a node is a neighbour of {this}, but the node is null!");
#endif
            return node == Parent || HasChild(node, MockCounter);
        }
    }
}
