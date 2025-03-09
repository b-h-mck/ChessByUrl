using ChessByUrl.Parser;
using ChessByUrl.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security;

namespace ChessByUrl.Pages
{
    public class PlayModel : PageModel
    {
        public Game? Game { get; set; }


        public class MoveToInfo
        {
            /// <summary>
            /// Destination of the move.
            /// </summary>
            public required string To { get; init; }

            /// <summary>
            /// URL of the move if it doesn't have variants. If it does, this will be null and the URLs will be in Variants.
            /// </summary>
            public string? Url { get; init; }

            /// <summary>
            /// URLs and VariantInfo for each variant, or null if there are no variants.
            /// </summary>
            public List<MoveVariantInfoAndUrl>? Variants { get; init; }
        }

        public class MoveVariantInfoAndUrl
        {
            public required MoveVariantInfo VariantInfo { get; init; }
            public required string Url { get; init; }
        }

        public Dictionary<string, List<MoveToInfo>>? MovesFromSquare { get; set; }

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
            var movesSoFar = parsers.ParseMoves(ruleset, initialBoard, movesString ?? "") ?? Enumerable.Empty<Move>();

            Game = new Game(ruleset, initialBoard).ApplyMoves(movesSoFar);

            // Get the current player's legal moves and generate URLs for them
            var legalMoves = Game.GetLegalMovesForPlayer(Game.CurrentPlayer);
            MovesFromSquare = new Dictionary<string, List<MoveToInfo>>();
            foreach (var move in legalMoves)
            {
                var movesAfter = movesSoFar.Append(move);
                var newMovesString = parsers.SerialiseMoves(ruleset, initialBoard, movesAfter);
                var newUrl = Url.Page("/Play", new { rulesetString, boardString, movesString = newMovesString });
                if (newUrl == null)
                    continue;

                var movesFromSquare = MovesFromSquare.GetValueOrDefault(move.From.ToString());
                if (movesFromSquare == null)
                {
                    movesFromSquare = new List<MoveToInfo>();
                    MovesFromSquare[move.From.ToString()] = movesFromSquare;
                }
                if (move.Variant == null)
                {
                    movesFromSquare.Add(new MoveToInfo { To = move.To.ToString(), Url = newUrl });
                }
                else
                {
                    var variantInfo = ruleset.GetMoveVariant(Game.CurrentBoard, move);

                    if (variantInfo == null)
                        continue;
                    var existingMove = movesFromSquare.FirstOrDefault(m => m.To == move.To.ToString());
                    if (existingMove == null)
                    {
                        existingMove = new MoveToInfo { To = move.To.ToString(), Variants = [] };
                        movesFromSquare.Add(existingMove);
                    }
                    existingMove.Variants!.Add(new MoveVariantInfoAndUrl { VariantInfo = variantInfo, Url = newUrl });
                }
            }
        }

    }
}
