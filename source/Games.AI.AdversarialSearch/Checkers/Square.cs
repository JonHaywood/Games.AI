using Games.AI.Search;
using System;

namespace Games.AI.AdversarialSearch.Checkers
{    
    /// <summary>
    /// The color of the square.
    /// </summary>
    public enum SquareColor { Black, Red }

    /// <summary>
    /// Represents a square on the board.
    /// </summary>
    public class Square : BaseObject, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Square"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="row">The row.</param>
        /// <param name="color">The color.</param>        
        /// <param name="piece">The piece on the square.</param>
        public Square(int column, int row, SquareColor color, Piece piece = null)
        {
            Column = column;
            Row = row;
            Color = color;
            Piece = piece;
        }

        [DomainSignature]
        public int Column { get; private set; }

        [DomainSignature]
        public int Row { get; private set; }

        [DomainSignature]
        public SquareColor Color { get; private set; }

        [DomainSignature]
        public Piece Piece { get; set; }

        /// <summary>
        /// Returns true if the square has a piece, otherwise false.
        /// </summary>
        public bool HasPiece
        {
            get { return Piece != null; }
        }

        /// <summary>
        /// Gets the coordinate. Is standard chess notation.
        /// </summary>
        public string Coordinate
        {
            get
            {
                return string.Format("{0}{1}", (char)(Column + 97), Row + 1);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Coordinate;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new Square(Column, Row, Color, Piece);
        }
    }
}