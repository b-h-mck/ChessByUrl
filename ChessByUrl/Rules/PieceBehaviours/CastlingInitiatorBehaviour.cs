﻿
namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Represents the behaviour of a piece that does the first move in castling.
    /// </summary>
    /// <remarks>
    /// Castling involves moving the initiator (King) to a given file, then moving the responder (Rook) to another given file. 
    /// Castling can only occur if the responder is in its expected file, all squares along the route are empty (except for the 
    /// initiator and responder), and none of the squares along the initiator's route are threatened.
    /// </remarks>
    public class CastlingInitiatorBehaviour : IGetLegalMovesBehaviour, IFilterLegalMoveCandidatesBehaviour, IApplyMoveBehaviour, IAdjustBoardAfterMoveBehaviour
    {
        public CastlingInitiatorBehaviour(int initiatorToFile, int responderFromFile, int responderToFile, Func<PieceType>? responderTransform)
        {
            InitiatorToFile = initiatorToFile;
            ResponderFromFile = responderFromFile;
            ResponderToFile = responderToFile;
            ResponderTransform = responderTransform;
        }

        public int InitiatorToFile { get; }
        public int ResponderFromFile { get; }
        public int ResponderToFile { get; }
        public Func<PieceType>? ResponderTransform { get; }

        public IEnumerable<Move> GetLegalMovesFrom(IRuleset ruleset, Board board, Coords from, PieceType fromPiece)
        {
            var responder = board.GetPiece(from.WithFile(ResponderFromFile));
            if (responder == null || responder.Player.Id != fromPiece.Player.Id || !responder.Behaviours.OfType<CastlingResponderBehaviour>().Any())
            {
                yield break;
            }
            if (!AllSquaresEmptyExceptCastlers(board, from))
                yield break;
            // Threat checking is taken care of in the filter method, to avoid stack blowout.
            yield return new Move { From = from, To = from.WithFile(InitiatorToFile) };
        }

        private bool AllSquaresEmptyExceptCastlers(Board board, Coords from)
        {
            var files = new[] { from.File, InitiatorToFile, ResponderFromFile, ResponderToFile };
            for (int file = files.Min(); file <= files.Max(); file++)
            {
                if (file == from.File || file == ResponderFromFile)
                    continue;
                if (board.GetPiece(from.WithFile(file)) != null)
                    return false;
            }
            return true;
        }

        private bool AnyThreatenedSquaresOnInitiatorRoute(IRuleset ruleset, Board board, Coords from, Player player)
        {
            var files = new[] { from.File, InitiatorToFile };
            for (int file = files.Min(); file <= files.Max(); file++)
            {
                var coords = from.WithFile(file);
                var hasThreat = ruleset.GetThreats(board, coords, player)?.Any() ?? false;
                if (hasThreat)
                    return true;
            }
            return false;
        }

        public IEnumerable<Move> FilterLegalMoveCandidates(IRuleset ruleset, Board board, Coords thisSquare, PieceType thisPiece, IEnumerable<Move> candidates)
        {
            foreach (var move in candidates)
            {
                if (move.From == thisSquare && IsCastle(move))
                {
                    if (AnyThreatenedSquaresOnInitiatorRoute(ruleset, board, thisSquare, thisPiece.Player))
                        continue;
                }
                yield return move;
            }
        }


        public Board ApplyMoveFrom(IRuleset ruleset, Board boardBeforeMove, Board boardSoFar, Move move, PieceType fromPiece)
        {
            if (IsCastle(move))
            {
                var responderFrom = move.From.WithFile(ResponderFromFile);
                var responderStartType = boardSoFar.GetPiece(responderFrom)!;
                var responderEndType = ResponderTransform?.Invoke() ?? responderStartType;
                boardSoFar = boardSoFar.ReplacePiece(responderFrom, null).ReplacePiece(responderFrom.WithFile(ResponderToFile), responderEndType);

            }
            return boardSoFar;
        }


        private bool IsCastle(Move move) => move.From.Rank == move.To.Rank && move.To.File == InitiatorToFile;

        public Board AdjustBoardAfterMove(IRuleset ruleset, Board boardAfterMove, Coords thisSquare, PieceType thisPiece, Move move)
        {
            var responderTransform = ResponderTransform?.Invoke();
            if (responderTransform == null)
                return boardAfterMove;

            var initiatorHasMoved = move.To == thisSquare;
            var initiatorOriginalSquare = initiatorHasMoved ? move.From : thisSquare;
            var responderOriginalSquare = initiatorOriginalSquare.WithFile(ResponderFromFile);
            var responderHasMoved = move.From == responderOriginalSquare;
            var responderCurrentSquare = responderHasMoved ? move.To : responderOriginalSquare;

            // If the initiator or responder (or piece in the responder square) has moved, remove castling rights for the piece in the responder square.
            // If it doesn't have castling rights, don't worry about it.
            if (initiatorHasMoved || responderHasMoved)
            {
                if (boardAfterMove.GetPiece(responderCurrentSquare)?.Behaviours.OfType<CastlingResponderBehaviour>().Any() ?? false)
                {
                    boardAfterMove = boardAfterMove.ReplacePiece(responderCurrentSquare, responderTransform);
                }
            }

            return boardAfterMove;

        }
    }
}
