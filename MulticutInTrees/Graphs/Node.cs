// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MulticutInTrees.Exceptions;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Implementation of <see cref="INode{N}"/> to be used for nodes an undirected graph that is not a tree.
    /// </summary>
    public class Node : INode<Node>
    {
        /// <summary>
        /// The internal <see cref="List{T}"/> of <see cref="Node"/>s that are neighbours of this <see cref="Node"/>.
        /// </summary>
        private List<Node> InternalNeighbours { get; }

        /// <summary>
        /// The internal <see cref="HashSet{T}"/> of <see cref="Node"/>s that are neighbours of this <see cref="Node"/>. Using a <see cref="HashSet{T}"/> makes lookups amortised faster.
        /// </summary>
        private HashSet<Node> InternalUniqueNeighbours { get; }

        /// <inheritdoc/>
        /// <value>The value of this identifier is given as paramter in the constructor.</value>
        public uint ID { get; }

        /// <inheritdoc/>
        public ReadOnlyCollection<Node> Neighbours => InternalNeighbours.AsReadOnly();

        /// <inheritdoc/>
        public int Degree => InternalNeighbours.Count;

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
            InternalNeighbours = new List<Node>();
            InternalUniqueNeighbours = new HashSet<Node>();
        }

        /// <summary>
        /// Constructor for a <see cref="Node"/>.
        /// <para>
        /// <b>Note:</b> DO NOT use this constructor when using this <see cref="Node"/> in combination with an <see cref="IGraph{N}"/>.
        /// </para>
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="Node"/>.</param>
        /// <param name="neighbours">An <see cref="IEnumerable{T}"/> with <see cref="Node"/>s that are neighbours of this <see cref="Node"/>.</param>
        /// <param name="directed">Whether the connections to the <see cref="Node"/>s in <paramref name="neighbours"/> are directed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbours"/> is <see langword="null"/>.</exception>
        public Node(uint id, IEnumerable<Node> neighbours, bool directed = false)
        {
            if (neighbours is null)
            {
                throw new ArgumentNullException("neighbours", $"Trying to create an instance of {GetType()} with neighbours but the IEnumerable of neighbours is null!");
            }

            ID = id;
            InternalNeighbours = new List<Node>(neighbours);
            InternalUniqueNeighbours = new HashSet<Node>(neighbours);

            if (!directed)
            {
                foreach (Node neighbour in neighbours)
                {
                    neighbour.AddNeighbour(this, true);
                }
            }
        }

        /// <summary>
        /// Add another <see cref="Node"/> as neighbour to this <see cref="Node"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.AddEdge(N, N, bool)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="neighbour">The new neighbour to be added.</param>
        /// <param name="directed">Wether the edge only goes in one direction.</param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="neighbour"/> is <see langword="null"/>.</exception>
        /// <exception cref="AddNeighbourToSelfException">Thrown when parameter <paramref name="neighbour"/> is the same <see cref="Node"/> as the <see cref="Node"/> this method is called from.</exception>
        /// <exception cref="AlreadyANeighbourException">Thrown when paramter <paramref name="neighbour"/> is already a neighbour of this <see cref="Node"/>.</exception>
        public void AddNeighbour(Node neighbour, bool directed = false)
        {
            if (neighbour is null)
            {
                throw new ArgumentNullException("neighbour", $"Trying to add a neighbour to {this}, but neighbour is null!");
            }
            if (neighbour == this)
            {
                throw new AddNeighbourToSelfException($"Trying to add {neighbour} as a neighbour to itself!");
            }
            if (InternalUniqueNeighbours.Contains(neighbour))
            {
                throw new AlreadyANeighbourException($"Trying to add {neighbour} as a neighbour to {this}, but {neighbour} is already a neighbour of {this}!");
            }

            InternalUniqueNeighbours.Add(neighbour);
            InternalNeighbours.Add(neighbour);

            if (!directed)
            {
                neighbour.AddNeighbour(this, true);
            }
        }

        /// <summary>
        /// Add multiple <see cref="Node"/>s as neighbours to this <see cref="Node"/>. Uses <see cref="AddNeighbour(Node, bool)"/> internally to add each neighbour individually.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.AddEdge(N, N, bool)"/> instead.
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> with neighbours to be added.</param>
        /// <param name="directed">Whether the connections are directed.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="IEnumerable{T}"/> parameter with neighbours is <see langword="null"/>.</exception>
        public void AddNeighbours(IEnumerable<Node> neighbours, bool directed = false)
        {
            if (neighbours is null)
            {
                throw new ArgumentNullException("neighbours", $"Trying to add a list of neighbours to {this}, but the list is null!");
            }

            foreach (Node neighbour in neighbours)
            {
                AddNeighbour(neighbour, directed);
            }
        }

        /// <summary>
        /// Removes all neighbours from this <see cref="Node"/>.
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.RemoveAllEdgesOfNode(N, bool)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        public void RemoveAllNeighbours(bool directed = false)
        {
            if (!directed)
            {
                foreach (Node neighbour in Neighbours)
                {
                    neighbour.RemoveNeighbour(this, true);
                }
            }

            InternalUniqueNeighbours.Clear();
            InternalNeighbours.Clear();
        }

        /// <summary>
        /// Removes a neighbour from this <see cref="Node"/>.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.RemoveEdge(N, N, bool)"/> instead.
        /// </para>
        /// <param name="neighbour">The <see cref="Node"/> that should be removed from this neighbours of this <see cref="Node"/>.</param>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="neighbour"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotANeighbourException">Thrown when the parameter <paramref name="neighbour"/> is not a neighbour of this <see cref="Node"/>.</exception>
        public void RemoveNeighbour(Node neighbour, bool directed = false)
        {
            if (neighbour is null)
            {
                throw new ArgumentNullException("neighbour", $"Trying to remove a neighbour from {this}, but neighbour is null!");
            }
            if (!InternalUniqueNeighbours.Contains(neighbour))
            {
                throw new NotANeighbourException($"Trying to remove {neighbour} from {this}'s neighbours, but {neighbour} is no neighbour of {this}!");
            }

            InternalUniqueNeighbours.Remove(neighbour);
            InternalNeighbours.Remove(neighbour);

            if (!directed)
            {
                neighbour.RemoveNeighbour(this, true);
            }
        }

        /// <summary>
        /// Removes multiple neighbours from this <see cref="Node"/>. Uses <see cref="RemoveNeighbour(Node, bool)"/> internally to remove each neighbour individually.
        /// </summary>
        /// <para>
        /// <b>NOTE:</b> If this <see cref="Node"/> is part of a graph, the graph does not see this new neighbour. Please use <see cref="IGraph{N}.RemoveEdges(IList{ValueTuple{N, N}}, bool)"/> instead.
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> with neighbours to be removed.</param>
        /// <param name="directed">Whether to remove only one direction of the connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="IEnumerable{T}"/> parameter is <see langword="null"/>.</exception>
        public void RemoveNeighbours(IEnumerable<Node> neighbours, bool directed = false)
        {
            if (neighbours is null)
            {
                throw new ArgumentNullException("neighbours", $"Trying to remove a list of neighbours from {this}, but the list is null!");
            }

            foreach (Node neighbour in neighbours)
            {
                RemoveNeighbour(neighbour, directed);
            }
        }

        /// <summary>
        /// Checks whether the parameter <paramref name="node"/> is a neighbour of this <see cref="Node"/>.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> for which we want to know if it is a neighbour of this <see cref="Node"/>.</param>
        /// <returns><see langword="true"/> if parameter <paramref name="node"/> is a neighbour of this <see cref="Node"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="node"/> is <see langword="null"/>.</exception>
        public bool HasNeighbour(Node node)
        {
            if (node is null)
            {
                throw new ArgumentNullException("node", $"Trying to see if {this} has a neighbour, but the neighbour is null!");
            }

            return InternalUniqueNeighbours.Contains(node);
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
    }
}