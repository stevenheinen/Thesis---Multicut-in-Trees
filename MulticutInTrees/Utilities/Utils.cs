// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Graphs;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.Utilities
{
    /// <summary>
    /// Static class containing utility functions used throughout the entire program.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Method that checks whether <paramref name="parameter"/> is <see langword="null"/>. Throws an <see cref="ArgumentNullException"/> if it is.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="parameter"/>.</typeparam>
        /// <param name="parameter">The <typeparamref name="T"/> for which we want to check if it is <see langword="null"/>.</param>
        /// <param name="paramName">The name of <paramref name="parameter"/> in the method this is called from.</param>
        /// <param name="message">Optional. The custom message that should be given to the <see cref="ArgumentNullException"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is <see langword="null"/>.</exception>
        internal static void NullCheck<T>(T parameter, string paramName, string message = null)
        {
            if (paramName is null)
            {
                throw new ArgumentNullException(nameof(paramName), "Trying to perform a null check on the parameters of a method, but the name of the parameter given to the null check method is null!");
            }

            if (parameter is null)
            {
                if (message is null)
                {
                    throw new ArgumentNullException(paramName);
                }

                throw new ArgumentNullException(paramName, message);
            }
        }

        /// <summary>
        /// Orders a tuple of <typeparamref name="TNode"/>s representing an edge in such a way that the <typeparamref name="TNode"/> with the smallest <see cref="AbstractNode{TNode}.ID"/> is the first element of the resulting tuple.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes the edge is between.</typeparam>
        /// <param name="edge">The tuple of <typeparamref name="TNode"/>s representing the edge we want to order.</param>
        /// <returns>A tuple with the same <typeparamref name="TNode"/>s as <paramref name="edge"/>, such that the <typeparamref name="TNode"/> with the smallest <see cref="AbstractNode{TNode}.ID"/> is the first element in the result.</returns>
        /// <exception cref="ArgumentNullException">Thrown whein either endpoint of <paramref name="edge"/> is <see langword="null"/>.</exception>
        public static (TNode, TNode) OrderEdgeSmallToLarge<TNode>((TNode, TNode) edge) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            NullCheck(edge.Item1, nameof(edge.Item1), "Trying to order an edge from smallest to largest, but the first endpoint of the edge is null!");
            NullCheck(edge.Item2, nameof(edge.Item2), "Trying to order an edge from smallest to largest, but the second endpoint of the edge is null!");
#endif
            return edge.Item1.ID < edge.Item2.ID ? edge : (edge.Item2, edge.Item1);
        }

        /// <summary>
        /// Orders a <typeparamref name="TEdge"/> in such a way that the <typeparamref name="TNode"/> with the smallest <see cref="AbstractNode{TNode}.ID"/> is the first element of the resulting tuple.
        /// </summary>
        /// <typeparam name="TEdge">The type of edge.</typeparam>
        /// <typeparam name="TNode">The type of nodes the edge is between.</typeparam>
        /// <param name="edge">The tuple of <typeparamref name="TNode"/>s representing the edge we want to order.</param>
        /// <returns>A tuple with the same <typeparamref name="TNode"/>s as <paramref name="edge"/>, such that the <typeparamref name="TNode"/> with the smallest <see cref="AbstractNode{TNode}.ID"/> is the first element in the result.</returns>
        /// <exception cref="ArgumentNullException">Thrown whein either endpoint of <paramref name="edge"/> is <see langword="null"/>.</exception>
        public static (TNode, TNode) OrderEdgeSmallToLarge<TEdge, TNode>(TEdge edge) where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            NullCheck(edge, nameof(edge), "Trying to order an edge from smallest to largest, but the edge is null!");
#endif
            return edge.Endpoint1.ID < edge.Endpoint2.ID ? (edge.Endpoint1, edge.Endpoint2) : (edge.Endpoint2, edge.Endpoint1);
        }

        /// <summary>
        /// Checks whether this <see cref="IList{T}"/> is a subset of another <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="IList{T}"/>s.</typeparam>
        /// <param name="subset">The current <see cref="IList{T}"/>, the potential subset.</param>
        /// <param name="largerSet">The other <see cref="IList{T}"/>, the potential superset.</param>
        /// <returns><see langword="true"/> if <paramref name="subset"/> is a subset of <paramref name="largerSet"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="subset"/> or<paramref name="largerSet"/> is <see langword="null"/>.</exception>
        public static bool IsSubsetOf<T>(this IEnumerable<T> subset, IEnumerable<T> largerSet)
        {
#if !EXPERIMENT
            NullCheck(subset, nameof(subset), "Trying to see if an IEnumerable is a subset of another IEnumerable, but the first IEnumerable is null!");
            NullCheck(largerSet, nameof(largerSet), "Trying to see if an IEnumerable is a subset of another IEnumerable, but the second IEnumerable is null!");
#endif
            if (subset.Count() > largerSet.Count())
            {
                return false;
            }

            HashSet<T> larger = new HashSet<T>(largerSet);
            return subset.All(n => larger.Contains(n));
        }

        /// <summary>
        /// Creates a <see cref="string"/> from an <see cref="IEnumerable{T}"/> like Python does.
        /// <br/>
        /// Looks like: "IEnumerable with {n} elements: [elem1, elem2, ..., elemn]" where n is a number, and elemi are elements of <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="list"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> we want to print.</param>
        /// <returns>A Python-like <see cref="string"/> representation of <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is <see langword="null"/>.</exception>
        public static string Print<T>(this IEnumerable<T> list)
        {
#if !EXPERIMENT
            NullCheck(list, nameof(list), "Trying to print an IEnumerable, but the IEnumerable is null!");
#endif
            StringBuilder sb = new StringBuilder();
            sb.Append($"{list.GetType()} with {list.Count()} elements: [");
            foreach (T elem in list)
            {
                sb.Append($"{elem}, ");
            }
            sb.Remove(sb.Length - 2, 2);
            if (!list.Any())
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append('.');
            }
            else
            {
                sb.Append("]");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks whether a path is a simple path (i.e. does not contain cycles).
        /// </summary>
        /// <typeparam name="TNode">The type of nodes on the path.</typeparam>
        /// <param name="path">An <see cref="IEnumerable{T}"/> of tuples of <typeparamref name="TNode"/>s that represent the edges on the path.</param>
        /// <returns><see langword="true"/> if <paramref name="path"/> is a simple path, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
        public static bool IsSimplePath<TNode>(IEnumerable<(TNode, TNode)> path) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            NullCheck(path, nameof(path), "Trying to see whether a path is a simple path, but the path is null!");
#endif
            for (int i = 0; i < path.Count() - 1; i++)
            {
                for (int j = i + 1; j < path.Count(); j++)
                {
                    if (path.ElementAt(i).Item1.Equals(path.ElementAt(j).Item2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Transforms an <see cref="IEnumerable{T}"/> with <typeparamref name="TNode"/>s representing a path to a <see cref="List{T}"/> with tuples of two <typeparamref name="TNode"/>s representing the edges on the path.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes on the path.</typeparam>
        /// <param name="path">An <see cref="IEnumerable{T}"/> with <typeparamref name="TNode"/>s we want to tranform to a <see cref="List{T}"/> with the edges on the path.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> with tuples of <typeparamref name="TNode"/>s representing the edges on <paramref name="path"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <see langword="null"/>.</exception>
        public static List<(TNode, TNode)> NodePathToEdgePath<TNode>(IEnumerable<TNode> path) where TNode : AbstractNode<TNode>
        {
#if !EXPERIMENT
            NullCheck(path, nameof(path), "Trying to transform a node path to an edge path, but the path is null!");
#endif
            List<(TNode, TNode)> result = new List<(TNode, TNode)>();
            for (int i = 0; i < path.Count() - 1; i++)
            {
                TNode endpoint1 = path.ElementAt(i);
                TNode endpoint2 = path.ElementAt(i + 1);
                result.Add((endpoint1, endpoint2));
            }

            return result;
        }

        /// <summary>
        /// Picks a random element from an <see cref="IEnumerable{T}"/> that fits a given condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> we want to pick a conditioned random element from.</param>
        /// <param name="condition">The <see cref="Func{T, TResult}"/> that a <typeparamref name="T"/> in <paramref name="list"/> must return <see langword="true"/> on to be considered to be picked.</param>
        /// <param name="random">The <see cref="Random"/> used to pick an arbitrary element.</param>
        /// <returns>A uniform randomly picked <typeparamref name="T"/> that returns <see langword="true"/> on <paramref name="condition"/> from <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/>, <paramref name="condition"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        public static T PickRandomWhere<T>(this IEnumerable<T> list, Func<T, bool> condition, Random random)
        {
#if !EXPERIMENT
            NullCheck(list, nameof(list), "Trying to pick a random element form an IEnumerable that fits a condition, but the IEnumerable is null!");
            NullCheck(condition, nameof(condition), "Trying to pick a random element form an IEnumerable that fits a condition, but the function with the condition is null!");
            NullCheck(random, nameof(random), "Trying to pick a random element form an IEnumerable that fits a condition, but the random number generator is null!");
#endif
            return list.Where(condition).PickRandom(random);
        }

        /// <summary>
        /// Picks a random element from an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> we want to pick a random element from.</param>
        /// <param name="random">The <see cref="Random"/> used to pick an arbitrary element.</param>
        /// <returns>A uniform randomly picked <typeparamref name="T"/> from <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> or <paramref name="random"/> is <see langword="null"/>.</exception>
        public static T PickRandom<T>(this IEnumerable<T> list, Random random)
        {
#if !EXPERIMENT
            NullCheck(list, nameof(list), "Trying to pick a random element from an IEnumerable, but the IEnumerable is null!");
            NullCheck(random, nameof(random), "Trying to pick a random element from an IEnumerable, but the random number generator is null!");
#endif
            return list.ElementAt(random.Next(list.Count()));
        }

        /// <summary>
        /// Finds the largest <see cref="int"/> for which <paramref name="function"/> returns <see langword="true"/>.
        /// </summary>
        /// <param name="minValue">The smallest value to check.</param>
        /// <param name="maxValue">The largest value to check.</param>
        /// <param name="function"><see cref="Func{T, TResult}"/> from <see cref="int"/> to <see cref="bool"/>. Given the numbers in the range between <paramref name="minValue"/> and <paramref name="maxValue"/>, should return any number of <see langword="true"/> results, followed by any number of <see langword="false"/> results.</param>
        /// <returns>The largest <see cref="int"/> for which <paramref name="function"/> returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="function"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="minValue"/> is larger than <paramref name="maxValue"/>.</exception>
        public static int BinarySearchGetLastTrue(int minValue, int maxValue, Func<int, bool> function)
        {
#if !EXPERIMENT
            NullCheck(function, nameof(function), "Trying to binary search for the last value that results in true, but the function is null!");
            if (minValue > maxValue)
            {
                throw new ArgumentException("Trying to binary search for the last value that results in true, but the start of the range of values to check is larger than the end of the range of values to check!");
            }
#endif
            int answer = -1;
            int current = (minValue + maxValue + 1) / 2;
            while (minValue <= maxValue)
            {
                if (function(current))
                {
                    answer = current;
                    minValue = current + 1;
                }
                else
                {
                    maxValue = current - 1;
                }
                current = (minValue + maxValue + 1) / 2;
            }
            return answer;
        }

        /// <summary>
        /// Finds the smallest <see cref="int"/> for which <paramref name="function"/> returns <see langword="true"/>.
        /// </summary>
        /// <param name="minValue">The smallest value to check.</param>
        /// <param name="maxValue">The largest value to check.</param>
        /// <param name="function"><see cref="Func{T, TResult}"/> from <see cref="int"/> to <see cref="bool"/>. Given the numbers in the range between <paramref name="minValue"/> and <paramref name="maxValue"/>, should return any number of <see langword="false"/> results, followed by any number of <see langword="true"/> results.</param>
        /// <returns>The smallest <see cref="int"/> for which <paramref name="function"/> returns <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="function"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="minValue"/> is larger than <paramref name="maxValue"/>.</exception>
        public static int BinarySearchGetFirstTrue(int minValue, int maxValue, Func<int, bool> function)
        {
#if !EXPERIMENT
            NullCheck(function, nameof(function), "Trying to binary search for the first value that results in true, but the function is null!");
            if (minValue > maxValue)
            {
                throw new ArgumentException("Trying to binary search for the first value that results in true, but the start of the range of values to check is larger than the end of the range of values to check!");
            }
#endif
            int answer = -1;
            int current = (minValue + maxValue + 1) / 2;
            while (minValue <= maxValue)
            {
                if (!function(current))
                {
                    minValue = current + 1;
                }
                else
                {
                    answer = current;
                    maxValue = current - 1;
                }
                current = (minValue + maxValue + 1) / 2;
            }
            return answer;
        }

        /// <summary>
        /// Find all subsets of size <paramref name="subsetSize"/> in <paramref name="list"/>.
        /// <br/>
        /// Source: <see href="https://stackoverflow.com/questions/23974569/how-to-generate-all-subsets-of-a-given-size"/>
        /// </summary>
        /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> in which to find subsets.</param>
        /// <param name="subsetSize">The required size of the subsets.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with subsets of <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="subsetSize"/> is negative.</exception>
        public static IEnumerable<IEnumerable<T>> AllSubsetsOfSize<T>(this IEnumerable<T> list, int subsetSize)
        {
#if !EXPERIMENT
            NullCheck(list, nameof(list), "Trying to find all subsets of a certain size in an IEnumerable, but the IEnumerable is null!");
            if (subsetSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(subsetSize), $"Trying to find all subsets of size {subsetSize} in a list. Make sure the asked subsetsize is not negative!");
            }
#endif
            // Generate list of sequences containing only 1 element e.g. {1}, {2}, ...
            IEnumerable<T[]> oneElemSequences = list.Select(x => new[] { x });

            // Generate List of T sequences
            List<List<T>> result = new List<List<T>>
            {
                // Add initial empty set
                new List<T>()
            };

            // Generate powerset, but skip sequences that are too long
            foreach (T[] oneElemSequence in oneElemSequences)
            {
                int length = result.Count;

                for (int i = 0; i < length; i++)
                {
                    if (result[i].Count >= subsetSize)
                    {
                        continue;
                    }

                    result.Add(result[i].Concat(oneElemSequence).ToList());
                }
            }

            return result.Where(x => x.Count == subsetSize);
        }

        /// <summary>
        /// Create a <see cref="RootedTree"/> from <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph to create the rooted tree from.</typeparam>
        /// <typeparam name="TEdge">The type of edges in the graph.</typeparam>
        /// <typeparam name="TNode">The type of nodes in the graph.</typeparam>
        /// <param name="graph">The <typeparamref name="TGraph"/> to create the <see cref="RootedTree"/> from.</param>
        /// <returns>A <see cref="RootedTree"/> created from <paramref name="graph"/>, and a <see cref="Dictionary{TKey, TValue}"/> from the original <typeparamref name="TNode"/>s in <paramref name="graph"/> to the newly created <see cref="RootedTreeNode"/>s in the tree.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="graph"/> is not a tree.</exception>
        public static (RootedTree, Dictionary<TNode, RootedTreeNode>) CreateRootedTreeFromGraph<TGraph, TEdge, TNode>(TGraph graph) where TGraph : AbstractGraph<TEdge, TNode> where TEdge : Edge<TNode> where TNode : AbstractNode<TNode>
        {
            Counter mockCounter = new Counter();
#if !EXPERIMENT
            NullCheck(graph, nameof(graph), "Trying to create a rooted tree from a graph, but the provided graph is null!");
            if (!graph.IsTree(mockCounter))
            {
                throw new ArgumentException("Trying to create a rooted tree from a graph, but the provided graph is not a tree!", nameof(graph));
            }
#endif
            RootedTree tree = new RootedTree();

            Dictionary<TNode, RootedTreeNode> nodeToTreeNode = new Dictionary<TNode, RootedTreeNode>();
            foreach (TNode node in graph.Nodes(mockCounter))
            {
                RootedTreeNode newNode = new RootedTreeNode(node.ID);
                nodeToTreeNode[node] = newNode;
                tree.AddNode(newNode, mockCounter);
            }

            if (graph.NumberOfEdges(mockCounter) == 0)
            {
                return (tree, nodeToTreeNode);
            }

            IEnumerable<TEdge> edges = graph.Edges(mockCounter);
            TEdge firstGraphEdge = edges.First();
            edges = edges.Skip(1);
            RootedTreeNode node1 = nodeToTreeNode[firstGraphEdge.Endpoint1];
            RootedTreeNode node2 = nodeToTreeNode[firstGraphEdge.Endpoint2];
            Edge<RootedTreeNode> firstEdge = new Edge<RootedTreeNode>(node1, node2);
            tree.AddEdge(firstEdge, mockCounter);

            Queue<RootedTreeNode> queue = new Queue<RootedTreeNode>();
            queue.Enqueue(node1);
            queue.Enqueue(node2);

            while (queue.Count > 0)
            {
                RootedTreeNode node = queue.Dequeue();
                IEnumerable<RootedTreeNode> children = edges.Where(n => n.Endpoint1.ID == node.ID || n.Endpoint2.ID == node.ID).Select(n => n.Endpoint1.ID == node.ID ? n.Endpoint2 : n.Endpoint1).Select(n => nodeToTreeNode[n]);
                edges = edges.Where(n => n.Endpoint1.ID != node.ID && n.Endpoint2.ID != node.ID).ToList();
                
                foreach (RootedTreeNode child in children)
                {
                    Edge<RootedTreeNode> edge = new Edge<RootedTreeNode>(node, child);
                    tree.AddEdge(edge, mockCounter);
                    queue.Enqueue(child);
                }
            }

            tree.UpdateNodeTypes();

#if !EXPERIMENT
            if (!tree.IsValid())
            {
                throw new Exception("The conversion from a graph to a rooted tree went wrong!");
            }
#endif
            return (tree, nodeToTreeNode);
        }

        /// <summary>
        /// Create a <see cref="AbstractGraph{TEdge, TNode}"/> with the given number of nodes and all edges in <paramref name="edges"/>.
        /// </summary>
        /// <param name="numberOfNodes">The required number of nodes in the <see cref="AbstractGraph{TEdge, TNode}"/>.</param>
        /// <param name="edges"><see cref="IEnumerable{T}"/> with tuples of <see cref="int"/>s that represent the identifiers of the endpoints of the edges.</param>
        /// <returns>A <see cref="AbstractGraph{TEdge, TNode}"/> with <paramref name="numberOfNodes"/> nodes and an edge for each element in <paramref name="edges"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="edges"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfNodes"/> is negative.</exception>
        /// <exception cref="ArgumentException">Thrown when there are not <paramref name="numberOfNodes"/> - 1 edges, or when <paramref name="edges"/> contains an identifier that is negative, or equal to or larger than <paramref name="numberOfNodes"/>.</exception>
        public static Graph CreateGraphWithEdges(int numberOfNodes, IEnumerable<(int, int)> edges)
        {
#if !EXPERIMENT
            NullCheck(edges, nameof(edges), "Trying to create a tree with a given list of int tuples as edges, but the list with int tuples is null!");
            if (numberOfNodes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfNodes), $"Trying to create a tree with a given list of edges, but the number of nodes in the tree is negative ({numberOfNodes})!");
            }
            if (edges.Count() != numberOfNodes - 1)
            {
                throw new ArgumentException($"Trying to create a tree with a given list of edges, but the number of edges in the tree is not equal to the number of nodes - 1 ({numberOfNodes} nodes, {edges.Count()} edges)!", nameof(edges));
            }
            foreach ((int, int) edge in edges)
            {
                if (edge.Item1 < 0 || edge.Item1 >= numberOfNodes)
                {
                    throw new ArgumentException($"Trying to create a tree with a given list of edges, but an edge has an invalid endpoint (should be in range 0..{numberOfNodes - 1}, but is {edge.Item1})!", nameof(edges));
                }
                if (edge.Item2 < 0 || edge.Item2 >= numberOfNodes)
                {
                    throw new ArgumentException($"Trying to create a tree with a given list of edges, but an edge has an invalid endpoint (should be in range 0..{numberOfNodes - 1}, but is {edge.Item2})!", nameof(edges));
                }
            }
#endif
            Counter mockCounter = new Counter();

            Graph graph = new Graph();

            Node[] nodes = new Node[numberOfNodes];
            for (uint i = 0; i < numberOfNodes; i++)
            {
                Node node = new Node(i);
                nodes[i] = node;
            }

            graph.AddNode(nodes[0], mockCounter);

            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(nodes[0]);

            while (queue.Count > 0)
            {
                Node node = queue.Dequeue();
                IEnumerable<Node> neighbours = edges.Where(n => n.Item1 == node.ID || n.Item2 == node.ID).Select(n => n.Item1 == node.ID ? n.Item2 : n.Item1).Select(n => nodes[n]);
                edges = edges.Where(n => n.Item1 != node.ID && n.Item2 != node.ID).ToList();
                foreach (Node neighbour in neighbours)
                {
                    if (!graph.HasNode(neighbour, mockCounter))
                    {
                        graph.AddNode(neighbour, mockCounter);
                    }
                    Edge<Node> edge = new Edge<Node>(node, neighbour);
                    graph.AddEdge(edge, mockCounter);
                    queue.Enqueue(neighbour);
                }
            }

            graph.UpdateNodeTypes();

            return graph;
        }

        /// <summary>
        /// Create a set of <see cref="DemandPair"/>s in <paramref name="tree"/> with endpoints identified by the <see cref="int"/> tuples in <paramref name="endpoints"/>.
        /// </summary>
        /// <param name="tree">The <see cref="AbstractGraph{TEdge, TNode}"/> in which to create the <see cref="DemandPair"/>s.</param>
        /// <param name="endpoints">The <see cref="IEnumerable{T}"/> with <see cref="int"/> tuples that represent the IDs of the <see cref="Node"/>s that are the endpoints of that <see cref="DemandPair"/>.</param>
        /// <returns>A <see cref="CountedCollection{T}"/> with <see cref="DemandPair"/>s.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tree"/> or <paramref name="endpoints"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when a tuple in <paramref name="endpoints"/> has a value that is not the ID of a <see cref="Node"/> in <paramref name="tree"/>.</exception>
        public static CountedCollection<DemandPair> CreateDemandPairs(Graph tree, IEnumerable<(int, int)> endpoints)
        {
#if !EXPERIMENT
            NullCheck(tree, nameof(tree), "Trying to create demand pairs given a list with int endpoints, but the tree is null!");
            NullCheck(endpoints, nameof(endpoints), "Trying to create demand pairs given a list with int endpoints, but the list with endpoints is null!");
            int nrNodes = tree.NumberOfNodes(new Counter());
            foreach ((int, int) dp in endpoints)
            {
                if (dp.Item1 < 0)
                {
                    throw new ArgumentException($"Trying to create demand pairs given a list with int endpoints, but a demand pair has an invalid endpoint (should be in range 0..{nrNodes - 1}, but is {dp.Item1})!", nameof(endpoints));
                }
                if (dp.Item2 < 0)
                {
                    throw new ArgumentException($"Trying to create demand pairs given a list with int endpoints, but a demand pair has an invalid endpoint (should be in range 0..{nrNodes - 1}, but is {dp.Item2})!", nameof(endpoints));
                }
            }
#endif
            Counter mockCounter = new Counter();
            CountedCollection<DemandPair> result = new CountedCollection<DemandPair>();

            uint id = 0;
            foreach ((int, int) dp in endpoints)
            {
                Node node1 = tree.Nodes(mockCounter).FirstOrDefault(n => n.ID == dp.Item1);
                Node node2 = tree.Nodes(mockCounter).FirstOrDefault(n => n.ID == dp.Item2);
#if !EXPERIMENT
                if (node1 is null)
                {
                    throw new ArgumentException($"Trying to create demand pairs given a list with int endpoints, but a demand pair has an invalid endpoint (it is {dp.Item1}, but is does not exist in the tree)!", nameof(endpoints));
                }
                if (node2 is null)
                {
                    throw new ArgumentException($"Trying to create demand pairs given a list with int endpoints, but a demand pair has an invalid endpoint (it is {dp.Item2}, but is does not exist in the tree)!", nameof(endpoints));
                }
#endif
                result.Add(new DemandPair(id, node1, node2, tree), mockCounter);
                id++;
            }

            return result;
        }
    }
}
