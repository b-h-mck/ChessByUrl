using ChessByUrl.Parser;
using ChessByUrl.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChessByUrl.Pages
{
    public class SvgModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public SvgModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Game? Game { get; set; }
        public Dictionary<string, string> SvgContents { get; set; } = new();

        public void OnGet(string? rulesetString, string? boardString, string? movesString)
        {
            // Parse the strings and create the Game
            var parsers = ParserCollection.Instance;
            var ruleset = parsers.ParseRuleset(rulesetString ?? "");
            var initialBoard = ruleset != null ? parsers.ParseBoard(ruleset, TrimExtension(boardString)) : null;
            if (ruleset == null || initialBoard == null)
            {
                return;
            }
            var movesSoFar = parsers.ParseMoves(ruleset, initialBoard, TrimExtension(movesString)) ?? Enumerable.Empty<Move>();

            Game = new Game(ruleset, initialBoard).ApplyMoves(movesSoFar);

            // Load SVG contents
            if (Game != null)
            {
                foreach (var pieceType in ruleset.PieceTypes)
                {
                    var svgPath = Path.Combine(_env.WebRootPath, "images", pieceType.SvgFileName);
                    if (System.IO.File.Exists(svgPath))
                    {
                        var svgContent = System.IO.File.ReadAllText(svgPath);
                        SvgContents[pieceType.SvgFileName] = CleanSvgContent(svgContent);
                    }
                }
            }
        }

        private static string CleanSvgContent(string svgContent)
        {
            // Remove XML declaration
            if (svgContent.StartsWith("<?xml"))
            {
                var xmlDeclarationEnd = svgContent.IndexOf("?>") + 2;
                svgContent = svgContent.Substring(xmlDeclarationEnd).TrimStart();
            }

            // Remove DOCTYPE declaration
            if (svgContent.StartsWith("<!DOCTYPE"))
            {
                var doctypeEnd = svgContent.IndexOf(">") + 1;
                svgContent = svgContent.Substring(doctypeEnd).TrimStart();
            }

            return svgContent;
        }

        private static string TrimExtension(string? input)
        {
            if (input == null)
            {
                return "";
            }
            var extensionIndex = input.LastIndexOf('.');
            return extensionIndex == -1 ? input : input.Substring(0, extensionIndex);
        }
    }
}
