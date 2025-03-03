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

        private (Game game, Player white, Player black) CreateGame()
        {
            var ruleset = new OrthodoxRuleset();
            var boardParser = new CustomBoardParser();
            var board = boardParser.Parse(ruleset, "cBgAFISABBkAQKAUWYQ0BDjEhBidGGDkmNWYSXyZ0Bm5iGjoebnVsXF47aBdsRVwwL31WbyE");
            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);
            Assert.IsNotNull(board);
            Assert.AreEqual(white, board.CurrentPlayer);
            return (new Game(ruleset, board), white, black);
        }

        [TestMethod]
        public void IsCheckmate()
        {
           (var game, var white, var black) = CreateGame();
            var gameStatus = game.Status;
            Assert.IsNotNull(gameStatus);
            Assert.IsTrue(gameStatus.IsFinished);
            CollectionAssert.AreEqual(new[] { (white, 0m), (black, 1m) }, gameStatus.PlayerPoints);
        }

        [TestMethod]
        public void WhiteKingIsThreatened()
        {
            (var game, var white, var black) = CreateGame();

            Coords whiteKingSquare = "e1";
            var whiteKing = game.CurrentBoard.GetPiece(whiteKingSquare);
            Assert.IsNotNull(whiteKing);
            Assert.AreEqual(white, whiteKing.Player);
            Assert.AreEqual("King", whiteKing.Name);

            var threats = game.GetThreats(whiteKingSquare, white);
            Assert.AreEqual(1, threats.Count());
        }

        [TestMethod]
        public void NoLegalMoves()
        {
            (var game, var white, var black) = CreateGame();

            foreach (var (coords, pieceType) in game.CurrentBoard)
            {
                if (pieceType?.Player == white)
                {
                    var legalMoves = game.GetLegalMovesFromSquare(coords);
                    Assert.AreEqual(0, legalMoves.Count());
                }
            }
        }
    }
}
