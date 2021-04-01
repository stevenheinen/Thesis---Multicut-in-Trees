// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using MulticutInTrees.Algorithms;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Experiments;
using MulticutInTrees.Graphs;
using MulticutInTrees.InstanceGeneration;
using MulticutInTrees.MulticutProblem;
using MulticutInTrees.Utilities;

namespace MulticutInTrees
{
    /// <summary>
    /// The entry class for the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry method for the program.
        /// </summary>
        /// <param name="args">Array with command line arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            if (args.Length == 0)
            {
                string[] split;
                
                //split = "--seed=0 --repetitions=10 --algorithm=GuoNiedermeierKernelisation --maxSolutionSize=400 --tree=Prüfer --dps=Random --nodes=5000 --demandpairs=4000.Split();
                //split = "--seed=0 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --maxSolutionSize=400 --tree=Caterpillar --dps=Random --nodes=1000 --demandpairs=700 -v".Split();
                split = "--seed=0 --repetitions=10 --algorithm=GuoNiedermeierKernelisation --tree=Prüfer --dps=Random --nodes=500 --demandpairs=350 --maxSolutionSize=28".Split();
                
                args = new string[split.Length + 2];
                split.CopyTo(args, 0);
                args[^1] = "--outputDir=D:\\Documents\\Universiteit\\Thesis\\ExperimentResults";
                args[^2] = "--instanceDir=D:\\Documents\\Universiteit\\Thesis\\Instances";
            }

            Parser parser = new Parser(p => 
            {
                p.AutoHelp = true;
                p.CaseInsensitiveEnumValues = true;
                p.CaseSensitive = false;
            });

            ParserResult<CommandLineOptions> result = parser.ParseArguments<CommandLineOptions>(args);
            result.WithParsed(ExperimentManager.RunExperiment).WithNotParsed(e => HandleParseError(result));
        }

        /// <summary>
        /// Writes the errors that occurred during the parsing of the command line arguments to the console and stops the execution of the program.
        /// </summary>
        /// <param name="parserResult">The result of the parsing of the command line arguments.</param>
        /// <exception cref="ApplicationException">Thrown to stop the program.</exception>
        private static void HandleParseError(ParserResult<CommandLineOptions> parserResult)
        {
            HelpText helpText = HelpText.AutoBuild(parserResult);
            helpText.AddEnumValuesToHelpText = true;
            helpText.AdditionalNewLineAfterOption = false;
            helpText.AddOptions(parserResult);
            Console.WriteLine(helpText);
            throw new ApplicationException("The command line arguments are not valid. Aborting.");
        }
    }
}