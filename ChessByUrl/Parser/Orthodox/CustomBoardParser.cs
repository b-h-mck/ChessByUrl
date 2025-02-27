using ChessByUrl.Rules;
using ChessByUrl.Rules.Rulesets.Orthodox;
using ChessByUrl.Utils;

namespace ChessByUrl.Parser.Orthodox
{
    public class CustomBoardParser : IBoardParser
    {
        public CustomBoardParser() { }

        public string? Serialise(IRuleset ruleset, Board board)
        {
            var orthodoxRuleset = ruleset as OrthodoxRuleset;
            if (orthodoxRuleset == null)
                return null;
            var maxPieceId = ruleset.PieceTypes.Max(piece => piece.Id);
            var eofId = maxPieceId + 1;
            var maxPlayerId = ruleset.Players.Max(player => player.Id);

            var byteWriter = new PackedByteWriter();
            byteWriter.Write(board.CurrentPlayer.Id, 0, maxPlayerId);

            for (var rank = 0; rank < 8; rank++)
            {
                for (var file = 0; file < 8; file++)
                {
                    var piece = board.GetPiece(new Coords(rank, file));
                    if (piece != null)
                    {
                        byteWriter.Write(piece.Id, 0, eofId);
                        byteWriter.Write(rank, 0, 8);
                        byteWriter.Write(file, 0, 8);
                    }
                }
            }
            byteWriter.Write(eofId, 0, eofId);
            var boardString = BytesToUrlFriendlyBase64(byteWriter.Bytes);

            return $"c{boardString}";
        }

        public Board? Parse(IRuleset ruleset, string boardString)
        {
            if (!boardString.StartsWith("c"))
                return null;

            var orthodoxRuleset = ruleset as OrthodoxRuleset;
            if (orthodoxRuleset == null)
                return null;
            var maxPieceId = ruleset.PieceTypes.Max(piece => piece.Id);
            var eofId = maxPieceId + 1;
            var maxPlayerId = ruleset.Players.Max(player => player.Id);

            var bytes = UrlFriendlyBase64ToBytes(boardString.Substring(1));
            var byteReader = new PackedByteReader(bytes);
            var currentPlayerId = byteReader.Read(0, maxPlayerId);
            var currentPlayer = ruleset.Players.First(player => player.Id == currentPlayerId);

            PieceType?[][] squares = new PieceType?[8][];
            for (int rank = 0; rank < squares.Length; rank++)
            {
                squares[rank] = new PieceType?[8];
            }

            var pieceId = byteReader.Read(0, eofId);
            while (pieceId != null && pieceId != eofId)
            {
                var rank = byteReader.Read(0, 8)!.Value;
                var file = byteReader.Read(0, 8)!.Value;
                squares[rank][file] = ruleset.PieceTypes.First(piece => piece.Id == pieceId);

                pieceId = byteReader.Read(0, maxPieceId + 1);
            }

            return new Board(currentPlayer, new BoardRanks(squares.Select(rank => new BoardRank(rank)).ToArray()));

        }

        private static string BytesToUrlFriendlyBase64(byte[] bytes)
        {
            var base64 = Convert.ToBase64String(bytes);
            return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        private static byte[] UrlFriendlyBase64ToBytes(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            int paddingNeeded = 4 - (base64.Length % 4);
            if (paddingNeeded < 4)
            {
                base64 = base64.PadRight(base64.Length + paddingNeeded, '=');
            }
            Convert.TryFromBase64Chars(base64, new Span<byte>(new byte[base64.Length]), out int bytesWritten);
            return Convert.FromBase64String(base64);
        }
    }
}
