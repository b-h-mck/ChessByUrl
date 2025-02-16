namespace ChessByUrl.Rules.Standard.Pieces
{
    public class PawnBehaviour : IPieceBehaviour
    {

        public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
        {
            if (from.Rank < 1 || from.Rank > 6 || from.File < 0 || from.File > 7)
            {
                return Enumerable.Empty<Move>();
            }
            var direction = board.CurrentPlayer.Id == 0 ? new Coords(1, 0) : new Coords(-1, 0);
            var moves = GetStraightMoves(board, from, direction);
            moves = moves.Concat(GetAttackMoves(board, from, direction));
            moves = moves.SelectMany(move => GetPromotionMoves(board, move));
            return moves;
        }


        private IEnumerable<Move> GetStraightMoves(Board board, Coords from, Coords direction)
        {
            var startingRank = board.CurrentPlayer.Id == 0 ? 1 : 6;
            var to = from + direction;

            // If there's an empty square ahead, we can move there.
            if (board.GetPiece(to) == null)
            {
                yield return new Move { From = from, To = to };

                // If there's also an empty square two ahead and we're on the starting rank, we can also move there.
                if (from.Rank == startingRank)
                {
                    to = to + direction;
                    if (board.GetPiece(to) == null)
                    {
                        yield return new Move { From = from, To = to };
                    }
                }
            }
        }

        private IEnumerable<Move> GetAttackMoves(Board board, Coords from, Coords direction)
        {
            // Check each diagonal.
            foreach (var diagonalDirection in new[] { direction + new Coords(0, 1), direction + new Coords(0, -1) })
            {
                var to = from + diagonalDirection;
                if (to.File < 0 || to.File > 7)
                {
                    continue;
                }

                var piece = board.GetPiece(to) as StandardPiece;

                // If there's an opposite-coloured piece in the square, we can move there.
                if (piece != null && piece.Player.Id != board.CurrentPlayer.Id)
                {
                    yield return new Move { From = from, To = to };
                }
                // If the square is empty but there's an en passant target behind it, we can move there
                else if (piece == null)
                {
                    // En passant target's rank will be the same as the attacker's.
                    var enPassantTargetPiece = board.GetPiece(from) as StandardPiece;
                    if (enPassantTargetPiece != null 
                        && enPassantTargetPiece.Player.Id != board.CurrentPlayer.Id
                        && enPassantTargetPiece.Type == StandardPieceType.PawnWhoJustMovedTwoSquares)
                    {
                        yield return new Move { From = from, To = to };
                    }
                }
            }
        }

        private IEnumerable<Move> GetPromotionMoves(Board board, Move move)
        {
            if (move.To.Rank == 0 || move.To.Rank == 7)
            {
                foreach (var promotionType in new[] { StandardPieceType.Queen, StandardPieceType.Rook, StandardPieceType.Bishop, StandardPieceType.Knight })
                {
                    var promotionPiece = StandardPiece.Create(board.CurrentPlayer, promotionType);
                    yield return new Move { From = move.From, To = move.To, ChangeTo = promotionPiece };
                }
            }
            else
            {
                yield return move;
            }
        }

    }
}
