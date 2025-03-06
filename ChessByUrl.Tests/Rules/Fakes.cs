using ChessByUrl.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Rules
{
    /// <summary>
    /// Some fakes to use in tests.
    /// </summary>
    /// <remarks>
    /// Test situation: 
    /// 
    /// Fake pieces on (1,1), (2,2), (3,3)
    /// Every fake piece threatens (4,4)
    /// But only (1,1) and (2,2) can legally move there ((3,3) is pinned or something).
    /// Applying this move does the default, plus adds a piece at (7,0)
    /// Adjusting the board after the move also adds a piece at (6,0)
    /// </remarks>
    public class Fakes
    {
        public Fakes()
        {
            Ruleset = new FakeRuleset();

            var emptyRank = new BoardRank(Enumerable.Repeat<PieceType?>(null, 8).ToArray());
            var emptyRanks = new BoardRanks(Enumerable.Repeat(emptyRank, 8).ToArray());
            Board = new Board(Ruleset.Players.First(), emptyRanks);
        }


        public FakeRuleset Ruleset { get; set; }
        public Board Board { get; set; }
        public Game Game => new Game(Ruleset, Board);

        /// <summary>
        /// Adds a piece or pieces of the given type to the board (replaces anything already there).
        /// </summary>
        public void AddPieces(PieceType pieceType, params Coords[] coords)
        {
            Ruleset.PieceTypeList.Add(pieceType);
            foreach (var coord in coords)
                Board = Board.ReplacePiece(coord, pieceType);
        }


        /// <summary>
        /// Adds a piece type with the given behaviour.
        /// </summary>
        public PieceType AddPieceType(int playerId, IPieceBehaviour behaviour)
        {
            var pieceType = CreateAndAddPieceType(playerId, [behaviour]);
            return pieceType;
        }

        /// <summary>
        /// Adds a piece type that does nothing. Useful for testing blocking, etc.
        /// </summary>
        public PieceType AddDummyPieceType(int playerId)
        {
            var pieceType = CreateAndAddPieceType(playerId, []);
            return pieceType;
        }

        /// <summary>
        /// Adds a piece type that threatens the given square (no matter where it is).
        /// </summary>
        public PieceType AddAttackingPieceType(int playerId, Coords attackedSquare)
        {
            var behaviour = new AttackSquareBehaviour(attackedSquare);
            var pieceType = CreateAndAddPieceType(playerId, [behaviour]);
            return pieceType;
        }

        private PieceType CreateAndAddPieceType(int playerId, IEnumerable<IPieceBehaviour> behaviours)
        {
            var result = new PieceType
            {
                Id = Ruleset.PieceTypeList.Count,
                Player = Ruleset.PlayerList[playerId],
                Name = "Test",
                Description = "Test",
                Unicode = "T",
                SvgFileName = "test.svg",
                Behaviours = behaviours
            };
            Ruleset.PieceTypeList.Add(result);
            return result;
        }

        private class AttackSquareBehaviour : IGetLegalMovesBehaviour
        {
            public AttackSquareBehaviour(Coords attackedSquare)
            {
                _attackedSquare = attackedSquare;
            }

            private readonly Coords _attackedSquare;

            public IEnumerable<Move> GetLegalMovesFrom(Game game, Coords from, PieceType fromPiece)
            {
                yield return new Move(from, _attackedSquare);
            }
        }

    }

    public class FakeRuleset : IRuleset
    {
        public IEnumerable<Player> Players => PlayerList;
        public List<Player> PlayerList { get; set; } = [
            new Player { Id = 0, Name = "White", ClosestRank = 0, FarthestRank = 8 },
                new Player { Id = 1, Name = "Black", ClosestRank = 8, FarthestRank = 0 }
        ];

        public IEnumerable<PieceType> PieceTypes => PieceTypeList;
        public List<PieceType> PieceTypeList { get; set; } = [];

        public GameStatus GameStatus { get; set; } = new GameStatus { IsFinished = false, StatusStrings = [] };

        public GameStatus GetGameStatus(Game game)
        {
            return GameStatus;
        }

        public Player GetNextPlayer(Player currentPlayer)
        {
            return PlayerList.First(p => p.Id != currentPlayer.Id);
        }

        public bool IsInBounds(Coords coords)
        {
            return coords.Rank >= 0 && coords.Rank < 8 && coords.File >= 0 && coords.File < 8;
        }
    }
}
