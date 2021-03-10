// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Implementation of <see cref="INode{N}"/> to be used for nodes an undirected graph that is not a tree.
    /// </summary>
    public class Node : INode<Node>
    {
        /// <summary>
        /// The internal <see cref="CountedCollection{T}"/> of <see cref="Node"/>s that are neighbours of this <see cref="Node"/>.
        /// </summary>
        private CountedCollection<Node> InternalNeighbours { get; }

        /// <inheritdoc/>
        /// <value>The value of this identifier is given as paramter in the constructor.</value>
        public uint ID { get; }

        /// <summary>
        /// The <see cref="NodeType"/> of this <see cref="Node"/>.
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should not impact performance.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for a <see cref="Node"/>.
        /// <para>
        /// <b>Note:</b> Use this constructor when using this <see cref="Node"/> in combination with an <see cref="IGraph{N}"/>.
        /// </para>
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="Node"/>.</param>
        public Node(uint id)
        {
            ID = id;
            InternalNeighbours = new CountedCollection<Node>();
            Type = NodeType.Other;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Constructor for a <see cref="Node"/>.
        /// <para>
        /// <b>Note:</b> DO NOT use this constructor when using this <see cref="Node"/> in combination with an <see cref="IGraph{N}"/>.
        /// </para>
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="Node"/>.</param>
        /// <param name="neighbours">An <see cref="IEnumerable{T}"/> with <see cref="Node"/>s that are neighbours of this <see cref="Node"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether the connections to the <see cref="Node"/>s in <paramref name="neighbours"/> are directed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbours"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        internal Node(uint id, IEnumerable<Node> neighbours, Counter counter, bool directed = false) : this(id)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbours, nameof(neighbours), $"Trying to create an instance of {GetType()} with neighbours but the IEnumerable of neighbours is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to create an instance of {GetType()} with neighbours but the counter is null!");
#endif
            InternalNeighbours = new CountedCollection<Node>(neighbours, counter);

            if (!directed)
            {
                foreach (Node neighbour in neighbours)
                {
                    neighbour.AddNeighbour(this, MockCounter, true);
                }
            }
        }

        /// <summary>
        /// Add another <see cref="Node"/> as neighbour to this <see cref="Node"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.AddEdge(N, N, Counter, bool)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="neighbour">The new neighbour to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Wether the edge only goes in one direction.</param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="neighbour"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="AddNeighbourToSelfException">Thrown when parameter <paramref name="neighbour"/> is the same <see cref="Node"/> as the <see cref="Node"/> this method is called from.</exception>
        /// <exception cref="AlreadyANeighbourException">Thrown when paramter <paramref name="neighbour"/> is already a neighbour of this <see cref="Node"/>.</exception>
        public void AddNeighbour(Node neighbour, Counter counter, bool directed = false)
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
                neighbour.AddNeighbour(this, MockCounter, true);
            }
        }

        /// <summary>
        /// Add multiple <see cref="Node"/>s as neighbours to this <see cref="Node"/>. Uses <see cref="AddNeighbour(Node, Counter, bool)"/> internally to add each neighbour individually.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.AddEdge(N, N, Counter, bool)"/> instead.
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> with neighbours to be added.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether the connections are directed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbours"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void AddNeighbours(IEnumerable<Node> neighbours, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbours, nameof(neighbours), $"Trying to add a list of neighbours to {this}, but the list is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to add a list of neighbours to {this}, but the counter is null!");
#endif
            foreach (Node neighbour in neighbours)
            {
                AddNeighbour(neighbour, counter, directed);
            }
        }

        /// <summary>
        /// Removes all neighbours from this <see cref="Node"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.RemoveAllEdgesOfNode(N, Counter, bool)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveAllNeighbours(Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to remove all neighbours from {this}, but the counter is null!");
#endif
            if (!directed)
            {
                foreach (Node neighbour in Neighbours(new Counter()))
                {
                    neighbour.RemoveNeighbour(this, MockCounter, true);
                }
            }

            InternalNeighbours.Clear(counter);
        }

        /// <summary>
        /// Removes a neighbour from this <see cref="Node"/>.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.RemoveEdge(N, N, Counter, bool)"/> instead.
        /// </para>
        /// <param name="neighbour">The <see cref="Node"/> that should be removed from this neighbours of this <see cref="Node"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="neighbour"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotANeighbourException">Thrown when the parameter <paramref name="neighbour"/> is not a neighbour of this <see cref="Node"/>.</exception>
        public void RemoveNeighbour(Node neighbour, Counter counter, bool directed = false)
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
                neighbour.RemoveNeighbour(this, MockCounter, true);
            }
        }

        /// <summary>
        /// Removes multiple neighbours from this <see cref="Node"/>. Uses <see cref="RemoveNeighbour(Node, Counter, bool)"/> internally to remove each neighbour individually.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.RemoveEdges(IList{ValueTuple{N, N}}, Counter, bool)"/> instead.
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> with neighbours to be removed.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbours"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public void RemoveNeighbours(IEnumerable<Node> neighbours, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbours, nameof(neighbours), $"Trying to remove a list of neighbours from {this}, but the list is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to remove a list of neighbours from {this}, but the counter is null!");
#endif
            foreach (Node neighbour in neighbours)
            {
                RemoveNeighbour(neighbour, counter, directed);
            }
        }

        /// <summary>
        /// Checks whether the parameter <paramref name="node"/> is a neighbour of this <see cref="Node"/>.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> for which we want to know if it is a neighbour of this <see cref="Node"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <returns><see langword="true"/> if parameter <paramref name="node"/> is a neighbour of this <see cref="Node"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool HasNeighbour(Node node, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(node, nameof(node), $"Trying to see if {this} has a neighbour, but the neighbour is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to see if {this} has a neighbour, but the counter is null!");
#endif
            return InternalNeighbours.Contains(node, counter);
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Node"/>.
        /// <br/>
        /// Looks like: "Node [ID]", where "[ID]" is the <see cref="ID"/> of this <see cref="Node"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of this <see cref="Node"/>.</returns>
        public override string ToString()
        {
            return $"Node {ID}";
        }

        /// <summary>
        /// Returns the neighbours of this <see cref="Node"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <returns>The neighbours of this <see cref="Node"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<Node> Neighbours(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to get the neighbours of {this}, but the counter is null!");
#endif
            _ = counter++;
            return new CountedEnumerable<Node>(InternalNeighbours.GetLinkedList(), counter);
        }

        /// <summary>
        /// Returns the number of neighbours of this <see cref="Node"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> used for this operation.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of neighbours of this <see cref="Node"/>.</returns>
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