
using System;
using Games.AI.Search;

namespace Games.AI.UninformedSearch.PegBoard
{
    /// <summary>
    /// Represents a potential move on the board.
    /// </summary>
    public class Jump : BaseObject, IAction
    {
        public Jump(Vertex current, Vertex jumped, Vertex destination)
        {
            if (current == null)
                throw new ArgumentNullException("current", "current is a required argument.");
            if (jumped == null)
                throw new ArgumentNullException("jumped", "current is a required argument.");
            if (destination == null)
                throw new ArgumentNullException("destination", "destination is a required argument.");

            Current = current;
            Jumped = jumped;
            Destination = destination;
        }
        
        [DomainSignature]
        public Vertex Current { get; private set; }
        [DomainSignature]
        public Vertex Jumped { get; private set; }
        [DomainSignature]
        public Vertex Destination { get; private set; }

        public override string ToString()
        {
            return string.Format("Peg at {0} jumps peg {1} to space {2}", Current.Index, Jumped.Index, Destination.Index);
        }
    }
}
