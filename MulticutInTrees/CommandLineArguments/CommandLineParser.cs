// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using MulticutInTrees.Experiments;

namespace MulticutInTrees.CommandLineArguments
{
    /// <summary>
    /// Class that can parse the command line arguments.
    /// </summary>
    internal static class CommandLineParser
    {
        /// <summary>
        /// Parses the given arguments to an <see cref="CommandLineOptions"/> instance, and runs the experiments corresponding to these options.
        /// </summary>
        /// <param name="args"></param>
        internal static void ParseAndExecute(IEnumerable<string> args)
        {
            Parser parser = new(p =>
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
