using ChessByUrl.Parser.Orthodox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessByUrl.Rules;
using ChessByUrl.Rules.Rulesets.Orthodox;
namespace ChessByUrl.Tests.Parser.Orthodox
{
    [TestClass]
    public class StartBoardParserTests
    {

        [TestMethod]
        public void Serialise_OtherRuleset_ReturnsNull()
        {
            var parser = new StartBoardParser();
            var ruleset = new OtherRuleset();
            var board = new Board(new Player { Id = 0, Name = "White", ClosestRank=0, FarthestRank=0 }, new BoardRanks(new List<BoardRank>()));
            var result = parser.Serialise(ruleset, board);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_OtherRulesetAndCorrectString_ReturnsNull()
        {
            var parser = new StartBoardParser();
            var ruleset = new OtherRuleset();
            var result = parser.Parse(ruleset, "s");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_OrthodoxRulesetAndIncorrectString_ReturnsNull()
        {
            var parser = new StartBoardParser();
            var ruleset = new OrthodoxRuleset();
            var result = parser.Parse(ruleset, "d");
            Assert.IsNull(result);
        }

        private class OtherRuleset : IRuleset
        {
            public IEnumerable<Player> Players => throw new NotImplementedException();
            public IEnumerable<PieceType> PieceTypes => throw new NotImplementedException();
            public Board ApplyMove(Board board, Move move)
            {
                throw new NotImplementedException();
            }

            public GameStatus GetGameStatus(Game game)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
            {
                throw new NotImplementedException();
            }

            public Player GetNextPlayer(Player currentPlayer)
            {
                throw new NotImplementedException();
            }

            public bool IsInBounds(Coords coords)
            {
                throw new NotImplementedException();
            }
        }



        [TestMethod]
        public void Serialise_NonStartBoard_ReturnsNull()
        {
            var parser = new StartBoardParser();
            var ruleset = new OrthodoxRuleset();
            var board = new Board(ruleset.Players.First(), new BoardRanks(new List<BoardRank>()));
            var result = parser.Serialise(ruleset, board);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Serialise_StartBoard_ReturnsCorrectString()
        {
            var parser = new StartBoardParser();
            var ruleset = new OrthodoxRuleset();
            var board = parser.Parse(ruleset, "s");
            Assert.IsNotNull(board);
            var result = parser.Serialise(ruleset, board);
            Assert.AreEqual("s", result);
        }

        [TestMethod]
        public void Parse_OrthodoxRulesetAndCorrectString_ReturnsCorrectBoard()
        {
            var parser = new StartBoardParser();
            var ruleset = new OrthodoxRuleset();
            var result = parser.Parse(ruleset, "s");

            Assert.IsNotNull(result);
            AssertBackRank(result, 0, 0);
            AssertPawnRank(result, 0, 1);
            for (int rank = 2; rank < 6; rank++)
            {
                AssertEmptyRank(result, rank);
            }
            AssertPawnRank(result, 1, 6);
            AssertBackRank(result, 1, 7);
        }

        private void AssertBackRank(Board board, int playerId, int rank)
        {
            var pieceSet = OrthodoxPieceTypes.Player(playerId);
            AssertPiece(pieceSet.RookWithCastlingRights, board, rank, 0);
            AssertPiece(pieceSet.Knight, board, rank, 1);
            AssertPiece(pieceSet.Bishop, board, rank, 2);
            AssertPiece(pieceSet.Queen, board, rank, 3);
            AssertPiece(pieceSet.King, board, rank, 4);
            AssertPiece(pieceSet.Bishop, board, rank, 5);
            AssertPiece(pieceSet.Knight, board, rank, 6);
            AssertPiece(pieceSet.RookWithCastlingRights, board, rank, 7);
        }

        private void AssertPawnRank(Board board, int playerId, int rank)
        {
            for (int file = 0; file < 8; file++)
            {
                AssertPiece(OrthodoxPieceTypes.Player(playerId).Pawn, board, rank, file);
            }
        }

        private void AssertEmptyRank(Board board, int rank)
        {
            for (int file = 0; file < 8; file++)
            {
                Assert.IsNull(board.GetPiece(new Coords(rank, file)));
            }
        }

        private void AssertPiece(PieceType expectedPieceType, Board board, int rank, int file)
        {
            var pieceType = board.GetPiece(new Coords(rank, file)) as PieceType;
            Assert.IsNotNull(pieceType);
            Assert.AreEqual(expectedPieceType.Id, pieceType.Id);
        }
    }
}
