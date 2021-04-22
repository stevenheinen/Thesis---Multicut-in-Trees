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
    /// Improved version of the <see cref="DominatedEdge"/> rule. This rule only compares edges to their direct neighbours instead of every other edge in the tree.
    /// </summary>
    public class ImprovedDominatedEdge : DominatedEdge
    {
        /// <summary>
        /// Constructor for <see cref="ImprovedDominatedEdge"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="IdleEdge"/> is part of.</param>
        /// <param name="demandPairsPerEdge">The <see cref="CountedDictionary{TKey, TValue}"/> with <see cref="Edge{TNode}"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public ImprovedDominatedEdge(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm, demandPairsPerEdge)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand paths per edge is null!");
#endif
        }

        /// <summary>
        /// Compare <paramref name="edge"/> to its direct neighbours and see if we can contract it.
        /// </summary>
        /// <param name="edge">The edge we want to contract.</param>
        /// <param name="edgesToBeContracted">The <see cref="HashSet{T}"/> containing all edges that will be contracted.</param>
        /// <param name="comparedEdge">The <see cref="Dictionary{TKey, TValue}"/> with per contracted edge the edge it was compared to that led to the contraction.</param>
        /// <returns><see langword="true"/> if we are able to contract <paramref name="edge"/> when looking at its direct neighbours, <see langword="false"/> otherwise.</returns>
        private bool CompareEdgeToNeighbours(Edge<Node> edge, HashSet<Edge<Node>> edgesToBeContracted, Dictionary<Edge<Node>, Edge<Node>> comparedEdge)
        {
            foreach (Edge<Node> neighbour in NeighbouringEdges(edge))
            {
                Edge<Node> compareEdge = neighbour;
                bool skip = false;
                while (edgesToBeContracted.Contains(compareEdge))
                {
                    if (comparedEdge[compareEdge] == edge)
                    {
                        skip = true;
                        break;
                    }
                    compareEdge = comparedEdge[compareEdge];
                }

                if (skip)
                {
                    continue;
                }

                if (AllDemandPairsPassThroughAnotherEdge(edge, compareEdge))
                {
                    edgesToBeContracted.Add(edge);
                    comparedEdge[edge] = compareEdge;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determine and return all edges that are a neighbour of <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">The edge for which we want to determine its neighbours.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all edges that are a neighbour of <paramref name="edge"/>.</returns>
        private IEnumerable<Edge<Node>> NeighbouringEdges(Edge<Node> edge)
        {
            IEnumerable<Edge<Node>> part1 = Tree.GetNeighbouringEdges(edge.Endpoint1, Measurements.TreeOperationsCounter).Where(e => e != edge);
            IEnumerable<Edge<Node>> part2 = Tree.GetNeighbouringEdges(edge.Endpoint2, Measurements.TreeOperationsCounter).Where(e => e != edge);
            return part1.Concat(part2);
        }

        /// <summary>
        /// Try to apply this <see cref="ReductionRule"/> on the edges in <paramref name="edgesToCheck"/>.
        /// </summary>
        /// <param name="edgesToCheck">The <see cref="IEnumerable{T}"/> with edges we want to check.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/> successfully, <see langword="false"/> otherwise.</returns>
        protected override bool TryApplyReductionRule(IEnumerable<Edge<Node>> edgesToCheck)
        {
            Dictionary<Edge<Node>, Edge<Node>> comparedEdge = new();
            HashSet<Edge<Node>> edgesToBeContracted = new();

            foreach (Edge<Node> edge in edgesToCheck)
            {
                if (edgesToBeContracted.Contains(edge) || !Tree.HasNode(edge.Endpoint1, Measurements.TreeOperationsCounter) || !Tree.HasNode(edge.Endpoint2, Measurements.TreeOperationsCounter) || !Tree.HasEdge(edge, Measurements.TreeOperationsCounter))
                {
                    continue;
                }

                // Compare the edge to its neighbours. If we contracted it this way, go to the next edge.
                if (CompareEdgeToNeighbours(edge, edgesToBeContracted, comparedEdge))
                {
                    continue;
                }

                // We have not contracted this edge yet, so we will look at all the edges (that are not yet contracted) on the shortest demand path through this edge and compare this edge to those.
                IEnumerable<Edge<Node>> demandPairEdges = FindEdgesOnShortestDemandPathThroughEdge(edge).Where(e => e != edge && !edgesToBeContracted.Contains(e));
                foreach (Edge<Node> otherEdge in demandPairEdges)
                {
                    if (AllDemandPairsPassThroughAnotherEdge(edge, otherEdge))
                    {
                        edgesToBeContracted.Add(edge);
                        comparedEdge[edge] = otherEdge;
                        break;
                    }
                }
            }

            return TryContractEdges(new CountedList<Edge<Node>>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }
    }
}
