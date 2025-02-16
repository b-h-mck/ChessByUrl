namespace ChessByUrl.Rules.Standard.Pieces
{
    public class StandardPiece : Piece
    {
        public static StandardPiece Create(Player player, StandardPieceType type)
        {
            return new StandardPiece
            {
                Id = Enum.GetValues<StandardPieceType>().Count() * player.Id + (int)type,
                Player = player,
                Type = type,
                Behaviour = GetPieceBehaviour(type),
                Name = GetPieceShortName(type),
                Description = GetPieceDescription(player, type),
                Unicode = GetPieceUnicode(player, type)
            };
        }
        public required StandardPieceType Type { get; init; }
        public required IPieceBehaviour Behaviour { get; init; }


        private static string GetPieceShortName(StandardPieceType pieceType)
        {
            return pieceType switch
            {
                StandardPieceType.PawnWhoJustMovedTwoSquares => "Pawn",
                StandardPieceType.RookWithCastlingRights => "Rook",
                _ => pieceType.ToString()
            };
        }

        private static IPieceBehaviour GetPieceBehaviour(StandardPieceType pieceType)
        {
            return pieceType switch
            {
                StandardPieceType.Pawn or
                StandardPieceType.PawnWhoJustMovedTwoSquares => new PawnBehaviour(),
                StandardPieceType.Knight => new KnightBehaviour(),
                StandardPieceType.Bishop => new SliderBehaviour(diagonal: true),
                StandardPieceType.Rook or
                StandardPieceType.RookWithCastlingRights => new SliderBehaviour(orthogonal: true),
                StandardPieceType.Queen => new SliderBehaviour(orthogonal: true, diagonal: true),
                StandardPieceType.King => new KingBehaviour(),
                _ => throw new ArgumentOutOfRangeException(nameof(pieceType))
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