using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules;
using ChessByUrl.Rules.PieceBehaviours;
using ChessByUrl.Rules.Rulesets.Orthodox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Rules.Rulesets.Orthodox.Positions
{
    /// <summary>
    /// Board after 1.e4 e6 2.e5 d5 (i.e. just before fastest possible en passant)
    /// </summary>
    [TestClass]
    public class FoolsEnPassantTests
    {

        private (Board board, Player white, Player black) CreateBoard(OrthodoxRuleset ruleset)
        {
            var boardParser = new CustomBoardParser();
            var board = boardParser.Parse(ruleset, "cBgAFISABBkAQKAUWYQ0BDjEhBidGGTEWdylNUC50Bm5iGjoubnZsV0M9YSdpR1wwL31WbyE");
            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);
            Assert.IsNotNull(board);
            Assert.AreEqual(white, board.CurrentPlayer);
            return (board, white, black);
        }

        [TestMethod]
        public void IsInProgess()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            var gameStatus = ruleset.GetGameStatus(board);
            Assert.IsNotNull(gameStatus);
            Assert.IsTrue(!gameStatus.IsFinished);
            Assert.IsNull(gameStatus.PlayerPoints);
        }

        [TestMethod]
        public void D5PawnIsVulnerableToEnPassant()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            var pieceType = board.GetPiece("d5");
            Assert.IsNotNull(pieceType);
            var behaviours = pieceType.Behaviours.OfType<EnPassantVictimBehaviour>();
            Assert.IsTrue(behaviours.Any(), "Expected EnPassantVictimBehaviour on D5.");
        }

        [TestMethod]
        public void E5PawnCanAttackD6()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            var moves = ruleset.GetLegalMoves(board, "e5");
            Assert.AreEqual(1, moves.Count(), "Expected one move for E5");
            var move = moves.Single();
            Assert.AreEqual((Coords)"e5", move.From);
            Assert.AreEqual((Coords)"d6", move.To);
        }
    }
}
