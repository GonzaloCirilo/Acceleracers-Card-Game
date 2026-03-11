using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.StateMachine.Phases
{
    /// <summary>
    /// Automatic: recalculate terrain bonuses, tick token countdowns.
    /// Also calculates AP for the upcoming Action phase.
    /// </summary>
    public class TuneUpPhase : IGamePhase
    {
        public GamePhaseId Id => GamePhaseId.TuneUp;
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
            var player = state.ActivePlayer;

            // Calculate AP for the action phase
            player.AP = ActionPointRules.CalculateAP(player);

            return commands;
        }

        public List<ICommand> GetLegalPlayerCommands(GameState state)
        {
            return new List<ICommand>();
        }

        public GamePhaseId GetNextPhase(GameState state)
        {
            return GamePhaseId.Action;
        }
    }
}
