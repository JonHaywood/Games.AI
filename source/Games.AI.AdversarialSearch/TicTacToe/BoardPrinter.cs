using System;
using System.Text;

namespace Games.AI.AdversarialSearch.TicTacToe
{
    public interface IBoardPrinter
    {
        string Print(Board board);
    }

    public class BoardPrinter : IBoardPrinter
    {
        public BoardPrinter()
        {
            BoardPlayerFormatter = DefaultBoardPlayerFormatter;
        }

        /// <summary>
        /// Gets or sets the board player formatter which is used
        /// to display a symbol for a board player (e.g. X, O).
        /// </summary>
        public Func<BoardPlayer, string> BoardPlayerFormatter { get; set; } 

        public string Print(Board board)
        {
            var str = new StringBuilder();
            for (int i = 0, j = 0; i < 9; i++, j++)
            {
                // set player symbol
                if (board[i] == (int)BoardPlayer.NoPlayer)
                    str.Append(" ");
                else
                    str.Append(BoardPlayerFormatter(board[i]));

                // start a new line
                if (j == 2 && i < 6)
                {
                    str.Append(string.Format("{0}-+-+-{0}", Environment.NewLine));
                    j = -1;
                }
                else
                {
                    if (j != 2)
                        str.Append("|");
                }
            }
            return str.ToString();
        }

        /// <summary>
        /// The default board player formatter, where Player 1 is X's and Player 2 is O's.
        /// </summary>
        /// <param name="boardPlayer">The board player.</param>
        /// <returns>Formatter.</returns>
        private string DefaultBoardPlayerFormatter(BoardPlayer boardPlayer)
        {
            return (boardPlayer == BoardPlayer.Player1) ? "X" : "O";
        }
    }
}
