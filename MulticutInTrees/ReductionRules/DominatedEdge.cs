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
    /// <see cref="ReductionRule"/> that contracts an edge if all <see cref="DemandPair"/>s whose path go over this edge also go over another edge.
    /// <br/>
    /// Rule: If all demand paths that pass through edge e_1 also pass through edge e_2, then contract e_1.
    /// </summary>
    public class DominatedEdge : ReductionRule
    {
        /// <summary>
        /// A <see cref="CountedDictionary{TKey, TValue}"/> with <see cref="Edge{TNode}"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.
        /// The value is all the <see cref="DemandPair"/>s whose path passes through the key.
        /// </summary>
        private CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for <see cref="DominatedEdge"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="IdleEdge"/> is part of.</param>
        /// <param name="demandPairsPerEdge">The <see cref="CountedDictionary{TKey, TValue}"/> with <see cref="Edge{TNode}"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public DominatedEdge(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand paths per edge is null!");
#endif
            DemandPairsPerEdge = demandPairsPerEdge;
        }

        /// <summary>
        /// Checks whether we can contract <paramref name="contractEdge"/>. We can do this if all <see cref="DemandPair"/>s passing through it also pass through <paramref name="otherEdge"/>.
        /// </summary>
        /// <param name="contractEdge">The edge for which we want to know whether we can contract it.</param>
        /// <param name="otherEdge">The edge we are comparing <paramref name="contractEdge"/> against.</param>
        /// <returns><see langword="true"/> if all <see cref="DemandPair"/>s that pas through <paramref name="contractEdge"/> also pass through <paramref name="otherEdge"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractEdge"/> or <paramref name="otherEdge"/> is <see langword="null"/>.</exception>
        protected bool AllDemandPairsPassThroughAnotherEdge(Edge<Node> contractEdge, Edge<Node> otherEdge)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractEdge, nameof(contractEdge), "Trying to see whether all demand paths that pass through an edge also pass through another, but the first edge is null!");
            Utils.NullCheck(otherEdge, nameof(otherEdge), "Trying to see whether all demand paths that pass through an edge also pass through another, but the second edge is null!");
#endif
            CountedCollection<DemandPair> set1 = DemandPairsPerEdge[contractEdge, Measurements.DemandPairsPerEdgeKeysCounter];
            CountedCollection<DemandPair> set2 = DemandPairsPerEdge[otherEdge, Measurements.DemandPairsPerEdgeKeysCounter];
            if (set1.Count(Measurements.DemandPairsPerEdgeValuesCounter) > set2.Count(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                return false;
            }

            return set1.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter).IsSubsetOf(set2.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter));
        }

        /// <summary>
        /// Finds all edges that are on the shortest <see cref="DemandPair"/> through <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">The <see cref="Edge{TNode}"/> for which we want to know the edges on the shortest <see cref="DemandPair"/> through it.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all edges on the shortest <see cref="DemandPair"/> through <paramref name="edge"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/> is <see langword="null"/>.</exception>
        protected IEnumerable<Edge<Node>> FindEdgesOnShortestDemandPathThroughEdge(Edge<Node> edge)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to find all edges on the shortest demand path through this edge, but the first endpoint of the edge is null!");
#endif
            if (!DemandPairsPerEdge.TryGetValue(edge, out CountedCollection<DemandPair> demandPathsOnEdge, Measurements.DemandPairsPerEdgeKeysCounter))
            {
                return new List<Edge<Node>>();
            }

            DemandPair shortestPathThroughThisEdge = demandPathsOnEdge.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter).Aggregate((n, m) => n.LengthOfPath(Measurements.DemandPairsOperationsCounter) < m.LengthOfPath(Measurements.DemandPairsOperationsCounter) ? n : m);
            return shortestPathThroughThisEdge.EdgesOnDemandPath(Measurements.TreeOperationsCounter);
        }

        /// <summary>
        /// Try to apply this <see cref="ReductionRule"/> on the edges in <paramref name="edgesToCheck"/>.
        /// </summary>
        /// <param name="edgesToCheck">The <see cref="IEnumerable{T}"/> with edges we want to check.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/> successfully, <see langword="false"/> otherwise.</returns>
        protected virtual bool TryApplyReductionRule(IEnumerable<Edge<Node>> edgesToCheck)
        {
            HashSet<Edge<Node>> edgesToBeContracted = new();
            foreach (Edge<Node> edge1 in edgesToCheck)
            {
                foreach (Edge<Node> edge2 in FindEdgesOnShortestDemandPathThroughEdge(edge1))
                {
                    if (edge1 == edge2 || edgesToBeContracted.Contains(edge2))
                    {
                        continue;
                    }

                    if (AllDemandPairsPassThroughAnotherEdge(edge1, edge2))
                    {
                        edgesToBeContracted.Add(edge1);
                        break;
                    }
                }
            }

            return TryContractEdges(new CountedList<Edge<Node>>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<Edge<Node>> edgesToBeChecked = new();

            foreach (DemandPair demandPair in LastRemovedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach (Edge<Node> edge in demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter))
                {
                    edgesToBeChecked.Add(edge);
                }
            }

            foreach ((CountedList<Edge<Node>> edges, DemandPair _) in LastChangedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach (Edge<Node> edge in edges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    edgesToBeChecked.Add(edge);
                }
            }

            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return TryApplyReductionRule(edgesToBeChecked);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule for the first time");
#endif
            HasRun = true;
            LastContractedEdges.Clear(MockCounter);
            LastRemovedDemandPairs.Clear(MockCounter);
            LastChangedDemandPairs.Clear(MockCounter);
            Measurements.TimeSpentCheckingApplicability.Start();
            return TryApplyReductionRule(Tree.Edges(Measurements.TreeOperationsCounter));
        }
    }
}