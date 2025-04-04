﻿using ChessByUrl.Parser.Orthodox;
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
    /// Tests analysing the quickest possible checkmate in chess.
    /// </summary>
    [TestClass]
    public class FoolsMate : MoveSequenceTestBase
    {
        protected override IEnumerable<Move> MoveSequence => [
            new Move("f2", "f3"),
                new Move("e7", "e6"),
                new Move("g2", "g4"),
                new Move("d8", "h4")
        ];

        [TestMethod]
        public void StartOfGame()
        {
            (var game, var white, var black) = CreateGame(0);

            Assert.AreEqual(white, game.CurrentPlayer);
            GameAssert.ParserRoundTrip(game);

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
            GameAssert.ParserRoundTrip(game);

            Assert.AreEqual(black, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "f2");
            GameAssert.PieceAtSquare(game, "f3", white, "Pawn");
            Assert.IsTrue(!game.Status.IsFinished);
        }

        [TestMethod]
        public void AfterBlacksFirstMove()
        {
            (var game, var white, var black) = CreateGame(2);
            GameAssert.ParserRoundTrip(game);

            Assert.AreEqual(white, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "e7");
            GameAssert.PieceAtSquare(game, "e6", black, "Pawn");
            Assert.IsTrue(!game.Status.IsFinished);
        }
        [TestMethod]
        public void AfterWhitesSecondMove()
        {
            (var game, var white, var black) = CreateGame(3);
            GameAssert.ParserRoundTrip(game);

            Assert.AreEqual(black, game.CurrentPlayer);
            GameAssert.SquareEmpty(game, "g2");
            GameAssert.PieceAtSquare(game, "g4", white, "Pawn vulnerable to en passant");
            Assert.IsTrue(!game.Status.IsFinished);
        }

        [TestMethod]
        public void AfterBlacksCheckmatingMove()
        {
            (var game, var white, var black) = CreateGame(4);
            GameAssert.ParserRoundTrip(game);

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
