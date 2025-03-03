
namespace ChessByUrl.Rules.PieceBehaviours
{
    public class RoyalBehaviour : IFilterLegalMoveCandidatesBehaviour
    {
        public IEnumerable<Move> FilterLegalMoveCandidates(Game game, Coords thisSquare, PieceType thisPiece, IEnumerable<Move> candidates)
        {
            foreach (var move in candidates)
            {
                var movingPiece = game.CurrentBoard.GetPiece(move.From);
                if (movingPiece?.Player.Id == thisPiece.Player.Id)
                {
                    var royalSquareAfterMove = thisSquare == move.From ? move.To : thisSquare;

                    var boardAfterMove = new Game(game, move);
                    var threats = game.GetThreats(royalSquareAfterMove, thisPiece.Player);
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
