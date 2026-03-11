using System;
using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.StateMachine
{
    /// <summary>
    /// Manages phase transitions and auto-processes automatic phases.
    /// </summary>
    public class GamePhaseMachine
    {
        private readonly Dictionary<GamePhaseId, IGamePhase> _phases = new Dictionary<GamePhaseId, IGamePhase>();
        private readonly CommandProcessor _commandProcessor;

        public IGamePhase CurrentPhase { get; private set; }

        /// <summary>Fired when a phase transition occurs.</summary>
        public event Action<GamePhaseId, GamePhaseId> OnPhaseChanged;

        public GamePhaseMachine(CommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public void RegisterPhase(IGamePhase phase)
        {
            _phases[phase.Id] = phase;
        }

        /// <summary>
        /// Transition to a specific phase. Auto-processes automatic phases
        /// until an interactive phase is reached or the game ends.
        /// </summary>
        public void TransitionTo(GamePhaseId phaseId, GameState state)
        {
            // Exit current phase
            CurrentPhase?.OnExit(state);
            var oldPhaseId = CurrentPhase?.Id ?? GamePhaseId.Setup;

            if (!_phases.TryGetValue(phaseId, out var newPhase))
                throw new InvalidOperationException($"Phase {phaseId} not registered.");

            CurrentPhase = newPhase;
            state.CurrentPhase = phaseId;

            OnPhaseChanged?.Invoke(oldPhaseId, phaseId);

            // Enter new phase
            CurrentPhase.OnEnter(state);

            // If game is over, stop
            if (state.Result != GameResult.InProgress)
                return;

            // Auto-process automatic phases
            if (CurrentPhase.IsAutomatic)
            {
                var commands = CurrentPhase.GetAutoCommands(state);
                foreach (var cmd in commands)
                {
                    _commandProcessor.ExecuteUnchecked(cmd, state);
                }

                // Check if game ended during auto phase
                if (state.Result != GameResult.InProgress)
                    return;

                // Move to next phase
                var nextPhase = CurrentPhase.GetNextPhase(state);
                TransitionTo(nextPhase, state);
            }
        }

        /// <summary>
        /// Called when the active player signals end of current interactive phase.
        /// </summary>
        public void AdvancePhase(GameState state)
        {
            if (CurrentPhase == null) return;
            if (state.Result != GameResult.InProgress) return;

            var nextPhaseId = CurrentPhase.GetNextPhase(state);
            TransitionTo(nextPhaseId, state);
        }

        /// <summary>
        /// Get legal commands for the current interactive phase.
        /// </summary>
        public List<ICommand> GetLegalCommands(GameState state)
        {
            if (CurrentPhase == null || CurrentPhase.IsAutomatic)
                return new List<ICommand>();

            return CurrentPhase.GetLegalPlayerCommands(state);
        }
    }
}
