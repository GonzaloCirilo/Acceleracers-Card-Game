using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.StateMachine
{
    /// <summary>
    /// A phase of the game turn. Phases are either automatic (no player input)
    /// or interactive (require player commands).
    /// </summary>
    public interface IGamePhase
    {
        GamePhaseId Id { get; }

        /// <summary>True if this phase requires no player input.</summary>
        bool IsAutomatic { get; }

        /// <summary>Called when the phase machine enters this phase.</summary>
        void OnEnter(GameState state);

        /// <summary>Called when the phase machine exits this phase.</summary>
        void OnExit(GameState state);

        /// <summary>
        /// For automatic phases: returns commands to execute.
        /// </summary>
        List<ICommand> GetAutoCommands(GameState state);

        /// <summary>
        /// For interactive phases: returns all legal commands the active player can issue.
        /// </summary>
        List<ICommand> GetLegalPlayerCommands(GameState state);

        /// <summary>
        /// Returns the next phase ID after this one completes.
        /// </summary>
        GamePhaseId GetNextPhase(GameState state);
    }
}
