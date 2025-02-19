namespace ChessByUrl.Rules
{

    public class Piece
    {
        public required int Id { get; init;  }
        public required Player Player { get; init; }

        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string Unicode { get; init; }

        public override string ToString() => $"{Id} {Description}";
    }
}
