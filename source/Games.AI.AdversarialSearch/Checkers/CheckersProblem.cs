using System;
using System.Collections.Generic;
using System.Linq;

namespace Games.AI.AdversarialSearch.Checkers
{
    public class CheckersProblem : IProblem
    {
        public CheckersProblem(
            BoardPlayer computerPlayer = BoardPlayer.Player1,
            BoardPlayer playerWithFirstMove = BoardPlayer.Player1,
            int maxLevel = 30)
        {
            ComputerPlayer = computerPlayer;
            PlayerWithFirstMove = playerWithFirstMove;
            MaxLevel = maxLevel;
        }

        public BoardPlayer ComputerPlayer { get; set; }
        public BoardPlayer PlayerWithFirstMove { get; set; }
        public int MaxLevel { get; set; }

        public double GetUtilityValue(IState state)
        {
            var board = (Board)state;
            var otherPlayer = ComputerPlayer == BoardPlayer.Player1 ? BoardPlayer.Player2 : BoardPlayer.Player1;

            if (board.Evaluate(ComputerPlayer))
                return 1;   // computer wins
            if (board.Evaluate(otherPlayer))
                return -1;  // computer loses
            if (!board.GetValidMoves(BoardPlayer.Player1).Any() && !board.GetValidMoves(BoardPlayer.Player2).Any())
                return 0; // there are no moves left, no one wins

            throw new InvalidOperationException("Board must be a terminal state in order to evaluate a utility value.");
        }

        public bool IsTerminalState(IState state)
        {
            var board = (Board)state;            
            return (board.Evaluate(BoardPlayer.Player1) ||
                    board.Evaluate(BoardPlayer.Player2) ||
                    (
                     !board.GetValidMoves(BoardPlayer.Player1).Any() &&
                     !board.GetValidMoves(BoardPlayer.Player2).Any() // there are no valid moves left
                    ));
        }

        public List<Successor> GetSuccessors(IState state)
        {
            var board = (Board)state;
            var player = GetPlayerForCurrentMove(board);

            var successors = new List<Successor>();
            var validMoves = board.GetValidMoves(player);
            foreach (var move in validMoves)
            {
                var newBoard = move.Execute(board);
                newBoard.Level++;
                successors.Add(new Successor(move, newBoard));
            }

            return successors;
        }


        private BoardPlayer GetPlayerForCurrentMove(Board board)
        {
            if (!board.LastPlayer.HasValue)
                return PlayerWithFirstMove;

            // whoever was the last player, it's the other person's turn
            return board.LastPlayer == BoardPlayer.Player1 ? BoardPlayer.Player2 : BoardPlayer.Player1;
        }
    }
}
