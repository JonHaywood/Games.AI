using System;
using System.Linq;

namespace Games.AI.AdversarialSearch.TicTacToe
{
    public interface IBoardSerializer
    {
        Board Deserialize(string boardStr);

        string Serialize(Board board);
    }
    
    public class BoardSerializer : IBoardSerializer
    {
        public BoardSerializer()
        {
            Separator = ",";
        }

        public string Separator { get; set; }

        public Board Deserialize(string boardStr)
        {
            var positions = (from pos in boardStr.Split(new []{Separator}, StringSplitOptions.None)
                            select Convert.ToInt32(pos)).ToArray();
            var board = new Board();
            for (int i = 0; i < positions.Length; i++)
            {
                var value = BoardPlayer.NoPlayer;
                if (positions[i] == (int)BoardPlayer.Player1)
                    value = BoardPlayer.Player1;
                if (positions[i] == (int)BoardPlayer.Player2)
                    value = BoardPlayer.Player2;
                board[i] = value;
            }
            return board;
        }

        public string Serialize(Board board)
        {
            var positions = new int[9];

            for (int i = 0; i < 9; i++)
            { 
                if (board[i] == BoardPlayer.Player1)
                    positions[i] = 1;
                else if (board[i] == BoardPlayer.Player2)
                    positions[i] = 2;
                else
                    positions[i] = 0;
            }
            return string.Join(Separator, positions);
        }
    }
}
