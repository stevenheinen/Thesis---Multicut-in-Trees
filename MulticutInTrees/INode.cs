// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MulticutInTrees
{
    public interface INode<N> : IEquatable<INode<N>> where N : INode<N>
    {
        // TODO: Add parent

        public int ID { get; }

        public ReadOnlyCollection<N> Neighbours { get; }

        public int Degree { get; }

        public void AddChild(N child);

        public void AddChildren(IEnumerable<N> children);

        public void RemoveChild(N child);

        public void RemoveChildren(IEnumerable<N> children);

        public void RemoveAllChildren();

        public string ToString();

        public bool HasNeighbour(N node);
    }
}