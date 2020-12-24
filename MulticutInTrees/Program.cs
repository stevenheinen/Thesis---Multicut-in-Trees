// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using MulticutInTrees.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

namespace MulticutInTrees
{
    /// <summary>
    /// The entry class for the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The global <see cref="System.Random"/> used throughout the entire program.
        /// </summary>
        public readonly static Random Random = new Random();

        /// <summary>
        /// The entry method for the program.
        /// </summary>
        public static void Main()
        {
            Console.WriteLine("Hello World!");
        }
    }
}