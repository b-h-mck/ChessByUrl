
namespace ChessByUrl.Rules.PieceBehaviours
{
    public class RoyalBehaviour : IFilterLegalMoveCandidatesBehaviour
    {
        public IEnumerable<Move> FilterLegalMoveCandidates(IRuleset ruleset, Board board, Coords thisSquare, PieceType thisPiece, IEnumerable<Move> candidates)
        {
            foreach (var move in candidates)
            {
                var royalSquare = thisSquare;
                var movingPiece = board.GetPiece(move.From);
                if (movingPiece?.Player.Id == thisPiece.Player.Id)
                {
                    var boardAfterMove = ruleset.ApplyMove(board, move);
                    // If the king itself has moves, check against its new position
                    if (move.From == royalSquare)
                    {
                        royalSquare = move.To;
                    }
                    var threats = ruleset.GetThreats(boardAfterMove, royalSquare, thisPiece.Player);
                    if (threats.Any())
                    {
                        continue;
                    }
                }
                yield return move;
            }
        }
    }
}
