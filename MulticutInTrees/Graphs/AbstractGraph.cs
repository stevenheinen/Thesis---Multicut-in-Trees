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
    /// Implementation of a graph.
    /// </summary>
    /// <typeparam name="TEdge">The type of edges in this <see cref="AbstractGraph{TEdge, TNode}"/>.</typeparam>
    /// <typeparam name="TNode">The type of nodes in this <see cref="AbstractGraph{TEdge, TNode}"/>.</typeparam>
    public abstract class AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
    {
        /// <summary>
        /// The internal <see cref="CountedCollection{T}"/> of edges in this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        protected CountedCollection<TEdge> InternalEdges { get; }

        /// <summary>
        /// The internal <see cref="CountedCollection{T}"/> of nodes in this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        protected CountedCollection<TNode> InternalNodes { get; }

        /// <summary>
        /// <see cref="Dictionary{TKey, TValue}"/> from a tuple of <typeparamref name="TNode"/>s to the <typeparamref name="TEdge"/> between them, if it exists.
        /// </summary>
        protected Dictionary<(TNode, TNode), TEdge> NodeTupleToEdge {get;}

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not affect the performance of an <see cref="Algorithms.Algorithm"/> or <see cref="ReductionRules.ReductionRule"/>.
        /// </summary>
        protected Counter MockCounter { get; }

        /// <summary>
        /// Constructor for a <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        public AbstractGraph()
        {
            MockCounter = new Counter();
            NodeTupleToEdge = new Dictionary<(TNode, TNode), TEdge>();
            InternalNodes = new CountedCollection<TNode>();
            InternalEdges = new CountedCollection<TEdge>();
        }

        /// <summary>
        /// The publically visible collection of edges in this <see cref="AbstractGraph{TEdge, TNode}"/>. Edges cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddEdge(TEdge, Counter)"/>, <seealso cref="AddEdges(IEnumerable{TEdge}, Counter)"/>, <seealso cref="RemoveEdge(TEdge, Counter)"/>, <seealso cref="RemoveEdges(IEnumerable{TEdge}, Counter)"/> and <seealso cref="RemoveAllEdgesOfNode(TNode, Counter)"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all edges in this <see cref="AbstractGraph{TEdge, TNode}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<TEdge> Edges(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the edges in a graph, but the counter is null!");
#endif
            counter++;
            return new CountedEnumerable<TEdge>(InternalEdges.GetLinkedList(), counter);
        }

        /// <summary>
        /// The publically visible collection of nodes in this <see cref="AbstractGraph{TEdge, TNode}"/>. Nodes cannot be edited directly.
        /// <br/>
        /// See also: <seealso cref="AddNode(TNode, Counter)"/>, <seealso cref="AddNodes(IEnumerable{TNode}, Counter)"/>, <seealso cref="RemoveNode(TNode, Counter)"/> and <seealso cref="RemoveNodes(IEnumerable{TNode}, Counter)"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all nodes in this <see cref="AbstractGraph{TEdge, TNode}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<TNode> Nodes(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the nodes in a graph, but the counter is null!");
#endif
            counter++;
            return new CountedEnumerable<TNode>(InternalNodes.GetLinkedList(), counter);
        }

        /// <summary>
        /// The number of nodes in this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that represents the number of nodes in this <see cref="AbstractGraph{TEdge, TNode}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int NumberOfNodes(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the number of nodes in a graph, but the counter is null!");
#endif
            return InternalNodes.Count(counter);
        }

        /// <summary>
        /// The number of edges in this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>An <see cref="int"/> that represents the number of edges in this <see cref="AbstractGraph{TEdge, TNode}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int NumberOfEdges(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the number of edges in a graph, but the counter is null!");
#endif
            return InternalEdges.Count(counter);
        }

        /// <summary>
        /// Finds whether <paramref name="node"/> is part of this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> for which we want to know if it is part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is part of this <see cref="AbstractGraph{TEdge, TNode}"/>, <see langword="false"/> otherwise.</returns>
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
        /// Finds whether the edge <paramref name="edge"/> is part of this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <returns><see langword="true"/> if <paramref name="edge"/> exists in this <see cref="AbstractGraph{TEdge, TNode}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either endpoint of <paramref name="edge"/> is not part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        public bool HasEdge(TEdge edge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), $"Trying to find out whether an edge exists in {this}, but the edge is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to find out whether an edge exists in {this}, but the counter is null!");
            if (!HasNode(edge.Endpoint1, MockCounter))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the first endpoint of the edge is not part of {this}!");
            }
            if (!HasNode(edge.Endpoint2, MockCounter))
            {
                throw new NotInGraphException($"Trying to find out whether an edge exists in {this}, but the second endpoint of the edge is not part of {this}!");
            }
#endif
            return InternalEdges.Contains(edge, counter);
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// Looks like "Graph with n nodes and m edges", where "n" and "m" are numbers.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="AbstractGraph{TEdge, TNode}"/>.</returns>
        public override string ToString()
        {
            return $"Abstract graph with {NumberOfNodes(MockCounter)} nodes and {NumberOfEdges(MockCounter)} edges";
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of <typeparamref name="TEdge"/>s that are connected to <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> for which we want to get its connected edges.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="TEdge"/>s that are connected to <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        public virtual IEnumerable<TEdge> GetNeighbouringEdges(TNode node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), "Trying to get the neighbouring edges of a node, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to get the neighbouring edges of a node, but the counter is null!");
            if (!HasNode(node, MockCounter))
            {
                throw new NotInGraphException($"Trying to get the neighbouring edges of a node, but the node is not part of this graph!");
            }
#endif
            List<TEdge> result = new();
            foreach (TNode neighbour in node.Neighbours(counter))
            {
                result.Add(NodeTupleToEdge[(node, neighbour)]);
            }
            return result;
        }

        /// <summary>
        /// Add a new <typeparamref name="TNode"/> <paramref name="node"/> to this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="AlreadyInGraphException">Thrown when <paramref name="node"/> is already part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        public virtual void AddNode(TNode node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to add a node to {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add a node to {this}, but the counter is null!");
            if (HasNode(node, MockCounter))
            {
                throw new AlreadyInGraphException($"Trying to add {node} to {this}, but {node} is already part of {this}!");
            }
#endif
            InternalNodes.Add(node, counter);
        }

        /// <summary>
        /// Add multiple new <typeparamref name="TNode"/>s to this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> is <see langword="null"/>.</exception>
        public void AddNodes(IEnumerable<TNode> nodes, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(nodes, nameof(nodes), $"Trying to add an IEnumerable of nodes to {this}, but the IEnumerable is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add an IEnumerable of nodes to {this}, but the counter is null!");
#endif
            foreach (TNode node in nodes)
            {
                AddNode(node, counter);
            }
        }

        /// <summary>
        /// Add an edge to this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="edge"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either endpoint of <paramref name="edge"/> is not part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        /// <exception cref="AlreadyInGraphException">Thrown when the edge to be added is already part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        public virtual void AddEdge(TEdge edge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), $"Trying to add an edge to {this}, but the edge is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add an edge to {this}, but the counter is null!");
            if (!HasNode(edge.Endpoint1, MockCounter))
            {
                throw new NotInGraphException($"Trying to add an edge between {edge.Endpoint1} and {edge.Endpoint2} to {this}, but {edge.Endpoint1} is not part of {this}!");
            }
            if (!HasNode(edge.Endpoint2, MockCounter))
            {
                throw new NotInGraphException($"Trying to add an edge between {edge.Endpoint1} and {edge.Endpoint2} to {this}, but {edge.Endpoint2} is not part of {this}!");
            }
            if (HasEdge(edge, MockCounter))
            {
                throw new AlreadyInGraphException($"Trying to add an edge between {edge.Endpoint1} and {edge.Endpoint2} to {this}, but this edge is already part of {this}!");
            }
#endif
            edge.Endpoint1.AddNeighbour(edge.Endpoint2, counter, edge.Directed);
            NodeTupleToEdge[(edge.Endpoint1, edge.Endpoint2)] = edge;
            if (!edge.Directed)
            {
                NodeTupleToEdge[(edge.Endpoint2, edge.Endpoint1)] = edge;
            }
            InternalEdges.Add(edge, counter);
        }

        /// <summary>
        /// Add multiple edges to this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void AddEdges(IEnumerable<TEdge> edges, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), $"Trying to add an IEnumerable of edges to {this}, but the IEnumerable is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to add an IEnumerable of edges to {this}, but the counter is null!");
#endif
            foreach (TEdge edge in edges)
            {
                AddEdge(edge, counter);
            }
        }

        /// <summary>
        /// Remove an <typeparamref name="TNode"/> from this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        public virtual void RemoveNode(TNode node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to remove a node from {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove a node from {this}, but the counter is null!");
            if (!HasNode(node, MockCounter))
            {
                throw new NotInGraphException($"Trying to remove {node} from {this}, but {node} is not part of {this}!");
            }
#endif
            RemoveAllEdgesOfNode(node, counter);
            InternalNodes.Remove(node, counter);
        }

        /// <summary>
        /// Remove multiple <typeparamref name="TNode"/>s from this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
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
        /// Removes all edges connected to a given <typeparamref name="TNode"/> from this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="node"/> is not part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        public void RemoveAllEdgesOfNode(TNode node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), $"Trying to remove all edges of a node in {this}, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove all edges of a node in {this}, but the counter is null!");
            if (!HasNode(node, MockCounter))
            {
                throw new NotInGraphException($"Trying to remove all edges of {node} from {this}, but {node} is not part of {this}!");
            }
#endif
            RemoveEdges(node.Neighbours(counter).Select(neighbour => NodeTupleToEdge[(node, neighbour)]), counter);
            RemoveEdges(node.Neighbours(counter).Select(neighbour => (neighbour, node)).Where(k => NodeTupleToEdge.ContainsKey(k)).Select(k => NodeTupleToEdge[k]), counter);
        }

        /// <summary>
        /// Remove multiple edges from this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveEdges(IEnumerable<TEdge> edges, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), $"Trying to remove multiple edges from {this}, but the IEnumerable with edges is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to remove multiple edges from {this}, but the counter is null!");
#endif
            foreach (TEdge edge in edges.ToList())
            {
                RemoveEdge(edge, counter);
            }
        }

        /// <summary>
        /// Remove an edge from this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when either endpoint of <paramref name="edge"/> or the edge between them is not part of this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        public virtual void RemoveEdge(TEdge edge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to remove an edge from a graph, but the edge is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to remove an edge from a graph, but the counter is null!");
            if (!HasNode(edge.Endpoint1, MockCounter))
            {
                throw new NotInGraphException("Trying to remove an edge from a graph, but the first endpoint of the edge is not part of this graph!");
            }
            if (!HasNode(edge.Endpoint2, MockCounter))
            {
                throw new NotInGraphException("Trying to remove an edge from a graph, but the second endpoint of the edge is not part of this graph!");
            }
            if (!HasEdge(edge, counter))
            {
                throw new NotInGraphException($"Trying to remove the edge {edge} from this graph, but this edge is not part of the graph!");
            }
#endif
            edge.Endpoint1.RemoveNeighbour(edge.Endpoint2, counter, edge.Directed);
            NodeTupleToEdge.Remove((edge.Endpoint1, edge.Endpoint2));
            if (!edge.Directed)
            {
                NodeTupleToEdge.Remove((edge.Endpoint2, edge.Endpoint1));
            }
            InternalEdges.Remove(edge, counter);
        }
        
        /// <summary>
        /// Removes all <typeparamref name="TEdge"/>s from this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        public virtual void RemoveAllEdges(Counter counter)
        {
            List<TEdge> edges = new(InternalEdges.GetCountedEnumerable(MockCounter));
            RemoveEdges(edges, counter);
        }

        /// <summary>
        /// Removes all <typeparamref name="TNode"/>s from this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        public virtual void RemoveAllNodes(Counter counter)
        {
            RemoveAllEdges(counter);
            List<TNode> nodes = new(InternalNodes.GetCountedEnumerable(MockCounter));
            RemoveNodes(nodes, counter);
        }

        /// <summary>
        /// Update the <see cref="NodeType"/> for all the nodes in this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        public void UpdateNodeTypes()
        {
            FindLeaves(out List<TNode> leaves, out HashSet<TNode> leafSet, out List<TNode> internalNodes);
            DetermineTypesOfInternalNodes(leafSet, internalNodes);
            DetermineTypesOfLeaves(leaves);
        }

        /// <summary>
        /// Find all the leaves in this <see cref="AbstractGraph{TEdge, TNode}"/>.
        /// </summary>
        /// <param name="leaves">The resulting <see cref="List{T}"/> with all leaves.</param>
        /// <param name="leafSet">The resulting <see cref="HashSet{T}"/> with all leaves.</param>
        /// <param name="internalNodes">The resulting <see cref="List{T}"/> with all internal nodes.</param>
        protected void FindLeaves(out List<TNode> leaves, out HashSet<TNode> leafSet, out List<TNode> internalNodes)
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
        protected void DetermineTypesOfInternalNodes(HashSet<TNode> leaves, List<TNode> internalNodes)
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
        protected void DetermineTypesOfLeaves(List<TNode> leaves)
        {
            foreach (TNode leaf in leaves)
            {
                TNode parent = leaf.Neighbours(MockCounter).First();

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

        /// <summary>
        /// Checks whether this <see cref="AbstractGraph{TEdge, TNode}"/> is acyclic.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used during this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="AbstractGraph{TEdge, TNode}"/> is acyclic, <see langword="false"/> if it is cyclic.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool IsAcyclic(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to find out whether a graph is acyclic, but the counter is null!");
#endif
            return DFS.IsAcyclic<AbstractGraph<TEdge, TNode>, TEdge, TNode>(this, counter);
        }

        /// <summary>
        /// Checks whether this <see cref="AbstractGraph{TEdge, TNode}"/> is connected.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used during this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="AbstractGraph{TEdge, TNode}"/> is connected, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool IsConnected(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to find out whether a graph is connected, but the counter is null!");
#endif
            return DFS.FindAllConnectedComponents(Nodes(counter), counter).Count < 2;
        }

        /// <summary>
        /// Checks whether this <see cref="AbstractGraph{TEdge, TNode}"/> is a tree.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used during this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="AbstractGraph{TEdge, TNode}"/> is a tree, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool IsTree(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to find out whether a graph is a tree, but the counter is null!");
#endif
            return IsAcyclic(counter) && IsConnected(counter);
        }

        /// <summary>
        /// Transforms an <see cref="IEnumerable{T}"/> with <typeparamref name="TNode"/>s representing a path to a <see cref="List{T}"/> with tuples of two <typeparamref name="TNode"/>s representing the edges on the path.
        /// </summary>
        /// <param name="path">An <see cref="IEnumerable{T}"/> with <typeparamref name="TNode"/>s we want to tranform to a <see cref="List{T}"/> with the edges on the path.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> with <typeparamref name="TEdge"/>s representing the edges on <paramref name="path"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
        public IEnumerable<TEdge> NodePathToEdgePath(IList<TNode> path)
        {
#if !EXPERIMENT
            Utils.NullCheck(path, nameof(path), "Trying to transform a node path to an edge path, but the path is null!");
#endif
            List<TEdge> result = new();
            for (int i = 0; i < path.Count - 1; i++)
            {
                TNode endpoint1 = path[i];
                TNode endpoint2 = path[i + 1];

                TEdge edge = NodeTupleToEdge[(endpoint1, endpoint2)];

                result.Add(edge);
            }

            return result;
        }

        /// <summary>
        /// Returns the <typeparamref name="TEdge"/> between <paramref name="endpoint1"/> and <paramref name="endpoint2"/>.
        /// </summary>
        /// <param name="endpoint1">The first endpoint of the <typeparamref name="TEdge"/>.</param>
        /// <param name="endpoint2">The second endpoint of the <typeparamref name="TEdge"/>.</param>
        /// <returns>The <typeparamref name="TEdge"/> between <paramref name="endpoint1"/> and <paramref name="endpoint2"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="endpoint1"/> or <paramref name="endpoint2"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when the <typeparamref name="TEdge"/> between <paramref name="endpoint1"/> and <paramref name="endpoint2"/> does not exist in this <see cref="AbstractGraph{TEdge, TNode}"/>.</exception>
        public TEdge GetEdgeBetween(TNode endpoint1, TNode endpoint2)
        {
#if !EXPERIMENT
            Utils.NullCheck(endpoint1, nameof(endpoint1), "Trying to get the edge between two nodes, but the first node is null!");
            Utils.NullCheck(endpoint2, nameof(endpoint2), "Trying to get the edge between two nodes, but the second node is null!");
#endif
            if (NodeTupleToEdge.TryGetValue((endpoint1, endpoint2), out TEdge result))
            {
                return result;
            }

            throw new NotInGraphException($"The requested edge between {endpoint1} and {endpoint2} is not part of this graph!");
        }

        /// <summary>
        /// Contract an edge.
        /// </summary>
        /// <param name="edge">The <typeparamref name="TEdge"/> to be contracted.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used during this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public TNode ContractEdge(TEdge edge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to contract an edge in a graph, but the edge is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to contract an edge in a graph, but the counter is null!");
#endif
            TNode newNode = edge.Endpoint1;
            UpdateNodeTypesDuringEdgeContraction(edge, newNode);

            foreach (TEdge neighbour in GetNeighbouringEdges(edge.Endpoint2, counter))
            {
                if (neighbour == edge)
                {
                    continue;
                }

                (TNode, TNode) oldTuple = (neighbour.Endpoint1, neighbour.Endpoint2);
                neighbour.ChangeEndpoint(edge.Endpoint2, edge.Endpoint1, counter);
                (TNode, TNode) newTuple = (neighbour.Endpoint1, neighbour.Endpoint2);
                NodeTupleToEdge[newTuple] = NodeTupleToEdge[oldTuple];
                if (!neighbour.Directed)
                {
                    (TNode, TNode) oldTuple2 = (oldTuple.Item2, oldTuple.Item1);
                    (TNode, TNode) newTuple2 = (newTuple.Item2, newTuple.Item1);
                    NodeTupleToEdge[newTuple2] = NodeTupleToEdge[oldTuple2];
                    NodeTupleToEdge.Remove(oldTuple2);
                }
                NodeTupleToEdge.Remove(oldTuple);
            }

            RemoveNode(edge.Endpoint2, counter);

            return newNode;
        }

        /// <summary>
        /// Change the endpoint of <paramref name="edge"/> from <paramref name="oldEndpoint"/> to <paramref name="newEndpoint"/>.
        /// </summary>
        /// <param name="edge">The <typeparamref name="TEdge"/> for which we want to change its endpoint.</param>
        /// <param name="oldEndpoint">The old endpoint of <paramref name="edge"/>.</param>
        /// <param name="newEndpoint">The new endpoint of <paramref name="edge"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for performance measurement.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/>, <paramref name="oldEndpoint"/>, <paramref name="newEndpoint"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void ChangeEndpointOfEdge(TEdge edge, TNode oldEndpoint, TNode newEndpoint, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to change the endpoint of an edge, but the edge is null!");
            Utils.NullCheck(oldEndpoint, nameof(oldEndpoint), "Trying to change the endpoint of an edge, but the old endpoint of the edge is null!");
            Utils.NullCheck(newEndpoint, nameof(newEndpoint), "Trying to change the endpoint of an edge, but the new endpoint of the edge is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to change the endpoint of an edge, but the counter is null!");
#endif
            (TNode, TNode) oldTuple = (edge.Endpoint1, edge.Endpoint2);
            edge.ChangeEndpoint(oldEndpoint, newEndpoint, counter);
            (TNode, TNode) newTuple = (edge.Endpoint1, edge.Endpoint2);
            NodeTupleToEdge[newTuple] = NodeTupleToEdge[oldTuple];
            if (!edge.Directed)
            {
                (TNode, TNode) oldTuple2 = (oldTuple.Item2, oldTuple.Item1);
                (TNode, TNode) newTuple2 = (newTuple.Item2, newTuple.Item1);
                NodeTupleToEdge[newTuple2] = NodeTupleToEdge[oldTuple2];
                NodeTupleToEdge.Remove(oldTuple2);
            }
            NodeTupleToEdge.Remove(oldTuple);
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="Node"/>s in the instance when an edge is contracted.
        /// </summary>
        /// <param name="contractedEdge">The <typeparamref name="TEdge"/> that is being contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the edge contraction.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdge"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="AbstractNode{TNode}.Type"/> of either endpoint of <paramref name="contractedEdge"/> is <see cref="NodeType.Other"/>. In that case, please update the types of the nodes by calling <seealso cref="AbstractGraph{TEdge, TNode}.UpdateNodeTypes()"/>. Or, when <paramref name="contractedEdge"/> exists between two nodes that have the wrong <see cref="NodeType"/>.</exception>
        private void UpdateNodeTypesDuringEdgeContraction(TEdge contractedEdge, TNode newNode)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdge, nameof(contractedEdge), "Trying to update the nodetypes of the nodes that are the endpoints of an edge that is being contracted, but the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to update the nodetypes of the nodes that are the endpoints of an edge that is being contracted, but the node that is the result of the edge contraction is null!");
            if (contractedEdge.Endpoint1.Type == NodeType.Other)
            {
                throw new NotSupportedException($"Trying to update the nodetypes of the nodes that are the endpoints of the edge {contractedEdge} that is begin contracted, but the type of {contractedEdge.Endpoint1} is not known. Please make sure every node has a nodetype to start with!");
            }
            if (contractedEdge.Endpoint2.Type == NodeType.Other)
            {
                throw new NotSupportedException($"Trying to update the nodetypes of the nodes that are the endpoints of the edge {contractedEdge} that is begin contracted, but the type of {contractedEdge.Endpoint2} is not known. Please make sure every node has a nodetype to start with!");
            }
#endif
            // In case of a contraction of an edge between two I1-nodes, two I2-nodes, or two I3-nodes, no changes have to be made.
            // We also do not need to change anything when contracting an edge between an I2-node and an L2-leaf, or between an I3-node and an L3-leaf (but we do have to check stuff for the other way around).
            switch (contractedEdge.Endpoint1.Type)
            {
                case NodeType.I1:
                    UpdateNodeTypesDuringEdgeContractionI1Node(contractedEdge.Endpoint1, contractedEdge.Endpoint2, newNode);
                    break;
                case NodeType.I2:
                    UpdateNodeTypesDuringEdgeContractionI2Node(contractedEdge.Endpoint1, contractedEdge.Endpoint2, newNode);
                    break;
                case NodeType.I3:
                    UpdateNodeTypesDuringEdgeContractionI3Node(contractedEdge.Endpoint1, contractedEdge.Endpoint2, newNode);
                    break;
                case NodeType.L1:
                    UpdateNodeTypesDuringEdgeContractionL1Node(contractedEdge.Endpoint1, contractedEdge.Endpoint2, newNode);
                    break;
                case NodeType.L2:
                    UpdateNodeTypesDuringEdgeContractionL2Node(contractedEdge.Endpoint1, contractedEdge.Endpoint2, newNode);
                    break;
                case NodeType.L3:
                    UpdateNodeTypesDuringEdgeContractionL3Node(contractedEdge.Endpoint1, contractedEdge.Endpoint2, newNode);
                    break;
            }
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="Node"/>s in the instance when an edge is contracted between an <see cref="NodeType.I1"/>-node and any other node.
        /// </summary>
        /// <param name="i1Node">The <see cref="NodeType.I1"/>-node that is an endpoint of the edge that is being contracted.</param>
        /// <param name="otherNode">The other endpoint of the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the edge contraction.</param>
        /// <exception cref="NotSupportedException"> Then when the edge that is being contracted exists between two nodes that have the wrong <see cref="NodeType"/>.</exception>
        private void UpdateNodeTypesDuringEdgeContractionI1Node(TNode i1Node, TNode otherNode, TNode newNode)
        {
            switch (otherNode.Type)
            {
                case NodeType.I2:
                    newNode.Type = NodeType.I1;
                    ChangeLeavesFromNodeToType(otherNode, NodeType.L2, NodeType.L1);
                    break;
                case NodeType.I3:
                    UpdateNodeTypesEdgeContractionI1I3(i1Node, otherNode, newNode);
                    break;
                case NodeType.L1:
                    UpdateNodeTypesEdgeContractionL1I1(i1Node, newNode);
                    break;
                case NodeType.I1:
                    break;
                default:
                    // These cases should not happen
                    // case (NodeType.I1, NodeType.L2):
                    // case (NodeType.I1, NodeType.L3):
                    throw new NotSupportedException($"Trying to contract an edge between a node with type {i1Node.Type} and a node with type {otherNode.Type}, but this should not happen!");
            }
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="Node"/>s in the instance when an edge is contracted between an <see cref="NodeType.I2"/>-node and any other node.
        /// </summary>
        /// <param name="i2Node">The <see cref="NodeType.I2"/>-node that is an endpoint of the edge that is being contracted.</param>
        /// <param name="otherNode">The other endpoint of the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the edge contraction.</param>
        /// <exception cref="NotSupportedException"> Then when the edge that is being contracted exists between two nodes that have the wrong <see cref="NodeType"/>.</exception>
        private void UpdateNodeTypesDuringEdgeContractionI2Node(TNode i2Node, TNode otherNode, TNode newNode)
        {
            switch (otherNode.Type)
            {
                case NodeType.I1:
                    newNode.Type = NodeType.I1;
                    ChangeLeavesFromNodeToType(i2Node, NodeType.L2, NodeType.L1);
                    break;
                case NodeType.I3:
                    newNode.Type = NodeType.I3;
                    ChangeLeavesFromNodeToType(i2Node, NodeType.L2, NodeType.L3);
                    break;
                case NodeType.L2:
                case NodeType.I2:
                    break;
                default:
                    // All other uncovered cases (listed below) should not occur.
                    // case (NodeType.I2, NodeType.L1):
                    // case (NodeType.I2, NodeType.L3):
                    throw new NotSupportedException($"Trying to contract an edge between a node with type {i2Node.Type} and a node with type {otherNode.Type}, but this should not happen!");
            }
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="Node"/>s in the instance when an edge is contracted between an <see cref="NodeType.I3"/>-node and any other node.
        /// </summary>
        /// <param name="i3Node">The <see cref="NodeType.I3"/>-node that is an endpoint of the edge that is being contracted.</param>
        /// <param name="otherNode">The other endpoint of the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the edge contraction.</param>
        /// <exception cref="NotSupportedException"> Then when the edge that is being contracted exists between two nodes that have the wrong <see cref="NodeType"/>.</exception>
        private void UpdateNodeTypesDuringEdgeContractionI3Node(TNode i3Node, TNode otherNode, TNode newNode)
        {
            switch (otherNode.Type)
            {
                case NodeType.I1:
                    UpdateNodeTypesEdgeContractionI1I3(otherNode, i3Node, newNode);
                    break;
                case NodeType.I2:
                    newNode.Type = NodeType.I3;
                    ChangeLeavesFromNodeToType(otherNode, NodeType.L2, NodeType.L3);
                    break;
                case NodeType.L3:
                case NodeType.I3:
                    break;
                default:
                    // All other uncovered cases (listed below) should not occur.
                    // case (NodeType.I3, NodeType.L1):
                    // case (NodeType.I3, NodeType.L2):
                    throw new NotSupportedException($"Trying to contract an edge between a node with type {i3Node.Type} and a node with type {otherNode.Type}, but this should not happen!");
            }
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="Node"/>s in the instance when an edge is contracted between an <see cref="NodeType.L1"/>-leaf and any other node.
        /// </summary>
        /// <param name="l1Leaf">The <see cref="NodeType.L1"/>-leaf that is an endpoint of the edge that is being contracted.</param>
        /// <param name="otherNode">The other endpoint of the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the edge contraction.</param>
        /// <exception cref="NotSupportedException"> Then when the edge that is being contracted exists between two nodes that have the wrong <see cref="NodeType"/>.</exception>
        private void UpdateNodeTypesDuringEdgeContractionL1Node(TNode l1Leaf, TNode otherNode, TNode newNode)
        {
            switch (otherNode.Type)
            {
                case NodeType.I1:
                    UpdateNodeTypesEdgeContractionL1I1(otherNode, newNode);
                    break;
                default:
                    // All other uncovered cases (listed below) should not occur.
                    // case (NodeType.L1, NodeType.L1):
                    // case (NodeType.L1, NodeType.L2):
                    // case (NodeType.L1, NodeType.L3):
                    // case (NodeType.L1, NodeType.I2):
                    // case (NodeType.L1, NodeType.I3):
                    throw new NotSupportedException($"Trying to contract an edge between a node with type {l1Leaf.Type} and a node with type {otherNode.Type}, but this should not happen!");
            }
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="Node"/>s in the instance when an edge is contracted between an <see cref="NodeType.L2"/>-leaf and any other node.
        /// </summary>
        /// <param name="l2Leaf">The <see cref="NodeType.L2"/>-leaf that is an endpoint of the edge that is being contracted.</param>
        /// <param name="otherNode">The other endpoint of the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the edge contraction.</param>
        /// <exception cref="NotSupportedException"> Then when the edge that is being contracted exists between two nodes that have the wrong <see cref="NodeType"/>.</exception>
        private static void UpdateNodeTypesDuringEdgeContractionL2Node(TNode l2Leaf, TNode otherNode, TNode newNode)
        {
            newNode.Type = otherNode.Type switch
            {
                NodeType.I2 => NodeType.I2,
                _ => throw new NotSupportedException($"Trying to contract an edge between a node with type {l2Leaf.Type} and a node with type {otherNode.Type}, but this should not happen!"),
                // All other uncovered cases (listed below) should not occur.
                // case (NodeType.L2, NodeType.L1):
                // case (NodeType.L2, NodeType.L2):
                // case (NodeType.L2, NodeType.L3):
                // case (NodeType.L2, NodeType.I1):
                // case (NodeType.L2, NodeType.I3):
            };
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="Node"/>s in the instance when an edge is contracted between an <see cref="NodeType.L3"/>-leaf and any other node.
        /// </summary>
        /// <param name="l3Leaf">The <see cref="NodeType.L3"/>-leaf that is an endpoint of the edge that is being contracted.</param>
        /// <param name="otherNode">The other endpoint of the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the edge contraction.</param>
        /// <exception cref="NotSupportedException"> Then when the edge that is being contracted exists between two nodes that have the wrong <see cref="NodeType"/>.</exception>
        private static void UpdateNodeTypesDuringEdgeContractionL3Node(TNode l3Leaf, TNode otherNode, TNode newNode)
        {
            newNode.Type = otherNode.Type switch
            {
                NodeType.I3 => NodeType.I3,
                _ => throw new NotSupportedException($"Trying to contract an edge between a node with type {l3Leaf.Type} and a node with type {otherNode.Type}, but this should not happen!"),
                // All other uncovered cases (listed below) should not occur.
                // case (NodeType.L3, NodeType.L1):
                // case (NodeType.L3, NodeType.L2):
                // case (NodeType.L3, NodeType.L3):
                // case (NodeType.L3, NodeType.I1):
                // case (NodeType.L3, NodeType.I2):
            };
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <typeparamref name="TNode"/>s in the instance when an edge between an <see cref="NodeType.I1"/>-node and an <see cref="NodeType.I3"/>-node is contracted.
        /// </summary>
        /// <param name="contractedEdgeI1Node">The <see cref="NodeType.I1"/>-node that is the endpoint of the contracted edge.</param>
        /// <param name="contractedEdgeI3Node">The <see cref="NodeType.I3"/>-node that is the endpoint of the contracted edge.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the edge contraction.</param>
        private void UpdateNodeTypesEdgeContractionI1I3(TNode contractedEdgeI1Node, TNode contractedEdgeI3Node, TNode newNode)
        {
            // If the I3-node has exactly three internal neighbours, the result of this edge contraction will be an I2-node.
            // If it has more than three internal neighbours, the result will be an I3-node.
            bool hasAtLeastFourInternalNeighbours = contractedEdgeI3Node.Neighbours(MockCounter).Count(n => n.Degree(MockCounter) > 1) > 3;
            NodeType contractedType;
            NodeType leafType;

            if (hasAtLeastFourInternalNeighbours)
            {
                contractedType = NodeType.I3;
                leafType = NodeType.L3;
            }
            else
            {
                contractedType = NodeType.I2;
                leafType = NodeType.L2;
                ChangeLeavesFromNodeToType(contractedEdgeI3Node, NodeType.L3, leafType);
            }

            newNode.Type = contractedType;
            ChangeLeavesFromNodeToType(contractedEdgeI1Node, NodeType.L1, leafType);
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <typeparamref name="TNode"/>s in the instance when an edge between an <see cref="NodeType.I1"/>-node and an <see cref="NodeType.L1"/>-leaf is contracted.
        /// </summary>
        /// <param name="contractedEdgeI1Node">The <see cref="NodeType.I1"/>-node that is the endpoint of the contracted edge.</param>
        /// <param name="newNode">The <typeparamref name="TNode"/> that is the result of the edge contraction.</param>
        private void UpdateNodeTypesEdgeContractionL1I1(TNode contractedEdgeI1Node, TNode newNode)
        {
            // If the I1-node has exactly one leaf (which is also the edge that is contracted), the resulting node will be a leaf. 
            // The type of this leaf depends on the type of the (unique) internal neighbour of the I1-node.
            bool hasExactlyOneLeaf = contractedEdgeI1Node.Neighbours(MockCounter).Count(n => n.Degree(MockCounter) == 1) == 1;
            if (hasExactlyOneLeaf)
            {
                TNode internalNeighbour = contractedEdgeI1Node.Neighbours(MockCounter).FirstOrDefault(n => n.Neighbours(MockCounter).Count() > 1);
                if (internalNeighbour is null)
                {
                    newNode.Type = NodeType.I1;
                }
                else if (internalNeighbour.Type == NodeType.I1)
                {
                    newNode.Type = NodeType.L1;
                }
                else if (internalNeighbour.Type == NodeType.I2)
                {
                    newNode.Type = NodeType.L1;
                    internalNeighbour.Type = NodeType.I1;
                    ChangeLeavesFromNodeToType(internalNeighbour, NodeType.L2, NodeType.L1);
                }
                else if (internalNeighbour.Neighbours(MockCounter).Count(n => n.Degree(MockCounter) > 1) == 3)
                {
                    newNode.Type = NodeType.L2;
                    internalNeighbour.Type = NodeType.I2;
                    ChangeLeavesFromNodeToType(internalNeighbour, NodeType.L3, NodeType.L2);
                }
                else
                {
                    newNode.Type = NodeType.L3;
                }
            }
            else
            {
                newNode.Type = NodeType.I1;
            }
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of all the leaves of <paramref name="parentNode"/>.
        /// </summary>
        /// <param name="parentNode">The <typeparamref name="TNode"/> for which we want to change the <see cref="NodeType"/> of its leaves.</param>
        /// <param name="oldType">The old <see cref="NodeType"/> of the leaves of <paramref name="parentNode"/>. Is equal to Lx, where Ix is the <see cref="AbstractNode{TNode}.Type"/> of <paramref name="parentNode"/>.</param>
        /// <param name="newType">The new <see cref="NodeType"/> of the leaves of <paramref name="parentNode"/>.</param>
        private void ChangeLeavesFromNodeToType(TNode parentNode, NodeType oldType, NodeType newType)
        {
            foreach (TNode leaf in parentNode.Neighbours(MockCounter).Where(n => n.Type == oldType))
            {
                leaf.Type = newType;
            }
        }
    }
}
