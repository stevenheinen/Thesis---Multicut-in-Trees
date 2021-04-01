// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulticutInTrees.CommandLineArguments;

namespace TESTS_MulticutInTrees.CommandLineArguments
{
    [TestClass]
    public class UnitTestCommandLineParser
    {
        [TestMethod]
        public void TestCorrectArguments()
        {
            string[] split = "--seed=0 --repetitions=2 --algorithm=GuoNiedermeierKernelisation --tree=Prüfer --dps=Random --nodes=10 --demandpairs=5 --maxSolutionSize=5".Split();
            string[] args = new string[split.Length + 2];
            split.CopyTo(args, 0);
            args[^1] = $"--outputDir={Directory.GetCurrentDirectory()}";
            args[^2] = $"--instanceDir={Directory.GetCurrentDirectory()}";

            CommandLineParser.ParseAndExecute(args);
        }

        [TestMethod]
        public void TestIncorrectArguments()
        {
            string[] split = "--seed=0 --repetitions=2 --algorithm=GuoNiedermeierKernelisation --tree=DoesNotExist --dps=DoesNotExist --nodes=-5 --demandpairs=5 --maxSolutionSize=5".Split();
            string[] args = new string[split.Length + 2];
            split.CopyTo(args, 0);
            args[^1] = "--outputDir=";
            args[^2] = "--instanceDir=";

            Assert.ThrowsException<ApplicationException>(() =>
            {
                CommandLineParser.ParseAndExecute(args);
            });
        }
    }
}
