
namespace ChessByUrl.Rules.PieceBehaviours
{
    /// <summary>
    /// Represents the behaviour of a piece that does the second move in castling.
    /// </summary>
    /// <remarks>
    /// This behaviour does nothing by itself -- all the logic's in CastlingInitiatorBehaviour -- but it's used to
    /// tag pieces that the initiator can treat as a responder.
    /// </remarks>
    public class CastlingResponderBehaviour : IPieceBehaviour
    {
    }
}
