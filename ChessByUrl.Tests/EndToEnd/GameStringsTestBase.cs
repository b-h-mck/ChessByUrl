using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules.Rulesets.Orthodox;
using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessByUrl.Parser;

namespace ChessByUrl.Tests.EndToEnd
{
    public abstract class GameStringsTestBase
    {
        protected (Game game, Player white, Player black) CreateGame(int numberOfMovesPerformed)
        {
            var parserCollection = new ParserCollection();
            var ruleset = parserCollection.ParseRuleset(RulesetString);
            Assert.IsNotNull(ruleset);
            var board = parserCollection.ParseBoard(ruleset, BoardString);
            Assert.IsNotNull(board);
            var moves = parserCollection.ParseMoves(ruleset, board, MovesString);
            if (string.IsNullOrEmpty(MovesString))
                Assert.IsNull(moves);
            else
                Assert.IsNotNull(moves);

            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);

            var game = new Game(ruleset, board);
            return (game, white, black);
        }

        protected virtual string RulesetString => "o";
        protected virtual string BoardString => "s";
        protected virtual string MovesString => "";

        protected void AssertWhiteToMove(Game game)
        {
            Assert.AreEqual("White", game.CurrentPlayer.Name);
        }

        protected void AssertBlackToMove(Game game)
        {
            Assert.AreEqual("Black", game.CurrentPlayer.Name);
        }

        protected void AssertSquareEmpty(Game game, Coords square)
        {
            var piece = game.CurrentBoard.GetPiece(square);
            Assert.IsNull(piece);
        }

        protected void AssertPieceAtSquare(Game game, Coords square, Player player, string pieceName)
        {
            var piece = game.CurrentBoard.GetPiece(square);
            Assert.IsNotNull(piece);
            Assert.AreEqual(pieceName, piece.Name);
        }

        protected void AssertMoveLegal(Game game, Move move, string? message = null)
        {
            var legalMoves = game.GetLegalMovesFromSquare(move.From);
            Assert.IsTrue(legalMoves.Any(m => m.To == move.To), message ?? $"Expected {move} to be legal.");
        }
        protected void AssertMoveIllegal(Game game, Move move, string? message = null)
        {
            var legalMoves = game.GetLegalMovesFromSquare(move.From);
            Assert.IsFalse(legalMoves.Any(m => m.To == move.To), message ?? $"Expected {move} to be illegal.");
        }

        protected void AssertParserRoundTrip(Game game)
        {
            var rulesetString = new RulesetParser().Serialise(game.Ruleset);
            var boardString = new StartBoardParser().Serialise(game.Ruleset, game.InitialBoard);
            var movesString = new EnumeratedMovesParser().Serialise(game.Ruleset, game.InitialBoard, game.MovesSoFar);

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