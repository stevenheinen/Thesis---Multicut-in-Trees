// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// The type of a <see cref="Node"/>.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// Internal node that has exactly one other internal node as neighbour, and at least one leaf.
        /// </summary>
        I1,
        /// <summary>
        /// Internal node that has exactly two other internal nodes as neighbours, and any number of leaves.
        /// </summary>
        I2,
        /// <summary>
        /// Internal node that has at least three other internal nodes as neighbours, and any number of leaves.
        /// </summary>
        I3,
        /// <summary>
        /// Leaf with an <see cref="I1"/>-node as parent.
        /// </summary>
        L1,
        /// <summary>
        /// Leaf with an <see cref="I2"/>-node as parent.
        /// </summary>
        L2,
        /// <summary>
        /// Leaf with an <see cref="I3"/>-node as parent.
        /// </summary>
        L3,
        /// <summary>
        /// The <see cref="NodeType"/> of this <see cref="Node"/> is not yet defined. This should not occur in non-trivial graphs.
        /// </summary>
        Other
    };
}
