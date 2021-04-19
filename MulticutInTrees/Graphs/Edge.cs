// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using MulticutInTrees.CountedDatastructures;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Object to be used for edges in a graph or tree.
    /// </summary>
    /// <typeparam name="TNode">The type of nodes this <see cref="Edge{TNode}"/> is connected to.</typeparam>
    public class Edge<TNode> where TNode : AbstractNode<TNode>
    {
        /// <summary>
        /// The first endpoint of this <see cref="Edge{TNode}"/>.
        /// </summary>
        public TNode Endpoint1 { get; private set; }

        /// <summary>
        /// The second endpoint of this <see cref="Edge{TNode}"/>.
        /// </summary>
        public TNode Endpoint2 { get; private set; }

        /// <summary>
        /// Whether this <see cref="Edge{TNode}"/> is directed.
        /// </summary>
        public bool Directed { get; }

        /// <summary>
        /// Constructor for an <see cref="Edge{TNode}"/>.
        /// </summary>
        /// <param name="endpoint1">The first endpoint of this <see cref="Edge{TNode}"/>.</param>
        /// <param name="endpoint2">The second endpoint of this <see cref="Edge{TNode}"/>.</param>
        /// <param name="directed">Optional. Whether this edge is directed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="endpoint1"/> or <paramref name="endpoint2"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="endpoint1"/> and <paramref name="endpoint2"/> are the same <typeparamref name="TNode"/>.</exception>
        public Edge(TNode endpoint1, TNode endpoint2, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(endpoint1, nameof(endpoint1), "Trying to create an edge, but the first endpoint is null!");
            Utilities.Utils.NullCheck(endpoint2, nameof(endpoint2), "Trying to create an edge, but the second endpoint is null!");
            if (endpoint1.Equals(endpoint2))
            {
                throw new ArgumentException("Trying to create an edge, but both endpoints are the same!");
            }
#endif
            Endpoint1 = endpoint1;
            Endpoint2 = endpoint2;
            Directed = directed;
        }

        /// <summary>
        /// Check whether <paramref name="node"/> is an endpoint of this <see cref="Edge{TNode}"/>.
        /// </summary>
        /// <param name="node">The <typeparamref name="TNode"/> for which we want to know if it is an endpoint of this <see cref="Edge{TNode}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="node"/> is an endpoint of this <see cref="Edge{TNode}"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
        public bool HasEndpoint(TNode node)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(node, nameof(node), "Trying to see if a node is an endpoint of this edge, but the node is null!");
#endif
            return Endpoint1 == node || Endpoint2 == node;
        }

        /// <summary>
        /// Change the endpoint of this <see cref="Edge{TNode}"/>.
        /// </summary>
        /// <param name="oldEndpoint">The old endpoint of this <see cref="Edge{TNode}"/>.</param>
        /// <param name="newEndpoint">The new endpoint of this <see cref="Edge{TNode}"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="oldEndpoint"/>, <paramref name="newEndpoint"/>, or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="oldEndpoint"/> is not an endpoint of this <see cref="Edge{TNode}"/>, or <paramref name="newEndpoint"/> is the other endpoint from this <see cref="Edge{TNode}"/>.</exception>
        internal void ChangeEndpoint(TNode oldEndpoint, TNode newEndpoint, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(oldEndpoint, nameof(oldEndpoint), "Trying to change the endpoint of an edge, but the old endpoint is null!");
            Utilities.Utils.NullCheck(newEndpoint, nameof(newEndpoint), "Trying to change the endpoint of an edge, but the new endpoint is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to change the endpoint of an edge, but the counter is null!");
            if (!oldEndpoint.Equals(Endpoint1) && !oldEndpoint.Equals(Endpoint2))
            {
                throw new InvalidOperationException("Trying to change the endpoint of an edge, but the old endpoint is not an endpoint of this edge!");
            }
            if ((oldEndpoint.Equals(Endpoint1) && newEndpoint.Equals(Endpoint2)) || (oldEndpoint.Equals(Endpoint2) && newEndpoint.Equals(Endpoint1)))
            {
                throw new InvalidOperationException("Trying to change the endpoint of an edge, but the new endpoint is already the other endpoint of this edge!");
            }
#endif
            if (oldEndpoint.Equals(newEndpoint))
            {
                return;
            }

            Endpoint1.RemoveNeighbour(Endpoint2, counter);

            if (oldEndpoint.Equals(Endpoint1))
            {
                newEndpoint.AddNeighbour(Endpoint2, counter);
                Endpoint1 = newEndpoint;
            }
            else
            {
                // We have already done the error checking, so in this else oldEndpoint is equal to Endpoint2.
                newEndpoint.AddNeighbour(Endpoint1, counter);
                Endpoint2 = newEndpoint;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Edge ({Endpoint1}, {Endpoint2})";
        }
    }
}
