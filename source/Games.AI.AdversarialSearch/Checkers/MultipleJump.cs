using System;
using System.Collections.Generic;
using System.Linq;

namespace Games.AI.AdversarialSearch.Checkers
{
    public class MultipleJump : Move
    {
        [DomainSignature]
        public List<Jump> Jumps { get; set; }

        public override Board Execute(Board board)
        {
            if (Jumps.Count <= 1)
                throw new InvalidOperationException(string.Format("Multiple jump move has {0} elements. Must have at least 2 to be multiple jump.", Jumps.Count));
            
            // perform jumps
            foreach (var jump in Jumps)
                board = jump.Execute(board);

            return board;
        }

        public override string ToString()
        {
            return string.Join(" ", Jumps.Select(j => j.ToString()));
        }
    }
}
