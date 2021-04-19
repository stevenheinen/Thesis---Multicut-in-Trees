// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.MulticutProblem
{
    /// <summary>
    /// Class that represents a demand pair in the input.
    /// </summary>
    public class DemandPair
    {
        /// <summary>
        /// The <see cref="CountedCollection{T}"/> containing the edges on the demand path of this <see cref="DemandPair"/>.
        /// </summary>
        private CountedCollection<Edge<Node>> Path { get; }

        /// <summary>
        /// The unique identifier of this <see cref="DemandPair"/>.
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// The first endpoint of this <see cref="DemandPair"/>.
        /// </summary>
        public Node Node1 { get; private set; }

        /// <summary>
        /// The second endpoint of this <see cref="DemandPair"/>.
        /// </summary>
        public Node Node2 { get; private set; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for modifications that should not impact the performance of an <see cref="Algorithms.Algorithm"/> or <see cref="ReductionRules.ReductionRule"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for a <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="DemandPair"/>.</param>
        /// <param name="node1">The first endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="node2">The second endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="tree">The <see cref="AbstractGraph{TEdge, TNode}"/> in which this <see cref="DemandPair"/> exists.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node1"/> or <paramref name="node2"/> is <see langword="null"/>.</exception>
        public DemandPair(uint id, Node node1, Node node2, AbstractGraph<Edge<Node>, Node> tree)
        {
#if !EXPERIMENT
            Utils.NullCheck(node1, nameof(node1), "Trying to create a DemandPair, but the first endpoint of this demandpair is null!");
            Utils.NullCheck(node2, nameof(node2), "Trying to create a DemandPair, but the second endpoint of this demandpair is null!");
            Utils.NullCheck(tree, nameof(tree), "Trying to create a DemandPair, but the tree in which it exist is null!");
#endif
            MockCounter = new Counter();
            ID = id;
            Node1 = node1;
            Node2 = node2;
            Path = new CountedCollection<Edge<Node>>(tree.NodePathToEdgePath(DFS.FindPathBetween(node1, node2, MockCounter)), MockCounter);
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of this <see cref="DemandPair"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of this <see cref="DemandPair"/>.</returns>
        public override string ToString()
        {
            return $"Demand pair {ID} ({Node1}, {Node2})";
        }

        /// <summary>
        /// Checks whether <paramref name="edge"/> is on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="edge">The <see cref="Edge{TNode}"/> for which we want to know whether it is part of this demand path.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> is part of the path of this <see cref="DemandPair"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool EdgeIsPartOfPath(Edge<Node> edge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Endpoint1, nameof(edge.Endpoint1), "Trying to see if an edge is part of a demandpair, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Endpoint2, nameof(edge.Endpoint2), "Trying to see if an edge is part of a demandpair, but the second endpoint of the edge is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to see if an edge is part of a demandpair, but the counter is null!");
#endif
            return Path.Contains(edge, counter);
        }

        /// <summary>
        /// Returns the number of edges on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of edges on the (unique) path between the endpoints of this <see cref="DemandPair"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int LengthOfPath(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the legnth of a demand path, but the counter is null!");
#endif
            return Path.Count(counter);
        }

        /// <summary>
        /// Returns the edge before and after <paramref name="edge"/> on the path of this <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="edge">The edge for which we want to know the edge before and after it.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <returns>A tuple with the edge before and after <paramref name="edge"/>, or <see langword="null"/> if any of these does not exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public (Edge<Node>, Edge<Node>) EdgeBeforeAndAfter(Edge<Node> edge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to get the edge before and after a given edge on a demand path, but the edge is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to get the edge before and after a given edge on a demand path, but the counter is null!");
#endif
            return Path.ElementBeforeAndAfter(edge, counter);
        }

        /// <summary>
        /// Returns both edges connected to <paramref name="node"/> on the path of this <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="node">The node for which we want to know the edge before and after it.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <returns>A tuple with the edge before and after <paramref name="node"/>, or <see langword="null"/> if any of these does not exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public (Edge<Node>, Edge<Node>) EdgeBeforeAndAfter(Node node, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), "Trying to get the edge before and after a given node on a demand path, but the node is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to get the edge before and after a given node on a demand path, but the counter is null!");
#endif
            Edge<Node> edge = Path.First(e => e.HasEndpoint(node), counter);
            (Edge<Node> before, Edge<Node> after) = EdgeBeforeAndAfter(edge, counter);
            if (!(before is null) && before.HasEndpoint(node))
            {
                return (before, edge);
            }
            return (edge, after);
        }

        /// <summary>
        /// The publically visible <see cref="CountedEnumerable{T}"/> of <see cref="Edge{TNode}"/> that represent the edges on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all edges on the unique path between the endpoints of this <see cref="DemandPair"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<Edge<Node>> EdgesOnDemandPath(Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(counter, nameof(counter), "Trying to get the edges on the path of a demand pair, but the counter is null!");
#endif
            return new CountedEnumerable<Edge<Node>>(Path.GetLinkedList(), counter);
        }

        /// <summary>
        /// Executed when one of the endpoints of this <see cref="DemandPair"/> changes.
        /// <br/>
        /// <b>NOTE:</b> A demand path can only be shortened. <paramref name="newEndpoint"/> should already be on the path between <see cref="Node1"/> and <see cref="Node2"/>.
        /// </summary>
        /// <param name="oldEndpoint">The old endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="newEndpoint">The new endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with the edges that were removed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="oldEndpoint"/>, <paramref name="newEndpoint"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotOnDemandPathException">Thrown when <paramref name="oldEndpoint"/> is not equal to <see cref="Node1"/> or <see cref="Node2"/>.</exception>
        /// <exception cref="ZeroLengthDemandPathException">Thrown when <paramref name="newEndpoint"/> is equal to the endpoint that is not <paramref name="oldEndpoint"/>, meaning the result of this new <see cref="DemandPair"/> would be between the same two <see cref="Node"/>s.</exception>
        internal IEnumerable<Edge<Node>> ChangeEndpoint(Node oldEndpoint, Node newEndpoint, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(oldEndpoint, nameof(oldEndpoint), $"Trying to change the endpoint of {this}, but the old endpoint is null!");
            Utils.NullCheck(newEndpoint, nameof(newEndpoint), $"Trying to change the endpoint of {this}, but the new endpoint is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to change the endpoint of {this}, but the counter is null!");
            if (oldEndpoint != Node1 && oldEndpoint != Node2)
            {
                throw new NotOnDemandPathException($"Trying to change the endpoint of {this}, but the old endpoint given as argument is not on this demand path!");
            }
#endif
            List<Edge<Node>> result = new List<Edge<Node>>();

            if (oldEndpoint == Node1)
            {
                if (newEndpoint == Node2)
                {
                    throw new ZeroLengthDemandPathException($"Trying to change the endpoint of {this}, but it would result in a demand path of zero length!");
                }
                Node1 = newEndpoint;

                result = Path.RemoveFromStartWhile(e => !e.HasEndpoint(newEndpoint), counter).ToList();
                result.AddRange(Path.RemoveFromStart(1, counter));
            }
            else if (oldEndpoint == Node2)
            {
                if (newEndpoint == Node1)
                {
                    throw new ZeroLengthDemandPathException($"Trying to change the endpoint of {this}, but it would result in a demand path of zero length!");
                }
                Node2 = newEndpoint;

                result = Path.RemoveFromEndWhile(e => !e.HasEndpoint(newEndpoint), counter).ToList();
                result.AddRange(Path.RemoveFromEnd(1, counter));
            }

            return result;
        }

        /// <summary>
        /// Update this <see cref="DemandPair"/> when an edge on it was contracted.
        /// </summary>
        /// <param name="contractedEdge">The <see cref="Edge{TNode}"/> that was contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdge"/>, <paramref name="newNode"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="ZeroLengthDemandPathException">Thrown when the contraction of <paramref name="contractedEdge"/> means this <see cref="DemandPair"/> now consists between the same nodes.</exception>
        internal void OnEdgeContracted(Edge<Node> contractedEdge, Node newNode, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdge, nameof(contractedEdge), $"Trying to update {this} after the contraction of an edge, but the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Endpoint1} and {contractedEdge.Endpoint2}, but the new node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Endpoint1} and {contractedEdge.Endpoint2}, but the counter is null!");
            if ((contractedEdge.Endpoint1 == Node1 && contractedEdge.Endpoint2 == Node2) || (contractedEdge.Endpoint1 == Node2 && contractedEdge.Endpoint2 == Node1))
            {
                throw new ZeroLengthDemandPathException($"After contracting edge {contractedEdge}, {this} is now a demand pair of length zero!");
            }
#endif
            UpdateEndpointsAfterEdgeContraction(contractedEdge, newNode);
            UpdateEdgesOnPathAfterEdgeContraction(contractedEdge, counter);
        }

        /// <summary>
        /// Update <see cref="Node1"/> and <see cref="Node2"/> after an edge was contracted.
        /// </summary>
        /// <param name="contractedEdge">The <see cref="Edge{TNode}"/> that was contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdge"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        public void UpdateEndpointsAfterEdgeContraction(Edge<Node> contractedEdge, Node newNode)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdge, nameof(contractedEdge), $"Trying to update the endpoints of {this} after the contraction of an edge, but the edge is null!");
            Utils.NullCheck(contractedEdge.Endpoint2, nameof(contractedEdge.Endpoint2), $"Trying to update the endpoints of {this} after the contraction of the edge between {contractedEdge.Endpoint1} and {contractedEdge.Endpoint2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update the endpoints of {this} after the contraction of the edge between {contractedEdge.Endpoint1} and {contractedEdge.Endpoint2}, but the new node is null!");
#endif
            if (contractedEdge.Endpoint1 == Node1 || contractedEdge.Endpoint2 == Node1)
            {
                Node1 = newNode;
            }
            else if (contractedEdge.Endpoint1 == Node2 || contractedEdge.Endpoint2 == Node2)
            {
                Node2 = newNode;
            }
        }

        /// <summary>
        /// Update the edges on the path of this <see cref="DemandPair"/> after an edge was contracted.
        /// </summary>
        /// <param name="contractedEdge">The <see cref="Edge{TNode}"/> that was contracted.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdge"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotOnDemandPathException">Thrown when <paramref name="contractedEdge"/> was not part of this <see cref="DemandPair"/>.</exception>
        private void UpdateEdgesOnPathAfterEdgeContraction(Edge<Node> contractedEdge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdge, nameof(contractedEdge), $"Trying to update the edges on the path of {this} after the contraction of an edge, but the edge is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Endpoint1} and {contractedEdge.Endpoint2}, but the counter is null!");
            if (!Path.Contains(contractedEdge, counter))
            {
                throw new NotOnDemandPathException($"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Endpoint1} and {contractedEdge.Endpoint2}, but this edge is not part of the demand path of {this}!");
            }
#endif
            Path.Remove(contractedEdge, counter);
        }
    }
}
