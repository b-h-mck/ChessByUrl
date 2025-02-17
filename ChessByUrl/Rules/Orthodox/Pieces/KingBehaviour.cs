namespace ChessByUrl.Rules.Orthodox.Pieces
{
    public class KingBehaviour : IPieceBehaviour
    {
        public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
        {
            return Enumerable.Empty<Move>();
        }

        public Board? TryApplySpecialMove(Board board, Move move)
        {
            return null;
        }
    }
}
