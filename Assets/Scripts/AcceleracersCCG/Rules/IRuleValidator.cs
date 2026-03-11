using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Validates whether a specific action is legal given the current game state.
    /// </summary>
    public interface IRuleValidator
    {
        /// <summary>
        /// Returns null/empty if valid, or a reason string if invalid.
        /// </summary>
        string Validate(GameState state);
    }
}
