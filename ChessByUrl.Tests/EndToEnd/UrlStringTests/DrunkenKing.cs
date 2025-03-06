using ChessByUrl.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.EndToEnd.UrlStringTests
{
    /// <summary>
    /// Tests analysing a position I came across after randomly clicking out a game, where moving Black's king from 
    /// f7-f6 seemed to make it go to f8, and other king moves also seem to make weird stuff happened.
    /// 
    /// This turned out to be a bug in EnumeratedMovesParser.
    /// </summary>
    [TestClass]
    public class DrunkenKing : UrlStringTestBase
    {
        protected override string MovesString => "eealcPg9uIBgeEQE";

        [TestMethod]
        public void StartPosition()
        {
            (var game, var white, var black) = CreateGame();
            GameAssert.ParserRoundTrip(game);
            GameAssert.BlackToMove(game);
            GameAssert.PieceAtSquare(game, "f7", black, "King");
            GameAssert.SquareThreatened(game, "f7", black);
            GameAssert.MoveLegal(game, new() { From = "f7", To = "f8" });
            GameAssert.MoveLegal(game, new() { From = "f7", To = "e7" });
            GameAssert.MoveLegal(game, new() { From = "f7", To = "e6" });
            GameAssert.MoveLegal(game, new() { From = "f7", To = "f6" });
            Assert.AreEqual(4, game.GetLegalMovesForPlayer(black).Count());
        }

        [TestMethod]
        public void AfterKingToF6()
        {
            (var game, var white, var black) = CreateGame();
            GameAssert.ParserRoundTrip(game);
            game = game.ApplyMove(new() { From = "f7", To = "f6" });

            GameAssert.ParserRoundTrip(game);
            GameAssert.WhiteToMove(game);
            GameAssert.PieceAtSquare(game, "f6", black, "King");
            GameAssert.SquareEmpty(game, "f8");
        }

        [TestMethod]
        public void MovesStringParserRoundTrip()
        {
            var ruleset = ParserCollection.Instance.ParseRuleset(RulesetString);
            Assert.IsNotNull(ruleset);
            var board = ParserCollection.Instance.ParseBoard(ruleset, BoardString);
            Assert.IsNotNull(board);

            var problemString = "ee6lcPg9uIBgeEQU";
            var movesAfterFirstParse = ParserCollection.Instance.ParseMoves(ruleset, board, problemString);
            var stringAfterFirstSerialise = ParserCollection.Instance.SerialiseMoves(ruleset, board, movesAfterFirstParse);
            Assert.AreEqual(problemString, stringAfterFirstSerialise);
        }

    }
}
