using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ChessByUrl.Rules.Rulesets.Orthodox;

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
            var board = new Board(new Player { Id = 0, Name = "White", ClosestRank=0, FarthestRank=8 }, new BoardRanks(new List<BoardRank>()));
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
            public IEnumerable<PieceType> PieceTypes => throw new NotImplementedException();
            public Board ApplyMove(Board board, Move move)
            {
                throw new NotImplementedException();
            }

            public GameStatus GetGameStatus(Board board)
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
        [DataRow(1, 0, PieceIds.White.Pawn, 3, 0, "cDIMA")]
        [DataRow(2, 0, PieceIds.White.PawnVulnerableToEnPassant, 3, 0, "cDoMA")]
        [DataRow(3, 0, PieceIds.Black.RookWithCastlingRights, 6, 7, "cVp4A")]
        [DataRow(4, 0, PieceIds.Black.Bishop, 3, 3, "cGI8A")]
        [DataRow(5, 1, PieceIds.White.King, 7, 7, "cQZ8A")]
        [DataRow(6, 1, PieceIds.White.Pawn, 3, 0, "cDYMA")]
        [DataRow(7, 1, PieceIds.Black.Rook, 6, 7, "cVZ4A")]
        public void Serialise_SinglePiece(int _, int currentPlayerId, int pieceTypeId, int rank, int file, string expectedBase64)
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();
            var board = PrepareBoard(ruleset, currentPlayerId, [(pieceTypeId, new Coords(rank, file))]);
            var result = parser.Serialise(ruleset, board);
            Assert.AreEqual(expectedBase64, result);
        }

        [TestMethod]
        [DataRow(1, 0, PieceIds.White.Knight, "cCgAFKSAFJkEUKAUmYQsBDSkhBSdFFCkVJmMLAhUpIiUmSRQqJSZlCwMdKSMlJ00UKzUmZ0sAJSkkRSZRFCxFJmlLAS0pJUUnVRQtVSZrSwI1KSZlJlkULmUmbUsDPSknZSddFC91Jm8h")]
        [DataRow(2, 0, PieceIds.Black.Pawn, "cHEAGcSAOZkI4KA52YB1BDnEhDmdGOCkedmIdQhZxIi5mSjgqLnZkHUMecSMuZ044Kz52Zl1AJnEkTmZSOCxOdmhdQS5xJU5nVjgtXnZqXUI2cSZuZlo4Lm52bF1DPnEnbmdeOC9-dm4h")]
        [DataRow(3, 0, PieceIds.White.PawnVulnerableToEnPassant, "cDgAHOSAHJkMcKAc2YQ8BDzkhBydHHCkXNmMPAhc5IicmSxwqJzZlDwMfOSMnJ08cKzc2Z08AJzkkRyZTHCxHNmlPAS85JUcnVxwtVzZrTwI3OSZnJlscLmc2bU8DPzknZydfHC93Nm8h")]
        [DataRow(4, 1, PieceIds.Black.King, "cEUAAQSAIRkAgKAhGYBFBCEEhCEdEICkYRmIRQhBBIihGSCAqKEZkEUMYQSMoR0wgKzhGZlFAIEEkSEZQICxIRmhRQShBJUhHVCAtWEZqUUIwQSZoRlggLmhGbFFDOEEnaEdcIC94Rm4h")]
        [DataRow(5, 1, PieceIds.White.Queen, "cAwABCSABBkEEKAEGYQMBCQkhAQdFBCkRBmMDAhEJIiEGSQQqIQZlAwMZCSMhB00EKzEGZ0MAIQkkQQZRBCxBBmlDASkJJUEHVQQtUQZrQwIxCSZhBlkELmEGbUMDOQknYQddBC9xBm8h")]
        [DataRow(6, 1, PieceIds.Black.RookWithCastlingRights, "cF0ADWSALRkMsKAtWYRdBC1khC0dHLCkbVmMXQhNZIitGSywqK1ZlF0MbWSMrR08sKztWZ1dAI1kkS0ZTLCxLVmlXQStZJUtHVywtW1ZrV0IzWSZrRlssLmtWbVdDO1kna0dfLC97Vm8h")]
        public void Serialise_SamePieceOnEverySquare(int _, int currentPlayerId, int pieceTypeId, string expectedBase64)
        {
            var parser = new CustomBoardParser();
            var ruleset = new OrthodoxRuleset();
            var pieces = new List<(int pieceTypeId, Coords Coords)>();
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    pieces.Add((pieceTypeId, new Coords(rank, file)));
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
            Assert.AreEqual($"cBgAFISABBkAQKAUWYQ0BDjEhBidGGCkWNmJdQjZxJm5mWjgubnZsV0M9YSdpR1wwL31WbyE", result);
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

            var pieces = new List<(int PieceTypeId, Coords Coords)> {
                (PieceIds.White.King, new Coords(0, 4)),
                (PieceIds.White.Queen, new Coords(3, 6)),
                (PieceIds.White.Knight, new Coords(3, 7)),
                (PieceIds.White.Pawn, new Coords(4, 1)),
                (PieceIds.White.Pawn, new Coords(5, 2)),
                (PieceIds.White.Pawn, new Coords(4, 3)),
                (PieceIds.White.Rook, new Coords(7, 6)),
                (PieceIds.White.RookWithCastlingRights, new Coords(0, 7)),

                (PieceIds.Black.King, new Coords(7, 7)),
                (PieceIds.Black.Queen, new Coords(7, 0)),
                (PieceIds.Black.Knight, new Coords(2, 7)),
                (PieceIds.Black.PawnVulnerableToEnPassant, new Coords(4, 6)),
                (PieceIds.Black.Pawn, new Coords(4, 7)),
                (PieceIds.Black.Bishop, new Coords(4, 3)),
                (PieceIds.Black.Rook, new Coords(6, 0)),
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




        private Board PrepareBoard(OrthodoxRuleset ruleset, int currentPlayer, List<(int pieceTypeId, Coords Coords)>? pieces = null)
        {
            var squares = new PieceType?[8][];
            for (int i = 0; i < 8; i++)
            {
                squares[i] = new PieceType?[8];
            }
            if (pieces != null)
            {
                foreach (var (pieceTypeId, coords) in pieces)
                {
                    squares[coords.Rank][coords.File] = ruleset.PieceTypes.First(p => p.Id == pieceTypeId);
                }
            }

            var ranks = new BoardRanks(squares.Select((rank) => new BoardRank(rank)).ToList());
            var board = new Board(ruleset.Players.First(p => p.Id == currentPlayer), ranks);
            return board;
        }
    }
}
