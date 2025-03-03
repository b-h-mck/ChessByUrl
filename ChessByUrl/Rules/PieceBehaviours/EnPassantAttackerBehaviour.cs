namespace ChessByUrl.Rules.PieceBehaviours
{
    public class EnPassantAttackerBehaviour : IGetLegalMovesBehaviour, IApplyMoveBehaviour
    {
        public IEnumerable<Move> GetLegalMovesFrom(Game game, Coords from, PieceType fromPiece)
        {
            var direction = fromPiece.Player.Direction;
            var farthestRank = fromPiece.Player.FarthestRank;
            Coords[] victimSquares = [new Coords(from.Rank, from.File + 1), new Coords(from.Rank, from.File - 1)];
            foreach (var victimSquare in victimSquares)
            {
                if (game.Ruleset.IsInBounds(victimSquare))
                {
                    var victim = game.CurrentBoard.GetPiece(victimSquare);
                    if (victim != null && victim.Player.Id != fromPiece.Player.Id && victim.Behaviours.OfType<EnPassantVictimBehaviour>().Any())
                    {
                        yield return new Move { From = from, To = victimSquare.WithRank(victimSquare.Rank + direction) };
                    }
                }
            }
        }

        public Board ApplyMoveFrom(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, PieceType fromPiece)
        {
            if (move.From.File != move.To.File && gameBeforeMove.CurrentBoard.GetPiece(move.To) == null)
            {
                var victimSquare = move.To.WithRank(move.To.Rank - fromPiece.Player.Direction);
                return boardAfterMoveSoFar.ReplacePiece(victimSquare, null);
            }
            return boardAfterMoveSoFar;
        }

    }
}
