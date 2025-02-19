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

        public Player OpposingPlayer(Player player) => Players.First(p => p.Id != player.Id);


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
            var moves = GetLegalMovesWithoutCheckingCheck(board, from, board.CurrentPlayer);
            moves = moves.Where(move => !WouldPutCurrentPlayerInCheck(board, move));
            return moves;
        }

        private IEnumerable<Move> GetLegalMovesWithoutCheckingCheck(Board board, Coords from, Player player)
        {
            var piece = board.GetPiece(from) as OrthodoxPiece;
            if (piece == null || piece.Player.Id != player.Id)
            {
                return Enumerable.Empty<Move>();
            }
            var moves = piece.Behaviour.GetLegalMoves(this, board, from);
            return moves.OrderBy(move => move.To.Rank).ThenBy(move => move.To.File).ToArray();
        }


        public Board ApplyMove(Board board, Move move)
        {
            var newPlayer = Players.Single(p => p.Id != board.CurrentPlayer.Id);
            board = board.SetCurrentPlayer(newPlayer);

            var piece = board.GetPiece(move.From) as OrthodoxPiece;
            if (piece == null)
                return board;

            var boardAfterSpecialMove = piece.Behaviour.TryApplySpecialMove(this, board, move);
            if (boardAfterSpecialMove != null)
                return boardAfterSpecialMove;

            board = board.ReplacePiece(move.From, null);
            board = board.ReplacePiece(move.To, piece);
            return board;
        }

        public bool WouldPutCurrentPlayerInCheck(Board board, Move move)
        {
            var currentPlayer = board.CurrentPlayer;
            var opponentPlayer = OpposingPlayer(board.CurrentPlayer);
            var boardAfterMove = ApplyMove(board, move);
            var kingCoords = boardAfterMove
                .FindSquares(p => (p as OrthodoxPiece)?.Type == OrthodoxPieceType.King && p?.Player.Id == currentPlayer.Id)
                .FirstOrDefault();
            return kingCoords != null && IsThreatened(boardAfterMove, kingCoords, opponentPlayer);
        }

        public bool IsThreatened(Board board, Coords targetSquare, Player attacker)
        {
            var opponentPieceSquares = board.FindSquares(p => p != null && p.Player.Id == attacker.Id);
            var result = opponentPieceSquares.Any(from => GetLegalMovesWithoutCheckingCheck(board, from, attacker).Any(move => move.To == targetSquare));
            return result;
        }
    }

}