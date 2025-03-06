using ChessByUrl.Parser;
using ChessByUrl.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security;

namespace ChessByUrl.Pages
{
    public class PlayModel : PageModel
    {
        public Game? Game { get; set; }

        public record MoveUrl (string To, string Url);
        public Dictionary<string, List<MoveUrl>>? LegalMoveUrlsFromSquare { get; set; }

        public void OnGet(string? rulesetString, string? boardString, string? movesString)
        {
            // Parse the strings
            var parsers = ParserCollection.Instance;
            var ruleset = parsers.ParseRuleset(rulesetString ?? "");
            var initialBoard = ruleset != null ? parsers.ParseBoard(ruleset, boardString ?? "") : null;
            if (ruleset == null || initialBoard == null)
            {
                return;
            }
            var moveList = parsers.ParseMoves(ruleset, initialBoard, movesString ?? "") ?? Enumerable.Empty<Move>();

            Game = new Game(ruleset, initialBoard).ApplyMoves(moveList);

            // Get the current player's legal moves and generate URLs for them
            var legalMoves = Game.GetLegalMovesForPlayer(Game.CurrentPlayer);
            LegalMoveUrlsFromSquare = new Dictionary<string, List<MoveUrl>>();
            foreach (var move in legalMoves)
            {
                var newMoveList = moveList.Append(move);
                var newMovesString = parsers.SerialiseMoves(ruleset, initialBoard, newMoveList);
                var newUrl = Url.Page("/Play", new { rulesetString, boardString, movesString = newMovesString });
                if (newUrl == null)
                    continue;

                var existingMovesFromSquare = LegalMoveUrlsFromSquare.GetValueOrDefault(move.From.ToString()) ?? new List<MoveUrl>();
                existingMovesFromSquare.Add(new MoveUrl(move.To.ToString(), newUrl));
                LegalMoveUrlsFromSquare[move.From.ToString()] = existingMovesFromSquare;
            }
        }

    }
}
