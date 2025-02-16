using ChessByUrl.Rules;
using ChessByUrl.Rules.Orthodox;

namespace ChessByUrl.Parser.Orthodox
{
    public class RulesetParser : IRulesetParser
    {
        public string? Serialise(IRuleset ruleset)
        {
            if (ruleset is Ruleset)
                return "s";
            return null;
        }

        public IRuleset? Parse(string rulesetString)
        {
            if (rulesetString == "s")
                return new Ruleset();
            return null;
        }

    }
}
