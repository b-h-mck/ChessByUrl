using ChessByUrl.Rules.Orthodox.Pieces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChessByUrl.Rules.Orthodox
{
    public class Ruleset : IRuleset
    {
        private IEnumerable<Player>? _players;
        public IEnumerable<Player> Players => _players ??=
        [
            new Player { Id = 0, Name = "White" },
            new Player { Id = 1, Name = "Black" }
        ];


        private IEnumerable<Piece>? _pieces;
        public IEnumerable<Piece> Pieces => _pieces ??=
            (from type in Enum.GetValues<OrthodoxPieceType>()
             from player in Players
             select OrthodoxPiece.Create(player, type)).ToList();

        public Piece GetOrthodoxPiece(OrthodoxPieceType pieceType, int playerId)
        {
            return Pieces.Cast<OrthodoxPiece>().Single(p => p.Player.Id == playerId && p.Type == pieceType);
        }


        public bool IsInBounds(Coords coords)
        {
            return coords.Rank >= 0 && coords.Rank <= 7 && coords.File >= 0 && coords.File <= 7;
        }

        public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
        {
            var piece = board.GetPiece(from) as OrthodoxPiece;
            if (piece == null || piece.Player.Id != board.CurrentPlayer.Id)
            {
                return Enumerable.Empty<Move>();
            }

            IEnumerable<Move> moves = piece.Behaviour.GetLegalMoves(board, from);

            //moves = moves.Where(move => !WouldPutCurrentPlayerInCheck(board, move));
            return moves.OrderBy(move => move.To.Rank).ThenBy(move => move.To.File).ToArray();
        }


        public Board ApplyMove(Board board, Move move)
        {
            var newPlayer = Players.Single(p => p.Id != board.CurrentPlayer.Id);
            board = board.SetCurrentPlayer(newPlayer);

            var piece = board.GetPiece(move.From) as OrthodoxPiece;
            if (piece == null)
                return board;

            var boardAfterSpecialMove = piece.Behaviour.TryApplySpecialMove(board, move);
            if (boardAfterSpecialMove != null)
                return boardAfterSpecialMove;

            board = board.ReplacePiece(move.From, null);
            board = board.ReplacePiece(move.To, piece);
            return board;
        }
    }

}