namespace ChessByUrl.Rules.PieceBehaviours
{
    public static class CastlingHelpers
    {
        public static bool IsInitiator(PieceType pieceType)
        {
            return pieceType.Behaviours.OfType<CastlingInitiatorBehaviour>().Any();
        }

        public static bool IsResponder(PieceType pieceType)
        {
            return pieceType.Behaviours.OfType<CastlingResponderBehaviour>().Any();
        }

        /// <summary>
        /// Returns whether there's a clear castling route (ignoring the initiator and responder).
        /// This is required for both the initiator and responder.
        /// </summary>
        public static bool IsRouteClear(Board board, int rank, int fromFile, int toFile)
        {
            var files = new[] { fromFile, toFile };
            for (int file = files.Min(); file <= files.Max(); file++)
            {
                if (file == fromFile)
                    continue;
                var piece = board.GetPiece(new Coords(rank, file));
                if (piece != null && !IsInitiator(piece) && !IsResponder(piece))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns whether the route is safe from threats.
        /// This is only required for the initiator.
        /// </summary>
        public static bool IsRouteSafe(Game game, int rank, int fromFile, int toFile)
        {
            var piece = game.CurrentBoard.GetPiece(new Coords(rank, fromFile));
            if (piece == null)
                return false;

            var files = new[] { fromFile, toFile };
            for (int file = files.Min(); file <= files.Max(); file++)
            {
                if (game.GetThreats(new Coords(rank, file), piece.Player)?.Any() ?? false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Finds the responder in the given direction if one exists.
        /// </summary>
        public static Coords? FindResponder(Game game, Coords initiatorCoords, int direction)
        {
            var initiator = game.CurrentBoard.GetPiece(initiatorCoords);
            if (initiator == null)
                return null;

            var coords = initiatorCoords.AddToFile(direction);
            while (game.Ruleset.IsInBounds(coords))
            {
                var piece = game.CurrentBoard.GetPiece(coords);
                if (piece != null && IsResponder(piece) && piece.Player.Id != initiator.Player.Id)
                    return coords;
                coords = coords.AddToFile(direction);
            }
            return null;
        }
    }
}
