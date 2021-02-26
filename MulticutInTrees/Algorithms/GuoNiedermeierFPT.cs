// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.Exceptions;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Implementation of the FPT algorithm by Guo and Niedermeier.
    /// <br/>
    /// Source: <see href="https://doi.org/10.1002/net.20081"/>
    /// </summary>
    public class GuoNiedermeierFPT : Algorithm
    {
        /// <summary>
        /// <see cref="Dictionary{TKey, TValue}"/> containing a <see cref="List{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> DemandPairsPerEdge { get; set; }

        /// <summary>
        /// <see cref="Dictionary{TKey, TValue}"/> containing a <see cref="List{T}"/> of <see cref="DemandPair"/> per <see cref="TreeNode"/>.
        /// </summary>
        private CountedDictionary<TreeNode, List<DemandPair>> DemandPairsPerNode { get; set; }

        // todo: comment
        private Counter DemandPairsPerEdgeValueCounter { get; }

        /// <summary>
        /// Constructor for <see cref="GuoNiedermeierFPT"/>.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public GuoNiedermeierFPT(MulticutInstance instance) : base(instance)
        {
            Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of a the Guo-Niedermeier FPT algorithm, but the problem instance is null!");

            DemandPairsPerEdgeValueCounter = new Counter();

            Preprocess();
        }

        /// <summary>
        /// Create the <see cref="List{T}"/> of <see cref="ReductionRule"/>s used by this algorithm.
        /// </summary>
        private void CreateReductionRules()
        {
            List<ReductionRule> reductionRules = new List<ReductionRule>();

            IdleEdge idleEdge = new IdleEdge(Tree, DemandPairs, this, Random, DemandPairsPerEdge);
            reductionRules.Add(idleEdge);

            UnitPath unitPath = new UnitPath(Tree, DemandPairs, this, Random, DemandPairsPerEdge);
            reductionRules.Add(unitPath);

            DominatedEdge dominatedEdge = new DominatedEdge(Tree, DemandPairs, this, Random, DemandPairsPerEdge);
            reductionRules.Add(dominatedEdge);

            DominatedPath dominatedPath = new DominatedPath(Tree, DemandPairs, this, Random, DemandPairsPerEdge);
            reductionRules.Add(dominatedPath);

            // TODO: add other reduction rules.
            // ...

            ReductionRules = new ReadOnlyCollection<ReductionRule>(reductionRules);
        }

        /// <summary>
        /// Fills <see cref="DemandPairsPerEdge"/> and <see cref="DemandPairsPerNode"/>.
        /// </summary>
        private void FillDemandPathsPerEdge()
        {
            DemandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>();
            DemandPairsPerNode = new CountedDictionary<TreeNode, List<DemandPair>>();

            // For each demand pair in the instance...
            foreach (DemandPair demandPair in DemandPairs.GetCountedEnumerable(new Counter()))
            {
                if (!DemandPairsPerNode.ContainsKey(demandPair.Node1))
                {
                    DemandPairsPerNode[demandPair.Node1] = new List<DemandPair>();
                }
                if (!DemandPairsPerNode.ContainsKey(demandPair.Node2))
                {
                    DemandPairsPerNode[demandPair.Node2] = new List<DemandPair>();
                }
                DemandPairsPerNode[demandPair.Node1].Add(demandPair);
                DemandPairsPerNode[demandPair.Node2].Add(demandPair);

                // For each edge on this demand pair...
                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath.GetCountedEnumerable(new Counter()))
                {
                    // Add this edge to the DemandPairsPerEdge dictionary.
                    (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);
                    if (!DemandPairsPerEdge.ContainsKey(usedEdge))
                    {
                        DemandPairsPerEdge[usedEdge] = new CountedList<DemandPair>(DemandPairsPerEdgeValueCounter);
                    }
                    DemandPairsPerEdge[usedEdge].Add(demandPair);
                }
            }
        }

        /// <summary>
        /// Removes a <see cref="DemandPair"/> from a given edge in <see cref="DemandPairsPerEdge"/>.
        /// </summary>
        /// <param name="edge">The edge from which <paramref name="demandPair"/> should be removed.</param>
        /// <param name="demandPair">The <see cref="DemandPair"/> that should be removed from <paramref name="edge"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/> or <paramref name="demandPair"/> is <see langword="null"/>.</exception>
        private void RemoveDemandPairFromEdge((TreeNode, TreeNode) edge, DemandPair demandPair)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to remove a demand pair from an edge, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to remove a demand pair from an edge, but the second endpoint of the edge is null");
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to remove a demand pair from an edge, but the demand pair is null!");

            (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);
            DemandPairsPerEdge[usedEdge].Remove(demandPair);
            
            // If, after removing this demand pair, there are no more demand pairs going over this edge, remove it from the dictionary.
            if (DemandPairsPerEdge[usedEdge].Count == 0)
            {
                DemandPairsPerEdge.Remove(usedEdge);
            }
        }

        /// <inheritdoc/>
        protected override void PrintCounters()
        {
            Console.WriteLine();
            Console.WriteLine($"GuoNiedermeier FPT counters:");
            Console.WriteLine($"==============================");
            Console.WriteLine($"DemandPairsPerEdge:       {DemandPairsPerEdge.OperationsCounter}");
            Console.WriteLine($"DemandPairsPerNode:       {DemandPairsPerNode.OperationsCounter}");
            Console.WriteLine($"DemandPairsPerEdgeValues: {DemandPairsPerEdgeValueCounter}");
            base.PrintCounters();
            Console.WriteLine();
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            FillDemandPathsPerEdge();
            CreateReductionRules();
        }

        /// <summary>
        /// Internal method for cutting an edge. Also returns the <see cref="TreeNode"/> that is the result of the edge contraction. See also: <seealso cref="CutEdge(ValueTuple{TreeNode, TreeNode})"/>.
        /// </summary>
        /// <param name="edge">The edge to be cut, represented by a tuple of two <see cref="TreeNode"/>s.</param>
        /// <returns>The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="edge"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/> is <see langword="null"/>.</exception>
        private TreeNode InternalCutEdge((TreeNode, TreeNode) edge)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to cut an edge, but the first item of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to cut an edge, but the second item of this edge is null!");
            if (!Tree.HasEdge(edge))
            {
                throw new NotInGraphException($"Trying to cut edge {edge}, but this edge is not part of the tree!");
            }
            if (edge.Item1 == edge.Item2)
            {
                throw new InvalidEdgeException($"Trying to cut edge {edge}, but this edge is a self loop and should not exist!");
            }

            PartialSolution.Add(edge);
            List<DemandPair> separatedDemandPairs = new List<DemandPair>(DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(edge)]);
            
            RemoveDemandPairs(separatedDemandPairs);
            TreeNode res = InternalContractEdge(edge);

            foreach (DemandPair demandPair in separatedDemandPairs)
            {
                if (demandPair.EdgesOnDemandPath.Count == 1)
                {
                    continue;
                }
                demandPair.OnEdgeContracted(edge, res);
            }

            return res;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either item of <paramref name="edge"/> is <see langword="null"/>.</exception>
        internal override void CutEdge((TreeNode, TreeNode) edge)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to cut an edge, but the first item of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to cut an edge, but the second item of this edge is null!");

            InternalCutEdge(edge);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> is <see langword="null"/>.</exception>
        internal override void CutEdges(IList<(TreeNode, TreeNode)> edges)
        {
            Utils.NullCheck(edges, nameof(edges), "Trying to cut multiple edges, but the IEnumerable of edges is null!");

            for (int i = 0; i < edges.Count(); i++)
            {
                TreeNode newNode = InternalCutEdge(edges[i]);
                for (int j = i + 1; j < edges.Count(); j++)
                {
                    if (edges[i].Item1 == edges[j].Item1 || edges[i].Item2 == edges[j].Item1)
                    {
                        edges[j] = (newNode, edges[j].Item2);
                    }
                    if (edges[i].Item1 == edges[j].Item2 || edges[i].Item2 == edges[j].Item2)
                    {
                        edges[j] = (edges[j].Item1, newNode);
                    }
                }
            }
        }

        /// <summary>
        /// Internal method for contracting an edge. Also returns the <see cref="TreeNode"/> that is the result of the edge contraction. See also: <seealso cref="ContractEdge(ValueTuple{TreeNode, TreeNode})"/>.
        /// </summary>
        /// <param name="edge">The edge to be contracted, represented by a tuple of two <see cref="TreeNode"/>s.</param>
        /// <returns>The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="edge"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="edge"/> is not part of the input.</exception>
        /// <exception cref="InvalidEdgeException">Thrown when <paramref name="edge"/> is a self-loop.</exception>
        private TreeNode InternalContractEdge((TreeNode, TreeNode) edge)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to contract an edge, but the first item of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item1), "Trying to contract an edge, but the second item of this edge is null!");
            if (!Tree.HasEdge(edge))
            {
                throw new NotInGraphException($"Trying to contract edge {edge}, but this edge is not part of the tree!");
            }
            if (edge.Item1 == edge.Item2)
            {
                throw new InvalidEdgeException($"Trying to contract edge {edge}, but this edge is a self loop and should not exist!");
            }

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine($"Contracting edge {edge}!");
            }

            TreeNode parent = edge.Item1;
            TreeNode child = edge.Item2;
            if (parent.Parent == child)
            {
                parent = edge.Item2;
                child = edge.Item1;
            }

            TreeNode newNode = parent;
            (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);
            UpdateNodeTypesDuringEdgeContraction(usedEdge, newNode);
            Tree.RemoveNode(child);
            CountedList<DemandPair> pairsOnEdge = RemoveDemandPairsFromContractedEdge(usedEdge, newNode);
            UpdateDemandPairsStartingAtContractedEdge(usedEdge, child, newNode, pairsOnEdge);
            UpdateDemandPairsGoingThroughChild(child, newNode);

            LastContractedEdges.Add((usedEdge, newNode, pairsOnEdge));
            LastIterationEdgeContraction = true;

            return newNode;
        }

        /// <summary>
        /// Update the <see cref="DemandPair"/>s that pass through <paramref name="child"/>, but not over the edge that is being contracted.
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> that will be removed by the contraction.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        private void UpdateDemandPairsGoingThroughChild(TreeNode child, TreeNode newNode)
        {
            Utils.NullCheck(child, nameof(child), "Trying to update the demand pairs going through the child of an edge that will be contracted, but the child is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to update the demand pairs going through the child of an edge that will be contracted, but the new node is null!");

            List<(TreeNode, TreeNode)> keysToBeRemoved = new List<(TreeNode, TreeNode)>();
            List<(TreeNode, TreeNode)> oldKeys = DemandPairsPerEdge.Keys.Where(n => n.Item1 == child || n.Item2 == child).ToList();
            foreach ((TreeNode, TreeNode) key in oldKeys)
            {
                foreach (DemandPair demandPair in DemandPairsPerEdge[key].GetCountedEnumerable(new Counter()))
                {
                    demandPair.OnEdgeNextToNodeOnDemandPathContracted(child, newNode);
                }

                if (key.Item1 == child)
                {
                    (TreeNode, TreeNode) newKey = Utils.OrderEdgeSmallToLarge((key.Item2, newNode));
                    if (!DemandPairsPerEdge.ContainsKey(newKey))
                    {
                        DemandPairsPerEdge[newKey] = new CountedList<DemandPair>(DemandPairsPerEdgeValueCounter);
                    }
                    DemandPairsPerEdge[newKey].AddRange(DemandPairsPerEdge[key]);
                    keysToBeRemoved.Add(key);
                }
                if (key.Item2 == child)
                {
                    (TreeNode, TreeNode) newKey = Utils.OrderEdgeSmallToLarge((key.Item1, newNode));
                    if (!DemandPairsPerEdge.ContainsKey(newKey))
                    {
                        DemandPairsPerEdge[newKey] = new CountedList<DemandPair>(DemandPairsPerEdgeValueCounter);
                    }
                    DemandPairsPerEdge[newKey].AddRange(DemandPairsPerEdge[key]);
                    keysToBeRemoved.Add(key);
                }
            }
            foreach ((TreeNode, TreeNode) key in keysToBeRemoved)
            {
                DemandPairsPerEdge.Remove(key);
            }

        }

        /// <summary>
        /// Update all <see cref="DemandPair"/>s that start at an edge that will be contracted.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s that represents the edge that is being contracted.</param>
        /// <param name="child">The <see cref="TreeNode"/> that will be removed by the contraction.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction.</param>
        /// <param name="pairsOnEdge">The <see cref="List{T}"/> of <see cref="DemandPair"/>s that go over <paramref name="edge"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, <paramref name="child"/>, <paramref name="newNode"/> or <paramref name="pairsOnEdge"/> is <see langword="null"/>.</exception>
        private void UpdateDemandPairsStartingAtContractedEdge((TreeNode, TreeNode) edge, TreeNode child, TreeNode newNode, CountedList<DemandPair> pairsOnEdge)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the first endpoint of the edge that will be contracted is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the second endpoint of the edge that will be contracted is null!");
            Utils.NullCheck(child, nameof(child), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the node that will be removed by the contraction is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the node that is the result of the contraction is null!");
            Utils.NullCheck(pairsOnEdge, nameof(pairsOnEdge), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the list of demand pairs going through the contracted edge is null!");

            if (DemandPairsPerNode.TryGetValue(child, out List<DemandPair> pairsAtChild))
            {
                if (!DemandPairsPerNode.ContainsKey(newNode))
                {
                    DemandPairsPerNode[newNode] = new List<DemandPair>();
                }
                foreach (DemandPair demandPair in pairsAtChild)
                {
                    // todo: delete?
                    //if (pairsOnEdge.Contains(demandPair))
                    //{
                    //    continue;
                    //}

                    demandPair.OnEdgeNextToDemandPathEndpointsContracted(edge, newNode);
                    DemandPairsPerNode[newNode].Add(demandPair);
                }
                DemandPairsPerNode.Remove(child);
            }
        }

        /// <summary>
        /// Removes all <see cref="DemandPair"/>s from the edge that will be contracted.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s that represents the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction.</param>
        /// <returns>A <see cref="List{T}"/> with all the <see cref="DemandPair"/>s that pass through <paramref name="edge"/>.</returns>
        private CountedList<DemandPair> RemoveDemandPairsFromContractedEdge((TreeNode, TreeNode) edge, TreeNode newNode)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to remove all demand paths going through an edge, but the first endpoint of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to remove all demand paths going through an edge, but the second endpoint of this edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to remove all demand paths going through an edge, but the node that is the result of the contraction is null!");

            if (DemandPairsPerEdge.TryGetValue(edge, out CountedList<DemandPair> pairsOnEdge))
            {
                foreach (DemandPair demandPair in pairsOnEdge.GetCountedEnumerable(new Counter()))
                {
                    demandPair.OnEdgeContracted(edge, newNode);
                }
                DemandPairsPerEdge.Remove(edge);
            }
            else
            {
                pairsOnEdge = new CountedList<DemandPair>();
            }

            return pairsOnEdge;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either item of <paramref name="edge"/> is <see langword="null"/>.</exception>
        internal override void ContractEdge((TreeNode, TreeNode) edge)
        {
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to contract an edge, but the first item of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item1), "Trying to contract an edge, but the second item of this edge is null!");

            InternalContractEdge(edge);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> is <see langword="null"/>.</exception>
        internal override void ContractEdges(IList<(TreeNode, TreeNode)> edges)
        {
            Utils.NullCheck(edges, nameof(edges), "Trying to contract multiple edges, but the IEnumerable of edges is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine($"Contracting edges {edges.Print()}");
            }

            for (int i = 0; i < edges.Count(); i++)
            {
                TreeNode newNode = InternalContractEdge(edges[i]);
                for (int j = i + 1; j < edges.Count(); j++)
                {
                    if (edges[i].Item1 == edges[j].Item1 || edges[i].Item2 == edges[j].Item1)
                    {
                        edges[j] = (newNode, edges[j].Item2);
                    }
                    if (edges[i].Item1 == edges[j].Item2 || edges[i].Item2 == edges[j].Item2)
                    {
                        edges[j] = (edges[j].Item1, newNode);
                    }
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/> is <see langword="null"/>.</exception>
        internal override void RemoveDemandPair(DemandPair demandPair)
        {
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to remove a demand pair, but the demand pair is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine($"Removing demand pair {demandPair}.");
            }

            LastRemovedDemandPairs.Add(demandPair);
            LastIterationDemandPairRemoval = true;

            DemandPairsPerNode[demandPair.Node1].Remove(demandPair);
            DemandPairsPerNode[demandPair.Node2].Remove(demandPair);

            if (DemandPairsPerNode[demandPair.Node1].Count == 0)
            {
                DemandPairsPerNode.Remove(demandPair.Node1);
            }
            if (DemandPairsPerNode[demandPair.Node2].Count == 0)
            {
                DemandPairsPerNode.Remove(demandPair.Node2);
            }

            // Remove this demand pair from each edge it is on.
            foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath.GetCountedEnumerable(new Counter()))
            {
                RemoveDemandPairFromEdge(edge, demandPair);
            }

            DemandPairs.Remove(demandPair);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPairs"/> is <see langword="null"/>.</exception>
        internal override void RemoveDemandPairs(IList<DemandPair> demandPairs)
        {
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to remove multiuple demand pairs, but the IEnumerable of demand pairs is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine($"Removing demand pairs {demandPairs.Print()}.");
            }

            foreach (DemandPair demandPair in demandPairs)
            {
                RemoveDemandPair(demandPair);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/>, <paramref name="oldEndpoint"/> or <paramref name="newEndpoint"/> is <see langword="null"/>.</exception>
        internal override void ChangeEndpointOfDemandPair(DemandPair demandPair, TreeNode oldEndpoint, TreeNode newEndpoint)
        {
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to change an endpoint of a demand pair, but the demand pair is null!");
            Utils.NullCheck(oldEndpoint, nameof(oldEndpoint), "Trying to change an endpoint of a demand pair, but the old endpoint of the demand pair is null!");
            Utils.NullCheck(newEndpoint, nameof(newEndpoint), "Trying to change an endpoint of a demand pair, but the new endpoint of the demand pair is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine($"Changing endpoint of demandpair {demandPair} from {oldEndpoint} to {newEndpoint}.");
            }

            List<(TreeNode, TreeNode)> oldEdges = new List<(TreeNode, TreeNode)>();
            if (oldEndpoint == demandPair.Node1)
            {
                // If the old endpoint is the first endpoint, we are removing edges from the start until the edge starts with the new endpoint.
                oldEdges.AddRange(demandPair.EdgesOnDemandPath.GetCountedEnumerable(new Counter()).TakeWhile(n => n.Item1 != newEndpoint));
            }
            else if (oldEndpoint == demandPair.Node2)
            {
                // If the old endpoint is the second endpoint, we are removing all edges from the new endpoint to the old endpoint. The extra Skip(1) is to exclude the last edge on the new demand path.
                oldEdges.AddRange(demandPair.EdgesOnDemandPath.GetCountedEnumerable(new Counter()).SkipWhile(n => n.Item2 != newEndpoint).Skip(1));
            }

            if (!DemandPairsPerNode.ContainsKey(newEndpoint))
            {
                DemandPairsPerNode[newEndpoint] = new List<DemandPair>();
            }

            DemandPairsPerNode[oldEndpoint].Remove(demandPair);
            DemandPairsPerNode[newEndpoint].Add(demandPair);

            LastChangedEdgesPerDemandPair.Add((oldEdges, demandPair));
            LastIterationDemandPairChange = true;

            foreach ((TreeNode, TreeNode) edge in oldEdges)
            {
                RemoveDemandPairFromEdge(edge, demandPair);
            }
            demandPair.ChangeEndpoint(oldEndpoint, newEndpoint);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPairEndpointTuples"/> is <see langword="null"/>.</exception>
        internal override void ChangeEndpointOfDemandPairs(IList<(DemandPair, TreeNode, TreeNode)> demandPairEndpointTuples)
        {
            Utils.NullCheck(demandPairEndpointTuples, nameof(demandPairEndpointTuples), "Trying to change the endpoints of multple demand pairs, but the IEnumerable with tuples with required information is null!");

            if (Program.PRINT_DEBUG_INFORMATION)
            {
                Console.WriteLine($"Changing endpoint of multiple demand pairs: {demandPairEndpointTuples.Print()}.");
            }

            foreach ((DemandPair, TreeNode, TreeNode) change in demandPairEndpointTuples)
            {
                ChangeEndpointOfDemandPair(change.Item1, change.Item2, change.Item3);
            }
        }
    }
}
