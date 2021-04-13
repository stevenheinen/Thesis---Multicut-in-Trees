// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Class that can compute the maximum multi-commodity flow in an <see cref="ITree{N}"/>. Based on a method in a paper by Garg et al.
    /// <br/>
    /// Source: <see href="https://link.springer.com/article/10.1007/BF02523685"/>
    /// </summary>
    public static class MaximumMultiCommodityFlowInTrees
    {
        /// <summary>
        /// The <see cref="Counter"/> that can be used for operations that should not impact the performance of this algorithm.
        /// </summary>
        private static readonly Counter MockCounter = new Counter();

        /// <summary>
        /// Compute the size of the maximum multi-commodity flow in <paramref name="tree"/> with <paramref name="commodities"/> as commodities.
        /// </summary>
        /// <typeparam name="TTree">The type of tree used. Implements <see cref="ITree{N}"/>.</typeparam>
        /// <typeparam name="TNode">The type of nodes in <typeparamref name="TTree"/>. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="tree">The <typeparamref name="TTree"/> in which we are solving the problem.</param>
        /// <param name="commodities">An <see cref="IEnumerable{T}"/> of tuples of two <typeparamref name="TNode"/>s that represent the commodities in the problem instance.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that measures the performance of this algorithm.</param>
        /// <returns>An <see cref="int"/> that represents the size of the maximum multi-commodity flow in the problem instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/>, <paramref name="commodities"/> or <paramref name="measurements"/> is <see langword="null"/>.</exception>
        public static int ComputeMaximumMultiCommodityFlow<TTree, TNode>(TTree tree, IEnumerable<(TNode, TNode)> commodities, PerformanceMeasurements measurements) where TTree : ITree<TNode> where TNode : ITreeNode<TNode>
        {
#if !EXPERIMENT
            Utils.NullCheck(tree, nameof(tree), "Trying to compute the multi commodity flow on a tree, but the tree is null!");
            Utils.NullCheck(commodities, nameof(commodities), "Trying to compute the multi commodity flow on a tree, but the list with commodities is null!");
            Utils.NullCheck(measurements, nameof(measurements), "Trying to compute the multi commodity flow on a tree, but the performance measures are null!");
#endif
            // Create a list with original commodities, that should not be modified, and a copy with commodities that can be modified.
            ReadOnlyCollection<Commodity<TNode>> originalCommodities = new ReadOnlyCollection<Commodity<TNode>>(commodities.Select(n => new Commodity<TNode>(n.Item1, n.Item2)).ToList());
            CountedList<Commodity<TNode>> modifiedCommodities = new CountedList<Commodity<TNode>>();
            foreach (Commodity<TNode> commodity in originalCommodities)
            {
                modifiedCommodities.Add(new Commodity<TNode>(commodity.EndPoint1, commodity.EndPoint2, commodity), MockCounter);
            }

            // Find the internal nodes and sort them on non-increasing depth.
            CountedList<TNode> internalNodesSortedOnDepth = new CountedList<TNode>(tree.Nodes(MockCounter).Where(n => { measurements.TreeOperationsCounter++; return n.NumberOfChildren(MockCounter) > 0; }).OrderByDescending(n => { measurements.TreeOperationsCounter++; return n.DepthFromRoot(measurements.TreeOperationsCounter); }).ToList(), MockCounter);

            // Execute the upward pass that checks for strictly advantageous commodities.
            UpwardPass(internalNodesSortedOnDepth, modifiedCommodities, measurements);

            // Reverse the internal nodes so they are now sorted on non-decreasing depth.
            CountedList<TNode> reversedInternalNodesSortedOnDepth = new CountedList<TNode>();
            foreach (TNode node in internalNodesSortedOnDepth.GetCountedEnumerable(MockCounter))
            {
                reversedInternalNodesSortedOnDepth.Insert(0, node, MockCounter);
            }

            // Pick the commodities that are part of the multicommodity flow in the downward pass.
            List<Commodity<TNode>> pickedCommodities = DownwardPass(reversedInternalNodesSortedOnDepth, modifiedCommodities, measurements);

            // Return the size of the picked commodities.
            return pickedCommodities.Count;
        }

        /// <summary>
        /// Upward pass of the algorithm. Determines all strictly advantageous commodities.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the problem instance. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="internalNodesSortedOnDepth">The <see cref="List{T}"/> of <typeparamref name="TNode"/>s that are internal nodes in the tree and sorted on non-increasing depth.</param>
        /// <param name="commodities">The <see cref="List{T}"/> of <see cref="Commodity{N}"/>s in the problem instance.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that measures the performance of this algorithm.</param>
        private static void UpwardPass<TNode>(CountedList<TNode> internalNodesSortedOnDepth, CountedList<Commodity<TNode>> commodities, PerformanceMeasurements measurements) where TNode : ITreeNode<TNode>
        {
            foreach (TNode node in internalNodesSortedOnDepth.GetCountedEnumerable(measurements.TreeOperationsCounter))
            {
                HashSet<TNode> children = new HashSet<TNode>(node.Children(measurements.TreeOperationsCounter));

                // Determine all commodities of length 1 and add their leaf-endpoint as already used node.
                HashSet<Commodity<TNode>> resolvedCommodities = new HashSet<Commodity<TNode>>(commodities.GetCountedEnumerable(measurements.DemandPairsOperationsCounter).Where(n => n.IsBetween(node, children)));
                HashSet<TNode> takenNodes = DetermineTakenNodes(children, resolvedCommodities);

                // Find all commodities in the subtree
                List<Commodity<TNode>> commoditiesInSubtree = commodities.GetCountedEnumerable(measurements.DemandPairsOperationsCounter).Where(n => n.IsBetween(children)).ToList();

                Dictionary<TNode, Node> nToNode = new Dictionary<TNode, Node>();
                Dictionary<Node, TNode> nodeToN = new Dictionary<Node, TNode>();

                // Create the graph for the matching and find a matching in it.
                Graph<Node> graph = CreateMatchingGraph(children, nToNode, nodeToN, takenNodes, commoditiesInSubtree, false, new Dictionary<(Node, Node), Commodity<TNode>>(), null);
                List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);

                // Determine all unmatched nodes
                HashSet<Node> unmatchedNodes = FindUnmatchedNodes(graph.Nodes(measurements.TreeOperationsCounter), matching);

                // Determine the strictly advantagous communities and change their endpoint to be the current node.
                DetermineStrictlyAdvantageousCommodities(node, commodities, takenNodes, unmatchedNodes, nToNode, matching, measurements);
            }
        }

        /// <summary>
        /// Find all unmatched <typeparamref name="TNode"/>s given <paramref name="matching"/>.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the instance. Implements <see cref="INode{N}"/>.</typeparam>
        /// <param name="allNodes"><see cref="IEnumerable{T}"/> with all <typeparamref name="TNode"/>s.</param>
        /// <param name="matching"><see cref="IEnumerable{T}"/> with tuples of two <typeparamref name="TNode"/>s that represent edges in the matching.</param>
        /// <returns>A <see cref="HashSet{T}"/> with <typeparamref name="TNode"/>s that are unmatched.</returns>
        private static HashSet<TNode> FindUnmatchedNodes<TNode>(IEnumerable<TNode> allNodes, IEnumerable<(TNode, TNode)> matching) where TNode : INode<TNode>
        {
            HashSet<TNode> result = new HashSet<TNode>(allNodes);
            foreach ((TNode, TNode) edge in matching)
            {
                result.Remove(edge.Item1);
                result.Remove(edge.Item2);
            }
            return result;
        }

        /// <summary>
        /// Change the endpoint of a strictly advantageous commodity.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the problem instance. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="commodity">The strictly advantageous <see cref="Commodity{N}"/>.</param>
        /// <param name="oldEndpoint">The old endpoint of <paramref name="commodity"/>.</param>
        /// <param name="newEndpoint">The new endpoint of <paramref name="commodity"/>.</param>
        private static void ChangeEndpointOfCommodity<TNode>(Commodity<TNode> commodity, TNode oldEndpoint, TNode newEndpoint) where TNode : ITreeNode<TNode>
        {
            if (oldEndpoint.Equals(commodity.EndPoint1))
            {
                commodity.EndPoint1 = newEndpoint;
            }
            else if (oldEndpoint.Equals(commodity.EndPoint2))
            {
                commodity.EndPoint2 = newEndpoint;
            }
        }

        /// <summary>
        /// Determines and modifies the strictly advantageous commodities during the downward pass.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the problem instance. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="node">The root of the current subtree.</param>
        /// <param name="commodities"><see cref="List{T}"/> with all <see cref="Commodity{N}"/>s.</param>
        /// <param name="takenNodes">The <see cref="HashSet{T}"/> with all <typeparamref name="TNode"/>s that are already claimed by <see cref="Commodity{N}"/>s of length 1.</param>
        /// <param name="unmatchedNodes"><see cref="HashSet{T}"/> of <see cref="Node"/>s that are unmatched.</param>
        /// <param name="nToNode"><see cref="Dictionary{TKey, TValue}"/> from an <typeparamref name="TNode"/> to its corresponding <see cref="Node"/>.</param>
        /// <param name="matching"><see cref="List{T}"/> of tuples of two <typeparamref name="TNode"/>s that represents edges in the current matching.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that measures the performance of this algorithm.</param>
        private static void DetermineStrictlyAdvantageousCommodities<TNode>(TNode node, CountedList<Commodity<TNode>> commodities, HashSet<TNode> takenNodes, HashSet<Node> unmatchedNodes, Dictionary<TNode, Node> nToNode, List<(Node, Node)> matching, PerformanceMeasurements measurements) where TNode : ITreeNode<TNode>
        {
            // Find all remaining free nodes in the current subtree.
            HashSet<TNode> subtree = new HashSet<TNode>(node.Children(measurements.TreeOperationsCounter)) { node };
            subtree.RemoveWhere(takenNodes.Contains);
            List<Commodity<TNode>> partlyInSubtreeCommodities = commodities.GetCountedEnumerable(measurements.DemandPairsOperationsCounter).Where(n => n.IsPartlyInSubtree(subtree, node, measurements.TreeOperationsCounter)).ToList();

            // Determine all free nodes.
            HashSet<Node> freeNodes = new HashSet<Node>(DFS.FreeNodes(new List<Node>(unmatchedNodes), new HashSet<(Node, Node)>(matching), measurements.TreeOperationsCounter));

            foreach (Commodity<TNode> commodity in partlyInSubtreeCommodities)
            {
                // If the commodity has the current node (root of the current subtree) as endpoint, it is strictly advantageous but does not need to be changed.
                if (commodity.EndPoint1.Equals(node) || commodity.EndPoint2.Equals(node))
                {
                    continue;
                }

                TNode insideN = commodity.EndpointInSubtree(subtree);
                Node inside = nToNode[insideN];

                // If the node inside the subtree is unmatched, the the commodity is strictly advantageous.
                if (unmatchedNodes.Contains(inside))
                {
                    ChangeEndpointOfCommodity(commodity, insideN, node);
                    continue;
                }

                // If any one of the free nodes is the endpoint of this commodity, this commodity is strictly advantageous.
                if (freeNodes.Contains(inside))
                {
                    ChangeEndpointOfCommodity(commodity, insideN, node);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="Graph{N}"/> with <see cref="Node"/>s to solve the problem on a height-1 tree.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the problem instance. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="children">The <see cref="HashSet{T}"/> of children of the current <typeparamref name="TNode"/>.</param>
        /// <param name="nToNode"><see cref="Dictionary{TKey, TValue}"/> from <typeparamref name="TNode"/> to <see cref="Node"/>. Will be filled in this method.</param>
        /// <param name="nodeToN"><see cref="Dictionary{TKey, TValue}"/> from <see cref="Node"/> to <typeparamref name="TNode"/>. Will be filled in this method.</param>
        /// <param name="takenNodes">The <see cref="HashSet{T}"/> with all <typeparamref name="TNode"/>s that are already claimed by <see cref="Commodity{N}"/>s of length 1.</param>
        /// <param name="commoditiesInSubtree"><see cref="List{T}"/> of all <see cref="Commodity{N}"/>s in the current subtree.</param>
        /// <param name="downward"><see cref="bool"/> whether this is currently the downward pass.</param>
        /// <param name="edgeToCommodity"><see cref="Dictionary{TKey, TValue}"/> from an edge, represented by a tuple of two <see cref="Node"/>s to the <see cref="Commodity{N}"/> that is picked on this edge. Only needed in downward pass.</param>
        /// <param name="pickedCommodity">The <see cref="Commodity{N}"/> that is picked on the edge between the root of the current subtree and its parent. Only needed in downward pass.</param>
        /// <returns>A <see cref="Graph{N}"/> with a <see cref="Node"/> for each leaf of the current subtree and an edge between them if there is a <see cref="Commodity{N}"/> between them.</returns>
        private static Graph<Node> CreateMatchingGraph<TNode>(HashSet<TNode> children, Dictionary<TNode, Node> nToNode, Dictionary<Node, TNode> nodeToN, HashSet<TNode> takenNodes, List<Commodity<TNode>> commoditiesInSubtree, bool downward, Dictionary<(Node, Node), Commodity<TNode>> edgeToCommodity, Commodity<TNode> pickedCommodity) where TNode : ITreeNode<TNode>
        {
            // Create nodes for the matching graph.
            List<Node> nodes = children.Select(n =>
            {
                Node node = new Node(n.ID);
                nToNode[n] = node;
                nodeToN[node] = n;
                return node;
            }).ToList();

            // Create an edge for each commodity that is in the subtree
            HashSet<(Node, Node)> edges = new HashSet<(Node, Node)>();
            foreach (Commodity<TNode> localCommodity in commoditiesInSubtree)
            {
                // If this is the upward pass, and we have picked a commodity in the subtree above this one, check if we can still use this edge. Otherwise, skip.
                if (downward && !(pickedCommodity is null) && pickedCommodity.OriginalCommodity.Path.Contains(localCommodity.EndPoint1) && pickedCommodity.OriginalCommodity.Path.Contains(localCommodity.EndPoint2))
                {
                    continue;
                }

                // If this commodity has a node that is already taken as endpoint, skip it.
                if (takenNodes.Contains(localCommodity.EndPoint1) || takenNodes.Contains(localCommodity.EndPoint2))
                {
                    continue;
                }

                Node endpoint1 = nToNode[localCommodity.EndPoint1];
                Node endpoint2 = nToNode[localCommodity.EndPoint2];
                (Node, Node) edge = Utils.OrderEdgeSmallToLarge((endpoint1, endpoint2));
                if (downward)
                {
                    edgeToCommodity[edge] = localCommodity;
                }
                edges.Add(edge);
            }

            // Create and return the graph.
            Graph<Node> graph = new Graph<Node>();
            graph.AddNodes(nodes, MockCounter);
            graph.AddEdges(edges, MockCounter);
            return graph;
        }

        /// <summary>
        /// Determines all <typeparamref name="TNode"/>s that are already claimed after <see cref="Commodity{N}"/>s of length-1 have been chosen.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the problem instance. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="children">The <see cref="HashSet{T}"/> of <typeparamref name="TNode"/>s in the current subtree.</param>
        /// <param name="resolvedCommodities"><see cref="HashSet{T}"/> of <see cref="Commodity{N}"/>s of length 1.</param>
        /// <returns>A <see cref="HashSet{T}"/> of <typeparamref name="TNode"/> that contains all <typeparamref name="TNode"/>s that are an endpoint of any <see cref="Commodity{N}"/> in <paramref name="resolvedCommodities"/>.</returns>
        private static HashSet<TNode> DetermineTakenNodes<TNode>(HashSet<TNode> children, HashSet<Commodity<TNode>> resolvedCommodities) where TNode : ITreeNode<TNode>
        {
            HashSet<TNode> takenNodes = new HashSet<TNode>();
            foreach (Commodity<TNode> commodity in new HashSet<Commodity<TNode>>(resolvedCommodities))
            {
                TNode endpointInSubtree = commodity.EndpointInSubtree(children);
                if (takenNodes.Contains(endpointInSubtree))
                {
                    resolvedCommodities.Remove(commodity);
                }
                else
                {
                    takenNodes.Add(endpointInSubtree);
                }
            }
            return takenNodes;
        }

        /// <summary>
        /// Pick final <see cref="Commodity{N}"/>s for a subtree.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the problem instace. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="pickedCommodities">The <see cref="List{T}"/> picked <see cref="Commodity{N}"/>s should be added to.</param>
        /// <param name="node">The root of the current subtree.</param>
        /// <param name="resolvedCommodities">The <see cref="List{T}"/> of <see cref="Commodity{N}"/>s of length 1.</param>
        /// <param name="matching"><see cref="List{T}"/> of tuples of two <see cref="Node"/>s that represent edges in the current matching.</param>
        /// <param name="edgeToPickedCommodity"><see cref="Dictionary{TKey, TValue}"/> with per edge (tuple of two <typeparamref name="TNode"/>s) the final picked <see cref="Commodity{N}"/>.</param>
        /// <param name="edgeToCommodity"><see cref="Dictionary{TKey, TValue}"/> with per edge (tuple of two <see cref="Node"/>s) in the current matching graph the <see cref="Commodity{N}"/> that corresponds to this edge.</param>
        private static void PickCommodities<TNode>(List<Commodity<TNode>> pickedCommodities, TNode node, HashSet<Commodity<TNode>> resolvedCommodities, List<(Node, Node)> matching, Dictionary<(TNode, TNode), Commodity<TNode>> edgeToPickedCommodity, Dictionary<(Node, Node), Commodity<TNode>> edgeToCommodity) where TNode : ITreeNode<TNode>
        {
            // Pick all commodities of size 1.
            foreach (Commodity<TNode> commodity in resolvedCommodities)
            {
                edgeToPickedCommodity[Utils.OrderEdgeSmallToLarge((commodity.EndPoint1, commodity.EndPoint2))] = commodity;
                pickedCommodities.Add(commodity.OriginalCommodity);
            }

            // Pick all commodites that belong to an edge in the matching.
            foreach ((Node, Node) edge in matching)
            {
                Commodity<TNode> commodityOnEdge = edgeToCommodity[edge];
                edgeToPickedCommodity[Utils.OrderEdgeSmallToLarge((node, commodityOnEdge.EndPoint1))] = commodityOnEdge;
                edgeToPickedCommodity[Utils.OrderEdgeSmallToLarge((node, commodityOnEdge.EndPoint2))] = commodityOnEdge;
                pickedCommodities.Add(commodityOnEdge.OriginalCommodity);
            }
        }

        /// <summary>
        /// Downward pass of the algorithm. Picks all <see cref="Commodity{N}"/>s per edge.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the problem instance. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        /// <param name="internalNodesSortedOnDepth"><see cref="List{T}"/> of all internal <typeparamref name="TNode"/>s sorted on non-decreasing depth.</param>
        /// <param name="commodities">The <see cref="List{T}"/> of <see cref="Commodity{N}"/>s in the current problem instance.</param>
        /// <param name="measurements">The <see cref="PerformanceMeasurements"/> that measures the performance of this algorithm.</param>
        /// <returns>A <see cref="List{T}"/> with the final <see cref="Commodity{N}"/>s that are picked.</returns>
        private static List<Commodity<TNode>> DownwardPass<TNode>(CountedList<TNode> internalNodesSortedOnDepth, CountedList<Commodity<TNode>> commodities, PerformanceMeasurements measurements) where TNode : ITreeNode<TNode>
        {
            // Final list of picked commodities
            List<Commodity<TNode>> pickedCommodities = new List<Commodity<TNode>>();

            // Dictionary with per edge in the problem instance the commodity that is picked along it.
            Dictionary<(TNode, TNode), Commodity<TNode>> edgeToPickedCommodity = new Dictionary<(TNode, TNode), Commodity<TNode>>();

            foreach (TNode node in internalNodesSortedOnDepth.GetCountedEnumerable(measurements.TreeOperationsCounter))
            {
                // Check if a commodity was picked on the edge between the root and its parent.
                Commodity<TNode> pickedCommodity = null;
                TNode parent = node.GetParent(measurements.TreeOperationsCounter);
                if (!(parent is null))
                {
                    // not the root, so there is probably a commodity on the edge between this node and its parent. We cannot use the edge it continues on
                    edgeToPickedCommodity.TryGetValue(Utils.OrderEdgeSmallToLarge((parent, node)), out pickedCommodity);
                }

                HashSet<TNode> children = new HashSet<TNode>(node.Children(measurements.TreeOperationsCounter));
                HashSet<Commodity<TNode>> resolvedCommodities = new HashSet<Commodity<TNode>>(commodities.GetCountedEnumerable(measurements.DemandPairsOperationsCounter).Where(n => n.IsBetween(node, children)));
                HashSet<TNode> takenNodes = DetermineTakenNodes(children, resolvedCommodities);
                List<Commodity<TNode>> commoditiesInSubtree = commodities.GetCountedEnumerable(measurements.DemandPairsOperationsCounter).Where(n => n.IsBetween(children)).ToList();

                Dictionary<TNode, Node> nToNode = new Dictionary<TNode, Node>();
                Dictionary<Node, TNode> nodeToN = new Dictionary<Node, TNode>();
                Dictionary<(Node, Node), Commodity<TNode>> edgeToCommodity = new Dictionary<(Node, Node), Commodity<TNode>>();

                // Compute the matching graph and the matching in that graph.
                Graph<Node> graph = CreateMatchingGraph(children, nToNode, nodeToN, takenNodes, commoditiesInSubtree, true, edgeToCommodity, pickedCommodity);
                List<(Node, Node)> matching = EdmondsMatching.FindMaximumMatching<Graph<Node>, Node>(graph);

                // Pick the commodities in the current subtree.
                PickCommodities(pickedCommodities, node, resolvedCommodities, matching, edgeToPickedCommodity, edgeToCommodity);
            }

            return pickedCommodities;
        }

        /// <summary>
        /// Representation of a single source-sink pair in the instance.
        /// </summary>
        /// <typeparam name="TNode">The type of node in the problem. Implements <see cref="ITreeNode{N}"/>.</typeparam>
        private class Commodity<TNode> where TNode : ITreeNode<TNode>
        {
            /// <summary>
            /// The original <see cref="Commodity{N}"/> this <see cref="Commodity{N}"/> once was.
            /// </summary>
            public Commodity<TNode> OriginalCommodity { get; }

            /// <summary>
            /// The first endpoint of this commodity.
            /// </summary>
            public TNode EndPoint1 { get; set; }

            /// <summary>
            /// The second endpoint of this commodity.
            /// </summary>
            public TNode EndPoint2 { get; set; }

            /// <summary>
            /// The <typeparamref name="TNode"/>s on the unique path between <see cref="EndPoint1"/> and <see cref="EndPoint2"/>.
            /// </summary>
            public HashSet<TNode> Path { get; }

            /// <summary>
            /// Constructor for a <see cref="Commodity{N}"/>.
            /// </summary>
            /// <param name="endPoint1">The first endpoint of this <see cref="Commodity{N}"/>.</param>
            /// <param name="endPoint2">The second endpoint of this <see cref="Commodity{N}"/>.</param>
            public Commodity(TNode endPoint1, TNode endPoint2)
            {
                EndPoint1 = endPoint1;
                EndPoint2 = endPoint2;
                OriginalCommodity = null;
                Path = new HashSet<TNode>(DFS.FindPathBetween(endPoint1, endPoint2, new Counter()));
            }

            /// <summary>
            /// Constructor for a <see cref="Commodity{N}"/> that is a copy of another existing <see cref="Commodity{N}"/>.
            /// </summary>
            /// <param name="endPoint1">The first endpoint of this <see cref="Commodity{N}"/>.</param>
            /// <param name="endPoint2">The second endpoint of this <see cref="Commodity{N}"/>.</param>
            /// <param name="originalCommodity">The original <see cref="Commodity{N}"/> this <see cref="Commodity{N}"/> is a copy of.</param>
            public Commodity(TNode endPoint1, TNode endPoint2, Commodity<TNode> originalCommodity)
            {
                EndPoint1 = endPoint1;
                EndPoint2 = endPoint2;
                OriginalCommodity = originalCommodity;
                Path = new HashSet<TNode>(originalCommodity.Path);
            }

            /// <summary>
            /// Returns whether this <see cref="Commodity{N}"/> is between <paramref name="node"/> and an <typeparamref name="TNode"/> in <paramref name="otherNodes"/>.
            /// </summary>
            /// <param name="node">The <typeparamref name="TNode"/> that should be one of the endpoints of this <see cref="Commodity{N}"/>.</param>
            /// <param name="otherNodes">The <see cref="HashSet{T}"/> of <typeparamref name="TNode"/>s that should contain the other endpoint of this <see cref="Commodity{N}"/>.</param>
            /// <returns><see langword="true"/> if this <see cref="Commodity{N}"/> is between <paramref name="node"/> and an <typeparamref name="TNode"/> in <paramref name="otherNodes"/>, <see langword="false"/> otherwise.</returns>
            public bool IsBetween(TNode node, HashSet<TNode> otherNodes)
            {
                return (node.Equals(EndPoint1) && otherNodes.Contains(EndPoint2)) || (node.Equals(EndPoint2) && otherNodes.Contains(EndPoint1));
            }

            /// <summary>
            /// Returns whether this <see cref="Commodity{N}"/> is between two <typeparamref name="TNode"/>s in <paramref name="nodes"/>.
            /// </summary>
            /// <param name="nodes">The <see cref="HashSet{T}"/> of <typeparamref name="TNode"/>s that should contain the endpoints of this <see cref="Commodity{N}"/>.</param>
            /// <returns><see langword="true"/> if this <see cref="Commodity{N}"/> is between two <typeparamref name="TNode"/>s in <paramref name="nodes"/>, <see langword="false"/> otherwise.</returns>
            public bool IsBetween(HashSet<TNode> nodes)
            {
                return nodes.Contains(EndPoint1) && nodes.Contains(EndPoint2);
            }

            /// <summary>
            /// Returns whether this <see cref="Commodity{N}"/> has one endpoint in <paramref name="nodesInSubtree"/> and leaves the subtree via <paramref name="root"/>.
            /// </summary>
            /// <param name="nodesInSubtree">The <see cref="HashSet{T}"/> of <typeparamref name="TNode"/>s in the subtree.</param>
            /// <param name="root">The <typeparamref name="TNode"/> that is the root of this subtree.</param>
            /// <param name="counter">The graph <see cref="Counter"/> used for performance measurement.</param>
            /// <returns><see langword="true"/> if exactly one of <see cref="EndPoint1"/> or <see cref="EndPoint2"/> is present in <paramref name="nodesInSubtree"/>, and <see cref="Path"/> contains <paramref name="root"/>, <see langword="false"/> otherwise.</returns>
            public bool IsPartlyInSubtree(HashSet<TNode> nodesInSubtree, TNode root, Counter counter)
            {
                TNode parent = root.GetParent(counter);
                if (parent is null)
                {
                    return false;
                }
                return ((nodesInSubtree.Contains(EndPoint1) && !nodesInSubtree.Contains(EndPoint2)) || (nodesInSubtree.Contains(EndPoint2) && !nodesInSubtree.Contains(EndPoint1))) && Path.Contains(parent);
            }

            /// <summary>
            /// Returns the <typeparamref name="TNode"/> that is the endpoint of this <see cref="Commodity{N}"/>.
            /// </summary>
            /// <param name="nodesInSubtree">The <see cref="HashSet{T}"/> of <typeparamref name="TNode"/>s that should contain <see cref="EndPoint1"/> or <see cref="EndPoint2"/>.</param>
            /// <returns><see cref="EndPoint1"/> if <see cref="EndPoint1"/> is present in <paramref name="nodesInSubtree"/>, <see cref="EndPoint2"/> if <see cref="EndPoint2"/> is present in <paramref name="nodesInSubtree"/>, <see langword="default"/> otherwise.</returns>
            public TNode EndpointInSubtree(HashSet<TNode> nodesInSubtree)
            {
                if (nodesInSubtree.Contains(EndPoint1))
                {
                    return EndPoint1;
                }
                if (nodesInSubtree.Contains(EndPoint2))
                {
                    return EndPoint2;
                }
                return default;
            }

            /// <summary>
            /// Returns a <see cref="string"/> representation of this <see cref="Commodity{N}"/>. Looks like "([ep1], [ep2])", where ep1 is the <see cref="string"/> representation of <see cref="EndPoint1"/>, and ep2 the <see cref="string"/> representation of <see cref="EndPoint2"/>.
            /// </summary>
            /// <returns>The <see cref="string"/> representation of this <see cref="Commodity{N}"/>.</returns>
            public override string ToString()
            {
                return $"({EndPoint1}, {EndPoint2})";
            }
        }
    }
}
