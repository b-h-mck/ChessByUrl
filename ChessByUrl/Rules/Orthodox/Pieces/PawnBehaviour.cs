namespace ChessByUrl.Rules.Orthodox.Pieces
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
            moves = moves.Select(move => AdjustForTwoSquareMoves(board, move));
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
                        var twoSquarePawn = OrthodoxPiece.Create(board.CurrentPlayer, OrthodoxPieceType.PawnWhoJustMovedTwoSquares);
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

                var piece = board.GetPiece(to) as OrthodoxPiece;

                // If there's an opposite-coloured piece in the square, we can move there.
                if (piece != null && piece.Player.Id != board.CurrentPlayer.Id)
                {
                    yield return new Move { From = from, To = to };
                }
                // If the square is empty but there's an en passant target behind it, we can move there
                else if (piece == null)
                {
                    // En passant target's rank will be the same as the attacker's.
                    var enPassantTargetPiece = board.GetPiece(new Coords(from.Rank, to.File)) as OrthodoxPiece;
                    if (enPassantTargetPiece != null 
                        && enPassantTargetPiece.Player.Id != board.CurrentPlayer.Id
                        && enPassantTargetPiece.Type == OrthodoxPieceType.PawnWhoJustMovedTwoSquares)
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
                foreach (var promotionType in new[] { OrthodoxPieceType.Queen, OrthodoxPieceType.Rook, OrthodoxPieceType.Bishop, OrthodoxPieceType.Knight })
                {
                    var promotionPiece = OrthodoxPiece.Create(board.CurrentPlayer, promotionType);
                    yield return new Move { From = move.From, To = move.To, ChangeTo = promotionPiece };
                }
            }
            else
            {
                yield return move;
            }
        }

        private Move AdjustForTwoSquareMoves(Board board, Move move)
        {
            var isTwoSquarePawn = (board.GetPiece(move.From) as OrthodoxPiece)?.Type == OrthodoxPieceType.PawnWhoJustMovedTwoSquares;
            var isTwoSquareMove = Math.Abs(move.From.Rank - move.To.Rank) == 2;
            if (isTwoSquarePawn && !isTwoSquareMove)
            {
                return new Move { From = move.From, To = move.To, ChangeTo = OrthodoxPiece.Create(board.CurrentPlayer, OrthodoxPieceType.Pawn) };
            }
            if (!isTwoSquarePawn && isTwoSquareMove)
            {
                return new Move { From = move.From, To = move.To, ChangeTo = OrthodoxPiece.Create(board.CurrentPlayer, OrthodoxPieceType.PawnWhoJustMovedTwoSquares) };
            }
            return move;
        }

        public Board? TryApplySpecialMove(Board board, Move move)
        {
            if (move.ChangeTo != null) // Promotion or double move
            {
                board = board.ReplacePiece(move.From, null);
                board = board.ReplacePiece(move.To, move.ChangeTo);
                return board;
            }
            else if (move.From.File != move.To.File && board.GetPiece(move.To) == null) // En passant
            {
                var pawn = board.GetPiece(move.From) as OrthodoxPiece;
                var victimCoords = new Coords(move.From.Rank, move.To.File);
                board = board.ReplacePiece(move.From, null);
                board = board.ReplacePiece(victimCoords, null);
                board = board.ReplacePiece(move.To, pawn);
                return board;
            }
            return null;
        }

    }
}
