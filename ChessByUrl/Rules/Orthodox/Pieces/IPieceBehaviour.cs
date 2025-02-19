namespace ChessByUrl.Rules.Orthodox.Pieces
{
    public interface IPieceBehaviour
    {
        IEnumerable<Move> GetLegalMoves(Ruleset ruleset, Board board, Coords from);


        Board? TryApplySpecialMove(Ruleset ruleset, Board board, Move move);
    }
}
