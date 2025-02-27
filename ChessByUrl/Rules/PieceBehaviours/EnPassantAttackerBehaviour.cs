namespace ChessByUrl.Rules.PieceBehaviours
{
    public class EnPassantAttackerBehaviour : IGetLegalMovesBehaviour, IApplyMoveBehaviour
    {
        public IEnumerable<Move> GetLegalMovesFrom(IRuleset ruleset, Board board, Coords from, PieceType fromPiece)
        {
            var direction = fromPiece.Player.Direction;
            var farthestRank = fromPiece.Player.FarthestRank;
            Coords[] victimSquares = [new Coords(from.Rank, from.File + 1), new Coords(from.Rank, from.File - 1)];
            foreach (var victimSquare in victimSquares)
            {
                if (ruleset.IsInBounds(victimSquare))
                {
                    var victim = board.GetPiece(victimSquare);
                    if (victim != null && victim.Player.Id != fromPiece.Player.Id && victim.Behaviours.OfType<EnPassantVictimBehaviour>().Any())
                    {
                        yield return new Move { From = from, To = victimSquare.WithRank(victimSquare.Rank + direction) };
                    }
                }
            }
        }

        public Board ApplyMoveFrom(IRuleset ruleset, Board board, Board boardSoFar, Move move, PieceType fromPiece)
        {
            if (move.From.File != move.To.File && board.GetPiece(move.To) == null)
            {
                var victimSquare = move.To.WithRank(move.To.Rank - fromPiece.Player.Direction);
                return boardSoFar.ReplacePiece(victimSquare, null);
            }
            return boardSoFar;
        }
    }
}
