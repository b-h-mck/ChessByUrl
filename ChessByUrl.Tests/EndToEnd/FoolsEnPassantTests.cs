using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules.Rulesets.Orthodox;
using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.EndToEnd
{
    /// <summary>
    /// Tests analysing the quickest possible en passant in chess.
    /// </summary>
    [TestClass]
    public class FoolsEnPassantTests : MoveSequenceTestBase
    {
        protected override IEnumerable<Move> MoveSequence => [
            new Move { From = "e2", To = "e4" },
            new Move { From = "e7", To = "e6" },
            new Move { From = "e4", To = "e5" },
            new Move { From = "d7", To = "d5" },
            new Move { From = "e5", To = "d6" },
        ];

        [TestMethod]
        public void StartOfGame()
        {
            (var game, var white, var black) = CreateGame(0);

            Assert.AreEqual(white, game.CurrentPlayer);
            AssertParserRoundTrip(game);

            // Sanity check to make sure the pieces involved are in the right place
            GameAssert.PieceAtSquare(game, "e2", white, "Pawn");
            GameAssert.PieceAtSquare(game, "e7", black, "Pawn");
            GameAssert.PieceAtSquare(game, "d7", black, "Pawn");
        }

        [TestMethod]
        public void AfterWhitesFirstMove()
        {
            (var game, var white, var black) = CreateGame(1);
            AssertParserRoundTrip(game);

            Assert.AreEqual(black, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "e2");
            GameAssert.PieceAtSquare(game, "e4", white, "Pawn vulnerable to en passant");
            Assert.IsFalse(game.Status.IsFinished);
        }

        [TestMethod]
        public void AfterBlacksFirstMove()
        {
            (var game, var white, var black) = CreateGame(2);
            AssertParserRoundTrip(game);

            Assert.AreEqual(white, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "e7");
            GameAssert.PieceAtSquare(game, "e6", black, "Pawn");
            Assert.IsFalse(game.Status.IsFinished);
        }
        [TestMethod]
        public void AfterWhitesSecondMove()
        {
            (var game, var white, var black) = CreateGame(3);
            AssertParserRoundTrip(game);

            Assert.AreEqual(black, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "e4");
            GameAssert.PieceAtSquare(game, "e5", white, "Pawn");
            Assert.IsFalse(game.Status.IsFinished);
        }

        [TestMethod]
        public void AfterBlacksSecondMove()
        {
            (var game, var white, var black) = CreateGame(4);
            AssertParserRoundTrip(game);

            Assert.AreEqual(white, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "d7");
            GameAssert.PieceAtSquare(game, "d5", black, "Pawn vulnerable to en passant");

            GameAssert.PieceAtSquare(game, "e5", white, "Pawn");
            GameAssert.SquareEmpty(game, "d6");
            GameAssert.MoveLegal(game, new Move { From = "e5", To = "d6" });

            Assert.IsFalse(game.Status.IsFinished);
        }

        [TestMethod]
        public void AfterEnPassantCapture()
        {
            (var game, var white, var black) = CreateGame(5);
            AssertParserRoundTrip(game);

            Assert.AreEqual(black, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "e5");
            GameAssert.SquareEmpty(game, "d5");
            GameAssert.PieceAtSquare(game, "d6", white, "Pawn");
            Assert.IsFalse(game.Status.IsFinished);
        }


    }
}
