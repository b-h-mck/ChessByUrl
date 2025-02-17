namespace ChessByUrl.Rules.Orthodox.Pieces
{
    public interface IPieceBehaviour
    {
        IEnumerable<Move> GetLegalMoves(Board board, Coords from);


        Board? TryApplySpecialMove(Board board, Move move);
    }
}
