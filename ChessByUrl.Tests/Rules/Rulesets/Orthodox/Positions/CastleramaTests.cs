using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules;
using ChessByUrl.Rules.PieceBehaviours;
using ChessByUrl.Rules.Rulesets.Orthodox;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Rules.Rulesets.Orthodox.Positions
{
    /// <summary>
    /// Board after 1.b3 b6 2. e3 e6 3. Nc3 Nc6 4. Qf3 Qf6 5. Nh3 Nh6.
    /// From this position, Bd3/Bd6 will allow each side to castle kingside, and Ba3/Ba6 will allow queenside.
    /// The king and both rooks also have space to shake off castling rights, and it's pretty easy for the queens to prevent castling.
    /// </summary>
    [TestClass]
    [Ignore("TODO: Migrate to EndToEnd")]
    public class CastleramaTests
    {

        //private (Board board, Player white, Player black) CreateBoard(OrthodoxRuleset ruleset)
        //{
        //    var boardParser = new CustomBoardParser();
        //    var board = boardParser.Parse(ruleset, "cBgAEAkAECkMbARYyIg0VDjZxJiIJGiIhJWVdRS1yRUlrVTsGbnIsXVY2dnZrYRwiJ3xVbyE");
        //    var white = ruleset.Players.Single(p => p.Id == 0);
        //    var black = ruleset.Players.Single(p => p.Id == 1);
        //    Assert.IsNotNull(board);
        //    Assert.AreEqual(white, board.CurrentPlayer);
        //    return (board, white, black);
        //}
        private (Game game, Player white, Player black) CreateGame()
        {
            var ruleset = new OrthodoxRuleset();
            var boardParser = new CustomBoardParser();
            var board = boardParser.Parse(ruleset, "cBgAEAkAECkMbARYyIg0VDjZxJiIJGiIhJWVdRS1yRUlrVTsGbnIsXVY2dnZrYRwiJ3xVbyE");
            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);
            Assert.IsNotNull(board);
            Assert.AreEqual(white, board.CurrentPlayer);
            return (new Game(ruleset, board), white, black);
        }

        private void AssertWhiteToMove(Game game)
        {
            Assert.AreEqual("White", game.CurrentPlayer.Name);
        }

        private void AssertBlackToMove(Game game)
        {
            Assert.AreEqual("Black", game.CurrentPlayer.Name);
        }

        private void AssertMoveLegal(Game game, Coords from, Coords to, string? message = null)
        {
            var legalMoves = game.GetLegalMovesFromSquare(from);
            Assert.IsTrue(legalMoves.Any(m => m.To == to), message ?? $"Expected {from}-{to} to be legal.");
        }
        private void AssertMoveIllegal(Game game, Coords from, Coords to, string? message = null)
        {
            var legalMoves = game.GetLegalMovesFromSquare(from);
            Assert.IsFalse(legalMoves.Any(m => m.To == to), message ?? $"Expected {from}-{to} to be illegal.");
        }

        private Game AssertAndApplyMove(Game game, Coords from, Coords to, string? message = null)
        {
            AssertMoveLegal(game, from, to, message);
            return game; // This won't work as is, but probably won't be needed
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
        public void InitialStateCorrect()
        {
            (var game, var white, var black) = CreateGame();

            Assert.AreEqual("Rook with castling rights", game.CurrentBoard.GetPiece("a1")?.Name);
            Assert.AreEqual("Rook with castling rights", game.CurrentBoard.GetPiece("h1")?.Name);
            Assert.AreEqual("Rook with castling rights", game.CurrentBoard.GetPiece("a8")?.Name);
            Assert.AreEqual("Rook with castling rights", game.CurrentBoard.GetPiece("h8")?.Name);
            Assert.AreEqual("King", game.CurrentBoard.GetPiece("e1")?.Name);
            Assert.AreEqual("King", game.CurrentBoard.GetPiece("e8")?.Name);

            var kingMoves = game.GetLegalMovesFromSquare("e1");
            Assert.AreEqual(2, kingMoves.Count());
            Assert.AreEqual((Coords)"e2", kingMoves.First().To);
            Assert.AreEqual((Coords)"d1", kingMoves.Last().To);
        }

        [TestMethod]
        public void CanCastleKingsideWhenBishopMoved()
        {
            (var game, var white, var black) = CreateGame();

            game = AssertAndApplyMove(game, "f1", "d3");
            game = AssertAndApplyMove(game, "f8", "d6");
            AssertWhiteToMove(game);

            AssertMoveLegal(game, "e1", "g1", "Expected white kingside castling to be legal");
            AssertMoveLegal(game, "e8", "g8", "Expected black kingside castling to be legal");
            AssertMoveIllegal(game, "e1", "c1", "Expected white queenside castling to be illegal");
            AssertMoveIllegal(game, "e8", "c8", "Expected black queenside castling to be illegal");
        }

        [TestMethod]
        public void CanCastleQueensideWhenBishopMoved()
        {
            (var game, var white, var black) = CreateGame();

            game = AssertAndApplyMove(game, "c1", "a3");
            game = AssertAndApplyMove(game, "c8", "a6");
            AssertWhiteToMove(game);

            AssertMoveLegal(game, "e1", "c1", "Expected white queenside castling to be legal");
            AssertMoveLegal(game, "e8", "c8", "Expected black queenside castling to be legal");
            AssertMoveIllegal(game, "e1", "g1", "Expected white kingside castling to be illegal");
            AssertMoveIllegal(game, "e8", "g8", "Expected black kingside castling to be illegal");
        }

        [TestMethod]
        public void CanCastleKingsideAndQueensideWhenBothBishopsMoved()
        {
            (var game, var white, var black) = CreateGame();

            // Move all bishops out of the way.
            game = AssertAndApplyMove(game, "f1", "d3");
            game = AssertAndApplyMove(game, "f8", "d6");
            game = AssertAndApplyMove(game, "c1", "a3");
            game = AssertAndApplyMove(game, "c8", "a6");
            AssertWhiteToMove(game);

            AssertMoveLegal(game, "e1", "g1", "Expected white kingside castling to be legal");
            AssertMoveLegal(game, "e8", "g8", "Expected black kingside castling to be legal");
            AssertMoveLegal(game, "e1", "c1", "Expected white queenside castling to be legal");
            AssertMoveLegal(game, "e8", "c8", "Expected black queenside castling to be legal");
        }

        [TestMethod]
        public void CastlingForbiddenAfterMovingRooks()
        {
            (var game, var white, var black) = CreateGame();

            // Move all bishops out of the way.
            game = AssertAndApplyMove(game, "f1", "d3");
            game = AssertAndApplyMove(game, "f8", "d6");
            game = AssertAndApplyMove(game, "c1", "a3");
            game = AssertAndApplyMove(game, "c8", "a6");

            // Shake off castling rights for white kingside and black queenside rooks.
            game = AssertAndApplyMove(game, "h1", "g1");
            game = AssertAndApplyMove(game, "a8", "b8");
            game = AssertAndApplyMove(game, "g1", "h1");
            game = AssertAndApplyMove(game, "b8", "a8");
            AssertWhiteToMove(game);

            AssertMoveLegal(game, "e8", "g8", "Expected black kingside castling to be legal");
            AssertMoveLegal(game, "e1", "c1", "Expected white queenside castling to be legal");
            AssertMoveIllegal(game, "e1", "g1", "Expected white kingside castling to be illegal after rook move");
            AssertMoveIllegal(game, "e8", "c8", "Expected black queenside castling to be illegal after rook move");

            // Shake off castling rights for the remaining rooks.
            game = AssertAndApplyMove(game, "a1", "b1");
            game = AssertAndApplyMove(game, "h8", "g8");
            game = AssertAndApplyMove(game, "b1", "a1");
            game = AssertAndApplyMove(game, "g8", "h8");
            AssertWhiteToMove(game);

            AssertMoveIllegal(game, "e1", "g1", "Expected white kingside castling to be illegal after rook move");
            AssertMoveIllegal(game, "e8", "g8", "Expected black kingside castling to be illegal after rook move");
            AssertMoveIllegal(game, "e1", "c1", "Expected white queenside castling to be illegal after rook move");
            AssertMoveIllegal(game, "e8", "c8", "Expected black queenside castling to be illegal after rook move");
        }

        [TestMethod]
        public void CastlingForbiddenAfterMovingKings()
        {
            (var game, var white, var black) = CreateGame();

            // Move all bishops out of the way.
            game = AssertAndApplyMove(game, "f1", "d3");
            game = AssertAndApplyMove(game, "f8", "d6");
            game = AssertAndApplyMove(game, "c1", "a3");
            game = AssertAndApplyMove(game, "c8", "a6");

            // Shake the kings, which should remove all castling rights.
            game = AssertAndApplyMove(game, "e1", "f1");
            game = AssertAndApplyMove(game, "e8", "f8");
            game = AssertAndApplyMove(game, "f1", "e1");
            game = AssertAndApplyMove(game, "f8", "e8");
            AssertWhiteToMove(game);

            AssertMoveIllegal(game, "e8", "g8", "Expected black kingside castling to be illegal after king move");
            AssertMoveIllegal(game, "e1", "c1", "Expected white queenside castling to be illegal after king move");
            AssertMoveIllegal(game, "e1", "g1", "Expected white kingside castling to be illegal after rook move");
            AssertMoveIllegal(game, "e8", "c8", "Expected black queenside castling to be illegal after rook move");

            // Shake off castling rights for the remaining rooks.
            game = AssertAndApplyMove(game, "a1", "b1");
            game = AssertAndApplyMove(game, "h8", "g8");
            game = AssertAndApplyMove(game, "b1", "a1");
            game = AssertAndApplyMove(game, "g8", "h8");
            AssertWhiteToMove(game);

            AssertMoveIllegal(game, "e1", "g1", "Expected white kingside castling to be illegal after rook move");
            AssertMoveIllegal(game, "e8", "g8", "Expected black kingside castling to be illegal after rook move");
            AssertMoveIllegal(game, "e1", "c1", "Expected white queenside castling to be illegal after rook move");
            AssertMoveIllegal(game, "e8", "c8", "Expected black queenside castling to be illegal after rook move");
        }
    }
}
