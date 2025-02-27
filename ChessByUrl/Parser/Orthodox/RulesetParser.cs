using ChessByUrl.Rules;
using ChessByUrl.Rules.Rulesets.Orthodox;

namespace ChessByUrl.Parser.Orthodox
{
    public class RulesetParser : IRulesetParser
    {
        public string? Serialise(IRuleset ruleset)
        {
            if (ruleset is OrthodoxRuleset)
                return "o";
            return null;
        }

        public IRuleset? Parse(string rulesetString)
        {
            if (rulesetString == "o")
                return new OrthodoxRuleset();
            return null;
        }

    }
}
