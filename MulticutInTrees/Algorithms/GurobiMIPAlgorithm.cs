﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using Gurobi;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// Algorithm that uses the Gurobi MIP solver to find the minimum possible solution size for an instance.
    /// </summary>
    public class GurobiMIPAlgorithm
    {
        /// <summary>
        /// The <see cref="Tree{T}"/> in the instance.
        /// </summary>
        private Tree<TreeNode> Tree { get; }

        /// <summary>
        /// The <see cref="DemandPair"/>s in the instance.
        /// </summary>
        private List<DemandPair> DemandPairs { get; }

        /// <summary>
        /// Counter that can be used for operations that should not impact the performance.
        /// </summary>
        private Counter MockCounter { get; }

        /// <summary>
        /// Constructor for the <see cref="GurobiMIPAlgorithm"/>.
        /// </summary>
        /// <param name="tree">The <see cref="Tree{T}"/> in the instance.</param>
        /// <param name="demandPairs">The <see cref="DemandPair"/>s in the instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="demandPairs"/> is <see langword="null"/>.</exception>
        public GurobiMIPAlgorithm(Tree<TreeNode> tree, List<DemandPair> demandPairs)
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to create an instance of the Gurobi MIP algorithm, but the tree is null!");
            Utils.NullCheck(demandPairs, nameof(demandPairs), "Trying to create an instance of the Gurobi MIP algorithm, but the list with demand pairs is null!");
#endif
            Tree = tree;
            DemandPairs = demandPairs;
            MockCounter = new Counter();
        }

        /// <summary>
        /// Find the smallest possible solution size in this instnace.
        /// </summary>
        /// <param name="verbose">Whether the output of the solver should be printed to the console.</param>
        /// <returns>The smallest possible solution size in this instance.</returns>
        /// <exception cref="GRBException">Thrown when something goes wrong during the execution of the solver.</exception>
        public int Run(bool verbose = false)
        {
            try
            {
                GRBEnv env = new GRBEnv(true);
                if (verbose)
                {
                    env.Set("LogToConsole", "1");
                }
                else
                {
                    env.Set("LogToConsole", "0");
                }
                env.Set("LogFile", "mipMinSolSizeSolver.log");
                env.Start();

                GRBModel model = new GRBModel(env);

                int nrEdges = Tree.NumberOfEdges(MockCounter);
                string[] edgeNames = Tree.Edges(MockCounter).Select(e => Utils.OrderEdgeSmallToLarge(e).ToString()).ToArray();

                Dictionary<string, GRBVar> edgeNameToVariable = new Dictionary<string, GRBVar>();

                GRBVar[] variables = new GRBVar[nrEdges];
                GRBLinExpr objective = new GRBLinExpr();
                for (int i = 0; i < nrEdges; i++)
                {
                    variables[i] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, edgeNames[i]);
                    edgeNameToVariable[edgeNames[i]] = variables[i];
                    objective.AddTerm(1.0, variables[i]);
                }

                model.SetObjective(objective);
                model.ModelSense = GRB.MINIMIZE;

                foreach (DemandPair demandPair in DemandPairs)
                {
                    GRBLinExpr pathValue = 0.0;
                    foreach ((TreeNode, TreeNode) edge in demandPair.EdgesOnDemandPath(MockCounter))
                    {
                        pathValue.AddTerm(1.0, edgeNameToVariable[Utils.OrderEdgeSmallToLarge(edge).ToString()]);
                    }
                    model.AddConstr(pathValue >= 1, demandPair.ToString());
                }

                model.Optimize();

                return (int)model.ObjVal;
            }
            catch (GRBException e)
            {
                Console.WriteLine($"Error in the MIP solver! {e}");
                Console.WriteLine("Throwing the same error to stop the program.");
                throw e;
            }
        }
    }
}