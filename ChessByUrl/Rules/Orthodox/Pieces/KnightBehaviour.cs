namespace ChessByUrl.Rules.Orthodox.Pieces
{
    public class KnightBehaviour : IPieceBehaviour
    {
        public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
        {
            foreach (var direction in Directions)
            {
                var to = new Coords(from.Rank + direction.Rank, from.File + direction.File);
                if (to.Rank < 0 || to.Rank > 7 || to.File < 0 || to.File > 7)
                {
                    continue;
                }
                var toPiece = board.GetPiece(to);
                if (toPiece == null || toPiece.Player.Id != board.CurrentPlayer.Id)
                {
                    yield return new Move { From = from, To = to };
                }
            }
        }

        private static readonly Coords[] Directions =
        [
            new Coords(2, 1),
            new Coords(1, 2),
            new Coords(-2, 1),
            new Coords(-1, 2),
            new Coords(2, -1),
            new Coords(1, -2),
            new Coords(-2, -1),
            new Coords(-1, -2)
        ];
    }
}
