
namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Behaviour for a pawn's special move from its starting rank. This advances the pawn to the target rank as
    /// long as the target square and all squares between are empty. Also optionally transforms the piece after the move.
    /// </summary>
    /// <remarks>
    /// In orthodox chess startRank will be 1 or 6, and targetRank will be 3 or 4.
    /// 
    /// transformTo can be used to transform to a piece vulnerable to en passant capture.
    /// </remarks>
    public class PawnLongMoveBehaviour : IGetLegalMovesBehaviour, IApplyMoveBehaviour
    {
        /// <param name="startRank">Pawn's start rank (1 for White, 6 for Black)</param>
        /// <param name="targetRank">The rank of the target square for this long move</param>
        /// <param name="transformTo">What this piece should transform to after it does this move (null if it shouldn't transform)</param>
        public PawnLongMoveBehaviour(int startRank, int targetRank, Func<PieceType>? transform)
        {
            _startRank = startRank;
            _targetRank = targetRank;
            _transform = transform;
        }

        private int _startRank;
        private int _targetRank;
        private Func<PieceType>? _transform;

        public IEnumerable<Move> GetLegalMovesFrom(Game game, Coords from, PieceType fromPiece)
        {
            if (from.Rank != _startRank)
            {
                yield break;
            }

            var direction = _startRank < _targetRank ? 1 : -1;
            var to = from;
            while (to.Rank != _targetRank)
            {
                to = to.AddToRank(direction);
                var piece = game.CurrentBoard.GetPiece(to);
                if (piece != null)
                {
                    yield break;
                }
                
            }
            yield return new Move { From = from, To = to };
        }


        public Board ApplyMoveFrom(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, PieceType fromPiece)
        {
            bool wasLongMove = move.From.Rank == _startRank && move.To.Rank == _targetRank;
            if (wasLongMove && _transform != null)
            {
                var newPieceType = _transform();
                boardAfterMoveSoFar = boardAfterMoveSoFar.ReplacePiece(move.To, newPieceType);
            }
            return boardAfterMoveSoFar;
        }

    }
}
