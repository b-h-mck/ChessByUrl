namespace ChessByUrl.Rules.Rulesets.Orthodox
{
    public static class OrthodoxPlayers
    {
        public static readonly Player White = new Player { Id = 0, ClosestRank = 0, FarthestRank = 7, Name = "White" };
        public static readonly Player Black = new Player { Id = 1, ClosestRank = 7, FarthestRank = 0, Name = "Black" };

        public static readonly Player[] All = [White, Black];
    }
}
