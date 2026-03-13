using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.StateMachine.Phases
{
    /// <summary>
    /// Automatic: reset per-turn flags and swap active player.
    /// </summary>
    public class EndTurnPhase : IGamePhase
    {
        public GamePhaseId Id => GamePhaseId.EndTurn;
        public bool IsAutomatic => true;

        public void OnEnter(GameState state)
        {
        }

        public void OnExit(GameState state)
        {
        }

        public List<ICommand> GetAutoCommands(GameState state)
        {
            var commands = new List<ICommand>();

            // Reset per-turn flags for the active player
            commands.Add(new EndTurnResetCommand(state.ActivePlayerIndex));

            // Swap active player
            commands.Add(new SetActivePlayerCommand(1 - state.ActivePlayerIndex));

            return commands;
        }

        public List<ICommand> GetLegalPlayerCommands(GameState state)
        {
            return new List<ICommand>();
        }

        public GamePhaseId GetNextPhase(GameState state)
        {
            return GamePhaseId.Draw;
        }
    }
}
