using ChessByUrl.Parser.Orthodox;
using OrthodoxRuleset = ChessByUrl.Rules.Orthodox.Ruleset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessByUrl.Rules;
using ChessByUrl.Rules.Orthodox.Pieces;

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
            var board = new Board(new Player { Id = 0, Name = "White" }, new BoardRanks(new List<BoardRank>()));
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
            public IEnumerable<Piece> Pieces => throw new NotImplementedException();
            public Board ApplyMove(Board board, Move move)
            {
                throw new NotImplementedException();
            }
            public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
            {
                throw new NotImplementedException();
            }
            public bool IsInBounds(Coords coords)
            {
                throw new NotImplementedException();
            }
        }



        [TestMethod]
        public void Serialise_OrthodoxRuleset_ReturnsCorrectString()
        {
            var parser = new StartBoardParser();
            var ruleset = new OrthodoxRuleset();
            var board = new Board(ruleset.Players.First(), new BoardRanks(new List<BoardRank>()));
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
            AssertPiece(OrthodoxPieceType.RookWithCastlingRights, playerId, board, rank, 0);
            AssertPiece(OrthodoxPieceType.Knight, playerId, board, rank, 1);
            AssertPiece(OrthodoxPieceType.Bishop, playerId, board, rank, 2);
            AssertPiece(OrthodoxPieceType.Queen, playerId, board, rank, 3);
            AssertPiece(OrthodoxPieceType.King, playerId, board, rank, 4);
            AssertPiece(OrthodoxPieceType.Bishop, playerId, board, rank, 5);
            AssertPiece(OrthodoxPieceType.Knight, playerId, board, rank, 6);
            AssertPiece(OrthodoxPieceType.RookWithCastlingRights, playerId, board, rank, 7);
        }

        private void AssertPawnRank(Board board, int playerId, int rank)
        {
            for (int file = 0; file < 8; file++)
            {
                AssertPiece(OrthodoxPieceType.Pawn, playerId, board, rank, file);
            }
        }

        private void AssertEmptyRank(Board board, int rank)
        {
            for (int file = 0; file < 8; file++)
            {
                Assert.IsNull(board.GetPiece(new Coords(rank, file)));
            }
        }

        private void AssertPiece(OrthodoxPieceType expectedType, int expectedPlayerId, Board board, int rank, int file)
        {
            var piece = board.GetPiece(new Coords(rank, file)) as OrthodoxPiece;
            Assert.IsNotNull(piece);
            Assert.AreEqual(expectedType, piece.Type);
            Assert.AreEqual(expectedPlayerId, piece.Player.Id);
        }
    }
}
