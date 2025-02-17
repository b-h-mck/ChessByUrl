using ChessByUrl.Rules;
using ChessByUrl.Rules.Orthodox;
using ChessByUrl.Rules.Orthodox.Pieces;

namespace ChessByUrl.Parser.Orthodox
{
    public class StartBoardParser : IBoardParser
    {
        public StartBoardParser() { }

        public string? Serialise(IRuleset ruleset, Board board)
        {
            //if (ruleset is Ruleset)
            //    return "s";
            return null;
        }

        public Board? Parse(IRuleset ruleset, string boardString)
        {
            var orthodoxRuleset = ruleset as Ruleset;

            if (orthodoxRuleset == null || boardString != "s") 
                return null;

            var ranks = new List<BoardRank>(8);
            ranks.Add(GetBackRank(orthodoxRuleset, 0));
            ranks.Add(GetPawnRank(orthodoxRuleset, 0));
            ranks.AddRange(Enumerable.Range(2, 4).Select(rank => new BoardRank(Enumerable.Repeat<Piece?>(null, 8).ToArray())));
            ranks.Add(GetPawnRank(orthodoxRuleset, 1));
            ranks.Add(GetBackRank(orthodoxRuleset, 1));

            return new Board(ruleset.Players.First(), new BoardRanks(ranks));
        }

        private static readonly OrthodoxPieceType[] BackRank =
        [
            OrthodoxPieceType.RookWithCastlingRights,
            OrthodoxPieceType.Knight,
            OrthodoxPieceType.Bishop,
            OrthodoxPieceType.Queen,
            OrthodoxPieceType.King,
            OrthodoxPieceType.Bishop,
            OrthodoxPieceType.Knight,
            OrthodoxPieceType.RookWithCastlingRights
        ];
        private static BoardRank GetBackRank(Ruleset ruleset, int playerId)
        {
            return new BoardRank(BackRank.Select(type => ruleset.GetOrthodoxPiece(type, playerId)).ToArray());
        }

        private static BoardRank GetPawnRank(Ruleset ruleset, int playerId)
        {
            return new BoardRank(Enumerable.Repeat(ruleset.GetOrthodoxPiece(OrthodoxPieceType.Pawn, playerId), 8).ToArray());
        }

    }
}
