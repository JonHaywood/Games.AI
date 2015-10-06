
namespace Games.AI.AdversarialSearch.Checkers
{
    /// <summary>
    /// The board players available.
    /// </summary>
    public enum BoardPlayer
    {
        Player1 = 1,  // Player1 is on bottom 
        Player2 = 2   // Player2 is on top
    }

    /// <summary>
    /// Represents a piece on the board.
    /// </summary>
    public class Piece : BaseObject
    {
        public Piece() { }

        public Piece(BoardPlayer player)
        {
            Player = player;
        }

        [DomainSignature]
        public BoardPlayer Player { get; set; }

        [DomainSignature]
        public bool IsKing { get; set; }
    }
}
