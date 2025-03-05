namespace ChessByUrl.Rules
{
    public class Move : IEquatable<Move>
    {
        // TODO: Add a constructor and use it everywhere

        public required Coords From { get; init; }
        public required Coords To { get; init; }

        /// <summary>
        /// What the piece should transform into after the move, or null for no transformation.
        /// </summary>
        /// <remarks>
        /// As well as pawn promotion, this is used to swap between Pawn & PawnWhoJustMovedTwoSquares
        /// and Rook & RookWithCastlingRights.
        /// </remarks>
        public PieceType? ChangeTo { get; init; }

        public override string ToString() => $"{From}-{To}-{ChangeTo?.Name}".TrimEnd('-');

        public override bool Equals(object? obj) => Equals(obj as Move);

        public bool Equals(Move? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return From.Equals(other.From) && To.Equals(other.To) && ChangeTo?.Id == other.ChangeTo?.Id;
        }

        public override int GetHashCode() => HashCode.Combine(From, To, ChangeTo);

        public static bool operator ==(Move? left, Move? right) => Equals(left, right);

        public static bool operator !=(Move? left, Move? right) => !Equals(left, right);

        // TODO: How will we distinguish between moving and castling for certain Chess960 layouts?
        // Might need something here, but see the comment on OrthodoxPieceType.
    }
}
