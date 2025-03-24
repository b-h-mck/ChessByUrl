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
    public class RleBoardParserTests
    {

        [TestMethod]
        public void Serialise_OtherRuleset_ReturnsNull()
        {
            var parser = new RleBoardParser();
            var ruleset = new OtherRuleset();
            var board = new Board(new Player { Id = 0, Name = "White", ClosestRank=0, FarthestRank=8 }, new BoardRanks(new List<BoardRank>()));
            var result = parser.Serialise(ruleset, board);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_OtherRulesetAndCorrectString_ReturnsNull()
        {
            var parser = new RleBoardParser();
            var ruleset = new OtherRuleset();
            var result = parser.Parse(ruleset, "c");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_OrthodoxRulesetAndIncorrectString_ReturnsNull()
        {
            var parser = new RleBoardParser();
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

            public GameStatus GetGameStatus(Game game)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Move> GetLegalMoves(Board board, Coords from)
            {
                throw new NotImplementedException();
            }

            public string GetMoveNotation(Board boardBeforeMove, Move move)
            {
                throw new NotImplementedException();
            }

            public MoveVariantInfo? GetMoveVariant(Board boardBeforeMove, Move move)
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
        public void RoundTrip_SerialiseThenParse()
        {
            var parser = new RleBoardParser();
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
