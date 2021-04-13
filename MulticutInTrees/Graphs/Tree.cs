// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// A tree that implements <see cref="ITree{N}"/> and consists of a class that implements <see cref="ITreeNode{T}"/>.
    /// </summary>
    public class Tree<TNode> : ITree<TNode> where TNode : ITreeNode<TNode>
    {
        /// <summary>
        /// The <see cref="CountedCollection{T}"/> of edges in this <see cref="Tree{N}"/>.
        /// </summary>
        private CountedCollection<(TNode, TNode)> InternalEdges { get; }

        /// <summary>
        /// The <see cref="CountedCollection{T}"/> of nodes in this <see cref="Tree{N}"/>.
        /// </summary>
        private CountedCollection<TNode> InternalNodes { get; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for modifications that should not impact the performance of an <see cref="Algorithms.Algorithm"/> or <see cref="ReductionRules.ReductionRule"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// The root of this <see cref="Tree{N}"/>.
        /// </summary>
        private TNode Root { get; set; }

        /// <summary>
        /// Constructor for a <see cref="Tree{N}"/>.
        /// </summary>
        public Tree()
        {
            InternalNodes = new CountedCollection<TNode>();
            InternalEdges = new CountedCollection<(TNode, TNode)>();
            MockCounter = new Counter();
        }

        /// <summary>
        /// The publically visible collection of edges in this <see cref="Tree{N}"/>. Edges cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddRoot(TNode, Counter)"/>, <seealso cref="AddChild(TNode, TNode, Counter)"/>, <seealso cref="AddChildren(TNode, IEnumerable{TNode}, Counter)"/>, <seealso cref="RemoveNode(TNode, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{TNode}, Counter)"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>+
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all edges in this <see cref="Tree{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<(TNode, TNode)> Edges(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the edges of a tree, but the counter is null!");
#endif
            _ = counter++;
            return new CountedEnumerable<(TNode, TNode)>(InternalEdges.GetLinkedList(), counter);
        }

        /// <summary>
        /// The publically visible collection of nodes in this <see cref="Tree{N}"/>. Nodes cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddRoot(TNode, Counter)"/>, <seealso cref="AddChild(TNode, TNode, Counter)"/>, <seealso cref="AddChildren(TNode, IEnumerable{TNode}, Counter)"/>, <seealso cref="RemoveNode(TNode, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{TNode}, Counter)"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all nodes in this <see cref="Tree{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<TNode> Nodes(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the nodes of a tree, but the counter is null!");
#endif
            _ = counter++;
            return new CountedEnumerable<TNode>(InternalNodes.GetLinkedList(), counter);
        }

        /// <summary>
        /// The height of this <see cref="Tree{N}"/>. Equal to the height of the subtree of the <see cref="Root"/> of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that is the height of this <see cref="Tree{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int Height(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the height of a tree, but the counter is null!");
#endif
            if (Root is null)
            {
                _ = counter++;
                return 0;
            }
            return Root.HeightOfSubtree(counter);
        }

        /// <summary>
        /// Returns the root of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>The <typeparamref name="TNode"/> that is the root of this <see cref="Tree{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public TNode GetRoot(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the root of a tree, but the counter is null!");
#endif
            _ = counter++;
            return Root;
        }

        /// <summary>
        /// Sets the root of this <see cref="Tree{N}"/> to <paramref name="newRoot"/>.
        /// </summary>
        /// <param name="newRoot">The <typeparamref name="TNode"/> that will be the new root of this <see cref="Tree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newRoot"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        private void SetRoot(TNode newRoot, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(newRoot, nameof(newRoot), "Trying to set the root of a tree, but the new root is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to set the root of a tree, but the counter is null!");
#endif
            _ = counter++;
            Root = newRoot;
        }

        /// <summary>
        /// Returns the number of nodes in this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that represents the number of nodes in this <see cref="Tree{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int NumberOfNodes(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the number of nodes in a tree, but the counter is null!");
#endif
            return InternalNodes.Count(counter);
        }

        /// <summary>
        /// Returns the number of edges in this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that represents the number of edges in this <see cref="Tree{N}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int NumberOfEdges(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the number of edges in a tree, but the counter is null!");
#endif
            return InternalEdges.Count(counter);
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="Graph{N}"/>.
        /// Looks like "Tree with n nodes, m edges and height h", where "n", "m" and "h" are numbers.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Graph{N}"/>.</returns>
        public override string ToString()
        {
            return $"Tree with {NumberOfNodes(MockCounter)} nodes, {NumberOfEdges(MockCounter)} edges and height {Height(MockCounter)}.";
        }

        /// <summary>
        /// Get the root with information from <see cref="Nodes"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>The first occurance of an <typeparamref name="TNode"/> in <see cref="Nodes"/> that is a root.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NoRootException">Thrown when there is no root in <see cref="Nodes"/>.</exception>
        /// <exception cref="MultipleRootsException">Thrown when there are multiple roots in <see cref="Nodes"/>.</exception>
        private TNode FindRoot(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to find the root of a tree, but the counter is null!");
            int numberOfRoots = InternalNodes.Count(n => n.GetParent(counter) is null, counter);
            if (numberOfRoots == 0)
            {
                throw new NoRootException($"Trying to find a root in {this}, but there is none!");
            }
            if (numberOfRoots > 1)
            {
                throw new MultipleRootsException($"Trying to update the root of {this}, but there are multiple roots!");
            }
#endif
            return InternalNodes.First(n => n.GetParent(counter) is null, counter);
        }

        /// <summary>
        /// Adds the children of <paramref name="node"/> as children to the parent of <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> whose children we want to connect to its parent.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="MultipleRootsException">Thrown when <paramref name="node"/> is the root of this <see cref="Tree{N}"/>.</exception>
        private void AddChildrenToParent(TNode node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), "Trying to add the children of a node to its parent, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to add the children of a node to its parent, but the counter is null!");
            if (node.Equals(Root))
            {
                throw new NotSupportedException($"Trying to add the children of {node} to its parent, but {node} is the root of its tree!");
            }
#endif

            if (!node.Children(counter).Any())
            {
                return;
            }

            TNode parent = node.GetParent(counter);
            foreach (TNode child in node.Children(counter))
            {
                InternalEdges.ChangeElement((node, child), (parent, child), counter);
            }
            parent.AddChildren(node.Children(counter), counter);
            node.RemoveAllChildren(counter);
        }

        /// <summary>
        /// Finds whether <paramref name="node"/> is part of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> for which we want to know if it is part of this <see cref="Tree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="Tree{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool HasNode(TNode node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to see if a node is in {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to see if a node is in {this}, but the counter is null!");
#endif
            return InternalNodes.Contains(node, counter);
        }

        /// <summary>
        /// Finds whether the edge between parameters <paramref name="parent"/> and <paramref name="child"/> is part of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <returns><see langword="true"/> if the edge between <paramref name="parent"/> and <paramref name="child"/> exists in this <see cref="Tree{N}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="parent"/>, <paramref name="child"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either <paramref name="parent"/> or <paramref name="child"/> is not part of this <see cref="Tree{N}"/>.</exception>
        public bool HasEdge(TNode parent, TNode child, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(parent, nameof(parent), $"Trying to find out whether an edge exists in {this}, but the parent of the edge is null!");
            Utils.NullCheck(child, nameof(child), $"Trying to find out whether an edge exists in {this}, but the child of the edge is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to find out whether an edge exists in {this}, but the counter is null!");
            if (!HasNode(parent, MockCounter))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the parent of the edge is not part of {this}!");
            }
            if (!HasNode(child, MockCounter))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the child of the edge is not part of {this}!");
            }
#endif
            return InternalEdges.Contains((parent, child), counter);
        }

        /// <summary>
        /// Checks whether the edge <paramref name="edge"/> is part of this <see cref="Tree{N}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either <typeparamref name="TNode"/> of <paramref name="edge"/>, or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either <typeparamref name="TNode"/> of <paramref name="edge"/> is not part of this <see cref="Tree{N}"/>.</exception>
        public bool HasEdge((TNode, TNode) edge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), $"Trying to find out whether an edge exists in {this}, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), $"Trying to find out whether an edge exists in {this}, but the second endpoint of the edge is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to find out whether an edge exists in {this}, but the counter is null!");
            if (!HasNode(edge.Item1, MockCounter))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the first endpoint of the edge is not part of {this}!");
            }
            if (!HasNode(edge.Item2, MockCounter))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the second endpoint of the edge is not part of {this}!");
            }
#endif
            TNode parent = edge.Item1;
            TNode child = edge.Item2;

            if (!parent.Equals(Root))
            {
                if (parent.GetParent(MockCounter).Equals(child))
                {
                    parent = edge.Item2;
                    child = edge.Item1;
                }
            }

            return InternalEdges.Contains((parent, child), counter);
        }

        /// <summary>
        /// Add <paramref name="newRoot"/> as root to this <see cref="Tree{N}"/>. The old root (if it exists) becomes a child of <paramref name="newRoot"/>.
        /// </summary>
        /// <param name="newRoot">The <typeparamref name="TNode"/> that will be the new root of this <see cref="Tree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newRoot"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void AddRoot(TNode newRoot, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(newRoot, nameof(newRoot), $"Trying to add a new root to {this}, but the new root is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add a new root to {this}, but the counter is null!");
#endif
            if (!(GetRoot(counter) is null))
            {
                newRoot.AddChild(Root, counter);
                InternalEdges.Add((newRoot, Root), counter);
            }

            InternalNodes.Add(newRoot, counter);
            SetRoot(newRoot, counter);
        }

        /// <summary>
        /// Add a new <typeparamref name="TNode"/> as child to <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="TNode"/> that will be the parent of <paramref name="child"/>. Must already be part of this <see cref="Tree{N}"/>.</param>
        /// <param name="child">The <typeparamref name="TNode"/> that will be the new child of <paramref name="parent"/>. Must not be part of this <see cref="Tree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/>, <paramref name="child"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="parent"/> is not part of this <see cref="Tree{N}"/>.</exception>
        /// <exception cref="AlreadyInGraphException">Thrown when <paramref name="child"/> is already part of this <see cref="Tree{N}"/>.</exception>
        public void AddChild(TNode parent, TNode child, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(parent, nameof(parent), $"Trying to add {child} as a child to a parent, but the parent is null!");
            Utils.NullCheck(child, nameof(child), $"Trying to add a child to {parent}, but the child is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add a child to {parent}, but the counter is null!");
            if (!HasNode(parent, MockCounter))
            {
                throw new NotInGraphException($"Trying to add {child} as a child to {parent}, but {parent} is not part of {this}!");
            }
            if (HasNode(child, MockCounter))
            {
                throw new AlreadyInGraphException($"Trying to add {child} to {parent}, but {child} is already part of {this}!");
            }
#endif
            parent.AddChild(child, counter);
            InternalNodes.Add(child, counter);
            (TNode, TNode) edge = (parent, child);
            InternalEdges.Add(edge, counter);
        }

        /// <summary>
        /// Add multiple new <typeparamref name="TNode"/> as children to <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The <typeparamref name="TNode"/> that will be the parent of each <typeparamref name="TNode"/> in <paramref name="children"/>. Must already be part of this <see cref="Tree{N}"/>.</param>
        /// <param name="children">The <see cref="IEnumerable{T}"/> of <typeparamref name="TNode"/>s that will be the new children of <paramref name="parent"/>. Must not be part of this <see cref="Tree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="parent"/>, <paramref name="children"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void AddChildren(TNode parent, IEnumerable<TNode> children, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(parent, nameof(parent), "Trying to add multiple children to a parent, but the parent is null!");
            Utils.NullCheck(children, nameof(children), $"Trying to add multiple children to {parent}, but the IEnumerable of children is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add multiple children to {parent}, but the counter is null!");
#endif
            foreach (TNode child in children)
            {
                AddChild(parent, child, counter);
            }
        }

        /// <summary>
        /// Add all children of <paramref name="node"/> to its parent, and then deletes <paramref name="node"/> from this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="Tree{N}"/>.</exception>
        public void RemoveNode(TNode node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to remove a node from {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove a node from {this}, but the counter is null!");
            if (!HasNode(node, MockCounter))
            {
                throw new NotInGraphException($"Trying to remove {node} from {this}, but {node} is not part of {this}!");
            }
#endif
            if (node.Equals(Root))
            {
                if (node.Degree(counter) > 1)
                {
                    throw new MultipleRootsException($"Trying to remove {node} from {this}, but {node} is the root of this tree and has more than 1 child!");
                }
                TNode newRoot = node.Children(counter).First();
                InternalEdges.Remove((node, newRoot), counter);
                SetRoot(newRoot, counter);
                node.RemoveChild(newRoot, counter);
                InternalNodes.Remove(node, counter);
                return;
            }

            AddChildrenToParent(node, counter);
            TNode parent = node.GetParent(counter);
            parent.RemoveChild(node, counter);
            InternalEdges.Remove((parent, node), counter);
            InternalNodes.Remove(node, counter);
        }

        /// <summary>
        /// Remove muliple nodes from this <see cref="Tree{N}"/>. Children of the nodes to be removed are added to the parent of the node to be removed.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}"/> of <typeparamref name="TNode"/>s to be removed from this <see cref="Tree{N}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveNodes(IEnumerable<TNode> nodes, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(nodes, nameof(nodes), $"Trying to remove multiple nodes from {this}, but the IEnumerable with nodes is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove multiple nodes from {this}, but the counter is null!");
#endif
            foreach (TNode node in nodes)
            {
                RemoveNode(node, counter);
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
                FindRoot(MockCounter);
            }
            catch (NoRootException)
            {
                return false;
            }
            catch (MultipleRootsException)
            {
                return false;
            }

            return DFS.IsAcyclicTree<Tree<TNode>, TNode>(this, MockCounter) && DFS.FindAllConnectedComponents(Nodes(MockCounter), MockCounter).Count == 1;
        }

        /// <summary>
        /// Update the <see cref="NodeType"/> for all the nodes in this <see cref="Tree{N}"/>.
        /// </summary>
        public void UpdateNodeTypes()
        {
            FindLeaves(out List<TNode> leaves, out HashSet<TNode> leafSet, out List<TNode> internalNodes);
            DetermineTypesOfInternalNodes(leafSet, internalNodes);
            DetermineTypesOfLeaves(leaves);
        }

        /// <summary>
        /// Find all the leaves in this <see cref="Tree{N}"/>.
        /// </summary>
        /// <param name="leaves">The resulting <see cref="List{T}"/> with all leaves.</param>
        /// <param name="leafSet">The resulting <see cref="HashSet{T}"/> with all leaves.</param>
        /// <param name="internalNodes">The resulting <see cref="List{T}"/> with all internal nodes.</param>
        private void FindLeaves(out List<TNode> leaves, out HashSet<TNode> leafSet, out List<TNode> internalNodes)
        {
            leaves = new List<TNode>();
            leafSet = new HashSet<TNode>();
            internalNodes = new List<TNode>();

            foreach (TNode node in Nodes(MockCounter))
            {
                if (node.Neighbours(MockCounter).Count() == 1)
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
        private void DetermineTypesOfInternalNodes(HashSet<TNode> leaves, List<TNode> internalNodes)
        {
            foreach (TNode node in internalNodes)
            {
                int internalNeighbours = node.Neighbours(MockCounter).Count(n => !leaves.Contains(n));
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
        private void DetermineTypesOfLeaves(List<TNode> leaves)
        {
            foreach (TNode leaf in leaves)
            {
                TNode parent = leaf.GetParent(MockCounter) ?? leaf.Children(MockCounter).First();

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
