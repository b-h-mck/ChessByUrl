namespace ChessByUrl.Rules.Standard
{
    public enum StandardPieceType
    {
        Pawn,
        /// <summary>
        /// A pawn that has just moved two squares and is vulnerable to en passant. Will become a Pawn on the next turn.
        /// </summary>
        PawnWhoJustMovedTwoSquares,
        Knight,
        Bishop,
        Rook,
        /// <summary>
        /// A rook that could castle, i.e. it hasn't moved and its King hasn't moved. Other castling rules are not taken into account.
        /// </summary>
        RookWithCastlingRights,
        Queen,
        King
    }

    public class StandardPiece : Piece
    {
        public static StandardPiece Create(Player player, StandardPieceType type)
        {
            return new StandardPiece
            {
                Id = Enum.GetValues<StandardPieceType>().Count() * player.Id + (int)type,
                Player = player,
                Type = type,
                Name = GetPieceShortName(type),
                Description = GetPieceDescription(player, type),
                Unicode = GetPieceUnicode(player, type)
            };
        }
        public required StandardPieceType Type { get; init; }


        private static string GetPieceShortName(StandardPieceType pieceType)
        {
            return pieceType switch
            {
                StandardPieceType.PawnWhoJustMovedTwoSquares => "Pawn",
                StandardPieceType.RookWithCastlingRights => "Rook",
                _ => pieceType.ToString()
            };
        }

        private static string GetPieceDescription(Player player, StandardPieceType pieceType)
        {
            var pieceTypeString = pieceType switch
            {
                StandardPieceType.PawnWhoJustMovedTwoSquares => "Pawn (moved two squares)",
                StandardPieceType.RookWithCastlingRights => "Rook (has castling rights)",
                _ => pieceType.ToString()
            };
            return $"{player.Name} {pieceTypeString}";
        }

        private static string GetPieceUnicode(Player player, StandardPieceType pieceType)
        {
            return pieceType switch
            {
                StandardPieceType.Pawn or StandardPieceType.PawnWhoJustMovedTwoSquares => player.Id == 0 ? "♙" : "♟",
                StandardPieceType.Rook or StandardPieceType.RookWithCastlingRights => player.Id == 0 ? "♖" : "♜",
                StandardPieceType.Knight => player.Id == 0 ? "♘" : "♞",
                StandardPieceType.Bishop => player.Id == 0 ? "♗" : "♝",
                StandardPieceType.Queen => player.Id == 0 ? "♕" : "♛",
                StandardPieceType.King => player.Id == 0 ? "♔" : "♚",
                _ => throw new ArgumentOutOfRangeException(nameof(pieceType))
            };
        }
    }

}