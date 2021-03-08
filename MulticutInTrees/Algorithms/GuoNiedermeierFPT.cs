// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Graphs;
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
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> DemandPairsPerEdge { get; set; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedList{T}"/> of <see cref="DemandPair"/> per <see cref="TreeNode"/>.
        /// </summary>
        private CountedDictionary<TreeNode, CountedList<DemandPair>> DemandPairsPerNode { get; set; }

        /// <summary>
        /// Constructor for <see cref="GuoNiedermeierFPT"/>.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public GuoNiedermeierFPT(MulticutInstance instance) : base(instance, nameof(GuoNiedermeierFPT))
        {
#if !EXPERIMENT
            Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of a the Guo-Niedermeier FPT algorithm, but the problem instance is null!");
#endif
            Preprocess();
        }

        /// <summary>
        /// Create the <see cref="List{T}"/> of <see cref="ReductionRule"/>s used by this algorithm.
        /// </summary>
        private void CreateReductionRules()
        {
            List<ReductionRule> reductionRules = new List<ReductionRule>();

            IdleEdge idleEdge = new IdleEdge(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(idleEdge);

            UnitPath unitPath = new UnitPath(Tree, DemandPairs, this);
            reductionRules.Add(unitPath);

            DominatedEdge dominatedEdge = new DominatedEdge(Tree, DemandPairs, this, DemandPairsPerEdge);
            reductionRules.Add(dominatedEdge);

            DominatedPath dominatedPath = new DominatedPath(Tree, DemandPairs, this, DemandPairsPerEdge);
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
            DemandPairsPerNode = new CountedDictionary<TreeNode, CountedList<DemandPair>>();

            // For each demand pair in the instance...
            foreach (DemandPair demandPair in DemandPairs.GetCountedEnumerable(AlgorithmPerformanceMeasurements.DemandPairsOperationsCounter))
            {
                if (!DemandPairsPerNode.ContainsKey(demandPair.Node1, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter))
                {
                    DemandPairsPerNode[demandPair.Node1, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter] = new CountedList<DemandPair>();
                }
                if (!DemandPairsPerNode.ContainsKey(demandPair.Node2, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter))
                {
                    DemandPairsPerNode[demandPair.Node2, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter] = new CountedList<DemandPair>();
                }
                DemandPairsPerNode[demandPair.Node1, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeValuesCounter);
                DemandPairsPerNode[demandPair.Node2, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeValuesCounter);

                // For each edge on this demand pair...
                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath(AlgorithmPerformanceMeasurements.TreeOperationsCounter))
                {
                    // Add this edge to the DemandPairsPerEdge dictionary.
                    (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);
                    if (!DemandPairsPerEdge.ContainsKey(usedEdge, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter))
                    {
                        DemandPairsPerEdge[usedEdge, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter] = new CountedList<DemandPair>();
                    }
                    DemandPairsPerEdge[usedEdge, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeValuesCounter);
                }
            }
        }

        /// <summary>
        /// Removes a <see cref="DemandPair"/> from a given edge in <see cref="DemandPairsPerEdge"/>.
        /// </summary>
        /// <param name="edge">The edge from which <paramref name="demandPair"/> should be removed.</param>
        /// <param name="demandPair">The <see cref="DemandPair"/> that should be removed from <paramref name="edge"/>.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, <paramref name="demandPair"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private void RemoveDemandPairFromEdge((TreeNode, TreeNode) edge, DemandPair demandPair, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to remove a demand pair from an edge, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to remove a demand pair from an edge, but the second endpoint of the edge is null");
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to remove a demand pair from an edge, but the demand pair is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove a demand pair from an edge, but the performance measurements are null!");
#endif
            (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);
            DemandPairsPerEdge[usedEdge, measurements.DemandPairsPerEdgeKeysCounter].Remove(demandPair, measurements.DemandPairsPerEdgeValuesCounter);

            // If, after removing this demand pair, there are no more demand pairs going over this edge, remove it from the dictionary.
            if (DemandPairsPerEdge[usedEdge, measurements.DemandPairsPerEdgeKeysCounter].Count(measurements.DemandPairsPerEdgeValuesCounter) == 0)
            {
                DemandPairsPerEdge.Remove(usedEdge, measurements.DemandPairsPerEdgeKeysCounter);
            }
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            FillDemandPathsPerEdge();
            CreateReductionRules();
        }

        /// <summary>
        /// Internal method for cutting an edge. Also returns the <see cref="TreeNode"/> that is the result of the edge contraction. See also: <seealso cref="CutEdge(ValueTuple{TreeNode, TreeNode}, PerformanceMeasurements)"/>.
        /// </summary>
        /// <param name="edge">The edge to be cut, represented by a tuple of two <see cref="TreeNode"/>s.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used for this modification.</param>
        /// <returns>The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="edge"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private TreeNode InternalCutEdge((TreeNode, TreeNode) edge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to cut an edge, but the first item of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to cut an edge, but the second item of this edge is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to cut an edge, but the performance measures to be used are null!");
            if (!Tree.HasEdge(edge, MockCounter))
            {
                throw new NotInGraphException($"Trying to cut edge {edge}, but this edge is not part of the tree!");
            }
            if (edge.Item1 == edge.Item2)
            {
                throw new InvalidEdgeException($"Trying to cut edge {edge}, but this edge is a self loop and should not exist!");
            }
#endif
            PartialSolution.Add(edge);
            CountedList<DemandPair> separatedDemandPairs = new CountedList<DemandPair>(DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(edge), measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(MockCounter), MockCounter);

            RemoveDemandPairs(separatedDemandPairs, measurements);
            TreeNode res = InternalContractEdge(edge, measurements);

            foreach (DemandPair demandPair in separatedDemandPairs.GetCountedEnumerable(measurements.DemandPairsPerEdgeValuesCounter))
            {
                if (demandPair.LengthOfPath(measurements.DemandPairsOperationsCounter) == 1)
                {
                    continue;
                }
                demandPair.OnEdgeContracted(edge, res, measurements.DemandPairsOperationsCounter);
            }

            return res;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either item of <paramref name="edge"/>, or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal override void CutEdge((TreeNode, TreeNode) edge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to cut an edge, but the first item of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to cut an edge, but the second item of this edge is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to cut an edge, but the performance measures to be used are null!");
#endif
            InternalCutEdge(edge, measurements);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal override void CutEdges(CountedList<(TreeNode, TreeNode)> edges, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), "Trying to cut multiple edges, but the IEnumerable of edges is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to cut multiple edges, but the performance measures to be used are null!");
#endif
            int nrEdges = edges.Count(measurements.TreeOperationsCounter);
            for (int i = 0; i < nrEdges; i++)
            {
                TreeNode newNode = InternalCutEdge(edges[i, measurements.TreeOperationsCounter], measurements);
                for (int j = i + 1; j < nrEdges; j++)
                {
                    if (edges[i, MockCounter].Item1 == edges[j, MockCounter].Item1 || edges[i, MockCounter].Item2 == edges[j, MockCounter].Item1)
                    {
                        edges[j, MockCounter] = (newNode, edges[j, MockCounter].Item2);
                    }
                    if (edges[i, MockCounter].Item1 == edges[j, MockCounter].Item2 || edges[i, MockCounter].Item2 == edges[j, MockCounter].Item2)
                    {
                        edges[j, MockCounter] = (edges[j, MockCounter].Item1, newNode);
                    }
                }
            }
        }

        /// <summary>
        /// Internal method for contracting an edge. Also returns the <see cref="TreeNode"/> that is the result of the edge contraction. See also: <seealso cref="ContractEdge(ValueTuple{TreeNode, TreeNode}, PerformanceMeasurements)"/>.
        /// </summary>
        /// <param name="edge">The edge to be contracted, represented by a tuple of two <see cref="TreeNode"/>s.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used for this modification.</param>
        /// <returns>The <see cref="TreeNode"/> that is the result of the contraction of <paramref name="edge"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="edge"/> is not part of the input.</exception>
        /// <exception cref="InvalidEdgeException">Thrown when <paramref name="edge"/> is a self-loop.</exception>
        private TreeNode InternalContractEdge((TreeNode, TreeNode) edge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to contract an edge, but the first item of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to contract an edge, but the second item of this edge is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to contract an edge, but performance measures to be used are null!");
            if (!Tree.HasEdge(edge, MockCounter))
            {
                throw new NotInGraphException($"Trying to contract edge {edge}, but this edge is not part of the tree!");
            }
            if (edge.Item1 == edge.Item2)
            {
                throw new InvalidEdgeException($"Trying to contract edge {edge}, but this edge is a self loop and should not exist!");
            }
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Contracting edge {edge}!");
#endif
            TreeNode parent = edge.Item1;
            TreeNode child = edge.Item2;
            if (parent.GetParent(measurements.TreeOperationsCounter) == child)
            {
                parent = edge.Item2;
                child = edge.Item1;
            }

            TreeNode newNode = parent;
            (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);
            UpdateNodeTypesDuringEdgeContraction(usedEdge, newNode);
            Tree.RemoveNode(child, measurements.TreeOperationsCounter);
            CountedList<DemandPair> pairsOnEdge = RemoveDemandPairsFromContractedEdge(usedEdge, newNode, measurements);
            UpdateDemandPairsStartingAtContractedEdge(usedEdge, child, newNode, pairsOnEdge, measurements);
            UpdateDemandPairsGoingThroughChild(child, newNode, measurements);

            LastContractedEdges.Add((usedEdge, newNode, pairsOnEdge), measurements.TreeOperationsCounter);
            LastIterationEdgeContraction = true;
            measurements.NumberOfContractedEdgesCounter++;

            return newNode;
        }

        /// <summary>
        /// Update the <see cref="DemandPair"/>s that pass through <paramref name="child"/>, but not over the edge that is being contracted.
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> that will be removed by the contraction.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/>, <paramref name="newNode"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private void UpdateDemandPairsGoingThroughChild(TreeNode child, TreeNode newNode, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(child, nameof(child), "Trying to update the demand pairs going through the child of an edge that will be contracted, but the child is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to update the demand pairs going through the child of an edge that will be contracted, but the new node is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to update the demand pairs going through the child of an edge that will be contracted, but the performance measures to be used are null!");
#endif
            List<(TreeNode, TreeNode)> keysToBeRemoved = new List<(TreeNode, TreeNode)>();
            CountedList<(TreeNode, TreeNode)> oldKeys = new CountedList<(TreeNode, TreeNode)>(DemandPairsPerEdge.GetKeys(measurements.DemandPairsPerEdgeKeysCounter).Where(n => n.Item1 == child || n.Item2 == child), MockCounter);
            foreach ((TreeNode, TreeNode) key in oldKeys.GetCountedEnumerable(measurements.DemandPairsPerEdgeKeysCounter))
            {
                foreach (DemandPair demandPair in DemandPairsPerEdge[key, measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(measurements.DemandPairsPerEdgeValuesCounter))
                {
                    demandPair.OnEdgeNextToNodeOnDemandPathContracted(child, newNode, measurements.DemandPairsOperationsCounter);
                }

                if (key.Item1 == child)
                {
                    (TreeNode, TreeNode) newKey = Utils.OrderEdgeSmallToLarge((key.Item2, newNode));
                    if (!DemandPairsPerEdge.ContainsKey(newKey, measurements.DemandPairsPerEdgeKeysCounter))
                    {
                        DemandPairsPerEdge[newKey, measurements.DemandPairsPerEdgeKeysCounter] = new CountedList<DemandPair>();
                    }
                    DemandPairsPerEdge[newKey, measurements.DemandPairsPerEdgeKeysCounter].AddRange(DemandPairsPerEdge[key, measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(MockCounter), measurements.DemandPairsPerEdgeValuesCounter);
                    keysToBeRemoved.Add(key);
                }
                if (key.Item2 == child)
                {
                    (TreeNode, TreeNode) newKey = Utils.OrderEdgeSmallToLarge((key.Item1, newNode));
                    if (!DemandPairsPerEdge.ContainsKey(newKey, measurements.DemandPairsPerEdgeKeysCounter))
                    {
                        DemandPairsPerEdge[newKey, measurements.DemandPairsPerEdgeKeysCounter] = new CountedList<DemandPair>();
                    }
                    DemandPairsPerEdge[newKey, measurements.DemandPairsPerEdgeKeysCounter].AddRange(DemandPairsPerEdge[key, measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(MockCounter), measurements.DemandPairsPerEdgeValuesCounter);
                    keysToBeRemoved.Add(key);
                }
            }
            foreach ((TreeNode, TreeNode) key in keysToBeRemoved)
            {
                DemandPairsPerEdge.Remove(key, measurements.DemandPairsPerEdgeKeysCounter);
            }
        }

        /// <summary>
        /// Update all <see cref="DemandPair"/>s that start at an edge that will be contracted.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s that represents the edge that is being contracted.</param>
        /// <param name="child">The <see cref="TreeNode"/> that will be removed by the contraction.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction.</param>
        /// <param name="pairsOnEdge">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s that go over <paramref name="edge"/>.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, <paramref name="child"/>, <paramref name="newNode"/>, <paramref name="pairsOnEdge"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private void UpdateDemandPairsStartingAtContractedEdge((TreeNode, TreeNode) edge, TreeNode child, TreeNode newNode, CountedList<DemandPair> pairsOnEdge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the first endpoint of the edge that will be contracted is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the second endpoint of the edge that will be contracted is null!");
            Utils.NullCheck(child, nameof(child), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the node that will be removed by the contraction is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the node that is the result of the contraction is null!");
            Utils.NullCheck(pairsOnEdge, nameof(pairsOnEdge), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the list of demand pairs going through the contracted edge is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the performance measures to be used are null!");
#endif
            if (DemandPairsPerNode.TryGetValue(child, out CountedList<DemandPair> pairsAtChild, measurements.DemandPairsPerEdgeKeysCounter))
            {
                if (!DemandPairsPerNode.ContainsKey(newNode, measurements.DemandPairsPerEdgeKeysCounter))
                {
                    DemandPairsPerNode[newNode, measurements.DemandPairsPerEdgeKeysCounter] = new CountedList<DemandPair>();
                }
                foreach (DemandPair demandPair in pairsAtChild.GetCountedEnumerable(measurements.DemandPairsPerEdgeValuesCounter))
                {
                    demandPair.OnEdgeNextToDemandPathEndpointsContracted(edge, newNode, measurements.DemandPairsOperationsCounter);
                    DemandPairsPerNode[newNode, measurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, measurements.DemandPairsPerEdgeValuesCounter);
                }
                DemandPairsPerNode.Remove(child, measurements.DemandPairsPerEdgeKeysCounter);
            }
        }

        /// <summary>
        /// Removes all <see cref="DemandPair"/>s from the edge that will be contracted.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s that represents the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used during this modification.</param>
        /// <returns>A <see cref="CountedList{T}"/> with all the <see cref="DemandPair"/>s that pass through <paramref name="edge"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, <paramref name="newNode"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private CountedList<DemandPair> RemoveDemandPairsFromContractedEdge((TreeNode, TreeNode) edge, TreeNode newNode, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to remove all demand paths going through an edge, but the first endpoint of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to remove all demand paths going through an edge, but the second endpoint of this edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to remove all demand paths going through an edge, but the node that is the result of the contraction is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove all demand paths going through an edge, but the performance measures to be used are null!");
#endif
            if (DemandPairsPerEdge.TryGetValue(edge, out CountedList<DemandPair> pairsOnEdge, measurements.DemandPairsPerEdgeKeysCounter))
            {
                foreach (DemandPair demandPair in pairsOnEdge.GetCountedEnumerable(measurements.DemandPairsPerEdgeValuesCounter))
                {
                    demandPair.OnEdgeContracted(edge, newNode, measurements.DemandPairsOperationsCounter);
                }
                DemandPairsPerEdge.Remove(edge, measurements.DemandPairsPerEdgeKeysCounter);
            }
            else
            {
                pairsOnEdge = new CountedList<DemandPair>();
            }

            return pairsOnEdge;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when either item of <paramref name="edge"/>, or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal override void ContractEdge((TreeNode, TreeNode) edge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to contract an edge, but the first item of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item1), "Trying to contract an edge, but the second item of this edge is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to contract an edge, but the performance measures to be used are null!");
#endif
            InternalContractEdge(edge, measurements);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal override void ContractEdges(CountedList<(TreeNode, TreeNode)> edges, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), "Trying to contract multiple edges, but the IEnumerable of edges is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to contract multiple edges, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Contracting edges {edges.Print()}");
#endif
            int nrEdges = edges.Count(measurements.TreeOperationsCounter);
            for (int i = 0; i < nrEdges; i++)
            {
                TreeNode newNode = InternalContractEdge(edges[i, measurements.TreeOperationsCounter], measurements);
                for (int j = i + 1; j < nrEdges; j++)
                {
                    if (edges[i, MockCounter].Item1 == edges[j, MockCounter].Item1 || edges[i, MockCounter].Item2 == edges[j, MockCounter].Item1)
                    {
                        edges[j, MockCounter] = (newNode, edges[j, MockCounter].Item2);
                    }
                    if (edges[i, MockCounter].Item1 == edges[j, MockCounter].Item2 || edges[i, MockCounter].Item2 == edges[j, MockCounter].Item2)
                    {
                        edges[j, MockCounter] = (edges[j, MockCounter].Item1, newNode);
                    }
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal override void RemoveDemandPair(DemandPair demandPair, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to remove a demand pair, but the demand pair is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove a demand pair, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Removing demand pair {demandPair}.");
#endif
            LastRemovedDemandPairs.Add(demandPair, measurements.DemandPairsOperationsCounter);
            LastIterationDemandPairRemoval = true;
            measurements.NumberOfRemovedDemandPairsCounter++;

            DemandPairsPerNode[demandPair.Node1, measurements.DemandPairsPerEdgeKeysCounter].Remove(demandPair, measurements.DemandPairsOperationsCounter);
            DemandPairsPerNode[demandPair.Node2, measurements.DemandPairsPerEdgeKeysCounter].Remove(demandPair, measurements.DemandPairsOperationsCounter);

            if (DemandPairsPerNode[demandPair.Node1, measurements.DemandPairsPerEdgeKeysCounter].Count(measurements.DemandPairsOperationsCounter) == 0)
            {
                DemandPairsPerNode.Remove(demandPair.Node1, measurements.DemandPairsPerEdgeKeysCounter);
            }
            if (DemandPairsPerNode[demandPair.Node2, measurements.DemandPairsPerEdgeKeysCounter].Count(measurements.DemandPairsOperationsCounter) == 0)
            {
                DemandPairsPerNode.Remove(demandPair.Node2, measurements.DemandPairsPerEdgeKeysCounter);
            }

            // Remove this demand pair from each edge it is on.
            foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath(measurements.TreeOperationsCounter))
            {
                RemoveDemandPairFromEdge(edge, demandPair, measurements);
            }

            DemandPairs.Remove(demandPair, measurements.DemandPairsOperationsCounter);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPairs"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal override void RemoveDemandPairs(CountedList<DemandPair> demandPairs, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to remove multiple demand pairs, but the IEnumerable of demand pairs is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove multiple demand pairs, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Removing demand pairs {demandPairs.Print()}.");
#endif
            foreach (DemandPair demandPair in demandPairs.GetCountedEnumerable(measurements.DemandPairsOperationsCounter))
            {
                RemoveDemandPair(demandPair, measurements);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/>, <paramref name="oldEndpoint"/>, <paramref name="newEndpoint"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal override void ChangeEndpointOfDemandPair(DemandPair demandPair, TreeNode oldEndpoint, TreeNode newEndpoint, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to change an endpoint of a demand pair, but the demand pair is null!");
            Utils.NullCheck(oldEndpoint, nameof(oldEndpoint), "Trying to change an endpoint of a demand pair, but the old endpoint of the demand pair is null!");
            Utils.NullCheck(newEndpoint, nameof(newEndpoint), "Trying to change an endpoint of a demand pair, but the new endpoint of the demand pair is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to change an endpoint of a demand pair, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Changing endpoint of demandpair {demandPair} from {oldEndpoint} to {newEndpoint}.");
#endif
            CountedList<(TreeNode, TreeNode)> oldEdges = new CountedList<(TreeNode, TreeNode)>();
            if (oldEndpoint == demandPair.Node1)
            {
                // If the old endpoint is the first endpoint, we are removing edges from the start until the edge starts with the new endpoint.
                oldEdges.AddRange(demandPair.EdgesOnDemandPath(measurements.TreeOperationsCounter).TakeWhile(n => n.Item1 != newEndpoint), measurements.TreeOperationsCounter);
            }
            else if (oldEndpoint == demandPair.Node2)
            {
                // If the old endpoint is the second endpoint, we are removing all edges from the new endpoint to the old endpoint. The extra Skip(1) is to exclude the last edge on the new demand path.
                oldEdges.AddRange(demandPair.EdgesOnDemandPath(measurements.TreeOperationsCounter).SkipWhile(n => n.Item2 != newEndpoint).Skip(1), measurements.TreeOperationsCounter);
            }

            if (!DemandPairsPerNode.ContainsKey(newEndpoint, measurements.DemandPairsPerEdgeKeysCounter))
            {
                DemandPairsPerNode[newEndpoint, measurements.DemandPairsPerEdgeKeysCounter] = new CountedList<DemandPair>();
            }

            DemandPairsPerNode[oldEndpoint, measurements.DemandPairsPerEdgeKeysCounter].Remove(demandPair, measurements.DemandPairsOperationsCounter);
            DemandPairsPerNode[newEndpoint, measurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, measurements.DemandPairsOperationsCounter);

            LastChangedEdgesPerDemandPair.Add((oldEdges, demandPair), measurements.DemandPairsOperationsCounter);
            LastIterationDemandPairChange = true;
            measurements.NumberOfChangedDemandPairsCounter++;

            foreach ((TreeNode, TreeNode) edge in oldEdges.GetCountedEnumerable(measurements.TreeOperationsCounter))
            {
                RemoveDemandPairFromEdge(edge, demandPair, measurements);
            }
            demandPair.ChangeEndpoint(oldEndpoint, newEndpoint, measurements.DemandPairsOperationsCounter);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPairEndpointTuples"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal override void ChangeEndpointOfDemandPairs(CountedList<(DemandPair, TreeNode, TreeNode)> demandPairEndpointTuples, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPairEndpointTuples, nameof(demandPairEndpointTuples), "Trying to change the endpoints of multple demand pairs, but the IEnumerable with tuples with required information is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to change the endpoints of multple demand pairs, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Changing endpoint of multiple demand pairs: {demandPairEndpointTuples.Print()}.");
#endif
            foreach ((DemandPair, TreeNode, TreeNode) change in demandPairEndpointTuples.GetCountedEnumerable(measurements.DemandPairsOperationsCounter))
            {
                ChangeEndpointOfDemandPair(change.Item1, change.Item2, change.Item3, measurements);
            }
        }
    }
}
