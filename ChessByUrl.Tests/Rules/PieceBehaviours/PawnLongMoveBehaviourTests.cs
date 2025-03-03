using ChessByUrl.Rules.PieceBehaviours;
using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Rules.PieceBehaviours
{
    [TestClass]
    public class PawnLongMoveBehaviourTests
    {
        [TestMethod]
        public void GetLegalMovesFrom_WrongRank()
        {
            var behaviour = new PawnLongMoveBehaviour(1, 3, null);
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b3", pieceType);
            Assert.AreEqual(0, actualMoves.Count());
        }

        [TestMethod]
        public void GetLegalMovesFrom_EmptySquareAhead()
        {
            var behaviour = new PawnLongMoveBehaviour(1, 3, null);
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b2", pieceType);
            Move[] expectedMoves = { new Move { From = "b2", To = "b4" } };
            CollectionAssert.AreEqual(expectedMoves, actualMoves);
        }

        [TestMethod]
        public void GetLegalMovesFrom_OpponentAhead()
        {
            var behaviour = new PawnLongMoveBehaviour(1, 3, null);
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);
            var opponentPieceType = fakes.AddDummyPieceType(1);
            fakes.AddPieces(opponentPieceType, "b4", "c3");

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b2", fakes.Ruleset.PieceTypes.First());
            Assert.AreEqual(0, actualMoves.Count());
            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "c2", fakes.Ruleset.PieceTypes.First());
            Assert.AreEqual(0, actualMoves.Count());
        }

        [TestMethod]
        public void GetLegalMovesFrom_SamePlayerAhead()
        {
            var behaviour = new PawnLongMoveBehaviour(1, 3, null);
            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);
            var samePlayerPieceType = fakes.AddDummyPieceType(0);
            fakes.AddPieces(samePlayerPieceType, "b4", "c3");

            var actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "b2", fakes.Ruleset.PieceTypes.First());
            Assert.AreEqual(0, actualMoves.Count());
            actualMoves = behaviour.GetLegalMovesFrom(fakes.Game, "c2", fakes.Ruleset.PieceTypes.First());
            Assert.AreEqual(0, actualMoves.Count());
        }

        [TestMethod]
        public void ApplyMove_TransformsPiece()
        {
            var fakes = new Fakes();
            var transformPieceType = fakes.AddDummyPieceType(0);
            var behaviour = new PawnLongMoveBehaviour(1, 3, () => transformPieceType);
            var originalPieceType = fakes.AddPieceType(0, behaviour);

            fakes.AddPieces(originalPieceType, "d2");
            var gameAfterMove = new Game(fakes.Game, new Move { From = "d2", To = "d4" });
            var actualPieceType = gameAfterMove.CurrentBoard.GetPiece("d4");
            Assert.IsNotNull(actualPieceType);
            Assert.AreEqual(transformPieceType.Id, actualPieceType.Id);
        }
    }
}
