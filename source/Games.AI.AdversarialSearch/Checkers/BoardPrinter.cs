using System;
using System.Linq;
using System.Text;

namespace Games.AI.AdversarialSearch.Checkers
{
    public class BoardPrinter : IPrinter<Board>
    {
        public string Print(Board board)
        {
            var size = Board.BoardSize;
            var output = new StringBuilder();

            for (int row = size - 1; row >= 0; row--)
            {                
                output.AppendLine("+" + string.Join("+", Enumerable.Repeat("-", size).ToArray()) + "+");
                output.Append("|");

                for (int column = 0; column < size; column++)
                {
                    var piece = board[column, row].Piece;
                    if (piece == null)
                        output.Append(" |");
                    else if (piece.Player == BoardPlayer.Player1)
                        output.Append("X|");
                    else if (piece.Player == BoardPlayer.Player2)
                        output.Append("O|");
                }
                output.Append(Environment.NewLine);
            }
            output.AppendLine("+" + string.Join("+", Enumerable.Repeat("-", size).ToArray()) + "+");
            return output.ToString();
        }
    }
}
