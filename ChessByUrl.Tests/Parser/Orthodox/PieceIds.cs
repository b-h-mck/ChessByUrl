using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Parser.Orthodox
{
    /// <summary>
    /// Contains constants for the piece IDs in the Orthodox ruleset, for use in unit tests.
    /// </summary>
    /// <remarks>
    /// Ideally we'd use e.g. OrthodoxPieceTypes.White.Pawn.Id directly instead of repeating them here,
    /// but the DataRow attributes need compile-time constants and not mere static readonly fields.
    /// </remarks>
    public static class PieceIds
    {
        public static class White
        {
            public const int King = 0;
            public const int Queen = 1;
            public const int Rook = 2;
            public const int RookWithCastlingRights = 3;
            public const int Bishop = 4;
            public const int Knight = 5;
            public const int Pawn = 6;
            public const int PawnVulnerableToEnPassant = 7;
        }

        public static class Black
        {
            public const int King = 8;
            public const int Queen = 9;
            public const int Rook = 10;
            public const int RookWithCastlingRights = 11;
            public const int Bishop = 12;
            public const int Knight = 13;
            public const int Pawn = 14;
            public const int PawnVulnerableToEnPassant = 15;
        }
    }
}
