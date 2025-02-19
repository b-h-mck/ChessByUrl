namespace ChessByUrl.Rules
{
    public class Move
    {
        public required Coords From { get; init; }
        public required Coords To { get; init; }

        /// <summary>
        /// What the piece should transform into after the move, or null for no transformation.
        /// </summary>
        /// <remarks>
        /// As well as pawn promotion, this is used to swap between Pawn & PawnWhoJustMovedTwoSquares
        /// and Rook & RookWithCastlingRights.
        /// </remarks>
        public Piece? ChangeTo { get; init; }

        public override string ToString() => $"{From} {To} {ChangeTo?.Name}";


        // TODO: How will we distinguish between moving and castling for certain Chess960 layouts?
        // Might need something here, but see the comment on OrthodoxPieceType.
    }
}
