// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// A tree that implements <see cref="ITree{N}"/> and consists of a class that implements <see cref="ITreeNode{T}"/>.
    /// </summary>
    public class Tree<N> : ITree<N> where N : ITreeNode<N>
    {
        ///// <summary>
        ///// The internal <see cref="List{T}"/> of edges in this <see cref="Tree{N}"/>.
        ///// </summary>
        //protected List<(N, N)> InternalEdges { get; set; }

        ///// <summary>
        ///// The internal <see cref="HashSet{T}"/> of edges in this <see cref="Tree{N}"/>.
        ///// </summary>
        //protected HashSet<(N, N)> UniqueInternalEdges { get; set; }

        ///// <summary>
        ///// The internal <see cref="HashSet{T}"/> of nodes in this <see cref="Tree{N}"/>
        ///// </summary>
        //protected HashSet<N> UniqueInternalNodes { get; set; }

        ///// <summary>
        ///// The internal <see cref="List{T}"/> of nodes in this <see cref="Tree{N}"/>.
        ///// </summary>
        //protected List<N> InternalNodes { get; set; }

        /// <summary>
        /// The <see cref="CountedCollection{T}"/> of edges in this <see cref="Tree{N}"/>.
        /// </summary>
        protected CountedCollection<(N, N)> InternalEdges { get; set; }

        /// <summary>
        /// The <see cref="CountedCollection{T}"/> of nodes in this <see cref="Tree{N}"/>.
        /// </summary>
        protected CountedCollection<N> InternalNodes { get; set; }

        /// <summary>
        /// The number of nodes in this <see cref="Tree{N}"/>.
        /// </summary>
        public int NumberOfNodes => InternalNodes.Count;

        /// <summary>
        /// The number of edges in this <see cref="Tree{N}"/>.
        /// </summary>
        public int NumberOfEdges => InternalEdges.Count;

        /// <summary>
        /// The publically visible collection of edges in this <see cref="Tree{N}"/>. Edges cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddRoot(N)"/>, <seealso cref="AddChild(N, N)"/>, <seealso cref="AddChildren(N, IEnumerable{N})"/>, <seealso cref="RemoveNode(N)"/> and <seealso cref="RemoveNodes(IEnumerable{N})"/>.
        /// </summary>
        public IEnumerable<(N, N)> Edges => InternalEdges.GetLinkedList();

        /// <summary>
        /// The publically visible collection of nodes in this <see cref="Tree{N}"/>. Nodes cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddRoot(N)"/>, <seealso cref="AddChild(N, N)"/>, <seealso cref="AddChildren(N, IEnumerable{N})"/>, <seealso cref="RemoveNode(N)"/> and <seealso cref="RemoveNodes(IEnumerable{N})"/>.
        /// </summary>
        public IEnumerable<N> Nodes => InternalNodes.GetLinkedList();

        /// <summary>
        /// The root of this <see cref="Tree{N}"/>.
        /// </summary>
        public N Root { get; private set; }

        /// <summary>
        /// The depth of this <see cref="Tree{N}"/>. Equal to the depth of the subtree of the <see cref="Root"/> of this <see cref="Tree{N}"/>.
        /// </summary>
        public int Height
        {
            get
            {
                if (Root is null)
                {
                    return 0;
                }
                return Root.HeightOfSubtree;
            }
        }

        /// <summary>
        /// Constructor for a <see cref="Tree{N}"/>.
        /// </summary>
        public Tree()
        {
            InternalNodes = new CountedCollection<N>();
            InternalEdges = new CountedCollection<(N, N)>();

            /*
            InternalNodes = new List<N>();
            UniqueInternalNodes = new HashSet<N>();
            InternalEdges = new List<(N, N)>();
            UniqueInternalEdges = new HashSet<(N, N)>();
            */
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="Graph{N}"/>.
        /// Looks like "Tree with n nodes, m edges and height h", where "n", "m" and "h" are numbers.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Graph{N}"/>.</returns>
        public override string ToString()
        {
            return $"Tree with {NumberOfNodes} nodes, {NumberOfEdges} edges and height {Height}.";
        }

        /// <summary>
        /// Get the root with information from <see cref="Nodes"/>.
        /// </summary>
        /// <returns>The first occurance of an <typeparamref name="N"/> in <see cref="Nodes"/> that is a root.</returns>
        /// <exception cref="NoRootException">Thrown when there is no root in <see cref="Nodes"/>.</exception>
        /// <exception cref="MultipleRootsException">Thrown when there are multiple roots in <see cref="Nodes"/>.</exception>
        private N FindRoot()
        {
            int numberOfRoots = Nodes.Count(n => n.Parent is null);
            if (numberOfRoots == 0)
            {
                throw new NoRootException($"Trying to find a root in {this}, but there is none!");
            }
            if (numberOfRoots > 1)
            {
                throw new MultipleRootsException($"Trying to update the root of {this}, but there are multiple roots!");
            }

            return Nodes.First(n => n.Parent is null);
        }

        /// <summary>
        /// Adds the children of <paramref name="node"/> as children to the parent of <paramref name="node"/>, and removes <paramref name="node"/> from this <see cref="Tree{N}"/>.
        /// <br/>
        /// If <paramref name="node"/> is the root of this <see cref="Tree{N}"/>, and it has a single child, its child will become the root. Otherwise, an <see cref="MultipleRootsException"/> is thrown.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> whose children we want to connect to its parent.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        /// <exception cref="MultipleRootsException">Thrown when <paramref name="node"/> is the root and has multiple children.</exception>
        private void AddChildrenToParent(N node)
        {
            Utils.NullCheck(node, nameof(node), "Trying to add the children of a node to its parent, but the node is null!");

            if (node.Equals(Root))
            {
                if (node.Children.Count > 1)
                {
                    throw new MultipleRootsException($"Trying to add the children of {node} to its parent, but {node} is the root of {this} and has more than 1 child ({node.Children.Count})!");
                }

                InternalEdges.Remove((Root, node));
                if (node.Children.Count == 1)
                {
                    Root = node.Children[0];
                }
                else
                {
                    Root = default;
                }
            }
            else
            {
                InternalEdges.Remove((node.Parent, node));
                //UniqueInternalEdges.Remove((node.Parent, node));

                //InternalEdges.RemoveAll(edge => edge.Item1.Equals(node));
                foreach (N child in node.Children) 
                {
                    InternalEdges.ChangeElement((node, child), (node.Parent, child));

                    /*
                    UniqueInternalEdges.Remove((node, child));
                    InternalEdges.Add((node.Parent, child));
                    UniqueInternalEdges.Add((node.Parent, child));
                    */
                }

                node.Parent.AddChildren(node.Children);
                node.RemoveAllChildren();
            }
        }

        /// <summary>
        /// Finds whether <paramref name="node"/> is part of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> for which we want to know if it is part of this <see cref="Tree{N}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="Tree{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        public bool HasNode(N node)
        {
            Utils.NullCheck(node, nameof(node), $"Trying to see if a node is in {this}, but the node is null!");

            return InternalNodes.Contains(node);
        }

        /// <summary>
        /// Finds whether the edge between parameters <paramref name="parent"/> and <paramref name="child"/> is part of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <returns><see langword="true"/> if the edge between <paramref name="parent"/> and <paramref name="child"/> exists in this <see cref="Tree{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="parent"/> or <paramref name="child"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either <paramref name="parent"/> or <paramref name="child"/> is not part of this <see cref="Tree{N}"/>.</exception>
        public bool HasEdge(N parent, N child)
        {
            Utils.NullCheck(parent, nameof(parent), $"Trying to find out whether an edge exists in {this}, but the parent of the edge is null!");
            Utils.NullCheck(child, nameof(child), $"Trying to find out whether an edge exists in {this}, but the child of the edge is null!");
            if (!HasNode(parent))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the parent of the edge is not part of {this}!");
            }
            if (!HasNode(child))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the child of the edge is not part of {this}!");
            }

            return InternalEdges.Contains((parent, child));
        }

        /// <summary>
        /// Checks whether the edge <paramref name="edge"/> is part of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="edge">The <see cref="ValueTuple{T1, T2}"/> of <typeparamref name="N"/>s for which we want to know if it is part of this <see cref="Tree{N}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> exists in this <see cref="ITree{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <typeparamref name="N"/> of <paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either <typeparamref name="N"/> of <paramref name="edge"/> is not part of this <see cref="Tree{N}"/>.</exception>
        public bool HasEdge((N, N) edge)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), $"Trying to find out whether an edge exists in {this}, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), $"Trying to find out whether an edge exists in {this}, but the second endpoint of the edge is null!");
            if (!HasNode(edge.Item1))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the first endpoint of the edge is not part of {this}!");
            }
            if (!HasNode(edge.Item2))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the second endpoint of the edge is not part of {this}!");
            }

            N parent = edge.Item1;
            N child = edge.Item2;

            if (!parent.Equals(Root))
            {
                if (parent.Parent.Equals(child))
                {
                    parent = edge.Item2;
                    child = edge.Item1;
                }
            }

            return InternalEdges.Contains((parent, child));
        }


        /// <summary>
        /// Add <paramref name="newRoot"/> as root to this <see cref="Tree{N}"/>. The old root (if it exists) becomes a child of <paramref name="newRoot"/>.
        /// </summary>
        /// <param name="newRoot">The <typeparamref name="N"/> that will be the new root of this <see cref="Tree{N}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newRoot"/> is <see langword="null"/>.</exception>
        public void AddRoot(N newRoot)
        {
            Utils.NullCheck(newRoot, nameof(newRoot), $"Trying to add a new root to {this}, but the new root is null!");

            if (!(Root is null))
            {
                newRoot.AddChild(Root);
                InternalEdges.Add((newRoot, Root));
                //UniqueInternalEdges.Add((newRoot, Root));
            }

            InternalNodes.Add(newRoot);
            //UniqueInternalNodes.Add(newRoot);
            Root = newRoot;
        }

        /// <summary>
        /// Add a new <typeparamref name="N"/> as child to <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="N"/> that will be the parent of <paramref name="child"/>. Must already be part of this <see cref="Tree{N}"/>.</param>
        /// <param name="child">The <typeparamref name="N"/> that will be the new child of <paramref name="parent"/>. Must not be part of this <see cref="Tree{N}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> or <paramref name="child"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="parent"/> is not part of this <see cref="Tree{N}"/>.</exception>
        /// <exception cref="AlreadyInGraphException">Thrown when <paramref name="child"/> is already part of this <see cref="Tree{N}"/>.</exception>
        public void AddChild(N parent, N child)
        {
            Utils.NullCheck(parent, nameof(parent), $"Trying to add {child} as a child to a parent, but the parent is null!");
            Utils.NullCheck(child, nameof(child), $"Trying to add a child to {parent}, but the child is null!");
            if (!HasNode(parent))
            {
                throw new NotInGraphException($"Trying to add {child} as a child to {parent}, but {parent} is not part of {this}!");
            }
            if (HasNode(child))
            {
                throw new AlreadyInGraphException($"Trying to add {child} to {parent}, but {child} is already part of {this}!");
            }

            parent.AddChild(child);
            InternalNodes.Add(child);
            //UniqueInternalNodes.Add(child);
            (N, N) edge = (parent, child);
            InternalEdges.Add(edge);
            //UniqueInternalEdges.Add(edge);
        }

        /// <summary>
        /// Add multiple new <typeparamref name="N"/> as children to <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="N"/> that will be the parent of each <typeparamref name="N"/> in <paramref name="children"/>. Must already be part of this <see cref="Tree{N}"/>.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s that will be the new children of <paramref name="parent"/>. Must not be part of this <see cref="Tree{N}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="parent"/> or <paramref name="children"/> is <see langword="null"/>.</exception>
        public void AddChildren(N parent, IEnumerable<N> children)
        {
            Utils.NullCheck(parent, nameof(parent), "Trying to add multiple children to a parent, but the parent is null!");
            Utils.NullCheck(children, nameof(children), $"Trying to add multiple children to {parent}, but the IEnumerable of children is null!");

            foreach (N child in children)
            {
                AddChild(parent, child);
            }
        }

        /// <summary>
        /// Add all children of <paramref name="node"/> to its parent, and then deletes <paramref name="node"/> from this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="N"/> to be removed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="Tree{N}"/>.</exception>
        public void RemoveNode(N node)
        {
            Utils.NullCheck(node, nameof(node), $"Trying to remove a node from {this}, but the node is null!");
            if (!HasNode(node))
            {
                throw new NotInGraphException($"Trying to remove {node} from {this}, but {node} is not part of {this}!");
            }
            bool wasRoot = node.Equals(Root);

            AddChildrenToParent(node);

            if (!wasRoot)
            {
                node.Parent.RemoveChild(node);
                InternalEdges.Remove((node.Parent, node));
                //UniqueInternalEdges.Remove((node.Parent, node));
            }
            InternalNodes.Remove(node);
            //UniqueInternalNodes.Remove(node);
        }

        /// <summary>
        /// Remove muliple nodes from this <see cref="Tree{N}"/>. Children of the nodes to be removed are added to the parent of the node to be removed.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> of <typeparamref name="N"/>s to be removed from this <see cref="Tree{N}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> is <see langword="null"/>.</exception>
        public void RemoveNodes(IEnumerable<N> nodes)
        {
            Utils.NullCheck(nodes, nameof(nodes), $"Trying to remove multiple nodes from {this}, but the IEnumerable with nodes is null!");

            foreach (N node in nodes)
            {
                RemoveNode(node);
            }
        }

        /// <summary>
        /// Checks if this <see cref="Tree{N}"/> is valid: it has exactly 1 root, is acyclic, and is connected.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="Tree{N}"/> is valid, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            try
            {
                FindRoot();
            }
            catch (NoRootException)
            {
                return false;
            }
            catch (MultipleRootsException)
            {
                return false;
            }
            
            return DFS.IsAcyclicTree<Tree<N>, N>(this) && DFS.FindAllConnectedComponents(Nodes).Count == 1;
        }

        /// <summary>
        /// Update the <see cref="NodeType"/> for all the nodes in this <see cref="Tree{N}"/>.
        /// </summary>
        public void UpdateNodeTypes()
        {
            FindLeaves(out List<N> leaves, out HashSet<N> leafSet, out List<N> internalNodes);
            DetermineTypesOfInternalNodes(leafSet, internalNodes);
            DetermineTypesOfLeaves(leaves);
        }

        /// <summary>
        /// Find all the leaves in this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="leaves">The resulting <see cref="List{T}"/> with all leaves.</param>
        /// <param name="leafSet">The resulting <see cref="HashSet{T}"/> with all leaves.</param>
        /// <param name="internalNodes">The resulting <see cref="List{T}"/> with all internal nodes.</param>
        private void FindLeaves(out List<N> leaves, out HashSet<N> leafSet, out List<N> internalNodes)
        {
            leaves = new List<N>();
            leafSet = new HashSet<N>();
            internalNodes = new List<N>();

            foreach (N node in Nodes)
            {
                if (node.Neighbours.Count == 1)
                {
                    leaves.Add(node);
                    leafSet.Add(node);
                }
                else
                {
                    internalNodes.Add(node);
                }
            }
        }

        /// <summary>
        /// Determines the <see cref="NodeType"/> of all internal nodes.
        /// </summary>
        /// <param name="leaves">The <see cref="HashSet{T}"/> with all leaves.</param>
        /// <param name="internalNodes">The <see cref="List{T}"/> with internal nodes.</param>
        private void DetermineTypesOfInternalNodes(HashSet<N> leaves, List<N> internalNodes)
        {
            foreach (N node in internalNodes)
            {
                int internalNeighbours = node.Neighbours.Count(n => !leaves.Contains(n));
                if (internalNeighbours <= 1)
                {
                    node.Type = NodeType.I1;
                }
                else if (internalNeighbours == 2)
                {
                    node.Type = NodeType.I2;
                }
                else
                {
                    node.Type = NodeType.I3;
                }
            }
        }

        /// <summary>
        /// Determines the <see cref="NodeType"/> of all leaves.
        /// </summary>
        /// <param name="leaves">The <see cref="List{T}"/> with all leaves.</param>
        private void DetermineTypesOfLeaves(List<N> leaves)
        {
            foreach (N leaf in leaves)
            {
                N parent = leaf.Parent;
                if (parent is null)
                {
                    parent = leaf.Children[0];
                }

                if (parent.Type == NodeType.I1)
                {
                    leaf.Type = NodeType.L1;
                }
                else if (parent.Type == NodeType.I2)
                {
                    leaf.Type = NodeType.L2;
                }
                else
                {
                    leaf.Type = NodeType.L3;
                }
            }
        }
    }
}
