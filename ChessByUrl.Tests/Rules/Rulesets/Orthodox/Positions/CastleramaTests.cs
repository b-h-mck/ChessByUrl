using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules;
using ChessByUrl.Rules.PieceBehaviours;
using ChessByUrl.Rules.Rulesets.Orthodox;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Rules.Rulesets.Orthodox.Positions
{
    /// <summary>
    /// Board after 1.b3 b6 2. e3 e6 3. Nc3 Nc6 4. Qf3 Qf6 5. Nh3 Nh6.
    /// From this position, Bd3/Bd6 will allow each side to castle kingside, and Ba3/Ba6 will allow queenside.
    /// The king and both rooks also have space to shake off castling rights, and it's pretty easy for the queens to prevent castling.
    /// </summary>
    [TestClass]
    public class CastleramaTests
    {

        private (Board board, Player white, Player black) CreateBoard(OrthodoxRuleset ruleset)
        {
            var boardParser = new CustomBoardParser();
            var board = boardParser.Parse(ruleset, "cBgAEAkAECkMbARYyIg0VDjZxJiIJGiIhJWVdRS1yRUlrVTsGbnIsXVY2dnZrYRwiJ3xVbyE");
            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);
            Assert.IsNotNull(board);
            Assert.AreEqual(white, board.CurrentPlayer);
            return (board, white, black);
        }

        private void AssertWhiteToMove(Board board)
        {
            Assert.AreEqual("White", board.CurrentPlayer.Name);
        }

        private void AssertBlackToMove(Board board)
        {
            Assert.AreEqual("Black", board.CurrentPlayer.Name);
        }

        private void AssertMoveLegal(OrthodoxRuleset ruleset, Board board, Coords from, Coords to, string? message = null)
        {
            var legalMoves = ruleset.GetLegalMoves(board, from);
            Assert.IsTrue(legalMoves.Any(m => m.To == to), message ?? $"Expected {from}-{to} to be legal.");
        }
        private void AssertMoveIllegal(OrthodoxRuleset ruleset, Board board, Coords from, Coords to, string? message = null)
        {
            var legalMoves = ruleset.GetLegalMoves(board, from);
            Assert.IsFalse(legalMoves.Any(m => m.To == to), message ?? $"Expected {from}-{to} to be illegal.");
        }

        private Board AssertAndApplyMove(OrthodoxRuleset ruleset, Board board, Coords from, Coords to, string? message = null)
        {
            AssertMoveLegal(ruleset, board, from, to, message);
            return ruleset.ApplyMove(board, new Move { From = from, To = to });
        }

        [TestMethod]
        public void IsInProgess()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            var gameStatus = ruleset.GetGameStatus(board);
            Assert.IsNotNull(gameStatus);
            Assert.IsTrue(!gameStatus.IsFinished);
            Assert.IsNull(gameStatus.PlayerPoints);
        }

        [TestMethod]
        public void InitialStateCorrect()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            Assert.AreEqual("Rook with castling rights", board.GetPiece("a1")?.Name);
            Assert.AreEqual("Rook with castling rights", board.GetPiece("h1")?.Name);
            Assert.AreEqual("Rook with castling rights", board.GetPiece("a8")?.Name);
            Assert.AreEqual("Rook with castling rights", board.GetPiece("h8")?.Name);
            Assert.AreEqual("King", board.GetPiece("e1")?.Name);
            Assert.AreEqual("King", board.GetPiece("e8")?.Name);

            var kingMoves = ruleset.GetLegalMoves(board, "e1");
            Assert.AreEqual(2, kingMoves.Count());
            Assert.AreEqual((Coords)"e2", kingMoves.First().To);
            Assert.AreEqual((Coords)"d1", kingMoves.Last().To);
        }

        [TestMethod]
        public void CanCastleKingsideWhenBishopMoved()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            board = AssertAndApplyMove(ruleset, board, "f1", "d3");
            board = AssertAndApplyMove(ruleset, board, "f8", "d6");
            AssertWhiteToMove(board);

            AssertMoveLegal(ruleset, board, "e1", "g1", "Expected white kingside castling to be legal");
            AssertMoveLegal(ruleset, board, "e8", "g8", "Expected black kingside castling to be legal");
            AssertMoveIllegal(ruleset, board, "e1", "c1", "Expected white queenside castling to be illegal");
            AssertMoveIllegal(ruleset, board, "e8", "c8", "Expected black queenside castling to be illegal");
        }

        [TestMethod]
        public void CanCastleQueensideWhenBishopMoved()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            board = AssertAndApplyMove(ruleset, board, "c1", "a3");
            board = AssertAndApplyMove(ruleset, board, "c8", "a6");
            AssertWhiteToMove(board);

            AssertMoveLegal(ruleset, board, "e1", "c1", "Expected white queenside castling to be legal");
            AssertMoveLegal(ruleset, board, "e8", "c8", "Expected black queenside castling to be legal");
            AssertMoveIllegal(ruleset, board, "e1", "g1", "Expected white kingside castling to be illegal");
            AssertMoveIllegal(ruleset, board, "e8", "g8", "Expected black kingside castling to be illegal");
        }

        [TestMethod]
        public void CanCastleKingsideAndQueensideWhenBothBishopsMoved()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            // Move all bishops out of the way.
            board = AssertAndApplyMove(ruleset, board, "f1", "d3");
            board = AssertAndApplyMove(ruleset, board, "f8", "d6");
            board = AssertAndApplyMove(ruleset, board, "c1", "a3");
            board = AssertAndApplyMove(ruleset, board, "c8", "a6");
            AssertWhiteToMove(board);

            AssertMoveLegal(ruleset, board, "e1", "g1", "Expected white kingside castling to be legal");
            AssertMoveLegal(ruleset, board, "e8", "g8", "Expected black kingside castling to be legal");
            AssertMoveLegal(ruleset, board, "e1", "c1", "Expected white queenside castling to be legal");
            AssertMoveLegal(ruleset, board, "e8", "c8", "Expected black queenside castling to be legal");
        }

        [TestMethod]
        public void CastlingForbiddenAfterMovingRooks()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            // Move all bishops out of the way.
            board = AssertAndApplyMove(ruleset, board, "f1", "d3");
            board = AssertAndApplyMove(ruleset, board, "f8", "d6");
            board = AssertAndApplyMove(ruleset, board, "c1", "a3");
            board = AssertAndApplyMove(ruleset, board, "c8", "a6");

            // Shake off castling rights for white kingside and black queenside rooks.
            board = AssertAndApplyMove(ruleset, board, "h1", "g1");
            board = AssertAndApplyMove(ruleset, board, "a8", "b8");
            board = AssertAndApplyMove(ruleset, board, "g1", "h1");
            board = AssertAndApplyMove(ruleset, board, "b8", "a8");
            AssertWhiteToMove(board);

            AssertMoveLegal(ruleset, board, "e8", "g8", "Expected black kingside castling to be legal");
            AssertMoveLegal(ruleset, board, "e1", "c1", "Expected white queenside castling to be legal");
            AssertMoveIllegal(ruleset, board, "e1", "g1", "Expected white kingside castling to be illegal after rook move");
            AssertMoveIllegal(ruleset, board, "e8", "c8", "Expected black queenside castling to be illegal after rook move");

            // Shake off castling rights for the remaining rooks.
            board = AssertAndApplyMove(ruleset, board, "a1", "b1");
            board = AssertAndApplyMove(ruleset, board, "h8", "g8");
            board = AssertAndApplyMove(ruleset, board, "b1", "a1");
            board = AssertAndApplyMove(ruleset, board, "g8", "h8");
            AssertWhiteToMove(board);

            AssertMoveIllegal(ruleset, board, "e1", "g1", "Expected white kingside castling to be illegal after rook move");
            AssertMoveIllegal(ruleset, board, "e8", "g8", "Expected black kingside castling to be illegal after rook move");
            AssertMoveIllegal(ruleset, board, "e1", "c1", "Expected white queenside castling to be illegal after rook move");
            AssertMoveIllegal(ruleset, board, "e8", "c8", "Expected black queenside castling to be illegal after rook move");
        }

        [TestMethod]
        public void CastlingForbiddenAfterMovingKings()
        {
            var ruleset = new OrthodoxRuleset();
            (var board, var white, var black) = CreateBoard(ruleset);

            // Move all bishops out of the way.
            board = AssertAndApplyMove(ruleset, board, "f1", "d3");
            board = AssertAndApplyMove(ruleset, board, "f8", "d6");
            board = AssertAndApplyMove(ruleset, board, "c1", "a3");
            board = AssertAndApplyMove(ruleset, board, "c8", "a6");

            // Shake the kings, which should remove all castling rights.
            board = AssertAndApplyMove(ruleset, board, "e1", "f1");
            board = AssertAndApplyMove(ruleset, board, "e8", "f8");
            board = AssertAndApplyMove(ruleset, board, "f1", "e1");
            board = AssertAndApplyMove(ruleset, board, "f8", "e8");
            AssertWhiteToMove(board);

            AssertMoveIllegal(ruleset, board, "e8", "g8", "Expected black kingside castling to be illegal after king move");
            AssertMoveIllegal(ruleset, board, "e1", "c1", "Expected white queenside castling to be illegal after king move");
            AssertMoveIllegal(ruleset, board, "e1", "g1", "Expected white kingside castling to be illegal after rook move");
            AssertMoveIllegal(ruleset, board, "e8", "c8", "Expected black queenside castling to be illegal after rook move");

            // Shake off castling rights for the remaining rooks.
            board = AssertAndApplyMove(ruleset, board, "a1", "b1");
            board = AssertAndApplyMove(ruleset, board, "h8", "g8");
            board = AssertAndApplyMove(ruleset, board, "b1", "a1");
            board = AssertAndApplyMove(ruleset, board, "g8", "h8");
            AssertWhiteToMove(board);

            AssertMoveIllegal(ruleset, board, "e1", "g1", "Expected white kingside castling to be illegal after rook move");
            AssertMoveIllegal(ruleset, board, "e8", "g8", "Expected black kingside castling to be illegal after rook move");
            AssertMoveIllegal(ruleset, board, "e1", "c1", "Expected white queenside castling to be illegal after rook move");
            AssertMoveIllegal(ruleset, board, "e8", "c8", "Expected black queenside castling to be illegal after rook move");
        }
    }
}
