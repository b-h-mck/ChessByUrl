using ChessByUrl.Rules;
using ChessByUrl.Rules.PieceBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Rules
{
    [TestClass]
    public class GameTests
    {

        


        [TestMethod]
        public void GetLegalMovesFromSquare_ReturnsFilteredMoves()
        {
            var fakes = CreateFakes();

            var moves = fakes.Game.GetLegalMovesFromSquare(new Coords(1, 1));
            CollectionAssert.AreEqual([new Move { From = new Coords(1, 1), To = new Coords(4, 4) }], moves.ToArray());
            moves = fakes.Game.GetLegalMovesFromSquare(new Coords(2, 2));
            CollectionAssert.AreEqual([new Move { From = new Coords(2, 2), To = new Coords(4, 4) }], moves.ToArray());
            moves = fakes.Game.GetLegalMovesFromSquare(new Coords(3, 3));
            CollectionAssert.AreEqual([], moves.ToArray());
        }

        [TestMethod]
        public void GetThreats_ReturnsUnfilteredThreats()
        {
            var fakes = CreateFakes();

            var threats = fakes.Game.GetThreats(new Coords(4, 4), fakes.Ruleset.Players.Last());
            CollectionAssert.AreEqual(
                [
                    new Move { From = new Coords(1, 1), To = new Coords(4, 4) },
                    new Move { From = new Coords(2, 2), To = new Coords(4, 4) },
                    new Move { From = new Coords(3, 3), To = new Coords(4, 4) }
                ], threats);
        }

        [TestMethod]
        public void ApplyMove_ReturnsAppliedAndAdjustedBoard()
        {
            var fakes = CreateFakes();

            var move = new Move { From = new Coords(1, 1), To = new Coords(4, 4) };
            var gameAfterMove = fakes.Game.ApplyMove(move);

            var pieceType = fakes.Ruleset.PieceTypes.First();
            Assert.IsNotNull(gameAfterMove);
            Assert.AreEqual(pieceType, gameAfterMove.CurrentBoard.GetPiece(new Coords(4, 4)));
            Assert.AreEqual(pieceType, gameAfterMove.CurrentBoard.GetPiece(new Coords(7, 0)));
            Assert.AreEqual(pieceType , gameAfterMove.CurrentBoard.GetPiece(new Coords(6, 0)));
            Assert.AreEqual(1, gameAfterMove.CurrentPlayer.Id);
        }

        private Fakes CreateFakes()
        {
            var result = new Fakes();
            var pieceType = result.AddPieceType(0, new FakeBehaviour());
            result.AddPieces(pieceType, "b2", "c3", "d4");
            return result;
        }

        private class FakeBehaviour : IGetLegalMovesBehaviour, IFilterLegalMoveCandidatesBehaviour, IApplyMoveBehaviour, IAdjustBoardAfterMoveBehaviour
        {
            public IEnumerable<Move> GetLegalMovesFrom(Game game, Coords from, PieceType fromPiece) => [
                new Move { From = from, To = new Coords(4,4) }
            ];
            public IEnumerable<Move> FilterLegalMoveCandidates(Game game, Coords thisSquare, PieceType thisPiece, IEnumerable<Move> candidates) =>
                candidates.Where(c => c.From.Rank != 3);
            public Board ApplyMoveFrom(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, PieceType fromPiece) => boardAfterMoveSoFar.ReplacePiece(new Coords(7, 0), fromPiece);
            public Board AdjustBoardAfterMove(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, Coords thisSquare, PieceType thisPiece) => boardAfterMoveSoFar.ReplacePiece(new Coords(6, 0), thisPiece);

        }

    }
}
