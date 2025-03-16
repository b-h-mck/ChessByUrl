
namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Represents the behaviour of a piece that does the second move in castling (e.g. Rook with castling rights).
    /// Responders also transform into a different piece (e.g. Rook without castling rights) after doing any move or 
    /// when invoked by the initiator.
    /// </summary>
    public class CastlingResponderBehaviour : IApplyMoveBehaviour
    {
        public CastlingResponderBehaviour(int destinationFile, Func<PieceType>? pieceTransform)
        {
            DestinationFile = destinationFile;
            PieceTransform = pieceTransform;
        }

        public int DestinationFile { get; }
        public Func<PieceType>? PieceTransform { get; }

        public bool CanRespondToCastle(Board board, Coords from)
        {
            return CastlingHelpers.IsRouteClear(board, from.Rank, from.File, DestinationFile);
        }

        /// <summary>
        /// Applies the responder's half of the castling move to the board.
        /// </summary>
        /// <remarks>
        /// This is called by CastlingInitiatorBehaviour.
        /// </remarks>
        public Board ApplyCastleResponse(Game gameBeforeMove, Board boardAfterInitiatorMove, Coords responderCoords)
        {
            var responder = boardAfterInitiatorMove.GetPiece(responderCoords);
            var targetCoords = responderCoords.WithFile(DestinationFile);
            if (responderCoords == targetCoords)
                return boardAfterInitiatorMove;
            if (boardAfterInitiatorMove.GetPiece(targetCoords) != null)
                throw new InvalidOperationException("Cannot castle: target square is occupied.");
            return boardAfterInitiatorMove.ReplacePiece(targetCoords, responder).ReplacePiece(responderCoords, null);
        }

        /// <summary>
        /// Applies the transford (to revoke castling rights). This is called by ApplyMoveFrom, and also from the CastlingInitiatorBehaviour.
        /// </summary>
        public Board ApplyTransform(Board board, Coords coords)
        {
            if (PieceTransform == null)
                return board;
            return board.ReplacePiece(coords, PieceTransform());
        }

        /// <summary>
        /// All this does is apply the transform whenever this piece is moved (to revoke castling rights).
        /// The actual move this piece does during castling is in ApplyCastleResponse.
        /// </summary>
        public Board ApplyMoveFrom(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, PieceType fromPiece)
        {
            return ApplyTransform(boardAfterMoveSoFar, move.To);
        }
    }
}
