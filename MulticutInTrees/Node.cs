// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

﻿// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace MulticutInTrees
{
    // TODO: Comments

    public class Node : INode<Node>
    {
        public Node(int id)
        {
            ID = id;
            InternalNeighbours = new List<Node>();
            InternalUniqueNeighbours = new HashSet<Node>();
        }

        public Node(int id, IEnumerable<Node> neighbours)
        {
            ID = id;

            if (neighbours is null)
            {
                InternalNeighbours = new List<Node>();
                InternalUniqueNeighbours = new HashSet<Node>();
            }
            else
            {
                InternalNeighbours = new List<Node>(neighbours);
                InternalUniqueNeighbours = new HashSet<Node>(neighbours);
            }
        }

        private List<Node> InternalNeighbours { get; }

        private HashSet<Node> InternalUniqueNeighbours { get; }

        // TODO: Add parent

        public int ID { get; set; }

        public ReadOnlyCollection<Node> Neighbours => InternalNeighbours.AsReadOnly();

        public int Degree => InternalNeighbours.Count;

        public void AddChild(Node child)
        {
            // TODO: Add this node as parent of the child

            if (child is null)
            {
                throw new Exception($"Trying to add a child to {this}, but child is null!");
            }
            if (child == this)
            {
                throw new Exception($"Trying to add {child} as a child to itself!");
            }
            if (InternalUniqueNeighbours.Contains(child))
            {
                throw new Exception($"Trying to add {child} as a neighbour to {this}, but {child} is already a neighbour of {this}!");
            }
            InternalUniqueNeighbours.Add(child);
            InternalNeighbours.Add(child);
        }

        public void AddChildren(IEnumerable<Node> children)
        {
            if (children is null)
            {
                throw new Exception($"Trying to add a list of children to {this}, but the list is null!");
            }
            foreach (var child in children) AddChild(child);
        }

        public void RemoveAllChildren()
        {
            // TODO: remove this node as parent of its children

            InternalUniqueNeighbours.Clear();
            InternalNeighbours.Clear();
        }

        public void RemoveChild(Node child)
        {
            // TODO: remove this node as parent of the child

            if (child is null)
            {
                throw new Exception($"Trying to remove a child from {this}, but child is null!");
            }
            if (!InternalUniqueNeighbours.Contains(child))
            {
                throw new Exception($"Trying to remove {child} from {this}'s neighbours, but {child} is no neighbour of {this}!");
            }
            InternalUniqueNeighbours.Remove(child);
            InternalNeighbours.Remove(child);
        }

        public void RemoveChildren(IEnumerable<Node> children)
        {
            if (children is null)
            {
                throw new Exception($"Trying to remove a list of children from {this}, but the list is null!");
            }
            foreach (var child in children) RemoveChild(child);
        }

        public override string ToString()
        {
            return $"Node {ID}";
        }

        public bool HasNeighbour(Node node)
        {
            return InternalUniqueNeighbours.Contains(node);
        }

        public bool Equals([AllowNull] INode<Node> other)
        {
            if (other is null)
            {
                throw new Exception($"Trying to compare {this} to null!");
            }
            return ID.CompareTo(other.ID) == 0;
        }

        public static bool operator ==(Node l, Node r)
        {
            if (l is null)
            {
                throw new Exception("Left hand side of == operator for Node is null!");
            }
            if (r is null)
            {
                throw new Exception("Right hand side of == operator for Node is null!");
            }
            return l.Equals(r);
        }

        public static bool operator !=(Node l, Node r)
        {
            if (l is null)
            {
                throw new Exception("Left hand side of != operator for Node is null!");
            }
            if (r is null)
            {
                throw new Exception("Right hand side of != operator for Node is null!");
            }
            return !l.Equals(r);
        }
    }
}