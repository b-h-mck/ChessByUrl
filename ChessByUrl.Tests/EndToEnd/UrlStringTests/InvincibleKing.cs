using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.EndToEnd.UrlStringTests
{
    /// <summary>
    /// Tests analysing a position I came across after clicking out a scholar's mate-ish after a promotion,
    /// where the game isn't picking it up as checkmate and lets us capture into check.
    /// 
    /// This was caused by a bug in RoyalPieceBehaviour, where it was checking for threats from the wrong position.
    /// </summary>
    [TestClass]
    public class InvincibleKing : UrlStringTestBase
    {
        protected override string MovesString => "eL6-ABCrYYj4PQgEIWA";

        [TestMethod]
        public void StartPosition()
        {
            (var game, var white, var black) = CreateGame();
            GameAssert.ParserRoundTrip(game);
            GameAssert.BlackToMove(game);
            GameAssert.PieceAtSquare(game, "e8", black, "King");
            GameAssert.SquareThreatened(game, "e8", black);

            Assert.AreEqual(0, game.GetLegalMovesForPlayer(black).Count());
        }
    }
}
