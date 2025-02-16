namespace ChessByUrl.Rules
{
    public interface IRuleset
    {
        IEnumerable<Player> Players { get; }
        IEnumerable<Piece> Pieces { get; }
        bool IsInBounds(Coords coords);
        IEnumerable<Move> GetLegalMoves(Board board, Coords from);
        Board ApplyMove(Board board, Move move);
    }
}
