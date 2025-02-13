using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChessByUrl.Pages
{
    public class SvgModel : PageModel
    {
        public string Ruleset { get; set; } = "standard";
        public string State { get; set; } = "default";
        public string Moves { get; set; } = "";

        public void OnGet(string? ruleset, string? state, string? moves)
        {
            Ruleset = ruleset ?? Ruleset;
            State = state ?? State;
            Moves = moves ?? Moves;
        }
    }
}
