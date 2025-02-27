namespace ChessByUrl.Rules
{
    /// <summary>
    /// Represents a position on the board. Ranks and files are zero-based, so (0,0) is A1 and (7,7) is H8.
    /// </summary>
    public record Coords(int Rank, int File)
    {
        public static implicit operator Coords(string str) =>
            new Coords(str[1] - '1', str.ToUpper()[0] - 'A');

        public static Coords operator +(Coords a, Coords b) =>
            new Coords(a.Rank + b.Rank, a.File + b.File);

        public static Coords operator -(Coords a, Coords b) =>
            new Coords(a.Rank - b.Rank, a.File - b.File);

        public Coords WithRank(int rank) => new Coords(rank, File);
        public Coords WithFile(int file) => new Coords(Rank, file);

        public Coords AddToRank(int addend) => new Coords(Rank + addend, File);
        public Coords AddToFile(int addend) => new Coords(Rank, File + addend);

        public override string ToString()
        {
            //var rank = (char)('A' + Rank);
            //var file = File + 1;
            var rank = Rank + 1;
            var file = (char)('a' + File);
            return $"{file}{rank}";
        }


    }
}
