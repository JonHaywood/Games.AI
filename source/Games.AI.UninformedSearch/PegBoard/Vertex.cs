using System;

namespace Games.AI.UninformedSearch.PegBoard
{
    /// <summary>
    /// Represents a position on the game board. Consists of a coordinate
    /// and a boolean indicating if the coordinate has a peg or not.
    /// Is immutable.
    /// </summary>
    public class Vertex : BaseObject, ICloneable
    {  
        public Vertex(int index, bool hasPeg = false)
        {
            Index = index;
            HasPeg = hasPeg;
        }

        /// <summary>
        /// Gets the index for the vertex.
        /// </summary>
        [DomainSignature]
        public int Index { get; }

        /// <summary>
        /// Gets whether this coordinate has a peg or not.
        /// </summary>
        [DomainSignature]
        public bool HasPeg { get; }

        /// <summary>
        /// Returns a new vertex with the peg set to the specified value.
        /// </summary>
        public Vertex SetPeg(bool hasPeg)
        {
            return new Vertex(Index, hasPeg);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Vertex(Index, HasPeg);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return $"Index {Index}, Has peg: {HasPeg}";
        }
    }
}
