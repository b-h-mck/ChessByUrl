namespace ChessByUrl.Rules
{
    public record Coords(int Rank, int File)
    {
        public static Coords operator +(Coords a, Coords b) =>
            new Coords(a.Rank + b.Rank, a.File + b.File);

        public override string ToString()
        {
            var rank = (char)('A' + Rank);
            var file = File + 1;
            return $"{rank}{file}";
        }
    }
}
