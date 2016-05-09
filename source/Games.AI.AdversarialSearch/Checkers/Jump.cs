
using System;

namespace Games.AI.AdversarialSearch.Checkers
{
    /// <summary>
    /// Represents a jump on the board.
    /// </summary>
    public class Jump : Move
    {     
        [DomainSignature]
        public string Jumped { get; set; }

        public override Board Execute(Board board)
        {
            if (board[Jumped].Piece == null)
                throw new InvalidOperationException(string.Format(
                    "Cannot execute jump over '{0}'. Jumped space empty. It must held by a different player.", Jumped));
            if (board[Jumped].Piece == board[At].Piece)
                throw new InvalidOperationException(string.Format(
                    "Cannot execute jump over '{0}'. Jumped space is occupied by {1}. It must not be empty and held by a different player.",
                    Jumped, board[Jumped].Piece.Player));

            var newBoard = base.Execute(board);
            newBoard[Jumped].Piece = null;
            return newBoard;
        }

        public override string ToString()
        {
            return string.Format("Piece at {0} will jump over {1} and move to {2}.", At, Jumped, MoveTo);
        }
    }
}
