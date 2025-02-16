namespace ChessByUrl.Rules.Standard.Pieces
{
    public class KingBehaviour : IPieceBehaviour
    {
        public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
        {
            return Enumerable.Empty<Move>();
        }
    }
}
