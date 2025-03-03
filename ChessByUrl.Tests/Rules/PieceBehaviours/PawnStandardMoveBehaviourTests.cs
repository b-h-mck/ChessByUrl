using ChessByUrl.Rules;
using ChessByUrl.Rules.PieceBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Rules.PieceBehaviours
{
    [TestClass]
    public class PawnStandardMoveBehaviourTests
    {
        [TestMethod]
        public void GetLegalMovesFrom_White_EmptySquareAhead()
        {
            var behaviour = new PawnStandardMoveBehaviour();
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b2", pieceType);
            Move[] expectedMoves = [ new Move { From = "b2", To = "b3" } ];
            CollectionAssert.AreEqual(expectedMoves, actualMoves);

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "c3", pieceType);
            expectedMoves = [ new Move { From = "c3", To = "c4" } ];
            CollectionAssert.AreEqual(expectedMoves, actualMoves);

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "g7", pieceType);
            expectedMoves = [ new Move { From = "g7", To = "g8" } ];
            CollectionAssert.AreEqual(expectedMoves, actualMoves);
        }

        [TestMethod]
        public void GetLegalMovesFrom_White_OpponentAhead()
        {
            var behaviour = new PawnStandardMoveBehaviour();
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);
            var opponentPieceType = fakes.AddDummyPieceType(1);
            fakes.AddPieces(opponentPieceType, "b3", "c4", "g8");

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b2", pieceType);
            Assert.AreEqual(0, actualMoves.Count());

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "c3", pieceType);
            Assert.AreEqual(0, actualMoves.Count());

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "g7", pieceType);
            Assert.AreEqual(0, actualMoves.Count());
        }

        [TestMethod]
        public void GetLegalMovesFrom_White_SamePlayerAhead()
        {
            var behaviour = new PawnStandardMoveBehaviour();
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);
            fakes.AddPieces(pieceType, "b3", "c4", "g8");

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b2", pieceType);
            Assert.AreEqual(0, actualMoves.Count());

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "c3", pieceType);
            Assert.AreEqual(0, actualMoves.Count());

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "g7", pieceType);
            Assert.AreEqual(0, actualMoves.Count());
        }

        [TestMethod]
        public void GetLegalMovesFrom_Black_EmptySquareAhead()
        {
            var behaviour = new PawnStandardMoveBehaviour();
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(1, behaviour);
            fakes.Board = fakes.Board.SetCurrentPlayer(fakes.Ruleset.Players.Last());

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b7", pieceType);
            Move[] expectedMoves = [ new Move { From = "b7", To = "b6" } ];
            CollectionAssert.AreEqual(expectedMoves, actualMoves);

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "c6", pieceType);
            expectedMoves = [ new Move { From = "c6", To = "c5" } ];
            CollectionAssert.AreEqual(expectedMoves, actualMoves);

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "g2", pieceType);
            expectedMoves = [ new Move { From = "g2", To = "g1" } ];
            CollectionAssert.AreEqual(expectedMoves, actualMoves);
        }

        [TestMethod]
        public void GetLegalMovesFrom_Black_OpponentAhead()
        {
            var behaviour = new PawnStandardMoveBehaviour();
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(1, behaviour);
            var opponentPieceType = fakes.AddDummyPieceType(0);
            fakes.AddPieces(opponentPieceType, "b6", "c5", "g1");
            fakes.Board = fakes.Board.SetCurrentPlayer(fakes.Ruleset.Players.Last());

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b7", pieceType);
            Assert.AreEqual(0, actualMoves.Count());

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "c6", pieceType);
            Assert.AreEqual(0, actualMoves.Count());

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "g2", pieceType);
            Assert.AreEqual(0, actualMoves.Count());
        }

        [TestMethod]
        public void GetLegalMovesFrom_Black_SamePlayerAhead()
        {
            var behaviour = new PawnStandardMoveBehaviour();
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(1, behaviour);
            fakes.AddPieces(pieceType, "b6", "c5", "g1");
            fakes.Board = fakes.Board.SetCurrentPlayer(fakes.Ruleset.Players.Last());

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b7", pieceType);
            Assert.AreEqual(0, actualMoves.Count());

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "c6", pieceType);
            Assert.AreEqual(0, actualMoves.Count());

            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "g2", pieceType);
            Assert.AreEqual(0, actualMoves.Count());
        }
    }
}
