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
    /// <see cref="ReductionRule"/> that contracts edges that are not used by any <see cref="DemandPair"/>s.
    /// <br/>
    /// Rule: If there is a tree edge with no demand path passing through it, contract this edge.
    /// </summary>
    public class IdleEdge : ReductionRule
    {
        /// <summary>
        /// A <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.
        /// The value is all the <see cref="DemandPair"/>s whose path passes through the key.
        /// </summary>
        private CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> DemandPairsPerEdge { get; }

        /// <summary>
        /// Constructor for <see cref="IdleEdge"/>.
        /// </summary>
        /// <param name="tree">The input <see cref="Tree{N}"/> of <see cref="TreeNode"/>s in the instance.</param>
        /// <param name="demandPairs">The <see cref="CountedList{T}"/> of <see cref="DemandPair"/>s in the instance.</param>
        /// <param name="algorithm">The <see cref="Algorithm"/> this <see cref="IdleEdge"/> is part of.</param>
        /// <param name="demandPairsPerEdge">The <see cref="CountedDictionary{TKey, TValue}"/> with edges represented by tuples of <see cref="TreeNode"/>s as key and a <see cref="CountedCollection{T}"/> of <see cref="DemandPair"/>s as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="demandPairs"/>, <paramref name="algorithm"/> or <paramref name="demandPairsPerEdge"/> is <see langword="null"/>.</exception>
        public IdleEdge(Tree<TreeNode> tree, CountedList<DemandPair> demandPairs, Algorithm algorithm, CountedDictionary<(TreeNode, TreeNode), CountedCollection<DemandPair>> demandPairsPerEdge) : base(tree, demandPairs, algorithm)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), $"Trying to create an instance of the {GetType().Name} rule, but the input tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), $"Trying to create an instance of the {GetType().Name} rule, but the list of demand pairs is null!");
            Utils.NullCheck(algorithm, nameof(algorithm), $"Trying to create an instance of the {GetType().Name} rule, but the algorithm it is part of is null!");
            Utils.NullCheck(demandPairsPerEdge, nameof(demandPairsPerEdge), $"Trying to create an instance of the {GetType().Name} rule, but the dictionary with demand paths per edge is null!");
#endif
            DemandPairsPerEdge = demandPairsPerEdge;
        }

        /// <summary>
        /// Checks whether an edge can be contracted. This is the case when no demand pairs use this edge.
        /// </summary>
        /// <param name="edge">The tuple of two <see cref="TreeNode"/>s for which we want to know if it can be contracted.</param>
        /// <returns><see langword="true"/> if we can contract <paramref name="edge"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <see cref="TreeNode"/> of <paramref name="edge"/> is <see langword="null"/>.</exception>
        private bool CanEdgeBeContracted((TreeNode, TreeNode) edge)
        {
#if !EXPERIMENT
            Utils.NullCheck(edge.Item1, nameof(edge.Item1), "Trying to see if an edge can be contracted, but the first endpoint of the edge is null!");
            Utils.NullCheck(edge.Item2, nameof(edge.Item2), "Trying to see if an edge can be contracted, but the second endpoint of the edge is null!");
#endif
            (TreeNode, TreeNode) usedEdge = Utils.OrderEdgeSmallToLarge(edge);

            // If this edge is not in the dictionary with demand paths per edge, or it is, but there are no demand paths going through this edge, we can contract this edge.
            if (Tree.HasNode(edge.Item1, Measurements.TreeOperationsCounter) && Tree.HasNode(edge.Item2, Measurements.TreeOperationsCounter) && Tree.HasEdge(edge, Measurements.TreeOperationsCounter) && (!DemandPairsPerEdge.ContainsKey(usedEdge, Measurements.DemandPairsPerEdgeKeysCounter) || DemandPairsPerEdge[usedEdge, Measurements.DemandPairsPerEdgeKeysCounter].Count(Measurements.DemandPairsPerEdgeValuesCounter) == 0))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to apply this <see cref="ReductionRule"/> on the edges in <paramref name="edgesToCheck"/>.
        /// </summary>
        /// <param name="edgesToCheck">The <see cref="IEnumerable{T}"/> with edges we want to check.</param>
        /// <returns><see langword="true"/> if we were able to apply this <see cref="ReductionRule"/> successfully, <see langword="false"/> otherwise.</returns>
        private bool TryApplyReductionRule(IEnumerable<(TreeNode, TreeNode)> edgesToCheck)
        {
            HashSet<(TreeNode, TreeNode)> edgesToBeContracted = new HashSet<(TreeNode, TreeNode)>();
            foreach ((TreeNode, TreeNode) edge in edgesToCheck)
            {
                if (CanEdgeBeContracted(edge))
                {
                    edgesToBeContracted.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            return TryContractEdges(new CountedList<(TreeNode, TreeNode)>(edgesToBeContracted, Measurements.TreeOperationsCounter));
        }

        /// <inheritdoc/>
        internal override bool RunFirstIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule for the first time");
#endif
            HasRun = true;
            Measurements.TimeSpentCheckingApplicability.Start();

            // In the first iteration, check all edges in the input tree.
            return TryApplyReductionRule(Tree.Edges(Measurements.TreeOperationsCounter).Select(Utils.OrderEdgeSmallToLarge));
        }

        /// <inheritdoc/>
        internal override bool RunLaterIteration()
        {
#if VERBOSEDEBUG
            Console.WriteLine($"Applying {GetType().Name} rule in a later iteration");
#endif
            Measurements.TimeSpentCheckingApplicability.Start();

            HashSet<(TreeNode, TreeNode)> edgesToBeChecked = new HashSet<(TreeNode, TreeNode)>();

            /* todo: should not be used. Check what happens in the following case when using Edge objects instead of tuples.
            
            Er is een DP, die is verwijderd uit de graaf. In een latere iteratie van de Idle Edge rule kijken we naar de edges die op dat DP liggen.
            Stel dat dit pad A,B,C,D is. Nadat dat DP verwijderd is, is een edge tussen C en E gecontract, met als resultaat node E. Dit betekent dus dat we voor dat DP in de Idle Edge rule A,B,E,D zouden willen bekijken, maar het pad is niet geüpdate, omdat het DP al verwijderd is. Daarom bekijken we nog steeds A,B,C,D. We vinden dus een edge die gecontract kan worden niet...
            Is dit op te lossen door een edge object te gebruiken ipv tuples?

            Instance:
            string[] split = "--seed=2 --repetitions=1 --experiments=1 --algorithm=GuoNiedermeierKernelisation --treeType=Prufer --dpType=Random --nrNodes=5000 --nrDPs=4000 --maxSolutionSize=0 -v".Split();

            Difference between 68 nodes (not looking at contracted edges) and 67 nodes (looking at contracted edges).
            foreach (((TreeNode, TreeNode) _, TreeNode newNode, CountedCollection<DemandPair> _) in contractedEdges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
            {
                foreach (TreeNode neighbour in newNode.Neighbours(Measurements.TreeOperationsCounter))
                {
                    if ((newNode.ID == 320 && neighbour.ID == 1792) || (newNode.ID == 1792 && neighbour.ID == 320))
                    {

                    }

                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge((newNode, neighbour)));
                }
            }
            */

            foreach (DemandPair demandPair in LastRemovedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath(Measurements.TreeOperationsCounter))
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            foreach ((CountedList<(TreeNode, TreeNode)> edges, DemandPair _) in LastChangedDemandPairs.GetCountedEnumerable(Measurements.DemandPairsOperationsCounter))
            {
                foreach ((TreeNode, TreeNode) edge in edges.GetCountedEnumerable(Measurements.TreeOperationsCounter))
                {
                    edgesToBeChecked.Add(Utils.OrderEdgeSmallToLarge(edge));
                }
            }

            LastContractedEdges.Clear(Measurements.TreeOperationsCounter);
            LastRemovedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);
            LastChangedDemandPairs.Clear(Measurements.DemandPairsOperationsCounter);

            return TryApplyReductionRule(edgesToBeChecked);
        }
    }
}
