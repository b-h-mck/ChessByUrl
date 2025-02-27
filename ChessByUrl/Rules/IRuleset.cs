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
        GameStatus GetGameStatus(Board board);

    }

    public static class RulesetExtensions
    {
        /// <summary>
        /// Returns all legal moves from the given square, or an empty sequence for an empty square or a piece without legal moves.
        /// </summary>
        /// <remarks>
        /// This is the result of combining all IGetLegalMovesBehaviours for the piece on the square, then passing it through
        /// all IFilterMoveCandidatesBehaviours for all pieces on the board.
        /// </remarks>
        public static IEnumerable<Move> GetLegalMoves(this IRuleset ruleset, Board board, Coords from)
        {
            var candidates = GetUnfilteredLegalMoveCandidates(ruleset, board, from);
            if (candidates == null || !candidates.Any())
                return Enumerable.Empty<Move>();

            var filteredMoves = FilterLegalMoveCandidates(ruleset, board, candidates);
            return filteredMoves;
        }

        /// <summary>
        /// Returns all moves that threaten the piece on the target square.
        /// </summary>
        /// <remarks>
        /// This is calculated by cycling through all enemy pieces on the board, finding their legal moves with their IGetLegalMovesBehaviours,
        /// and filtering them down to the ones that target the given square.
        /// It does *not* filter the moves through IFilterMoveCandidatesBehaviours, so it doesn't check for check (a pinned piece can
        /// still threaten the King).
        /// </remarks>
        public static IEnumerable<Move> GetThreats(this IRuleset ruleset, Board board, Coords target, Player threatenedPlayer)
        {
            foreach (var (coords, pieceType) in board)
            {
                if (pieceType == null || pieceType.Player.Id == threatenedPlayer.Id)
                    continue;

                var legalMoveCandidates = GetUnfilteredLegalMoveCandidates(ruleset, board, coords);
                foreach (var move in legalMoveCandidates)
                {
                    if (move.To == target)
                    {
                        yield return move;
                    }
                }
            }
        }

        private static IEnumerable<Move> GetUnfilteredLegalMoveCandidates(this IRuleset ruleset, Board board, Coords from)
        {
            var fromPiece = board.GetPiece(from);
            if (fromPiece == null)
                return Enumerable.Empty<Move>();
            var getLegalMoveBehaviours = fromPiece.Behaviours.OfType<IGetLegalMovesBehaviour>();
            List<Move> allLegalMoveCandidates = [];
            foreach (var behaviour in  getLegalMoveBehaviours)
            {
                var legalMoves = behaviour.GetLegalMovesFrom(ruleset, board, from, fromPiece);
                allLegalMoveCandidates.AddRange(legalMoves);
            }
            //var legalMoveCandidates = getLegalMoveBehaviours.SelectMany(behaviour => behaviour.GetLegalMovesFrom(ruleset, board, from, fromPiece));
            return allLegalMoveCandidates;
        }

        private static IEnumerable<Move> FilterLegalMoveCandidates(this IRuleset ruleset, Board board, IEnumerable<Move> candidates)
        {
            foreach (var (coords, pieceType) in board)
            {
                if (pieceType == null)
                    continue;
                var filters = pieceType.Behaviours.OfType<IFilterLegalMoveCandidatesBehaviour>();
                foreach (var filter in filters)
                {
                    candidates = filter.FilterLegalMoveCandidates(ruleset, board, coords, pieceType, candidates);
                }
                //candidates = filters.Aggregate(candidates, (current, filter) =>
                //    filter.FilterLegalMoveCandidates(ruleset, board, coords, pieceType, current));
            }
            return candidates;
        }

        /// <summary>
        /// Applies the given move to the board, returning the new board.
        /// </summary>
        /// <remarks>
        /// This is the result of running all IApplyMoveBehaviours on the moving piece, then running all IAdjustBoardAfterMoveBehaviours
        /// on all pieces on the board.
        /// </remarks>
        public static Board ApplyMove(this IRuleset ruleset, Board board, Move move)
        {
            var fromPiece = board.GetPiece(move.From);
            if (fromPiece == null)
            {
                throw new InvalidOperationException("Cannot apply a move from an empty square");
            }

            var resultBoard = board.ReplacePiece(move.From, null).ReplacePiece(move.To, board.GetPiece(move.From));

            var behaviours = fromPiece.Behaviours.OfType<IApplyMoveBehaviour>();
            foreach (var behaviour in behaviours)
            {
                resultBoard = behaviour.ApplyMoveFrom(ruleset, board, resultBoard, move, fromPiece);
            }

            foreach (var (coords, pieceType) in resultBoard)
            {
                if (pieceType == null)
                    continue;
                var adjustBehaviours = pieceType.Behaviours.OfType<IAdjustBoardAfterMoveBehaviour>();
                foreach (var adjustBehaviour in adjustBehaviours)
                {
                    resultBoard = adjustBehaviour.AdjustBoardAfterMove(ruleset, resultBoard, coords, pieceType, move);
                }
            }

            resultBoard = resultBoard.SetCurrentPlayer(ruleset.Players.Single(p => p.Id != board.CurrentPlayer.Id));

            return resultBoard;
        }
    }
}
