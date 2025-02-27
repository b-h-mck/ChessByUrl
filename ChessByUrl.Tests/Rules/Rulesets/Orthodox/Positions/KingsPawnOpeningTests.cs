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
    /// Board after 1. e3
    /// </summary>
    [TestClass]
    public class KingsPawnOpeningTests
    {

        private (Board board, Player white, Player black) CreateBoard(OrthodoxRuleset ruleset)
        {
            var boardParser = new CustomBoardParser();
            var board = boardParser.Parse(ruleset, "cBwAFISABBkAQKAUWYQ0BDjEhBidGGTEWN0ZcQjZxJm5mWjgubnZsV0M9YSdpR1wwL31WbyE");
            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);
            Assert.IsNotNull(board);
            Assert.AreEqual(black, board.CurrentPlayer);
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
        public void CanMovePawns()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);
            for (int file = 0; file < 8; file++)
            {
                var from = new Coords(6, file);
                var moves = ruleset.GetLegalMoves(board, from);
                Assert.AreEqual(2, moves.Count());
                Assert.IsTrue(moves.All(m => m.From == from));
                Assert.IsTrue(moves.All(m => m.To.File == file));
                Assert.IsTrue(moves.Any(m => m.To.Rank == 5));
                Assert.IsTrue(moves.Any(m => m.To.Rank == 4));
            }
        }
    }
}
