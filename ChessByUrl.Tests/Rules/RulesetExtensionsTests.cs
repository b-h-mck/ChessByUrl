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
    public class RulesetExtensionsTests
    {

        


        [TestMethod]
        public void GetLegalMoves_ReturnsFilteredMoves()
        {
            var fakes = CreateFakes();

            var moves = fakes.Ruleset.GetLegalMoves(fakes.Board, new Coords(1, 1));
            CollectionAssert.AreEqual([new Move { From = new Coords(1, 1), To = new Coords(4, 4) }], moves.ToArray());
            moves = fakes.Ruleset.GetLegalMoves(fakes.Board, new Coords(2, 2));
            CollectionAssert.AreEqual([new Move { From = new Coords(2, 2), To = new Coords(4, 4) }], moves.ToArray());
            moves = fakes.Ruleset.GetLegalMoves(fakes.Board, new Coords(3, 3));
            CollectionAssert.AreEqual([], moves.ToArray());
        }

        [TestMethod]
        public void GetThreats_ReturnsUnfilteredThreats()
        {
            var fakes = CreateFakes();
            var threats = fakes.Ruleset.GetThreats(fakes.Board, new Coords(4, 4), fakes.Ruleset.Players.Last());
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
            var result = fakes.Ruleset.ApplyMove(fakes.Board, move);
            var pieceType = fakes.Ruleset.PieceTypes.First();
            Assert.IsNotNull(result);
            Assert.AreEqual(pieceType, result.GetPiece(new Coords(4, 4)));
            Assert.AreEqual(pieceType, result.GetPiece(new Coords(7, 0)));
            Assert.AreEqual(pieceType , result.GetPiece(new Coords(6, 0)));
            Assert.AreEqual(1, result.CurrentPlayer.Id);
        }

        private Fakes CreateFakes()
        {
            var result = new Fakes(); //.AddPiecesWithBehaviour(0, new FakeBehaviour(), "b2", "c3", "d4");
            var pieceType = result.AddPieceType(0, new FakeBehaviour());
            result.AddPieces(pieceType, "b2", "c3", "d4");
            return result;
        }

        private class FakeBehaviour : IGetLegalMovesBehaviour, IFilterLegalMoveCandidatesBehaviour, IApplyMoveBehaviour, IAdjustBoardAfterMoveBehaviour
        {
            public IEnumerable<Move> GetLegalMovesFrom(IRuleset ruleset, Board board, Coords from, PieceType fromPiece) => [
                new Move { From = from, To = new Coords(4,4) }
            ];
            public IEnumerable<Move> FilterLegalMoveCandidates(IRuleset ruleset, Board board, Coords thisSquare, PieceType thisPiece, IEnumerable<Move> candidates) =>
                candidates.Where(c => c.From.Rank != 3);
            public Board ApplyMoveFrom(IRuleset ruleset, Board board, Board boardSoFar, Move move, PieceType fromPiece) => boardSoFar.ReplacePiece(new Coords(7, 0), fromPiece);
            public Board AdjustBoardAfterMove(IRuleset ruleset, Board boardAfterMove, Coords thisSquare, PieceType thisPiece, Move move) => boardAfterMove.ReplacePiece(new Coords(6, 0), thisPiece);
        }

    }
}
