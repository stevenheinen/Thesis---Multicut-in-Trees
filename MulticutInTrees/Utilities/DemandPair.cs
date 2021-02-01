// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Class that represents a demand pair in the input.
    /// </summary>
    public class DemandPair
    {
        /// <summary>
        /// The private <see cref="List{T}"/> of <see cref="TreeNode"/>s that represent all nodes on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        private List<TreeNode> InternalDemandPath { get; set; }

        /// <summary>
        /// The private <see cref="List{T}"/> of tuples of <see cref="TreeNode"/>s that represent the edges on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        private List<(TreeNode, TreeNode)> InternalEdgesOnDemandPath { get; set; }

        /// <summary>
        /// The first endpoint of this <see cref="DemandPair"/>.
        /// </summary>
        public TreeNode Node1 { get; private set; }

        /// <summary>
        /// The second endpoint of this <see cref="DemandPair"/>.
        /// </summary>
        public TreeNode Node2 { get; private set; }

        /// <summary>
        /// The publically visible <see cref="ReadOnlyCollection{T}"/> of all <see cref="TreeNode"/>s on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        public ReadOnlyCollection<TreeNode> DemandPath => InternalDemandPath.AsReadOnly();

        /// <summary>
        /// The publically visible <see cref="ReadOnlyCollection{T}"/> of tuples of <see cref="TreeNode"/>s that represent the edges on the path between the endpoints of this <see cref="DemandPair"/>.
        /// </summary>
        public ReadOnlyCollection<(TreeNode, TreeNode)> EdgesOnDemandPath => InternalEdgesOnDemandPath.AsReadOnly();

        /// <summary>
        /// Constructor for a <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="node1">The first endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="node2">The second endpoint of this <see cref="DemandPair"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node1"/> or <paramref name="node2"/> is <see langword="null"/>.</exception>
        public DemandPair(TreeNode node1, TreeNode node2)
        {
            Utils.NullCheck(node1, nameof(node1), $"Trying to create a DemandPair, but the first endpoint of this demandpair is null!");
            Utils.NullCheck(node2, nameof(node2), $"Trying to create a DemandPair, but the second endpoint of this demandpair is null!");

            Node1 = node1;
            Node2 = node2;
            InternalDemandPath = new List<TreeNode>(DFS.FindPathBetween(node1, node2));
            InternalEdgesOnDemandPath = new List<(TreeNode, TreeNode)>();
            CreateEdgesOnPath();
        }

        /// <summary>
        /// Creates the <see cref="List{T}"/> of all edges on the path between <see cref="Node1"/> and <see cref="Node2"/> from <see cref="InternalDemandPath"/>.
        /// </summary>
        private void CreateEdgesOnPath()
        {
            InternalEdgesOnDemandPath.Clear();
            for (int i = 0; i < InternalDemandPath.Count - 1; i++)
            {
                InternalEdgesOnDemandPath.Add((InternalDemandPath[i], InternalDemandPath[i + 1]));
            }
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
        /// Executed when one of the endpoints of this <see cref="DemandPair"/> changes.
        /// <br/>
        /// <b>NOTE:</b> A demand path can only be shortened. <paramref name="newEndpoint"/> should already be on the path between <see cref="Node1"/> and <see cref="Node2"/>.
        /// </summary>
        /// <param name="oldEndpoint">The old endpoint of this <see cref="DemandPair"/>.</param>
        /// <param name="newEndpoint">The new endpoint of this <see cref="DemandPair"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="oldEndpoint"/> or <paramref name="newEndpoint"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotOnDemandPathException">Thrown when <paramref name="oldEndpoint"/> is not equal to <see cref="Node1"/> or <see cref="Node2"/>.</exception>
        /// <exception cref="ZeroLengthDemandPathException">Thrown when <paramref name="newEndpoint"/> is equal to the endpoint that is not <paramref name="oldEndpoint"/>, meaning the result of this new <see cref="DemandPair"/> would be between the same two <see cref="TreeNode"/>s.</exception>
        public void ChangeEndpoint(TreeNode oldEndpoint, TreeNode newEndpoint)
        {
            Utils.NullCheck(oldEndpoint, nameof(oldEndpoint), $"Trying to change the endpoint of {this}, but the old endpoint is null!");
            Utils.NullCheck(newEndpoint, nameof(newEndpoint), $"Trying to change the endpoint of {this}, but the new endpoint is null!");
            if (oldEndpoint != Node1 && oldEndpoint != Node2)
            {
                throw new NotOnDemandPathException($"Trying to change the endpoint of {this}, but the old endpoint given as argument is not on this demand path!");
            }

            bool shorter = false;
            if (InternalDemandPath.Contains(newEndpoint))
            {
                shorter = true;
            }

            if (oldEndpoint == Node1)
            {
                if (newEndpoint == Node2)
                {
                    throw new ZeroLengthDemandPathException($"Trying to change the endpoint of {this}, but it would result in a demand path of zero length!");
                }
                Node1 = newEndpoint;

                if (shorter)
                {
                    InternalDemandPath = InternalDemandPath.SkipWhile(n => n != Node1).ToList();
                    InternalEdgesOnDemandPath = InternalEdgesOnDemandPath.SkipWhile(n => n.Item1 != Node1).ToList();
                }
            }
            else if (oldEndpoint == Node2)
            {
                if (newEndpoint == Node1)
                {
                    throw new ZeroLengthDemandPathException($"Trying to change the endpoint of {this}, but it would result in a demand path of zero length!");
                }
                Node2 = newEndpoint;

                if (shorter)
                {
                    InternalDemandPath = InternalDemandPath.TakeWhile(n => n != Node2).ToList();
                    InternalDemandPath.Add(Node2);
                    InternalEdgesOnDemandPath = InternalEdgesOnDemandPath.TakeWhile(n => n.Item1 != Node2).ToList();
                }
            }

            if (!shorter)
            {
                InternalDemandPath = DFS.FindPathBetween(Node1, Node2);
                CreateEdgesOnPath();
            }
        }

        /// <summary>
        /// Update this <see cref="DemandPair"/> when an edge on it was contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of <see cref="TreeNode"/>s that represents the edge that was contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the elements of <paramref name="contractedEdge"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        /// <exception cref="ZeroLengthDemandPathException">Thrown when the contraction of <paramref name="contractedEdge"/> means this <see cref="DemandPair"/> now consists between the same nodes.</exception>
        public void OnEdgeContracted((TreeNode, TreeNode) contractedEdge, TreeNode newNode)
        {
            Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the first endpoint of the edge is null!");
            Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the new node is null!");
            if ((contractedEdge.Item1 == Node1 && contractedEdge.Item2 == Node2) || (contractedEdge.Item1 == Node2 && contractedEdge.Item2 == Node1))
            {
                throw new ZeroLengthDemandPathException($"After contracting edge {contractedEdge}, {this} is now a demand pair of length zero!");
            }

            // Update the nodes and edges of the demand path.
            UpdateEndpointsAfterEdgeContraction(contractedEdge, newNode);
            UpdateNodesOnPathAfterEdgeContraction(contractedEdge, newNode);
            UpdateEdgesOnPathAfterEdgeContraction(contractedEdge, newNode);
        }

        /// <summary>
        /// Update <see cref="Node1"/> and <see cref="Node2"/> after an edge was contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of <see cref="TreeNode"/>s that represents the edge that was contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the elements of <paramref name="contractedEdge"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        private void UpdateEndpointsAfterEdgeContraction((TreeNode, TreeNode) contractedEdge, TreeNode newNode)
        {
            Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), $"Trying to update the endpoints of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the first endpoint of the edge is null!");
            Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), $"Trying to update the endpoints of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update the endpoints of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the new node is null!");

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
        /// Update the nodes on the path of this <see cref="DemandPair"/> after an edge was contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of <see cref="TreeNode"/>s that represents the edge that was contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the elements of <paramref name="contractedEdge"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotOnDemandPathException">Thrown when <paramref name="contractedEdge"/> was not part of this <see cref="DemandPair"/>.</exception>
        private void UpdateNodesOnPathAfterEdgeContraction((TreeNode, TreeNode) contractedEdge, TreeNode newNode)
        {
            Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), $"Trying to update the nodes on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the first endpoint of the edge is null!");
            Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), $"Trying to update the nodes on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update the nodes on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the new node is null!");
            
            // Find the index of the first index of the contracted edge.
            int index = InternalDemandPath.FindIndex(n => n == contractedEdge.Item1);
            if (index == -1)
            {
                throw new NotOnDemandPathException($"Trying to update the nodes on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but {contractedEdge.Item1} is not part of the demand path of {this}!");
            }
            if (index > 0 && InternalDemandPath[index - 1] == contractedEdge.Item2)
            {
                // If the second node of the contracted edge is in front of the first node.
                InternalDemandPath[index - 1] = newNode;
                InternalDemandPath.RemoveAt(index);
            }
            else if (index < InternalDemandPath.Count - 1 && InternalDemandPath[index + 1] == contractedEdge.Item2)
            {
                // If the second node of the contracted edge is after the first node.
                InternalDemandPath[index] = newNode;
                InternalDemandPath.RemoveAt(index + 1);
            }
            else
            {
                // We were either not able to find the second endpoint of the contracted edge, or the contracted edge is not part of this demand path.
                throw new NotOnDemandPathException($"Trying to update the nodes on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but {contractedEdge.Item1} and {contractedEdge.Item2} are not next to each other on the demand path of {this}!");
            }
        }

        /// <summary>
        /// Update the edges on the path of this <see cref="DemandPair"/> after an edge was contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of <see cref="TreeNode"/>s that represents the edge that was contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="contractedEdge"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the elements of <paramref name="contractedEdge"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotOnDemandPathException">Thrown when <paramref name="contractedEdge"/> was not part of this <see cref="DemandPair"/>.</exception>
        private void UpdateEdgesOnPathAfterEdgeContraction((TreeNode, TreeNode) contractedEdge, TreeNode newNode)
        {
            Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), $"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the first endpoint of the edge is null!");
            Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), $"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the second endpoint of the edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), $"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but the new node is null!");
            
            // Update the list of edges on this demand path.
            (TreeNode, TreeNode) flippedEdge = (contractedEdge.Item2, contractedEdge.Item1);
            int index = InternalEdgesOnDemandPath.FindIndex(n => n == contractedEdge || n == flippedEdge);
            if (index == -1)
            {
                throw new NotOnDemandPathException($"Trying to update the edges on the path of {this} after the contraction of the edge between {contractedEdge.Item1} and {contractedEdge.Item2}, but this edge is not part of the demand path of {this}!");
            }
            if (index < InternalEdgesOnDemandPath.Count - 1)
            {
                InternalEdgesOnDemandPath[index + 1] = (newNode, InternalEdgesOnDemandPath[index + 1].Item2);
            }
            if (index > 0)
            {
                InternalEdgesOnDemandPath[index - 1] = (InternalEdgesOnDemandPath[index - 1].Item1, newNode);
            }
            InternalEdgesOnDemandPath.RemoveAt(index);
        }
    }
}
