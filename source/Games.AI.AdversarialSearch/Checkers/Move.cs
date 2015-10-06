
using System;

namespace Games.AI.AdversarialSearch.Checkers
{
    /// <summary>
    /// Represents a move on the board.
    /// </summary>
    public class Move : BaseObject, IAction
    {
        [DomainSignature]
        public string At { get; set; }

        [DomainSignature]
        public string MoveTo { get; set; }

        public virtual Board Execute(Board board)
        {
            if (board[MoveTo].Piece != null)
                throw new InvalidOperationException(string.Format("Cannot execute move to '{0}'. Space is occupied by {1}.", MoveTo, board[MoveTo].Piece.Player));

            // moves the player at the current space
            // to the new space
            var newBoard = (Board)board.Clone();
            newBoard[MoveTo].Piece = newBoard[At].Piece;
            newBoard[At].Piece = null;

            // if the piece has reached the opponents side 
            // then it is a king
            if (newBoard[MoveTo].Piece.Player == BoardPlayer.Player1 && newBoard[MoveTo].Row == Board.BoardSize - 1)
                newBoard[MoveTo].Piece.IsKing = true;
            if (newBoard[MoveTo].Piece.Player == BoardPlayer.Player2 && newBoard[MoveTo].Row == 0)
                newBoard[MoveTo].Piece.IsKing = true;

            // the piece that moved is the last player
            newBoard.LastPlayer = board[At].Piece.Player;

            return newBoard;
        }
    }
}
