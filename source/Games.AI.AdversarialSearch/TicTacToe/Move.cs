
namespace Games.AI.AdversarialSearch.TicTacToe
{
    /// <summary>
    /// Represents a move on the board (a player and where they are making a move)
    /// </summary>
    public class Move : BaseObject, IAction
    {
        public Move(int index, BoardPlayer player)
        {
            Index = index;
            Player = player;
        }

        [DomainSignature]
        public int Index { get; private set; }

        [DomainSignature]
        public BoardPlayer Player { get; set; }

        public Board Execute(Board board)
        {
            var newBoard = (Board)board.Clone();
            newBoard[Index] = Player;
            return newBoard;
        }
    }
}
