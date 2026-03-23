using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects.Implementations;
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
            int ap = ActionPointRules.CalculateAP(player);
            commands.Add(new SetAPCommand(state.ActivePlayerIndex, ap));

            // Tick countdown tokens on this player's equipment (applied by opponent's hazards)
            foreach (var stack in player.VehiclesInPlay)
            {
                foreach (var equip in stack.AllEquipment())
                {
                    var tokenKey = $"{TimedDestructionEffect.TokenKey}_{equip.UniqueId}";
                    int remaining = stack.Tokens.Get(tokenKey);
                    if (remaining <= 0) continue;

                    if (remaining == 1)
                    {
                        commands.Add(new SetTokenCommand(state.ActivePlayerIndex, stack.Vehicle.UniqueId, tokenKey, 0));
                        commands.Add(new JunkCardCommand(state.ActivePlayerIndex, stack.Vehicle.UniqueId, equip.UniqueId));
                    }
                    else
                    {
                        commands.Add(new TickTokenCommand(state.ActivePlayerIndex, stack.Vehicle.UniqueId, tokenKey));
                    }
                }
            }

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
