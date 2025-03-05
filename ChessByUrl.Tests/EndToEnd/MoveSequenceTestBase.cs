using ChessByUrl.Parser.Orthodox;
using ChessByUrl.Rules.Rulesets.Orthodox;
using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.EndToEnd
{
    public abstract class MoveSequenceTestBase
    {
        protected (Game game, Player white, Player black) CreateGame(int numberOfMovesPerformed)
        {
            var ruleset = new OrthodoxRuleset();
            var boardParser = new StartBoardParser();
            var board = boardParser.Parse(ruleset, "s");
            var white = ruleset.Players.Single(p => p.Id == 0);
            var black = ruleset.Players.Single(p => p.Id == 1);
            Assert.IsNotNull(board);
            Assert.AreEqual(white, board.CurrentPlayer);

            var game = new Game(ruleset, board);
            if (numberOfMovesPerformed == 0) return (game, white, black);

            var moves = MoveSequence.Take(numberOfMovesPerformed).ToList();
            foreach (var move in moves)
            {
                GameAssert.MoveLegal(game, move);
                game = game.ApplyMove(move);
            }
            return (game, white, black);
        }

        protected abstract IEnumerable<Move> MoveSequence { get; }


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