using System.Collections.ObjectModel;

namespace ChessByUrl.Rules
{
    public class Board
    {
        public Board(Player player, BoardRanks ranks)
        {
            CurrentPlayer = player;
            Ranks = ranks;
        }

        public Player CurrentPlayer { get; private set; }

        public BoardRanks Ranks { get; private set; }

        public Piece? GetPiece(Coords coords)
        {
            return Ranks[coords.Rank][coords.File];
        }

        public Board ReplacePiece(Coords coords, Piece? newPiece)
        {
            List<BoardRank> newRanks = new List<BoardRank>(Ranks.Count);
            for (int rank = 0; rank < Ranks.Count; rank++)
            {
                if (rank == coords.Rank)
                {
                    List<Piece?> newRank = new List<Piece?>(Ranks[rank].Count);
                    for (int file = 0; file < Ranks[rank].Count; file++)
                    {
                        newRank.Add(file == coords.File ? newPiece : Ranks[rank][file]);
                    }
                    newRanks.Add(new BoardRank(newRank));
                }
                else
                {
                    newRanks.Add(Ranks[rank]);
                }
            }
            return new Board(CurrentPlayer, new BoardRanks(newRanks));
        }

        public Board SetCurrentPlayer(Player player)
        {
            return new Board(player, Ranks);
        }
    }

    public class BoardRanks : ReadOnlyCollection<BoardRank>
    {
        public BoardRanks(IList<BoardRank> list) : base(list)
        {
        }
    }

    public class BoardRank : ReadOnlyCollection<Piece?>
    {
        public BoardRank(IList<Piece?> list) : base(list)
        {
        }
    }
}
