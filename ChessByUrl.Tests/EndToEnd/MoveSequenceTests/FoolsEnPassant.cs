using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules.Rulesets.Orthodox;
using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.EndToEnd.MoveSequenceTests
{
    /// <summary>
    /// Tests analysing the quickest possible en passant in chess.
    /// </summary>
    [TestClass]
    public class FoolsEnPassant : MoveSequenceTestBase
    {
        protected override IEnumerable<Move> MoveSequence => [
            new Move("e2", "e4"),
                new Move("e7", "e6"),
                new Move("e4", "e5"),
                new Move("d7", "d5"),
                new Move("e5", "d6"),
            ];

        [TestMethod]
        public void StartOfGame()
        {
            (var game, var white, var black) = CreateGame(0);

            Assert.AreEqual(white, game.CurrentPlayer);
            GameAssert.ParserRoundTrip(game);

            // Sanity check to make sure the pieces involved are in the right place
            GameAssert.PieceAtSquare(game, "e2", white, "Pawn");
            GameAssert.PieceAtSquare(game, "e7", black, "Pawn");
            GameAssert.PieceAtSquare(game, "d7", black, "Pawn");
        }

        [TestMethod]
        public void AfterWhitesFirstMove()
        {
            (var game, var white, var black) = CreateGame(1);
            GameAssert.ParserRoundTrip(game);
            Assert.AreEqual(black, game.CurrentPlayer);
            Assert.IsFalse(game.Status.IsFinished);

            // White did a double move, so pawn should be vulnerable to en passant
            GameAssert.SquareEmpty(game, "e2");
            GameAssert.PieceAtSquare(game, "e4", white, "Pawn vulnerable to en passant");

        }

        [TestMethod]
        public void AfterBlacksFirstMove()
        {
            (var game, var white, var black) = CreateGame(2);
            GameAssert.ParserRoundTrip(game);
            Assert.AreEqual(white, game.CurrentPlayer);
            Assert.IsFalse(game.Status.IsFinished);

            GameAssert.SquareEmpty(game, "e7");
            GameAssert.PieceAtSquare(game, "e6", black, "Pawn");

            // White's pawn should no longer be vulnerable to en passant
            GameAssert.PieceAtSquare(game, "e4", white, "Pawn");
        }

        [TestMethod]
        public void AfterWhitesSecondMove()
        {
            (var game, var white, var black) = CreateGame(3);
            GameAssert.ParserRoundTrip(game);
            Assert.AreEqual(black, game.CurrentPlayer);
            Assert.IsFalse(game.Status.IsFinished);

            GameAssert.SquareEmpty(game, "e4");
            GameAssert.PieceAtSquare(game, "e5", white, "Pawn");
        }

        [TestMethod]
        public void AfterBlacksSecondMove()
        {
            (var game, var white, var black) = CreateGame(4);
            GameAssert.ParserRoundTrip(game);
            Assert.AreEqual(white, game.CurrentPlayer);
            Assert.IsFalse(game.Status.IsFinished);

            GameAssert.SquareEmpty(game, "d7");
            GameAssert.PieceAtSquare(game, "d5", black, "Pawn vulnerable to en passant");

            // White should be able to capture en passant
            GameAssert.PieceAtSquare(game, "e5", white, "Pawn");
            GameAssert.SquareEmpty(game, "d6");
            GameAssert.MoveLegal(game, new Move("e5", "d6"));

        }

        [TestMethod]
        public void AfterEnPassantCapture()
        {
            (var game, var white, var black) = CreateGame(5);
            GameAssert.ParserRoundTrip(game);
            Assert.AreEqual(black, game.CurrentPlayer);
            Assert.IsFalse(game.Status.IsFinished);

            GameAssert.SquareEmpty(game, "e5");
            GameAssert.SquareEmpty(game, "d5");
            GameAssert.PieceAtSquare(game, "d6", white, "Pawn");
        }


    }
}
