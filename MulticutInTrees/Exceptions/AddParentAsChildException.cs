// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿using System;

namespace MulticutInTrees.Exceptions
{
    /// <summary>
    /// Represents an error for when a node tries to add its parent as child. Subclass of <see cref="Exception"/>.
    /// </summary>
    public class AddParentAsChildException : Exception
    {
        /// <summary>
        /// Constructor for <see cref="AddNeighbourToSelfException"/>.
        /// </summary>
        public AddParentAsChildException() : base()
        {

        }

        /// <summary>
        /// Constructor for <see cref="AddParentAsChildException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The specified error message.</param>
        public AddParentAsChildException(string message) : base(message)
        {

        }

        /// <summary>
        /// Constructor for <see cref="AddParentAsChildException"/> with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The specified error message.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception.</param>
        public AddParentAsChildException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
