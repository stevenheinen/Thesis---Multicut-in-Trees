// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
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
        private CountedCollection<(TreeNode, TreeNode)> Path { get; set; }

        /// <summary>
        /// The first endpoint of this <see cref="DemandPair"/>.
        /// </summary>
        public TreeNode Node1 { get; private set; }

        /// <summary>
        /// The second endpoint of this <see cref="DemandPair"/>.
        /// </summary>
        public TreeNode Node2 { get; private set; }

        /// <summary>
        /// The publically visible <see cref="IEnumerable{T}"/> of tuples of <see cref="TreeNode"/>s that represent the edges on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        public CountedCollection<(TreeNode, TreeNode)> EdgesOnDemandPath => Path;

        /// <summary>
        /// <see cref="Counter"/> that can be used for modifications that should not impact the performance of an <see cref="Algorithms.Algorithm"/> or <see cref="ReductionRules.ReductionRule"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for a <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="node1">The first endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="node2">The second endpoint of this <see cref="DemandPair"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node1"/> or <paramref name="node2"/> is <see langword="null"/>.</exception>
        public DemandPair(TreeNode node1, TreeNode node2)
        {
#if !EXPERIMENT
            Utils.NullCheck(node1, nameof(node1), "Trying to create a DemandPair, but the first endpoint of this demandpair is null!");
            Utils.NullCheck(node2, nameof(node2), "Trying to create a DemandPair, but the second endpoint of this demandpair is null!");
#endif
            MockCounter = new Counter();
            Node1 = node1;
            Node2 = node2;
            Path = new CountedCollection<(TreeNode, TreeNode)>(Utils.NodePathToEdgePath(DFS.FindPathBetween(node1, node2, MockCounter)), MockCounter);
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of this <see cref="DemandPair"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of this <see cref="DemandPair"/>.</returns>
        public override string ToString()
        {
            return $"Demand pair ({Node1}, {Node2})";
        }

        /// <summary>
        /// Checks whether <paramref name="edge"/> is on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="edge">The tuple of two <see cref="TreeNode"/>s for which we want to know whether it is part of this demand path.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <returns><see langword="true"/> if <paramref name="edge"/> is part of the path of this <see cref="DemandPair"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool EdgeIsPartOfPath((TreeNode, TreeNode) edge, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to see if an edge is part of a demandpair, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to see if an edge is part of a demandpair, but the second endpoint of the edge is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to see if an edge is part of a demandpair, but the counter is null!");
#endif
            return Path.Contains(edge, counter) || Path.Contains((edge.Item2, edge.Item1), counter);
        }

        /// <summary>
        /// Executed when one of the endpoints of this <see cref="DemandPair"/> changes.
        /// <br/>
        /// <b>NOTE:</b> A demand path can only be shortened. <paramref name="newEndpoint"/> should already be on the path between <see cref="Node1"/> and <see cref="Node2"/>.
        /// </summary>
        /// <param name="oldEndpoint">The old endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="newEndpoint">The new endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="oldEndpoint"/>, <paramref name="newEndpoint"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotOnDemandPathException">Thrown when <paramref name="oldEndpoint"/> is not equal to <see cref="Node1"/> or <see cref="Node2"/>.</exception>
        /// <exception cref="ZeroLengthDemandPathException">Thrown when <paramref name="newEndpoint"/> is equal to the endpoint that is not <paramref name="oldEndpoint"/>, meaning the result of this new <see cref="DemandPair"/> would be between the same two <see cref="TreeNode"/>s.</exception>
        internal void ChangeEndpoint(TreeNode oldEndpoint, TreeNode newEndpoint, Counter counter)
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
            if (oldEndpoint == Node1)
            {
                if (newEndpoint == Node2)
                {
                    throw new ZeroLengthDemandPathException($"Trying to change the endpoint of {this}, but it would result in a demand path of zero length!");
                }
                Node1 = newEndpoint;

                Path.RemoveFromStartWhile(n => n.Item1 != Node1, counter);
            }
            else if (oldEndpoint == Node2)
            {
                if (newEndpoint == Node1)
                {
                    throw new ZeroLengthDemandPathException($"Trying to change the endpoint of {this}, but it would result in a demand path of zero length!");
                }
                Node2 = newEndpoint;

                Path.RemoveFromEndWhile(n => n.Item2 != Node2, counter);
            }
        }

        /// <summary>
        /// Update this <see cref="DemandPair"/> when an edge on it was contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of <see cref="TreeNode"/>s that represents the edge that was contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the elements of <paramref name="contractedEdge"/>, <paramref name="newNode"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="ZeroLengthDemandPathException">Thrown when the contraction of <paramref name="contractedEdge"/> means this <see cref="DemandPair"/> now consists between the same nodes.</exception>
        internal void OnEdgeContracted((TreeNode, TreeNode) contractedEdge, TreeNode newNode, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the first endpoint of the edge is null!");
            Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the new node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the counter is null!");
            if ((contractedEdge.Item1 == Node1 && contractedEdge.Item2 == Node2) || (contractedEdge.Item1 == Node2 && contractedEdge.Item2 == Node1))
            {
                throw new ZeroLengthDemandPathException($"After contracting edge {contractedEdge}, {this} is now a demand pair of length zero!");
            }
#endif
            UpdateEndpointsAfterEdgeContraction(contractedEdge, newNode);
            UpdateEdgesOnPathAfterEdgeContraction(contractedEdge, newNode, counter);
        }

        /// <summary>
        /// Update this <see cref="DemandPair"/> when an edge incident to one of the <see cref="TreeNode"/>s on this <see cref="DemandPair"/>'s path is changed.
        /// </summary>
        /// <param name="oldNode">The <see cref="TreeNode"/> that is on this <see cref="DemandPair"/>, but will be removed by the edge contraction.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the edge contraction.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="oldNode"/>, <paramref name="newNode"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        internal void OnEdgeNextToNodeOnDemandPathContracted(TreeNode oldNode, TreeNode newNode, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(oldNode, nameof(oldNode), "Trying to update a demand pair when one of the endpoints of the contracted edge is part of this demand pair, but the old node is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to update a demand pair when one of the endpoints of the contracted edge is part of this demand pair, but the new node is null!");
            Utils.NullCheck(counter, nameof(counter), "Trying to update a demand pair when one of the endpoints of the contracted edge is part of this demand pair, but the counter is null!");
#endif
            // TODO: Should not be necessary.
            if (oldNode == newNode)
            {
                return;
            }

            Path.ChangeOccurrence(edge => edge.Item1 == oldNode ? (newNode, edge.Item2) : edge.Item2 == oldNode ? (edge.Item1, newNode) : edge, counter);
        }

        /// <summary>
        /// Update this <see cref="DemandPair"/> when an edge that is next to either <see cref="Node1"/> or <see cref="Node2"/> is contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of <see cref="TreeNode"/>s representing the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the edge contraction.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="contractedEdge"/>, <paramref name="newNode"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        internal void OnEdgeNextToDemandPathEndpointsContracted((TreeNode, TreeNode) contractedEdge, TreeNode newNode, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the first endpoint of the edge is null!");
            Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the new node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the counter is null!");
            if ((contractedEdge.Item1 == Node1 && contractedEdge.Item2 == Node2) || (contractedEdge.Item1 == Node2 && contractedEdge.Item2 == Node1))
            {
                throw new ZeroLengthDemandPathException($"After contracting edge {contractedEdge}, {this} is now a demand pair of length zero!");
            }
#endif
            if (contractedEdge.Item1 == Node1 || contractedEdge.Item2 == Node1)
            {
                Node1 = newNode;

                (TreeNode, TreeNode) firstElement = Path.First(counter);
                Path.ChangeElement(firstElement, (newNode, firstElement.Item2), counter);
            }
            else if (contractedEdge.Item1 == Node2 || contractedEdge.Item2 == Node2)
            {
                Node2 = newNode;

                (TreeNode, TreeNode) lastElement = Path.Last(counter);
                Path.ChangeElement(lastElement, (lastElement.Item1, newNode), counter);
            }
            else
            {
                throw new InvalidOperationException($"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but this edge is not incident to the endpoints of this demand path!");
            }
        }

        /// <summary>
        /// Update <see cref="Node1"/> and <see cref="Node2"/> after an edge was contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of <see cref="TreeNode"/>s that represents the edge that was contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the elements of <paramref name="contractedEdge"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        private void UpdateEndpointsAfterEdgeContraction((TreeNode, TreeNode) contractedEdge, TreeNode newNode)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), $"Trying to update the endpoints of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the first endpoint of the edge is null!");
            Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), $"Trying to update the endpoints of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update the endpoints of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the new node is null!");
#endif
            if (contractedEdge.Item1 == Node1 || contractedEdge.Item2 == Node1)
            {
                Node1 = newNode;
            }
            else if (contractedEdge.Item1 == Node2 || contractedEdge.Item2 == Node2)
            {
                Node2 = newNode;
            }
        }

        /// <summary>
        /// Update the edges on the path of this <see cref="DemandPair"/> after an edge was contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of <see cref="TreeNode"/>s that represents the edge that was contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <param name="counter">The <see cref="Counter"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the elements of <paramref name="contractedEdge"/>, <paramref name="newNode"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotOnDemandPathException">Thrown when <paramref name="contractedEdge"/> was not part of this <see cref="DemandPair"/>.</exception>
        private void UpdateEdgesOnPathAfterEdgeContraction((TreeNode, TreeNode) contractedEdge, TreeNode newNode, Counter counter)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), $"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the first endpoint of the edge is null!");
            Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), $"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the new node is null!");
            Utils.NullCheck(counter, nameof(counter), $"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the counter is null!");
#endif
            // Update the list of edges on this demand path.
            (TreeNode, TreeNode) usedEdge = contractedEdge;
            if (!Path.Contains(usedEdge, counter))
            {
                usedEdge = (contractedEdge.Item2, contractedEdge.Item1);
                if (!Path.Contains(usedEdge, counter))
                {
                    throw new NotOnDemandPathException($"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but this edge is not part of the demand path of {this}!");
                }
            }

            ((TreeNode, TreeNode) before, (TreeNode, TreeNode) after) = Path.ElementBeforeAndAfter(usedEdge, counter);
            if (!(before.Item1 is null || before.Item2 is null))
            {
                Path.ChangeElement(before, (before.Item1, newNode), counter);
            }
            if (!(after.Item1 is null || after.Item2 is null))
            {
                Path.ChangeElement(after, (newNode, after.Item2), counter);
            }
            Path.Remove(usedEdge, counter);
        }
    }
}
