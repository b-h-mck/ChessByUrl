using ChessByUrl.Parser;
using ChessByUrl.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChessByUrl.Pages
{
    public class SvgModel : PageModel
    {
        public IRuleset? Ruleset { get; set; } 
        public Board? Board { get; set; } 
        public IEnumerable<Move>? Moves { get; set; } 

        public void OnGet(string? ruleset, string? state, string? moves)
        {
            //var parsers = ParserCollection.Instance;
            //Ruleset = parsers.ParseRuleset(ruleset ?? "");
            //Board = Ruleset != null ? parsers.ParseBoard(Ruleset, state ?? "") : null;
            //Moves = Ruleset != null && Board != null ? parsers.ParseMoves(Ruleset, Board, moves ?? "") : null;
        }
    }
}
