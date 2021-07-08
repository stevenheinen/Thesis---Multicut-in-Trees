// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// <see cref="ReductionRule"/> that removes <see cref="DemandPair"/>s when they intersect enough other <see cref="DemandPair"/>s to be automatically separated.
    /// <br/>
    /// <b>Rule:</b> Let P0 be a demand path. If k + 1 demand paths P1, ..., P(k + 1) different from P0 but intersecting P0 are such that for every i != j, the common factor of Pi and Pj is a subset of P0, i.e. (Pi ∩ Pj) ⊆ P0, then delete P0 from the set of demand paths.
    /// </summary>
    public class CommonFactor : ReductionRule
    {
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="Edge{TNode}"/>.
        /// </summary>
        protected CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> DemandPairsPerEdge { get; }
        
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="Node"/>.
        /// </summary>
        protected CountedDictionary<Node, CountedCollection<DemandPair>> DemandPairsPerNode { get; }

        /// <summary>
        /// The maximum size the solution is allowed to have.
        /// </summary>
        private int MaxSolutionSize { get; }

        /// <summary>
        /// The part of the solution that has been found thus far.
        /// </summary>
        private List<Edge<Node>> PartialSolution { get; }

        /// <summary>
        /// Constructor for <see cref="CommonFactor"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="demandPairsPerEdge"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="Edge{TNode}"/>.</param>
        /// <param name="demandPairsPerNode"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="Node"/>.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>"
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public CommonFactor(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge, CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode, List<Edge<Node>> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per edge is null!");
            Utils.NullCheck(demandPairsPerNode, nameof(demandPairsPerNode), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per node is null!");
            Utils.NullCheck(partialSolution, nameof(partialSolution), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with the partial solution is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException($"Trying to create an instance of the {GetType().Name} reduction rule, but the maximum solution size parameter is smaller than zero!");
            }
#endif
            MockCounter = new Counter();
            DemandPairsPerEdge = demandPairsPerEdge;
            DemandPairsPerNode = demandPairsPerNode;
            PartialSolution = partialSolution;
            MaxSolutionSize = maxSolutionSize;
        }

        /// <summary>
        /// Uses the <see cref="AbstractNode{TNode}.ID"/> of the endpoints of <paramref name="e1"/> and <paramref name="e2"/> to sort <paramref name="e1"/> and <paramref name="e2"/> in order to be used as key.
        /// </summary>
        /// <param name="e1">The first <see cref="Edge{TNode}"/>.</param>
        /// <param name="e2">The second <see cref="Edge{TNode}"/>.</param>
        /// <returns>A tuple with <paramref name="e1"/> and <paramref name="e2"/> sorted on the unique ID of their endpoints.</returns>

        private static (Edge<Node>, Edge<Node>) GetIntersectionKey(Edge<Node> e1, Edge<Node> e2)
        {
            if (e1.Endpoint1.ID < e2.Endpoint1.ID)
            {
                return (e1, e2);
            }
            if (e2.Endpoint1.ID < e1.Endpoint1.ID)
            {
                return (e2, e1);
            }
            if (e1.Endpoint2.ID < e2.Endpoint2.ID)
            {
                return (e1, e2);
            }
            return (e2, e1);
        }

        /// <summary>
        /// Checks whether this <see cref="ReductionRule"/> is applicable on any of the <see cref="DemandPair"/>s in <paramref name="demandPairsToCheck"/>, and applies it.
        /// </summary>
        /// <param name="demandPairsToCheck">The <see cref="DemandPair"/>s we want to try this <see cref="ReductionRule"/> on.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/>, <see langword="false"/> otherwise.</returns>
        private bool CheckApplicability(IEnumerable<DemandPair> demandPairsToCheck)
        {
            foreach (DemandPair p0 in demandPairsToCheck)
            {
                if (CheckApplicabilitySinglePair(p0))
                {
                    CountedList<DemandPair> pairsToBeRemoved = new(); 
                    pairsToBeRemoved.Add(p0, Measurements.DemandPairsOperationsCounter);
                    return TryRemoveDemandPairs(pairsToBeRemoved);
                }
            }

            return false;
        }

        /// <summary>
        /// Find the set of edges Z: the edges that share an endpoint with a node on <paramref name="p0"/>, but do not lie on <paramref name="p0"/> itself. Also computes the set <paramref name="dps"/> with <see cref="DemandPair"/>s that start at a node on <paramref name="p0"/>.
        /// </summary>
        /// <param name="p0">The current <see cref="DemandPair"/> we are checking.</param>
        /// <param name="dps">The set with <see cref="DemandPair"/>s that start at a node on <paramref name="p0"/>.</param>
        /// <returns>A <see cref="HashSet{T}"/> with the edges that share an endpoint with a node on <paramref name="p0"/>, but do not lie on <paramref name="p0"/> itself.</returns>
        private HashSet<Edge<Node>> FindSetZ(DemandPair p0, out HashSet<DemandPair> dps)
        {
            HashSet<Node> nodesOnP0 = new();
            foreach (Edge<Node> edge in p0.EdgesOnDemandPath(Measurements.TreeOperationsCounter))
            {
                nodesOnP0.Add(edge.Endpoint1);
                nodesOnP0.Add(edge.Endpoint2);
            }

            dps = new();
            HashSet<Edge<Node>> z = new();
            foreach (Node node in nodesOnP0)
            {
                if (DemandPairsPerNode.TryGetValue(node, out CountedCollection<DemandPair> d, Measurements.DemandPairsPerEdgeKeysCounter))
                {
                    foreach (DemandPair dp in d.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                    {
                        dps.Add(dp);
                    }
                }
                foreach (Edge<Node> neighbour in Tree.GetNeighbouringEdges(node, Measurements.TreeOperationsCounter))
                {
                    z.Add(neighbour);
                }
            }
            foreach (Edge<Node> edge in p0.EdgesOnDemandPath(Measurements.TreeOperationsCounter))
            {
                z.Remove(edge);
            }

            return z;
        }

        /// <summary>
        /// Find the set Y: the set of edges in Z, such that there starts a <see cref="DemandPair"/> at a node on P_0 that goes through this edge.
        /// </summary>
        /// <param name="p0">The current <see cref="DemandPair"/> we are checking.</param>
        /// <param name="z">The set Z.</param>
        /// <param name="dps">The set with <see cref="DemandPair"/>s that start at a node in P_0.</param>
        /// <returns>The <see cref="HashSet{T}"/> Y.</returns>
        private HashSet<Edge<Node>> FindSetY(DemandPair p0, HashSet<Edge<Node>> z, HashSet<DemandPair> dps)
        {
            HashSet<Edge<Node>> y = new();
            foreach (Edge<Node> edge in z)
            {
                if (DemandPairsPerEdge.TryGetValue(edge, out CountedCollection<DemandPair> d, Measurements.DemandPairsPerEdgeKeysCounter))
                {
                    foreach (DemandPair dp in d.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
                    {
                        if (dps.Contains(dp) && dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Any(e => p0.EdgeIsPartOfPath(e, Measurements.TreeOperationsCounter)))
                        {
                            y.Add(edge);
                            break;
                        }
                    }
                }
            }

            return y;
        }

        /// <summary>
        /// Find the set of edges in the matching graph. An edge exists between two vertices if there is a <see cref="DemandPair"/> that goes through both edges the vertices represent.
        /// </summary>
        /// <param name="vertices"><see cref="IEnumerable{T}"/> of <see cref="Edge{TNode}"/>s that will be vertices in the matching graph.</param>
        /// <returns>A <see cref="List{T}"/> with a tuple of two <see cref="Edge{TNode}"/>s that will become edges in the matching graph.</returns>
        private List<(Edge<Node>, Edge<Node>)> FindEdgesForMatchingGraph(IEnumerable<Edge<Node>> vertices)
        {
            HashSet<(Edge<Node>, Edge<Node>)> checkedPairs = new();
            List<(Edge<Node>, Edge<Node>)> edges = new();
            foreach (Edge<Node> e1 in vertices)
            {
                foreach (Edge<Node> e2 in vertices)
                {
                    if (e1 == e2)
                    {
                        continue;
                    }

                    (Edge<Node>, Edge<Node>) key = GetIntersectionKey(e1, e2);
                    if (checkedPairs.Contains(key))
                    {
                        continue;
                    }
                    checkedPairs.Add(key);

                    if (DemandPairsPerEdge[key.Item1, Measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter).Any(dp => DemandPairsPerEdge[key.Item2, Measurements.DemandPairsPerEdgeKeysCounter].Contains(dp, Measurements.DemandPairsPerEdgeValuesCounter)))
                    {
                        edges.Add(key);
                    }
                }
            }
            return edges;
        }

        /// <summary>
        /// Create the matching graph for the <see cref="DemandPair"/> <paramref name="p0"/> and check whether it has a matching of sufficient size.
        /// </summary>
        /// <param name="p0">The current <see cref="DemandPair"/> we are checking.</param>
        /// <returns><see langword="true"/> if we can apply this <see cref="ReductionRule"/> on <paramref name="p0"/>, <see langword="false"/> otherwise.</returns>
        private bool CheckApplicabilitySinglePair(DemandPair p0)
        {
            HashSet<Edge<Node>> z = FindSetZ(p0, out HashSet<DemandPair> dps);
            HashSet<Edge<Node>> y = FindSetY(p0, z, dps);

            IEnumerable<Edge<Node>> vertices = z.Except(y);
            List<(Edge<Node>, Edge<Node>)> edges = FindEdgesForMatchingGraph(vertices);

            Dictionary<Edge<Node>, Node> edgeToNodeInMatchingGraph = new();
            List<Node> matchingNodes = new();
            List<Edge<Node>> matchingEdges = new();
            uint nodeCounter = 0;
            foreach (Edge<Node> edge in vertices)
            {
                Node n = new(nodeCounter++);
                edgeToNodeInMatchingGraph[edge] = n;
                matchingNodes.Add(n);
            }
            foreach ((Edge<Node>, Edge<Node>) edgePair in edges)
            {
                Edge<Node> e = new(edgeToNodeInMatchingGraph[edgePair.Item1], edgeToNodeInMatchingGraph[edgePair.Item2]);
                matchingEdges.Add(e);
            }

            Graph matchingGraph = new();
            matchingGraph.AddNodes(matchingNodes, Measurements.TreeOperationsCounter);
            matchingGraph.AddEdges(matchingEdges, Measurements.TreeOperationsCounter);

            int k = MaxSolutionSize - PartialSolution.Count + 1 - y.Count;
            return EdmondsMatching.HasMatchingOfSize<Graph, Edge<Node>, Node>(matchingGraph, k, Measurements.TreeOperationsCounter);
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Executing {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            return CheckApplicability(DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Executing {GetType().Name} rule for the first time");
#endif
            HasRun = true;
            LastContractedEdges.Clear(MockCounter);
            LastRemovedDemandPairs.Clear(MockCounter);
            LastChangedDemandPairs.Clear(MockCounter);
            Measurements.TimeSpentCheckingApplicability.Start();
            return CheckApplicability(DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter));
        }
    }
}
