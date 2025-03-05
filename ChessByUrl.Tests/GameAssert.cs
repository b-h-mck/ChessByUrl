using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests
{
    public static class GameAssert
    {
        public static void WhiteToMove(Game game)
        {
            Assert.AreEqual("White", game.CurrentPlayer.Name);
        }

        public static void BlackToMove(Game game)
        {
            Assert.AreEqual("Black", game.CurrentPlayer.Name);
        }

        public static void SquareEmpty(Game game, Coords square)
        {
            var piece = game.CurrentBoard.GetPiece(square);
            Assert.IsNull(piece);
        }

        public static void PieceAtSquare(Game game, Coords square, Player player, string pieceName)
        {
            var piece = game.CurrentBoard.GetPiece(square);
            Assert.IsNotNull(piece);
            Assert.AreEqual(pieceName, piece.Name);
        }

        public static void MoveLegal(Game game, Move move, string? message = null)
        {
            var legalMoves = game.GetLegalMovesFromSquare(move.From);
            Assert.IsTrue(legalMoves.Any(m => m.To == move.To), message ?? $"Expected {move} to be legal.");
        }

        public static void MoveIllegal(Game game, Move move, string? message = null)
        {
            var legalMoves = game.GetLegalMovesFromSquare(move.From);
            Assert.IsFalse(legalMoves.Any(m => m.To == move.To), message ?? $"Expected {move} to be illegal.");
        }
    }
}
