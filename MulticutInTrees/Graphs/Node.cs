// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Implementation of a node.
    /// </summary>
    public class Node : AbstractNode<Node>
    {
        /// <summary>
        /// Constructor for a <see cref="Node"/>.
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="Node"/>.</param>
        public Node(uint id) : base(id)
        {

        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Node {ID}";
        }
    }
}
