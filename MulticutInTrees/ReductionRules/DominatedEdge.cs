// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// A <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="List{T}"/> of <see cref="DemandPair"/>s as value.
        /// The value is all the <see cref="DemandPair"/>s whose path passes through the key.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for <see cref="IdleEdge"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="IdleEdge"/> is part of.</param>
        /// <param name="random">The <see cref="Random"/> used for random number generation.</param>
        /// <param name="demandPairsPerEdge">The <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="List{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="random"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public DominatedEdge(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, Random random, CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm, random, "Dominated Edge")
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the DominatedEdge reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the DominatedEdge reduction rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), "Trying to create an instance of the DominatedEdge reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(random, nameof(random), "Trying to create an instance of the DominatedEdge reduction rule, but the random is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), "Trying to create an instance of the DominatedEdge reduction rule, but the dictionary with demand paths per edge is null!");
#endif
            DemandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedList<DemandPair>>(demandPairsPerEdge);
        }

        /// <summary>
        /// Checks whether we can contract <paramref name="contractEdge"/>. We can do this if all <see cref="DemandPair"/>s passing through it also pass through <paramref name="otherEdge"/>.
        /// </summary>
        /// <param name="contractEdge">The edge for which we want to know whether we can contract it.</param>
        /// <param name="otherEdge">The edge we are comparing <paramref name="contractEdge"/> against.</param>
        /// <returns><see langword="true"/> if all <see cref="DemandPair"/>s that pas through <paramref name="contractEdge"/> also pass through <paramref name="otherEdge"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="contractEdge"/> or <paramref name="otherEdge"/> is <see langword="null"/>.</exception>
        private bool AllDemandPairsPassThroughAnotherEdge((TreeNode, TreeNode) contractEdge, (TreeNode, TreeNode) otherEdge)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractEdge.Item1, nameof(contractEdge.Item1), "Trying to see whether all demand paths that pass through an edge also pass through another, but the first endpoint of the first edge is null!");
            Utils.NullCheck(contractEdge.Item2, nameof(contractEdge.Item2), "Trying to see whether all demand paths that pass through an edge also pass through another, but the second endpoint of the first edge is null!");
            Utils.NullCheck(otherEdge.Item1, nameof(otherEdge.Item1), "Trying to see whether all demand paths that pass through an edge also pass through another, but the first endpoint of the second edge is null!");
            Utils.NullCheck(otherEdge.Item2, nameof(otherEdge.Item2), "Trying to see whether all demand paths that pass through an edge also pass through another, but the second endpoint of the second edge is null!");
#endif
            return DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(contractEdge)].GetCountedEnumerable(new Counter()).IsSubsetOf(DemandPairsPerEdge[Utils.OrderEdgeSmallToLarge(otherEdge)].GetCountedEnumerable(new Counter()));
        }

        /// <summary>
        /// Finds all edges that are on the shortest <see cref="DemandPair"/> through <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        private ReadOnlyCollection<(TreeNode, TreeNode)> FindEdgesOnShortestDemandPathThroughEdge((TreeNode, TreeNode) edge)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to find all edges on the shortest demand path through this edge, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to find all edges on the shortest demand path through this edge, but the second endpoint of the edge is null!");
#endif
            if (!DemandPairsPerEdge.TryGetValue(edge, out CountedList<DemandPair> demandPathsOnEdge)) 
            {
                return new List<(TreeNode, TreeNode)>().AsReadOnly();
            }

            DemandPair shortestPathThroughThisEdge = demandPathsOnEdge.GetCountedEnumerable(new Counter()).Aggregate((n, m) => n.EdgesOnDemandPath.Count(Measurements.DemandPairsOperationsCounter) < m.EdgesOnDemandPath.Count(Measurements.DemandPairsOperationsCounter) ? n : m);
            return shortestPathThroughThisEdge.EdgesOnDemandPath.GetCountedEnumerable(new Counter()).Select(n => Utils.OrderEdgeSmallToLarge(n)).ToList().AsReadOnly();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="changedEdgesPerDemandPairList"/> is <see langword="null"/>.</exception>
        // TODO: Van parameter list maken zodat counted niet doorgegeven kan worden, en dan hier een countedlist ervan maken met een counter die alleen hier bestaat. Ook bij andere methoden en de andere reduction rules.
        internal override bool AfterDemandPathChanged(CountedList<(List<(TreeNode, TreeNode)>, DemandPair)> changedEdgesPerDemandPairList)
        {
#if !EXPERIMENT
            Utils.NullCheck(changedEdgesPerDemandPairList, nameof(changedEdgesPerDemandPairList), "Trying to apply the Dominated Edge rule after a demand path was changed, but the IEnumerable of changed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Dominated Edge rule after a demand path was changed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeChecked = new HashSet<(TreeNode, TreeNode)>();
            foreach ((List<(TreeNode, TreeNode)> edges, DemandPair _) in changedEdgesPerDemandPairList.GetCountedEnumerable(new Counter()))
            {
                foreach ((TreeNode, TreeNode) edge in edges)
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            List<(TreeNode, TreeNode)> edgesToBeContracted = new List<(TreeNode, TreeNode)>();

            foreach ((TreeNode, TreeNode) edge in edgesToBeChecked)
            {
                foreach ((TreeNode, TreeNode) otherEdge in FindEdgesOnShortestDemandPathThroughEdge(edge))
                {
                    if (edge == otherEdge)
                    {
                        continue;
                    }
                    if (AllDemandPairsPassThroughAnotherEdge(edge, otherEdge))
                    {
                        edgesToBeContracted.Add(edge);
                        break;
                    }
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();

            return TryContractEdges(edgesToBeContracted);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removedDemandPairs"/> is <see langword="null"/>.</exception>
        internal override bool AfterDemandPathRemove(CountedList<DemandPair> removedDemandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(removedDemandPairs, nameof(removedDemandPairs), "Trying to apply the Dominated Edge rule after a demand path was removed, but the IEnumerable of removed demand paths is null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine("Applying Dominated Edge rule after a demand path was removed...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeChecked = new HashSet<(TreeNode, TreeNode)>();
            foreach (DemandPair demandPair in removedDemandPairs.GetCountedEnumerable(new Counter()))
            {
                if (demandPair.EdgesOnDemandPath.Count(Measurements.DemandPairsOperationsCounter) == 0)
                {
                    continue;
                }

                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath.GetCountedEnumerable(new Counter()))
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();

            foreach ((TreeNode, TreeNode) edge in edgesToBeChecked)
            {
                if (edgesToBeContracted.Contains(edge))
                {
                    continue;
                }
                foreach ((TreeNode, TreeNode) otherEdge in FindEdgesOnShortestDemandPathThroughEdge(edge))
                {
                    if (edge == otherEdge || edgesToBeContracted.Contains(otherEdge))
                    {
                        continue;
                    }
                    if (AllDemandPairsPassThroughAnotherEdge(edge, otherEdge))
                    {
                        edgesToBeContracted.Add(edge);
                        break;
                    }
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();

            return TryContractEdges(edgesToBeContracted.ToList());
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contractedEdgeNodeTupleList"/> is <see langword="null"/>.</exception>
        internal override bool AfterEdgeContraction(CountedList<((TreeNode, TreeNode), TreeNode, CountedList<DemandPair>)> contractedEdgeNodeTupleList)
        {
#if !EXPERIMENT
            Utils.NullCheck(contractedEdgeNodeTupleList, nameof(contractedEdgeNodeTupleList), "Trying to apply the Dominated Edge rule after an edge was contracted, but the IEnumerable of contracted edges is null!");
#endif
            return false;
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine("Applying Dominated Edge rule for the first time...");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();

            foreach ((TreeNode, TreeNode) edge1 in Tree.Edges)
            {
                foreach ((TreeNode, TreeNode) edge2 in Tree.Edges)
                {
                    if (edgesToBeContracted.Contains(edge1))
                    {
                        break;
                    }
                    if (edge1 == edge2)
                    {
                        continue;
                    }
                    if (edgesToBeContracted.Contains(edge2))
                    {
                        continue;
                    }
                    if (AllDemandPairsPassThroughAnotherEdge(edge2, edge1))
                    {
                        edgesToBeContracted.Add(edge2);
                    }
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();

            return TryContractEdges(edgesToBeContracted.ToList());
        }

        /// <inheritdoc/>
        protected override void Preprocess()
        {
            return;
        }

        /// <summary>
        /// Contract all edges in <paramref name="edgesToBeContracted"/>.
        /// </summary>
        /// <param name="edgesToBeContracted">The <see cref="List{T}"/> with all edges to be contracted.</param>
        /// <returns><see langword="true"/> if <paramref name="edgesToBeContracted"/> has any elements, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edgesToBeContracted"/> is <see langword="null"/>.</exception>
        private bool TryContractEdges(List<(TreeNode, TreeNode)> edgesToBeContracted)
        {
#if !EXPERIMENT
            Utils.NullCheck(edgesToBeContracted, nameof(edgesToBeContracted), $"Trying to contract edges, but the List with edges is null!");
#endif
            if (edgesToBeContracted.Count == 0)
            {
                return false;
            }

            Algorithm.ContractEdges(edgesToBeContracted, Measurements);
            return true;
        }
    }
}