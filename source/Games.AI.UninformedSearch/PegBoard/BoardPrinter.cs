using System;
using System.Collections.Generic;
using System.Text;

namespace Games.AI.UninformedSearch.PegBoard
{
    /// <summary>
    /// Class which can print a board to the console in a human-recognizable string.
    /// </summary>
    public class BoardPrinter : IPrinter<Board>
    {
        private const string Space = "_";
        private const string Peg = "X";
        private const string EmptySpace = "O";

        public string Print(Board board)
        {
            if (board.VertexCount != 15)
                throw new ArgumentException(string.Format("Unable to format board. 15 pegs were expected but {0} were found,", board.VertexCount));

            var output = new StringBuilder();
            var queue = new Queue<Vertex>(board.Vertices);

            const int rows = 5; // there are 5 rows in the triangle
            const int length = 9; // each row is 9 spaces long
            
            for (int i = 0; i < rows; i++)
            {
                // pop off needed vertices
                var pegTokens = new List<string>();                
                for (int j = 0; j < i + 1; j++)
                {
                    pegTokens.Add(queue.Dequeue().HasPeg ? Peg : EmptySpace);
                }

                var pegsAndSpaces = string.Join(Space, pegTokens); // add spaces between peg or peg holes
                var padding = (length - pegsAndSpaces.Length)/2; // figure out padding on left and right. this will always be even
                var paddingStr = new string(Space[0], padding); // create string with needed padding
                pegsAndSpaces = paddingStr + pegsAndSpaces + paddingStr;

                output.AppendLine(pegsAndSpaces);
            }

            return output.ToString();
        }
    }
}
