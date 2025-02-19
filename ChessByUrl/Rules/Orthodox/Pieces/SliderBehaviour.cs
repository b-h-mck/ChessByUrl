using System.Runtime.CompilerServices;

namespace ChessByUrl.Rules.Orthodox.Pieces
{
    public class SliderBehaviour : IPieceBehaviour
    {
        public SliderBehaviour(bool orthogonal = false, bool diagonal = false)
        {
            Orthogonal = orthogonal;
            Diagonal = diagonal;
        }
        public bool Orthogonal { get; private set; }
        public bool Diagonal { get; private set; }

        public IEnumerable<Move> GetLegalMoves(Ruleset ruleset, Board board, Coords from)
        {
            var moves = new List<Move>();
            if (Orthogonal)
            {
                foreach (var direction in OrthogonalDirections)
                {
                    AddMovesInDirection(board, from, direction, moves);
                }
            }
            if (Diagonal)
            {
                foreach (var direction in DiagonalDirections)
                {
                    AddMovesInDirection(board, from, direction, moves);
                }
            }
            return moves;
        }

        private static readonly Coords[] OrthogonalDirections =
        [
            new Coords(1, 0),
            new Coords(0, 1),
            new Coords(-1, 0),
            new Coords(0, -1)
        ];

        private static readonly Coords[] DiagonalDirections =
        [
            new Coords(1, 1),
            new Coords(-1, 1),
            new Coords(1, -1),
            new Coords(-1, -1)
        ];

        private static void AddMovesInDirection(Board board, Coords from, Coords direction, List<Move> moves)
        {
            var to = new Coords(from.Rank + direction.Rank, from.File + direction.File);
            while (to.Rank >= 0 && to.Rank <= 7 && to.File >= 0 && to.File <= 7)
            {
                var piece = board.GetPiece(to) as OrthodoxPiece;

                // If there's a piece here, add it if it's a different colour. In either case, stop.
                if (piece != null)
                {
                    if (piece.Player.Id != board.CurrentPlayer.Id)
                    {
                        moves.Add(new Move { From = from, To = to });
                    }
                    break;
                }

                // Square is empty, so add it and keep going.
                moves.Add(new Move { From = from, To = to });
                to = new Coords(to.Rank + direction.Rank, to.File + direction.File);
            }
        }

        public Board? TryApplySpecialMove(Ruleset ruleset, Board board, Move move)
        {
            return null;
        }
    }
}
