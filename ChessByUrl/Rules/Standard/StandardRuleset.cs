
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChessByUrl.Rules.Standard
{
    public class StandardRuleset : IRuleset
    {
        private IEnumerable<Player>? _players;
        public IEnumerable<Player> Players => _players ??=
        [
            new Player { Id = 0, Name = "White" },
            new Player { Id = 1, Name = "Black" }
        ];

        public Coords MaxRankFile => new Coords(7, 7);

        private IEnumerable<Piece>? _pieces;
        public IEnumerable<Piece> Pieces => _pieces ??=
            (from type in Enum.GetValues<StandardPieceType>()
             from player in Players
             select StandardPiece.Create(player, type)).ToList();



        public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
        {
            var piece = board.Squares[from.Rank, from.File] as StandardPiece;
            if (piece == null || piece.Player.Id != board.CurrentPlayer.Id)
            {
                return Enumerable.Empty<Move>();
            }

            IEnumerable<Move> moves = piece.Type switch
            {
                StandardPieceType.Pawn or StandardPieceType.PawnWhoJustMovedTwoSquares => GetPawnMoves(board, from),
                StandardPieceType.Knight => GetKnightMoves(board, from),
                StandardPieceType.Bishop => GetSliderMoves(board, from, diagonal: true),
                StandardPieceType.Rook or StandardPieceType.RookWithCastlingRights => GetSliderMoves(board, from, orthogonal: true),
                StandardPieceType.Queen => GetSliderMoves(board, from, orthogonal: true, diagonal: true),
                StandardPieceType.King => GetKingMoves(board, from),
                _ => throw new ArgumentOutOfRangeException(nameof(piece.Type))
            };

            moves = moves.Where(move => !WouldPutCurrentPlayerInCheck(board, move));
            return moves.OrderBy(move => move.To.Rank).ThenBy(move => move.To.File).ToArray();
        }


        public Board ApplyMove(Board board, Move move)
        {
            throw new NotImplementedException();
        }
    }

}