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
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s per edge.
        /// </summary>
        protected CountedDictionary<Edge<Node>, CountedCollection<DemandPair>> DemandPairsPerEdge { get; private set; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> containing a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/> per <see cref="Node"/>.
        /// </summary>
        protected CountedDictionary<Node, CountedCollection<DemandPair>> DemandPairsPerNode { get; private set; }

        /// <summary>
        /// <see cref="CountedDictionary{TKey, TValue}"/> with the identifier of the caterpillar component each <see cref="Node"/> is part of, or -1 if it is not part of any caterpillar component.
        /// </summary>
        protected CountedDictionary<Node, int> CaterpillarComponentPerNode { get; }

        /// <summary>
        /// The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s in the input.
        /// </summary>
        protected CountedCollection<DemandPair> DemandPairs { get; }

        /// <summary>
        /// The input <see cref="Graph"/>.
        /// </summary>
        protected Graph Tree { get; }

        /// <summary>
        /// The <see cref="List{T}"/> of tuples of <see cref="Node"/>s that contains the already found part of the solution.
        /// </summary>
        protected List<Edge<Node>> PartialSolution { get; }

        /// <summary>
        /// The size the cutset is allowed to be.
        /// </summary>
        protected int K { get; }
        
        /// <summary>
        /// The <see cref="MulticutInstance"/> this <see cref="Algorithm"/> is trying to solve.
        /// </summary>
        private MulticutInstance Instance { get; }

        /// <summary>
        /// <see cref="PerformanceMeasurements"/> that are used by the central parts an <see cref="Algorithm"/> does that has nothing to do with <see cref="ReductionRule"/>s, like preprocessing.
        /// </summary>
        private PerformanceMeasurements AlgorithmPerformanceMeasurements { get; }

        /// <summary>
        /// <see cref="Counter"/> that can be used for operations that should <b>NOT</b> be counted for the performance of this <see cref="Algorithm"/>.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// The <see cref="AlgorithmType"/> of this <see cref="Algorithm"/>.
        /// </summary>
        private AlgorithmType AlgorithmType { get; }

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
            PartialSolution = new List<Edge<Node>>();
            MockCounter = new Counter();
            AlgorithmType = algorithmType;
            AlgorithmPerformanceMeasurements = new PerformanceMeasurements(algorithmType.ToString());
            CaterpillarComponentPerNode = new CountedDictionary<Node, int>();

            FillDemandPathsPerEdge();
            CreateReductionRules();
        }

        /// <summary>
        /// Runs the algorithm by using the <see cref="ReductionRule.RunFirstIteration"/> method exclusively. This means unnecessary parts of the graph are checked, but it results in smaller kernels. This part is still WIP. We would like to find a way to skip checking unnecessary parts, but still find the smallest kernel.
        /// </summary>
        /// <returns>A tuple with the <see cref="Graph"/> that is left after kernelisation, a <see cref="List{T}"/> with tuples of two <see cref="Node"/>s representing the edges that are part of the solution, and a <see cref="List{T}"/> of <see cref="DemandPair"/>s that are not yet separated.</returns>
        public (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) RunNaively()
        {
            for (int i = 0; i < ReductionRules.Count; i++)
            {
                if (DemandPairs.Count(AlgorithmPerformanceMeasurements.DemandPairsOperationsCounter) == 0)
                {
                    goto returntrue;
                }
                if (PartialSolution.Count >= K)
                {
                    goto returnfalse;
                }

#if VERBOSEDEBUG
                Console.WriteLine($"Now applying rule {i + 1} ({ReductionRules[i].GetType().Name}).");
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
                    Console.WriteLine($"The application of rule {i + 1} ({ReductionRules[i].GetType().Name}) was not successful...");
                }
#endif
            }

            returntrue:
            return (Tree, PartialSolution, DemandPairs.GetLinkedList().ToList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, Instance.K, Instance.OptimalK, true, Tree.NumberOfNodes(MockCounter), DemandPairs.Count(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));

            returnfalse:
            return (Tree, PartialSolution, DemandPairs.GetLinkedList().ToList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, Instance.K, Instance.OptimalK, false, Tree.NumberOfNodes(MockCounter), DemandPairs.Count(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));
        }

        /// <summary>
        /// Try to solve the instance.
        /// </summary>
        /// <returns>A tuple with the <see cref="Graph"/> that is left after kernelisation, a <see cref="List{T}"/> with tuples of two <see cref="Node"/>s representing the edges that are part of the solution, and a <see cref="List{T}"/> of <see cref="DemandPair"/>s that are not yet separated.</returns>
        public (Graph, List<Edge<Node>>, List<DemandPair>, ExperimentOutput) Run()
        {
            bool[] appliedReductionRule = new bool[ReductionRules.Count];

            for (int i = 0; i < ReductionRules.Count; i++)
            {
                if (DemandPairs.Count(AlgorithmPerformanceMeasurements.DemandPairsOperationsCounter) == 0)
                {
                    goto returntrue;
                }
                if (PartialSolution.Count >= K)
                {
                    goto returnfalse;
                }

#if VERBOSEDEBUG
                Console.WriteLine($"Now applying rule {i + 1} ({ReductionRules[i].GetType().Name}).");
#endif
                // If we have not executed this rule before, execute it now.
                if (!appliedReductionRule[i])
                {
                    appliedReductionRule[i] = true;
                    if (ReductionRules[i].RunFirstIteration())
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
                        Console.WriteLine($"The application of rule {i + 1} ({ReductionRules[i].GetType().Name}) was not successful...");
                    }
#endif
                    continue;
                }

                // If we applied the rule successfully, go back to rule 0.
                if (ReductionRules[i].RunLaterIteration())
                {
                    if (ReductionRules[i].TrueMeansInfeasibleInstance)
                    {
                        goto returnfalse;
                    }

                    i = -1;
                }
#if VERBOSEDEBUG
                else
                {
                    Console.WriteLine($"The application of rule {i + 1} ({ReductionRules[i].GetType().Name}) was not successful...");
                }
#endif
            }

            returntrue:
            return (Tree, PartialSolution, DemandPairs.GetLinkedList().ToList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, Instance.K, Instance.OptimalK, true, Tree.NumberOfNodes(MockCounter), DemandPairs.Count(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));

            returnfalse:
            return (Tree, PartialSolution, DemandPairs.GetLinkedList().ToList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, Instance.K, Instance.OptimalK, false, Tree.NumberOfNodes(MockCounter), DemandPairs.Count(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));
        }

        /// <summary>
        /// Contract an edge.
        /// </summary>
        /// <param name="edge">The <see cref="Edge{TNode}"/> to be contracted.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <returns>The <see cref="Node"/> that is the resulting node of the edge contraction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="edge"/> is not part of the input.</exception>
        /// <exception cref="InvalidEdgeException">Thrown when <paramref name="edge"/> is a self-loop.</exception>
        protected internal Node ContractEdge(Edge<Node> edge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to contract an edge, but the edge is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to contract an edge, but performance measures to be used are null!");
            if (!Tree.HasEdge(edge, MockCounter))
            {
                throw new NotInGraphException($"Trying to contract edge {edge}, but this edge is not part of the tree!");
            }
            if (edge.Endpoint1 == edge.Endpoint2)
            {
                throw new InvalidEdgeException($"Trying to contract edge {edge}, but this edge is a self loop and should not exist!");
            }
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Contracting edge {edge}!");
#endif
            UpdateCaterpillarComponents(edge, measurements);
            Node newNode = Tree.ContractEdge(edge, measurements.TreeOperationsCounter);
            Node toBeDeletedNode = edge.Endpoint1 == newNode ? edge.Endpoint2 : edge.Endpoint1;
            CountedCollection<DemandPair> pairsOnEdge = RemoveDemandPairsFromContractedEdge(edge, newNode, measurements);
            UpdateDemandPairsStartingAtContractedEdge(edge, toBeDeletedNode, newNode, pairsOnEdge, measurements);

            //UpdateDemandPairsGoingThroughChild(toBeDeletedNode, newNode, measurements);

            // Tell the reduction rules what information in the input has been modified.
            for (int i = 0; i < ReductionRules.Count; i++)
            {
                // If this reduction rule has not run yet, do not update its information. The following ones will also not have run.
                if (!ReductionRules[i].HasRun)
                {
                    break;
                }
                ReductionRules[i].LastContractedEdges.Add((edge, newNode, pairsOnEdge), measurements.TreeOperationsCounter);
            }

            measurements.NumberOfContractedEdgesCounter++;

            return newNode;
        }

        /// <summary>
        /// Remove a <see cref="DemandPair"/> from the problem instance.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> to be removed.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPair"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        protected internal void RemoveDemandPair(DemandPair demandPair, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to remove a demand pair, but the demand pair is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove a demand pair, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Removing demand pair {demandPair}.");
#endif
            // Tell the reduction rules what information in the input has been modified.
            for (int i = 0; i < ReductionRules.Count; i++)
            {
                // If this reduction rule has not run yet, do not update its information. The following ones will also not have run.
                if (!ReductionRules[i].HasRun)
                {
                    break;
                }
                ReductionRules[i].LastRemovedDemandPairs.Add(demandPair, measurements.DemandPairsOperationsCounter);
            }
            measurements.NumberOfRemovedDemandPairsCounter++;

            RemoveDemandPairFromNode(demandPair.Node1, demandPair, measurements);
            RemoveDemandPairFromNode(demandPair.Node2, demandPair, measurements);

            // Remove this demand pair from each edge it is on.
            foreach (Edge<Node> edge in demandPair.EdgesOnDemandPath(measurements.TreeOperationsCounter))
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
        protected internal void ChangeEndpointOfDemandPair(DemandPair demandPair, Node oldEndpoint, Node newEndpoint, PerformanceMeasurements measurements)
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
            CountedList<Edge<Node>> oldEdges = new(demandPair.ChangeEndpoint(oldEndpoint, newEndpoint, measurements.DemandPairsOperationsCounter), measurements.TreeOperationsCounter);

            RemoveDemandPairFromNode(oldEndpoint, demandPair, measurements);
            if (!DemandPairsPerNode.ContainsKey(newEndpoint, measurements.DemandPairsPerEdgeKeysCounter))
            {
                DemandPairsPerNode[newEndpoint, measurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
            }
            DemandPairsPerNode[newEndpoint, measurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, measurements.DemandPairsOperationsCounter);

            // Tell the reduction rules what information in the input has been modified.
            for (int i = 0; i < ReductionRules.Count; i++)
            {
                // If this reduction rule has not run yet, do not update its information. The following ones will also not have run.
                if (!ReductionRules[i].HasRun)
                {
                    break;
                }
                ReductionRules[i].LastChangedDemandPairs.Add((oldEdges, demandPair), measurements.DemandPairsOperationsCounter);
            }
            measurements.NumberOfChangedDemandPairsCounter++;

            foreach (Edge<Node> edge in oldEdges.GetCountedEnumerable(measurements.TreeOperationsCounter))
            {
                RemoveDemandPairFromEdge(edge, demandPair, measurements);
            }
        }

        /// <summary>
        /// Add this edge to the solution, remove all <see cref="DemandPair"/>s that go over this edge, and contract it.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="Node"/>s representing the edge to be cut.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <returns>The <see cref="Node"/> that is the resulting node of the edge contraction.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotInGraphException">Thrown when <paramref name="edge"/> is not part of the tree.</exception>
        /// <exception cref="InvalidEdgeException">Thrown when <paramref name="edge"/> is not a valid edge.</exception>
        protected internal Node CutEdge(Edge<Node> edge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to cut an edge, but the edge is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to cut an edge, but the performance measures to be used are null!");
            if (!Tree.HasEdge(edge, MockCounter))
            {
                throw new NotInGraphException($"Trying to cut edge {edge}, but this edge is not part of the tree!");
            }
            if (edge.Endpoint1 == edge.Endpoint2)
            {
                throw new InvalidEdgeException($"Trying to cut edge {edge}, but this edge is a self loop and should not exist!");
            }
#endif
            PartialSolution.Add(edge);
            CountedList<DemandPair> separatedDemandPairs = new(DemandPairsPerEdge[edge, measurements.DemandPairsPerEdgeKeysCounter].GetCountedEnumerable(MockCounter), MockCounter);

            RemoveDemandPairs(separatedDemandPairs, measurements);
            Node res = ContractEdge(edge, measurements);

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
        /// <param name="demandPairEndpointTuples">The <see cref="IList{T}"/> of tuples containing the <see cref="DemandPair"/> that is changed, the <see cref="Node"/> old endpoint and <see cref="Node"/> new endpoint.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="demandPairEndpointTuples"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal void ChangeEndpointOfDemandPairs(CountedList<(DemandPair, Node, Node)> demandPairEndpointTuples, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(demandPairEndpointTuples, nameof(demandPairEndpointTuples), "Trying to change the endpoints of multple demand pairs, but the IEnumerable with tuples with required information is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to change the endpoints of multple demand pairs, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Changing endpoint of multiple demand pairs: {demandPairEndpointTuples.GetInternalList().Print()}.");
#endif
            foreach ((DemandPair, Node, Node) change in demandPairEndpointTuples.GetCountedEnumerable(measurements.DemandPairsOperationsCounter))
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
        /// <param name="edges">The <see cref="IList{T}"/> of <see cref="Edge{TNode}"/>s to be contracted.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal void ContractEdges(CountedList<Edge<Node>> edges, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), "Trying to contract multiple edges, but the IEnumerable of edges is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to contract multiple edges, but the performance measures to be used are null!");
#endif
#if VERBOSEDEBUG
            Console.WriteLine($"Contracting edges {edges.GetInternalList().Print()}");
#endif
            int nrEdges = edges.Count(measurements.TreeOperationsCounter);
            foreach (Edge<Node> edge in edges.GetCountedEnumerable(measurements.TreeOperationsCounter))
            {
                ContractEdge(edge, measurements);
            }
        }

        /// <summary>
        /// Add multiple edges to the solution, remove all <see cref="DemandPair"/>s that go over these edges, and contract them.
        /// </summary>
        /// <param name="edges">The <see cref="IList{T}"/> of <see cref="Edge{TNode}"/>s to be cut.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        internal void CutEdges(CountedList<Edge<Node>> edges, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edges, nameof(edges), "Trying to cut multiple edges, but the IEnumerable of edges is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to cut multiple edges, but the performance measures to be used are null!");
#endif
            int nrEdges = edges.Count(measurements.TreeOperationsCounter);
            foreach (Edge<Node> edge in edges.GetCountedEnumerable(measurements.TreeOperationsCounter))
            {
                CutEdge(edge, measurements);
            }
        }

        /// <summary>
        /// Removes a <see cref="DemandPair"/> from a given edge in <see cref="DemandPairsPerEdge"/>.
        /// </summary>
        /// <param name="edge">The edge from which <paramref name="demandPair"/> should be removed.</param>
        /// <param name="demandPair">The <see cref="DemandPair"/> that should be removed from <paramref name="edge"/>.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/>, <paramref name="demandPair"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private void RemoveDemandPairFromEdge(Edge<Node> edge, DemandPair demandPair, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to remove a demand pair from an edge, but the edge is null!");
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to remove a demand pair from an edge, but the demand pair is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove a demand pair from an edge, but the performance measurements are null!");
#endif
            DemandPairsPerEdge[edge, measurements.DemandPairsPerEdgeKeysCounter].Remove(demandPair, measurements.DemandPairsPerEdgeValuesCounter);

            // If, after removing this demand pair, there are no more demand pairs going over this edge, remove it from the dictionary.
            if (DemandPairsPerEdge[edge, measurements.DemandPairsPerEdgeKeysCounter].Count(measurements.DemandPairsPerEdgeValuesCounter) == 0)
            {
                DemandPairsPerEdge.Remove(edge, measurements.DemandPairsPerEdgeKeysCounter);
            }
        }

        /// <summary>
        /// Removes a <see cref="DemandPair"/> from a given edge in <see cref="DemandPairsPerNode"/>.
        /// </summary>
        /// <param name="node">The node from which <paramref name="demandPair"/> should be removed.</param>
        /// <param name="demandPair">The <see cref="DemandPair"/> that should be removed from <paramref name="node"/>.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used for this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/>, <paramref name="demandPair"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private void RemoveDemandPairFromNode(Node node, DemandPair demandPair, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(node, nameof(node), "Trying to remove a demand pair from a node, but the node is null!");
            Utils.NullCheck(demandPair, nameof(demandPair), "Trying to remove a demand pair from a node, but the demand pair is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to remove a demand pair from a node, but the performance measurements are null!");
#endif
            DemandPairsPerNode[node, measurements.DemandPairsPerEdgeKeysCounter].Remove(demandPair, measurements.DemandPairsPerEdgeValuesCounter);

            // If, after removing this demand pair, there are no more demand pairs going over this edge, remove it from the dictionary.
            if (DemandPairsPerNode[node, measurements.DemandPairsPerEdgeKeysCounter].Count(measurements.DemandPairsPerEdgeValuesCounter) == 0)
            {
                DemandPairsPerNode.Remove(node, measurements.DemandPairsPerEdgeKeysCounter);
            }
        }

        /// <summary>
        /// Removes all <see cref="DemandPair"/>s from the edge that will be contracted.
        /// </summary>
        /// <param name="edge">The <see cref="Edge{TNode}"/> that is being contracted.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the contraction.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used during this modification.</param>
        /// <returns>A <see cref="CountedCollection{T}"/> with all the <see cref="DemandPair"/>s that pass through <paramref name="edge"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/>, <paramref name="newNode"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private CountedCollection<DemandPair> RemoveDemandPairsFromContractedEdge(Edge<Node> edge, Node newNode, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to remove all demand paths going through an edge, but the edge is null!");
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
        /// Update all <see cref="DemandPair"/>s that start at an edge that will be contracted.
        /// </summary>
        /// <param name="edge">The <see cref="Edge{TNode}"/> that is being contracted.</param>
        /// <param name="child">The <see cref="Node"/> that will be removed by the contraction.</param>
        /// <param name="newNode">The <see cref="Node"/> that is the result of the contraction.</param>
        /// <param name="pairsOnEdge">The <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s that go over <paramref name="edge"/>.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> to be used during this modification.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edge"/>, <paramref name="child"/>, <paramref name="newNode"/>, <paramref name="pairsOnEdge"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        private void UpdateDemandPairsStartingAtContractedEdge(Edge<Node> edge, Node child, Node newNode, CountedCollection<DemandPair> pairsOnEdge, PerformanceMeasurements measurements)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge, nameof(edge), "Trying to update the demand pairs starting at the endpoints of the edge that will be contracted, but the edge that will be contracted is null!");
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
                    //demandPair.OnEdgeNextToDemandPathEndpointsContracted(edge, newNode, measurements.DemandPairsOperationsCounter);
                    demandPair.UpdateEndpointsAfterEdgeContraction(edge, newNode);
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
        private void UpdateCaterpillarComponents(Edge<Node> contractedEdge, PerformanceMeasurements measurements)
        {
            if (CaterpillarComponentPerNode.Count(MockCounter) == 0)
            {
                return;
            }

            switch (contractedEdge.Endpoint1.Type, contractedEdge.Endpoint2.Type)
            {
                case (NodeType.I1, NodeType.I2):
                    UpdateCaterpillarComponentsI1I2Node(contractedEdge.Endpoint2, measurements);
                    break;
                case (NodeType.I2, NodeType.I1):
                    UpdateCaterpillarComponentsI1I2Node(contractedEdge.Endpoint1, measurements);
                    break;
                case (NodeType.I1, NodeType.I3):
                    UpdateCaterpillarComponentsI1I3Node(contractedEdge.Endpoint1, contractedEdge.Endpoint2, measurements);
                    break;
                case (NodeType.I3, NodeType.I1):
                    UpdateCaterpillarComponentsI1I3Node(contractedEdge.Endpoint2, contractedEdge.Endpoint1, measurements);
                    break;
                case (NodeType.I2, NodeType.I3):
                    UpdateCaterpillarComponentsI2I3Node(contractedEdge.Endpoint1, measurements);
                    break;
                case (NodeType.I3, NodeType.I2):
                    UpdateCaterpillarComponentsI2I3Node(contractedEdge.Endpoint2, measurements);
                    break;
                case (NodeType.L1, NodeType.I1):
                    UpdateCaterpillarComponentsL1I1Node(contractedEdge.Endpoint2, measurements);
                    break;
                case (NodeType.I1, NodeType.L1):
                    UpdateCaterpillarComponentsL1I1Node(contractedEdge.Endpoint1, measurements);
                    break;
            }
        }

        /// <summary>
        /// Update the caterpillar components when an edge between a <see cref="NodeType.L1"/>-leaf and a <see cref="NodeType.I1"/>-node is contracted.
        /// </summary>
        /// <param name="i1Node">The <see cref="NodeType.I1"/>-node of the contracted edge.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that need to be used.</param>
        private void UpdateCaterpillarComponentsL1I1Node(Node i1Node, PerformanceMeasurements measurements)
        {
            if (i1Node.Neighbours(measurements.TreeOperationsCounter).Count() > 2)
            {
                return;
            }
            Node internalNeighbour = i1Node.Neighbours(measurements.TreeOperationsCounter).First(n => n.Degree(MockCounter) > 1);
            int caterpillar = CaterpillarComponentPerNode[internalNeighbour, measurements.TreeOperationsCounter];
            if (caterpillar == -1)
            {
                return;
            }
            CaterpillarComponentPerNode[internalNeighbour, measurements.TreeOperationsCounter] = -1;
            foreach (Node leaf in internalNeighbour.Neighbours(measurements.TreeOperationsCounter).Where(n => n.Degree(MockCounter) == 1))
            {
                CaterpillarComponentPerNode[leaf, measurements.TreeOperationsCounter] = -1;
            }
        }

        /// <summary>
        /// Update the caterpillar components when an edge between a <see cref="NodeType.I2"/>-node and a <see cref="NodeType.I3"/>-node is contracted.
        /// </summary>
        /// <param name="i2Node">The <see cref="NodeType.I2"/>-node of the contracted edge.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that need to be used.</param>
        private void UpdateCaterpillarComponentsI2I3Node(Node i2Node, PerformanceMeasurements measurements)
        {
            foreach (Node leaf in i2Node.Neighbours(measurements.TreeOperationsCounter).Where(n => n.Degree(MockCounter) == 1))
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
        private void UpdateCaterpillarComponentsI1I3Node(Node i1Node, Node i3Node, PerformanceMeasurements measurements)
        {
            IEnumerable<Node> internalNeighbours = i3Node.Neighbours(measurements.TreeOperationsCounter).Where(n => n.Degree(MockCounter) > 1);
            if (internalNeighbours.Count() > 3)
            {
                return;
            }
            int oldValue = CaterpillarComponentPerNode[internalNeighbours.First(n => !n.Equals(i1Node)), MockCounter];
            int newValue = CaterpillarComponentPerNode[internalNeighbours.Last(n => !n.Equals(i1Node)), MockCounter];
            List<Node> keysToBeModified = new();
            foreach (KeyValuePair<Node, int> kv in CaterpillarComponentPerNode.GetCountedEnumerable(measurements.TreeOperationsCounter))
            {
                if (kv.Value == oldValue)
                {
                    keysToBeModified.Add(kv.Key);
                }
            }
            foreach (Node key in keysToBeModified)
            {
                CaterpillarComponentPerNode[key, MockCounter] = newValue;
            }
        }

        /// <summary>
        /// Update the caterpillar components when an edge between a <see cref="NodeType.I1"/>-node and a <see cref="NodeType.I2"/>-node is contracted.
        /// </summary>
        /// <param name="i2Node">The <see cref="NodeType.I2"/>-node of the contracted edge.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that need to be used.</param>
        private void UpdateCaterpillarComponentsI1I2Node(Node i2Node, PerformanceMeasurements measurements)
        {
            foreach (Node leaf in i2Node.Neighbours(measurements.TreeOperationsCounter).Where(n => n.Degree(MockCounter) == 1))
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
            DemandPairsPerEdge = new CountedDictionary<Edge<Node>, CountedCollection<DemandPair>>();
            DemandPairsPerNode = new CountedDictionary<Node, CountedCollection<DemandPair>>();

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
                foreach (Edge<Node> edge in demandPair.EdgesOnDemandPath(AlgorithmPerformanceMeasurements.TreeOperationsCounter))
                {
                    // Add this edge to the DemandPairsPerEdge dictionary.
                    if (!DemandPairsPerEdge.ContainsKey(edge, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter))
                    {
                        DemandPairsPerEdge[edge, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter] = new CountedCollection<DemandPair>();
                    }
                    DemandPairsPerEdge[edge, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeKeysCounter].Add(demandPair, AlgorithmPerformanceMeasurements.DemandPairsPerEdgeValuesCounter);
                }
            }
        }

        /// <summary>
        /// Fills <see cref="ReductionRules"/> with the <see cref="ReductionRule"/>s used by this algorithm.
        /// </summary>
        protected abstract void CreateReductionRules();
    }
}
