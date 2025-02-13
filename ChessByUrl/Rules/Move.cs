namespace ChessByUrl.Rules
{
    public class Move
    {
        public required Coords From { get; init; }
        public required Coords To { get; init; }
        public Piece? ChangeTo { get; init; }
    }
}
