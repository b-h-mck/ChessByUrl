using ChessByUrl.Parser;
using ChessByUrl.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using System.Security;
using System.Text.Encodings.Web;

namespace ChessByUrl.Pages
{
    public class PlayModel : PageModel
    {

        public string? RulesetString { get; set; }
        public string? BoardString { get; set; }
        public string? MovesString { get; set; }

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
            RulesetString = rulesetString;
            BoardString = boardString;
            MovesString = movesString;

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

        public string GetLinkedImageHtml()
        {
            var svgUrl = GetCurrentPositionUrl("svg");
            var playUrl = GetCurrentPositionUrl("play");
            var html = $@"<a href=\x27{playUrl}\x27><img src=\x27{svgUrl}\x27 /></a>";
            return html;
        }

        public string GetLinkedImageMarkdown()
        {
            var svgUrl = GetCurrentPositionUrl("svg");
            var playUrl = GetCurrentPositionUrl("play");
            var markdown = $"[![Current position]({svgUrl})]({playUrl})";
            return markdown;
        }

        public string? GetCurrentPositionUrl(string urlAction)
        {
            urlAction = urlAction.StartsWith("/") ? urlAction : "/" + urlAction;
            var urlParams = new { rulesetString = RulesetString, boardString = BoardString, movesString = MovesString };
            return Url.Page(urlAction, null, urlParams, "https");
        }

        public string? GetRestartGameUrl()
        {
            if (RulesetString == null || BoardString == null)
                return null;
            var urlParams = new { rulesetString = RulesetString, boardString = BoardString, movesString = "" };
            var result = Url.Page("/Play", null, urlParams, "https");
            return result;
        }

        public string? GetRestartFromThisPositionUrl()
        {
            if (RulesetString == null || Game == null)
                return null;
            var newBoardString = ParserCollection.Instance.SerialiseBoard(Game.Ruleset, Game.CurrentBoard);
            var urlParams = new { rulesetString = RulesetString, boardString = newBoardString, movesString = "" };
            return Url.Page("/Play", null, urlParams, "https");
        }


        public class MoveNotation
        {
            public required int MoveNumber { get; init; }
            public required string WhiteHalfMove { get; init; }
            public required string BlackHalfMove { get; init; }
        }

        public IEnumerable<MoveNotation> GetMoveNotations()
        {
            if (Game == null)
                return Enumerable.Empty<MoveNotation>();
            var result = new List<MoveNotation>();
            var currentFullMoveNumber = 1;
            string currentWhiteMove = "...", currentBlackMove = "";
            var currentGame = new Game(Game.Ruleset, Game.InitialBoard);
            var moves = Game.MovesSoFar.ToList();
            for (int i = 0; i < moves.Count; i++)
            {
                var currentMove = Game.Ruleset.GetMoveNotation(currentGame.CurrentBoard, moves[i]);
                if (currentGame.CurrentPlayer.Name == "White")
                {
                    currentWhiteMove = currentMove;
                }
                else
                {
                    currentBlackMove = currentMove;
                }

                if (currentGame.CurrentPlayer.Name == "Black" || i == moves.Count - 1)
                {
                    result.Add(new MoveNotation
                    {
                        MoveNumber = currentFullMoveNumber,
                        WhiteHalfMove = currentWhiteMove,
                        BlackHalfMove = currentBlackMove
                    });
                    currentFullMoveNumber++;
                    currentWhiteMove = currentBlackMove = "";
                }

                currentGame = currentGame.ApplyMove(moves[i]);
            }
            return result;

        }
    }
}
