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
    public class CastlingBehavioursTests
    {

        private (CastlingInitiatorBehaviour initiatorBehaviour, PieceType initiatorPieceType, 
                 PieceType responderPieceType, PieceType responderTransformPieceType) 
            SetupPieceTypes(Fakes fakes)
        {
            var responderTransformPieceType = fakes.AddDummyPieceType(0);
            var initiatorBehaviour = new CastlingInitiatorBehaviour(2, 0, 3, () => responderTransformPieceType);
            var responderBehaviour = new CastlingResponderBehaviour();
            var initiatorPieceType = fakes.AddPieceType(0, initiatorBehaviour);
            var responderPieceType = fakes.AddPieceType(0, responderBehaviour);

            return (initiatorBehaviour, initiatorPieceType, responderPieceType, responderTransformPieceType);
        }


        [TestMethod]
        [DataRow("Happy path", null, null, true)]
        [DataRow("Responder in wrong square", "a2", null, false)]
        [DataRow("Responder in wrong square with blocker in responder square", "a2", "a1", false)]
        [DataRow("Responder route blocked", null, "b1", false)]
        [DataRow("Initiator target blocked", null, "c1", false)]
        [DataRow("Initiator route blocked", null, "d1", false)]
        [DataRow("Blocker on other side 1", null, "f1", true)]
        [DataRow("Blocker on other side 2", null, "g1", true)]
        [DataRow("Blocker on other side 3", null, "h1", true)]
        public void CastlingBasics(string caseDescription, string? responderCoordsString, string? blockerCoordsString, bool expectCastlingAvailable)
        {
            var responderCoords = (Coords)(responderCoordsString ?? "a1");
            var blockerCoords = blockerCoordsString != null ? (Coords)blockerCoordsString : null;
            var fakes = new Fakes();
            (var initiatorBehaviour, var initiatorPieceType, var responderPieceType, var responderTransformPieceType) = SetupPieceTypes(fakes);

            fakes.AddPieces(initiatorPieceType, "e1");
            fakes.AddPieces(responderPieceType, responderCoords);
            if (blockerCoords != null)
            {
                var blockerPieceType = fakes.AddDummyPieceType(0);
                fakes.AddPieces(blockerPieceType, blockerCoords);
            }

            if (expectCastlingAvailable) {
                var expectedLegalMoves = new Move[] { new Move { From = "e1", To = "c1" } };
                var actualLegalMoves = initiatorBehaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, "e1", responderTransformPieceType);
                CollectionAssert.AreEqual(expectedLegalMoves, actualLegalMoves, $"{caseDescription}: Expected castling to be available.");
            }
            else
            {
                var expectedLegalMoves = new Move[] { };
                var actualLegalMoves = initiatorBehaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, "e1", responderTransformPieceType);
                CollectionAssert.AreEqual(expectedLegalMoves, actualLegalMoves, $"{caseDescription}: Expected castling to be unavailable.");
            }
        }

        [TestMethod]
        [DataRow("Responder start threatened", "a1", true)]
        [DataRow("Responder route threatened", "b1", true)]
        [DataRow("Initiator destination threatened", "c1", false)]
        [DataRow("Initiator route threatened", "d1", false)]
        [DataRow("Initiator start threatened", "e1", false)]
        [DataRow("Other side threatened 1", "f1", true)]
        [DataRow("Other side threatened 2", "g1", true)]
        [DataRow("Other side threatened 3", "h1", true)]
        public void CastlingThreats(string caseDescription, string attackedSquareCoords, bool expectCastlingAvailable)
        {
            var attackedSquare = (Coords)attackedSquareCoords;
            var fakes = new Fakes();
            (var initiatorBehaviour, var initiatorPieceType, var responderPieceType, var responderTransformPieceType) = SetupPieceTypes(fakes);

            fakes.AddPieces(initiatorPieceType, "e1");
            fakes.AddPieces(responderPieceType, "a1");

            var attackerPieceType = fakes.AddAttackingPieceType(1, attackedSquare);
            fakes.AddPieces(attackerPieceType, "e4");

            var expectedLegalMovesCandidates = new Move[] { new Move { From = "e1", To = "c1" } };
            var legalMoveCandidates = initiatorBehaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, "e1", responderTransformPieceType);
            CollectionAssert.AreEqual(expectedLegalMovesCandidates, legalMoveCandidates, "Expected castling to be legal at Get stage (but filtered later).");

            if (expectCastlingAvailable) 
            {
                var expectedFilteredLegalMoves = new Move[] { new Move { From = "e1", To = "c1" } };
                var actualFilteredLegalMoves = initiatorBehaviour.FilterLegalMoveCandidates(fakes.Ruleset, fakes.Board, "e1", initiatorPieceType, legalMoveCandidates);
                CollectionAssert.AreEqual(expectedFilteredLegalMoves, actualFilteredLegalMoves, $"{caseDescription}: Expected castling to be available.");
            }
            else
            {
                var expectedFilteredLegalMoves = new Move[] { };
                var actualFilteredLegalMoves = initiatorBehaviour.FilterLegalMoveCandidates(fakes.Ruleset, fakes.Board, "e1", initiatorPieceType, legalMoveCandidates);
                CollectionAssert.AreEqual(expectedFilteredLegalMoves, actualFilteredLegalMoves, $"{caseDescription}:Expected castling to be filtered out.");
            }
        }

        //[TestMethod]
        //public void FilterLegalMoveCandidates_InitiatorStartThreatened_NoCastle()
        //{
        //    AssertCastleWithThreat("e1", false);
        //}

        //[TestMethod]
        //public void GetLegalMovesFrom_InitiatorRouteThreatened_NoCastle()
        //{
        //    AssertCastleWithThreat("e1", []);
        //}

        //[TestMethod]
        //public void GetLegalMovesFrom_InitiatorTargetThreatened_NoCastle()
        //{
        //    AssertCastleWithThreat("e1", []);
        //}

        //[TestMethod]
        //public void GetLegalMovesFrom_ResponderRouteThreatened_CanCastle()
        //{
        //    AssertCastleWithThreat("e1", [new Move { From = "e1", To=}]);

        //    var expectedLegalMoves = new Move[] { new Move { From = "e1", To = "c1" } };
        //    var actualLegalMoves = initiatorBehaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, "e1", responderTransformPieceType);
        //    CollectionAssert.AreEqual(expectedLegalMoves, actualLegalMoves);
        //}

        //[TestMethod]
        //public void GetLegalMovesFrom_ReponderStartThreatened_CanCastle()
        //{
        //    var fakes = new Fakes();
        //    (var initiatorBehaviour, var initiatorPieceType, var responderPieceType, var responderTransformPieceType) = SetupPieceTypes(fakes);

        //    fakes.AddPieces(initiatorPieceType, "e1");
        //    fakes.AddPieces(responderPieceType, "a1");

        //    var attackerPieceType = fakes.AddAttackingPieceType(1, "a1");
        //    fakes.AddPieces(attackerPieceType, "e4");

        //    var expectedLegalMoves = new Move[] { new Move { From = "e1", To = "c1" } };
        //    var actualLegalMoves = initiatorBehaviour.GetLegalMovesFrom(fakes.Ruleset, fakes.Board, "e1", responderTransformPieceType);
        //    CollectionAssert.AreEqual(expectedLegalMoves, actualLegalMoves);
        //}
    }
}
