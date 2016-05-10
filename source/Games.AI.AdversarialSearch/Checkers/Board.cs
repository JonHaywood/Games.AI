using System;
using System.Collections.Generic;
using System.Linq;

namespace Games.AI.AdversarialSearch.Checkers
{    
    [Serializable]
    public class Board : BaseObject, ICloneable, IEnumerable<Square>, IState
    {
        public const int BoardSize = 8;
        private static readonly int[][] DiagonalDirections =
        {
            new [] {-1,-1}, 
            new [] { 1,-1},                    
            new [] {-1, 1}, 
            new [] { 1, 1}
        };
        private readonly Square[] squares;

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class with an empty board.
        /// </summary>
        /// <param name="setStartingPositions">If set to <c>true</c> then player starting pieces will be set.</param>
        public Board(bool setStartingPositions = false)
        {     
            Printer = new BoardPrinter();
            Level = 0;
            var tempSquares = new List<Square>(BoardSize);  

            // create a new board with pieces set in starting positions
            for (int i = 0; i < BoardSize; i++) // rows
            {
                bool isBlack = i%2 == 0;                
                for (int j = 0; j < BoardSize; j++) // columns
                {
                    var square = new Square(i, j, isBlack ? SquareColor.Black : SquareColor.Red);

                    if (setStartingPositions)
                    {
                        if (j < 3 && isBlack)
                            square.Piece = new Piece(BoardPlayer.Player1);
                        if (j > 4 && isBlack)
                            square.Piece = new Piece(BoardPlayer.Player2);
                    }

                    tempSquares.Add(square);
                    isBlack = !isBlack;                    
                }
            }
            squares = tempSquares.ToArray();     
        }

        /// <summary>
        /// Constructor - used for cloning.
        /// </summary>
        /// <param name="internalSquares">The internal squares.</param>
        /// <param name="level">The state level.</param>
        /// <param name="printer">The printer.</param>
        private Board(Square[] internalSquares, int level, IPrinter<Board> printer = null)
        {
            Printer = printer ?? new BoardPrinter();
            Level = level;
            squares = internalSquares;
        }

        /// <summary>
        /// Gets the <see cref="Square"/> with the specified chess coordinate.
        /// </summary>           
        public Square this[string coordinate]
        {
            get
            {
                var square = squares.SingleOrDefault(s => s.Coordinate == coordinate);
                if (square == null) throw new ArgumentException(string.Format("Invalid coordinate: '{0}'.", coordinate));
                return square;
            }
        }

        /// <summary>
        /// Gets the <see cref="Square"/> with the specified column and row coordinate.
        /// </summary>   
        public Square this[int column, int row]
        {
            get
            {
                var square = squares.SingleOrDefault(s => s.Column == column && s.Row == row);
                if (square == null) throw new ArgumentException(string.Format("Invalid coordinate: ({0},{1}).", column, row));
                return square;
            }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>        
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the board printer.
        /// </summary>
        public IPrinter<Board> Printer { get; set; }

        /// <summary>
        /// Gets the squares.
        /// </summary>
        public IEnumerable<Square> Squares
        {
            get { return squares; }
        }

        /// <summary>
        /// Gets or sets the player that played the last move.
        /// </summary>
        public BoardPlayer? LastPlayer { get; set; }

        /// <summary>
        /// Evaluates if player has won or not
        /// </summary>
        /// <param name="player">BoardPlayer</param>
        /// <returns>True if given player has won.</returns>
        public bool Evaluate(BoardPlayer player)
        {
            var opponentPlayer = (player == BoardPlayer.Player1) ? BoardPlayer.Player2 : BoardPlayer.Player1;
            var opponentCount = squares.Count(s => s.Piece != null && s.Piece.Player == opponentPlayer);            

            return opponentCount == 0;
        }

        /// <summary>
        /// Gets the valid moves for this board.
        /// </summary>
        /// <param name="player">Board player to find valid moves for.</param>
        /// <returns>All valid moves.</returns>
        public IEnumerable<Move> GetValidMoves(BoardPlayer player)
        {
            var moves = new List<Move>();
            var squaresForPlayer = squares.Where(s => s.Piece != null && s.Piece.Player == player);
            var opponentPlayer = (player == BoardPlayer.Player1) ? BoardPlayer.Player2 : BoardPlayer.Player1;
            
            foreach (var square in squaresForPlayer)
            {   
                // check for moves in all valid directions
                foreach (var direction in GetMovableDirections(square.Piece))
                {
                    var x = square.Column + direction[0];
                    var y = square.Row + direction[1];                    

                    // in this case we've gone off the board, so skip
                    if (x < 0 || y < 0 || x >= BoardSize || y >= BoardSize)
                        continue;

                    // see if square is empty - if so is a valid move
                    var target = squares.Single(s => s.Column == x && s.Row == y);
                    if (target.Piece == null)
                    {
                        moves.Add(new Move {At = square.Coordinate, MoveTo = target.Coordinate});
                    }
                    // see if square is our opponent - we might be able to make a jump
                    else if (target.Piece.Player == opponentPlayer)
                    {
                        FindBoardJumps(this, square, opponentPlayer, moves, new List<Jump>());
                    }
                }
            }

            return moves;
        }

        /// <summary>
        /// Default ToString
        /// </summary>
        /// <returns>String representation of object</returns>
        public override string ToString()
        {
            return Printer.Print(this);
        }

        /// <summary>
        /// Determines if another board is equal to given board
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        // ReSharper disable once CSharpWarnings::CS0659
        public override bool Equals(object obj)
        {
            // NOTE: we can't use [DomainSignature] because we have
            // to do a manual comparison of the board array values
            if (ReferenceEquals(this, obj))
                return true;
            if (!(obj is Board))
                return false;
            var other = (Board)obj;            
            for (int i = 0; i < 9; i++)
            {
                if (squares[i] != other.squares[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            // see http://stackoverflow.com/questions/8094867/good-gethashcode-override-for-list-of-foo-objects-respecting-the-order
            return squares.Aggregate(19, (current, square) => current*31 + square.GetHashCode());
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {            
            var squareClones = squares.Select(s => (Square)s.Clone()).ToArray();
            var newBoard = new Board(squareClones, Level, Printer);
            return newBoard;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Square> GetEnumerator()
        {
            return squares.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return squares.GetEnumerator();
        }

        /// <summary>
        /// Finds board jumps.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <param name="square">The square.</param>
        /// <param name="opponentPlayer">The opponent player.</param>
        /// <param name="moves">The list of available moves.</param>
        /// <param name="currentJumps">The list of current jumps being made (might be recursing down a multiple jump)</param>
        private void FindBoardJumps(Board board, Square square, BoardPlayer opponentPlayer, List<Move> moves, List<Jump> currentJumps)
        {
            foreach (var direction in GetMovableDirections(square.Piece))
            {
                var jumped = board.squares.SingleOrDefault(s => s.Piece != null && s.Piece.Player == opponentPlayer &&
                    s.Column == square.Column + (direction[0]*1) && s.Row == square.Row + (direction[1]*1));
                var target = board.squares.SingleOrDefault(s => s.Piece == null &&
                    s.Column == square.Column + (direction[0]*2) && s.Row == square.Row + (direction[1]*2));

                // if either don't exist then we either went off the board
                // or the spaces aren't valid
                if (jumped == null || target == null)
                    continue;
                
                // create new jump
                var jump = new Jump {At = square.Coordinate, MoveTo = target.Coordinate, Jumped = jumped.Coordinate};

                // clone jump list (so modifying list doesn't affect recursing) and add jump
                var currentJumpsClone = currentJumps.ToList();
                currentJumpsClone.Add(jump);

                Move move = jump;

                // create a multiple jump if there's more than one
                if (currentJumpsClone.Count > 1)
                    move = new MultipleJump {Jumps = currentJumpsClone};

                moves.Insert(0, move); // add to beginning so that we try jumps first

                // find a potential next jump using a new board
                var newBoard = jump.Execute(board);
                FindBoardJumps(newBoard, newBoard[target.Coordinate], opponentPlayer, moves, currentJumpsClone);
            }
        }

        /// <summary>
        /// Gets the movable directions for a piece based on the player type and if it's a king.
        /// </summary>
        /// <param name="piece">The piece.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">piece;Piece cannot be null.</exception>
        private IEnumerable<int[]> GetMovableDirections(Piece piece)
        {
            if (piece == null)
                throw new ArgumentNullException("piece", "Piece cannot be null.");
            if (piece.IsKing)
                return DiagonalDirections; // return all directions
            if (piece.Player == BoardPlayer.Player1)                            
                return DiagonalDirections.Skip(2); // player 1 is on bottom, get directions that move up (the last 2)

            return DiagonalDirections.Take(2); // player 2 is on top, get directions that move down (the first 2)
        }
    }
}
