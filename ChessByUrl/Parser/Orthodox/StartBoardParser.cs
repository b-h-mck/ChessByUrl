using ChessByUrl.Rules;
using ChessByUrl.Rules.Rulesets.Orthodox;

namespace ChessByUrl.Parser.Orthodox
{
    public class StartBoardParser : IBoardParser
    {
        public StartBoardParser() { }

        public string? Serialise(IRuleset ruleset, Board board)
        {
            var startBoard = Parse(ruleset, "s");
            if (startBoard?.SequenceEqual(board) ?? false)
            {
                return "s";
            }
            return null;
        }

        public Board? Parse(IRuleset ruleset, string boardString)
        {
            var orthodoxRuleset = ruleset as OrthodoxRuleset;

            if (orthodoxRuleset == null || boardString != "s")
                return null;

            var ranks = new List<BoardRank>(8);
            ranks.Add(GetBackRank(0));
            ranks.Add(GetPawnRank(0));
            ranks.AddRange(Enumerable.Range(2, 4).Select(rank => new BoardRank(Enumerable.Repeat<PieceType?>(null, 8).ToArray())));
            ranks.Add(GetPawnRank(1));
            ranks.Add(GetBackRank(1));

            return new Board(ruleset.Players.First(), new BoardRanks(ranks));
        }

        private static BoardRank GetBackRank(int playerId)
        {
            var pieces = playerId == 0 ? OrthodoxPieceTypes.White : OrthodoxPieceTypes.Black;
            PieceType[] rank =
                [
                    pieces.RookWithCastlingRights,
                    pieces.Knight,
                    pieces.Bishop,
                    pieces.Queen,
                    pieces.King,
                    pieces.Bishop,
                    pieces.Knight,
                    pieces.RookWithCastlingRights
                ];
            return new BoardRank(rank);
        }

        private static BoardRank GetPawnRank(int playerId)
        {
            var pieces = playerId == 0 ? OrthodoxPieceTypes.White : OrthodoxPieceTypes.Black;
            var rank = Enumerable.Repeat(pieces.Pawn, 8).ToArray();
            return new BoardRank(rank);
        }


    }
}
