using System.Collections;
using System.Collections.ObjectModel;
using System.Text;

namespace ChessByUrl.Rules
{
    /// <summary>
    /// Represents the state of the board at a given point in time.
    /// </summary>
    /// <remarks>
    /// This just contains the current player and the pieces on the board.
    /// Other state (e.g. castling and en passant rights) is tracked with the pieces themselves.
    /// </remarks>
    public class Board : IEnumerable<(Coords, PieceType?)>
    {
        public Board(Player player, BoardRanks ranks)
        {
            CurrentPlayer = player;
            Ranks = ranks;
        }

        public Player CurrentPlayer { get; private set; }

        public BoardRanks Ranks { get; private set; }

        public PieceType? GetPiece(Coords coords)
        {
            return Ranks[coords.Rank][coords.File];
        }

        public Board ReplacePiece(Coords coords, PieceType? newPiece)
        {
            List<BoardRank> newRanks = new List<BoardRank>(Ranks.Count);
            for (int rank = 0; rank < Ranks.Count; rank++)
            {
                if (rank == coords.Rank)
                {
                    List<PieceType?> newRank = new List<PieceType?>(Ranks[rank].Count);
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

        public IEnumerable<Coords> FindSquares(Predicate<PieceType?> piecePredicate)
        {
            var result = new List<Coords>();
            for (int rank = 0; rank < Ranks.Count; rank++)
            {
                for (int file = 0; file < Ranks[rank].Count; file++)
                {
                    var piece = Ranks[rank][file];
                    if (piecePredicate(piece))
                        result.Add(new Coords(rank, file));
                }
            }
            return result;
        }

        public IEnumerator<(Coords, PieceType?)> GetEnumerator()
        {
            var result = new List<(Coords, PieceType?)>();
            for (int rank = 0; rank < Ranks.Count; rank++)
            {
                for (int file = 0; file < Ranks[rank].Count; file++)
                {
                    var piece = Ranks[rank][file];
                    if (piece != null)
                    {
                        result.Add((new Coords(rank, file), piece));
                    }
                }
            }
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string ToPieceString()
        {
            var sb = new StringBuilder();
            for (int rank = 0; rank < Ranks.Count; rank++)
            {
                for (int file = 0; file < Ranks[rank].Count; file++)
                {
                    var piece = Ranks[rank][file];
                    sb.Append(piece?.Unicode ?? ".");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

    }

    public class BoardRanks : ReadOnlyCollection<BoardRank>
    {
        public BoardRanks(IList<BoardRank> list) : base(list)
        {
        }
    }

    public class BoardRank : ReadOnlyCollection<PieceType?>
    {
        public BoardRank(IList<PieceType?> list) : base(list)
        {
        }
    }
}
