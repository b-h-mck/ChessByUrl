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
    /// Tests analysing the quickest possible promotion.
    /// </summary>
    [TestClass]
    public class FoolsPromotion : MoveSequenceTestBase
    {
        protected override IEnumerable<Move> MoveSequence => [
                new Move("a2", "a4"),
                new Move("b7", "b5"),
                new Move("a4", "b5"),
                new Move("b8", "c6"),
                new Move("b5", "b6"),
                new Move("e7", "e5"),
                new Move("b6", "b7"),
                new Move("d7", "d5"),
            ];

        [TestMethod]
        public void StartOfGame()
        {
            (var game, var white, var black) = CreateGame(0);

            Assert.AreEqual(white, game.CurrentPlayer);
            GameAssert.ParserRoundTrip(game);

            // Sanity check to make sure the pieces involved are in the right place
            GameAssert.PieceAtSquare(game, "a2", white, "Pawn");
            GameAssert.PieceAtSquare(game, "b7", black, "Pawn");
        }

        [TestMethod]
        public void BeforePromotionMove()
        {
            (var game, var white, var black) = CreateGame(8);
            GameAssert.ParserRoundTrip(game);

            Assert.AreEqual(white, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "b8");
            GameAssert.PieceAtSquare(game, "b7", white, "Pawn");
            Assert.IsFalse(game.Status.IsFinished);

            var legalMoves = game.GetLegalMovesForPlayer(white);
        }
    }
}
