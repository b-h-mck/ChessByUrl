
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

        public GameStatus GetGameStatus(Game game)
        {
            //var currentPlayerPieces = game.CurrentBoard.FindSquares(square => square?.Player.Id == board.CurrentPlayer.Id);
            //var hasLegalMoves = currentPlayerPieces.SelectMany(square => game.GetLegalMoves(board, square)).Any();
            bool hasLegalMoves = game.GetLegalMovesForPlayer(game.CurrentPlayer).Any();

            var currentPlayerKing = game.CurrentBoard.FindSquares(square => 
                    square?.Player.Id == game.CurrentPlayer.Id && 
                    square.Behaviours.OfType<RoyalBehaviour>().Any()
                ).First();

            bool inCheck = game.GetThreats(currentPlayerKing, game.CurrentPlayer).Any();

            IEnumerable<(Player, decimal)>? playerPoints = null;
            var statusStrings = new List<(Player?, string)>();
            statusStrings.Add((game.CurrentPlayer, "to move"));

            if (inCheck && !hasLegalMoves)
            {
                statusStrings.Add((game.CurrentPlayer, "in checkmate"));
                playerPoints = this.Players.Select(player => (player, player.Id == game.CurrentPlayer.Id ? 0m : 1m));
            }
            else if (inCheck)
            {
                statusStrings.Add((game.CurrentPlayer, "in check"));
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

        public Player GetNextPlayer(Player currentPlayer)
        {
            return OrthodoxPlayers.All.First(player => player.Id != currentPlayer.Id);
        }

        public MoveVariantInfo? GetMoveVariant(Board boardBeforeMove, Move move)
        {
            var promotionVariant = GetPawnPromotionVariant(boardBeforeMove, move);
            return promotionVariant;
        }

        private MoveVariantInfo? GetPawnPromotionVariant(Board boardBeforeMove, Move move)
        {
            if (move.Variant == null)
                return null;
            var pieceType = boardBeforeMove.GetPiece(move.From);
            if (pieceType == null)
                return null;
            var player = pieceType.Player;
            var pieceSet = OrthodoxPieceTypes.Player(player.Id);
            if (pieceType != pieceSet.Pawn)
                return null;
            if (move.To.Rank != player.FarthestRank)
                return null;

            var promotionPiece = move.Variant switch
            {
                0 => pieceSet.Queen,
                1 => pieceSet.Rook,
                2 => pieceSet.Bishop,
                3 => pieceSet.Knight,
                _ => null
            };

            if (promotionPiece == null)
                return null;

            return new MoveVariantInfo
            {
                VariantNumber = move.Variant.Value,
                VariantName = $"Promote to {promotionPiece.Name}",
                Unicode = promotionPiece.Unicode,
                SvgFileName = promotionPiece.SvgFileName
            };
        }
    }
}
