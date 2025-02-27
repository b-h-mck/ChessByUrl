using System.Numerics;

namespace ChessByUrl.Rules
{

    public class Player
    {
        public required int Id { get; init; }
        public required string Name { get; init; }

        /// <summary>
        /// The rank closest to the player's side of the board. 
        /// In orthodox chess this is 0 for White and 7 for Black.
        /// </summary>
        public required int ClosestRank { get; init; }

        /// <summary>
        /// The rank closest to the opponent's side of the board. 
        /// In orthodox chess this is 7 for White and 1 for Black.
        /// </summary>
        public required int FarthestRank { get; init; }

        /// <summary>
        /// The number to add to the rank to get closer to FarthestRank (1 for White, -1 for Black).
        /// </summary>
        public int Direction => ClosestRank < FarthestRank ? 1 : -1;

    }
}