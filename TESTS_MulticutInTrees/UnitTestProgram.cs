// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees;

namespace TESTS_MulticutInTrees
{
    [TestClass]
    public class UnitTestProgram
    {
        [TestMethod]
        public void TestMain()
        {
            string[] split = "--seed=0 --repetitions=3 --experiments=5 --algorithm=ImprovedGuoNiedermeierKernelisation --treeType=Prufer --dpType=Random --nrNodes=50 --nrDPs=35 --maxSolutionSize=0 -v".Split();
            string[] args = new string[split.Length + 2];
            split.CopyTo(args, 0);
            args[^1] = "--outputDir=D:\\Documents\\Universiteit\\Thesis\\ExperimentResults";
            args[^2] = "--instanceDir=D:\\Documents\\Universiteit\\Thesis\\Instances";

            Program.Main(args);
        }
    }
}