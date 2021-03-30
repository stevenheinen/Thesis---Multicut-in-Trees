// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

namespace MulticutInTrees.Algorithms
{
    /// <summary>
    /// The type of <see cref="Algorithm"/> to run in the current experiment.
    /// </summary>
    public enum AlgorithmType
    {
        /// <summary>
        /// The branching algorithm by Guo and Niedermeier.
        /// </summary>
        GuoNiederMeierBranching,
        /// <summary>
        /// The kernelisation algorithm by Guo and Niedermeier that finds a kernel of size O(k^{3k}).
        /// </summary>
        GuoNiedermeierKernelisation,
    }
}
