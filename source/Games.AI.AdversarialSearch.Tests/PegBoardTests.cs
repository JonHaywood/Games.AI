using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Games.AI.UninformedSearch;
using Games.AI.UninformedSearch.PegBoard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Games.AI.Tests
{
    [TestClass]
    public class PegBoardTests
    {
        public const string Category = "PegBoard";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod, TestCategory(Category)]
        public void Board_PrintsCorrectly()
        {
            var board = new Board();

            WriteOutputFile(board.ToString());
        }

        [TestMethod, TestCategory(Category)]
        public void Board_EqualsWorksCorrectly()
        {
            var board1 = new Board();
            var board2 = new Board();
            var board3 = new Board();
            board3 = board3.ExecuteJump(new Jump(board3[3], board3[1], board3[0]));

            Assert.AreEqual(board1, board2);
            Assert.AreNotEqual(board1, board3);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_HashCodeWorksCorrectly()
        {
            var board1 = new Board();
            var board2 = new Board();
            var board3 = new Board();
            board3 = board3.ExecuteJump(new Jump(board3[3], board3[1], board3[0]));
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
        public void EmptyBoard_ConstructedCorrectly()
        {
            var board = Board.Empty;
            Assert.AreEqual(0, board.PegCount);
        }

        [TestMethod, TestCategory(Category)]
        public void Board_Rotate_WorksCorrectly()
        {
            var boardAt10 = NewBoardWithPegAt(10);
            var boardAt14 = NewBoardWithPegAt(14);

            var board = NewBoardWithPegAt(0).Rotate();
            Assert.AreEqual(boardAt10, board);

            board = board.Rotate();
            Assert.AreEqual(boardAt14, board);
        }

        [TestMethod, TestCategory(Category)]
        public void Solve_ValidConfiguration_ShouldSolve()
        {
            // create the boad            
            var board = new Board();

            // create the problem
            IProblem problem = new PegBoardProblem(board);

            // create the algorithm
            IAlgorithm algorithm = new DepthFirstSearchAlgorithm();

            // get all solutions
            var result = algorithm.Solve(problem);

            // make sure we have solutions
            Assert.IsTrue(result.Solutions.Any());
            Debug.WriteLine($"{result.Solutions.Count()} total solutions found.");

            // output best solution            
            Solution bestSolution = result.Solutions.First(s => ((Board)s.FinalState).PegCount == result.Solutions.Min(s2 => ((Board)s2.FinalState).PegCount));
            Debug.WriteLine(bestSolution);
            Debug.WriteLine(result.Statistics);
        }

        private void WriteOutputFile(string contents)
        {
            string file = Path.Combine(TestContext.TestResultsDirectory, "output.txt");
            File.WriteAllText(file, contents);
            TestContext.AddResultFile(file);
        }

        private Board NewBoardWithPegAt(int index)
        {
            var config = BoardConfiguration.GetEmptyBoardConfiguration();

            var vertices = config.CreateVertexListFromVertices().ToList();
            var adjacencyMatrix = config.CreateAdjacencyMatrixFromEdges();

            vertices[index] = vertices[index].SetPeg(true);

            return new Board(vertices, adjacencyMatrix);
        }
    }
}
