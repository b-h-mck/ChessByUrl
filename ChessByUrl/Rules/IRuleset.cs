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
        /// Returns the next player when the given player finishes their turn.
        /// </summary>
        Player GetNextPlayer(Player currentPlayer);

    }
}
