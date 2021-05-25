// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;
using MulticutInTrees.Utilities.Matching;

namespace MulticutInTrees.ReductionRules
{
    /// <summary>
    /// Implementation of the Generalised Dominating Wingspan reduction rule.
    /// <br/>
    /// <b>Rule:</b> Assume that u is an L2-leaf of the caterpillar C, and that u covers C. Assume that for every closest neighbour v of u in A(u), there exist k+1 endpoint-disjoint demand paths between a node lying toward b(u) from u and a node toward v from a(u). Then we contract the edge connected to u.
    /// </summary>
    public class GeneralisedDominatingWingspan : ReductionRule
    {
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> with the identifier of the caterpillar component each <see cref="Node"/> is part of, or -1 if it is not part of any caterpillar component.
        /// </summary>
        protected CountedDictionary<Node, int> CaterpillarComponentPerNode { get; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="Node"/>.
        /// </summary>
        protected CountedDictionary<Node, CountedCollection<DemandPair>> DemandPairsPerNode { get; }

        /// <summary>
        /// The <see cref="List{T}"/> of tuples of <see cref="Node"/>s that contains the already found part of the solution.
        /// </summary>
        protected List<Edge<Node>> PartialSolution { get; }

        /// <summary>
        /// The size the cutset is allowed to be.
        /// </summary>
        protected int MaxSolutionSize { get; }

        /// <summary>
        /// Constructor for <see cref="GeneralisedDominatingWingspan"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Graph"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="ReductionRule"/> is part of.</param>
        /// <param name="demandPairsPerNode"><see cref="CountedDictionary{TKey, TValue}"/> with for each <see cref="Node"/> all <see cref="DemandPair"/>s that start at that <see cref="Node"/>.</param>
        /// <param name="caterpillarComponentPerNode"><see cref="CountedDictionary{TKey, TValue}"/> with the identifier of the caterpillar component per node.</param>
        /// <param name="partialSolution">The <see cref="List{T}"/> with the edges that are definitely part of the solution.</param>
        /// <param name="maxSolutionSize">The maximum size the solution is allowed to be.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/>, <paramref name="demandPairsPerNode"/>, <paramref name="caterpillarComponentPerNode"/> or <paramref name="partialSolution"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSolutionSize"/> is smaller than zero.</exception>
        public GeneralisedDominatingWingspan(Graph tree, CountedCollection<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<Node, CountedCollection<DemandPair>> demandPairsPerNode, CountedDictionary<Node, int> caterpillarComponentPerNode, List<Edge<Node>> partialSolution, int maxSolutionSize) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} reduction rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} reduction rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerNode, nameof(demandPairsPerNode), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with demand pairs per node is null!");
            Utils.NullCheck(caterpillarComponentPerNode, nameof(caterpillarComponentPerNode), $"Trying to create an instance of the {GetType().Name} reduction rule, but the dictionary with the caterpillar component per node is null!");
            Utils.NullCheck(partialSolution, nameof(partialSolution), $"Trying to create an instance of the {GetType().Name} reduction rule, but the list with the partial solution is null!");
            if (maxSolutionSize < 0)
            {
                throw new ArgumentOutOfRangeException($"Trying to create an instance of the {GetType().Name} reduction rule, but the maximum solution size parameter is smaller than zero!");
            }
#endif
            DemandPairsPerNode = demandPairsPerNode;
            CaterpillarComponentPerNode = caterpillarComponentPerNode;
            PartialSolution = partialSolution;
            MaxSolutionSize = maxSolutionSize;
        }

        /// <summary>
        /// Find the extremities of the caterpillar component <paramref name="node"/> belongs to.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> for which we want to know the extremities of its caterpillar component.</param>
        /// <returns>A tuple of <see cref="Node"/>s that are the extremities of the caterpillar component <paramref name="node"/> belongs to.</returns>
        private (Node, Node) ComputeExtremities(Node node)
        {
            int caterpillar = CaterpillarComponentPerNode[node, Measurements.TreeOperationsCounter];
            Node extremity1 = CaterpillarComponentPerNode.GetCountedEnumerable(Measurements.TreeOperationsCounter).First(kv => kv.Value == caterpillar && !(kv.Key.Neighbours(Measurements.TreeOperationsCounter).FirstOrDefault(n => CaterpillarComponentPerNode[n, Measurements.TreeOperationsCounter] != caterpillar) is null)).Key;
            Node extremity2 = CaterpillarComponentPerNode.GetCountedEnumerable(Measurements.TreeOperationsCounter).Last(kv => kv.Value == caterpillar && !(kv.Key.Neighbours(Measurements.TreeOperationsCounter).FirstOrDefault(n => CaterpillarComponentPerNode[n, Measurements.TreeOperationsCounter] != caterpillar) is null)).Key;
            return (extremity1, extremity2);
        }

        /// <summary>
        /// Find the minimal <see cref="DemandPair"/> going left and the minimal <see cref="DemandPair"/> going right from <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> for which we want to knwo its minimal <see cref="DemandPair"/>s.</param>
        /// <returns>The minimal <see cref="DemandPair"/> going left and the minimal <see cref="DemandPair"/> going right from <paramref name="node"/>, or <see langword="null"/> if any of these cannot be found.</returns>
        private (DemandPair, DemandPair) MinimalDPs(Node node)
        {
            Edge<Node> leafEdge = Tree.GetNeighbouringEdges(node, Measurements.TreeOperationsCounter).First();
            Node parent = node.Neighbours(Measurements.TreeOperationsCounter).First();
            Edge<Node> left = Tree.GetNeighbouringEdges(parent, Measurements.TreeOperationsCounter).First(e => e.BetweenInternalNodes());
            Edge<Node> right = Tree.GetNeighbouringEdges(parent, Measurements.TreeOperationsCounter).Last(e => e.BetweenInternalNodes());

            DemandPair leftDP = null;
            DemandPair rightDP = null;

            foreach (DemandPair dp in DemandPairsPerNode[node, Measurements.DemandPairsPerEdgeValuesCounter].GetCountedEnumerable(Measurements.DemandPairsPerEdgeValuesCounter))
            {
                if (dp.EdgeIsPartOfPath(left, Measurements.DemandPairsOperationsCounter))
                {
                    if (leftDP is null || dp.LengthOfPath(Measurements.DemandPairsOperationsCounter) < leftDP.LengthOfPath(MockCounter))
                    {
                        leftDP = dp;
                    }
                }
                else
                {
                    if (rightDP is null || dp.LengthOfPath(Measurements.DemandPairsOperationsCounter) < rightDP.LengthOfPath(MockCounter))
                    {
                        rightDP = dp;
                    }
                }
            }

            return (leftDP, rightDP);
        }

        /// <summary>
        /// Checks whether the caterpillar component of a <see cref="Node"/> is covered by that node given the extremities of the wingspan and the minimal <see cref="DemandPair"/>s going left and right.
        /// </summary>
        /// <param name="extremity1">One of the extremities of the wingspan.</param>
        /// <param name="extremity2">The other extremity of the wingspan.</param>
        /// <param name="leftDP">The minimal <see cref="DemandPair"/> going left.</param>
        /// <param name="rightDP">The minimal <see cref="DemandPair"/> going right.</param>
        /// <returns><see langword="true"/> if the caterpillar component between <paramref name="extremity1"/> and <paramref name="extremity2"/> is coverd by <paramref name="leftDP"/> and <paramref name="rightDP"/>, <see langword="false"/> otherwise.</returns>
        private bool LeafCoversItsWingspan(Node extremity1, Node extremity2, DemandPair leftDP, DemandPair rightDP)
        {
            return (leftDP.NodeIsPartOfPath(extremity1, MockCounter) && rightDP.NodeIsPartOfPath(extremity2, MockCounter)) || (leftDP.NodeIsPartOfPath(extremity2, MockCounter) && rightDP.NodeIsPartOfPath(extremity1, MockCounter));
        }

        /// <summary>
        /// Apply this <see cref="ReductionRule"/> on a single <see cref="Node"/>.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> on which we want to apply this <see cref="ReductionRule"/>.</param>
        /// <returns><see langword="true"/> if we can apply this <see cref="ReductionRule"/> on <paramref name="node"/>, <see langword="false"/> otherwise.</returns>
        private bool ApplyReductionRule(Node node)
        {
            (Node extremity1, Node extremity2) = ComputeExtremities(node);
            (DemandPair leftDP, DemandPair rightDP) = MinimalDPs(node);

            if (leftDP is null || rightDP is null || !LeafCoversItsWingspan(extremity1, extremity2, leftDP, rightDP))
            {
                return false;
            }

            Node v = leftDP.Node1 == node ? leftDP.Node2 : leftDP.Node1;
            v = v.Degree(Measurements.TreeOperationsCounter) == 1 ? v.Neighbours(Measurements.TreeOperationsCounter).First() : v;
            Node extremityA = leftDP.NodeIsPartOfPath(extremity1, MockCounter) ? extremity1 : extremity2;
            Node extremityB = extremityA == extremity1 ? extremity2 : extremity1;

            HashSet<Node> allNodes = new();

            IEnumerable<Node> leftSide = DFS.FindPathBetween(extremityA, v, Measurements.TreeOperationsCounter);
            foreach (Node n in leftSide)
            {
                allNodes.Add(n);
                foreach (Node leaf in n.Neighbours(Measurements.TreeOperationsCounter).Where(neighbour => neighbour.Degree(Measurements.TreeOperationsCounter) == 1))
                {
                    allNodes.Add(leaf);
                }
            }

            IEnumerable<Node> rightSide = DFS.FindPathBetween(extremityB, node, Measurements.TreeOperationsCounter);
            foreach (Node n in rightSide)
            {
                allNodes.Add(n);
                foreach (Node leaf in n.Neighbours(Measurements.TreeOperationsCounter).Where(neighbour => neighbour.Degree(Measurements.TreeOperationsCounter) == 1))
                {
                    allNodes.Add(leaf);
                }
            }

            IEnumerable<DemandPair> dpsInMatching = DemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter).Where(dp => allNodes.Contains(dp.Node1) && allNodes.Contains(dp.Node2));

            return CreateGraphAndComputeMatching(allNodes, dpsInMatching);
        }

        /// <summary>
        /// Create a <see cref="Graph"/> with a node for each element in <paramref name="allNodes"/> nodes and an edge for each element in <paramref name="dpsInMatching"/>, and perform a matching on it.
        /// </summary>
        /// <param name="allNodes"><see cref="IEnumerable{T}"/> with <see cref="Node"/>s that should be in the matching <see cref="Graph"/>.</param>
        /// <param name="dpsInMatching"><see cref="IEnumerable{T}"/> with <see cref="DemandPair"/>s that respresent edges in the matching <see cref="Graph"/>.</param>
        /// <returns><see langword="true"/> if the matching in the graph has size at least K+1, <see langword="false"/> otherwise.</returns>
        private bool CreateGraphAndComputeMatching(IEnumerable<Node> allNodes, IEnumerable<DemandPair> dpsInMatching)
        {
            Graph matchingGraph = new();
            Dictionary<Node, Node> oldToNewNodes = new();
            List<Node> newNodes = new();
            foreach (Node n in allNodes)
            {
                Node newN = new(n.ID);
                newNodes.Add(newN);
                oldToNewNodes[n] = newN;
                matchingGraph.AddNode(newN, MockCounter);
            }
            foreach (DemandPair demandPair in dpsInMatching)
            {
                Edge<Node> newEdge = new(oldToNewNodes[demandPair.Node1], oldToNewNodes[demandPair.Node2]);
                matchingGraph.AddEdge(newEdge, MockCounter);
            }

            int requiredSize = MaxSolutionSize - PartialSolution.Count + 1;
            return MatchingLibrary.HasMatchingOfSize<Graph, Edge<Node>, Node>(matchingGraph, requiredSize, Measurements.TreeOperationsCounter);
        }

        /// <summary>
        /// Try to apply this <see cref="ReductionRule"/> on all <see cref="Node"/>s in <paramref name="nodesToCheck"/>.
        /// <br/>
        /// <b>NOTE:</b> This <see cref="ReductionRule"/> is only applied on a single node per application!
        /// </summary>
        /// <param name="nodesToCheck">The <see cref="IEnumerable{T}"/> with all <see cref="Node"/>s to check.</param>
        /// <returns><see langword="true"/> if we can apply this <see cref="ReductionRule"/>, <see langword="false"/> otherwise.</returns>
        private bool TryApplyReductionRule(IEnumerable<Node> nodesToCheck)
        {
            foreach (Node node in nodesToCheck)
            {
                if (node.Type != NodeType.L2 || !DemandPairsPerNode.ContainsKey(node, Measurements.DemandPairsPerEdgeKeysCounter))
                {
                    continue;
                }

                if (ApplyReductionRule(node))
                {
                    // todo: Immediately stops after a single node was successful for now.
                    return TryContractEdges(new CountedList<Edge<Node>>(new List<Edge<Node>>() { Tree.GetNeighbouringEdges(node, Measurements.TreeOperationsCounter).First() }, Measurements.TreeOperationsCounter));
                }
            }

            Measurements.TimeSpentCheckingApplicability.Stop();
            return false;
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying the {GetType().Name} reduction rule for the first time");
#endif
            HasRun = true;
            LastContractedEdges.Clear(MockCounter);
            LastRemovedDemandPairs.Clear(MockCounter);
            LastChangedDemandPairs.Clear(MockCounter);
            if (CaterpillarComponentPerNode.Count(MockCounter) == 0)
            {
                foreach (KeyValuePair<Node, int> kv in DFS.DetermineCaterpillarComponents(Tree.Nodes(Measurements.TreeOperationsCounter), Measurements.TreeOperationsCounter))
                {
                    CaterpillarComponentPerNode.Add(kv.Key, kv.Value, MockCounter);
                }
            }

            Measurements.TimeSpentCheckingApplicability.Start();

            return TryApplyReductionRule(Tree.Nodes(Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying the {GetType().Name} reduction rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();
            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            return TryApplyReductionRule(Tree.Nodes(Measurements.TreeOperationsCounter));
        }
    }
}
