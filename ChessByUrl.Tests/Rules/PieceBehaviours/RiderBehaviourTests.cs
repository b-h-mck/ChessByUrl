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
    public class RiderBehaviourTests
    {
        [TestMethod]
        public void GetLegalMovesFrom_Orthogonal()
        {
            var behaviour = new RiderBehaviour(new LeaperBehaviour(0, 1));

            string middleFrom = "e5";
            string edgeFrom = "e1";
            string cornerFrom = "h8";

            string[] expectedMiddleTos = {
                "f5", "g5", "h5",
                "d5", "c5", "b5", "a5",
                "e6", "e7", "e8",
                "e4", "e3", "e2", "e1",
                };
            string[] expectedEdgeTos = {
                "f1", "g1", "h1",
                "d1", "c1", "b1", "a1",
                "e2", "e3", "e4", "e5", "e6", "e7", "e8",
                };
            string[] expectedCornerTos = {
                "g8", "f8", "e8", "d8", "c8", "b8", "a8",
                "h7", "h6", "h5", "h4", "h3", "h2", "h1",
                };

            var expectedMiddleMoves = expectedMiddleTos.Select(to => new Move { From = middleFrom, To = to }).ToArray();
            var expectedEdgeMoves = expectedEdgeTos.Select(to => new Move { From = edgeFrom, To = to }).ToArray();
            var expectedCornerMoves = expectedCornerTos.Select(to => new Move { From = cornerFrom, To = to }).ToArray();

            var fakes = new Fakes(); //.AddPiecesWithBehaviour(0, behaviour);
            var pieceType = fakes.AddPieceType(0, behaviour);

            var actualMiddleMoves = behaviour.GetLegalMovesFrom(fakes.Game, middleFrom, pieceType);
            var actualEdgeMoves = behaviour.GetLegalMovesFrom(fakes.Game, edgeFrom, pieceType);
            var actualCornerMoves = behaviour.GetLegalMovesFrom(fakes.Game, cornerFrom, pieceType);

            CollectionAssert.AreEqual(expectedMiddleMoves, actualMiddleMoves);
            CollectionAssert.AreEqual(expectedEdgeMoves, actualEdgeMoves);
            CollectionAssert.AreEqual(expectedCornerMoves, actualCornerMoves);
        }


        [TestMethod]
        public void GetLegalMovesFrom_Diagonal()
        {
            var behaviour = new RiderBehaviour(new LeaperBehaviour(1, 1));

            string middleFrom = "e5";
            string edgeFrom = "e1";
            string cornerFrom = "h8";

            string[] expectedMiddleTos = {
                "f6", "g7", "h8",
                "d6", "c7", "b8",
                "f4", "g3", "h2",
                "d4", "c3", "b2", "a1"
                };
            string[] expectedEdgeTos = {
                "f2", "g3", "h4",
                "d2", "c3", "b4", "a5"
                };
            string[] expectedCornerTos = {
                "g7", "f6", "e5", "d4", "c3", "b2", "a1"
                };

            var expectedMiddleMoves = expectedMiddleTos.Select(to => new Move { From = middleFrom, To = to }).ToArray();
            var expectedEdgeMoves = expectedEdgeTos.Select(to => new Move { From = edgeFrom, To = to }).ToArray();
            var expectedCornerMoves = expectedCornerTos.Select(to => new Move { From = cornerFrom, To = to }).ToArray();


            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);

            var actualMiddleMoves = behaviour.GetLegalMovesFrom(fakes.Game, middleFrom, pieceType);
            var actualEdgeMoves = behaviour.GetLegalMovesFrom(fakes.Game, edgeFrom, pieceType);
            var actualCornerMoves = behaviour.GetLegalMovesFrom(fakes.Game, cornerFrom, pieceType);

            CollectionAssert.AreEqual(expectedMiddleMoves, actualMiddleMoves);
            CollectionAssert.AreEqual(expectedEdgeMoves, actualEdgeMoves);
            CollectionAssert.AreEqual(expectedCornerMoves, actualCornerMoves);
        }
    }
}
