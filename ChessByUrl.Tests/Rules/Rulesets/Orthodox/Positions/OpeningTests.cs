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
        private (Game game, Player white, Player black) CreateGame()
        {
            var ruleset = new OrthodoxRuleset();
            var boardParser = new StartBoardParser();
            var board = boardParser.Parse(ruleset, "s");
            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);
            Assert.IsNotNull(board);
            Assert.AreEqual(white, board.CurrentPlayer);
            return (new Game(ruleset, board), white, black);
        }

        [TestMethod]
        public void IsInProgess()
        {
            (var game, var white, var black) = CreateGame();

            var gameStatus = game.Status;
            Assert.IsNotNull(gameStatus);
            Assert.IsTrue(!gameStatus.IsFinished);
            Assert.IsNull(gameStatus.PlayerPoints);
        }

        [TestMethod]
        public void CanMovePawns()
        {
            (var game, var white, var black) = CreateGame();

            for (int file = 0; file < 8; file++)
            {
                var from = new Coords(1, file);
                var moves = game.GetLegalMovesFromSquare(from);
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
            (var game, var white, var black) = CreateGame();

            var move = new Move { From = "a2", To = "a3" };
            var gameAfterMove = new Game(game, move);
            Assert.AreEqual(black, gameAfterMove.CurrentPlayer);

            var pieceType = gameAfterMove.CurrentBoard.GetPiece("a3");
            Assert.IsNotNull(pieceType);
            Assert.AreEqual(white, pieceType.Player);
            Assert.AreEqual("Pawn", pieceType.Name);
            Assert.IsFalse(pieceType.Behaviours.OfType<EnPassantVictimBehaviour>().Any(), "Unexpected EnPassantVictimBehaviour");
        }

        [TestMethod]
        public void MoveTwo()
        {
            (var game, var white, var black) = CreateGame();

            var move = new Move { From = "a2", To = "a4" };
            var gameAfterMove = new Game(game, move);
            Assert.AreEqual(black, gameAfterMove.CurrentPlayer);

            var pieceType = gameAfterMove.CurrentBoard.GetPiece("a4");
            Assert.IsNotNull(pieceType);
            Assert.AreEqual(white, pieceType.Player);
            Assert.AreEqual("Pawn vulnerable to en passant", pieceType.Name);
            Assert.IsTrue(pieceType.Behaviours.OfType<EnPassantVictimBehaviour>().Any(), "Expected EnPassantVictimBehaviour");
        }
    }
}
