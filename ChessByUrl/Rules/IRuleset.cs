namespace ChessByUrl.Rules
{
    public interface IRuleset
    {
        Coords MaxRankFile { get; }
        IEnumerable<Player> Players { get; }
        IEnumerable<Piece> Pieces { get; }
        IEnumerable<Move> GetLegalMoves(Board board, Coords from);
        Board ApplyMove(Board board, Move move);
    }
}
