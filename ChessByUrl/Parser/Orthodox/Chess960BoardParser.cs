using ChessByUrl.Rules;
using ChessByUrl.Rules.Rulesets.Orthodox;

namespace ChessByUrl.Parser.Orthodox
{
    public class Chess960BoardParser : IBoardParser
    {
        public string? Serialise(IRuleset ruleset, Board board)
        {
            return null;
        }


        public Board? Parse(IRuleset ruleset, string boardString)
        {
            var orthodoxRuleset = ruleset as OrthodoxRuleset;

            if (orthodoxRuleset == null || !boardString.StartsWith("f"))
                return null;

            int positionNumber;
            if (!int.TryParse(boardString.Substring(1), out positionNumber) || positionNumber < 0 || positionNumber >= 960)
                return null;


            var ranks = new List<BoardRank>(8);
            ranks.Add(GetBackRank(0, positionNumber));
            ranks.Add(GetPawnRank(0));
            ranks.AddRange(Enumerable.Range(2, 4).Select(rank => new BoardRank(Enumerable.Repeat<PieceType?>(null, 8).ToArray())));
            ranks.Add(GetPawnRank(1));
            ranks.Add(GetBackRank(1, positionNumber));

            return new Board(ruleset.Players.First(), new BoardRanks(ranks));
        }

        private static BoardRank GetPawnRank(int playerId)
        {
            var pieces = playerId == 0 ? OrthodoxPieceTypes.White : OrthodoxPieceTypes.Black;
            var rank = Enumerable.Repeat(pieces.Pawn, 8).ToArray();
            return new BoardRank(rank);
        }

        private static BoardRank GetBackRank(int playerId, int positionNumber)
        {
            var pieceSet = OrthodoxPieceTypes.Player(playerId);
            PieceType?[] rank = new PieceType?[8];
            var n = positionNumber;

            (n, var lightBishopPosition) = Math.DivRem(n, 4);
            (n, var darkBishopPosition) = Math.DivRem(n, 4);
            (n, var queenPosition) = Math.DivRem(n, 6);
            (var knight1Position, var knight2Position) = GetKnightPositions(n);

            rank[(lightBishopPosition + 1) * 2] = pieceSet.Bishop;
            rank[(darkBishopPosition + 1) * 2 + 1] = pieceSet.Bishop;
            rank[GetNthEmptyIndex(rank, queenPosition)] = pieceSet.Queen;
            rank[GetNthEmptyIndex(rank, knight1Position)] = pieceSet.Knight;
            rank[GetNthEmptyIndex(rank, knight2Position)] = pieceSet.Knight;
            rank[GetNthEmptyIndex(rank, 0)] = pieceSet.RookWithCastlingRights;
            rank[GetNthEmptyIndex(rank, 0)] = pieceSet.King;
            rank[GetNthEmptyIndex(rank, 0)] = pieceSet.RookWithCastlingRights;

            return new BoardRank(rank);
        }


        private static (int, int) GetKnightPositions(int n) =>
            n switch
            {
                0 => (0, 1),
                1 => (0, 2),
                2 => (0, 3),
                3 => (0, 4),
                4 => (1, 2),
                5 => (1, 3),
                6 => (1, 4),
                7 => (2, 3),
                8 => (2, 4),
                9 => (3, 4),
                _ => throw new ArgumentOutOfRangeException(nameof(n), "Should be 0-9")
            };

        private static int GetNthEmptyIndex(PieceType?[] rank, int n)
        {
            for (int i = 0; i < rank.Length; i++)
            {
                if (rank[i] == null)
                {
                    if (n == 0)
                    {
                        return i;
                    }
                    n--;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(n), "Not enough empty spaces in rank");
        }
    }
}
