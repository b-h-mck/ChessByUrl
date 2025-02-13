namespace ChessByUrl.Rules.Standard.Pieces
{
    public interface IPieceBehaviour
    {
        IEnumerable<Move> GetLegalMoves(Board board, Coords from);
        void ApplySpecialMove(Board board, Move move);
    }
}
