// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Implementation of a rooted tree.
    /// </summary>
    public class RootedTree : AbstractGraph<Edge<RootedTreeNode>, RootedTreeNode>
    {
        /// <summary>
        /// The root of this <see cref="RootedTree"/>.
        /// </summary>
        protected RootedTreeNode Root { get; set; }

        /// <summary>
        /// Constructor for a <see cref="RootedTree"/>.
        /// </summary>
        public RootedTree() : base()
        {

        }

        /// <summary>
        /// The height of this <see cref="RootedTree"/>. Equal to the height of the subtree of the <see cref="Root"/> of this <see cref="RootedTree"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that is the height of this <see cref="RootedTree"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int Height(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the height of a tree, but the counter is null!");
#endif
            if (Root is null)
            {
                counter++;
                return 0;
            }
            return Root.HeightOfSubtree(counter);
        }

        /// <summary>
        /// Returns the root of this <see cref="RootedTree"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>The <see cref="RootedTreeNode"/> that is the root of this <see cref="RootedTree"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public RootedTreeNode GetRoot(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the root of a tree, but the counter is null!");
#endif
            counter++;
            return Root;
        }

        /// <summary>
        /// Sets the root of this <see cref="RootedTree"/> to <paramref name="newRoot"/>.
        /// </summary>
        /// <param name="newRoot">The <see cref="RootedTreeNode"/> that will be the new root of this <see cref="RootedTree"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newRoot"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        protected void SetRoot(RootedTreeNode newRoot, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(newRoot, nameof(newRoot), "Trying to set the root of a tree, but the new root is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to set the root of a tree, but the counter is null!");
#endif
            counter++;
            Root = newRoot;
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="RootedTree"/>.
        /// Looks like "RootedTree with n nodes, m edges and height h", where "n", "m" and "h" are numbers.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="RootedTree"/>.</returns>
        public override string ToString()
        {
            return $"RootedTree with {NumberOfNodes(MockCounter)} nodes, {NumberOfEdges(MockCounter)} edges and height {Height(MockCounter)}";
        }

        /// <summary>
        /// Get the root with information from <see cref="AbstractGraph{TEdge, TNode}.Nodes(Counter)"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <returns>The first occurance of an <see cref="RootedTreeNode"/> in <see cref="AbstractGraph{TEdge, TNode}.Nodes(Counter)"/> that is a root.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NoRootException">Thrown when there is no root in <see cref="AbstractGraph{TEdge, TNode}.Nodes(Counter)"/>.</exception>
        /// <exception cref="MultipleRootsException">Thrown when there are multiple roots in <see cref="AbstractGraph{TEdge, TNode}.Nodes(Counter)"/>.</exception>
        protected RootedTreeNode FindRoot(Counter counter)
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
        /// <param name="node">The <see cref="RootedTreeNode"/> whose children we want to connect to its parent.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="MultipleRootsException">Thrown when <paramref name="node"/> is the root of this <see cref="RootedTree"/>.</exception>
        protected void AddChildrenToParent(RootedTreeNode node, Counter counter)
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

            RootedTreeNode parent = node.GetParent(counter);

            foreach (RootedTreeNode child in node.Children(counter).ToList())
            {
                (RootedTreeNode, RootedTreeNode) key = Utils.OrderEdgeSmallToLarge((node, child));
                Edge<RootedTreeNode> edge = NodeTupleToEdge[key];
                if (!edge.Directed)
                {
                    NodeTupleToEdge.Remove((key.Item2, key.Item1));
                }
                NodeTupleToEdge.Remove(key);
                (RootedTreeNode, RootedTreeNode) newKey = Utils.OrderEdgeSmallToLarge((parent, child));
                NodeTupleToEdge[newKey] = edge;
                if (!edge.Directed)
                {
                    NodeTupleToEdge[(newKey.Item2, newKey.Item1)] = edge;
                }
                edge.ChangeEndpoint(node, parent, counter);
            }
        }

        /// <inheritdoc/>
        public override void AddNode(RootedTreeNode node, Counter counter)
        {
            base.AddNode(node, counter);
            if (Root is null)
            {
                SetRoot(node, counter);
            }
        }
        
        /// <inheritdoc/>
        public override void AddEdge(Edge<RootedTreeNode> edge, Counter counter)
        {
            base.AddEdge(edge, counter);
            if (edge.Endpoint1.GetParent(counter) is null)
            {
                SetRoot(edge.Endpoint1, counter);
            }
        }

        /// <summary>
        /// DO NOT USE THIS METHOD!!! Method to remove an edge from this <see cref="RootedTree"/>.
        /// </summary>
        /// <param name="edge">The <see cref="RootedTreeNode"/> to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="InvalidOperationException">Thrown when this method gets called.</exception>
        public override void RemoveEdge(Edge<RootedTreeNode> edge, Counter counter)
        {
            throw new InvalidOperationException("Trying to remove an edge from a rooted tree, but that would make the tree invalid.");
        }

        /// <summary>
        /// Add all children of <paramref name="node"/> to its parent, and then deletes <paramref name="node"/> from this <see cref="RootedTree"/>.
        /// </summary>
        /// <param name="node">The <see cref="RootedTreeNode"/> to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when we try to delete a node from an invalid <see cref="RootedTree"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="RootedTree"/>.</exception>
        public override void RemoveNode(RootedTreeNode node, Counter counter)
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
                RootedTreeNode newRoot = node.Children(counter).First();
                (RootedTreeNode, RootedTreeNode) edgeKey = (node, newRoot);
                Edge<RootedTreeNode> rootEdge = NodeTupleToEdge[edgeKey];
                InternalEdges.Remove(rootEdge, counter);
                NodeTupleToEdge.Remove(edgeKey);
                if (!rootEdge.Directed)
                {
                    NodeTupleToEdge.Remove((edgeKey.Item2, edgeKey.Item1));
                }
                SetRoot(newRoot, counter);
                node.RemoveNeighbour(newRoot, counter);
                InternalNodes.Remove(node, counter);
                return;
            }

            AddChildrenToParent(node, counter);
            RootedTreeNode parent = node.GetParent(counter);
            parent.RemoveNeighbour(node, counter);
            (RootedTreeNode, RootedTreeNode) key = (parent, node);
            Edge<RootedTreeNode> edge = NodeTupleToEdge[key];
            InternalEdges.Remove(edge, counter);
            NodeTupleToEdge.Remove(key);
            if (!edge.Directed)
            {
                NodeTupleToEdge.Remove((key.Item2, key.Item1));
            }
            InternalNodes.Remove(node, counter);
        }

        /// <summary>
        /// Checks if this <see cref="RootedTree"/> is valid: it has exactly 1 root, is acyclic, and is connected.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="RootedTree"/> is valid, <see langword="false"/> otherwise.</returns>
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

            return DFS.IsAcyclic<RootedTree, Edge<RootedTreeNode>, RootedTreeNode>(this, MockCounter) && DFS.FindAllConnectedComponents(Nodes(MockCounter), MockCounter).Count == 1;
        }
    }
}
