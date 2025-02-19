namespace ChessByUrl.Rules.Orthodox.Pieces
{
    public enum OrthodoxPieceType
    {
        Pawn,
        /// <summary>
        /// A pawn that has just moved two squares and is vulnerable to en passant. Will become a Pawn on the next turn.
        /// </summary>
        PawnWhoJustMovedTwoSquares,
        Knight,
        Bishop,
        Rook,
        /// <summary>
        /// A rook that could castle, i.e. it hasn't moved and its King hasn't moved. Other castling rules are not taken into account.
        /// </summary>
        RookWithCastlingRights,
        Queen,
        King

        // TODO: Maybe we could add KingWhoJustCastled? That would help distinguish between moves and castles for certain Chess960 layouts.
    }
}
