namespace ChessByUrl.Rules
{
    public class Move
    {
        public required Coords From { get; init; }
        public required Coords To { get; init; }


        // TODO: These two properties are very specific to orthodox chess. Could they be generalised?
        public Piece? ChangeTo { get; init; }
        public bool? IsCastle { get; init; } // Some Chess960 games make castling impossible to determine from Coords alone.
    }
}
