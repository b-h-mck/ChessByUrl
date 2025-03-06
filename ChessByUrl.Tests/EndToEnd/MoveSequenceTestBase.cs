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



    }
}