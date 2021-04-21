// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl).

using System;
using System.Collections.Generic;
using MulticutInTrees.CountedDatastructures;
using MulticutInTrees.Exceptions;

namespace MulticutInTrees.Graphs
{
    /// <summary>
    /// Implementation of a node in a tree.
    /// </summary>
    public class RootedTreeNode : AbstractNode<RootedTreeNode>
    {
        /// <summary>
        /// The <see cref="CountedCollection{T}"/> with children of this <see cref="RootedTreeNode"/>.
        /// </summary>
        protected CountedCollection<RootedTreeNode> InternalChildren { get; }

        /// <summary>
        /// The internal representation for the parent of this <see cref="RootedTreeNode"/>.
        /// </summary>
        protected RootedTreeNode Parent { get; set; }

        /// <summary>
        /// Constructor for a <see cref="RootedTreeNode"/>.
        /// <para>
        /// <b>Note:</b> Use this constructor when using this <see cref="RootedTreeNode"/> in combination with an <see cref="RootedTree"/>.
        /// </para>
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="RootedTreeNode"/>.</param>
        public RootedTreeNode(uint id) : base(id)
        {
            InternalChildren = new CountedCollection<RootedTreeNode>();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public RootedTreeNode GetParent(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the root of a RootedTreeNode, but the counter is null!");
#endif            
            counter++;
            return Parent;
        }

        /// <summary>
        /// The <see cref="CountedEnumerable{T}"/> with children of this <see cref="RootedTreeNode"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <returns>A <see cref="CountedEnumerable{T}"/> with all children of this <see cref="RootedTreeNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public CountedEnumerable<RootedTreeNode> Children(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the children of a RootedTreeNode, but the counter is null!");
#endif
            counter++;
            return new CountedEnumerable<RootedTreeNode>(InternalChildren.GetLinkedList(), MockCounter);
        }

        /// <summary>
        /// Set the parent of this <see cref="RootedTreeNode"/> to <paramref name="newParent"/>.
        /// </summary>
        /// <param name="newParent">The <see cref="RootedTreeNode"/> that will become the new parent of this <see cref="RootedTreeNode"/>.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        protected void SetParent(RootedTreeNode newParent, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to set the root of a RootedTreeNode, but the counter is null!");
#endif
            counter++;

            if (newParent is null)
            {
                InternalNeighbours.Remove(Parent, counter);
            }
            else
            {
                InternalNeighbours.Add(newParent, counter);
            }

            Parent = newParent;
        }

        /// <summary>
        /// The number of children this <see cref="RootedTreeNode"/> has.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <returns>An <see cref="int"/> that is equal to the number of children of this <see cref="RootedTreeNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int NumberOfChildren(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the number of children of a RootedTreeNode, but the counter is null!");
#endif
            return InternalChildren.Count(counter);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int DepthFromRoot(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the depth of a RootedTreeNode, but the counter is null!");
#endif
            if (Parent is null)
            {
                counter++;
                return 0;
            }
            return Parent.DepthFromRoot(counter) + 1;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public int HeightOfSubtree(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), "Trying to get the height of the subtree of a RootedTreeNode, but the counter is null!");
#endif
            counter++;
            if (InternalChildren.Count(counter) == 0)
            {
                return 0;
            }
            int maxDepth = 0;
            foreach (RootedTreeNode child in InternalChildren.GetCountedEnumerable(MockCounter))
            {
                maxDepth = Math.Max(maxDepth, child.HeightOfSubtree(MockCounter));
            }
            return maxDepth + 1;
        }

        /// <summary>
        /// Checks whether this <see cref="RootedTreeNode"/> is the root of the <see cref="RootedTree"/> it is in.
        /// <br/>
        /// It checks whether the parent of this <see cref="RootedTreeNode"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <returns><see langword="true"/> if this <see cref="RootedTreeNode"/> is the root of its <see cref="RootedTree"/>, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool IsRoot(Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to check if {this} is a root, but the counter is null!");
#endif
            counter++;
            return Parent is null;
        }

        /// <summary>
        /// Finds all ancestors for this <see cref="RootedTreeNode"/>. Includes this <see cref="RootedTreeNode"/> itself.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> with <see cref="RootedTreeNode"/>s that are ancestors of this <see cref="RootedTreeNode"/>. Ordered from this <see cref="RootedTreeNode"/> to the root.</returns>
        public List<RootedTreeNode> FindAllAncestors()
        {
            List<RootedTreeNode> ancestors = new() { this };
            RootedTreeNode parent = this;
            while ((parent = parent.GetParent(MockCounter)) != null)
            {
                ancestors.Add(parent);
            }
            return ancestors;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="RootedTreeNode"/>.
        /// <br/>
        /// Looks like: "RootedTreeNode [ID]", where "[ID]" is the <see cref="AbstractNode{TNode}.ID"/> of this <see cref="RootedTreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of this <see cref="RootedTreeNode"/>.</returns>
        public override string ToString()
        {
            return $"RootedTreeNode {ID}";
        }

        /// <summary>
        /// Adds <paramref name="neighbour"/> as neighbour to this <see cref="RootedTreeNode"/>.
        /// </summary>
        /// <param name="neighbour">The <see cref="RootedTreeNode"/> to be added as neighbour.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <param name="directed">Unused.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbour"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="AddNeighbourToSelfException">Thrown when <paramref name="neighbour"/> is the sa,e <see cref="RootedTree"/> as this node.</exception>
        /// <exception cref="AddParentAsChildException">Thrown when parameter <paramref name="neighbour"/> is the parent of the <see cref="RootedTreeNode"/> this method is called from.</exception>
        /// /// <exception cref="AlreadyANeighbourException">Thrown when <paramref name="neighbour"/> is already a neighbour of this <see cref="RootedTreeNode"/>.</exception>
        internal override void AddNeighbour(RootedTreeNode neighbour, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbour, nameof(neighbour), $"Trying to add a neighbour to {this}, but the neighbour is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to add {neighbour} as neighbour to {this}, but the counter is null!");
            if (neighbour == this)
            {
                throw new AddNeighbourToSelfException($"Trying to add {neighbour} as neighbour to {this}, but {neighbour} is the same node as {this}!");
            }
            if (!(Parent is null) && neighbour == Parent)
            {
                throw new AddParentAsChildException($"Trying to add the parent of {this} as child to {this}!");
            }
            if (InternalChildren.Contains(neighbour, MockCounter))
            {
                throw new AlreadyANeighbourException($"Trying to add {neighbour} as neighbour to {this}, but {neighbour} is already a neighbour of {this}!");
            }
#endif
            if (directed)
            {
                InternalChildren.Add(neighbour, counter);
                InternalNeighbours.Add(neighbour, counter);
                return;
            }

            if (neighbour.Parent is null)
            {
                neighbour.SetParent(this, counter);
                InternalChildren.Add(neighbour, counter);
                InternalNeighbours.Add(neighbour, counter);
            }
            else
            {
                SetParent(neighbour, counter);
            }
        }

        /// <summary>
        /// Remove a <see cref="RootedTreeNode"/> from the neighbours of this <see cref="RootedTreeNode"/>.
        /// </summary>
        /// <param name="neighbour">The neighbour to be removed.</param>
        /// <param name="counter"><see cref="Counter"/> to be used for this operation.</param>
        /// <param name="directed">Unused.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="neighbour"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotANeighbourException">Thrown when <paramref name="neighbour"/> is not a neighbour of this <see cref="RootedTreeNode"/>.</exception>
        internal override void RemoveNeighbour(RootedTreeNode neighbour, Counter counter, bool directed = false)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(neighbour, nameof(neighbour), $"Trying to remove a neighbour from {this}, but the neighbour is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to remove a neighbour from {this}, but the counter is null!");
            if (!InternalNeighbours.Contains(neighbour, MockCounter))
            {
                throw new NotANeighbourException($"Trying to remove {neighbour} from the neighbours of {this}, but {neighbour} is not a neighbour of {this}!");
            }
#endif
            if (neighbour == Parent)
            {
                SetParent(null, counter);
            }
            else
            {
                InternalChildren.Remove(neighbour, counter);
                InternalNeighbours.Remove(neighbour, counter);
            }

            if (!directed)
            {
                neighbour.RemoveNeighbour(this, counter, true);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="counter"/> is <see langword="null"/>.</exception>
        public bool HasChild(RootedTreeNode node, Counter counter)
        {
#if !EXPERIMENT
            Utilities.Utils.NullCheck(node, nameof(node), $"Trying to find out whether a node is a neighbour of {this}, but the node is null!");
            Utilities.Utils.NullCheck(counter, nameof(counter), $"Trying to find out whether a node is a neighbour of {this}, but the counter is null!");
#endif
            return InternalChildren.Contains(node, counter);
        }
    }
}
