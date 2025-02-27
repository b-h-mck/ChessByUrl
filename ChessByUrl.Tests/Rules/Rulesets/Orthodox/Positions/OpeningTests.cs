using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules.Rulesets.Orthodox;
using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessByUrl.Rules.PieceBehaviours;

namespace ChessByUrl.Tests.Rules.Rulesets.Orthodox.Positions
{

    /// <summary>
    /// Board at start of game
    /// </summary>
    [TestClass]
    public class OpeningTests
    {
        private (Board board, Player white, Player black) CreateBoard(OrthodoxRuleset ruleset)
        {
            var boardParser = new StartBoardParser();
            var board = boardParser.Parse(ruleset, "s");
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
        public void CanMovePawns()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);
            for (int file = 0; file < 8; file++)
            {
                var from = new Coords(1, file);
                var moves = ruleset.GetLegalMoves(board, from);
                Assert.AreEqual(2, moves.Count());
                Assert.IsTrue(moves.All(m => m.From == from));
                Assert.IsTrue(moves.All(m => m.To.File == file));
                Assert.IsTrue(moves.Any(m => m.To.Rank == 2));
                Assert.IsTrue(moves.Any(m => m.To.Rank == 3));
            }
        }

        [TestMethod]
        public void MoveOne()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);
            var move = new Move { From = "a2", To = "a3" };
            var boardAfterMove = ruleset.ApplyMove(board, move);
            Assert.AreEqual(black, boardAfterMove.CurrentPlayer);

            var pieceType = boardAfterMove.GetPiece("a3");
            Assert.IsNotNull(pieceType);
            Assert.AreEqual(white, pieceType.Player);
            Assert.AreEqual("Pawn", pieceType.Name);
            Assert.IsFalse(pieceType.Behaviours.OfType<EnPassantVictimBehaviour>().Any(), "Unexpected EnPassantVictimBehaviour");
        }

        [TestMethod]
        public void MoveTwo()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);
            var move = new Move { From = "a2", To = "a4" };
            var boardAfterMove = ruleset.ApplyMove(board, move);
            Assert.AreEqual(black, boardAfterMove.CurrentPlayer);

            var pieceType = boardAfterMove.GetPiece("a4");
            Assert.IsNotNull(pieceType);
            Assert.AreEqual(white, pieceType.Player);
            Assert.AreEqual("Pawn vulnerable to en passant", pieceType.Name);
            Assert.IsTrue(pieceType.Behaviours.OfType<EnPassantVictimBehaviour>().Any(), "Expected EnPassantVictimBehaviour");
        }
    }
}
