using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChessByUrl.Rules.Orthodox.Pieces
{
    public class KingBehaviour : IPieceBehaviour
    {
        public IEnumerable<Move> GetLegalMoves(Ruleset ruleset, Board board, Coords from)
        {
            return GetStandardMoves(board, from).Concat(GetCastlingMoves(ruleset, board, from));
        }

        private IEnumerable<Move> GetStandardMoves(Board board, Coords from)
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
            new Coords(1, 0),
            new Coords(0, 1),
            new Coords(-1, 0),
            new Coords(0, -1),
            new Coords(1, 1),
            new Coords(-1, 1),
            new Coords(1, -1),
            new Coords(-1, -1)
        ];

        private IEnumerable<Move> GetCastlingMoves(Ruleset ruleset, Board board, Coords from)
        {
            var kingSquare = new Coords(board.CurrentPlayer.Id == 0 ? 0 : 7, 4);
            if (from != kingSquare)
            {
                yield break;
            }
            ;
            if (CanCastle(ruleset, board, kingSquare, queenside: false))
            {
                yield return new Move { From = kingSquare, To = new Coords(kingSquare.Rank, 6) };
            }
            if (CanCastle(ruleset, board, kingSquare, queenside: true))
            {
                yield return new Move { From = kingSquare, To = new Coords(kingSquare.Rank, 2) };
            }
        }

        

        private bool CanCastle(Ruleset ruleset, Board board, Coords kingSquare, bool queenside)
        {
            var rookSquare = new Coords(kingSquare.Rank, queenside ? 0 : 7);
            if ((board.GetPiece(rookSquare) as OrthodoxPiece)?.Type != OrthodoxPieceType.RookWithCastlingRights)
            {
                return false;
            }

            var emptySquares = (queenside ? QueensideCastlingEmptyFiles : KingsideCastlingEmptyFiles)
                .Select(file => new Coords(kingSquare.Rank, file));
            if (emptySquares.Any(square => board.GetPiece(square) != null))
            {
                return false;
            }

            var nonThreatenedSquares = (queenside ? QueensideCastlingNonThreatenedFiles : KingsideCastlingNonThreatenedFiles)
                .Select(file => new Coords(kingSquare.Rank, file));
            if (nonThreatenedSquares.Any(square => ruleset.IsThreatened(board, square, ruleset.OpposingPlayer(board.CurrentPlayer))))
            {
                return false;
            }

            return true;
        }

        private static readonly int[] KingsideCastlingEmptyFiles = [5, 6];
        private static readonly int[] QueensideCastlingEmptyFiles = [1, 2, 3];
        private static readonly int[] KingsideCastlingNonThreatenedFiles = [4, 5, 6];
        private static readonly int[] QueensideCastlingNonThreatenedFiles = [2, 3, 4];

        //private bool IsSquareInCheck(Ruleset ruleset, Board board, Coords kingSquare, Coords square)
        //{
        //    if (square != kingSquare)
        //    {
        //        board = board.ReplacePiece(square, board.GetPiece(kingSquare));
        //        board = board.ReplacePiece(kingSquare, null);
        //    }
        //    var isCheck = ruleset.IsInCheck(board, board.CurrentPlayer);
        //    return isCheck;
        //}


        public Board? TryApplySpecialMove(Ruleset ruleset, Board board, Move move)
        {
            if (move.From.File == 4 && move.To.File == 6)
            {
                return ApplyCastling(board, move, queenside: false);
            }
            if (move.From.File == 4 && move.To.File == 2)
            {
                return ApplyCastling(board, move, queenside: true);
            }

            return RevokeCastlingRights(board, move);
        }

        private Board ApplyCastling(Board board, Move move, bool queenside)
        {
            var kingSquare = move.From;
            var rookFrom = new Coords(kingSquare.Rank, queenside ? 0 : 7);
            var rookTo = new Coords(kingSquare.Rank, queenside ? 3 : 5);

            board = board
                .ReplacePiece(kingSquare, null)
                .ReplacePiece(rookFrom, null)
                .ReplacePiece(move.To, board.GetPiece(kingSquare))
                .ReplacePiece(rookTo, board.GetPiece(rookFrom));

            return board;
        }

        private Board? RevokeCastlingRights(Board board, Move move)
        {
            var king = board.GetPiece(move.From) as OrthodoxPiece;
            if (king == null || move.From.File != 4 || move.From.Rank != (king.Player.Id == 0? 0 : 7))
            {
                return null;
            }
            var rookRank = move.From.Rank;
            Coords[] rookSquares = [new Coords(rookRank, 0), new Coords(rookRank, 7)];
            foreach (var square in rookSquares)
            {
                var piece = board.GetPiece(square) as OrthodoxPiece;
                if (piece?.Type == OrthodoxPieceType.RookWithCastlingRights && piece?.Player.Id == king.Player.Id)
                {
                    board = board.ReplacePiece(square, OrthodoxPiece.Create(king.Player, OrthodoxPieceType.Rook));
                }
            }
            board = board.ReplacePiece(move.To, board.GetPiece(move.From));
            board = board.ReplacePiece(move.From, null);
            return board;
        }



    }
}
