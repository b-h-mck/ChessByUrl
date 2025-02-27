namespace ChessByUrl.Rules
{
    /// <summary>
    /// Represents the current status of the game (e.g. check, checkmate, stalemate).
    /// </summary>
    public class GameStatus
    {
        /// <summary>
        /// Whether the game is finished (won or drawn).
        /// </summary>
        public required bool IsFinished { get; init; }

        /// <summary>
        /// Current points assigned to each player, or null if not applicable.
        /// </summary>
        /// <remarks>
        /// In orthodox chess, this will be null is IsFinished is false, or else 1-0, 0-1, or 0.5-0.5.
        /// </remarks>
        public IEnumerable<(Player player, decimal points)>? PlayerPoints { get; init; }

        /// <summary>
        /// Strings to display to the end user, optionally with a player to highlight.
        /// </summary>
        /// <remarks>
        /// These will be things like (White, "to move"), (Black, "in check") or (null, "stalemate").
        /// </remarks>
        public required IEnumerable<(Player? player, string statusString)> StatusStrings { get; init; }
    }
}
