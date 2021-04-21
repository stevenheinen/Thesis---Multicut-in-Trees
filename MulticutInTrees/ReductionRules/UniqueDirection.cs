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
    /// <see cref="ReductionRule"/> that contracts edges when all <see cref="DemandPair"/>s starting at a node go in the same direction.
    /// <br/>
    /// <b>Rule:</b> If all the demand paths starting at a leaf u have the same direction (for at least 2 edges), then contract the edge that is connected to u. If all the demand paths starting at an inner node u have the same direction (for at least 1 edge), then contract the edge e adjacent to u which does not belong to any demand path starting at u.
    /// </summary>
    public class UniqueDirection : ReductionRule
    {
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per <see cref="Node"/>.
        /// </summary>
        private CountedDictionary<Node, CountedCollection<DemandPair>> DemandPairsPerNode { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge.
        /// </summary>
        private CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for the <see cref="DominatedPath"/> reduction rule.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is used by.</param>
        /// <param name="demandPairsPerNode"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per <see cref="Node"/>.</param>
        /// <param name="demandPairsPerEdge"><see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="demandPairsPerNode"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public UniqueDirection(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode, CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with demand paths is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerNode, nameof(demandPairsPerNode), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per node is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per edge is null!");
#endif
            DemandPairsPerNode = demandPairsPerNode;
            DemandPairsPerEdge = demandPairsPerEdge;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Determine for a given <see cref="Node"/> which edges can be contracted according to this <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> for which we want to determine which edges can be contracted.</param>
        /// <param name="edgesToBeContracted"><see cref="HashSet{T}"/> containing all edges that can be contracted in this application of this <see cref="ReductionRule"/>.</param>
        private void CheckApplicabilityNode(Node node, HashSet<Edge<Node>> edgesToBeContracted)
        {
            // todo: temp
            Console.WriteLine($"Now checking {node}");

            if (!DemandPairsPerNode.TryGetValue(node, out CountedCollection<DemandPair> dpsAtNode, Measurements.DemandPairsPerEdgeKeysCounter))
            {
                // todo: temp
                Console.WriteLine($"No DPs at {node}, going to the next one");
                return;
            }

            if (node.Degree(Measurements.TreeOperationsCounter) == 1)
            {
                // todo: temp
                Console.WriteLine($"{node} is a leaf");
                CheckApplicabilityLeaf(node, dpsAtNode, edgesToBeContracted);
            }
            else
            {
                // todo: temp
                Console.WriteLine($"{node} is an internal node");
                CheckApplicabilityInternalNode(node, dpsAtNode, edgesToBeContracted);
            }
        }

        /// <summary>
        /// Determine for a given internal <see cref="Node"/> which edges can be contracted according to this <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> for which we want to determine which edges can be contracted.</param>
        /// <param name="dpsAtNode">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s that start at <paramref name="node"/>.</param>
        /// <param name="edgesToBeContracted"><see cref="HashSet{T}"/> containing all edges that can be contracted in this application of this <see cref="ReductionRule"/>.</param>
        private void CheckApplicabilityInternalNode(Node node, CountedCollection<DemandPair> dpsAtNode, HashSet<Edge<Node>> edgesToBeContracted)
        {
            foreach (Node neighbour in node.Neighbours(Measurements.TreeOperationsCounter))
            {
                if (neighbour.Type == NodeType.L1 || neighbour.Type == NodeType.L2 || neighbour.Type == NodeType.L3)
                {
                    // todo: temp
                    Console.WriteLine($"{node} has a leaf, so we do nothing");

                    return;
                }
            }

            Edge<Node> firstEdge = DetermineCommonEdge(node, dpsAtNode);

            if (firstEdge is null)
            {
                // todo: temp
                Console.WriteLine($"The DPs do not share a common edge {node}");
                return;
            }

            // For the other edges connected to this node, check if we can contract it. We can contract it if, for each demand path on it, not all edges on that demand path are being contracted.
            foreach (Edge<Node> edge in Tree.GetNeighbouringEdges(node, Measurements.TreeOperationsCounter))
            {
                if (edge == firstEdge)
                {
                    continue;
                }

                // todo: temp
                Console.WriteLine($"Now checking {edge}");

                if (CanContractNeighbouringEdge(edge, edgesToBeContracted))
                {
                    edgesToBeContracted.Add(edge);
                }
            }
        }

        /// <summary>
        /// Checks for a given edge whether it can be contracted. This is the case when it is not used by a <see cref="DemandPair"/> whose all other edges will be contracted.
        /// </summary>
        /// <param name="edge">The edge we will potentially contract.</param>
        /// <param name="edgesToBeContracted">The <see cref="HashSet{T}"/> with all edges that will be contracted.</param>
        /// <returns><see langword="true"/> if we can contract <paramref name="edge"/>, <see langword="false"/> otherwise.</returns>
        private bool CanContractNeighbouringEdge(Edge<Node> edge, HashSet<Edge<Node>> edgesToBeContracted)
        {
            if (!DemandPairsPerEdge.TryGetValue(edge, out CountedCollection<DemandPair> dps, Measurements.DemandPairsPerEdgeKeysCounter))
            {
                // todo: temp
                Console.WriteLine($"{edge} is not used by any DP, so we can contract it");

                // No demand pairs use this edge, so we can safely contract it.
                return true;
            }

            foreach (DemandPair dp in dps.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                bool allOtherEdgesOnPathWillBeContracted = true;
                foreach (Edge<Node> e in dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter))
                {
                    if (edge == e)
                    {
                        continue;
                    }

                    if (!edgesToBeContracted.Contains(e))
                    {
                        allOtherEdgesOnPathWillBeContracted = false;
                        break;
                    }
                }

                if (allOtherEdgesOnPathWillBeContracted)
                {
                    // todo: temp
                    Console.WriteLine($"all other edges on a dp going through {edge} will be contracted, so we cannot contract {edge}");
                    
                    return false;
                }
            }

            // todo: temp
            Console.WriteLine($"We can contract {edge}");

            return true;
        }

        /// <summary>
        /// Determine the edge that all <see cref="DemandPair"/>s that start at <paramref name="node"/> have in common, if it exists.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> at which all <see cref="DemandPair"/>s start.</param>
        /// <param name="dpsAtNode">The <see cref="DemandPair"/>s that start at <paramref name="node"/>.</param>
        /// <returns>The edge connected to <paramref name="node"/> that all <see cref="DemandPair"/>s in <paramref name="dpsAtNode"/> use, or <see langword="null"/> if that edge does not exist.</returns>
        private Edge<Node> DetermineCommonEdge(Node node, CountedCollection<DemandPair> dpsAtNode)
        {
            Edge<Node> firstEdge = null;

            // Determine whether the demand pairs that start at this node go in the same direction.
            foreach (DemandPair dp in dpsAtNode.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                Edge<Node> edge;

                if (dp.Node1 == node)
                {
                    edge = dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(0);
                }
                else
                {
                    int length = dp.LengthOfPath(MockCounter);
                    edge = dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(length - 1);
                }

                if (firstEdge is null)
                {
                    firstEdge = edge;
                }
                else if (firstEdge != edge)
                {
                    return null;
                }
            }

            return firstEdge;
        }

        /// <summary>
        /// Determine for a given leaf <see cref="Node"/> which edges can be contracted according to this <see cref="ReductionRule"/>.
        /// </summary>
        /// <param name="leaf">The <see cref="Node"/> for which we want to determine which edges can be contracted.</param>
        /// <param name="dpsAtNode">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s that start at <paramref name="leaf"/>.</param>
        /// <param name="edgesToBeContracted"><see cref="HashSet{T}"/> containing all edges that can be contracted in this application of this <see cref="ReductionRule"/>.</param>
        private void CheckApplicabilityLeaf(Node leaf, CountedCollection<DemandPair> dpsAtNode, HashSet<Edge<Node>> edgesToBeContracted)
        {
            Edge<Node> secondEdge = null;

            foreach (DemandPair dp in dpsAtNode.GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                if (dp.LengthOfPath(Measurements.DemandPairsOperationsCounter) < 2)
                {
                    return;
                }

                Edge<Node> edge;
                bool allOtherEdgesOnPathWillBeContracted = true;

                if (dp.Node1 == leaf)
                {
                    edge = dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(1);

                    // Check whether all other edges on this demand path are already being contracted. If so, we cannot contract this edge.
                    foreach (Edge<Node> e in dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Skip(1))
                    {
                        if (!edgesToBeContracted.Contains(e))
                        {
                            allOtherEdgesOnPathWillBeContracted = false;
                            break;
                        }
                    }
                }
                else
                {
                    int length = dp.LengthOfPath(MockCounter);
                    edge = dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).ElementAt(length - 2);

                    // Check whether all other edges on this demand path are already being contracted. If so, we cannot contract this edge.
                    foreach (Edge<Node> e in dp.EdgesOnDemandPath(Measurements.TreeOperationsCounter).Take(length - 1))
                    {
                        if (!edgesToBeContracted.Contains(e))
                        {
                            allOtherEdgesOnPathWillBeContracted = false;
                            break;
                        }
                    }
                }

                if (allOtherEdgesOnPathWillBeContracted)
                {
                    // todo: temp
                    Console.WriteLine($"all other edges on a dp going through the second edge connected to {leaf} will be contracted, so we cannot contract the edge connected to {leaf}");

                    return;
                }

                if (secondEdge is null)
                {
                    secondEdge = edge;
                }
                else if (secondEdge != edge)
                {
                    // todo: temp
                    Console.WriteLine($"the dps starting at {leaf} do not share the second edge, so we cannot contract it");

                    return;
                }
            }

            edgesToBeContracted.Add(Tree.GetNeighbouringEdges(leaf, Measurements.TreeOperationsCounter).First());
        }

        /// <summary>
        /// Finds and contracts all edges that can be contracted by checking all <see cref="Node"/>s in <paramref name="nodesToCheck"/>.
        /// </summary>
        /// <param name="nodesToCheck">The <see cref="IEnumerable{T}"/> with <see cref="Node"/>s to check.</param>
        /// <returns>Whether this <see cref="ReductionRule"/> was applicable.</returns>
        private bool TryApplyReductionRule(IEnumerable<Node> nodesToCheck)
        {
            HashSet<Edge<Node>> edgesToBeContracted = new();
            foreach (Node node in nodesToCheck)
            {
                CheckApplicabilityNode(node, edgesToBeContracted);
            }

            return TryContractEdges(new CountedList<Edge<Node>>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            HashSet<Node> nodesToCheck = new();

            foreach ((Edge<Node> _, Node node, CountedCollection<DemandPair> _) in LastContractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                nodesToCheck.Add(node);
            }

            foreach (DemandPair dp in LastRemovedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                nodesToCheck.Add(dp.Node1);
                nodesToCheck.Add(dp.Node2);
            }

            foreach ((CountedList<Edge<Node>> edges, DemandPair _) in LastChangedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach (Edge<Node> edge in edges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    nodesToCheck.Add(edge.Endpoint1);
                    nodesToCheck.Add(edge.Endpoint2);
                }
            }

            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return TryApplyReductionRule(nodesToCheck);
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule for the first time");
#endif
            HasRun = true;
            Measurements.TimeSpentCheckingApplicability.Start();
            return TryApplyReductionRule(Tree.Nodes(Measurements.TreeOperationsCounter));
        }
    }
}
