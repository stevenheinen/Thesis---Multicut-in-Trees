// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;
using MulticutInTrees.MulticutProblem;

namespace MulticutInTrees.Exceptions
{
    /// <summary>
    /// Exception that occurs when a node or edge is used as if it were on the path of a <see cref="DemandPair"/> while it is not on that path.
    /// </summary>
    public class NotOnDemandPathException : Exception
    {
        /// <summary>
        /// Constructor for <see cref="NotOnDemandPathException"/>.
        /// </summary>
        public NotOnDemandPathException()
        {

        }

        /// <summary>
        /// Constructor for <see cref="NotOnDemandPathException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The specified error message.</param>
        public NotOnDemandPathException(string message) : base(message)
        {

        }

        /// <summary>
        /// Constructor for <see cref="NotOnDemandPathException"/> with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The specified error message.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception.</param>
        public NotOnDemandPathException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
