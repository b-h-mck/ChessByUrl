namespace ChessByUrl.Rules
{
    public record Move(Coords From, Coords To, int? Variant = null) : IEquatable<Move>
    {

        public override string ToString() => $"{From}-{To}{(Variant == null ? string.Empty : "-" + Variant.ToString())}";

    }
}
