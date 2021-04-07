// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

namespace MulticutInTrees.InstanceGeneration
{
    /// <summary>
    /// The type of input tree used in an experiment.
    /// </summary>
    public enum InputTreeType
    {
        /// <summary>
        /// Default value. Should not be used.
        /// </summary>
        None,
        /// <summary>
        /// Random trees generated using a Prüfer sequence.
        /// </summary>
        Prufer,
        /// <summary>
        /// Randomly generated caterpillars.
        /// </summary>
        Caterpillar,
        /// <summary>
        /// Star graphs generated from Vertex Cover instances.
        /// </summary>
        VertexCover,
        /// <summary>
        /// Trees generated from CNF-SAT instances.
        /// </summary>
        CNFSAT,
        /// <summary>
        /// A fixed tree, for instance used in tests.
        /// </summary>
        Fixed
    }
}
