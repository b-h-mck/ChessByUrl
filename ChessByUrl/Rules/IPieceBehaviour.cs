
namespace ChessByUrl.Rules
{
    /// <summary>
    /// Represent how a piece acts on the board. Each PieceType has a list of behaviours, and they are used as needed to get
    /// the legal moves and apply them to the board.
    /// </summary>
    /// <remarks>
    /// This interface is just to group behaviours together on a PieceType. 
    /// Behaviours themselves will have to inherit one (or more) of the derived interfaces to actually do anything.
    /// </remarks>
    public interface IPieceBehaviour { }

    /// <summary>
    /// Provides a method to get legal moves for a piece on the board.
    /// </summary>
    /// <remarks>
    /// This will be called for each piece on the board for the current player's turn, and the results will be aggregated.
    /// This shouldn't take check into account -- that's the responsibility of the king's FilterLegalMoves.
    /// </remarks>
    public interface IGetLegalMovesBehaviour : IPieceBehaviour
    {
        IEnumerable<Move> GetLegalMovesFrom(Game game, Coords from, PieceType fromPiece);
    }

    /// <summary>
    /// Provides a method to filter/adapt to the full list of all legal move candidates on the board.
    /// </summary>
    /// <remarks>
    /// This will be called for every piece on the board (regardless of player) after aggregating all the legal moves from GetLegalMovesFrom.
    /// 
    /// Most pieces will not include this kind of behaviour, but Kings can use it to filter out moves that would put them in check.
    /// Pawns can also use it to transform moves to the final rank into promotions.
    /// </remarks>
    public interface IFilterLegalMoveCandidatesBehaviour : IPieceBehaviour
    {
        IEnumerable<Move> FilterLegalMoveCandidates(Game game, Coords thisSquare, PieceType thisPiece, IEnumerable<Move> candidates);
    }

    /// <summary>
    /// Provides a method to apply a move on the board, returning a new board.
    /// </summary>
    /// <remarks>
    /// Calling defaultApply will apply the default movement behaviour (i.e. move the piece from move.From to move.To,
    /// and switch to the next player). The result of this can modified or replaced entirely before returning.
    /// 
    /// Pieces only need this behaviour if they have special moves (castling, en passant, promotion) that need to do
    /// more than the default behaviour.
    /// 
    /// If a piece has multiple IApplyMoveBehaviours, they will all be called when the piece moves, with the running result
    /// passed between them as defaultApply. So it's important to check if the move does is applicable to this behaviour, or
    /// otherwise just return defaultApply().
    /// For example, a King's Castling behaviour needs to check if the move is actually a Castle before shuffling round the pieces.
    /// </remarks>
    public interface IApplyMoveBehaviour : IPieceBehaviour
    {
        Board ApplyMoveFrom(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, PieceType fromPiece);
    }

    /// <summary>
    /// Makes any necessary adjustments to the board after a move has been made.
    /// </summary>
    /// <remarks>
    /// This will be called for every piece on the board (regardless of player) after ApplyMoveFrom has been called for the moving piece.
    /// 
    /// This is an opportunity to make adjustments for weird behaviours that don't depend on the current move, such as en passant expiration.
    /// </remarks>
    public interface IAdjustBoardAfterMoveBehaviour : IPieceBehaviour
    {
        Board AdjustBoardAfterMove(Game gameBeforeMove, Board boardAfterMoveSoFar, Move move, Coords thisSquare, PieceType thisPiece);
    }

}
