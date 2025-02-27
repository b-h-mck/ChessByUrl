using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules;
using ChessByUrl.Rules.Rulesets.Orthodox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Rules.Rulesets.Orthodox.Positions
{
    /// <summary>
    /// Board after 1. f3 e6 2. g4?? Qh4# (fastest possible checkmate)
    /// </summary>
    [TestClass]
    public class FoolsMateTests
    {

        private (Board board, Player white, Player black) CreateBoard(OrthodoxRuleset ruleset)
        {
            var boardParser = new CustomBoardParser();
            var board = boardParser.Parse(ruleset, "cBgAFISABBkAQKAUWYQ0BDjEhBidGGDkmNWYSXyZ0Bm5iGjoebnVsXF47aBdsRVwwL31WbyE");
            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);
            Assert.IsNotNull(board);
            Assert.AreEqual(white, board.CurrentPlayer);
            return (board, white, black);
        }

        [TestMethod]
        public void IsCheckmate()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            var gameStatus = ruleset.GetGameStatus(board);
            Assert.IsNotNull(gameStatus);
            Assert.IsTrue(gameStatus.IsFinished);
            CollectionAssert.AreEqual([(white, 0), (black, 1)], gameStatus.PlayerPoints);
        }

        [TestMethod]
        public void WhiteKingIsThreatened()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            Coords whiteKingSquare = "e1";
            var whiteKing = board.GetPiece(whiteKingSquare);
            Assert.IsNotNull(whiteKing);
            Assert.AreEqual(white, whiteKing.Player);
            Assert.AreEqual("King", whiteKing.Name);

            var threats = ruleset.GetThreats(board, whiteKingSquare, white);
            Assert.AreEqual(1, threats.Count());
        }

        [TestMethod]
        public void NoLegalMoves()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);
            foreach (var (coords, pieceType) in board)
            {
                if (pieceType?.Player == white)
                {
                    var legalMoves = ruleset.GetLegalMoves(board, coords);
                    Assert.AreEqual(0, legalMoves.Count());
                }
            }
        }
    }
}
