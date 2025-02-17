using ChessByUrl.Rules;
using ChessByUrl.Rules.Orthodox;

namespace ChessByUrl.Parser.Orthodox
{
    public class RulesetParser : IRulesetParser
    {
        public string? Serialise(IRuleset ruleset)
        {
            if (ruleset is Ruleset)
                return "o";
            return null;
        }

        public IRuleset? Parse(string rulesetString)
        {
            if (rulesetString == "o")
                return new Ruleset();
            return null;
        }

    }
}
