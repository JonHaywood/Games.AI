using System;

namespace Games.AI.UninformedSearch.PegBoard
{
    public class AdjacencyMatrix
    {
        private readonly int[,] matrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjacencyMatrix"/> class.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public AdjacencyMatrix(int[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix", "matrix is a required field.");
            if (matrix.GetLength(0) != matrix.GetLength(1))
                throw new ArgumentException("matrix must be square.", "matrix");
            this.matrix = matrix;
        }

        /// <summary>
        /// Gets the matrix size.
        /// </summary>        
        public int Size
        {
            get { return matrix.GetLength(0); }
        }

        /// <summary>
        /// Determines whether the specified path from one index to another has an edge.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        /// <returns>True if an edge exists, otherwise false.</returns>
        public bool HasEdge(int fromIndex, int toIndex)
        {
            return this[fromIndex, toIndex] >= 0;
        }

        /// <summary>
        /// Gets the edge value for the specified indices.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        public int this[int fromIndex, int toIndex]
        {
            get
            {
                if (fromIndex < 0 || fromIndex >= matrix.GetLength(0))
                    throw new ArgumentException(
                        string.Format("{0} is an invalid 'from index'. Index must be between 0 and {1}.", fromIndex,
                            matrix.GetLength(0)));
                if (toIndex < 0 || toIndex >= matrix.GetLength(0))
                    throw new ArgumentException(
                        string.Format("{0} is an invalid 'to index'. Index must be between 0 and {1}.", toIndex,
                            matrix.GetLength(0)));
                return matrix[fromIndex, toIndex];
            }
        }
    }
}
