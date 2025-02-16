using ChessByUrl.Rules;
using ChessByUrl.Rules.Standard;
using ChessByUrl.Rules.Standard.Pieces;

namespace ChessByUrl.Parser.Standard
{
    public class StandardStartBoardParser : IBoardParser
    {
        public StandardStartBoardParser() { }

        public string? Serialise(IRuleset ruleset, Board board)
        {
            if (ruleset is StandardRuleset)
                return "d";
            return null;
        }

        public Board? Parse(IRuleset ruleset, string boardString)
        {
            var standardRuleset = ruleset as StandardRuleset;

            if (standardRuleset == null || boardString != "d") 
                return null;

            var ranks = new List<BoardRank>(8);
            ranks.Add(GetBackRank(standardRuleset, 0));
            ranks.Add(GetPawnRank(standardRuleset, 0));
            ranks.AddRange(Enumerable.Range(2, 4).Select(rank => new BoardRank(Enumerable.Repeat<Piece?>(null, 8).ToArray())));
            ranks.Add(GetPawnRank(standardRuleset, 1));
            ranks.Add(GetBackRank(standardRuleset, 1));

            return new Board(ruleset.Players.First(), new BoardRanks(ranks));
        }

        private static readonly StandardPieceType[] BackRank =
        [
            StandardPieceType.RookWithCastlingRights,
            StandardPieceType.Knight,
            StandardPieceType.Bishop,
            StandardPieceType.Queen,
            StandardPieceType.King,
            StandardPieceType.Bishop,
            StandardPieceType.Knight,
            StandardPieceType.RookWithCastlingRights
        ];
        private static BoardRank GetBackRank(StandardRuleset ruleset, int playerId)
        {
            return new BoardRank(BackRank.Select(type => ruleset.GetStandardPiece(type, playerId)).ToArray());
        }

        private static BoardRank GetPawnRank(StandardRuleset ruleset, int playerId)
        {
            return new BoardRank(Enumerable.Repeat(ruleset.GetStandardPiece(StandardPieceType.Pawn, playerId), 8).ToArray());
        }

    }
}
