// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;
using MulticutInTrees.Experiments;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Abstract class for an algorithm that solves the Multicut in Trees problem.
    /// </summary>
    public abstract class Algorithm
    {
        /// <summary>
        /// The <see cref="ReadOnlyCollection{T}"/> of reduction rules (of type <see cref="ReductionRule"/>) this <see cref="Algorithm"/> uses.
        /// </summary>
        public ReadOnlyCollection<ReductionRule> ReductionRules { get; protected set; }
        
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge, represented by a tuple of two <see cref="TreeNode"/>s.
        /// </summary>
        protected CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> DemandPairsPerEdge { get; set; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="TreeNode"/>.
        /// </summary>
        protected CountedDictionary<TreeNode, CountedCollection<DemandPair>> DemandPairsPerNode { get; set; }
        
        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> with the identifier of the caterpillar component each <see cref="TreeNode"/> is part of, or -1 if it is not part of any caterpillar component.
        /// </summary>
        protected CountedDictionary<TreeNode, int> CaterpillarComponentPerNode { get; set; }

        /// <summary>
        /// The <see cref="MulticutInstance"/> this <see cref="Algorithm"/> is trying to solve.
        /// </summary>
        protected MulticutInstance Instance { get; }

        /// <summary>
        /// The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the input.
        /// </summary>
        protected CountedList<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The input <see cref="Tree{N}"/>.
        /// </summary>
        protected Tree<TreeNode> Tree { get; }

        /// <summary>
        /// The <see cref="List{T}"/> of tuples of <see cref="TreeNode"/>s that contains the already found part of the solution.
        /// </summary>
        protected List<(TreeNode, TreeNode)> PartialSolution { get; }

        /// <summary>
        /// The size the cutset is allowed to be.
        /// </summary>
        protected int K { get; }

        /// <summary>
        /// <see langword="true"/> if in the last iteration an edge was contracted, <see langword="false"/> otherwise.
        /// </summary>
        protected bool LastIterationEdgeContraction { get; set; }

        /// <summary>
        /// <see langword="true"/> if in the last iteration a demand pair was removed, <see langword="false"/> otherwise.
        /// </summary>
        protected bool LastIterationDemandPairRemoval { get; set; }

        /// <summary>
        /// <see langword="true"/> if in the last iteration a demand pair was changed, <see langword="false"/> otherwise.
        /// </summary>
        protected bool LastIterationDemandPairChange { get; set; }

        /// <summary>
        /// A <see cref="CountedList{T}"/> of all edges that were removed in the last iteration, their contracted nodes, and the <see cref="DemandPair"/>s on the contracted edge.
        /// </summary>
        protected CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> LastContractedEdges { get; set; }

        /// <summary>
        /// A <see cref="CountedList{T}"/> of all <see cref="DemandPair"/>s that were removed in the last iteration.
        /// </summary>
        protected CountedList<DemandPair> LastRemovedDemandPairs { get; set; }

        /// <summary>
        /// A <see cref="CountedList{T}"/> of tuples of changed edges for a <see cref="DemandPair"/> and the <see cref="DemandPair"/> itself.
        /// </summary>
        protected CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> LastChangedEdgesPerDemandPair { get; set; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should <b>NOT</b> be counted for the performance of this <see cref="Algorithm"/>.
        /// </summary>
        protected Counter MockCounter { get; }

        /// <summary>
        /// <see cref="PerformanceMeasurements"/> that are used by the central parts an <see cref="Algorithm"/> does that has nothing to do with <see cref="ReductionRule"/>s, like preprocessing.
        /// </summary>
        protected PerformanceMeasurements AlgorithmPerformanceMeasurements { get; }

        /// <summary>
        /// The <see cref="AlgorithmType"/> of this <see cref="Algorithm"/>.
        /// </summary>
        protected AlgorithmType AlgorithmType { get; }

        /// <summary>
        /// Constructor for an <see cref="Algorithm"/>.
        /// </summary>
        /// <param name="instance">The <see cref="MulticutInstance"/> we want to solve.</param>
        /// <param name="algorithmType">The <see cref="AlgorithmType"/> of the current <see cref="Algorithm"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        protected Algorithm(MulticutInstance instance, AlgorithmType algorithmType)
        {
#if !EXPERIMENT
            Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of a Multicut algorithm, but the problem instance is null!");
#endif
            Instance = instance;
            Tree = instance.Tree;
            DemandPairs = instance.DemandPairs;
            K = instance.K;
            PartialSolution = new List<(TreeNode, TreeNode)>();
            MockCounter = new Counter();
            AlgorithmType = algorithmType;
            AlgorithmPerformanceMeasurements = new PerformanceMeasurements(algorithmType.ToString());
            CaterpillarComponentPerNode = new CountedDictionary<TreeNode, int>();

            LastContractedEdges = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>();
            LastRemovedDemandPairs = new CountedList<DemandPair>();
            LastChangedEdgesPerDemandPair = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>();
            
            FillDemandPathsPerEdge();
        }

        // todo: check whether this results in smaller kernels. It is probably a lot slower. Smaller kernels? Yes. A lot slower? Not as much as expected, but it is slower.
        /// <summary>
        /// Runs the algorithm by using the <see cref="ReductionRule.RunFirstIteration"/> method exclusively. This means unnecessary parts of the graph are checked, but it results in smaller kernels. This part is still WIP. We would like to find a way to skip checking unnecessary parts, but still find the smallest kernel.
        /// </summary>
        /// <returns>A tuple with the <see cref="Tree{N}"/> that is left after kernelisation, a <see cref="List{T}"/> with tuples of two <see cref="TreeNode"/>s representing the edges that are part of the solution, and a <see cref="List{T}"/> of <see cref="DemandPair"/>s that are not yet separated.</returns>
        public (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) RunNaively()
        {
            for (int i = 0; i < ReductionRules.Count; i++)
            {
                if (DemandPairs.Count(AlgorithmPerformanceMeasurements.DemandPairsOperationsCounter) == 0)
                {
                    goto returntrue;
                }
                if (PartialSolution.Count == K)
                {
                    goto returnfalse;
                }

#if VERBOSEDEBUG
                Console.WriteLine($"Now applying rule {i + 1}.");
#endif
                bool success = ReductionRules[i].RunFirstIteration();
                if (success)
                {
                    if (ReductionRules[i].TrueMeansInfeasibleInstance)
                    {
                        goto returnfalse;
                    }

                    // If the application of the i-th reduction rule was a success, start again at rule 0.
                    i = -1;
                }
#if VERBOSEDEBUG
                else
                {
                    Console.WriteLine($"The application of rule {i + 1} was not successful...");
                }
#endif
            }

            returntrue:
            return (Tree, PartialSolution, DemandPairs.GetInternalList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, Instance.K, Instance.OptimalK, true, Tree.NumberOfNodes(MockCounter), DemandPairs.Count(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));

            returnfalse:
            return (Tree, PartialSolution, DemandPairs.GetInternalList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, Instance.K, Instance.OptimalK, false, Tree.NumberOfNodes(MockCounter), DemandPairs.Count(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));
        }

        /// <summary>
        /// Try to solve the instance.
        /// </summary>
        /// <returns>A tuple with the <see cref="Tree{N}"/> that is left after kernelisation, a <see cref="List{T}"/> with tuples of two <see cref="TreeNode"/>s representing the edges that are part of the solution, and a <see cref="List{T}"/> of <see cref="DemandPair"/>s that are not yet separated.</returns>
        public (Tree<TreeNode>, List<(TreeNode, TreeNode)>, List<DemandPair>, ExperimentOutput) Run()
        {
            bool[] appliedReductionRule = new bool[ReductionRules.Count];

            for (int i = 0; i < ReductionRules.Count; i++)
            {
                if (DemandPairs.Count(AlgorithmPerformanceMeasurements.DemandPairsOperationsCounter) == 0)
                {
                    goto returntrue;
                }
                if (PartialSolution.Count == K)
                {
                    goto returnfalse;
                }

#if VERBOSEDEBUG
                Console.WriteLine($"Now applying rule {i + 1}.");
                Console.WriteLine($"In the last iteration we?");
                Console.WriteLine($"Contracted an edge: {LastIterationEdgeContraction}");
                Console.WriteLine($"Changed a dem path: {LastIterationDemandPairChange}");
                Console.WriteLine($"Removed a dem path: {LastIterationDemandPairRemoval}");
#endif
                // If we have not executed this rule before, execute it now.
                if (!appliedReductionRule[i])
                {
                    appliedReductionRule[i] = true;
                    bool success = ReductionRules[i].RunFirstIteration();
                    if (success)
                    {
                        if (ReductionRules[i].TrueMeansInfeasibleInstance)
                        {
                            goto returnfalse;
                        }

                        // If the first application of the i-th reduction rule was a success, start again at rule 0.
                        i = -1;
                    }
#if VERBOSEDEBUG
                    else
                    {
                        Console.WriteLine($"The application of rule {i + 1} was not successful...");
                    }
#endif
                    continue;
                }

                bool successfulAfterDemandPairChanged = false;
                bool successfulAfterDemandPairRemoved = false;
                bool successfulAfterEdgeContracted = false;

                // We have already applied the i-th rule before. Try to apply it again, depending on what happened in the last iteration.
                if (LastIterationDemandPairChange)
                {
                    successfulAfterDemandPairChanged = RunAfterDemandPairChanged(i);
                }
                if (LastIterationDemandPairRemoval)
                {
                    successfulAfterDemandPairRemoved = RunAfterDemandPairRemoved(i);
                }
                if (LastIterationEdgeContraction)
                {
                    successfulAfterEdgeContracted = RunAfterEdgeContraction(i);
                }

                // If we applied the rule successfully, go back to rule 0.
                if (successfulAfterDemandPairChanged || successfulAfterDemandPairRemoved || successfulAfterEdgeContracted)
                {
                    if (ReductionRules[i].TrueMeansInfeasibleInstance)
                    {
                        goto returnfalse;
                    }

                    i = -1;
                }
            }

            returntrue:
            return (Tree, PartialSolution, DemandPairs.GetInternalList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, Instance.K, Instance.OptimalK, true, Tree.NumberOfNodes(MockCounter), DemandPairs.Count(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));

            returnfalse:
            return (Tree, PartialSolution, DemandPairs.GetInternalList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, Instance.K, Instance.OptimalK, false, Tree.NumberOfNodes(MockCounter), DemandPairs.Count(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));
        }

        /// <summary>
        /// Contract an edge.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s representing the edge to be contracted.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <returns>The <see cref="TreeNode"/> that is the resulting node of the edge contraction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either item of <paramref name="edge"/>, or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="edge"/> is not part of the input.</exception>
        /// <exception cref="InvalidEdgeException">Thrown when <paramref name="edge"/> is a self-loop.</exception>
        internal virtual TreeNode ContractEdge((TreeNode, TreeNode) edge, PerformanceMeasurements measurements)
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
            UpdateCaterpillarComponents(usedEdge, measurements);
            UpdateNodeTypesDuringEdgeContraction(usedEdge, newNode);
            Tree.RemoveNode(child, measurements.TreeOperationsCounter);
            CountedCollection<DemandPair> pairsOnEdge = RemoveDemandPairsFromContractedEdge(usedEdge, newNode, measurements);
            UpdateDemandPairsStartingAtContractedEdge(usedEdge, child, newNode, pairsOnEdge, measurements);
            UpdateDemandPairsGoingThroughChild(child, newNode, measurements);

            LastContractedEdges.Add((usedEdge, newNode, pairsOnEdge), measurements.TreeOperationsCounter);
            LastIterationEdgeContraction = true;
            measurements.NumberOfContractedEdgesCounter++;

            return newNode;
        }

        /// <summary>
        /// Remove a <see cref="DemandPair"/> from the problem instance.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> to be removed.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal virtual void RemoveDemandPair(DemandPair demandPair, PerformanceMeasurements measurements)
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

        /// <summary>
        /// Change an endpoint of a <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> whose endpoint changes.</param>
        /// <param name="oldEndpoint">The old endpoint of <paramref name="demandPair"/>.</param>
        /// <param name="newEndpoint">The new endpoint of <paramref name="demandPair"/>.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/>, <paramref name="oldEndpoint"/>, <paramref name="newEndpoint"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal virtual void ChangeEndpointOfDemandPair(DemandPair demandPair, TreeNode oldEndpoint, TreeNode newEndpoint, PerformanceMeasurements measurements)
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
                DemandPairsPerNode[newEndpoint, measurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
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

        /// <summary>
        /// Add this edge to the solution, remove all <see cref="DemandPair"/>s that go over this edge, and contract it.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s representing the edge to be cut.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <returns>The <see cref="TreeNode"/> that is the resulting node of the edge contraction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either item of <paramref name="edge"/>, or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="edge"/> is not part of the tree.</exception>
        /// <exception cref="InvalidEdgeException">Thrown when <paramref name="edge"/> is not a valid edge.</exception>
        internal virtual TreeNode CutEdge((TreeNode, TreeNode) edge, PerformanceMeasurements measurements)
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
            TreeNode res = ContractEdge(edge, measurements);

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

        /// <summary>
        /// Change an endpoint of multiple <see cref="DemandPair"/>s.
        /// </summary>
        /// <param name="demandPairEndpointTuples">The <see cref="IList{T}"/> of tuples containing the <see cref="DemandPair"/> that is changed, the <see cref="TreeNode"/> old endpoint and <see cref="TreeNode"/> new endpoint.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPairEndpointTuples"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal void ChangeEndpointOfDemandPairs(CountedList<(DemandPair, TreeNode, TreeNode)> demandPairEndpointTuples, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPairEndpointTuples, nameof(demandPairEndpointTuples), "Trying to change the endpoints of multple demand pairs, but the IEnumerable with tuples with required information is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to change the endpoints of multple demand pairs, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Changing endpoint of multiple demand pairs: {demandPairEndpointTuples.GetInternalList().Print()}.");
#endif
            foreach ((DemandPair, TreeNode, TreeNode) change in demandPairEndpointTuples.GetCountedEnumerable(measurements.DemandPairsOperationsCounter))
            {
                ChangeEndpointOfDemandPair(change.Item1, change.Item2, change.Item3, measurements);
            }
        }

        /// <summary>
        /// Remove multiple <see cref="DemandPair"/>s from the problem instance.
        /// </summary>
        /// <param name="demandPairs">The <see cref="IList{T}"/> of <see cref="DemandPair"/>s to be removed.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPairs"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal void RemoveDemandPairs(CountedList<DemandPair> demandPairs, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to remove multiple demand pairs, but the IEnumerable of demand pairs is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove multiple demand pairs, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Removing demand pairs {demandPairs.GetInternalList().Print()}.");
#endif
            foreach (DemandPair demandPair in demandPairs.GetCountedEnumerable(measurements.DemandPairsOperationsCounter))
            {
                RemoveDemandPair(demandPair, measurements);
            }
        }

        /// <summary>
        /// Contract multiple edges.
        /// </summary>
        /// <param name="edges">The <see cref="IList{T}"/> of tuples of <see cref="TreeNode"/>s representing the edges to be contracted.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal void ContractEdges(CountedList<(TreeNode, TreeNode)> edges, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), "Trying to contract multiple edges, but the IEnumerable of edges is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to contract multiple edges, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Contracting edges {edges.GetInternalList().Print()}");
#endif
            int nrEdges = edges.Count(measurements.TreeOperationsCounter);
            for (int i = 0; i < nrEdges; i++)
            {
                TreeNode newNode = ContractEdge(edges[i, measurements.TreeOperationsCounter], measurements);
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
        /// Add multiple edges to the solution, remove all <see cref="DemandPair"/>s that go over these edges, and contract them.
        /// </summary>
        /// <param name="edges">The <see cref="IList{T}"/> of tuples of <see cref="TreeNode"/>s that represent the edges to be cut.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal void CutEdges(CountedList<(TreeNode, TreeNode)> edges, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), "Trying to cut multiple edges, but the IEnumerable of edges is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to cut multiple edges, but the performance measures to be used are null!");
#endif
            int nrEdges = edges.Count(measurements.TreeOperationsCounter);
            for (int i = 0; i < nrEdges; i++)
            {
                TreeNode newNode = CutEdge(edges[i, measurements.TreeOperationsCounter], measurements);
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
        /// Removes a <see cref="DemandPair"/> from a given edge in <see cref="Algorithm.DemandPairsPerEdge"/>.
        /// </summary>
        /// <param name="edge">The edge from which <paramref name="demandPair"/> should be removed.</param>
        /// <param name="demandPair">The <see cref="DemandPair"/> that should be removed from <paramref name="edge"/>.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, <paramref name="demandPair"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        protected void RemoveDemandPairFromEdge((TreeNode, TreeNode) edge, DemandPair demandPair, PerformanceMeasurements measurements)
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

        /// <summary>
        /// Removes all <see cref="DemandPair"/>s from the edge that will be contracted.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s that represents the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used during this modification.</param>
        /// <returns>A <see cref="CountedCollection{T}"/> with all the <see cref="DemandPair"/>s that pass through <paramref name="edge"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, <paramref name="newNode"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        protected CountedCollection<DemandPair> RemoveDemandPairsFromContractedEdge((TreeNode, TreeNode) edge, TreeNode newNode, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to remove all demand paths going through an edge, but the first endpoint of this edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to remove all demand paths going through an edge, but the second endpoint of this edge is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to remove all demand paths going through an edge, but the node that is the result of the contraction is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove all demand paths going through an edge, but the performance measures to be used are null!");
#endif
            if (DemandPairsPerEdge.TryGetValue(edge, out CountedCollection<DemandPair> pairsOnEdge, measurements.DemandPairsPerEdgeKeysCounter))
            {
                foreach (DemandPair demandPair in pairsOnEdge.GetCountedEnumerable(measurements.DemandPairsPerEdgeValuesCounter))
                {
                    demandPair.OnEdgeContracted(edge, newNode, measurements.DemandPairsOperationsCounter);
                }
                DemandPairsPerEdge.Remove(edge, measurements.DemandPairsPerEdgeKeysCounter);
            }
            else
            {
                pairsOnEdge = new CountedCollection<DemandPair>();
            }

            return pairsOnEdge;
        }

        /// <summary>
        /// Update the <see cref="DemandPair"/>s that pass through <paramref name="child"/>, but not over the edge that is being contracted.
        /// </summary>
        /// <param name="child">The <see cref="TreeNode"/> that will be removed by the contraction.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the contraction.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/>, <paramref name="newNode"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        protected void UpdateDemandPairsGoingThroughChild(TreeNode child, TreeNode newNode, PerformanceMeasurements measurements)
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
                        DemandPairsPerEdge[newKey, measurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
                    }
                    foreach (DemandPair demandPair in DemandPairsPerEdge[key, measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(MockCounter))
                    {
                        DemandPairsPerEdge[newKey, measurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, measurements.DemandPairsPerEdgeValuesCounter);
                    }
                    keysToBeRemoved.Add(key);
                }
                if (key.Item2 == child)
                {
                    (TreeNode, TreeNode) newKey = Utils.OrderEdgeSmallToLarge((key.Item1, newNode));
                    if (!DemandPairsPerEdge.ContainsKey(newKey, measurements.DemandPairsPerEdgeKeysCounter))
                    {
                        DemandPairsPerEdge[newKey, measurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
                    }
                    foreach (DemandPair demandPair in DemandPairsPerEdge[key, measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(MockCounter))
                    {
                        DemandPairsPerEdge[newKey, measurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, measurements.DemandPairsPerEdgeValuesCounter);
                    }
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
        /// <param name="pairsOnEdge">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s that go over <paramref name="edge"/>.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when either endpoint of <paramref name="edge"/>, <paramref name="child"/>, <paramref name="newNode"/>, <paramref name="pairsOnEdge"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        protected void UpdateDemandPairsStartingAtContractedEdge((TreeNode, TreeNode) edge, TreeNode child, TreeNode newNode, CountedCollection<DemandPair> pairsOnEdge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the first endpoint of the edge that will be contracted is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the second endpoint of the edge that will be contracted is null!");
            Utils.NullCheck(child, nameof(child), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the node that will be removed by the contraction is null!");
            Utils.NullCheck(newNode, nameof(newNode), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the node that is the result of the contraction is null!");
            Utils.NullCheck(pairsOnEdge, nameof(pairsOnEdge), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the list of demand pairs going through the contracted edge is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the performance measures to be used are null!");
#endif
            if (DemandPairsPerNode.TryGetValue(child, out CountedCollection<DemandPair> pairsAtChild, measurements.DemandPairsPerEdgeKeysCounter))
            {
                if (!DemandPairsPerNode.ContainsKey(newNode, measurements.DemandPairsPerEdgeKeysCounter))
                {
                    DemandPairsPerNode[newNode, measurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
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
        /// Update the caterpillar components for the affected nodes when an edge is contracted.
        /// </summary>
        /// <param name="contractedEdge">The edge that is contracted.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that need to be used.</param>
        protected void UpdateCaterpillarComponents((TreeNode, TreeNode) contractedEdge, PerformanceMeasurements measurements)
        {
            if (CaterpillarComponentPerNode.Count(MockCounter) == 0)
            {
                return;
            }

            switch (contractedEdge.Item1.Type, contractedEdge.Item2.Type)
            {
                case (NodeType.I1, NodeType.I2):
                    UpdateCaterpillarComponentsI1I2Node(contractedEdge.Item2, measurements);
                    break;
                case (NodeType.I2, NodeType.I1):
                    UpdateCaterpillarComponentsI1I2Node(contractedEdge.Item1, measurements);
                    break;
                case (NodeType.I1, NodeType.I3):
                    UpdateCaterpillarComponentsI1I3Node(contractedEdge.Item1, contractedEdge.Item2, measurements);
                    break;
                case (NodeType.I3, NodeType.I1):
                    UpdateCaterpillarComponentsI1I3Node(contractedEdge.Item2, contractedEdge.Item1, measurements);
                    break;
                case (NodeType.I2, NodeType.I3):
                    UpdateCaterpillarComponentsI2I3Node(contractedEdge.Item1, measurements);
                    break;
                case (NodeType.I3, NodeType.I2):
                    UpdateCaterpillarComponentsI2I3Node(contractedEdge.Item2, measurements);
                    break;
                case (NodeType.L1, NodeType.I1):
                    UpdateCaterpillarComponentsL1I1Node(contractedEdge.Item2, measurements);
                    break;
                case (NodeType.I1, NodeType.L1):
                    UpdateCaterpillarComponentsL1I1Node(contractedEdge.Item1, measurements);
                    break;
            }
        }

        /// <summary>
        /// Update the caterpillar components when an edge between a <see cref="NodeType.L1"/>-leaf and a <see cref="NodeType.I1"/>-node is contracted.
        /// </summary>
        /// <param name="i1Node">The <see cref="NodeType.I1"/>-node of the contracted edge.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that need to be used.</param>
        private void UpdateCaterpillarComponentsL1I1Node(TreeNode i1Node, PerformanceMeasurements measurements)
        {
            if (i1Node.Neighbours(measurements.TreeOperationsCounter).Count() > 2)
            {
                return;
            }
            TreeNode internalNeighbour = i1Node.Neighbours(measurements.TreeOperationsCounter).First(n => n.Degree(MockCounter) > 1);
            int caterpillar = CaterpillarComponentPerNode[internalNeighbour, measurements.TreeOperationsCounter];
            if (caterpillar == -1)
            {
                return;
            }
            CaterpillarComponentPerNode[internalNeighbour, measurements.TreeOperationsCounter] = -1;
            foreach (TreeNode leaf in internalNeighbour.Neighbours(measurements.TreeOperationsCounter).Where(n => n.Degree(MockCounter) == 1))
            {
                CaterpillarComponentPerNode[leaf, measurements.TreeOperationsCounter] = -1;
            }
        }

        /// <summary>
        /// Update the caterpillar components when an edge between a <see cref="NodeType.I2"/>-node and a <see cref="NodeType.I3"/>-node is contracted.
        /// </summary>
        /// <param name="i2Node">The <see cref="NodeType.I2"/>-node of the contracted edge.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that need to be used.</param>
        private void UpdateCaterpillarComponentsI2I3Node(TreeNode i2Node, PerformanceMeasurements measurements)
        {
            foreach (TreeNode leaf in i2Node.Neighbours(measurements.TreeOperationsCounter).Where(n => n.Degree(MockCounter) == 1))
            {
                CaterpillarComponentPerNode[leaf, measurements.TreeOperationsCounter] = -1;
            }
            CaterpillarComponentPerNode[i2Node, measurements.TreeOperationsCounter] = -1;
        }

        /// <summary>
        /// Update the caterpillar components when an edge between a <see cref="NodeType.I1"/>-node and a <see cref="NodeType.I3"/>-node is contracted.
        /// </summary>
        /// <param name="i1Node">The <see cref="NodeType.I1"/>-node of the contracted edge.</param>
        /// <param name="i3Node">The <see cref="NodeType.I3"/>-node of the contracted edge.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that need to be used.</param>
        private void UpdateCaterpillarComponentsI1I3Node(TreeNode i1Node, TreeNode i3Node, PerformanceMeasurements measurements)
        {
            IEnumerable<TreeNode> internalNeighbours = i3Node.Neighbours(measurements.TreeOperationsCounter).Where(n => n.Degree(MockCounter) > 1);
            if (internalNeighbours.Count() > 3)
            {
                return;
            }
            int oldValue = CaterpillarComponentPerNode[internalNeighbours.First(n => !n.Equals(i1Node)), MockCounter];
            int newValue = CaterpillarComponentPerNode[internalNeighbours.Last(n => !n.Equals(i1Node)), MockCounter];
            List<TreeNode> keysToBeModified = new List<TreeNode>();
            foreach (KeyValuePair<TreeNode, int> kv in CaterpillarComponentPerNode.GetCountedEnumerable(measurements.TreeOperationsCounter))
            {
                if (kv.Value == oldValue)
                {
                    keysToBeModified.Add(kv.Key);
                }
            }
            foreach (TreeNode key in keysToBeModified)
            {
                CaterpillarComponentPerNode[key, MockCounter] = newValue;
            }
        }

        /// <summary>
        /// Update the caterpillar components when an edge between a <see cref="NodeType.I1"/>-node and a <see cref="NodeType.I2"/>-node is contracted.
        /// </summary>
        /// <param name="i2Node">The <see cref="NodeType.I2"/>-node of the contracted edge.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that need to be used.</param>
        private void UpdateCaterpillarComponentsI1I2Node(TreeNode i2Node, PerformanceMeasurements measurements)
        {
            foreach (TreeNode leaf in i2Node.Neighbours(measurements.TreeOperationsCounter).Where(n => n.Degree(MockCounter) == 1))
            {
                CaterpillarComponentPerNode[leaf, measurements.TreeOperationsCounter] = -1;
            }
            CaterpillarComponentPerNode[i2Node, measurements.TreeOperationsCounter] = -1;
        }

        /// <summary>
        /// Fills <see cref="DemandPairsPerEdge"/> and <see cref="DemandPairsPerNode"/>.
        /// </summary>
        private void FillDemandPathsPerEdge()
        {
            DemandPairsPerEdge = new CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>>();
            DemandPairsPerNode = new CountedDictionary<TreeNode, CountedCollection<DemandPair>>();

            // For each demand pair in the instance...
            foreach (DemandPair demandPair in DemandPairs.GetCountedEnumerable(AlgorithmPerformanceMeasurements.DemandPairsOperationsCounter))
            {
                if (!DemandPairsPerNode.ContainsKey(demandPair.Node1, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter))
                {
                    DemandPairsPerNode[demandPair.Node1, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
                }
                if (!DemandPairsPerNode.ContainsKey(demandPair.Node2, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter))
                {
                    DemandPairsPerNode[demandPair.Node2, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
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
                        DemandPairsPerEdge[usedEdge, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
                    }
                    DemandPairsPerEdge[usedEdge, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeValuesCounter);
                }
            }
        }

        /// <summary>
        /// The part of <see cref="Run"/> that is executed for a <see cref="ReductionRule"/> after an edge was contracted in the last iteration.
        /// </summary>
        /// <param name="i">The index of the <see cref="ReductionRule"/> that needs to be executed.</param>
        /// <returns>A <see cref="bool"/> whether the application of the <paramref name="i"/>th rule was successful.</returns>
        private bool RunAfterEdgeContraction(int i)
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying rule {i + 1} after an edge was contracted.");
#endif
            CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)> oldLastContractedEdges = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>(LastContractedEdges.GetCountedEnumerable(MockCounter), MockCounter);
            LastContractedEdges = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>();
            LastIterationEdgeContraction = false;
            bool successful = ReductionRules[i].AfterEdgeContraction(oldLastContractedEdges);
            if (!successful)
            {
#if VERBOSEDEBUG
                Console.WriteLine($"The application of rule {i + 1} was not successful...");
#endif
                LastIterationEdgeContraction = true;
                LastContractedEdges = oldLastContractedEdges;
            }
            return successful;
        }

        /// <summary>
        /// The part of <see cref="Run"/> that is executed for a <see cref="ReductionRule"/> after a <see cref="DemandPair"/> was removed in the last iteration.
        /// </summary>
        /// <param name="i">The index of the <see cref="ReductionRule"/> that needs to be executed.</param>
        /// <returns>A <see cref="bool"/> whether the application of the <paramref name="i"/>th rule was successful.</returns>
        private bool RunAfterDemandPairRemoved(int i)
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying rule {i + 1} after a demand pair was removed.");
#endif
            CountedList<DemandPair> oldLastRemovedDemandPairs = new CountedList<DemandPair>(LastRemovedDemandPairs.GetCountedEnumerable(MockCounter), MockCounter);
            LastRemovedDemandPairs = new CountedList<DemandPair>();
            LastIterationDemandPairRemoval = false;
            bool successful = ReductionRules[i].AfterDemandPathRemove(oldLastRemovedDemandPairs);
            if (!successful)
            {
#if VERBOSEDEBUG
                Console.WriteLine($"The application of rule {i + 1} was not successful...");
#endif
                LastIterationDemandPairRemoval = true;
                LastRemovedDemandPairs = oldLastRemovedDemandPairs;
            }
            return successful;
        }

        /// <summary>
        /// The part of <see cref="Run"/> that is executed for a <see cref="ReductionRule"/> after a <see cref="DemandPair"/> changed in the last iteration.
        /// </summary>
        /// <param name="i">The index of the <see cref="ReductionRule"/> that needs to be executed.</param>
        /// <returns>A <see cref="bool"/> whether the application of the <paramref name="i"/>th rule was successful.</returns>
        private bool RunAfterDemandPairChanged(int i)
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying rule {i + 1} after a demand pair changed.");
#endif
            CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)> oldLastChangedEdgesPerDemandPair = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>(LastChangedEdgesPerDemandPair.GetCountedEnumerable(MockCounter), MockCounter);
            LastChangedEdgesPerDemandPair = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>();
            LastIterationDemandPairChange = false;
            bool successful = ReductionRules[i].AfterDemandPathChanged(oldLastChangedEdgesPerDemandPair);
            if (!successful)
            {
#if VERBOSEDEBUG
                Console.WriteLine($"The application of rule {i + 1} was not successful...");
#endif
                LastIterationDemandPairChange = true;
                LastChangedEdgesPerDemandPair = oldLastChangedEdgesPerDemandPair;
            }
            return successful;
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="TreeNode"/>s in the instance when an edge is contracted.
        /// </summary>
        /// <param name="contractedEdge">The tuple of two <see cref="TreeNode"/>s that represents the edge that is being contracted.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the edge contraction.</param>
        /// <exception cref="ArgumentNullException">Thrown when either element of <paramref name="contractedEdge"/> or <paramref name="newNode"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="TreeNode.Type"/> of either endpoint of <paramref name="contractedEdge"/> is <see cref="NodeType.Other"/>. In that case, please update the types of the nodes by calling <seealso cref="Tree{N}.UpdateNodeTypes()"/>.</exception>
        protected void UpdateNodeTypesDuringEdgeContraction((TreeNode, TreeNode) contractedEdge, TreeNode newNode)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(contractedEdge.Item1, nameof(contractedEdge.Item1), "Trying to update the nodetypes of the nodes that are the endpoints of an edge that is being contracted, but the first endpoint of the edge is null!");
            Utilities.Utils.NullCheck(contractedEdge.Item2, nameof(contractedEdge.Item2), "Trying to update the nodetypes of the nodes that are the endpoints of an edge that is being contracted, but the second endpoint of the edge is null!");
            Utilities.Utils.NullCheck(newNode, nameof(newNode), "Trying to update the nodetypes of the nodes that are the endpoints of an edge that is being contracted, but the node that is the result of the edge contraction is null!");
            if (contractedEdge.Item1.Type == NodeType.Other)
            {
                throw new NotSupportedException($"Trying to update the nodetypes of the nodes that are the endpoints of the edge {contractedEdge} that is begin contracted, but the type of {contractedEdge.Item1} is not known. Please make sure every node has a nodetype to start with!");
            }
            if (contractedEdge.Item2.Type == NodeType.Other)
            {
                throw new NotSupportedException($"Trying to update the nodetypes of the nodes that are the endpoints of the edge {contractedEdge} that is begin contracted, but the type of {contractedEdge.Item2} is not known. Please make sure every node has a nodetype to start with!");
            }
#endif
            switch (contractedEdge.Item1.Type, contractedEdge.Item2.Type)
            {
                // In case of a contraction of an edge between two I1-nodes, two I2-nodes, or two I3-nodes, no changes have to be made.
                // We also do not need to change anything when contracting an edge between an L2-leaf and an I2-node, or between an L3-leaf and an I3-node.
                case (NodeType.I1, NodeType.I2):
                    newNode.Type = NodeType.I1;
                    ChangeLeavesFromNodeToType(contractedEdge.Item2, NodeType.L2, NodeType.I1);
                    break;
                case (NodeType.I2, NodeType.I1):
                    newNode.Type = NodeType.I1;
                    ChangeLeavesFromNodeToType(contractedEdge.Item1, NodeType.L2, NodeType.I1);
                    break;
                case (NodeType.I1, NodeType.I3):
                    UpdateNodeTypesEdgeContractionI1I3(contractedEdge.Item1, contractedEdge.Item2, newNode);
                    break;
                case (NodeType.I3, NodeType.I1):
                    UpdateNodeTypesEdgeContractionI1I3(contractedEdge.Item2, contractedEdge.Item1, newNode);
                    break;
                case (NodeType.I2, NodeType.I3):
                    newNode.Type = NodeType.I3;
                    ChangeLeavesFromNodeToType(contractedEdge.Item1, NodeType.L2, NodeType.L3);
                    break;
                case (NodeType.I3, NodeType.I2):
                    newNode.Type = NodeType.I3;
                    ChangeLeavesFromNodeToType(contractedEdge.Item2, NodeType.L2, NodeType.L3);
                    break;
                case (NodeType.L1, NodeType.I1):
                    UpdateNodeTypesEdgeContractionL1I1(contractedEdge.Item2, newNode);
                    break;
                case (NodeType.I1, NodeType.L1):
                    UpdateNodeTypesEdgeContractionL1I1(contractedEdge.Item1, newNode);
                    break;
            }
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="TreeNode"/>s in the instance when an edge between an <see cref="NodeType.I1"/>-node and an <see cref="NodeType.I3"/>-node is contracted.
        /// </summary>
        /// <param name="contractedEdgeI1Node">The <see cref="NodeType.I1"/>-node that is the endpoint of the contracted edge.</param>
        /// <param name="contractedEdgeI3Node">The <see cref="NodeType.I3"/>-node that is the endpoint of the contracted edge.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the edge contraction.</param>
        private void UpdateNodeTypesEdgeContractionI1I3(TreeNode contractedEdgeI1Node, TreeNode contractedEdgeI3Node, TreeNode newNode)
        {
            // If the I3-node has exactly three internal neighbours, the result of this edge contraction will be an I2-node.
            // If it has more than three internal neighbours, the result will be an I3-node.
            bool hasAtLeastFourInternalNeighbours = contractedEdgeI3Node.Neighbours(MockCounter).Count(n => n.Neighbours(MockCounter).Count() > 1) > 3;
            NodeType contractedType;
            NodeType leafType;

            if (hasAtLeastFourInternalNeighbours)
            {
                contractedType = NodeType.I3;
                leafType = NodeType.L3;
            }
            else
            {
                contractedType = NodeType.I2;
                leafType = NodeType.L2;
                ChangeLeavesFromNodeToType(contractedEdgeI3Node, NodeType.L3, leafType);
            }

            newNode.Type = contractedType;
            ChangeLeavesFromNodeToType(contractedEdgeI1Node, NodeType.L1, leafType);
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of the <see cref="TreeNode"/>s in the instance when an edge between an <see cref="NodeType.I1"/>-node and an <see cref="NodeType.L1"/>-leaf is contracted.
        /// </summary>
        /// <param name="contractedEdgeI1Node">The <see cref="NodeType.I1"/>-node that is the endpoint of the contracted edge.</param>
        /// <param name="newNode">The <see cref="TreeNode"/> that is the result of the edge contraction.</param>
        private void UpdateNodeTypesEdgeContractionL1I1(TreeNode contractedEdgeI1Node, TreeNode newNode)
        {
            // If the I1-node has exactly one leaf (which is also the edge that is contracted), the resulting node will be a leaf. 
            // The type of this leaf depends on the type of the (unique) internal neighbour of the I1-node.
            bool hasExactlyOneLeaf = contractedEdgeI1Node.Neighbours(MockCounter).Count(n => n.Neighbours(MockCounter).Count() == 1) == 1;
            if (hasExactlyOneLeaf)
            {
                TreeNode internalNeighbour = contractedEdgeI1Node.Neighbours(MockCounter).FirstOrDefault(n => n.Neighbours(MockCounter).Count() > 1);
                if (internalNeighbour is default(TreeNode))
                {
                    newNode.Type = NodeType.I1;
                }
                else if (internalNeighbour.Type == NodeType.I1)
                {
                    newNode.Type = NodeType.L1;
                }
                else if (internalNeighbour.Type == NodeType.I2)
                {
                    newNode.Type = NodeType.L2;
                }
                else
                {
                    newNode.Type = NodeType.L3;
                }
            }
        }

        /// <summary>
        /// Update the <see cref="NodeType"/>s of all the leaves of <paramref name="parentNode"/>.
        /// </summary>
        /// <param name="parentNode">The <see cref="TreeNode"/> for which we want to change the <see cref="NodeType"/> of its leaves.</param>
        /// <param name="oldType">The old <see cref="NodeType"/> of the leaves of <paramref name="parentNode"/>. Is equal to Lx, where Ix is the <see cref="TreeNode.Type"/> of <paramref name="parentNode"/>.</param>
        /// <param name="newType">The new <see cref="NodeType"/> of the leaves of <paramref name="parentNode"/>.</param>
        private void ChangeLeavesFromNodeToType(TreeNode parentNode, NodeType oldType, NodeType newType)
        {
            foreach (TreeNode leaf in parentNode.Neighbours(MockCounter).Where(n => n.Type == oldType))
            {
                leaf.Type = newType;
            }
        }
        
        /// <summary>
        /// Fills <see cref="ReductionRules"/> with the <see cref="ReductionRule"/>s used by this algorithm.
        /// </summary>
        protected abstract void CreateReductionRules();
    }
}
