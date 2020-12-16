// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MulticutInTrees.Exceptions;

namespace MulticutInTrees
{
    /// <summary>
    /// Implementation of <see cref="IGraphNode{N}"/> to be used for nodes an undirected graph that is not a tree.
    /// </summary>
    public class GraphNode : IGraphNode<GraphNode>
    {
        /// <summary>
        /// The internal <see cref="List{T}"/> of <see cref="GraphNode"/>s that are neighbours of this <see cref="GraphNode"/>.
        /// </summary>
        private List<GraphNode> InternalNeighbours { get; }

        /// <summary>
        /// The internal <see cref="HashSet{T}"/> of <see cref="GraphNode"/>s that are neighbours of this <see cref="GraphNode"/>. Using a <see cref="HashSet{T}"/> makes lookups amortised faster.
        /// </summary>
        private HashSet<GraphNode> InternalUniqueNeighbours { get; }

        /// <inheritdoc/>
        /// <value>The value of this identifier is given as paramter in the constructor.</value>
        public uint ID { get; }

        /// <summary>
        /// The number of neighbours this <see cref="GraphNode"/> has.
        /// </summary>
        public int Degree => InternalNeighbours.Count;

        /// <inheritdoc/>
        public ReadOnlyCollection<GraphNode> Neighbours => InternalNeighbours.AsReadOnly();

        /// <summary>
        /// Constructor for a <see cref="GraphNode"/>.
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="GraphNode"/>.</param>
        public GraphNode(uint id)
        {
            ID = id;
            InternalNeighbours = new List<GraphNode>();
            InternalUniqueNeighbours = new HashSet<GraphNode>();
        }

        /// <summary>
        /// Constructor for a <see cref="GraphNode"/>.
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="GraphNode"/>.</param>
        /// <param name="neighbours">An <see cref="IEnumerable{T}"/> with <see cref="GraphNode"/>s that are neighbours of this <see cref="GraphNode"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbours"/> is <see langword="null"/>.</exception>
        public GraphNode(uint id, IEnumerable<GraphNode> neighbours)
        {
            if (neighbours is null)
            {
                throw new ArgumentNullException("neighbours", $"Trying to create an instance of {GetType()} with neighbours but the IEnumerable of neighbours is null!");
            }

            ID = id;
            InternalNeighbours = new List<GraphNode>(neighbours);
            InternalUniqueNeighbours = new HashSet<GraphNode>(neighbours);
        }


        /// <summary>
        /// Add another <see cref="GraphNode"/> as neighbour to this <see cref="GraphNode"/>.
        /// <para>
        /// <b>Note:</b> This neighbour connection is 1-way!
        /// </para>
        /// </summary>
        /// <param name="neighbour">The new neighbour to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="neighbour"/> is <see langword="null"/>.</exception>
        /// <exception cref="AddNeighbourToSelfException">Thrown when parameter <paramref name="neighbour"/> is the same <see cref="GraphNode"/> as the <see cref="GraphNode"/> this method is called from.</exception>
        /// <exception cref="AlreadyANeighbourException">Thrown when paramter <paramref name="neighbour"/> is already a neighbour of this <see cref="GraphNode"/>.</exception>
        public void AddNeighbour(GraphNode neighbour)
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
        }

        /// <summary>
        /// Add multiple <see cref="GraphNode"/>s as neighbours to this <see cref="GraphNode"/>. Uses <see cref="AddNeighbour(GraphNode)"/> internally to add each neighbour individually.
        /// </summary>
        /// <para>
        /// <b>Note:</b> These neighbour connections are 1-way!
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> with neighbours to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="IEnumerable{T}"/> parameter with neighbours is <see langword="null"/>.</exception>
        public void AddNeighbours(IEnumerable<GraphNode> neighbours)
        {
            if (neighbours is null)
            {
                throw new ArgumentNullException("neighbours", $"Trying to add a list of neighbours to {this}, but the list is null!");
            }

            foreach (GraphNode neighbour in neighbours)
            {
                AddNeighbour(neighbour);
            }
        }

        /// <summary>
        /// Removes all neighbours from this <see cref="GraphNode"/>.
        /// <para>
        /// <b>Note:</b> Neighbour connections are 1-way, so this only breaks one direction of the connections!
        /// </para>
        /// </summary>
        public void RemoveAllNeighbours()
        {
            InternalUniqueNeighbours.Clear();
            InternalNeighbours.Clear();
        }

        /// <summary>
        /// Removes a neighbour from this <see cref="GraphNode"/>.
        /// </summary>
        /// <para>
        /// <b>Note:</b> Neighbour connections are 1-way, so this only breaks one direction of the connection!
        /// </para>
        /// <param name="neighbour">The <see cref="GraphNode"/> that should be removed from this neighbours of this <see cref="GraphNode"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="neighbour"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotANeighbourException">Thrown when the parameter <paramref name="neighbour"/> is not a neighbour of this <see cref="GraphNode"/>.</exception>
        public void RemoveNeighbour(GraphNode neighbour)
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
        }

        /// <summary>
        /// Removes multiple neighbours from this <see cref="GraphNode"/>. Uses <see cref="RemoveNeighbour(GraphNode)"/> internally to remove each neighbour individually.
        /// </summary>
        /// <para>
        /// <b>Note:</b> Neighbour connections are 1-way, so this only breaks one direction of the connections!
        /// </para>
        /// <param name="neighbours">The <see cref="IEnumerable{T}"/> with neighbours to be removed.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="IEnumerable{T}"/> parameter is <see langword="null"/>.</exception>
        public void RemoveNeighbours(IEnumerable<GraphNode> neighbours)
        {
            if (neighbours is null)
            {
                throw new ArgumentNullException("neighbours", $"Trying to remove a list of neighbours from {this}, but the list is null!");
            }

            foreach (GraphNode neighbour in neighbours)
            {
                RemoveNeighbour(neighbour);
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representing the current object.
        /// </summary>
        /// <returns>The <see cref="string"/> "GraphNode [ID]", where [ID] is <see cref="ID"/>.</returns>
        public override string ToString()
        {
            return $"GraphNode {ID}";
        }

        /// <summary>
        /// Checks whether the parameter <paramref name="node"/> is a neighbour of this <see cref="GraphNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="GraphNode"/> for which we want to know if it is a neighbour of this <see cref="GraphNode"/>.</param>
        /// <returns><see langword="true"/> if parameter <paramref name="node"/> is a neighbour of this <see cref="GraphNode"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the parameter <paramref name="node"/> is <see langword="null"/>.</exception>
        public bool HasNeighbour(GraphNode node)
        {
            if (node is null)
            {
                throw new ArgumentNullException("node", $"Trying to see if {this} has a neighbour, but the neighbour is null!");
            }

            return InternalUniqueNeighbours.Contains(node);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Since the ID is (should be) unique and can only be set in the constructor, it can be used as hashcode for GraphNodes.
            return ID.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current <see cref="GraphNode"/> is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare to the current <see cref="GraphNode"/>.</param>
        /// <returns><see langword="true"/> if the current <see cref="GraphNode"/> is equal to <paramref name="obj"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <see langword="null"/>.</exception>
        /// <exception cref="IncompatibleTypesException">Thrown when the type of <paramref name="obj"/> cannot be compared to a <see cref="GraphNode"/>.</exception>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException("obj", $"Trying to compare {this} to null!");
            }
            if (obj.GetType() != typeof(GraphNode))
            {
                throw new IncompatibleTypesException($"Type of {obj} (type: {obj.GetType()}) cannot be compared to {this} (type: {typeof(GraphNode)})!");
            }

            return Equals((GraphNode)obj);
        }

        /// <summary>
        /// Indicates whether the current <see cref="GraphNode"/> is equal to the <see cref="GraphNode"/> <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="GraphNode"/> to compare to the current <see cref="GraphNode"/>.</param>
        /// <returns><see langword="true"/> if the current <see cref="GraphNode"/> is equal to <paramref name="other"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is <see langword="null"/>.</exception>
        public bool Equals(IGraphNode<GraphNode> other)
        {
            if (other is null)
            {
                throw new ArgumentNullException("other", $"Trying to compare {this} to null!");
            }

            return ID.Equals(other.ID);
        }

        /// <summary>
        /// Checks two <see cref="GraphNode"/>s for equality.
        /// </summary>
        /// <param name="lhs">The first <see cref="GraphNode"/>.</param>
        /// <param name="rhs">The second <see cref="GraphNode"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lhs"/> or <paramref name="rhs"/> is <see langword="null"/>.</exception>
        public static bool operator ==(GraphNode lhs, GraphNode rhs)
        {
            if (lhs is null)
            {
                throw new ArgumentNullException("lhs", "Left hand side of == operator for GraphNode is null!");
            }
            if (rhs is null)
            {
                throw new ArgumentNullException("rhs", "Right hand side of == operator for GraphNode is null!");
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Checks two <see cref="GraphNode"/>s for inequality.
        /// </summary>
        /// <param name="lhs">The first <see cref="GraphNode"/>.</param>
        /// <param name="rhs">The second <see cref="GraphNode"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lhs"/> or <paramref name="rhs"/> is <see langword="null"/>.</exception>
        public static bool operator !=(GraphNode lhs, GraphNode rhs)
        {
            if (lhs is null)
            {
                throw new ArgumentNullException("lhs", "Left hand side of != operator for GraphNode is null!");
            }
            if (rhs is null)
            {
                throw new ArgumentNullException("rhs", "Right hand side of != operator for GraphNode is null!");
            }

            return !lhs.Equals(rhs);
        }
    }
}