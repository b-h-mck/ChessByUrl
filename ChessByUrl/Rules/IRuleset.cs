namespace ChessByUrl.Rules
{
    /// <summary>
    /// Represents the rules of a chess-like game.
    /// </summary>
    public interface IRuleset
    {
        /// <summary>
        /// All the players in the game (usually 2)
        /// </summary>
        IEnumerable<Player> Players { get; }

        /// <summary>
        /// Returns the next player when the given player finishes their turn.
        /// </summary>
        Player GetNextPlayer(Player currentPlayer);

        /// <summary>
        /// All the unique pieces in the game, across all players, and including stateful pseudo-pieces. 
        /// Examples include "White King", "Black Bishop", "White Rook with castling rights", "Black pawn vulnerable to en passant".
        /// </summary>
        IEnumerable<PieceType> PieceTypes { get; }

        /// <summary>
        /// Returns whether the given coordinates are within the bounds of the board.
        /// </summary>
        bool IsInBounds(Coords coords);

        /// <summary>
        /// Returns the status of the given board (check, checkmate, stalemate, etc).
        /// </summary>
        GameStatus GetGameStatus(Game game);

        /// <summary>
        /// Returns information about the given move's variant (e.g. promotion piece). null if move.Variant is null.
        /// </summary>
        /// <remarks>
        /// IPieceBehaviours are responsible for creating variants, and doing what's neceesary to apply the variant when applying the move.
        /// This method is used by the UI to help the user select a variant.
        /// </remarks>>
        MoveVariantInfo? GetMoveVariant(Board boardBeforeMove, Move move);

        string GetMoveNotation(Board boardBeforeMove, Move move);

    }
}
