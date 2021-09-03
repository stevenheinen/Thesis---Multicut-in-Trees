// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// The type of <see cref="Algorithm"/> to run in the current experiment.
    /// </summary>
    public enum AlgorithmType
    {
        /// <summary>
        /// Default value. Should not be used.
        /// </summary>
        None,
        /// <summary>
        /// No algorithm, just generating instances.
        /// </summary>
        GenerateInstances,
        /// <summary>
        /// The MIP solver used to find the minimum possible solution size.
        /// </summary>
        GurobiMIPSolver,
        /// <summary>
        /// Brute force algorithm that checks all possible subsets of edges of a certain size.
        /// </summary>
        BruteForce,
        /// <summary>
        /// The branching algorithm by Guo and Niedermeier.
        /// </summary>
        GuoNiederMeierBranching,
        /// <summary>
        /// The kernelisation algorithm by Guo and Niedermeier that finds a kernel of size O(k^{3k}).
        /// </summary>
        GuoNiedermeierKernelisation,
        /// <summary>
        /// The kernelisation algorithm by Bousquet et al. that finds a kernel of size O(k^6).
        /// </summary>
        BousquetKernelisation,
        /// <summary>
        /// The kernelisation algorithm by Chen et al. that finds a kernel of size O(k^3).
        /// </summary>
        ChenKernelisation,
    }
}
