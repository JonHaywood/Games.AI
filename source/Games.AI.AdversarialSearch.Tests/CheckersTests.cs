using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Games.AI.AdversarialSearch;
using Games.AI.AdversarialSearch.Checkers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Games.AI.Tests
{
    [TestClass]
    public class CheckersTests
    {
        public const string Category = "Checkers";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod, TestCategory(Category)]
        public void Board_PrintsCorrectly()
        {
            var board = new Board(true);
            var output = new BoardPrinter().Print(board);
            WriteOutputFile(output);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_EqualsWorksCorrectly()
        {
            var board1 = new Board(true);
            var board2 = board1.Clone();

            Assert.AreEqual(board1, board2);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_HashCodeWorksCorrectly()
        {
            var board1 = new Board(true);
            var board2 = (Board)board1.Clone();
            var board3 = new Board();
            var expected = 1;
            var dictionary = new Dictionary<Board, int>
            {
                {board1, expected}
            };

            int value;
            var result1 = dictionary.TryGetValue(board2, out value); // board 1 and 2 are the equivalent, should be able get value out using either
            var result2 = dictionary.TryGetValue(board3, out value); // board 3 is different, should NOT be able to get value

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
            Assert.AreEqual(expected, dictionary[board2]);         
        }

        [TestMethod, TestCategory(Category)]
        public void Board_SquareChessCoordinateIsCorrect()
        {
            var board = new Board(true);
            var a1 = board["a3"];
            Assert.AreEqual(a1.Coordinate, "a3");
            Assert.AreEqual(a1.Column, 0);
            Assert.AreEqual(a1.Row, 2);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMoves_AreCorrectForPlayer1()
        {
            var board = new Board(true);
            var moves = board.GetValidMoves(BoardPlayer.Player1);

            Assert.IsNotNull(moves);
            Assert.AreEqual(moves.Count(), 7); // there are 7 available moves at a start board
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMoves_AreCorrectForPlayer2()
        {
            var board = new Board(true);
            var moves = board.GetValidMoves(BoardPlayer.Player2);

            Assert.IsNotNull(moves);
            Assert.AreEqual(moves.Count(), 7); // there are 7 available moves at a start board
        }

        [TestMethod, TestCategory(Category)]
        public void Board_EvaluatePlayer1_ReturnsFalseForStartBoard()
        {
            var board = new Board(true);
            var result = board.Evaluate(BoardPlayer.Player1);

            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_EvaluatePlayer2_ReturnsFalseForStartBoard()
        {
            var board = new Board(true);
            var result = board.Evaluate(BoardPlayer.Player2);

            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_IsExecutedCorrectly()
        {
            var board = new Board(true);
            var move = board.GetValidMoves(BoardPlayer.Player1).First();
            var newBoard = move.Execute(board);
            
            Assert.AreNotEqual(board, newBoard);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsSingleOpenMoveCorrectly()
        {
            var board = new Board();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);            

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(1, validMoves.Count);
            Assert.AreEqual("b2", validMoves[0].MoveTo);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsMultipleOpenMovesCorrectly()
        {
            var board = new Board();
            board["b2"].Piece = new Piece(BoardPlayer.Player1);
            board["c3"].Piece = new Piece(BoardPlayer.Player2);

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(2, validMoves.Count);            
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "a3"));            
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "d4"));
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsAvailableJumpCorrectly()
        {
            var board = new Board();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);
            board["b2"].Piece = new Piece(BoardPlayer.Player2);

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(1, validMoves.Count);
            Assert.AreEqual("c3", validMoves[0].MoveTo);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsDoubleJumpCorrectly()
        {
            var board = new Board();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);
            board["b2"].Piece = new Piece(BoardPlayer.Player2);
            board["d4"].Piece = new Piece(BoardPlayer.Player2);

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(2, validMoves.Count);   
            Assert.IsTrue(validMoves.Any(m => m is Jump));
            Assert.IsTrue(validMoves.Any(m => m is MultipleJump));

            var multipleJump = (MultipleJump)validMoves.First(m => m is MultipleJump);
            Assert.AreEqual("c3", multipleJump.Jumps[0].MoveTo);
            Assert.AreEqual("e5", multipleJump.Jumps[1].MoveTo);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_ExecutesDoubleJumpCorrectly()
        {
            var board = new Board();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);
            board["b2"].Piece = new Piece(BoardPlayer.Player2);
            board["d4"].Piece = new Piece(BoardPlayer.Player2);

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();
            var move = validMoves.First(m => m is MultipleJump);
            var newBoard = move.Execute(board);

            var squares = newBoard.Where(s => s.HasPiece).ToList();
            Assert.AreEqual(1, squares.Count);
            Assert.AreEqual("e5", squares[0].Coordinate);
            Assert.AreEqual(BoardPlayer.Player1, squares[0].Piece.Player);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsKingMovesCorrectly()
        {
            var board = new Board();
            board["b2"].Piece = new Piece(BoardPlayer.Player1) { IsKing = true };
            board["c3"].Piece = new Piece(BoardPlayer.Player2);

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(4, validMoves.Count);
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "a1"));
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "a3"));
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "c1"));
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "d4"));
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_AtEndOfBoardBecomesKing()
        {
            var board = new Board();
            board["a7"].Piece = new Piece(BoardPlayer.Player1);            

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();
            var newBoard = validMoves[0].Execute(board);

            Assert.IsTrue(newBoard["b8"].Piece.IsKing);
        }        

        [TestMethod, TestCategory(Category)]
        public void Board_Evaluate_ScenarioWithPlayer1Win()
        {
            var board = new Board();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);

            var player1Result = board.Evaluate(BoardPlayer.Player1);
            var player2Result = board.Evaluate(BoardPlayer.Player2);

            Assert.IsTrue(player1Result);
            Assert.IsFalse(player2Result);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_Evaluate_ScenarioWithPlayer2Win()
        {
            var board = new Board();
            board["a1"].Piece = new Piece(BoardPlayer.Player2);

            var player1Result = board.Evaluate(BoardPlayer.Player1);
            var player2Result = board.Evaluate(BoardPlayer.Player2);

            Assert.IsFalse(player1Result);
            Assert.IsTrue(player2Result);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_NearEndOfGame_AlgorithmFindsSolution()
        {
            var board = new Board();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);
            board["b2"].Piece = new Piece(BoardPlayer.Player2);
            board["h4"].Piece = new Piece(BoardPlayer.Player2);
            board["f2"].Piece = new Piece(BoardPlayer.Player1);

            IProblem problem = new CheckersProblem();
            IAlgorithm algorithm = new MinimaxWithAlphaBetaPruningAlgorithm{UseTranspositionTable = true};

            var result = algorithm.SolveForBestAction(problem, board);
            var move = (Move)result.Action;

            Assert.IsTrue(result.Solved);
            Assert.IsNotNull(move);
            Debug.WriteLine(result.Statistics);
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
