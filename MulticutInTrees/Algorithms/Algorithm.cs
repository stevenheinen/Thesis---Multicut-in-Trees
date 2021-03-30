// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Experiments;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.ReductionRules;

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
            Utilities.Utils.NullCheck(instance, nameof(instance), "Trying to create an instance of a Multicut algorithm, but the problem instance is null!");
#endif
            Instance = instance;
            Tree = instance.Tree;
            DemandPairs = instance.DemandPairs;
            K = instance.K;
            PartialSolution = new List<(TreeNode, TreeNode)>();
            MockCounter = new Counter();
            AlgorithmType = algorithmType;
            AlgorithmPerformanceMeasurements = new PerformanceMeasurements(algorithmType.ToString());

            LastContractedEdges = new CountedList<((TreeNode, TreeNode), TreeNode, CountedCollection<DemandPair>)>();
            LastRemovedDemandPairs = new CountedList<DemandPair>();
            LastChangedEdgesPerDemandPair = new CountedList<(CountedList<(TreeNode, TreeNode)>, DemandPair)>();
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
            return (Tree, PartialSolution, DemandPairs.GetInternalList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, true, Tree.NumberOfNodes(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));

            returnfalse:
            return (Tree, PartialSolution, DemandPairs.GetInternalList(), new ExperimentOutput(Instance.NumberOfNodes, Instance.NumberOfDemandPairs, Instance.TreeType, Instance.DPType, AlgorithmType, Instance.RandomSeed, false, Tree.NumberOfNodes(MockCounter), AlgorithmPerformanceMeasurements, ReductionRules.Select(r => r.Measurements).ToList().AsReadOnly()));
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
        /// Computes all information needed prior to the first iteration of the algorithm.
        /// </summary>
        protected abstract void Preprocess();

        /// <summary>
        /// Add this edge to the solution, remove all <see cref="DemandPair"/>s that go over this edge, and contract it.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s representing the edge to be cut.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        internal abstract void CutEdge((TreeNode, TreeNode) edge, PerformanceMeasurements measurements);

        /// <summary>
        /// Add multiple edges to the solution, remove all <see cref="DemandPair"/>s that go over these edges, and contract them.
        /// </summary>
        /// <param name="edges">The <see cref="IList{T}"/> of tuples of <see cref="TreeNode"/>s that represent the edges to be cut.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        internal abstract void CutEdges(CountedList<(TreeNode, TreeNode)> edges, PerformanceMeasurements measurements);

        /// <summary>
        /// Contract an edge.
        /// </summary>
        /// <param name="edge">The tuple of <see cref="TreeNode"/>s representing the edge to be contracted.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        internal abstract void ContractEdge((TreeNode, TreeNode) edge, PerformanceMeasurements measurements);

        /// <summary>
        /// Contract multiple edges.
        /// </summary>
        /// <param name="edges">The <see cref="IList{T}"/> of tuples of <see cref="TreeNode"/>s representing the edges to be contracted.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        internal abstract void ContractEdges(CountedList<(TreeNode, TreeNode)> edges, PerformanceMeasurements measurements);

        /// <summary>
        /// Remove a <see cref="DemandPair"/> from the problem instance.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> to be removed.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        internal abstract void RemoveDemandPair(DemandPair demandPair, PerformanceMeasurements measurements);

        /// <summary>
        /// Remove multiple <see cref="DemandPair"/>s from the problem instance.
        /// </summary>
        /// <param name="demandPairs">The <see cref="IList{T}"/> of <see cref="DemandPair"/>s to be removed.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        internal abstract void RemoveDemandPairs(CountedList<DemandPair> demandPairs, PerformanceMeasurements measurements);

        /// <summary>
        /// Change an endpoint of a <see cref="DemandPair"/>.
        /// </summary>
        /// <param name="demandPair">The <see cref="DemandPair"/> whose endpoint changes.</param>
        /// <param name="oldEndpoint">The old endpoint of <paramref name="demandPair"/>.</param>
        /// <param name="newEndpoint">The new endpoint of <paramref name="demandPair"/>.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        internal abstract void ChangeEndpointOfDemandPair(DemandPair demandPair, TreeNode oldEndpoint, TreeNode newEndpoint, PerformanceMeasurements measurements);

        /// <summary>
        /// Change an endpoint of multiple <see cref="DemandPair"/>s.
        /// </summary>
        /// <param name="demandPairEndpointTuples">The <see cref="IList{T}"/> of tuples containing the <see cref="DemandPair"/> that is changed, the <see cref="TreeNode"/> old endpoint and <see cref="TreeNode"/> new endpoint.</param>
        /// <param name="measurements">The set of <see cref="PerformanceMeasurements"/> that should be used to count the operations needed during this modification.</param>
        internal abstract void ChangeEndpointOfDemandPairs(CountedList<(DemandPair, TreeNode, TreeNode)> demandPairEndpointTuples, PerformanceMeasurements measurements);
    }
}
