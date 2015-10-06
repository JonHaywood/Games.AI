using System;
using System.IO;
using System.Linq;
using System.Text;
using Games.AI.AdversarialSearch.TicTacToe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Games.AI.AdversarialSearch.Tests
{
    [TestClass]
    public class TicTacToeTests
    {
        public const string Category = "TicTacToe";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod, TestCategory(Category)]
        public void Board_PrintsCorrectly()
        {
            var board = new Board();
            board[0, 0] = BoardPlayer.Player1;
            board[1, 0] = BoardPlayer.Player2;
            board[2, 0] = BoardPlayer.Player1;

            WriteOutputFile(board.ToString());
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMoves_AreCorrectForEmptyBoard()
        {
            var board = new Board();
            var validMoves = board.GetValidMoves();

            Assert.AreEqual(9, validMoves.Count());
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMoves_AreCorrectAfterOneMove()
        {
            var board = new Board();
            board[0, 0] = BoardPlayer.Player1;

            var validMoves = board.GetValidMoves().ToList();

            Assert.AreEqual(8, validMoves.Count);
            Assert.IsFalse(validMoves.Any(move => move == 0)); // the move we made should not be valid
        }

        [TestMethod, TestCategory(Category)]
        public void Board_BoardWithTwoMovesLeft_AlgorithmShouldPickRightMove()
        {
            var output = new StringBuilder();

            var board = new Board();
            board[0, 0] = BoardPlayer.Player1;
            board[1, 0] = BoardPlayer.Player2;
            board[2, 0] = BoardPlayer.Player1;
            board[0, 1] = BoardPlayer.Player2;
            board[1, 1] = BoardPlayer.Player2;
            board[0, 2] = BoardPlayer.Player1;
            board[1, 2] = BoardPlayer.Player1;           

            IProblem problem = new TicTacToeProblem(computerPlayer: BoardPlayer.Player2);
            IAlgorithm algorithm = new MinimaxWithAlphaBetaPruningAlgorithm();

            var result = algorithm.SolveForBestAction(problem, board);
            var action = (Move) result.Action;

            Console.WriteLine("Best action: {0}", action.Index);
            output.AppendLine("Current board:");
            output.AppendLine(board.ToString());
            output.AppendLine("Recommended action:");
            output.AppendLine(action.Execute(board).ToString());
            WriteOutputFile(output);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_StartBoard_AlgorithmAgainstRandomizedOpponentShouldWinOrDraw()
        {            
            var board = new Board();
            var randomizer = new Random();
            var randomPlayer = BoardPlayer.Player1;
            var computerPlayer = BoardPlayer.Player2;

            IProblem problem = new TicTacToeProblem(computerPlayer: computerPlayer);
            IAlgorithm algorithm = new MinimaxWithAlphaBetaPruningAlgorithm();

            while (!problem.IsTerminalState(board))
            {
                // random player makes a move
                var validMoves = board.GetValidMoves().ToArray();
                var randomMove = randomizer.Next(validMoves.Length);
                var move = new Move(validMoves[randomMove], randomPlayer);
                board = move.Execute(board);

                // board could be full here
                if (problem.IsTerminalState(board))
                    break;

                // solve board for best opposing move
                var result = algorithm.SolveForBestAction(problem, board);
                var action = (Move)result.Action;
                board = action.Execute(board);
            }

            bool didComputerWinOrDraw = board.Evaluate(computerPlayer) || board.IsFull();
            WriteOutputFile(board.ToString());
            Assert.IsTrue(didComputerWinOrDraw);
        }

        private void WriteOutputFile(StringBuilder builder)
        {
            WriteOutputFile(builder.ToString());
        }

        private void WriteOutputFile(string contents)
        {
            string file = Path.Combine(TestContext.TestResultsDirectory, "output.txt");
            File.WriteAllText(file, contents);
            TestContext.AddResultFile(file);
        }
    }
}
