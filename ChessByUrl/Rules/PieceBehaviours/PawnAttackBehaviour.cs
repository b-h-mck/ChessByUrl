﻿namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Standard pawn move behaviour (1 square in a forward diagonal as long as that square has an opponent's piece).
    /// </summary>
    public class PawnAttackBehaviour : IGetLegalMovesBehaviour
    {
        public IEnumerable<Move> GetLegalMovesFrom(IRuleset ruleset, Board board, Coords from, PieceType fromPiece)
        {
            var direction = fromPiece.Player.Direction;
            var farthestRank = fromPiece.Player.FarthestRank;
            Coords[] captureSquares = [new Coords(from.Rank + direction, from.File + 1), new Coords(from.Rank + direction, from.File - 1)];
            foreach (var to in captureSquares)
            {
                if (ruleset.IsInBounds(to))
                {
                    var toPiece = board.GetPiece(to);
                    if (toPiece != null && toPiece.Player.Id != fromPiece.Player.Id)
                    {
                        yield return new Move { From = from, To = to };
                    }
                }
            }
        }
    }
}
