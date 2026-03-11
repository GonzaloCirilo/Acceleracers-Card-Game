using System.Collections.Generic;
using AcceleracersCCG.Commands;
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
            // Reset per-turn flags
            state.ActivePlayer.HasPlayedVehicleThisTurn = false;
            state.ActivePlayer.AP = 0;

            // Swap active player
            state.ActivePlayerIndex = 1 - state.ActivePlayerIndex;

            return new List<ICommand>();
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
