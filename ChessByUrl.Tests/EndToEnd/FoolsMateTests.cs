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
    /// Tests analysing the quickest possible checkmate in chess.
    /// </summary>
    [TestClass]
    public class FoolsMateTests : MoveSequenceTestBase
    {
        protected override IEnumerable<Move> MoveSequence => [
            new Move { From = "f2", To = "f3" },
            new Move { From = "e7", To = "e6" },
            new Move { From = "g2", To = "g4" },
            new Move { From = "d8", To = "h4" }
        ];

        [TestMethod]
        public void StartOfGame()
        {
            (var game, var white, var black) = CreateGame(0);

            Assert.AreEqual(white, game.CurrentPlayer);
            AssertParserRoundTrip(game);

            // Sanity check to make sure the pieces involved are in the right place
            GameAssert.PieceAtSquare(game, "e1", white, "King");
            GameAssert.PieceAtSquare(game, "f2", white, "Pawn");
            GameAssert.SquareEmpty(game, "f3");
            GameAssert.PieceAtSquare(game, "g2", white, "Pawn");
            GameAssert.SquareEmpty(game, "g3");
            GameAssert.SquareEmpty(game, "g4");
            GameAssert.PieceAtSquare(game, "e8", black, "King");
            GameAssert.PieceAtSquare(game, "e7", black, "Pawn");
            GameAssert.SquareEmpty(game, "e6");
            GameAssert.PieceAtSquare(game, "d8", black, "Queen");
            GameAssert.SquareEmpty(game, "g3");
            GameAssert.SquareEmpty(game, "h4");
        }

        [TestMethod]
        public void AfterWhitesFirstMove()
        {
            (var game, var white, var black) = CreateGame(1);
            AssertParserRoundTrip(game);

            Assert.AreEqual(black, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "f2");
            GameAssert.PieceAtSquare(game, "f3", white, "Pawn");
            Assert.IsTrue(!game.Status.IsFinished);
        }

        [TestMethod]
        public void AfterBlacksFirstMove()
        {
            (var game, var white, var black) = CreateGame(2);
            AssertParserRoundTrip(game);

            Assert.AreEqual(white, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "e7");
            GameAssert.PieceAtSquare(game, "e6", black, "Pawn");
            Assert.IsTrue(!game.Status.IsFinished);
        }
        [TestMethod]
        public void AfterWhitesSecondMove()
        {
            (var game, var white, var black) = CreateGame(3);
            AssertParserRoundTrip(game);

            Assert.AreEqual(black, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "g2");
            GameAssert.PieceAtSquare(game, "g4", white, "Pawn vulnerable to en passant");
            Assert.IsTrue(!game.Status.IsFinished);
        }

        [TestMethod]
        public void AfterBlacksCheckmatingMove()
        {
            (var game, var white, var black) = CreateGame(4);
            AssertParserRoundTrip(game);

            Assert.AreEqual(white, game.CurrentPlayer);
            Assert.AreEqual(game.GetLegalMovesForPlayer(white).Count(), 0);
            Assert.AreEqual(game.GetThreats("e1", white).Count(), 1);
            GameAssert.SquareEmpty(game, "d8");
            GameAssert.PieceAtSquare(game, "h4", black, "Queen");
            Assert.IsTrue(game.Status.IsFinished);
            CollectionAssert.AreEqual([(white, 0m), (black, 1m)], game.Status.PlayerPoints);
        }
    }
}
