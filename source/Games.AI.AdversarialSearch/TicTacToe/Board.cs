using System;
using System.Collections.Generic;
using System.Linq;

namespace Games.AI.AdversarialSearch.TicTacToe
{
    public enum BoardPlayer
    {
        NoPlayer = 0, Player1 = 1, Player2 = 2
    }

    [Serializable]
    public class Board : ICloneable, IEnumerable<int>, IState
    {
        private int[] board;

        /// <summary>
        /// Constructor - initializes empty board.
        /// </summary>
        public Board()
        {
            Printer = new BoardPrinter();
            board = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };       
        }

        /// <summary>
        /// Gets the value at the specified 0-based index
        /// </summary>
        /// <param name="index">Index to retrieve</param>
        /// <returns>BoardPlayer</returns>
        public BoardPlayer this[int index]
        {
            get { return (BoardPlayer)board[index]; }
            set { board[index] = (int)value; }
        }

        /// <summary>
        /// Gets the value based on x,y coordinates, where 0,0 is 
        /// the top left.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>BoardPlayer</returns>
        public BoardPlayer this[int x, int y]
        {
            get { return (BoardPlayer)board[y * 3 + x]; }
            set { board[y * 3 + x] = (int)value; }
        }

        /// <summary>
        /// Gets or sets the board printer.
        /// </summary>
        public IBoardPrinter Printer { get; set; }

        /// <summary>
        /// Boolean based on if there is a player on the board.
        /// </summary>
        /// <returns>True if board is empty.</returns>
        public bool IsEmpty()
        {
            return board.All(i => i == 0);
        }

        /// <summary>
        /// Boolean based on if there the board is full or not.
        /// </summary>
        /// <returns>True if board is full.</returns>
        public bool IsFull()
        {
            return board.All(i => i != 0);
        }

        /// <summary>
        /// Evaluates if player has won or not
        /// </summary>
        /// <param name="player">BoardPlayer</param>
        /// <returns>True if given player has won.</returns>
        public bool Evaluate(BoardPlayer player)
        {
            int x;
            // check columns
            for (int i = 0; i < 3; i++)
            {
                x = 0;
                for (int j = 0; j < 3; j++)
                    if ((int)this[i, j] == (int)player) x++;
                if (x == 3) return true;
            }
            // check rows
            for (int i = 0; i < 3; i++)
            {
                x = 0;
                for (int j = 0; j < 3; j++)
                    if ((int)this[j, i] == (int)player) x++;
                if (x == 3) return true;
            }
            // check diagonal
            x = 0;
            for (int i = 0; i < 3; i++)
            {
                if ((int)this[i, i] == (int)player) x++;
                if (x == 3) return true;
            }
            // check diagonal the other way
            x = 0;
            for (int i = 2, j = 0; i >= 0; i--, j++)
            {
                if ((int)this[i, j] == (int)player) x++;
                if (x == 3) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all board spots that are not taken.
        /// </summary>
        /// <returns>Array of indices that are free on the board.</returns>
        public IEnumerable<int> GetValidMoves()
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == 0)
                    yield return i;
            }
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
            if (this == obj)
                return true;
            if (!(obj is Board))
                return false;
            var other = (Board)obj;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != other.board[i])
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
            return board.Aggregate(19, (current, i) => current * 31 + i.GetHashCode());
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var newBoard = (Board)MemberwiseClone();
            newBoard.board = (int[])board.Clone();
            return newBoard;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<int> GetEnumerator()
        {
            return board.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return board.GetEnumerator();
        }
    }
}
