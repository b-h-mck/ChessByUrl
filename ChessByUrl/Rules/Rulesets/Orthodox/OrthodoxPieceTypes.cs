using ChessByUrl.Rules.PieceBehaviours;

namespace ChessByUrl.Rules.Rulesets.Orthodox
{
    public static class OrthodoxPieceTypes
    {
        public static readonly PieceSet White = new PieceSet(OrthodoxPlayers.White);
        public static readonly PieceSet Black = new PieceSet(OrthodoxPlayers.Black);
        public static IEnumerable<PieceType> All => White.All.Concat(Black.All);
        public static PieceSet Player(int playerId) => playerId == 0 ? White : Black;

        public class PieceSet
        {
            private static string GetSvgFileName(int playerId, string pieceLetter) => $"Chess_{pieceLetter}{(playerId == 0 ? "l" : "d")}t45.svg";
            public PieceSet(Player player)
            {
                var id = player.Id * 8; // Start at 0 for White or 8 for Black
                var pawnStartRank = player.ClosestRank + player.Direction;
                var pawnPrePromotionRank = player.FarthestRank - player.Direction;
                var pawnDoubleMoveRank = pawnStartRank + (2 * player.Direction);
                King = new()
                {
                    Id = id++,
                    Player = player,
                    Name = "King",
                    Description = "King",
                    Unicode = player.Id == 0 ? "♔" : "♚",
                    SvgFileName = GetSvgFileName(player.Id, "k"),
                    Behaviours = new IPieceBehaviour[]
                    {
                        new LeaperBehaviour(1, 1),
                        new LeaperBehaviour(1, 0),
                        new RoyalBehaviour(),
                        new CastlingInitiatorBehaviour(2, 0, 3, () => Rook!),
                        new CastlingInitiatorBehaviour(6, 7, 5, () => Rook!)
                    }
                };
                Queen = new()
                {
                    Id = id++,
                    Player = player,
                    Name = "Queen",
                    Description = "Queen",
                    Unicode = player.Id == 0 ? "♕" : "♛",
                    SvgFileName = GetSvgFileName(player.Id, "q"),
                    Behaviours = new IPieceBehaviour[]
                    {
                        new RiderBehaviour(new LeaperBehaviour(1, 1)),
                        new RiderBehaviour(new LeaperBehaviour(1, 0)),
                    }
                };
                Rook = new()
                {
                    Id = id++,
                    Player = player,
                    Name = "Rook",
                    Description = "Rook",
                    Unicode = player.Id == 0 ? "♖" : "♜",
                    SvgFileName = GetSvgFileName(player.Id, "r"),
                    Behaviours = new IPieceBehaviour[]
                    {
                        new RiderBehaviour(new LeaperBehaviour(1, 0))
                    }
                };
                RookWithCastlingRights = new()
                {
                    Id = id++,
                    Player = player,
                    Name = "Rook with castling rights",
                    Description = "Rook with castling rights",
                    Unicode = player.Id == 0 ? "♖" : "♜",
                    SvgFileName = GetSvgFileName(player.Id, "r"),
                    Behaviours = new IPieceBehaviour[]
                    {
                        new RiderBehaviour(new LeaperBehaviour(1, 0)),
                        new CastlingResponderBehaviour()
                    }
                };
                Bishop = new()
                {
                    Id = id++,
                    Player = player,
                    Name = "Bishop",
                    Description = "Bishop",
                    Unicode = player.Id == 0 ? "♗" : "♝",
                    SvgFileName = GetSvgFileName(player.Id, "b"),
                    Behaviours = new IPieceBehaviour[]
                    {
                        new RiderBehaviour(new LeaperBehaviour(1, 1))
                    }
                };
                Knight = new()
                {
                    Id = id++,
                    Player = player,
                    Name = "Knight",
                    Description = "Knight",
                    Unicode = player.Id == 0 ? "♘" : "♞",
                    SvgFileName = GetSvgFileName(player.Id, "n"),
                    Behaviours = new IPieceBehaviour[]
                    {
                        new LeaperBehaviour(2, 1)
                    }
                };
                Pawn = new()
                {
                    Id = id++,
                    Player = player,
                    Name = "Pawn",
                    Description = "Pawn",
                    Unicode = player.Id == 0 ? "♙" : "♟",
                    SvgFileName = GetSvgFileName(player.Id, "p"),
                    Behaviours = new IPieceBehaviour[]
                    {
                        new PawnStandardMoveBehaviour(),
                        new PawnAttackBehaviour(),
                        new PawnLongMoveBehaviour(pawnStartRank, pawnDoubleMoveRank, () => PawnVulnerableToEnPassant!),
                        new PromotionBehaviour(player.FarthestRank, () => [Queen, Rook, Bishop, Knight]),
                        new EnPassantAttackerBehaviour()
                    }
                };
                PawnVulnerableToEnPassant = new()
                {
                    Id = id++,
                    Player = player,
                    Name = "Pawn vulnerable to en passant",
                    Description = "Pawn vulnerable to en passant",
                    Unicode = player.Id == 0 ? "♙" : "♟",
                    SvgFileName = GetSvgFileName(player.Id, "p"),
                    Behaviours = new IPieceBehaviour[]
                    {
                        new PawnStandardMoveBehaviour(),
                        new PawnAttackBehaviour(),
                        new EnPassantVictimBehaviour(() => Pawn)
                    }
                };

            }



            public PieceType King { get; }
            public PieceType Queen { get; }
            public PieceType Rook { get; }
            public PieceType RookWithCastlingRights { get; }
            public PieceType Bishop { get; }
            public PieceType Knight { get; }
            public PieceType Pawn { get; }
            public PieceType PawnVulnerableToEnPassant { get; }
            public IEnumerable<PieceType> All => new[] { King, Queen, Rook, RookWithCastlingRights, Bishop, Knight, Pawn, PawnVulnerableToEnPassant };
        }
    }
}
