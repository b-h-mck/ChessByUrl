using ChessByUrl.Rules;
using ChessByUrl.Rules.Rulesets.Orthodox;
using ChessByUrl.Utils;

namespace ChessByUrl.Parser.Orthodox
{
    public class EnumeratedMovesParser : IMovesParser
    {
        

        public string? Serialise(IRuleset ruleset, Board initialBoard, IEnumerable<Move> moves)
        {
            var orthodoxRuleset = ruleset as OrthodoxRuleset;
            if (orthodoxRuleset == null)
            {
                return null;
            }
            var byteWriter = new PackedByteWriter();
            var currentGame = new Game(ruleset, initialBoard);
            foreach (var move in moves)
            {
                // Find the index of the legal move (bail out if it's not legal).
                var legalMoves = currentGame.GetLegalMovesForPlayer(currentGame.CurrentPlayer).ToList();
                var moveIndex = legalMoves.IndexOf(move);
                if (moveIndex == -1)
                {
                    return null;
                }

                // We need to pretend there are at least 2 moves so that it can output at least a bit.
                var numberOfLegalMoves = Math.Max(2, legalMoves.Count);
                byteWriter.Write(moveIndex, 0, numberOfLegalMoves - 1);
                currentGame = currentGame.ApplyMove(move);
            }
            return $"e{byteWriter.ToBase64()}";
        }

        public IEnumerable<Move>? Parse(IRuleset ruleset, Board initialBoard, string movesString)
        {
            var orthodoxRuleset = ruleset as OrthodoxRuleset;
            if (orthodoxRuleset == null)
            {
                return null;
            }
            if (!movesString.StartsWith("e"))
            {
                return null;
            }
            var byteReader = new PackedByteReader(movesString.Substring(1));
            var moves = new List<Move>();
            var finished = false;
            var currentGame = new Game(ruleset, initialBoard);
            while (!finished)
            {
                var legalMoves = currentGame.GetLegalMovesForPlayer(currentGame.CurrentPlayer).ToList();
                if (legalMoves.Count == 0)
                {
                    finished = true;
                    continue;
                }
                var numberOfLegalMoves = Math.Max(2, legalMoves.Count);
                var moveIndex = byteReader.Read(0, legalMoves.Count);
                if (moveIndex == null)
                {
                    finished = true;
                    continue;
                }
                if (moveIndex == -1 || moveIndex >= legalMoves.Count)
                {
                    return null;
                }
                var move = legalMoves[moveIndex.Value];
                moves.Add(move);
                currentGame = currentGame.ApplyMove(move);
            }
            return moves;
        }
    }
}
