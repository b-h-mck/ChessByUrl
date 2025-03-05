namespace ChessByUrl.Rules
{
    /// <summary>
    /// Represents the state of the game at a given point in time. This includes the ruleset, the initial board, and all moves so far.
    /// Also provides methods and properties 
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Create a game with an initial board and no moves so far.
        /// </summary>
        public Game(IRuleset ruleset, Board initialBoard)
        {
            Ruleset = ruleset;
            InitialBoard = initialBoard;
            CurrentBoard = initialBoard;
            MovesSoFar = [];
        }

        public IRuleset Ruleset { get; }
        public Board InitialBoard { get; }
        public IEnumerable<Move> MovesSoFar { get; private set; }

        public Board CurrentBoard { get; private set; }
        public Player CurrentPlayer => CurrentBoard.CurrentPlayer;
        public GameStatus Status => Ruleset.GetGameStatus(this);

        public Game ApplyMove(Move move)
        {
            var fromPiece = CurrentBoard.GetPiece(move.From)
                ?? throw new InvalidOperationException("Cannot apply a move from an empty square");

            // Do the default part of the move (From to To).
            var boardAfterMove = CurrentBoard
                .ReplacePiece(move.From, null)
                .ReplacePiece(move.To, CurrentBoard.GetPiece(move.From));

            // Get the ApplyMoveBehaviours for the moving piece and apply them
            var behaviours = fromPiece.Behaviours.OfType<IApplyMoveBehaviour>();
            foreach (var behaviour in behaviours)
            {
                boardAfterMove = behaviour.ApplyMoveFrom(this, boardAfterMove, move, fromPiece);
            }

            // Get the AdjustBoardAfterMoveBehaviours for all pieces and apply them
            foreach (var (coords, pieceType) in boardAfterMove)
            {
                if (pieceType == null)
                    continue;
                var adjustBehaviours = pieceType.Behaviours.OfType<IAdjustBoardAfterMoveBehaviour>();
                foreach (var adjustBehaviour in adjustBehaviours)
                {
                    boardAfterMove = adjustBehaviour.AdjustBoardAfterMove(this, boardAfterMove, move, coords, pieceType);
                }
            }

            var nextPlayer = Ruleset.GetNextPlayer(CurrentPlayer);
            boardAfterMove = boardAfterMove.SetCurrentPlayer(nextPlayer);

            var result = new Game(Ruleset, InitialBoard);
            result.CurrentBoard = boardAfterMove;
            result.MovesSoFar = MovesSoFar.Concat(new[] { move });
            return result;
        }

        public Game ApplyMoves(IEnumerable<Move> moves)
        {
            var game = this;
            foreach (var move in moves)
            {
                game = game.ApplyMove(move);
            }
            return game;
        }


        /// <summary>
        /// Returns all legal moves from the given square, or an empty sequence for an empty square or a piece without legal moves.
        /// This excludes moves that would put the same-coloured King in check.
        /// </summary>
        /// <remarks>
        /// This is the result of combining all IGetLegalMovesBehaviours for the piece on the square, then passing it through
        /// all IFilterMoveCandidatesBehaviours for all pieces on the board.
        /// </remarks>
        public IEnumerable<Move> GetLegalMovesFromSquare(Coords from)
        {
            var candidates = GetUnfilteredLegalMoveCandidates(from);
            if (candidates == null || !candidates.Any())
                return Enumerable.Empty<Move>();

            var filteredMoves = FilterLegalMoveCandidates(candidates);
            return filteredMoves;
        }

        /// <summary>
        /// Returns all legal moves for the given player. Pass in <see cref="CurrentPlayer" /> for the current player.
        /// </summary>
        public IEnumerable<Move> GetLegalMovesForPlayer(Player player)
        {
            return CurrentBoard.FindSquares(piece => piece?.Player == player).SelectMany(GetLegalMovesFromSquare);
        }

        /// <summary>
        /// Returns all moves that threaten the target square. This is the set of moves that <paramref name="threatenedPlayer"/>'s
        /// opponent could make that would end on this square, but WITHOUT taking into account whether that move would put the
        /// threatening player's King in check.
        /// </summary>
        /// <remarks>
        /// This is the result of combining all IGetLegalMovesBehaviours for the opponent's pieces that that would end on the
        /// square but does NOT filter the moves through IFilterMoveCandidatesBehaviours.
        /// </remarks>
        public IEnumerable<Move> GetThreats(Coords target, Player threatenedPlayer)
        {
            return CurrentBoard.FindSquares(piece => piece?.Player != threatenedPlayer)
                .SelectMany(coords => GetUnfilteredLegalMoveCandidates(coords))
                .Where(move => move.To == target);
        }



        private IEnumerable<Move> GetUnfilteredLegalMoveCandidates(Coords from)
        {
            var fromPiece = CurrentBoard.GetPiece(from);
            if (fromPiece == null)
                return Enumerable.Empty<Move>();
            var getLegalMoveBehaviours = fromPiece.Behaviours.OfType<IGetLegalMovesBehaviour>();
            List<Move> allLegalMoveCandidates = [];
            foreach (var behaviour in getLegalMoveBehaviours)
            {
                var legalMoves = behaviour.GetLegalMovesFrom(this, from, fromPiece);
                allLegalMoveCandidates.AddRange(legalMoves);
            }
            return allLegalMoveCandidates;
        }

        private IEnumerable<Move> FilterLegalMoveCandidates(IEnumerable<Move> candidates)
        {
            foreach (var (coords, pieceType) in CurrentBoard)
            {
                if (pieceType == null)
                    continue;
                var filters = pieceType.Behaviours.OfType<IFilterLegalMoveCandidatesBehaviour>();
                foreach (var filter in filters)
                {
                    candidates = filter.FilterLegalMoveCandidates(this, coords, pieceType, candidates);
                }
            }
            return candidates;
        }

    }
}
