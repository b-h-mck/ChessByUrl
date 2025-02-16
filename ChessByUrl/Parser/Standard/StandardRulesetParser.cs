using ChessByUrl.Rules;
using ChessByUrl.Rules.Standard;

namespace ChessByUrl.Parser.Standard
{
    public class StandardRulesetParser : IRulesetParser
    {
        public string? Serialise(IRuleset ruleset)
        {
            if (ruleset is StandardRuleset)
                return "s";
            return null;
        }

        public IRuleset? Parse(string rulesetString)
        {
            if (rulesetString == "s")
                return new StandardRuleset();
            return null;
        }

    }
}
