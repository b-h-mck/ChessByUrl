namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Represents the behaviour of a piece that is vulnerable to en passant capture.
    /// </summary>
    /// <remarks>
    /// All this behaviour does by itself is transform the piece to its non-vulnerable equivalent after the move (as long as the
    /// move itself wasn't the move that made it vulnerable in the first place).
    /// 
    /// This behaviour also acts as a tag for EnPassantAttackerBehaviour to know which pieces can be captured en passant.
    /// </remarks>
    public class EnPassantVictimBehaviour : IAdjustBoardAfterMoveBehaviour
    {
        public EnPassantVictimBehaviour(Func<PieceType> transform)
        {
            Transform = transform;
        }

        public Func<PieceType> Transform { get; }

        public Board AdjustBoardAfterMove(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, Coords thisSquare, PieceType thisPiece)
        {
            // If this piece just did a long move, return the board unchanged
            if (thisSquare == move.To && Math.Abs(move.From.Rank - move.To.Rank) > 1)
            {
                return boardAfterMoveSoFar;
            }

            // Otherwise, transform the piece
            return boardAfterMoveSoFar.ReplacePiece(thisSquare, Transform());
        }

        
    }
}
