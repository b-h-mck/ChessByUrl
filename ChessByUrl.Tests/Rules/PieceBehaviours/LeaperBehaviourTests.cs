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
    public class LeaperBehaviourTests
    {
        [TestMethod]
        public void GetLegalMovesFrom_Orthogonal()
        {
            var behaviour = new LeaperBehaviour(0, 2);

            string middleFrom = "e5";
            Coords[] expectedMiddleTos = ["g5", "c5", "e7", "e3"];

            string edgeFrom = "e1";
            Coords[] expectedEdgeTos = ["g1", "c1", "e3"];

            string cornerFrom = "h8";
            Coords[] expectedCornerTos = ["f8", "h6"];

            var expectedMiddleMoves = expectedMiddleTos.Select(to => new Move { From = middleFrom, To = to }).ToArray();
            var expectedEdgeMoves = expectedEdgeTos.Select(to => new Move { From = edgeFrom, To = to }).ToArray();
            var expectedCornerMoves = expectedCornerTos.Select(to => new Move { From = cornerFrom, To = to }).ToArray();

            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);

            var actualMiddleMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, middleFrom, pieceType);
            var actualEdgeMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, edgeFrom, pieceType);
            var actualCornerMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, cornerFrom, pieceType);

            CollectionAssert.AreEqual(expectedMiddleMoves, actualMiddleMoves);
            CollectionAssert.AreEqual(expectedEdgeMoves, actualEdgeMoves);
            CollectionAssert.AreEqual(expectedCornerMoves, actualCornerMoves);
        }


        [TestMethod]
        public void GetLegalMovesFrom_Diagonal()
        {
            var behaviour = new LeaperBehaviour(2,2);

            string middleFrom = "e5";
            Coords[] expectedMiddleTos = ["g7", "c7", "g3", "c3"];

            string edgeFrom = "e1";
            Coords[] expectedEdgeTos = ["g3", "c3"];

            string cornerFrom = "h8";
            Coords[] expectedCornerTos = ["f6"];


            var expectedMiddleMoves = expectedMiddleTos.Select(to => new Move { From = middleFrom, To = to }).ToArray();
            var expectedEdgeMoves = expectedEdgeTos.Select(to => new Move { From = edgeFrom, To = to }).ToArray();
            var expectedCornerMoves = expectedCornerTos.Select(to => new Move { From = cornerFrom, To = to }).ToArray();

            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);

            var actualMiddleMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, middleFrom, pieceType);
            var actualEdgeMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, edgeFrom, pieceType);
            var actualCornerMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, cornerFrom, pieceType);

            CollectionAssert.AreEqual(expectedMiddleMoves, actualMiddleMoves);
            CollectionAssert.AreEqual(expectedEdgeMoves, actualEdgeMoves);
            CollectionAssert.AreEqual(expectedCornerMoves, actualCornerMoves);
        }

        [TestMethod]
        public void GetLegalMovesFrom_KnightMove()
        {
            var behaviour = new LeaperBehaviour(1,2);

            string middleFrom = "e5";
            Coords[] expectedMiddleTos = ["g6", "c6", "g4", "c4", "f7", "d7", "f3", "d3"];

            string edgeFrom = "e1";
            Coords[] expectedEdgeTos = ["g2", "c2", "f3", "d3"];

            string cornerFrom = "h8";
            Coords[] expectedCornerTos = ["f7", "g6"];

            var expectedMiddleMoves = expectedMiddleTos.Select(to => new Move { From = middleFrom, To = to }).ToArray();
            var expectedEdgeMoves = expectedEdgeTos.Select(to => new Move { From = edgeFrom, To = to }).ToArray();
            var expectedCornerMoves = expectedCornerTos.Select(to => new Move { From = cornerFrom, To = to }).ToArray();

            var fakes = new Fakes();
            var pieceType = fakes.AddPieceType(0, behaviour);

            var actualMiddleMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, middleFrom, pieceType);
            var actualEdgeMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, edgeFrom, pieceType);
            var actualCornerMoves = behaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, cornerFrom, pieceType);

            CollectionAssert.AreEqual(expectedMiddleMoves, actualMiddleMoves);
            CollectionAssert.AreEqual(expectedEdgeMoves, actualEdgeMoves);
            CollectionAssert.AreEqual(expectedCornerMoves, actualCornerMoves);
        }

    }
}
