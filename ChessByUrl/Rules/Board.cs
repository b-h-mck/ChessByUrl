namespace ChessByUrl.Rules
{
    public class Board
    {
        public required Piece?[,] Squares { get; init; }
        public required Player CurrentPlayer { get; init; }
    }
}
