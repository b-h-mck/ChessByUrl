
using ChessByUrl.Rules.PieceBehaviours;
using System.Text;

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

            var promotionPiece = GetPromotionPiece(move.Variant.Value, player);

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

        private PieceType? GetPromotionPiece(int variant, Player player)
        {
            var pieceSet = OrthodoxPieceTypes.Player(player.Id);
            return variant switch
            {
                0 => pieceSet.Queen,
                1 => pieceSet.Rook,
                2 => pieceSet.Bishop,
                3 => pieceSet.Knight,
                _ => null
            };
        }

        public string GetMoveNotation(Board boardBeforeMove, Move move)
        {
            var pieceType = boardBeforeMove.GetPiece(move.From);
            if (pieceType == null)
                return "";
            var pieceSet = OrthodoxPieceTypes.Player(pieceType.Player.Id);

            var sb = new StringBuilder();
            if (pieceType == pieceSet.Pawn || pieceType == pieceSet.PawnVulnerableToEnPassant)
            {
                AppendPawnMove(boardBeforeMove, move, sb);

            }
            else if (pieceType == pieceSet.King && Math.Abs(move.From.File - move.To.File) > 1)
            {
                AppendCastlingMove(boardBeforeMove, move, sb);
            }
            else
            {
                AppendNormalMove(boardBeforeMove, move, sb);
            }

            AppendCheck(boardBeforeMove, move, sb);


            return sb.ToString();

        }


        private void AppendPawnMove(Board boardBeforeMove, Move move, StringBuilder sb)
        {
            sb.Append(move.From.ToString()[0]);
            if (move.From.File != move.To.File)
            {
                sb.Append('x').Append(move.To.ToString()[0]);
            }
            sb.Append(move.To.ToString()[1]);
            if (move.Variant != null)
            {
                var promotionPiece = GetPromotionPiece(move.Variant.Value, boardBeforeMove.CurrentPlayer);
                if (promotionPiece != null)
                {
                    sb.Append('=').Append(promotionPiece.Unicode);
                }
            }
            if (move.From.File != move.To.File && boardBeforeMove.GetPiece(move.To) == null)
            {
                sb.Append(" e.p.");
            }
        }

        private void AppendCastlingMove(Board boardBeforeMove, Move move, StringBuilder sb)
        {
            if (move.From.File - move.To.File == 2)
                sb.Append("O-O-O");
            if (move.From.File - move.To.File == -2)
                sb.Append("O-O");
        }

        private void AppendNormalMove(Board boardBeforeMove, Move move, StringBuilder sb)
        {
            var gameBeforeMove = new Game(this, boardBeforeMove);
            var pieceType = boardBeforeMove.GetPiece(move.From);
            var otherPlayer = this.GetNextPlayer(boardBeforeMove.CurrentPlayer);
            if (pieceType == null || otherPlayer == null)
            {
                return;
            }

            sb.Append(pieceType.Unicode);

            // Find other pieces that could move into this square and disambiguate if needed.
            var otherMovesTargetingSquare = gameBeforeMove.GetThreats(move.To, otherPlayer);
            foreach (var otherMove in otherMovesTargetingSquare)
            {
                if (otherMove.From == move.From)
                    continue;
                var otherPieceType = boardBeforeMove.GetPiece(otherMove.From);
                if (otherPieceType != pieceType)
                    continue;

                if (otherMove.From.File != move.From.File)
                    sb.Append(move.From.ToString()[0]);
                else if (otherMove.From.Rank != move.From.Rank)
                    sb.Append(move.From.ToString()[1]);
                else
                    sb.Append(move.From.ToString());
            }
            sb.Append(move.To.ToString());
        }


        private void AppendCheck(Board boardBeforeMove, Move move, StringBuilder sb)
        {
            var gameAfterMove = new Game(this, boardBeforeMove).ApplyMove(move);
            var kingSquare = gameAfterMove.CurrentBoard.FindSquares(square =>
                square?.Player.Id == gameAfterMove.CurrentPlayer.Id &&
                square.Behaviours.OfType<RoyalBehaviour>().Any()
            ).First();
            var inCheck = gameAfterMove.GetThreats(kingSquare, gameAfterMove.CurrentPlayer).Any();
            if (inCheck)
            {
                if (gameAfterMove.GetLegalMovesForPlayer(gameAfterMove.CurrentPlayer).Any())
                {
                    sb.Append("+");
                }
                else
                {
                    sb.Append("#");
                }
            }
        }
    }
}
