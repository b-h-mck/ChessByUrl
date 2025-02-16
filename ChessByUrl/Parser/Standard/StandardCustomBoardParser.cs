using ChessByUrl.Rules;
using ChessByUrl.Rules.Standard;
using ChessByUrl.Rules.Standard.Pieces;
using ChessByUrl.Utils;

namespace ChessByUrl.Parser.Standard
{
    public class StandardCustomBoardParser : IBoardParser
    {
        public StandardCustomBoardParser() { }

        public string? Serialise(IRuleset ruleset, Board board)
        {
            var standardRuleset = ruleset as StandardRuleset;
            if (standardRuleset == null)
                return null;
            var maxPieceId = ruleset.Pieces.Max(piece => piece.Id);
            var maxPlayerId = ruleset.Players.Max(player => player.Id);

            var byteWriter = new PackedByteWriter();
            byteWriter.Write(board.CurrentPlayer.Id, 0, maxPlayerId);

            for (var rank = 0; rank < 8; rank++)
            {
                for (var file = 0; file < 8; file++)
                {
                    var piece = board.GetPiece(new Coords(rank, file)) as StandardPiece;
                    if (piece != null)
                    {
                        byteWriter.Write(piece.Id, 0, maxPieceId);
                        byteWriter.Write(rank, 0, 8);
                        byteWriter.Write(file, 0, 8);
                    }
                }
            }

            var bytes = byteWriter.Bytes;
            var base64String = Convert.ToBase64String(bytes);
            var urlFriendlyBase64 = base64String.Replace('+', '-').Replace('/', '_').TrimEnd('=');

            return urlFriendlyBase64;
        }

        public Board? Parse(IRuleset ruleset, string boardString)
        {

            var standardRuleset = ruleset as StandardRuleset;
            if (standardRuleset == null)
                return null;
            var maxPieceId = ruleset.Pieces.Max(piece => piece.Id);
            var maxPlayerId = ruleset.Players.Max(player => player.Id);

            var base64String = boardString.Replace('-', '+').Replace('_', '/').PadRight(boardString.Length + (4 - boardString.Length % 4) % 4, '=');
            var bytes = Convert.FromBase64String(base64String);
            var byteReader = new PackedByteReader(bytes);
            var currentPlayerId = byteReader.Read(0, maxPlayerId);
            var currentPlayer = ruleset.Players.First(player => player.Id == currentPlayerId);

            Piece?[][] squares = new Piece?[8][];
            for (int rank = 0; rank < squares.Length; rank++)
            {
                squares[rank] = new Piece?[8];
            }

            var pieceId = byteReader.Read(0, maxPieceId);
            while (pieceId != null)
            {
                var rank = byteReader.Read(0, 8)!.Value;
                var file = byteReader.Read(0, 8)!.Value;
                squares[rank][file] = ruleset.Pieces.First(piece => piece.Id == pieceId);

                pieceId = byteReader.Read(0, maxPieceId);
            }

            return new Board(currentPlayer, new BoardRanks(squares.Select(rank => new BoardRank(rank)).ToArray()));

        }
    }
}
