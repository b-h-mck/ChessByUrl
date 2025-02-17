using ChessByUrl.Parser;
using ChessByUrl.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security;

namespace ChessByUrl.Pages
{
    public class PlayModel : PageModel
    {
        public IRuleset? Ruleset { get; set; } 
        public Board? InitialBoard { get; set; } 
        public Board? Board { get; set; }
        public IEnumerable<Move>? MovesSoFar { get; set; }

        public class MoveUrl
        {
            public required Move Move { get; init; }
            public required string Url { get; init; }
        }

        public List<MoveUrl>[][]? LegalMoves { get; set; }

        public void OnGet(string? ruleset, string? state, string? moves)
        {
            var parsers = ParserCollection.Instance;
            Ruleset = parsers.ParseRuleset(ruleset ?? "");
            InitialBoard = Ruleset != null ? parsers.ParseBoard(Ruleset, state ?? "") : null;
            MovesSoFar = Ruleset != null && InitialBoard != null ? parsers.ParseMoves(Ruleset, InitialBoard, moves ?? "") : null;

            // TODO: Play the moves onto the board
            Board = InitialBoard;

            if (Ruleset == null || InitialBoard == null || Board == null)
            {
                return;
            }

            LegalMoves = new List<MoveUrl>[8][];

            for (int rank = 0; rank < 8; rank++)
            {
                LegalMoves[rank] = new List<MoveUrl>[8];
                for (int file = 0; file < 8; file++)
                {
                    var from = new Coords(rank, file);
                    if (InitialBoard != null && InitialBoard.GetPiece(from)?.Player == InitialBoard.CurrentPlayer)
                    {
                        var legalMoves = Ruleset.GetLegalMoves(InitialBoard, from);
                        foreach (var move in legalMoves)
                        {
                            var legalMovesForSquare = LegalMoves[from.Rank][from.File] ?? new List<MoveUrl>();
                            LegalMoves[from.Rank][from.File] = legalMovesForSquare;

                            var newBoard = Ruleset.ApplyMove(Board, move);
                            var newBoardString = parsers.SerialiseBoard(Ruleset, newBoard);
                            var url = $"/Play/{ruleset}/{newBoardString}";
                            legalMovesForSquare.Add(new MoveUrl { Move = move, Url = url });
                        }
                    }
                }
            }
        }
    }
}
