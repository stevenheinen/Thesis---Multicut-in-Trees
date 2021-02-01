// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using MulticutInTrees.Utilities;

namespace MulticutInTrees.Exceptions
{
    /// <summary>
    /// Exception that occurs when a <see cref="DemandPair"/> has length 0.
    /// </summary>
    public class ZeroLengthDemandPathException : Exception
    {
        /// <summary>
        /// Constructor for <see cref="ZeroLengthDemandPathException"/>.
        /// </summary>
        public ZeroLengthDemandPathException() : base()
        {

        }

        /// <summary>
        /// Constructor for <see cref="ZeroLengthDemandPathException"/> with a specified error message.
        /// <param name="message">The specified error message.</param>
        /// </summary>
        public ZeroLengthDemandPathException(string message) : base(message)
        {

        }

        /// <summary>
        /// Constructor for <see cref="ZeroLengthDemandPathException"/> with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The specified error message.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception.</param>
        public ZeroLengthDemandPathException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
