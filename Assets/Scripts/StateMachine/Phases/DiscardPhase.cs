using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.StateMachine.Phases
{
    /// <summary>
    /// Conditional interactive: if hand exceeds 7, player must discard down to 7.
    /// If already at/below 7, auto-advances.
    /// </summary>
    public class DiscardPhase : IGamePhase
    {
        public GamePhaseId Id => GamePhaseId.Discard;

        public bool IsAutomatic => false; // Can require player choice

        public void OnEnter(GameState state)
        {
        }

        public void OnExit(GameState state)
        {
        }

        public List<ICommand> GetAutoCommands(GameState state)
        {
            return new List<ICommand>();
        }

        public List<ICommand> GetLegalPlayerCommands(GameState state)
        {
            var commands = new List<ICommand>();
            var player = state.ActivePlayer;
            var playerIdx = state.ActivePlayerIndex;

            if (player.Hand.Count > player.EffectiveMaxHandSize)
            {
                // Must discard — offer each card as a discard option
                foreach (var card in player.Hand.Cards)
                {
                    commands.Add(new DiscardCardCommand(playerIdx, card.UniqueId));
                }
            }
            else
            {
                // At or below limit — can end phase
                commands.Add(new EndPhaseCommand(playerIdx));
            }

            return commands;
        }

        public GamePhaseId GetNextPhase(GameState state)
        {
            // If still over, stay in discard phase
            if (state.ActivePlayer.Hand.Count > state.ActivePlayer.EffectiveMaxHandSize)
                return GamePhaseId.Discard;

            return GamePhaseId.EndTurn;
        }
    }
}
