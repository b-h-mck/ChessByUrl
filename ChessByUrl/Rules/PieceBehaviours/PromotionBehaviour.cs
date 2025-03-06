
namespace ChessByUrl.Rules.PieceBehaviours
{
    public class PromotionBehaviour : IFilterLegalMoveCandidatesBehaviour
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
            foreach (Move move in candidates)
            {
                if (move.From == thisSquare && move.To.Rank == PromotionRank)
                {
                    for (int i = 0; i < AvailablePromotions().Count(); i++)
                    {
                        yield return new Move(move.From, move.To, i);
                    }
                }
                else
                {
                    yield return move;
                }
            }
        }

        
    }
}
