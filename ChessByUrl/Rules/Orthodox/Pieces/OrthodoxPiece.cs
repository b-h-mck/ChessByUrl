namespace ChessByUrl.Rules.Orthodox.Pieces
{
    public class OrthodoxPiece : Piece
    {
        public static OrthodoxPiece Create(Player player, OrthodoxPieceType type)
        {
            return new OrthodoxPiece
            {
                Id = Enum.GetValues<OrthodoxPieceType>().Count() * player.Id + (int)type,
                Player = player,
                Type = type,
                Behaviour = GetPieceBehaviour(type),
                Name = GetPieceShortName(type),
                Description = GetPieceDescription(player, type),
                Unicode = GetPieceUnicode(player, type)
            };
        }
        public required OrthodoxPieceType Type { get; init; }
        public required IPieceBehaviour Behaviour { get; init; }


        private static string GetPieceShortName(OrthodoxPieceType pieceType)
        {
            return pieceType switch
            {
                OrthodoxPieceType.PawnWhoJustMovedTwoSquares => "Pawn",
                OrthodoxPieceType.RookWithCastlingRights => "Rook",
                _ => pieceType.ToString()
            };
        }

        private static IPieceBehaviour GetPieceBehaviour(OrthodoxPieceType pieceType)
        {
            return pieceType switch
            {
                OrthodoxPieceType.Pawn or
                OrthodoxPieceType.PawnWhoJustMovedTwoSquares => new PawnBehaviour(),
                OrthodoxPieceType.Knight => new KnightBehaviour(),
                OrthodoxPieceType.Bishop => new SliderBehaviour(diagonal: true),
                OrthodoxPieceType.Rook or
                OrthodoxPieceType.RookWithCastlingRights => new SliderBehaviour(orthogonal: true),
                OrthodoxPieceType.Queen => new SliderBehaviour(orthogonal: true, diagonal: true),
                OrthodoxPieceType.King => new KingBehaviour(),
                _ => throw new ArgumentOutOfRangeException(nameof(pieceType))
            };
        }

        private static string GetPieceDescription(Player player, OrthodoxPieceType pieceType)
        {
            var pieceTypeString = pieceType switch
            {
                OrthodoxPieceType.PawnWhoJustMovedTwoSquares => "Pawn (moved two squares)",
                OrthodoxPieceType.RookWithCastlingRights => "Rook (has castling rights)",
                _ => pieceType.ToString()
            };
            return $"{player.Name} {pieceTypeString}";
        }

        private static string GetPieceUnicode(Player player, OrthodoxPieceType pieceType)
        {
            return pieceType switch
            {
                OrthodoxPieceType.Pawn or OrthodoxPieceType.PawnWhoJustMovedTwoSquares => player.Id == 0 ? "♙" : "♟",
                OrthodoxPieceType.Rook or OrthodoxPieceType.RookWithCastlingRights => player.Id == 0 ? "♖" : "♜",
                OrthodoxPieceType.Knight => player.Id == 0 ? "♘" : "♞",
                OrthodoxPieceType.Bishop => player.Id == 0 ? "♗" : "♝",
                OrthodoxPieceType.Queen => player.Id == 0 ? "♕" : "♛",
                OrthodoxPieceType.King => player.Id == 0 ? "♔" : "♚",
                _ => throw new ArgumentOutOfRangeException(nameof(pieceType))
            };
        }
    }

}