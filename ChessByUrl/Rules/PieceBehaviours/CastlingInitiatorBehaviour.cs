
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
        public CastlingInitiatorBehaviour(int destinationFile)
        {

        }

        public int DestinationFile { get; }

        public IEnumerable<Move> GetLegalMovesFrom(Game game, Coords from, PieceType fromPiece)
        {
            var destination = from.WithFile(DestinationFile);
            var direction = from.File < DestinationFile ? 1 : -1;
            var responder = CastlingHelpers.FindResponder(game, from, direction);
            if (responder == null)

            // Threat checking is taken care of in the filter method, to avoid stack blowout.

            // TODO: Disambiguation
            var result = new Move(from, destination);
            return [result];
        }


        public IEnumerable<Move> FilterLegalMoveCandidates(Game game, Coords thisSquare, PieceType thisPiece, IEnumerable<Move> candidates)
        {
            var result = new List<Move>();
            foreach (var move in candidates)
            {
                if (move.From == thisSquare && IsCastle(move))
                {
                    if (!CastlingHelpers.IsRouteClear(game.CurrentBoard, move.From.Rank, move.From.File, DestinationFile))
                        continue;
                }
                result.Add(move);
            }
            return result;
        }


        public Board ApplyMoveFrom(Game gameBeforeMove, Board boardSoFar, Move move, PieceType fromPiece)
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

        //public Board AdjustBoardAfterMove(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, Coords thisSquare, PieceType thisPiece)
        //{
        //    var responderTransform = ResponderTransform?.Invoke();
        //    if (responderTransform == null)
        //        return boardAfterMoveSoFar;

        //    var initiatorHasMoved = move.To == thisSquare;
        //    var initiatorOriginalSquare = initiatorHasMoved ? move.From : thisSquare;
        //    var responderOriginalSquare = initiatorOriginalSquare.WithFile(ResponderFromFile);
        //    var responderHasMoved = move.From == responderOriginalSquare;
        //    var responderCurrentSquare = responderHasMoved ? move.To : responderOriginalSquare;

        //    // If the initiator or responder (or piece in the responder square) has moved, remove castling rights for the piece in the responder square.
        //    // If it doesn't have castling rights, don't worry about it.
        //    if (initiatorHasMoved || responderHasMoved)
        //    {
        //        if (boardAfterMoveSoFar.GetPiece(responderCurrentSquare)?.Behaviours.OfType<CastlingResponderBehaviour>().Any() ?? false)
        //        {
        //            boardAfterMoveSoFar = boardAfterMoveSoFar.ReplacePiece(responderCurrentSquare, responderTransform);
        //        }
        //    }

        //    return boardAfterMoveSoFar;

        //}

    }
}
