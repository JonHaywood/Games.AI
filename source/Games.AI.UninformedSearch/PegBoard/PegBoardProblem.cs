using System.Collections.Generic;
using System.Linq;

namespace Games.AI.UninformedSearch.PegBoard
{
    public class PegBoardProblem : IProblem
    {
        public PegBoardProblem(Board initialState)
        {
            InitialState = initialState;
        }

        public IState InitialState { get; private set; }

        public virtual List<Successor> GetSuccessors(IState state)
        {
            var board = (Board)state;
            var successors = new List<Successor>();
            var pegSpaces = board.GetSpacesWithPegs();

            foreach (var peg in pegSpaces)
            {
                var jumps = GetAvailableJumps(board, peg);
                if (jumps.Count > 0)                
                    successors.AddRange(jumps.Select(jump => new Successor(jump, board.ExecuteJump(jump))));                
            }
            return successors;
        }

        private List<Jump> GetAvailableJumps(Board board, Vertex peg)
        {
            var jumps = new List<Jump>();

            for (int i = 0; i < board.VertexCount; i++)
            {
                if (board.AdjacencyMatrix.HasEdge(peg.Index, i))
                {
                    var jumped = board[board.AdjacencyMatrix[peg.Index, i]];
                    var destination = board[i];
                    if (jumped.HasPeg && destination.HasPeg == false)
                        jumps.Add(new Jump(peg, jumped, destination));
                }
            }

            return jumps;
        }
    }
}
