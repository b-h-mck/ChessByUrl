using ChessByUrl.Rules;
using ChessByUrl.Rules.Rulesets.Orthodox;
using ChessByUrl.Utils;

namespace ChessByUrl.Parser.Orthodox
{
    /// <summary>
    /// A board parser that run-length encodes empty squares. Inspired by the FEN format.
    /// </summary>
    public class RleBoardParser : IBoardParser
    {
        public RleBoardParser() { }

        public string? Serialise(IRuleset ruleset, Board board)
        {
            var orthodoxRuleset = ruleset as OrthodoxRuleset;
            if (orthodoxRuleset == null)
                return null;

            var byteWriter = new PackedByteWriter();

            byteWriter.Write(board.CurrentPlayer.Id, 0, ruleset.Players.Max(player => player.Id));

            var maxPieceId = ruleset.PieceTypes.Max(piece => piece.Id);
            var emptySquares = 0;
            var squares = board.Ranks.SelectMany(rank => rank).ToList();
            foreach (var piece in squares)
            {
                if (piece == null)
                {
                    emptySquares++;
                }
                else
                {
                    WriteEmptyRuns(byteWriter, emptySquares);
                    emptySquares = 0;
                    byteWriter.Write(1, 0, 1);
                    byteWriter.Write(piece.Id, 0, maxPieceId);
                }
            }
            WriteEmptyRuns(byteWriter, emptySquares);

            var boardString = byteWriter.ToBase64();
            return $"r{boardString}";
        }

        private static void WriteEmptyRuns(PackedByteWriter byteWriter, int emptySquares)
        {
            while (emptySquares > 0)
            {
                int count = Math.Min(emptySquares, MaxRunLength);
                byteWriter.Write(0, 0, 1);
                byteWriter.Write(count, MinRunLength, MaxRunLength);
                emptySquares -= count;
            }
        }

        private const int MinRunLength = 1;
        private const int MaxRunLength = 8;

        public Board? Parse(IRuleset ruleset, string boardString)
        {
            if (!boardString.StartsWith("r"))
                return null;

            var orthodoxRuleset = ruleset as OrthodoxRuleset;
            if (orthodoxRuleset == null)
                return null;

            var byteReader = new PackedByteReader(boardString.Substring(1));
            var squares = new List<PieceType?>();

            var currentPlayerId = byteReader.Read(0, ruleset.Players.Max(player => player.Id));

            int? typeBit = byteReader.Read(0, 1);
            while (typeBit != null)
            {
                if (typeBit == 0)
                {
                    var runLength = byteReader.Read(MinRunLength, MaxRunLength);
                    for (int i = 0; i < runLength; i++)
                    {
                        squares.Add(null);
                    }
                }
                else
                {
                    var pieceId = byteReader.Read(0, ruleset.PieceTypes.Max(piece => piece.Id));
                    squares.Add(ruleset.PieceTypes.First(piece => piece.Id == pieceId));
                }
                typeBit = byteReader.Read(0, 1);
            }

            var ranks = new BoardRanks(squares.Chunk(8).Select(pieces => new BoardRank(pieces)).ToList());

            return new Board(ruleset.Players.First(player => player.Id == currentPlayerId), ranks);
        }
    }
}
