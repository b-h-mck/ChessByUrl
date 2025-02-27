namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Represents the behaviour of a Rider i.e. a piece that moves like a Leaper, but can then continue
    /// in the same direction for as long as it encounters empty squares. It can optionally capture an 
    /// enemy piece at the end of its ride.
    /// </summary>
    /// <remarks>
    /// In orthodox chess this is the behaviour of the Rook [(1,0)], Bishop [(1,1)], and Queen [(1,0)+(1,1)].
    /// </remarks>
    public class RiderBehaviour : IGetLegalMovesBehaviour
    {
        public RiderBehaviour(LeaperBehaviour initialLeaper)
        {
            InitialLeaper = initialLeaper;
        }


        public LeaperBehaviour InitialLeaper;

        public IEnumerable<Move> GetLegalMovesFrom(IRuleset ruleset, Board board, Coords from, PieceType fromPiece)
        {
            // Find the first leaps for the inner leaper, then for each of them continue in the same direction
            // until we hit something.
            var firstLeaps = InitialLeaper.GetLegalMovesFrom(ruleset, board, from, fromPiece);
            foreach (var firstLeap in firstLeaps)
            {
                var to = firstLeap.To;
                var offset = firstLeap.To - firstLeap.From;
                while (ruleset.IsInBounds(to))
                {
                    var toPiece = board.GetPiece(to);
                    if (toPiece == null)
                    {
                        yield return new Move { From = from, To = to };
                    }
                    else if (toPiece.Player.Id != fromPiece.Player.Id)
                    {
                        yield return new Move { From = from, To = to };
                        break;
                    }
                    else
                    {
                        break;

                    }
                    to += offset;
                }
            }
        }
    }
}
