
using ChessByUrl.Rules.PieceBehaviours;

namespace ChessByUrl.Rules.Rulesets.Orthodox
{
    public class OrthodoxRuleset : IRuleset
    {
        public IEnumerable<Player> Players => OrthodoxPlayers.All;

        public IEnumerable<PieceType> PieceTypes => OrthodoxPieceTypes.All;

        public bool IsInBounds(Coords coords)
        {
            return coords.File >= 0 && coords.File < 8 && coords.Rank >= 0 && coords.Rank < 8;
        }

        public GameStatus GetGameStatus(Board board)
        {
            var currentPlayerPieces = board.FindSquares(square => square?.Player.Id == board.CurrentPlayer.Id);
            var hasLegalMoves = currentPlayerPieces.SelectMany(square => this.GetLegalMoves(board, square)).Any();
            
            var currentPlayerKing = board.FindSquares(square => 
                    square?.Player.Id == board.CurrentPlayer.Id && 
                    square.Behaviours.OfType<RoyalBehaviour>().Any()
                ).First();

            var inCheck = this.GetThreats(board, currentPlayerKing, board.CurrentPlayer).Any();

            IEnumerable<(Player, decimal)>? playerPoints = null;
            var statusStrings = new List<(Player?, string)>();
            statusStrings.Add((board.CurrentPlayer, "to move"));

            if (inCheck && !hasLegalMoves)
            {
                statusStrings.Add((board.CurrentPlayer, "in checkmate"));
                playerPoints = this.Players.Select(player => (player, player.Id == board.CurrentPlayer.Id ? 0m : 1m));
            }
            else if (inCheck)
            {
                statusStrings.Add((board.CurrentPlayer, "in check"));
            }
            else if (!hasLegalMoves)
            {
                statusStrings.Add((null, "stalemate"));
                playerPoints = this.Players.Select(player => (player, 0.5m));
            }

            return new GameStatus
            { 
                IsFinished = !hasLegalMoves,
                PlayerPoints = playerPoints,
                StatusStrings = statusStrings
            };
        }



    }
}
