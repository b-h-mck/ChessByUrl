using ChessByUrl.Rules;

namespace ChessByUrl.Parser
{
    public interface IMovesParser
    {
        string? Serialise(IRuleset ruleset, Board initialBoard, IEnumerable<Move> moves);
        IEnumerable<Move>? Parse(IRuleset ruleset, Board initialBoard, string movesString);
    }
}
