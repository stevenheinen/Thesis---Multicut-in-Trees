// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// The type of input demand pairs used in an experiment.
    /// </summary>
    public enum InputDemandPairsType
    {
        /// <summary>
        /// Create demand pairs by uniform randomly picking two nodes.
        /// </summary>
        Random,
        /// <summary>
        /// Create demand pairs by uniform randomly picking a node, and picking a node with a certain distance from this node.
        /// </summary>
        LengthDistribution,
        /// <summary>
        /// A fixed set of demand pairs, for instance used in tests.
        /// </summary>
        Fixed
    }
}
