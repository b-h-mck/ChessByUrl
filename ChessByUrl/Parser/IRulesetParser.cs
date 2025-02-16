using ChessByUrl.Rules;

namespace ChessByUrl.Parser
{
    public interface IRulesetParser
    {
        string? Serialise(IRuleset ruleset);
        IRuleset? Parse(string rulesetString);
    }
}
