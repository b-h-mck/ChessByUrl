
namespace ChessByUrl.Rules.PieceBehaviours
{
    public class PromotionBehaviour : IFilterLegalMoveCandidatesBehaviour, IApplyMoveBehaviour
    {
        public PromotionBehaviour(int promotionRank, Func<IEnumerable<PieceType>> availablePromotions)
        {
            PromotionRank = promotionRank;
            AvailablePromotions = availablePromotions;
        }

        public int PromotionRank;
        public Func<IEnumerable<PieceType>> AvailablePromotions;

        public IEnumerable<Move> FilterLegalMoveCandidates(Game game, Coords thisSquare, PieceType thisPiece, IEnumerable<Move> candidates)
        {
            var result = new List<Move>();
            foreach (Move move in candidates)
            {
                if (move.From == thisSquare && move.To.Rank == PromotionRank)
                {
                    for (int i = 0; i < AvailablePromotions().Count(); i++)
                    {
                        result.Add(new Move(move.From, move.To, i));
                    }
                }
                else
                {
                    result.Add(move);
                }
            }
            return result;
        }

        public Board ApplyMoveFrom(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, PieceType fromPiece)
        {
            if (move.Variant == null)
            {
                return boardAfterMoveSoFar;
            }
            var promotionPiece = AvailablePromotions().ElementAt((int)move.Variant);
            return boardAfterMoveSoFar.ReplacePiece(move.To, promotionPiece);
        }
    }
}
