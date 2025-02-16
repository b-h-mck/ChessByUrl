using ChessByUrl.Rules;

namespace ChessByUrl.Parser
{
    public interface IBoardParser
    {
        string? Serialise(IRuleset ruleset, Board board);
        Board? Parse(IRuleset ruleset, string boardString);
    }
}
