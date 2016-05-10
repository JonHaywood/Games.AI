using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Games.AI.UninformedSearch.PegBoard
{
    public class Board : IState
    {
        private readonly List<Vertex> vertices;
        private readonly AdjacencyMatrix adjacencyMatrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="adjacencyMatrix">The adjecency matrix.</param>
        /// <param name="level">The state level.</param>
        /// <param name="printer">The board printer.</param>
        public Board(IEnumerable<Vertex> vertices, AdjacencyMatrix adjacencyMatrix, int level = 0, IPrinter<Board> printer = null)
        {
            if (vertices == null)
                throw new ArgumentNullException("vertices", "vertices is a required field.");
            if (adjacencyMatrix == null)
                throw new ArgumentNullException("adjacencyMatrix", "adjacencyMatrix is a required field.");            
            Level = level;
            Printer = printer ?? new BoardPrinter();
            this.vertices = vertices.ToList();
            this.adjacencyMatrix = adjacencyMatrix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class using
        /// a board configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Board(BoardConfiguration configuration) :
            this(configuration.CreateVertexListFromVertices(), configuration.CreateAdjacencyMatrixFromEdges())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class using
        /// the default board configuration.
        /// </summary>
        public Board() : this(BoardConfiguration.GetDefault())
        { }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the board printer.
        /// </summary>        
        public IPrinter<Board> Printer { get; set; }

        /// <summary>
        /// Gets the number of pegs on the board.
        /// </summary>
        /// <returns>Current peg count.</returns>
        public int PegCount
        {
            get { return vertices.Count(s => s.HasPeg); }
        }

        /// <summary>
        /// Gets the vertex count.
        /// </summary>
        public int VertexCount
        {
            get { return vertices.Count; }
        }

        /// <summary>
        /// Gets the adjecency matrix.
        /// </summary>
        public AdjacencyMatrix AdjacencyMatrix
        {
            get { return adjacencyMatrix; }
        }

        /// <summary>
        /// Gets the board vertices.
        /// </summary>        
        public IReadOnlyList<Vertex> Vertices
        {
            get { return vertices.AsReadOnly(); }
        }

        /// <summary>
        /// Gets whether there is a peg at the given index.
        /// </summary>        
        public Vertex this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentException("index must greater than 0.");
                if (index > vertices.Count)
                    throw new ArgumentException("index must be less than " + vertices.Count);                
                return vertices[index];
            }
        }

        /// <summary>
        /// Gets the indices on the board that have a peg.
        /// </summary>
        /// <returns>Peg coordinates.</returns>
        public IEnumerable<Vertex> GetSpacesWithPegs()
        {
            return vertices.Where(s => s.HasPeg);
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
                if (vertices[i] != other.vertices[i])
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
            return vertices.OrderBy(v => v.Index).Aggregate(19, (current, i) => current * 31 + (i.HasPeg ? 1 : 0).GetHashCode());
        }

        /// <summary>
        /// Executes the provided jump on the board. Returns a new version of the board
        /// that has the executed jump.
        /// </summary>
        /// <param name="jump">The jump to execute.</param>
        /// <returns>New board with the moved pegs.</returns>
        public Board ExecuteJump(Jump jump)
        {
            var spaces = vertices;

            // find index of vertices
            int currentIndex = vertices.FindIndex(v => v.Index == jump.Current.Index);
            int jumpedIndex = vertices.FindIndex(v => v.Index == jump.Jumped.Index);
            int destinationIndex = vertices.FindIndex(v => v.Index == jump.Destination.Index);

            // make sure the move is valid
            if (!spaces[currentIndex].HasPeg)
                throw new ArgumentNullException("jump", "current must be space with a peg.");
            if (!spaces[jumpedIndex].HasPeg)
                throw new ArgumentNullException("jump", "jumped must be space with a peg.");
            if (spaces[destinationIndex].HasPeg)
                throw new ArgumentNullException("jump",  "destination must be an empty space.");

            // make a clone of all spaces
            var newSpaces = spaces.Select(v => (Vertex)v.Clone()).ToArray();

            // perform the jump and change associated values
            newSpaces[currentIndex] = newSpaces[currentIndex].SetPeg(false);
            newSpaces[jumpedIndex] = newSpaces[jumpedIndex].SetPeg(false);
            newSpaces[destinationIndex] = newSpaces[destinationIndex].SetPeg(true);

            // create a new board
            return new Board(newSpaces, adjacencyMatrix, Level);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Printer.Print(this);
        }

        /// <summary>
        /// Creates a configuration for the current board.
        /// </summary>
        /// <returns>Board configuration.</returns>
        public BoardConfiguration CreateConfiguration()
        {
            var boardConfiguration = new BoardConfiguration
            {
                Edges = new List<string>(),
                Vertices = new Dictionary<string, string>()
            };

            // fill vertices
            for (int i = 0; i < VertexCount; i++)
            {
                boardConfiguration.Vertices[i.ToString(CultureInfo.InvariantCulture)] = this[i].HasPeg 
                    ? BoardConfiguration.HasPegToken 
                    : BoardConfiguration.EmptyPegToken;
            }

            // fill edges
            for (int i = 0; i < AdjacencyMatrix.Size; i++)
            {
                for (int j = 0; j < AdjacencyMatrix.Size; j++)
                {
                    var value = AdjacencyMatrix[i, j];
                    if (value == -1)
                        continue;
                    boardConfiguration.Edges.Add(string.Format("{1}{0}{2}{0}{3}", BoardConfiguration.SeparatorToken, i, value, j));
                }
            }

            return boardConfiguration;
        }
    }
}
