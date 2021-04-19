// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Abstract class for a node.
    /// </summary>
    /// <typeparam name="TNode">THe type of neighbours of this <see cref="AbstractNode{TNode}"/>.</typeparam>
    public abstract class AbstractNode<TNode> where TNode : AbstractNode<TNode>
    {
        /// <summary>
        /// The internal <see cref="CountedCollection{T}"/> of <typeparamref name="TNode"/>s that are neighbours of this <see cref="AbstractNode{TNode}"/>.
        /// </summary>
        protected CountedCollection<TNode> InternalNeighbours { get; }

        /// <inheritdoc/>
        /// <value>The value of this identifier is given as paramter in the constructor.</value>
        public uint ID { get; }

        /// <summary>
        /// The <see cref="NodeType"/> of this <see cref="AbstractNode{TNode}"/>.
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not impact performance.
        /// </summary>
        protected Counter MockCounter { get; }

        /// <summary>
        /// Constructor for a <see cref="AbstractNode{TNode}"/>.
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="AbstractNode{TNode}"/>.</param>
        public AbstractNode(uint id)
        {
            ID = id;
            InternalNeighbours = new CountedCollection<TNode>();
            Type = NodeType.Other;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Add another <typeparamref name="TNode"/> as neighbour to this <see cref="AbstractNode{TNode}"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="AbstractNode{TNode}"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="AbstractGraph{TEdge, TNode}.AddEdge(TEdge, Counter)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="neighbour">The new neighbour to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Wether the edge only goes in one direction.</param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="neighbour"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="AddNeighbourToSelfException">Thrown when parameter <paramref name="neighbour"/> is the same <see cref="AbstractNode{TNode}"/> as the <see cref="AbstractNode{TNode}"/> this method is called from.</exception>
        /// <exception cref="AlreadyANeighbourException">Thrown when paramter <paramref name="neighbour"/> is already a neighbour of this <see cref="AbstractNode{TNode}"/>.</exception>
        internal virtual void AddNeighbour(TNode neighbour, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbour, nameof(neighbour), $"Trying to add a neighbour to {this}, but the neighbour is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to add a neighbour to {this}, but the counter is null!");
            if (neighbour == this)
            {
                throw new AddNeighbourToSelfException($"Trying to add {neighbour} as a neighbour to itself!");
            }
            if (InternalNeighbours.Contains(neighbour, MockCounter))
            {
                throw new AlreadyANeighbourException($"Trying to add {neighbour} as a neighbour to {this}, but {neighbour} is already a neighbour of {this}!");
            }
#endif
            InternalNeighbours.Add(neighbour, counter);

            if (!directed)
            {
                neighbour.AddNeighbour((TNode)this, MockCounter, true);
            }
        }

        /// <summary>
        /// Add multiple <typeparamref name="TNode"/>s as neighbours to this <see cref="AbstractNode{TNode}"/>. Uses <see cref="AddNeighbour(TNode, Counter, bool)"/> internally to add each neighbour individually.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="AbstractNode{TNode}"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="AbstractGraph{TEdge, TNode}.AddEdge(TEdge, Counter)"/> instead.
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> with neighbours to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether the connections are directed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbours"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        internal void AddNeighbours(IEnumerable<TNode> neighbours, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbours, nameof(neighbours), $"Trying to add a list of neighbours to {this}, but the list is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to add a list of neighbours to {this}, but the counter is null!");
#endif
            foreach (TNode neighbour in neighbours)
            {
                AddNeighbour(neighbour, counter, directed);
            }
        }

        /// <summary>
        /// Removes all neighbours from this <see cref="AbstractNode{TNode}"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="AbstractNode{TNode}"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="AbstractGraph{TEdge, TNode}.RemoveAllEdgesOfNode(TNode, Counter)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        internal void RemoveAllNeighbours(Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to remove all neighbours from {this}, but the counter is null!");
#endif
            RemoveNeighbours(Neighbours(counter).ToList(), counter, directed);
        }

        /// <summary>
        /// Removes a neighbour from this <see cref="AbstractNode{TNode}"/>.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="AbstractNode{TNode}"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="AbstractGraph{TEdge, TNode}.RemoveEdge(TEdge, Counter)"/> instead.
        /// </para>
        /// <param name="neighbour">The <typeparamref name="TNode"/> that should be removed from this neighbours of this <see cref="AbstractNode{TNode}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="neighbour"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotANeighbourException">Thrown when the parameter <paramref name="neighbour"/> is not a neighbour of this <see cref="AbstractNode{TNode}"/>.</exception>
        internal virtual void RemoveNeighbour(TNode neighbour, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbour, nameof(neighbour), $"Trying to remove a neighbour from {this}, but the neighbour is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to remove a neighbour from {this}, but the counter is null!");
            if (!InternalNeighbours.Contains(neighbour, MockCounter))
            {
                throw new NotANeighbourException($"Trying to remove {neighbour} from {this}'s neighbours, but {neighbour} is no neighbour of {this}!");
            }
#endif
            InternalNeighbours.Remove(neighbour, counter);

            if (!directed)
            {
                neighbour.RemoveNeighbour((TNode)this, MockCounter, true);
            }
        }

        /// <summary>
        /// Removes multiple neighbours from this <see cref="AbstractNode{TNode}"/>. Uses <see cref="RemoveNeighbour(TNode, Counter, bool)"/> internally to remove each neighbour individually.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="AbstractNode{TNode}"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="AbstractGraph{TEdge, TNode}.RemoveEdges(IEnumerable{TEdge}, Counter)"/> instead.
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> with neighbours to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbours"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        internal void RemoveNeighbours(IEnumerable<TNode> neighbours, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbours, nameof(neighbours), $"Trying to remove a list of neighbours from {this}, but the list is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to remove a list of neighbours from {this}, but the counter is null!");
#endif
            foreach (TNode neighbour in neighbours)
            {
                RemoveNeighbour(neighbour, counter, directed);
            }
        }

        /// <summary>
        /// Checks whether the parameter <paramref name="node"/> is a neighbour of this <see cref="AbstractNode{TNode}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> for which we want to know if it is a neighbour of this <see cref="AbstractNode{TNode}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <returns><see langword="true"/> if parameter <paramref name="node"/> is a neighbour of this <see cref="AbstractNode{TNode}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool HasNeighbour(TNode node, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(node, nameof(node), $"Trying to see if {this} has a neighbour, but the neighbour is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to see if {this} has a neighbour, but the counter is null!");
#endif
            return InternalNeighbours.Contains(node, counter);
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="AbstractNode{TNode}"/>.
        /// <br/>
        /// Looks like: "AbstractNode [ID]", where "[ID]" is the <see cref="ID"/> of this <see cref="AbstractNode{TNode}"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of this <see cref="AbstractNode{TNode}"/>.</returns>
        public override string ToString()
        {
            return $"AbstractNode {ID}";
        }

        /// <summary>
        /// Returns the neighbours of this <see cref="AbstractNode{TNode}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <returns>The neighbours of this <see cref="AbstractNode{TNode}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<TNode> Neighbours(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to get the neighbours of {this}, but the counter is null!");
#endif
            counter++;
            return new CountedEnumerable<TNode>(InternalNeighbours.GetLinkedList(), MockCounter);
        }

        /// <summary>
        /// Returns the number of neighbours of this <see cref="AbstractNode{TNode}"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of neighbours of this <see cref="AbstractNode{TNode}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int Degree(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to get the degree of {this}, but the counter is null!");
#endif
            return InternalNeighbours.Count(counter);
        }
    }
}