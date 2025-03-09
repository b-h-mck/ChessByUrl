namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Standard pawn move behaviour (1 square forward as long as that square is unoccupied).
    /// </summary>
    public class PawnStandardMoveBehaviour : IGetLegalMovesBehaviour
    {
        public PawnStandardMoveBehaviour()
        {
        }

        public IEnumerable<Move> GetLegalMovesFrom(Game game, Coords from, PieceType fromPiece)
        {
            var direction = fromPiece.Player.Direction;
            var farthestRank = fromPiece.Player.FarthestRank;
            var to = new Coords(from.Rank + direction, from.File);
            if (!game.Ruleset.IsInBounds(to))
            {
                return [];
            }

            var piece = game.CurrentBoard.GetPiece(to);
            if (piece == null)
            {
                return [new Move(from, to)];
            }
            return [];
        }
    }
}
