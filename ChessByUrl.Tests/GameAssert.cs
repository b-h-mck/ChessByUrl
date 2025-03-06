using ChessByUrl.Parser;
using ChessByUrl.Parser.Orthodox;
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

        public static void SquareThreatened(Game game, Coords square, Player threatenedPlayer)
        {
            var threats = game.GetThreats(square, threatenedPlayer);
            Assert.AreNotEqual(0, threats.Count(), $"Expected {square} to be threatened.");
        }
        public static void SquareNotThreatened(Game game, Coords square, Player threatenedPlayer)
        {
            var threats = game.GetThreats(square, threatenedPlayer);
            Assert.AreEqual(0, threats.Count(), $"Expected {square} not to be threatened.");
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
        public static void ParserRoundTrip(Game game)
        {
            var rulesetString = ParserCollection.Instance.SerialiseRuleset(game.Ruleset);
            var boardString = ParserCollection.Instance.SerialiseBoard(game.Ruleset, game.InitialBoard);
            var movesString = ParserCollection.Instance.SerialiseMoves(game.Ruleset, game.InitialBoard, game.MovesSoFar);

            Assert.IsFalse(string.IsNullOrWhiteSpace(rulesetString), "Expected non-empty ruleset string");
            Assert.IsFalse(string.IsNullOrWhiteSpace(boardString), "Expected non-empty board string");
            Assert.IsFalse(string.IsNullOrWhiteSpace(movesString), "Expected non-empty moves string");

            var parsedRuleset = new RulesetParser().Parse(rulesetString);
            Assert.IsNotNull(parsedRuleset, "Expected to parse ruleset");
            var parsedBoard = new StartBoardParser().Parse(parsedRuleset, boardString);
            Assert.IsNotNull(parsedBoard, "Expected to parse board");
            var parsedMoves = new EnumeratedMovesParser().Parse(parsedRuleset, parsedBoard, movesString);
            Assert.IsNotNull(parsedMoves, "Expected to parse moves");

            CollectionAssert.AreEqual(game.InitialBoard, parsedBoard);
            CollectionAssert.AreEqual(game.MovesSoFar, parsedMoves);
        }

    }
}
