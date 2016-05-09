using System;
using System.Collections.Generic;
using System.Linq;

namespace Games.AI.AdversarialSearch.TicTacToe
{
    public class TicTacToeProblem : IProblem
    {
        public TicTacToeProblem(
            BoardPlayer computerPlayer = BoardPlayer.Player1,
            BoardPlayer playerWithFirstMove = BoardPlayer.Player1)
        {
            ComputerPlayer = computerPlayer;
            PlayerWithFirstMove = playerWithFirstMove;
            MaxLevel = int.MaxValue; // there is no max level
        }

        public int MaxLevel { get; set; }
        public BoardPlayer ComputerPlayer { get; set; }
        public BoardPlayer PlayerWithFirstMove { get; private set; }

        public double GetUtilityValue(IState state)
        {
            var board = (Board) state;
            var otherPlayer = ComputerPlayer == BoardPlayer.Player1 ? BoardPlayer.Player2 : BoardPlayer.Player1;

            if (board.Evaluate(ComputerPlayer))
                return 1;   // computer wins
            if (board.Evaluate(otherPlayer))
                return -1;  // computer loses
            if (board.IsFull())
                return 0;   // cats game

            throw new InvalidOperationException("Board must be a terminal state in order to evaluate a utility value.");
        }

        public bool IsTerminalState(IState state)
        {
            var board = (Board) state;
            return (board.Evaluate(BoardPlayer.Player1) ||
                    board.Evaluate(BoardPlayer.Player2) ||
                    board.IsFull());
        }

        public List<Successor> GetSuccessors(IState state)
        {
            var board = (Board) state;
            var player = GetPlayerForCurrentMove(board);

            var successors = new List<Successor>();
            var validMoves = board.GetValidMoves();
            foreach (var moveIndex in validMoves)
            {
                var move = new Move(moveIndex, player);
                var newBoard = move.Execute(board);
                newBoard.Level++;
                successors.Add(new Successor(move, newBoard));
            }

            return successors;
        }

        private BoardPlayer GetPlayerForCurrentMove(Board board)
        {
            var otherPlayer = PlayerWithFirstMove == BoardPlayer.Player1 ? BoardPlayer.Player2 : BoardPlayer.Player1;
            var playerWithFirstMoveCount = board.Count(i => i == (int) PlayerWithFirstMove);
            var otherPlayerCount = board.Count(i => i == (int) otherPlayer);

            if (playerWithFirstMoveCount == otherPlayerCount)
                return PlayerWithFirstMove;
            if (playerWithFirstMoveCount > otherPlayerCount)
                return otherPlayer;

            throw new InvalidOperationException("Invalid board configuration! Player with the first move has less moves than the other player.");
        }
    }
}
