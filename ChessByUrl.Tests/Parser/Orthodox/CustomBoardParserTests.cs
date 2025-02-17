using ChessByUrl.Parser.Orthodox;
using OrthodoxRuleset = ChessByUrl.Rules.Orthodox.Ruleset;
using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessByUrl.Rules.Orthodox.Pieces;
using System.Data;

namespace ChessByUrl.Tests.Parser.Orthodox
{
    [TestClass]
    public class CustomBoardParserTests
    {

        [TestMethod]
        public void Serialise_OtherRuleset_ReturnsNull()
        {
            var parser = new CustomBoardParser();
            var ruleset = new OtherRuleset();
            var board = new Board(new Player { Id = 0, Name = "White" }, new BoardRanks(new List<BoardRank>()));
            var result = parser.Serialise(ruleset, board);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_OtherRulesetAndCorrectString_ReturnsNull()
        {
            var parser = new CustomBoardParser();
            var ruleset = new OtherRuleset();
            var result = parser.Parse(ruleset, "c");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_OrthodoxRulesetAndIncorrectString_ReturnsNull()
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();
            var result = parser.Parse(ruleset, "s");
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
        [DataRow(0, "cIA")]
        [DataRow(1, "cIQ")]
        public void Serialise_EmptyBoard(int playerId, string expectedBase64)
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();
            var board = PrepareBoard(ruleset, playerId);
            var result = parser.Serialise(ruleset, board);
            Assert.AreEqual(expectedBase64, result);
        }

        [TestMethod]
        [DataRow(0, 0, OrthodoxPieceType.Pawn, 3, 0, "cAIMA")]
        [DataRow(0, 0, OrthodoxPieceType.PawnWhoJustMovedTwoSquares, 3, 0, "cAoMA")]
        [DataRow(0, 0, OrthodoxPieceType.RookWithCastlingRights, 6, 7, "cSp4A")]
        [DataRow(0, 1, OrthodoxPieceType.Bishop, 3, 3, "cFo8A")]
        [DataRow(0, 1, OrthodoxPieceType.King, 7, 7, "cXp8A")]
        [DataRow(0, 1, OrthodoxPieceType.Pawn, 3, 0, "cEIMA")]
        [DataRow(0, 1, OrthodoxPieceType.Rook, 6, 7, "cWJ4A")]
        public void Serialise_SinglePiece(int currentPlayerId, int piecePlayerId, OrthodoxPieceType pieceType, int rank, int file, string expectedBase64)
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();
            var board = PrepareBoard(ruleset, currentPlayerId, [(piecePlayerId, pieceType, new Coords(rank, file))]);
            var result = parser.Serialise(ruleset, board);
            Assert.AreEqual(expectedBase64, result);
        }

        [TestMethod]
        [DataRow(0, 0, OrthodoxPieceType.Knight, "cBAACESACBkIIKAIWYAUBChEhAgdGCCkSFmIFAhIRIiIGSggqIhZkBQMaESMiB04IKzIWZkUAIhEkQgZSCCxCFmhFASoRJUIHVggtUhZqRQIyESZiBloILmIWbEUDOhEnYgdeCC9yFm4h")]
        [DataRow(0, 0, OrthodoxPieceType.Pawn, "cAAAAASAABkAAKAAGYAEBCAEhAAdEACkQBmIBAhABIiAGSAAqIAZkAQMYASMgB0wAKzAGZkEAIAEkQAZQACxABmhBASgBJUAHVAAtUAZqQQIwASZgBlgALmAGbEEDOAEnYAdcAC9wBm4h")]
        [DataRow(0, 0, OrthodoxPieceType.PawnWhoJustMovedTwoSquares, "cAgABCSABBkEEKAEGYQMBCQkhAQdFBCkRBmMDAhEJIiEGSQQqIQZlAwMZCSMhB00EKzEGZ0MAIQkkQQZRBCxBBmlDASkJJUEHVQQtUQZrQwIxCSZhBlkELmEGbUMDOQknYQddBC9xBm8h")]
        [DataRow(0, 1, OrthodoxPieceType.King, "cHkAHeSAPZkM8KA92YR9BD3khD2dHPCkfdmMfQhd5Ii9mSzwqL3ZlH0MfeSMvZ088Kz92Z19AJ3kkT2ZTPCxPdmlfQS95JU9nVzwtX3ZrX0I3eSZvZls8Lm92bV9DP3knb2dfPC9_dm8h")]
        [DataRow(0, 1, OrthodoxPieceType.Queen, "cHEAGcSAOZkI4KA52YB1BDnEhDmdGOCkedmIdQhZxIi5mSjgqLnZkHUMecSMuZ044Kz52Zl1AJnEkTmZSOCxOdmhdQS5xJU5nVjgtXnZqXUI2cSZuZlo4Lm52bF1DPnEnbmdeOC9-dm4h")]
        [DataRow(0, 1, OrthodoxPieceType.RookWithCastlingRights, "cGkAFaSANZkE0KA1mYRtBDWkhDWdFNCkdZmMbQhVpIi1mSTQqLWZlG0MdaSMtZ000Kz1mZ1tAJWkkTWZRNCxNZmlbQS1pJU1nVTQtXWZrW0I1aSZtZlk0Lm1mbVtDPWknbWddNC99Zm8h")]
        public void Serialise_SamePieceOnEverySquare(int currentPlayerId, int piecePlayerId, OrthodoxPieceType pieceType, string expectedBase64)
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();
            var pieces = new List<(int PlayerId, OrthodoxPieceType PieceType, Coords Coords)>();
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    pieces.Add((piecePlayerId, pieceType, new Coords(rank, file)));
                }
            }
            var board = PrepareBoard(ruleset, currentPlayerId, pieces);
            var result = parser.Serialise(ruleset, board);
            Assert.AreEqual(expectedBase64, result);
        }

        [TestMethod]
        public void Serialise_StartBoardPosition()
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();

            var startParser = new StartBoardParser();
            var startBoard = startParser.Parse(ruleset, "s");

            var result = parser.Serialise(ruleset, startBoard!);
            Assert.AreEqual($"cCgACGSAGJkMMKAImYQEBCAEhAAdEACkQBmJRQjBBJmhGWCAuaEZsW0M6WSduZ18sL3pmbyE", result);
        }


        private Board PrepareBoard(OrthodoxRuleset ruleset, int currentPlayer, List<(int PlayerId, OrthodoxPieceType PieceType, Coords Coords)>? pieces = null)
        {
            var squares = new Piece?[8][];
            for (int i = 0; i < 8; i++)
            {
                squares[i] = new Piece?[8];
            }
            if (pieces != null)
            {
                foreach (var (playerId, pieceType, coords) in pieces)
                {
                    squares[coords.Rank][coords.File] = ruleset.GetOrthodoxPiece(pieceType, playerId);
                }
            }

            var ranks = new BoardRanks(squares.Select((rank) => new BoardRank(rank)).ToList());
            var board = new Board(ruleset.Players.First(p => p.Id == currentPlayer), ranks);
            return board;
        }


        [TestMethod]
        [DataRow("cIA")]
        [DataRow("cAgABCSABBkEEKAEGYQMBCQkhAQdFBCkRBmMDAhEJIiEGSQQqIQZlAwMZCSMhB00EKzEGZ0MAIQkkQQZRBCxBBmlDASkJJUEHVQQtUQZrQwIxCSZhBlkELmEGbUMDOQknYQddBC9xBm8h")]
        [DataRow("cCgACGSAGJkMMKAImYQEBCAEhAAdEACkQBmJRQjBBJmhGWCAuaEZsW0M6WSduZ18sL3pmbyE")]
        public void RoundTrip_ParseThenSerialise(string boardString)
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();

            var board = parser.Parse(ruleset, boardString);
            Assert.IsNotNull(board);
            var result = parser.Serialise(ruleset, board!);
            Assert.AreEqual(boardString, result);
        }

        [TestMethod]
        public void RoundTrip_SerialiseThenParse()
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();

            var pieces = new List<(int PlayerId, OrthodoxPieceType PieceType, Coords Coords)> {
                (0, OrthodoxPieceType.King, new Coords(0, 4)),
                (0, OrthodoxPieceType.Queen, new Coords(3, 6)),
                (0, OrthodoxPieceType.Knight, new Coords(3, 7)),
                (0, OrthodoxPieceType.Pawn, new Coords(4, 1)),
                (0, OrthodoxPieceType.Pawn, new Coords(5, 2)),
                (0, OrthodoxPieceType.Pawn, new Coords(4, 3)),
                (0, OrthodoxPieceType.Rook, new Coords(7, 6)),
                (0, OrthodoxPieceType.RookWithCastlingRights, new Coords(0, 7)),

                (1, OrthodoxPieceType.King, new Coords(7, 7)),
                (1, OrthodoxPieceType.Queen, new Coords(7, 0)),
                (1, OrthodoxPieceType.Knight, new Coords(2, 7)),
                (1, OrthodoxPieceType.PawnWhoJustMovedTwoSquares, new Coords(4, 6)),
                (1, OrthodoxPieceType.Pawn, new Coords(4, 7)),
                (1, OrthodoxPieceType.Bishop, new Coords(4, 3)),
                (1, OrthodoxPieceType.Rook, new Coords(6, 0)),
            };
            var board = PrepareBoard(ruleset, 1, pieces);


            var boardString = parser.Serialise(ruleset, board);
            Assert.IsNotNull(boardString);
            var result = parser.Parse(ruleset, boardString);
            Assert.IsNotNull(result);

            Assert.AreEqual(board.CurrentPlayer.Id, result!.CurrentPlayer.Id);
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = board.GetPiece(new Coords(rank, file));
                    var resultPiece = result.GetPiece(new Coords(rank, file));
                    Assert.AreEqual(piece?.Id, resultPiece?.Id);
                }
            }

        }
    }
}
