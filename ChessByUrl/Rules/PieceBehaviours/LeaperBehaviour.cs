
using System.Collections.Immutable;

namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Represents the behaviour of a Leaper i.e. a piece that can move a certain distance in any direction,
    /// then a certain distance orthogonally, without regard to any pieces in between.
    /// </summary>
    /// <remarks>
    /// In orthodox chess this is the behaviour of Knights [(2,1)]. Kings also use two instances of this behaviour [(1,0)+(1,1)].
    /// </remarks>
    public class LeaperBehaviour : IGetLegalMovesBehaviour
    {
        public LeaperBehaviour(int firstDistance, int secondDistance)
        {
            AllOffsets = GetOffsets(firstDistance, secondDistance).Distinct();
        }

        public IEnumerable<Coords> AllOffsets;

        private IEnumerable<Coords> GetOffsets(int firstDistance, int secondDistance)
        {
            return [
                    new Coords(firstDistance, secondDistance),
                    new Coords(firstDistance, -secondDistance),
                    new Coords(-firstDistance, secondDistance),
                    new Coords(-firstDistance, -secondDistance),
                    new Coords(secondDistance, firstDistance),
                    new Coords(secondDistance, -firstDistance),
                    new Coords(-secondDistance, firstDistance),
                    new Coords(-secondDistance, -firstDistance)
                ];
        }

        public IEnumerable<Move> GetLegalMovesFrom(Game game, Coords from, PieceType fromPiece)
        {
            var legalMoves = new List<Move>();
            var candidates = AllOffsets.Select(offset => from + offset);
            foreach (var to in candidates)
            {
                if (game.Ruleset.IsInBounds(to))
                {
                    var toPiece = game.CurrentBoard.GetPiece(to);
                    if (toPiece == null || toPiece?.Player.Id != fromPiece.Player.Id)
                    {
                        legalMoves.Add(new Move(from, to));
                    }
                }
            }
            return legalMoves;
        }
    }
}
