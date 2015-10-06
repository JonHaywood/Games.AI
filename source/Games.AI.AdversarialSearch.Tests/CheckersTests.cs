﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Games.AI.AdversarialSearch.Checkers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Games.AI.AdversarialSearch.Tests
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
            var board = new Board();
            var output = new BoardPrinter().Print(board);
            WriteOutputFile(output);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_EqualsWorksCorrectly()
        {
            var board1 = new Board();
            var board2 = board1.Clone();

            Assert.AreEqual(board1, board2);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_SquareChessCoordinateIsCorrect()
        {
            var board = new Board();
            var a1 = board["a3"];
            Assert.AreEqual(a1.Coordinate, "a3");
            Assert.AreEqual(a1.Column, 0);
            Assert.AreEqual(a1.Row, 2);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMoves_AreCorrectForPlayer1()
        {
            var board = new Board();
            var moves = board.GetValidMoves(BoardPlayer.Player1);

            Assert.IsNotNull(moves);
            Assert.AreEqual(moves.Count(), 7); // there are 7 available moves at a start board
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMoves_AreCorrectForPlayer2()
        {
            var board = new Board();
            var moves = board.GetValidMoves(BoardPlayer.Player2);

            Assert.IsNotNull(moves);
            Assert.AreEqual(moves.Count(), 7); // there are 7 available moves at a start board
        }

        [TestMethod, TestCategory(Category)]
        public void Board_EvaluatePlayer1_ReturnsFalseForStartBoard()
        {
            var board = new Board();
            var result = board.Evaluate(BoardPlayer.Player1);

            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_EvaluatePlayer2_ReturnsFalseForStartBoard()
        {
            var board = new Board();
            var result = board.Evaluate(BoardPlayer.Player2);

            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_IsExecutedCorrectly()
        {
            var board = new Board();
            var move = board.GetValidMoves(BoardPlayer.Player1).First();
            var newBoard = move.Execute(board);
            
            Assert.AreNotEqual(board, newBoard);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsSingleOpenMoveCorrectly()
        {
            var board = Board.CreateEmptyBoard();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);            

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(1, validMoves.Count);
            Assert.AreEqual("b2", validMoves[0].MoveTo);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsMultipleOpenMovesCorrectly()
        {
            var board = Board.CreateEmptyBoard();
            board["b2"].Piece = new Piece(BoardPlayer.Player1);
            board["c3"].Piece = new Piece(BoardPlayer.Player2);

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(2, validMoves.Count);            
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "a3"));            
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "d4"));
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsJumpCorrectly()
        {
            var board = Board.CreateEmptyBoard();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);
            board["b2"].Piece = new Piece(BoardPlayer.Player2);

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(1, validMoves.Count);
            Assert.AreEqual("c3", validMoves[0].MoveTo);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsDoubleJumpCorrectly()
        {
            var board = Board.CreateEmptyBoard();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);
            board["b2"].Piece = new Piece(BoardPlayer.Player2);
            board["d4"].Piece = new Piece(BoardPlayer.Player2);

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();

            Assert.AreEqual(2, validMoves.Count);
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "c3"));
            Assert.IsTrue(validMoves.Any(m => m.MoveTo == "e5"));
        }

        [TestMethod, TestCategory(Category)]
        public void Board_ValidMove_FindsKingMovesCorrectly()
        {
            var board = Board.CreateEmptyBoard();
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
            var board = Board.CreateEmptyBoard();
            board["a7"].Piece = new Piece(BoardPlayer.Player1);            

            var validMoves = board.GetValidMoves(BoardPlayer.Player1).ToList();
            var newBoard = validMoves[0].Execute(board);

            Assert.IsTrue(newBoard["b8"].Piece.IsKing);
        }        

        [TestMethod, TestCategory(Category)]
        public void Board_Evaluate_ScenarioWithPlayer1Win()
        {
            var board = Board.CreateEmptyBoard();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);

            var player1Result = board.Evaluate(BoardPlayer.Player1);
            var player2Result = board.Evaluate(BoardPlayer.Player2);

            Assert.IsTrue(player1Result);
            Assert.IsFalse(player2Result);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_Evaluate_ScenarioWithPlayer2Win()
        {
            var board = Board.CreateEmptyBoard();
            board["a1"].Piece = new Piece(BoardPlayer.Player2);

            var player1Result = board.Evaluate(BoardPlayer.Player1);
            var player2Result = board.Evaluate(BoardPlayer.Player2);

            Assert.IsFalse(player1Result);
            Assert.IsTrue(player2Result);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_NearEndOfGame_AlgorithmFindsSolution()
        {
            var board = Board.CreateEmptyBoard();
            board["a1"].Piece = new Piece(BoardPlayer.Player1);
            board["b2"].Piece = new Piece(BoardPlayer.Player2);
            board["h4"].Piece = new Piece(BoardPlayer.Player2);
            board["f2"].Piece = new Piece(BoardPlayer.Player1);

            IProblem problem = new CheckersProblem(computerPlayer: BoardPlayer.Player1);
            IAlgorithm algorithm = new IterativeDeepeningWithABPruningAlgorithm(50);

            var result = algorithm.SolveForBestAction(problem, board);
            var move = (Move)result.Action;

            Assert.IsNotNull(move);
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
