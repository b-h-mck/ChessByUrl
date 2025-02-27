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


        public IEnumerable<Move> GetLegalMovesFrom(IRuleset ruleset, Board board, Coords from, PieceType fromPiece)
        {
            var direction = fromPiece.Player.Direction;
            var farthestRank = fromPiece.Player.FarthestRank;
            var to = new Coords(from.Rank + direction, from.File);
            if (!ruleset.IsInBounds(to))
            {
                yield break;
            }
            
            var piece = board.GetPiece(to);
            if (piece == null)
            {
                yield return new Move { From = from, To = to };
            }
        }
    }
}
